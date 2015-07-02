using System;
using System.Runtime.Serialization;
using System.Text;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public abstract class TextSerializerBase<T> : ISimpleSerializer<T> where T: IEntity
    {
        public const char PropertySeparator = '#';

        public virtual T Deserialize(string objString)
        {
            Ensure.IsNotNullNorWhiteSpace(objString);

            var raw = objString.Trim().Split(PropertySeparator);

            T res;
            if (raw.Length > 0)
            {
                res = CreateEntity(raw);
                if (res == null)
                    throw new SerializationException(typeof(T).Name + " object could not be deserialized");
            }
            else
                throw new SerializationException(typeof (T).Name + " object could not be deserialized");

            return res;
        }

        protected abstract T CreateEntity(string[] raw);

        protected abstract string[] CreateStringChain(T obj);

        public virtual string Serialize(T obj)
        {
            Ensure.IsNotNull(obj);

            var raw = CreateStringChain(obj);
            if (raw == null)
                throw new SerializationException(typeof(T).Name + " object could not be serialized");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < raw.Length; i++)
            {
                if (i + 1 == raw.Length)
                    sb.Append(raw[i]);
                else
                    sb.AppendFormat("{0}{1}", raw[i], PropertySeparator);
            }

            var result = sb.ToString();
            if (String.IsNullOrWhiteSpace(result))
                throw new SerializationException(typeof(T).Name + " object could not be serialized");
            return result;
        }
    }
}
