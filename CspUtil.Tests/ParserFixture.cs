using CspUtils;
using FluentAssertions;
using NUnit.Framework;

namespace CspUtil.Tests
{
    public class Parse
    {
        [Test]
        public void CanParseASimplePolicy()
        {
            var actual = Parser.Parse("script-src octopus.com");
            var expected = new Csp(
                new CspDirective("script-src", "octopus.com")
            );

            actual.Should().Be(expected);
        }
        
        [Test]
        public void CanParseAComplexPolicy()
        {
            var actual = Parser.Parse("default-src 'self'; script-src 'self' octopus.com; connect-src octopus.com");
            var expected = new Csp(
                new CspDirective("default-src", "'self'"),
                new CspDirective("script-src", "'self'", "octopus.com"),
                new CspDirective("connect-src", "octopus.com")
            );

            actual.Should().Be(expected);
        }
    }
}