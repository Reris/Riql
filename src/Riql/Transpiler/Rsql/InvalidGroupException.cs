using System;
using Antlr4.Runtime.Tree;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class InvalidGroupException : RsqlException
    {
        public InvalidGroupException([NotNull] IParseTree context, [CanBeNull] Exception innerException = null)
            : base(context, $"Invalid group: {context.GetText()}", innerException)
        {
        }
    }
}