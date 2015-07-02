using System;
using System.IO;
using System.Runtime.Serialization;
using Lexicon.Core.Tests;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class SimpleTextStorageTests
    {
        private SimpleTextStorage _storage;
        private ISerializerRegistry _registry;
        private ITextFileModifier _textFileModifier;
        private string _defaultPath;

        [SetUp]
        public void SetUp()
        {
            _registry = Substitute.For<ISerializerRegistry>();
            _textFileModifier = Substitute.For<ITextFileModifier>();

            _defaultPath = Path.Combine(Environment.CurrentDirectory, "test_file.txt");

            _storage = new SimpleTextStorage(_defaultPath, _textFileModifier, _registry);
        }

        [Test]
        public void When_serializer_returns_null_GetObjectT_throws_SerializationException()
        {
            _textFileModifier.ReadLine().Returns("[10]тест#test", String.Empty);
            _registry.GetSerializer<DummyEntity>().Returns(Substitute.For<ISimpleSerializer<DummyEntity>>());

            Assert.Throws<SerializationException>(() => _storage.GetObject<DummyEntity>(10));
        }

        [Test]
        public void When_text_file_service_reads_correct_object_string_GetObjectT_returns_deserialized_object()
        {
            _textFileModifier.ReadLine().Returns("[10]тест#test##");
            _registry.GetSerializer<DummyEntity>().Returns(new DummySerializer());

            var actual = _storage.GetObject<DummyEntity>(10);

            Assert.AreEqual(10, actual.Id);
            Assert.AreEqual("тест", actual.Name);
            Assert.AreEqual("test", actual.Meaning);
        }

        [Test]
        public void When_text_file_service_reads_two_or_more_object_strings_GetObjectT_returns_deserialized_object_for_the_first_occurence()
        {
            _textFileModifier.ReadLine().Returns("[9]skip#пропустить##", "[10]тест#test##", "[10]задача#task##");
            _registry.GetSerializer<DummyEntity>().Returns(new DummySerializer());

            var actual = _storage.GetObject<DummyEntity>(10);

            Assert.AreEqual(10, actual.Id);
            Assert.AreEqual("тест", actual.Name);
            Assert.AreEqual("test", actual.Meaning);
        }

        [Test]
        public void When_text_file_service_doesnot_read_an_object_string_GetObjectT_returns_null()
        {
            _textFileModifier.ReadLine().Returns("[10]тест#test", String.Empty);

            var actual = _storage.GetObject<DummyEntity>(122);

            Assert.IsNull(actual);
        }

        [Test]
        public void When_text_file_service_doesnot_read_an_object_string_GetAllT_returns_empty_list()
        {
            _textFileModifier.ReadLine().Returns(String.Empty);

            var actual = _storage.GetAll<DummyEntity>();

            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void When_text_file_service_reads_some_object_strings_GetAllT_returns_deserialized_object_for_the_all_of_them()
        {
            _textFileModifier.ReadLine().Returns("[9]skip#пропустить##verb", "[10]тест#test##noun", "[11]задача#task##noun", String.Empty);
            _registry.GetSerializer<DummyEntity>().Returns(new DummySerializer());

            var actual = _storage.GetAll<DummyEntity>();

            CollectionAssertEx.ContainsOnly(actual,
                new DummyEntity {Id = 9, Name = "skip", Meaning = "пропустить", PartOfSpeech = "verb", Usage = ""},
                new DummyEntity { Id = 10, Name = "тест", Meaning = "test", PartOfSpeech = "noun", Usage = "" },
                new DummyEntity { Id = 11, Name = "задача", Meaning = "task", PartOfSpeech = "noun", Usage = "" });
        }
    }
}
