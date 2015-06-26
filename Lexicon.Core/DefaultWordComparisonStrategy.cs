using Lexicon.Common;

namespace Lexicon.Core
{
    public class DefaultWordComparisonStrategy : IWordComparisonStrategy
    {
        public bool IsMatch(Word word1, Word word2)
        {
            EnsureValid(word1);
            EnsureValid(word2);
            return IsMatch(word1.Value, word2.Value);
        }

        public bool IsMatch(Word word1, string word2)
        {
            EnsureValid(word1);
            EnsureValid(word2);
            return IsMatch(word1.Value, word2);
        }

        public bool IsMatch(string word1, Word word2)
        {
            EnsureValid(word1);
            EnsureValid(word2);
            return IsMatch(word1, word2.Value);
        }

        public bool IsMatch(string word1, string word2)
        {
            EnsureValid(word1);
            EnsureValid(word2);
            return unify(word1).Equals(unify(word2));
        }

        private void EnsureValid(string word)
        {
            Ensure.IsNotNullNorWhiteSpace(word);
        }

        private void EnsureValid(Word word)
        {
            Ensure.IsNotNull(word);
            EnsureValid(word.Value);
        }

        private string unify(string str)
        {
            return str.ToLower().Trim();
        }
    }
}
