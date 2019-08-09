using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class ComparisonBuilder
    {
        private const string EscapedLikePlaceholder = "\uFFFD";

        private static readonly Type[] DefaultEquatableTypes =
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(bool),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(Guid)
        };

        private static readonly Type[] DefaultComparableTypes =
        {
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(byte),
            typeof(sbyte)
        };

        [NotNull]
        private readonly PropertyAccessor _propertyAccessor;


        public ComparisonBuilder([NotNull] PropertyAccessor propertyAccessor)
        {
            this._propertyAccessor = propertyAccessor ?? throw new ArgumentNullException(nameof(propertyAccessor));
        }

        public virtual bool IsEquatableType([NotNull] Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return type.IsEnum || ComparisonBuilder.DefaultEquatableTypes.Contains(type);
        }


        public void EnsureEquatable([NotNull] Type type, [NotNull] RsqlParser.ComparisonContext context)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var effectiveType = type.FindNullableValueType() ?? type;
            if (!this.IsEquatableType(effectiveType))
            {
                throw new InvalidComparatorException(context);
            }
        }

        public virtual bool IsComparableType([NotNull] Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return ComparisonBuilder.DefaultComparableTypes.Contains(type);
        }

        public void EnsureComparable([NotNull] Type type, [NotNull] RsqlParser.ComparisonContext context)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var effectiveType = type.FindNullableValueType() ?? type;
            if (!this.IsComparableType(effectiveType))
            {
                throw new InvalidComparatorException(context);
            }
        }


        [NotNull]
        public virtual Expression<Func<T, bool>> BuildComparison<T>(
            [NotNull] string comparator,
            [NotNull] ParameterExpression parameter,
            [NotNull] RsqlParser.ComparisonContext context)
        {
            comparator = comparator ?? throw new ArgumentNullException(nameof(comparator));
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            switch (comparator)
            {
                case "==":
                case "=eq=":
                    return this.EqualComparator<T>(parameter, context);
                case "!=":
                case "=neq=":
                    return this.NotEqualComparator<T>(parameter, context);
                case "<":
                case "=lt=":
                    return this.LessThanComparator<T>(parameter, context);
                case "<=":
                case "=le=":
                    return this.LessOrEqualComparator<T>(parameter, context);
                case ">":
                case "=gt=":
                    return this.GreaterThanComparator<T>(parameter, context);
                case ">=":
                case "=ge=":
                    return this.GreaterOrEqualComparator<T>(parameter, context);
                case "=is-null=":
                case "=nil=":
                    return this.IsNullComparator<T>(parameter, context);
                case "=in=":
                    return this.InComparator<T>(parameter, context);
                case "=out=":
                case "=nin=":
                    return this.NotInComparator<T>(parameter, context);
                default:
                    throw new UnknownComparatorException(context);
            }
        }

        [NotNull]
        private static object GetSingleValue([NotNull] Type type, [NotNull] RsqlParser.ComparisonContext context)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var value = context.arguments().value();
            if (value.Length > 1)
            {
                throw new TooManyArgumentsException(context);
            }

            return ValueParser.GetValue(type, value.First());
        }

        [NotNull]
        public static Expression PathCanBeNull([NotNull] ExpressionPath expressionPath, [NotNull] Expression compare)
        {
            expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
            return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, true, compare, true);
        }

        [NotNull]
        public static Expression PathNotNull([NotNull] ExpressionPath expressionPath, [NotNull] Expression compare)
        {
            expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
            return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, false, compare, true);
        }

        [NotNull]
        public static Expression FullPathNotNull([NotNull] ExpressionPath expressionPath, [NotNull] Expression compare)
        {
            expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
            return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, false, compare, false);
        }

        [NotNull]
        private static Expression CheckNullInPath(
            [NotNull] IReadOnlyList<Expression> steps,
            bool canBeNull,
            [NotNull] Expression compare,
            bool ignoreLast)
        {
            steps = steps ?? throw new ArgumentNullException(nameof(steps));
            compare = compare ?? throw new ArgumentNullException(nameof(compare));

            var result = compare;
            var combiner = canBeNull ? (Func<Expression, Expression, Expression>) Expression.OrElse : Expression.AndAlso;
            var comparer = canBeNull ? (Func<Expression, Expression, Expression>) Expression.Equal : Expression.NotEqual;
            var until = ignoreLast ? steps.Count - 1 : steps.Count;
            for (var i = until - 1; i >= 0; i--)
            {
                var property = steps[i];
                if (ComparisonBuilder.CanBeNull(property.Type))
                {
                    result = combiner(
                        comparer(
                            property,
                            Expression.Constant(null, typeof(object))),
                        result);
                }
            }

            return result;
        }

        public static bool CanBeNull([NotNull] Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return !type.IsValueType || type.FindNullableValueType() != null;
        }

        [NotNull]
        public Expression<Func<T, bool>> EqualComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureEquatable(expressionPath.Type, context);

            var value = ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            if (expressionPath.Type != typeof(string))
            {
                return Expression.Lambda<Func<T, bool>>(
                    ComparisonBuilder.PathNotNull(
                        expressionPath,
                        Expression.Equal(
                            expressionPath.Expression,
                            Expression.Constant(value, expressionPath.Type)
                        )),
                    parameter);
            }

            var escapedLike = ((string) value).Replace(@"\*", ComparisonBuilder.EscapedLikePlaceholder);
            if (escapedLike.Contains('*'))
            {
                return this.LikeComparator<T>(parameter, context);
            }

            escapedLike = escapedLike.Replace(ComparisonBuilder.EscapedLikePlaceholder, "*");
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.Equal(
                        expressionPath.Expression,
                        Expression.Constant(escapedLike, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> NotEqualComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expression = this.EqualComparator<T>(parameter, context);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> IsNullComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            var value = (bool) ComparisonBuilder.GetSingleValue(typeof(bool), context);

            var lastNullableExpression = expressionPath.ExpressionSteps.LastOrDefault(e => ComparisonBuilder.CanBeNull(e.Type));
            if (lastNullableExpression == null)
            {
                // Can never be null
                return Expression.Lambda<Func<T, bool>>(Expression.Constant(!value), parameter);
            }

            var result = Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathCanBeNull(
                    expressionPath,
                    Expression.Equal(
                        lastNullableExpression,
                        Expression.Constant(null, typeof(object)))),
                parameter);
            if (value)
            {
                return result;
            }

            var body = Expression.Not(result.Body);
            result = Expression.Lambda<Func<T, bool>>(body, parameter);
            return result;
        }

        [NotNull]
        public Expression<Func<T, bool>> LessThanComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureComparable(expressionPath.Type, context);

            var value = ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.LessThan(
                        expressionPath.Expression,
                        Expression.Constant(value, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> LessOrEqualComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureComparable(expressionPath.Type, context);

            var value = ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.LessThanOrEqual(
                        expressionPath.Expression,
                        Expression.Constant(value, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> GreaterThanComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureComparable(expressionPath.Type, context);

            var value = ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.GreaterThan(
                        expressionPath.Expression,
                        Expression.Constant(value, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> GreaterOrEqualComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureComparable(expressionPath.Type, context);

            var value = ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.GreaterThanOrEqual(
                        expressionPath.Expression,
                        Expression.Constant(value, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> LikeComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            if (expressionPath.Type != typeof(string))
            {
                throw new InvalidComparatorException(context);
            }

            var like = (string) ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
            like = like.Replace(@"\*", ComparisonBuilder.EscapedLikePlaceholder);
            MethodInfo method;
            var startsWild = like.StartsWith("*");
            var endsWild = like.EndsWith("*");
            if (!startsWild && endsWild)
            {
                method = ContainsHelper.StringStartsWithMethod;
            }
            else if (startsWild && !endsWild)
            {
                method = ContainsHelper.StringEndsWithMethod;
            }
            else
            {
                method = ContainsHelper.StringContainsMethod;
            }

            like = like.Replace("*", string.Empty).Replace(ComparisonBuilder.EscapedLikePlaceholder, "*");
            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.FullPathNotNull(
                    expressionPath,
                    Expression.Call(
                        expressionPath.Expression,
                        method,
                        Expression.Constant(like, expressionPath.Type))),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> InComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
            this.EnsureEquatable(expressionPath.Type, context);

            var values = ValueParser.GetValues(expressionPath.Type, context.arguments());
            var methodContainsInfo = ContainsHelper.GetOrRegistryContainsMethodInfo(expressionPath.Type);

            return Expression.Lambda<Func<T, bool>>(
                ComparisonBuilder.PathNotNull(
                    expressionPath,
                    Expression.Call(
                        Expression.Constant(methodContainsInfo.Convert(values)),
                        methodContainsInfo.ContainsMethod,
                        expressionPath.Expression)),
                parameter);
        }

        [NotNull]
        public Expression<Func<T, bool>> NotInComparator<T>([NotNull] ParameterExpression parameter, [NotNull] RsqlParser.ComparisonContext context)
        {
            parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            context = context ?? throw new ArgumentNullException(nameof(context));

            var expression = this.InComparator<T>(parameter, context);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}