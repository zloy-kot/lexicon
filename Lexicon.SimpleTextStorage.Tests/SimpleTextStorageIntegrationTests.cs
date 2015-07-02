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
            _storage = new SimpleTextStorage(_defaultPath, new TextFileModifier(), registry);
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
            var actual = _storage.GetObject<DummyEntity>(4);

            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Id);
            Assert.AreEqual("вопрос", actual.Name);
            Assert.AreEqual("question", actual.Meaning);
            Assert.AreEqual("noun", actual.PartOfSpeech);
        }
    }
}