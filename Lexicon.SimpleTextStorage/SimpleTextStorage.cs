using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexicon.Common;
using Lexicon.SimpleTextStorage.Fetch;
using Lexicon.SimpleTextStorage.Persistence;

namespace Lexicon.SimpleTextStorage
{
    public class SimpleTextStorage
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileAccessor _textFileAccessor;
        private readonly ISerializerRegistry _serializerRegistry;
        private readonly GetByIdFetcher _getByIdFetcher;
        private readonly GetAllFetcher _getAllFetcher;
        private readonly Persister _persister;

        public SimpleTextStorage(string objectFilename, ITextFileAccessor textFileAccessor, ISerializerRegistry serializerRegistry)
            : this(objectFilename, textFileAccessor, serializerRegistry, new ObjectStringParser())
        {   }

        public SimpleTextStorage(string objectFilename, ITextFileAccessor textFileAccessor, ISerializerRegistry serializerRegistry, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileAccessor = Ensure.IsNotNull(textFileAccessor);
            _serializerRegistry = Ensure.IsNotNull(serializerRegistry);

            _getByIdFetcher = new GetByIdFetcher(_objectFilename, _textFileAccessor, _objectStringParser);
            _getAllFetcher = new GetAllFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            _persister = new Persister(_objectFilename, _textFileAccessor, _objectStringParser);
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

        public void UpdateLine<T>(T entity) where T: IEntity
        {
            var serializer = _serializerRegistry.GetSerializer<T>();
            var tmp = serializer.Serialize(entity);
            var serialized = String.Format("[{0}]{1}", entity.Id, tmp);

            _persister.Persist(entity.Id, serialized);
        }
    }
}
