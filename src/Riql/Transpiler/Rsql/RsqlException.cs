using System;
using Antlr4.Runtime.Tree;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class RsqlException : RiqlParserException
    {
        public RsqlException([NotNull] IParseTree context, string message, [CanBeNull] Exception innerException = null)
            : base(context, message, innerException)
        {
        }
    }
}