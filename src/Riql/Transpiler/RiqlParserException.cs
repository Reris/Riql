using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler
{
    public class RiqlParserException : Exception
    {
        public RiqlParserException(IParseTree context, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IParseTree Context { get; }
    }
}