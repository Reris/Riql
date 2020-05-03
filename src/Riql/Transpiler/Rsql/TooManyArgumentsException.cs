using System;

namespace Riql.Transpiler.Rsql
{
    public class TooManyArgumentsException
        : RsqlException
    {
        public TooManyArgumentsException(RsqlParser.ComparisonContext context, Exception? innerException = null)
            : base(context, $"Too many arguments: {context.selector().GetText()}", innerException)
        {
        }
    }
}