using System;
using System.Collections.Generic;

namespace Riql.Transpiler.Rsql;

public static class ValueParser
{
    public delegate ParseResult TryParse(Type type, string str);

    public static readonly Dictionary<Type, TryParse> Parsers = new()
    {
        { typeof(string), (_, s) => ParseResult.Succeed(s) },
        { typeof(int), (_, s) => int.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(uint), (_, s) => uint.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(short), (_, s) => short.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(ushort), (_, s) => ushort.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(long), (_, s) => long.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(ulong), (_, s) => ulong.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(double), (_, s) => double.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(float), (_, s) => float.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(decimal), (_, s) => decimal.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(DateTime), (_, s) => DateTime.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(Guid), (_, s) => Guid.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(DateTimeOffset), (_, s) => DateTimeOffset.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(bool), (_, s) => bool.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(char), (_, s) => char.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(byte), (_, s) => byte.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        { typeof(sbyte), (_, s) => sbyte.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail() },
        {
            typeof(Enum), (t, s) =>
            {
                try
                {
                    var v = Enum.Parse(t, s);
                    return ParseResult.Succeed(v);
                }
                catch
                {
                    return ParseResult.Fail();
                }
            }
        }
    };

    private static object ParseValue(RsqlParser.ValueContext valueContext, Type type, TryParse parser)
    {
        valueContext = valueContext ?? throw new ArgumentNullException(nameof(valueContext));
        type = type ?? throw new ArgumentNullException(nameof(type));
        parser = parser ?? throw new ArgumentNullException(nameof(parser));

        string text;
        if (valueContext.SINGLE_QUOTE() != null || valueContext.DOUBLE_QUOTE() != null)
        {
            var replace = valueContext.DOUBLE_QUOTE() != null ? "\"" : "'";
            var value = valueContext.GetText();
            text = value.Length == 2
                       ? string.Empty
                       : value[1..^1].Replace("\\" + replace, replace);
        }
        else
        {
            text = valueContext.GetText();
        }

        var result = parser(type, text);
        if (!result.Success)
        {
            throw new InvalidConversionException(valueContext, type);
        }

        return result.Value;
    }

    public static List<object> GetValues(Type type, RsqlParser.ArgumentsContext argumentsContext)
    {
        type = type.FindNullableValueType() ?? type ?? throw new ArgumentNullException(nameof(type));
        argumentsContext = argumentsContext ?? throw new ArgumentNullException(nameof(argumentsContext));
        var parserType = type.IsEnum ? typeof(Enum) : type;

        if (!ValueParser.Parsers.TryGetValue(parserType, out var parser))
        {
            throw new UnknownParserException(argumentsContext, type);
        }

        var items = new List<object>();
        foreach (var valueContext in argumentsContext.value())
        {
            var value = ValueParser.ParseValue(valueContext, type, parser);
            items.Add(value);
        }

        return items;
    }

    public static object GetValue(Type type, RsqlParser.ValueContext valueContext)
    {
        type = type ?? throw new ArgumentNullException(nameof(type));
        valueContext = valueContext ?? throw new ArgumentNullException(nameof(valueContext));

        var effectiveType = type.FindNullableValueType() ?? type;
        var parserType = effectiveType.IsEnum ? typeof(Enum) : effectiveType;
        if (!ValueParser.Parsers.TryGetValue(parserType, out var parser))
        {
            throw new UnknownParserException(valueContext, effectiveType);
        }


        var result = ValueParser.ParseValue(valueContext, effectiveType, parser);
        return result;
    }

    public readonly struct ParseResult
    {
        public bool Success { get; }
        public object Value { get; }

        private ParseResult(bool success, object value)
        {
            this.Success = success;
            this.Value = value;
        }

        public static ParseResult Succeed(object value)
        {
            return new ParseResult(true, value);
        }

        public static ParseResult Fail()
        {
            return new ParseResult(false, null!);
        }
    }
}