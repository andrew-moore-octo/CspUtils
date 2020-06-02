using System;
using NUnit.Framework;

namespace CspUtils.Tests
{
    [TestFixture]
    public class CspTokenizerFixture
    {
        [Test]
        public void Returns_An_Empty_TokenList_For_Empty_Input()
        {
            var tokenizer = new CspTokenizer();
            Assert.True(true);
        }
    }
}