using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler
{
    public class ErrorNodeException
        : RiqlParserException
    {
        public ErrorNodeException(IErrorNode node, Exception? innerException = null)
            : base(node, $"Error parsing: {node.ToStringTree()}", innerException)
        {
        }
    }
}