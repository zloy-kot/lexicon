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
        }
    }
}
