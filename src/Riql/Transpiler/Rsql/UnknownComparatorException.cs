using System;

namespace Riql.Transpiler.Rsql
{
    public class UnknownComparatorException
        : RsqlException
    {
        public UnknownComparatorException(RsqlParser.ComparisonContext context, Exception? innerException = null)
            : base(context, $"Unknown comparator: {context.comparator().GetText()}", innerException)
        {
        }
    }
}