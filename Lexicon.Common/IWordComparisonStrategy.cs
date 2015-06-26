
namespace Lexicon.Common
{
    public interface IWordComparisonStrategy
    {
        bool IsMatch(Word word1, Word word2);
        bool IsMatch(Word word1, string word2);
        bool IsMatch(string word1, string word2);
    }
}
