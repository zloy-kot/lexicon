using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    [TestFixture]
    public class DefaultWordComparisonStrategyTests
    {
        [Test]
        public void When_two_identical_words_are_passed_IsMatch_returns_true()
        {
            var strategy = new DefaultWordComparisonStrategy();
            string word1 = "word";
            string word2 = "word";

            var res = strategy.IsMatch(word1, word2);
            Assert.IsTrue(res);
        }

        [Test]
        public void When_two_same_but_with_different_cases_words_are_passed_IsMatch_returns_true()
        {
            var strategy = new DefaultWordComparisonStrategy();
            string word1 = "WorD";
            string word2 = "woRd";

            var res = strategy.IsMatch(word1, word2);
            Assert.IsTrue(res);
        }

        [Test]
        public void When_two_different_words_are_passed_IsMatch_returns_false()
        {
            var strategy = new DefaultWordComparisonStrategy();
            string word1 = "word";
            string word2 = "another word";

            var res = strategy.IsMatch(word1, word2);
            Assert.IsFalse(res);
        }

        [Test]
        public void When_two_identical_words_that_start_or_end_with_whitespace_are_passed_IsMatch_returns_true()
        {
            var strategy = new DefaultWordComparisonStrategy();
            string word1 = "word ";
            string word2 = " word";

            var res = strategy.IsMatch(word1, word2);
            Assert.IsTrue(res);
        }
    }
}
