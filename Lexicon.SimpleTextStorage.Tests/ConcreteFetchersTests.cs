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
        private ITextFileModifier _textFileModifier;
        private IObjectStringParser _objectStringParser;

        [SetUp]
        public void SetUp()
        {
            _objectFilename = Path.Combine(Environment.CurrentDirectory, "testfile.txt");
            _textFileModifier = Substitute.For<ITextFileModifier>();
            _objectStringParser = new ObjectStringParser();
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_id_if_only_one_object_string_found()
        {
            _textFileModifier.ReadLine().Returns("[13]тест#test##noun", String.Empty);

            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(13, actual);
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_max_identifier_no_matter_what_the_order_of_object_strings_is()
        {
            _textFileModifier.ReadLine().Returns("[9]skip#пропустить##verb", "[12]тест#test##noun", "[11]задача#task##noun", String.Empty);
            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(12, actual);
        }

        [Test]
        public void GetByConditionFetcher_FetchMaxId_returns_0_if_no_object_strings_found()
        {
            _textFileModifier.ReadLine().Returns(String.Empty);

            var fetcher = new GetByConditionFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.FetchMaxId();

            Assert.AreEqual(0, actual);
        }

        [Test]
        public void GetAllFetcher_Fetch_returns_empty_list_if_no_object_string_found()
        {
            _textFileModifier.ReadLine().Returns(String.Empty);

            var fetcher = new GetAllFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssert.IsEmpty(actual);
        }

        [Test] 
        public void GetAllFetcher_Fetch_returns_all_object_strings()
        {
            _textFileModifier.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", String.Empty);

            var fetcher = new GetAllFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssertEx.ContainsOnly(actual, new FetchResult(4, "test1"), new FetchResult(6, "test2"), new FetchResult(9, "test3"));
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_null_if_no_object_string_was_found_by_id()
        {
            _textFileModifier.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", String.Empty);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.Fetch(7);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_null_if_text_file_service_reads_nothing()
        {
            _textFileModifier.ReadLine().Returns(String.Empty);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.Fetch(7);

            Assert.IsNull(actual);
        }

        [Test]
        public void GetByIdFetcher_Fetch_returns_object_if_it_is_found_by_id()
        {
            _textFileModifier.ReadLine().Returns("[4]test1", "[6]test2", "[9]test3", String.Empty);

            var fetcher = new GetByIdFetcher(_objectFilename, _textFileModifier, _objectStringParser);

            var actual = fetcher.Fetch(6);

            Assert.IsNotNull(actual);
            Assert.AreEqual(6, actual.ObjectId);
            Assert.AreEqual("test2", actual.ObjectBody);
        }
    }
}