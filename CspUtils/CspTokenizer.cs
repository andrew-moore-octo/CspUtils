using System.Collections.Generic;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace CspUtils
{
    public class CspTokenizer : Tokenizer<CspTokenType>
    {
        private static readonly TextParser<TextSpan> DirectiveNameOrSourceExpressionParser = Span.WithoutAny(c => c == ';' || char.IsWhiteSpace(c));
        private static readonly TextParser<TextSpan> WhitespaceParser = Span.WithoutAny(c => !char.IsWhiteSpace(c));
        
        protected override IEnumerable<Result<CspTokenType>> Tokenize(TextSpan remainder, TokenizationState<CspTokenType> state)
        {
            while (true)
            {
                var next = remainder.ConsumeChar();
                if (!next.HasValue)
                    yield break;

                if (next.Value == ';')
                {
                    yield return Result.Value(
                        CspTokenType.Semicolon,
                        next.Location,
                        next.Remainder
                    );
                    remainder = next.Remainder;
                }
                else if (next.Value == ' ')
                {
                    var consumed = WhitespaceParser(remainder);
                    yield return Result.Value(
                        CspTokenType.Whitespace,
                        consumed.Location,
                        consumed.Remainder
                    );
                    remainder = consumed.Remainder;
                }
                else if (IsValidFirstCharForDirectiveNameOrSourceExpression(next.Value))
                {
                    var consumed = DirectiveNameOrSourceExpressionParser(remainder);
                    yield return Result.Value(
                        CspTokenType.DirectiveNameOrSourceExpression,
                        consumed.Location,
                        consumed.Remainder
                    );
                    remainder = consumed.Remainder;
                }
                else
                {
                    // Couldn't parse.
                    yield return Result.Empty<CspTokenType>(remainder);
                }
            }
        }

        public bool IsValidFirstCharForDirectiveNameOrSourceExpression(char c)
        {
            // Filter out newlines, tabs or anything else that might be a problem.
            return !char.IsWhiteSpace(c) && c != ';';
        }
    }
}