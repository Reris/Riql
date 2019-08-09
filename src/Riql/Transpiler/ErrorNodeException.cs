using System;
using Antlr4.Runtime.Tree;
using JetBrains.Annotations;

namespace Riql.Transpiler
{
    public class ErrorNodeException
        : RiqlParserException
    {
        public ErrorNodeException([NotNull] IErrorNode node, [CanBeNull] Exception innerException = null)
            : base(node, $"Error parsing: {node.ToStringTree()}", innerException)
        {
        }
    }
}