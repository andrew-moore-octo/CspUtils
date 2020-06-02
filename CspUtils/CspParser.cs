using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace CspUtils
{
    public class CspParser
    {
        private static readonly TokenListParser<CspTokenType, string> DirectiveNameParser =
            from name in Token.EqualTo(CspTokenType.DirectiveNameOrSourceExpression)
            select name.ToStringValue();

        static readonly TokenListParser<CspTokenType, string> SourceExpressionParser =
            from expr in Token.EqualTo(CspTokenType.DirectiveNameOrSourceExpression)
            select expr.ToStringValue();

        private static readonly TokenListParser<CspTokenType, Token<CspTokenType>> SemicolonParser =
            from semicolon in Token.EqualTo(CspTokenType.Semicolon)
            from ws in WhitespaceParser.Optional()
            select semicolon;

        private static readonly TokenListParser<CspTokenType, Token<CspTokenType>> WhitespaceParser =
            Token.EqualTo(CspTokenType.Whitespace);

        private static readonly TokenListParser<CspTokenType, string> SourceExpressionWithWhitespaceParser =
            from ws in WhitespaceParser
            from expression in SourceExpressionParser
            select expression;
            
        private static readonly TokenListParser<CspTokenType, CspDirective> DirectiveParser =
            from directiveName in DirectiveNameParser
            from sourceList in SourceExpressionWithWhitespaceParser.AtLeastOnce()
            select new CspDirective(
                directiveName,
                sourceList
            );

        private static readonly TokenListParser<CspTokenType, Csp> RootParser =
            from directives in DirectiveParser.ManyDelimitedBy(SemicolonParser)
            select new Csp(directives);

        public TokenListParserResult<CspTokenType, Csp> Parse(TokenList<CspTokenType> tokens)
        {
            return RootParser(tokens);
        }
        
        public Csp ParseOrThrow(TokenList<CspTokenType> tokens)
        {
            return RootParser.Parse(tokens);
        }
    }
}