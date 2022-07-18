using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler.Rsql;

public class InvalidGroupException : RsqlException
{
    public InvalidGroupException(IParseTree context, Exception? innerException = null)
        : base(context, $"Invalid group: {context.GetText()}", innerException)
    {
    }
}