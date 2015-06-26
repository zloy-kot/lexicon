using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexicon.Common;

namespace Lexicon.Core
{
    public class VocabularyManager : IVocabularyManager
    {
        private readonly IWordComparisonStrategy _wordComparisonStrategy;
        private readonly IVocabularyRepository _vocabularyRepository;
        private readonly IImporter[] _importers;

        public VocabularyManager(IVocabularyRepository vocabularyRepository, IWordComparisonStrategy wordComparisonStrategy, params IImporter[] importers)
        {
            _wordComparisonStrategy = Ensure.IsNotNull(wordComparisonStrategy);
            _vocabularyRepository = Ensure.IsNotNull(vocabularyRepository);
            _importers = Ensure.IsNotNull(importers);

            WordPairs = new List<WordPair>();
            NativeWords = new List<Word>();
            ForeignWords = new List<Word>();
        }

        internal IList<WordPair> WordPairs { get; private set; }

        internal IList<Word> NativeWords { get; private set; }

        internal IList<Word> ForeignWords { get; private set; }

        public void CreateWord(WordDefinition wordDefinition)
        {
            foreach (var tran in wordDefinition.Translations)
            {
                var native = resolveWord(NativeWords, wordDefinition.NativeWord);
                var foreign = resolveWord(ForeignWords, tran);
                WordPairs.Add(new WordPair(native, foreign));
            }
        }

        public IList<WordPair> GetWordPairs(string word)
        {
            return WordPairs.Where(x => _wordComparisonStrategy.IsMatch(x.NativeWord, word)).ToList();
        }

        private Word resolveWord(ICollection<Word> collection, string value)
        {
            var word = collection.SingleOrDefault(x => _wordComparisonStrategy.IsMatch(x, value));
            if (word == null)
            {
                word = new Word(value);
                collection.Add(word);
            }
            return word;
        }
    }
}
