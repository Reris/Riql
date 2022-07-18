using System;
using Antlr4.Runtime.Tree;

namespace Riql.Transpiler.Rsql;

public class InvalidConversionException
    : RsqlException
{
    public InvalidConversionException(IParseTree context, Type type, Exception? innerException = null)
        : base(context, $"{context.GetText()} is not convertible to {type.FullName}", innerException)
    {
    }
}