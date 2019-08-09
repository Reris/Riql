using System;
using Antlr4.Runtime.Tree;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public class RiqlParserException : Exception
    {
        public RiqlParserException([NotNull] IParseTree context, string message, [CanBeNull] Exception innerException = null)
            : base(message, innerException)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [NotNull]
        public IParseTree Context { get; }
    }
}