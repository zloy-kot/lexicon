using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public class TextStorageFactory
    {
        private readonly ITextFileModifier _textFileReader;
        private readonly ISerializerRegistry _serializerRegistry;

        public TextStorageFactory(ITextFileModifier textFileReader, ISerializerRegistry serializerRegistry)
        {
            _textFileReader = Ensure.IsNotNull(textFileReader);
            _serializerRegistry = Ensure.IsNotNull(serializerRegistry);
        }

        public SimpleTextStorage Create(string objectFilename)
        {
            return new SimpleTextStorage(objectFilename, _textFileReader, _serializerRegistry);
        }
    }
}