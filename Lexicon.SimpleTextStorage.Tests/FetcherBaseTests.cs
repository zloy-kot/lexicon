using System;
using System.Collections.Generic;
using System.IO;
using Lexicon.Core.Tests;
using Lexicon.SimpleTextStorage.Fetch;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class FetcherBaseTests
    {
        private ITextFileModifier _textFileModifier;
        private IObjectStringParser _parser;
        private DummyFetcher _fetcher;
        private string _defaultPath;

        [SetUp]
        public void SetUp()
        {
            _textFileModifier = Substitute.For<ITextFileModifier>();
            _parser = new ObjectStringParser();

            _defaultPath = Path.Combine(Environment.CurrentDirectory, "test_file.txt");

            _fetcher = new DummyFetcher(_defaultPath, _textFileModifier, _parser);
        }

        [Test]
        public void When_text_file_service_reads_nothing_Fetch_returns_empty_list()
        {
            _textFileModifier.ReadLine().Returns((string)null);

            var actual = _fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssert.IsEmpty(actual);
            _textFileModifier.Received(1).ReadLine();
        }

        [Test]
        public void When_text_file_service_reads_whitespaces_Fetch_returns_empty_list()
        {
            _textFileModifier.ReadLine().Returns("\t   \t ");

            var actual = _fetcher.Fetch();

            Assert.IsNotNull(actual);
            CollectionAssert.IsEmpty(actual);
            _textFileModifier.Received(1).ReadLine();
        }

        [Test]
        public void When_text_file_service_reads_broken_line_Fetch_throws_corresponding_SimpleTextException_but_Close_is_still_invoked_for_every_case()
        {
            _textFileModifier.ReadLine().Returns("2]тест#test");
            //act/assert
            var ex0 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.MissedObjectId, ex0.Reason);

            _textFileModifier.ReadLine().Returns("[]тест#test");
            //act/assert
            var ex1 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.CorruptedObjectId, ex1.Reason);

            _textFileModifier.ReadLine().Returns("[12тест#test");
            //act/assert
            var ex2 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.CorruptedObjectId, ex2.Reason);

            _textFileModifier.ReadLine().Returns("[12a]тест#test");
            //act/assert
            var ex3 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.CorruptedObjectId, ex3.Reason);

            _textFileModifier.ReadLine().Returns("]2тест#test");
            //act/assert
            var ex4 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.MissedObjectId, ex4.Reason);

            _textFileModifier.ReadLine().Returns("[12]");
            //act/assert
            var ex5 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.MissedObjectData, ex5.Reason);

            _textFileModifier.ReadLine().Returns("[12] \t  ");
            //act/assert
            var ex6 = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());
            Assert.AreEqual(SimpleTextExceptionReason.MissedObjectData, ex6.Reason);

            _textFileModifier.Received(7).Close();
        }

        [Test]
        public void When_text_file_service_fail_to_open_the_file_Fetch_throws_corresponding_SimpleTextException_but_Close_is_still_invoked()
        {
            _textFileModifier.When(x => x.Open(_defaultPath)).Do(x => { throw new Exception(); });

            var ex = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());

            Assert.AreEqual(SimpleTextExceptionReason.FileOpenningFailure, ex.Reason);
            _textFileModifier.Received(1).Close();
        }

        [Test]
        public void When_text_file_service_fail_to_read_a_line_the_file_Fetch_throws_corresponding_SimpleTextException_but_Close_is_still_invoked()
        {
            _textFileModifier.When(x => x.ReadLine()).Do(x => { throw new Exception(); });

            var ex = Assert.Throws<SimpleTextException>(() => _fetcher.Fetch());

            Assert.AreEqual(SimpleTextExceptionReason.LineReadingFailure, ex.Reason);
            _textFileModifier.Received(1).Close();
        }

        [Test]
        public void When_text_file_service_reads_correct_line_Fetch_asks_whether_to_include_the_line_into_the_final_result()
        {
            _textFileModifier.ReadLine().Returns("[12]test", "[13]quest", "[16]thing", "[18]state", "[21]option", String.Empty);
            _fetcher.SetTestLineResults(false, true, true, false, true);

            var actual1 = _fetcher.Fetch();

            CollectionAssertEx.ContainsOnly(actual1, new FetchResult(13, "quest"), new FetchResult(16, "thing"), new FetchResult(21, "option"));
        }

        [Test]
        public void When_text_file_service_reads_correct_line_Fetch_asks_whether_to_include_the_line_into_the_final_result_but_exits_on_the_first_complement_sign()
        {
            _textFileModifier.ReadLine().Returns("[12]test", "[13]quest", "[16]thing", "[18]state", "[21]option", String.Empty);
            _fetcher.SetTestLineResults(false, true, true, false, true);
            _fetcher.SetTestLineComplements(false, false, false, true);

            var actual1 = _fetcher.Fetch();

            CollectionAssertEx.ContainsOnly(actual1, new FetchResult(13, "quest"), new FetchResult(16, "thing"));
        }
    }

    internal class DummyFetcher : FetcherBase
    {
        private int _calls;
        private bool[] _results, _complements;

        public DummyFetcher(string objectFilename, ITextFileModifier textFileReader, IObjectStringParser objectStringParser) 
            : base(objectFilename, textFileReader, objectStringParser)
        {
            _calls = 0;
            _results = new [] {false};
            _complements = new[] { false };
        }

        public IList<FetchResult> Fetch()
        {
            return FetchInternal(null);
        }

        public void SetTestLineResults(params bool[] results)
        {
            _results = results;
        }

        public void SetTestLineComplements(params bool[] complements)
        {
            _complements = complements;
        }

        public void ResetCalls()
        {
            _calls = 0;
        }

        protected override bool TestLine(long objId, string objBody, object condition, out bool fetchComplete)
        {
            fetchComplete = _calls < _complements.Length ? _complements[_calls] : _complements[_complements.Length - 1];
            bool result = _calls < _results.Length ? _results[_calls] : _results[_results.Length - 1];
            _calls++;
            return result;
        }
    }
}