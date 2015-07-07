using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lexicon.Common;
using Lexicon.SimpleTextStorage.Impl;

namespace Lexicon.SimpleTextStorage
{
    public class SimpleTextStorage
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ISerializerRegistry _serializerRegistry;
        private readonly IAccessorRegistry _accessorRegistry;

        private readonly static ConcurrentDictionary<string, long> CurrentIds;

        static SimpleTextStorage()
        {
            CurrentIds = new ConcurrentDictionary<string, long>();
        }

        public SimpleTextStorage(string objectFilename, ITextFileAccessor textFileAccessor, ISerializerRegistry serializerRegistry)
            : this(objectFilename, textFileAccessor, serializerRegistry, new ObjectStringParser())
        {   }

        public SimpleTextStorage(string objectFilename, ITextFileAccessor textFileAccessor, ISerializerRegistry serializerRegistry, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _serializerRegistry = Ensure.IsNotNull(serializerRegistry);

            _accessorRegistry = new AccessorRegistry(_objectFilename, Ensure.IsNotNull(textFileAccessor), objectStringParser);
        }

        public IList<T> GetAll<T>() where T : IEntity
        {
            var fetched = _accessorRegistry.GetAll.Fetch();

            IList<T> result = new List<T>();
            var serializer = _serializerRegistry.GetSerializer<T>();
            foreach (var f in fetched)
                result.Add(CreateObject(serializer, f));

            return result;
        }

        public T GetOne<T>(long id) where T: IEntity
        {
            var fetched = _accessorRegistry.GetById.Fetch(id);
            if (fetched == null)
                return default(T);

            var serializer = _serializerRegistry.GetSerializer<T>();
            var obj = CreateObject(serializer, fetched);

            if (obj.Id != id)
                throw new SimpleTextException(SimpleTextExceptionReason.LineFetchingFailure, "Failed to fetch the object string");
            return obj;
        }

        public long Save<T>(T entity) where T : IEntity
        {
            var serializer = _serializerRegistry.GetSerializer<T>();

            if (entity.Id == default(long))
            {
                entity.Id = GenNextId();
                var line = CreateString(serializer, entity);
                _accessorRegistry.CreateNew.Add(line);
            }
            else
            {
                var line = CreateString(serializer, entity);
                _accessorRegistry.ModifyById.Update(entity.Id, line);
            }
            return entity.Id;
        }

        public void Remove(long id)
        {
            _accessorRegistry.ModifyById.Remove(id);
        }

        internal long GenNextId()
        {
            long lastId;
            long newId;
            do
            {
                lastId = GetLastId();
                newId = lastId + 1;
            }
            while (!CurrentIds.TryUpdate(_objectFilename, newId, lastId));
            return newId;
        }

        private long GetLastId()
        {
            if (!CurrentIds.ContainsKey(_objectFilename))
                CurrentIds.TryAdd(_objectFilename, _accessorRegistry.GetByCondition.FetchMaxId());
            return CurrentIds[_objectFilename];
        }

        private T CreateObject<T>(ISimpleSerializer<T> serializer, AccessItem accessItem) where T : IEntity
        {
            var obj = serializer.Deserialize(accessItem.ObjectBody);
            if (obj == null)
                throw new SerializationException("Failed to deserialize the object string");
            obj.Id = accessItem.ObjectId;
            return obj;
        }

        private string CreateString<T>(ISimpleSerializer<T> serializer, T entity) where T : IEntity
        {
            var body = serializer.Serialize(entity);
            if (body == null)
                throw new SerializationException("Failed to serialize the entity");

            var line = _objectStringParser.BuildObjectString(entity.Id, body);
            return line;
        }
    }
}
