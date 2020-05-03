using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime.Tree;
using Riql.Transpiler.Rsql;

namespace Riql.Transpiler
{
    public class RiqlVisitor<T> : RiqlBaseVisitor<IQueryable<T>>
    {
        [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Debug comparisons")]
        private readonly IQueryable<T> _original;

        private readonly PropertyAccessor _propertyAccessor;
        private IQueryable<T> _current;

        private bool _hasTake;
        private bool _orderingBy;

        public RiqlVisitor(IQueryable<T> queryable, PropertyAccessor propertyAccessor, int maxTake)
        {
            this._propertyAccessor = propertyAccessor ?? throw new ArgumentNullException(nameof(propertyAccessor));
            this._original = queryable ?? throw new ArgumentNullException(nameof(queryable));
            this._current = this._original;
            this.MaxTake = maxTake;
        }

        public int MaxTake { get; }
        protected override IQueryable<T> DefaultResult => this._current;

        public override IQueryable<T> VisitStart(RiqlParser.StartContext context)
        {
            var result = base.VisitStart(context);
            if (!this._hasTake && this.MaxTake >= 0)
            {
                return result.Take(this.MaxTake);
            }

            return result;
        }

        public override IQueryable<T> VisitWhere(RiqlParser.WhereContext context)
        {
            var previous = base.VisitWhere(context);
            var where = context.GetText();

            if (!string.IsNullOrWhiteSpace(where))
            {
                var predicate = Riql.GetRsqlPredicate<T>(where, new ComparisonBuilder(this._propertyAccessor));
                this._current = previous.Where(predicate);
            }

            return this._current;
        }

        public override IQueryable<T> VisitOrderby(RiqlParser.OrderbyContext context)
        {
            var propertyPath = context.propertyPath().GetText();
            var ascending = context.K_DESC() == null;
            var parameter = Expression.Parameter(typeof(T), "i");
            var expressionPath = this._propertyAccessor.ParsePath(parameter, propertyPath);
            var keySelector = Expression.Lambda(expressionPath.Expression, parameter);

            Expression<Func<IQueryable<T>, Expression<Func<T, object>>, bool, IQueryable<T>>> stub = (a, b, c) => this.AddOrderBy(a, b, c);
            var method = ((MethodCallExpression) stub.Body).Method.GetGenericMethodDefinition().MakeGenericMethod(keySelector.ReturnType);
            this._current = (IQueryable<T>) method.Invoke(this, new object[] {this._current, keySelector, ascending});

            try
            {
                this._orderingBy = true;
                return base.VisitOrderby(context);
            }
            finally
            {
                this._orderingBy = true;
            }
        }

        private IQueryable<T> AddOrderBy<TKey>(IQueryable<T> queryable, Expression<Func<T, TKey>> keySelector, bool ascending)
        {
            if (!(queryable is IOrderedQueryable<T> ordered) || !this._orderingBy)
            {
                return ascending
                           ? queryable.OrderBy(keySelector)
                           : queryable.OrderByDescending(keySelector);
            }

            return ascending
                       ? ordered.ThenBy(keySelector)
                       : ordered.ThenByDescending(keySelector);
        }

        public override IQueryable<T> VisitReduce(RiqlParser.ReduceContext context)
        {
            var previous = base.VisitReduce(context);

            var parameter = Expression.Parameter(typeof(T), "i");
            var inits = context.property()
                               .Select(a => this._propertyAccessor.ParsePath(parameter, a.GetText()))
                               .Select(v => (MemberExpression) v.Expression)
                               .Select(m => Expression.Bind(m.Member, m));
            var create = Expression.New(typeof(T));
            var init = Expression.MemberInit(create, inits);

            var reducer = Expression.Lambda<Func<T, T>>(init, parameter);
            this._current = previous.Select(reducer);

            return this._current;
        }


        public override IQueryable<T> VisitSkip(RiqlParser.SkipContext context)
        {
            if (!int.TryParse(context.GetText(), out var skip) || skip < 0)
            {
                throw new RequiresIntegerExeption(context);
            }

            return this._current = this._current.Skip(skip);
        }

        public override IQueryable<T> VisitTake(RiqlParser.TakeContext context)
        {
            if (!int.TryParse(context.GetText(), out var take) || take <= 0)
            {
                throw new RequiresIntegerExeption(context);
            }

            this._hasTake = true;
            return this._current = this._current.Take(this.LimitTake(take));
        }

        private int LimitTake(int take)
        {
            return this.MaxTake <= 0 ? take : Math.Min(take, this.MaxTake);
        }

        public override IQueryable<T> VisitErrorNode(IErrorNode node)
        {
            throw new ErrorNodeException(node);
        }
    }
}