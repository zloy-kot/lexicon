using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class SimpleTextStorageIntegrationTests
    {
        private string _defaultPath;
        private SimpleTextStorage _storage;

        [SetUp]
        public void SetUp()
        {
            _defaultPath = Path.Combine(Environment.CurrentDirectory, "test_file.txt");
            CleanUp(_defaultPath);
            Init(_defaultPath);

            var registry = new SerializerRegistry();
            registry.Register(new DummySerializer());
            _storage = new SimpleTextStorage(_defaultPath, new TextFileAccessor(), registry);
        }

        [TearDown]
        public void TearDown()
        {
            CleanUp(_defaultPath);
        }

        private void Init(string path)
        {
            StringBuilder contents = new StringBuilder();
            contents.AppendLine("[1]тест#test##verb");
            contents.AppendLine("[2]задача#task##noun");
            contents.AppendLine("[3]найти#find out##phrasal verb");
            contents.AppendLine("[4]вопрос#question##noun");
            contents.AppendLine("[5]продолжить#go on##phrasal verb");
            File.AppendAllText(path, contents.ToString());
        }

        private void CleanUp(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }

        [Test]
        public void read_object_from_file_by_id()
        {
            var actual = _storage.GetOne<DummyEntity>(4);

            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Id);
            Assert.AreEqual("вопрос", actual.Name);
            Assert.AreEqual("question", actual.Meaning);
            Assert.AreEqual("noun", actual.PartOfSpeech);
        }

        [Test]
        public void update_object_in_the_file_by_id()
        {
            var e = new DummyEntity
            {
                Id = 3,
                Name = "отыскать",
                Meaning = "find out",
                PartOfSpeech = "phrasal verb",
                Usage = ""
            };
            var actualId = _storage.Save(e);

            var lines = File.ReadAllLines(_defaultPath);

            Assert.IsNotNull(lines);
            Assert.AreEqual(5, lines.Length);
            Assert.AreEqual(3, actualId);
            Assert.AreEqual("[3]отыскать#find out##phrasal verb", lines[2]);
        }

        [Test]
        public void add_object_to_the_file()
        {
            var e = new DummyEntity
            {
                Name = "ответ",
                Meaning = "answer",
                PartOfSpeech = "noun",
                Usage = ""
            };
            var actualId = _storage.Save(e);

            var lines = File.ReadAllLines(_defaultPath);

            Assert.IsNotNull(lines);
            Assert.AreEqual(6, actualId);
            Assert.AreEqual("[6]ответ#answer##noun", lines[lines.Length - 1]);
        }

        [Test]
        public void remove_object_from_the_file()
        {
            _storage.Remove(4);

            var lines = File.ReadAllLines(_defaultPath);

            Assert.IsNotNull(lines);
            Assert.AreEqual(4, lines.Length);
            Assert.AreEqual("[3]найти#find out##phrasal verb", lines[lines.Length - 2]);
            Assert.AreEqual("[5]продолжить#go on##phrasal verb", lines[lines.Length - 1]);
        }
    }
}