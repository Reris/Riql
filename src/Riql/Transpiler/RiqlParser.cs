namespace Riql.Transpiler
{
    public partial class RiqlParser
    {
        private void UseFullText(bool use)
        {
            var lexer = (RiqlLexer) this.TokenStream.TokenSource;
            lexer.UseFullText = use;
        }
    }
}