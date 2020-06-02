using System;
using System.Linq;
using CspUtils;
using FluentAssertions;
using NUnit.Framework;
using Superpower;
using Superpower.Model;

namespace CspUtil.Tests
{
    public class CspTokenizerFixture
    {
        private void TestInputThatShouldTokenize(string input, params (string TokenValue, CspTokenType Type)[] expectedTokens)
        {
            var tokenizer = new CspTokenizer();
            var actual = tokenizer.Tokenize(input).ToArray();

            if (expectedTokens.Length == 0)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                var predicates = expectedTokens
                    .Select(exToken => new Action<Token<CspTokenType>>(acToken =>
                        {
                            acToken.Span.ToStringValue().Should().Be(exToken.TokenValue);
                            acToken.Kind.Should().Be(exToken.Type);
                        })
                );

                actual.Should().SatisfyRespectively(predicates);
            }
        }

        private void TestInputThatShouldThrow(string input, Position expectedErrorPosition)
        {
            var tokenizer = new CspTokenizer();
            tokenizer.Invoking(t => t.Tokenize(input))
                .Should()
                .Throw<ParseException>()
                .Where(ex => ex.ErrorPosition.HasValue 
                    && ex.ErrorPosition.Column == expectedErrorPosition.Column
                    && ex.ErrorPosition.Line == expectedErrorPosition.Line
                    && ex.ErrorPosition.Absolute == expectedErrorPosition.Absolute);
        }
        
        [Test]
        public void CanTokenizeEmptyInput()
        {
            TestInputThatShouldTokenize("");
        }
        
        [Test]
        public void CanTokenizeSimplestCsp()
        {
            TestInputThatShouldTokenize("default-src *",
                ("default-src", CspTokenType.DirectiveNameOrSourceExpression),
                (" ", CspTokenType.Whitespace),
                ("*", CspTokenType.DirectiveNameOrSourceExpression));
        }
        
        [Test]
        public void CanTokenizeMultipleDirectives()
        {
            TestInputThatShouldTokenize("default-src mydomain.com; font-src *",
                ("default-src", CspTokenType.DirectiveNameOrSourceExpression),
                (" ", CspTokenType.Whitespace),
                ("mydomain.com", CspTokenType.DirectiveNameOrSourceExpression),
                (";", CspTokenType.Semicolon),
                (" ", CspTokenType.Whitespace),
                ("font-src", CspTokenType.DirectiveNameOrSourceExpression),
                (" ", CspTokenType.Whitespace),
                ("*", CspTokenType.DirectiveNameOrSourceExpression));
        }
        
        [Test]
        public void CanTokenizeQuotedStrings()
        {
            TestInputThatShouldTokenize("default-src 'self' 'unsafe-eval'",
                ("default-src", CspTokenType.DirectiveNameOrSourceExpression),
                (" ", CspTokenType.Whitespace),
                ("'self'", CspTokenType.DirectiveNameOrSourceExpression),
                (" ", CspTokenType.Whitespace),
                ("'unsafe-eval'", CspTokenType.DirectiveNameOrSourceExpression));
        }
        
        [Test]
        public void ThrowsOnTabs()
        {
            TestInputThatShouldThrow("default-src *\tmydomain.com", new Position(13, 1, 14));
        }
        
        [Test]
        public void ThrowsOnNewlines()
        {
            TestInputThatShouldThrow("default-src\r\n*", new Position(11, 1, 12));
        }
    }
}