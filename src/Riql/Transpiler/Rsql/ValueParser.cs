using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public static class ValueParser
    {
        public delegate ParseResult TryParse([NotNull] Type type, [NotNull] string str);

        [NotNull]
        public static readonly Dictionary<Type, TryParse> Parsers = new Dictionary<Type, TryParse>
        {
            {typeof(string), (t, s) => ParseResult.Succeed(s)},
            {typeof(int), (t, s) => int.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(uint), (t, s) => uint.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(short), (t, s) => short.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(ushort), (t, s) => ushort.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(long), (t, s) => long.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(ulong), (t, s) => ulong.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(double), (t, s) => double.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(float), (t, s) => float.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(decimal), (t, s) => decimal.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(DateTime), (t, s) => DateTime.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(Guid), (t, s) => Guid.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(DateTimeOffset), (t, s) => DateTimeOffset.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(bool), (t, s) => bool.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(char), (t, s) => char.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(byte), (t, s) => byte.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
            {typeof(sbyte), (t, s) => sbyte.TryParse(s, out var v) ? ParseResult.Succeed(v) : ParseResult.Fail()},
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

        [NotNull]
        private static object ParseValue([NotNull] RsqlParser.ValueContext valueContext, [NotNull] Type type, [NotNull] TryParse parser)
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
                           : value.Substring(1, value.Length - 2).Replace("\\" + replace, replace);
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

        [NotNull]
        public static List<object> GetValues([NotNull] Type type, [NotNull] RsqlParser.ArgumentsContext argumentsContext)
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


        [NotNull]
        public static object GetValue([NotNull] Type type, [NotNull] RsqlParser.ValueContext valueContext)
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

        public struct ParseResult
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
                return new ParseResult(false, null);
            }
        }
    }
}