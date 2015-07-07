using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public class TextStorageFactory
    {
        private readonly ITextFileAccessor _textFileAccessor;
        private readonly ISerializerRegistry _serializerRegistry;

        public TextStorageFactory(ITextFileAccessor textFileAccessor, ISerializerRegistry serializerRegistry)
        {
            _textFileAccessor = Ensure.IsNotNull(textFileAccessor);
            _serializerRegistry = Ensure.IsNotNull(serializerRegistry);
        }

        public SimpleTextStorage Create(string objectFilename)
        {
            return new SimpleTextStorage(objectFilename, _textFileAccessor, _serializerRegistry);
        }
    }
}