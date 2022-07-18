using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Riql.Transpiler.Rsql;

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

    private readonly PropertyAccessor _propertyAccessor;

    public ComparisonBuilder(PropertyAccessor propertyAccessor)
    {
        this._propertyAccessor = propertyAccessor ?? throw new ArgumentNullException(nameof(propertyAccessor));
    }

    public virtual bool IsEquatableType(Type type)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));

        return type.IsEnum || ComparisonBuilder.DefaultEquatableTypes.Contains(type);
    }


    public void EnsureEquatable(Type type, RsqlParser.ComparisonContext context)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var effectiveType = type.FindNullableValueType() ?? type;
        if (!this.IsEquatableType(effectiveType))
        {
            throw new InvalidComparatorException(context);
        }
    }

    public virtual bool IsComparableType(Type type)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));

        return ComparisonBuilder.DefaultComparableTypes.Contains(type);
    }

    public void EnsureComparable(Type type, RsqlParser.ComparisonContext context)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var effectiveType = type.FindNullableValueType() ?? type;
        if (!this.IsComparableType(effectiveType))
        {
            throw new InvalidComparatorException(context);
        }
    }

    public virtual Expression<Func<T, bool>> BuildComparison<T>(
        string comparator,
        ParameterExpression parameter,
        RsqlParser.ComparisonContext context)
    {
        comparator = comparator ?? throw new ArgumentNullException(nameof(comparator));
        parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        context = context ?? throw new ArgumentNullException(nameof(context));

        return comparator switch
        {
            "==" => this.EqualComparator<T>(parameter, context),
            "=eq=" => this.EqualComparator<T>(parameter, context),
            "!=" => this.NotEqualComparator<T>(parameter, context),
            "=neq=" => this.NotEqualComparator<T>(parameter, context),
            "<" => this.LessThanComparator<T>(parameter, context),
            "=lt=" => this.LessThanComparator<T>(parameter, context),
            "<=" => this.LessOrEqualComparator<T>(parameter, context),
            "=le=" => this.LessOrEqualComparator<T>(parameter, context),
            ">" => this.GreaterThanComparator<T>(parameter, context),
            "=gt=" => this.GreaterThanComparator<T>(parameter, context),
            ">=" => this.GreaterOrEqualComparator<T>(parameter, context),
            "=ge=" => this.GreaterOrEqualComparator<T>(parameter, context),
            "=is-null=" => this.IsNullComparator<T>(parameter, context),
            "=nil=" => this.IsNullComparator<T>(parameter, context),
            "=in=" => this.InComparator<T>(parameter, context),
            "=out=" => this.NotInComparator<T>(parameter, context),
            "=nin=" => this.NotInComparator<T>(parameter, context),
            _ => throw new UnknownComparatorException(context)
        };
    }

    private static object GetSingleValue(Type type, RsqlParser.ComparisonContext context)
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

    public static Expression PathCanBeNull(ExpressionPath expressionPath, Expression compare)
    {
        expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
        return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, true, compare, true);
    }

    public static Expression PathNotNull(ExpressionPath expressionPath, Expression compare)
    {
        expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
        return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, false, compare, true);
    }

    public static Expression FullPathNotNull(ExpressionPath expressionPath, Expression compare)
    {
        expressionPath = expressionPath ?? throw new ArgumentNullException(nameof(expressionPath));
        return ComparisonBuilder.CheckNullInPath(expressionPath.ExpressionSteps, false, compare, false);
    }

    private static Expression CheckNullInPath(
        IReadOnlyList<Expression> steps,
        bool canBeNull,
        Expression compare,
        bool ignoreLast)
    {
        steps = steps ?? throw new ArgumentNullException(nameof(steps));
        compare = compare ?? throw new ArgumentNullException(nameof(compare));

        var result = compare;
        var combiner = canBeNull ? (Func<Expression, Expression, Expression>)Expression.OrElse : Expression.AndAlso;
        var comparer = canBeNull ? (Func<Expression, Expression, Expression>)Expression.Equal : Expression.NotEqual;
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

    public static bool CanBeNull(Type type)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));

        return !type.IsValueType || type.FindNullableValueType() != null;
    }

    public Expression<Func<T, bool>> EqualComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

        var escapedLike = ((string)value).Replace(@"\*", ComparisonBuilder.EscapedLikePlaceholder);
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

    public Expression<Func<T, bool>> NotEqualComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
    {
        parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var expression = this.EqualComparator<T>(parameter, context);
        var body = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    public Expression<Func<T, bool>> IsNullComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
    {
        parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
        var value = (bool)ComparisonBuilder.GetSingleValue(typeof(bool), context);

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

    public Expression<Func<T, bool>> LessThanComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

    public Expression<Func<T, bool>> LessOrEqualComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

    public Expression<Func<T, bool>> GreaterThanComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

    public Expression<Func<T, bool>> GreaterOrEqualComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

    public Expression<Func<T, bool>> LikeComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
    {
        parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var expressionPath = this._propertyAccessor.ParsePath(parameter, context.selector().GetText());
        if (expressionPath.Type != typeof(string))
        {
            throw new InvalidComparatorException(context);
        }

        var like = (string)ComparisonBuilder.GetSingleValue(expressionPath.Type, context);
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

    public Expression<Func<T, bool>> InComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
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

    public Expression<Func<T, bool>> NotInComparator<T>(ParameterExpression parameter, RsqlParser.ComparisonContext context)
    {
        parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        context = context ?? throw new ArgumentNullException(nameof(context));

        var expression = this.InComparator<T>(parameter, context);
        var body = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}