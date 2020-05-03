using System;

namespace Riql.Transpiler.Rsql
{
    public class InvalidConversionException
        : RsqlException
    {
        public InvalidConversionException(RsqlParser.ValueContext context, Type type, Exception? innerException = null)
            : base(context, $"{context.GetText()} is not convertible to {type.FullName}", innerException)
        {
        }
    }
}