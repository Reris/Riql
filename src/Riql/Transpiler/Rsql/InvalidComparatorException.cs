using System;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class InvalidComparatorException
        : RsqlException
    {
        public InvalidComparatorException([NotNull] RsqlParser.ComparisonContext context, [CanBeNull] Exception innerException = null)
            : base(context, $"Invalid comparator: {context.selector().GetText()}", innerException)
        {
        }
    }
}