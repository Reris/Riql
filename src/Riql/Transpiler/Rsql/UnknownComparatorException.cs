using System;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class UnknownComparatorException
        : RsqlException
    {
        public UnknownComparatorException([NotNull] RsqlParser.ComparisonContext context, [CanBeNull] Exception innerException = null)
            : base(context, $"Unknown comparator: {context.comparator().GetText()}", innerException)
        {
        }
    }
}