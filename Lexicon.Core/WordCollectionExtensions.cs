using System;
using System.Collections.Generic;
using System.Linq;
using Lexicon.Common;

namespace Lexicon.Core
{
    public static class WordCollectionExtensions
    {
        public static Word ResolveWord(this ICollection<Word> collection, string value)
        {
            var word = collection.SingleOrDefault(x => x.Value.Equals(value));
            if (word == null)
            {
                word = new Word(value);
                collection.Add(word);
            }
            return word;
        }
    }
}
