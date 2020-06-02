using CspUtils;
using FluentAssertions;
using NUnit.Framework;

namespace CspUtil.Tests
{
    public class CspFixture
    {
        // TODO: any edge cases around default-src?
        // TODO: any edge cases around *?
        
        [Test]
        public void PoliciesCanBeMerged()
        {
            var octopusServerPolicy = new Csp(
                new CspDirective("default-src", "'self'"),
                new CspDirective("script-src", "'self'"),
                new CspDirective("connect-src", "'self'"),
                new CspDirective("font-src", "'none'")
            );

            var octopusDotComPolicy = new Csp(
                new CspDirective("script-src", "octopus.com"),
                new CspDirective("connect-src", "octopus.com"),
                new CspDirective("img-src", "octopus.com")
            );

            var otherServicePolicy = new Csp(
                new CspDirective("script-src", "otherservice.com"),
                new CspDirective("connect-src", "otherservice.com"),
                new CspDirective("font-src", "otherservice.com")
            );

            var actual = octopusServerPolicy
                .Union(octopusDotComPolicy)
                .Union(otherServicePolicy);

            var expected = new Csp(
                new CspDirective("default-src", "'self'"),
                new CspDirective("script-src", "'self'", "octopus.com", "otherservice.com"),
                new CspDirective("connect-src", "'self'", "octopus.com", "otherservice.com"),
                new CspDirective("font-src", "otherservice.com"),
                new CspDirective("img-src", "octopus.com")
            );

            actual.Should().Be(expected);
        }
    }
}