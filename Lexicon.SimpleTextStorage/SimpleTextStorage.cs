using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexicon.Common;
using Lexicon.SimpleTextStorage.Fetch;

namespace Lexicon.SimpleTextStorage
{
    public class SimpleTextStorage
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileModifier _textFileReader;
        private readonly ISerializerRegistry _serializerRegistry;
        private readonly GetByIdFetcher _getByIdFetcher;
        private readonly GetAllFetcher _getAllFetcher;

        public SimpleTextStorage(string objectFilename, ITextFileModifier textFileReader, ISerializerRegistry serializerRegistry)
            : this(objectFilename, textFileReader, serializerRegistry, new ObjectStringParser())
        {   }

        public SimpleTextStorage(string objectFilename, ITextFileModifier textFileReader, ISerializerRegistry serializerRegistry, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileReader = Ensure.IsNotNull(textFileReader);
            _serializerRegistry = Ensure.IsNotNull(serializerRegistry);

            _getByIdFetcher = new GetByIdFetcher(_objectFilename, _textFileReader, _objectStringParser);
            _getAllFetcher = new GetAllFetcher(_objectFilename, _textFileReader, _objectStringParser);
        }

        public IList<T> GetAll<T>() where T : IEntity
        {
            var fetched = _getAllFetcher.Fetch();

            IList<T> result = new List<T>();
            var serializer = _serializerRegistry.GetSerializer<T>();
            foreach (var f in fetched)
                result.Add(createObject(serializer, f));

            return result;
        }

        public T GetObject<T>(long id) where T: IEntity
        {
            var fetched = _getByIdFetcher.Fetch(id);
            if (fetched == null)
                return default(T);

            var serializer = _serializerRegistry.GetSerializer<T>();
            var obj = createObject(serializer, fetched);

            if (obj.Id != id)
                throw new SimpleTextException(SimpleTextExceptionReason.LineFetchingFailure, "Failed to fetch the object string");
            return obj;
        }

        private T createObject<T>(ISimpleSerializer<T> serializer, FetchResult fetchResult) where T : IEntity
        {
            var obj = serializer.Deserialize(fetchResult.ObjectBody);
            if (obj == null)
                throw new SerializationException("Failed to deserialize the object string");
            obj.Id = fetchResult.ObjectId;
            return obj;
        }
    }
}
