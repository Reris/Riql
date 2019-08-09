using System;
using JetBrains.Annotations;

namespace Riql.Transpiler.Rsql
{
    public class InvalidConversionException
        : RsqlException
    {
        public InvalidConversionException([NotNull] RsqlParser.ValueContext context, [NotNull] Type type, [CanBeNull] Exception innerException = null)
            : base(context, $"{context.GetText()} is not convertible to {type.FullName}", innerException)
        {
        }
    }
}