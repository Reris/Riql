using System;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class TooManyArgumentsException
        : RsqlException
    {
        public TooManyArgumentsException([NotNull] RsqlParser.ComparisonContext context, [CanBeNull] Exception innerException = null)
            : base(context, $"Too many arguments: {context.selector().GetText()}", innerException)
        {
        }
    }
}