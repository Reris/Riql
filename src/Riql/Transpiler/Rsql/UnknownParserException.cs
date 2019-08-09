using System;
using Antlr4.Runtime;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class UnknownParserException
        : RsqlException
    {
        public UnknownParserException([NotNull] ParserRuleContext context, [NotNull] Type type, [CanBeNull] Exception innerException = null)
            : base(context, $"No known parser for type '{type.Name}'.", innerException)
        {
        }
    }
}