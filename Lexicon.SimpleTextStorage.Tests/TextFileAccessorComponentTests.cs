using System;
using System.IO;
using System.Text;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class TextFileAccessorComponentTests
    {
        private string _defaultPath;
        private TextFileAccessor _textFileAccessor;

        [SetUp]
        public void SetUp()
        {
            _defaultPath = Path.Combine(Environment.CurrentDirectory, "test_file.txt");
            CleanUp(_defaultPath);

            _textFileAccessor = new TextFileAccessor();
        }

        [TearDown]
        public void TearDown()
        {
            if (_textFileAccessor != null)
                _textFileAccessor.Dispose();
            CleanUp(_defaultPath);
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
        public void ReadLine_throws_ObjectDisposedException_when_target_was_disposed_of()
        {
            _textFileAccessor.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _textFileAccessor.ReadLine());
        }

        [Test]
        public void ReadLine_returns_null_when_reading_an_empty_file()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.IsNull(actual);
        }

        [Test]
        public void ReadLine_returns_empty_string_when_reading_newline_symbol()
        {
            File.AppendAllText(_defaultPath, Environment.NewLine);
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.IsEmpty(actual);
        }

        [Test]
        public void ReadLine_returns_string_when_it_ends_with_eof()
        {
            File.AppendAllText(_defaultPath, "someting");
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.AreEqual("someting", actual);
        }

        [Test]
        public void ReadLine_returns_string_when_it_ends_with_r()
        {
            File.AppendAllText(_defaultPath, "someting\rnewline");
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.AreEqual("someting", actual);
        }

        [Test]
        public void ReadLine_returns_string_when_it_ends_with_n()
        {
            File.AppendAllText(_defaultPath, "someting\nnewline");
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.AreEqual("someting", actual);
        }

        [Test]
        public void ReadLine_returns_string_when_it_ends_with_rn()
        {
            File.AppendAllText(_defaultPath, "someting\r\nnewline");
            _textFileAccessor.Open(_defaultPath);

            var actual = _textFileAccessor.ReadLine();

            Assert.AreEqual("someting", actual);
        }

        [Test]
        public void consecutive_ReadLine_calls_returns_strings_in_the_right_order()
        {
            File.AppendAllText(_defaultPath, "someting\r\nnewline\r\nanother\r\ntest");
            _textFileAccessor.Open(_defaultPath);

            var line1 = _textFileAccessor.ReadLine();
            Assert.AreEqual("someting", line1);

            var line2 = _textFileAccessor.ReadLine();
            Assert.AreEqual("newline", line2);

            var line3 = _textFileAccessor.ReadLine();
            Assert.AreEqual("another", line3);

            var line4 = _textFileAccessor.ReadLine();
            Assert.AreEqual("test", line4);
        }

        [Test]
        public void SeekLines_throws_ObjectDisposedException_when_target_was_disposed_of()
        {
            _textFileAccessor.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _textFileAccessor.SeekLines(Arg.Any<int>()));
        }

        [Test]
        public void get_CurrentPosition_throws_ObjectDisposedException_when_target_was_disposed_of()
        {
            _textFileAccessor.Dispose();

            long pos = 0;
            Assert.Throws<ObjectDisposedException>(() => pos = _textFileAccessor.CurrentPosition);
        }

        [Test]
        public void SeekLines_does_nothing_if_the_file_is_empty()
        {
            long pos = 0;
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(10);

            pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(0, pos);

            _textFileAccessor.SeekLines(-10);

            pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void SeekLines_does_nothing_when_count_passed_is_zero()
        {
            string toSeek = "some test\r\n";
            File.AppendAllText(_defaultPath, toSeek + "another line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(1);
            _textFileAccessor.SeekLines(0);

            long pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(toSeek.Length, pos);
        }

        [Test]
        public void When_count_is_positive_and_is_less_then_line_amount_available_SeekLines_seeks_as_many_number_of_lines_forward_as_count_has_passed()
        {
            var toSeek = "some test\r\nanother line\r\n";
            File.AppendAllText(_defaultPath, toSeek + "hey");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(2);

            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(toSeek.Length, pos);
            Assert.AreEqual("hey", _textFileAccessor.ReadLine());
        }

        [Test]
        public void When_count_is_positive_and_is_same_as_line_amount_available_SeekLines_seeks_to_the_end_of_the_file()
        {
            var toSeek = "some test\r\nanother line\r\nhey";
            File.AppendAllText(_defaultPath, toSeek);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(3);

            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(toSeek.Length, pos);
            Assert.AreEqual(null, _textFileAccessor.ReadLine());
        }

        [Test]
        public void When_count_is_positive_and_is_greater_then_line_amount_available_SeekLines_seeks_to_the_end_of_the_file()
        {
            var toSeek = "some test\r\nanother line\r\nhey";
            File.AppendAllText(_defaultPath, toSeek);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(30);

            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(toSeek.Length, pos);
            Assert.AreEqual(null, _textFileAccessor.ReadLine());
        }

        [Test]
        public void When_count_is_negative_and_is_less_then_line_amount_available_SeekLines_seeks_as_many_number_of_lines_backward_as_count_has_passed()
        {
            var toSeek = "some test\r\n";
            var toSeekAll = toSeek + "another line\r\nhey";
            File.AppendAllText(_defaultPath, toSeekAll);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(3);
            Assert.AreEqual(toSeekAll.Length, _textFileAccessor.CurrentPosition);

            _textFileAccessor.SeekLines(-2);
            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(toSeek.Length, pos);
            Assert.AreEqual("another line", _textFileAccessor.ReadLine());
        }

        [Test]
        public void When_count_is_negative_and_is_same_as_line_amount_available_SeekLines_seeks_to_the_beginning_of_the_file()
        {
            var toSeek = "some test\r\nanother line\r\nhey";
            File.AppendAllText(_defaultPath, toSeek);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(3);
            Assert.AreEqual(toSeek.Length, _textFileAccessor.CurrentPosition);

            _textFileAccessor.SeekLines(-3);
            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(0, pos);
            Assert.AreEqual("some test", _textFileAccessor.ReadLine());
        }

        [Test]
        public void When_count_is_negative_and_is_greater_then_line_amount_available_SeekLines_seeks_to_the_beginning_of_the_file()
        {
            var toSeek = "some test\r\nanother line\r\nhey";
            File.AppendAllText(_defaultPath, toSeek);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(3);
            Assert.AreEqual(toSeek.Length, _textFileAccessor.CurrentPosition);

            _textFileAccessor.SeekLines(-20);
            var pos = _textFileAccessor.CurrentPosition;
            Assert.AreEqual(0, pos);
            Assert.AreEqual("some test", _textFileAccessor.ReadLine());
        }

        [Test]
        public void AddLine_throws_ObjectDisposedException_when_target_has_been_disposed_of()
        {
            _textFileAccessor.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _textFileAccessor.AddLine("something"));
        }

        [Test]
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_empty()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine("some test");

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("some test", lines[0]);
        }

        [Test]
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_not_empty()
        {
            File.AppendAllText(_defaultPath, "some test\r\nanother line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine("new string");

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual("some test", lines[0]);
            Assert.AreEqual("another line", lines[1]);
            Assert.AreEqual("new string", lines[2]);
        }

        [Test]
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_not_empty_and_ends_with_eol()
        {
            File.AppendAllText(_defaultPath, "some test\r\nanother line\r\n");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine("new string");

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual("some test", lines[0]);
            Assert.AreEqual("another line", lines[1]);
            Assert.AreEqual("new string", lines[2]);
        }

        [Test]
        public void AddLine_adds_newline_only_when_an_empty_string_is_passed()
        {
            File.AppendAllText(_defaultPath, "some test\r\nanother line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine(String.Empty);

            _textFileAccessor.Dispose();

            var lines = File.ReadAllText(_defaultPath);
            Assert.AreEqual("some test\r\nanother line\r\n", lines);
        }

        [Test]
        public void AddLine_throws_ArgumentNullException_when_null_is_passed()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            Assert.Throws<ArgumentNullException>(() => _textFileAccessor.AddLine(null));
        }

        [Test]
        public void UpdateLine_throws_ObjectDisposedException_when_target_has_been_disposed_of()
        {
            _textFileAccessor.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _textFileAccessor.UpdateLine("something"));
        }

        [Test]
        public void UpdateLine_throws_ArgumentNullException_when_null_or_empty_string_is_passed()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            Assert.Throws<ArgumentNullException>(() => _textFileAccessor.UpdateLine(null));

            Assert.Throws<ArgumentNullException>(() => _textFileAccessor.UpdateLine(String.Empty));
        }
    }
}