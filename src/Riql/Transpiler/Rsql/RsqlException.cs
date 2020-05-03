using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler.Rsql
{
    public class RsqlException : RiqlParserException
    {
        public RsqlException(IParseTree context, string message, Exception? innerException = null)
            : base(context, message, innerException)
        {
        }
    }
}