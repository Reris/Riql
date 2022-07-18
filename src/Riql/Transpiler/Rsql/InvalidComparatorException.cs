using System;

namespace Riql.Transpiler.Rsql;

public class InvalidComparatorException
    : RsqlException
{
    public InvalidComparatorException(RsqlParser.ComparisonContext context, Exception? innerException = null)
        : base(context, $"Invalid comparator: {context.selector().GetText()}", innerException)
    {
    }
}