using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public class WordRepository
    {
        private readonly SimpleTextStorage _storage;

        public WordRepository(TextStorageFactory storageFactory)
        {
            _storage = Ensure.IsNotNull(storageFactory).Create("words");
        }

        public Word GetWord(long id)
        {
            return _storage.GetObject<Word>(id);
        }
    }
}
