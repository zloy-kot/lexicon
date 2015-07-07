using System;
using System.IO;
using Lexicon.Core.Tests;
using Lexicon.SimpleTextStorage.Fetch;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class ConcreteFetchersTests
    {
        private string _objectFilename;
        private ITextFileAccessor _textFileAccessor;
        private IObjectStringParser _objectStringParser;
        private static string EOF = null;

        [SetUp]
        public void SetUp()
        {
            _objectFilename = Path.Combine(Environment.CurrentDirectory, "testfile.txt");
            _textFileAccessor = Substitute.For<ITextFileAccessor>();
            _objectStringParser = new ObjectStringParser();
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_id_if_only_one_object_string_found()
        {
            _textFileAccessor.ReadLine().Returns("[13]тест#test##noun", EOF);

            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(13, actual);
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_max_identifier_no_matter_what_the_order_of_object_strings_is()
        {
            _textFileAccessor.ReadLine().Returns("[9]skip#пропустить##verb", "[12]тест#test##noun", "[11]задача#task##noun", EOF);
            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(12, actual);
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_0_if_no_object_strings_found()
        {
            _textFileAccessor.ReadLine().Returns(EOF);

            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(0, actual);
        }

        [Test]
        public void GetAllFetcher_Fetch_returns_empty_list_if_no_object_string_found()
        {
            _textFileAccessor.ReadLine().Returns(EOF);

            var fetcher = new GetAllFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssert.IsEmpty(actual);
        }

        [Test] 
        public void GetAllFetcher_Fetch_returns_all_object_strings()
        {
            _textFileAccessor.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", EOF);

            var fetcher = new GetAllFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssertEx.ContainsOnly(actual, new AccessItem(4, "test1"), new AccessItem(6, "test2"), new AccessItem(9, "test3"));
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_null_if_no_object_string_was_found_by_id()
        {
            _textFileAccessor.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", EOF);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.Fetch(7);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_null_if_text_file_service_reads_nothing()
        {
            _textFileAccessor.ReadLine().Returns(EOF);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.Fetch(7);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_object_if_it_is_found_by_id()
        {
            _textFileAccessor.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", EOF);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileAccessor, _objectStringParser);

            var actual = fetcher.Fetch(6);

            Assert.IsNotNull(actual);
            Assert.AreEqual(6, actual.ObjectId);
            Assert.AreEqual("test2", actual.ObjectBody);
        }
    }
}