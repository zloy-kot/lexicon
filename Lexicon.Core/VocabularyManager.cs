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
        private readonly IVocabularyRepository _vocabularyRepository;
        private readonly IImporter[] _importers;

        public VocabularyManager(IVocabularyRepository vocabularyRepository, params IImporter[] importers)
        {
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
                var native = NativeWords.ResolveWord(wordDefinition.NativeWord);
                var foreign = ForeignWords.ResolveWord(tran);
                WordPairs.Add(new WordPair(native, foreign));
            }
        }

        //public WordDefinition CheckOccurence()?
    }
}
