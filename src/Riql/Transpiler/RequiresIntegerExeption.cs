using Antlr4.Runtime.Tree;

namespace Riql.Transpiler
{
    public class RequiresIntegerExeption
        : RiqlParserException
    {
        public RequiresIntegerExeption(IParseTree context)
            : base(context, $"Requires an integer: {context.GetText()}")
        {
        }
    }
}