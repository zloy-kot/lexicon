using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexicon.Common;

namespace Lexicon.Core.Tests
{
    public static class VocabularyInternalCacheExtensions
    {
        public static IList<string> ForeignsFor(this IList<WordPair> thisWordPairs, string native)
        {
            return thisWordPairs.Where(x => x.NativeWord.Value.Equals(native)).Select(x => x.ForeignWord.Value).ToList();
        }

        public static IList<string> AllValues(this IList<Word> thisWords)
        {
            return thisWords.Select(x => x.Value).ToList();
        }

        public static IList<string> AllValues(this IList<Word> thisWords, params string[] words)
        {
            return thisWords.Where(x => words.Contains(x.Value)).Select(x => x.Value).ToList();
        }
    }
}
