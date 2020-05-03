using System;
using Antlr4.Runtime;

namespace Riql.Transpiler.Rsql
{
    public class UnknownParserException
        : RsqlException
    {
        public UnknownParserException(ParserRuleContext context, Type type, Exception? innerException = null)
            : base(context, $"No known parser for type '{type.Name}'.", innerException)
        {
        }
    }
}