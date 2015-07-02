using System;
using System.Collections.Generic;

namespace Lexicon.SimpleTextStorage
{
    public interface ISerializerRegistry
    {
        void Register<T>(ISimpleSerializer<T> serializer);
        ISimpleSerializer<T> GetSerializer<T>();
    }

    public class SerializerRegistry : ISerializerRegistry
    {
        private readonly IDictionary<string, object> _serializers;

        public SerializerRegistry()
        {
            _serializers = new Dictionary<string, object>();
        }

        public void Register<T>(ISimpleSerializer<T> serializer)
        {
            _serializers.Add(typeof(T).FullName, serializer);
        }

        public ISimpleSerializer<T> GetSerializer<T>()
        {
            var fqn = typeof(T).FullName;
            if (_serializers.ContainsKey(fqn))
                return (ISimpleSerializer<T>)_serializers[fqn];
            throw new ArgumentException("Unable to resolve serializer for type '{0}'", fqn);
        }
    }
}
