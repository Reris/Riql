using System;
using System.Linq.Expressions;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler.Rsql
{
    public class RsqlVisitor<T> : RsqlBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly ComparisonBuilder _expressionHelper;
        private readonly ParameterExpression _parameter;

        public RsqlVisitor(ComparisonBuilder expressionHelper)
        {
            this._expressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
            this._parameter = Expression.Parameter(typeof(T), "x");
        }

        public override Expression<Func<T, bool>> VisitStart(RsqlParser.StartContext context)
        {
            return context.or().Accept(this);
        }

        public override Expression<Func<T, bool>> VisitOr(RsqlParser.OrContext context)
        {
            var right = context.and()[0].Accept(this);
            if (context.and().Length == 1)
            {
                return right;
            }

            for (var i = 1; i < context.and().Length; i++)
            {
                var left = context.and()[i].Accept(this);
                right = Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, right.Body), left.Parameters);
            }

            return right;
        }

        public override Expression<Func<T, bool>> VisitAnd(RsqlParser.AndContext context)
        {
            var right = context.constraint()[0].Accept(this);
            if (context.constraint().Length == 1)
            {
                return right;
            }

            for (var i = 1; i < context.constraint().Length; i++)
            {
                var left = context.constraint()[i].Accept(this);
                right = Expression.Lambda<Func<T, bool>>(Expression.And(left.Body, right.Body), left.Parameters);
            }

            return right;
        }

        public override Expression<Func<T, bool>> VisitConstraint(RsqlParser.ConstraintContext context)
        {
            if (context.group() != null)
            {
                return context.group().Accept(this);
            }

            return context.comparison()?.Accept(this)!;
        }


        public override Expression<Func<T, bool>> VisitGroup(RsqlParser.GroupContext context)
        {
            if (context.GROUP_START().Symbol.TokenIndex != -1 && context.GROUP_END().Symbol.TokenIndex != -1)
            {
                return context.or()?.Accept(this)!;
            }

            throw new InvalidGroupException(context);
        }

        public override Expression<Func<T, bool>> VisitErrorNode(IErrorNode node)
        {
            throw new ErrorNodeException(node);
        }

        public override Expression<Func<T, bool>> VisitComparison(RsqlParser.ComparisonContext context)
        {
            var comparator = context.comparator().GetText().ToLowerInvariant();
            return this._expressionHelper.BuildComparison<T>(comparator, this._parameter, context);
        }
    }
}