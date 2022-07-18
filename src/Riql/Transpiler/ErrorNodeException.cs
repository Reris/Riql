using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler;

public class ErrorNodeException
    : RiqlParserException
{
    public ErrorNodeException(IParseTree node, Exception? innerException = null)
        : base(node, $"Error parsing: {node.ToStringTree()}", innerException)
    {
    }
}