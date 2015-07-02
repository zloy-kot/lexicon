using System;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class TextSerializerBaseTests
    {
        private TextSerializerBase<DummyEntity> _serializer;
        
        [Test]
        public void When_null_passed_Serialize_throws_ArgumentNullException()
        {
            _serializer = new DummySerializer();
            Assert.Throws<ArgumentNullException>(() => _serializer.Serialize(null));
        }

        [Test]
        public void When_concrete_impl_returns_null_Serialize_throws_SerializationException()
        {
            _serializer = new EmptySerializer();

            Assert.Throws<SerializationException>(() => _serializer.Serialize(new DummyEntity()));
        }

        [Test]
        public void When_concrete_impl_returns_empty_array_Serialize_throws_SerializationException()
        {
            _serializer = new EmptySerializer(true);

            Assert.Throws<SerializationException>(() => _serializer.Serialize(new DummyEntity()));
        }

        [Test]
        public void When_concrete_impl_returns_valid_string_chain_Serialize_joins_the_chain_using_separator()
        {
            _serializer = new DummySerializer();
            char sep = TextSerializerBase<DummyEntity>.PropertySeparator;

            var actual1 = _serializer.Serialize(new DummyEntity{ Name = "test", Meaning = "noun" });
            Assert.AreEqual("test" + sep + "noun" + sep + "" + sep, actual1);

            var actual2 = _serializer.Serialize(new DummyEntity { Name = "test", Meaning = "" });
            Assert.AreEqual("test" + sep + sep + "" + sep, actual2);

            var actual3 = _serializer.Serialize(new DummyEntity { Name = "test", Meaning = null });
            Assert.AreEqual("test" + sep + sep + "" + sep, actual3);

            var actual4 = _serializer.Serialize(new DummyEntity { Name = "test", Meaning = null, Usage = "mil" });
            Assert.AreEqual("test" + sep + "" + sep + "mil" + sep, actual4);
        }
    }
}
