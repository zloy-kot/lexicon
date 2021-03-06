﻿using System;
using System.IO;
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
        public void consecutive_ReadLine_calls_returns_strings_in_the_right_order_seeking_to_the_next_line_after_each_reading()
        {
            string initLine1 = "someting\r\n", initLine2 = "newline\r\n", initLine3 = "another\r\n", initLine4 = "test";
            File.AppendAllText(_defaultPath, initLine1 + initLine2 + initLine3 + initLine4);
            _textFileAccessor.Open(_defaultPath);

            var line1 = _textFileAccessor.ReadLine();
            Assert.AreEqual("someting", line1);
            Assert.AreEqual(initLine1.Length, _textFileAccessor.CurrentPosition);

            var line2 = _textFileAccessor.ReadLine();
            Assert.AreEqual("newline", line2);
            Assert.AreEqual(initLine1.Length + initLine2.Length, _textFileAccessor.CurrentPosition);

            var line3 = _textFileAccessor.ReadLine();
            Assert.AreEqual("another", line3);
            Assert.AreEqual(initLine1.Length + initLine2.Length + initLine3.Length, _textFileAccessor.CurrentPosition);

            var line4 = _textFileAccessor.ReadLine();
            Assert.AreEqual("test", line4);
            Assert.AreEqual(initLine1.Length + initLine2.Length + initLine3.Length + initLine4.Length, _textFileAccessor.CurrentPosition);
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
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_empty_and_seeks_to_the_end()
        {
            var line = "some test";
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine(line);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("some test", lines[0]);
            Assert.AreEqual(line.Length, curPos);
        }

        [Test]
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_not_empty_and_seeks_to_the_end()
        {
            string line1 = "some test\r\nanother line", line2 = "new string";
            File.AppendAllText(_defaultPath, line1);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine(line2);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual("some test", lines[0]);
            Assert.AreEqual("another line", lines[1]);
            Assert.AreEqual("new string", lines[2]);
            Assert.AreEqual((line1 + line2 + Environment.NewLine).Length, curPos);
        }

        [Test]
        public void AddLine_adds_a_line_to_the_end_if_the_file_is_not_empty_and_ends_with_eol_and_seeks_to_the_end()
        {
            string line1 = "some test\r\nanother line\r\n", line2 = "new string";
            File.AppendAllText(_defaultPath, line1);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine(line2);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var lines = File.ReadAllLines(_defaultPath);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual("some test", lines[0]);
            Assert.AreEqual("another line", lines[1]);
            Assert.AreEqual("new string", lines[2]);
            Assert.AreEqual((line1 + line2).Length, curPos);
        }

        [Test]
        public void AddLine_adds_just_newline_when_an_empty_string_is_passed_and_seeks_to_the_end()
        {
            var line = "some test\r\nanother line";
            File.AppendAllText(_defaultPath, line);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.AddLine(String.Empty);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var lines = File.ReadAllText(_defaultPath);
            Assert.AreEqual("some test\r\nanother line\r\n", lines);
            Assert.AreEqual((line + Environment.NewLine).Length, curPos);
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

        [Test]
        public void UpdateLine_adds_a_line_if_the_file_is_empty_and_seeks_to_the_end()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            var line = "some line";
            _textFileAccessor.UpdateLine(line);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("some line", actual[0]);
            Assert.AreEqual(line.Length, curPos);
        }

        [Test]
        public void UpdateLine_updates_the_line_if_the_file_has_only_newline_symbol_and_seeks_to_the_end()
        {
            File.AppendAllText(_defaultPath, "\r\n");
            _textFileAccessor.Open(_defaultPath);

            var line = "some line";
            _textFileAccessor.UpdateLine(line);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("some line", actual[0]);
            Assert.AreEqual(line.Length, curPos);
        }

        [Test]
        public void UpdateLine_adds_the_line_to_the_end_of_the_file_if_current_position_is_in_the_end_of_the_file_and_seeks_to_the_end()
        {
            string line1 = "other line", line2 = "some line";
            File.AppendAllText(_defaultPath, line1);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(10);
            _textFileAccessor.UpdateLine(line2);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual((line1 + line2 + Environment.NewLine).Length, curPos);
        }
        
        [Test]
        public void UpdateLine_adds_the_line_to_the_end_of_the_file_if_current_position_is_in_the_end_of_the_file_and_the_file_ends_with_eol_and_seeks_to_the_end()
        {
            string line1 = "other line\r\n", line2 = "some line";
            File.AppendAllText(_defaultPath, line1);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(10);
            _textFileAccessor.UpdateLine(line2);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual((line1 + line2).Length, curPos);
        }

        [Test]
        public void UpdateLine_updates_a_line_at_the_current_position_when_existing_line_is_longer_then_the_new_one_and_seeks_to_the_beginning_of_the_next_line()
        {
            string init = "other line\r\n", newline = "test";
            File.AppendAllText(_defaultPath, init + "some line\r\none more line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(1);
            _textFileAccessor.UpdateLine(newline);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("test", actual[1]);
            Assert.AreEqual("one more line", actual[2]);
            Assert.AreEqual((init + newline + Environment.NewLine).Length, curPos);
        }

        [Test]
        public void UpdateLine_updates_a_line_at_the_current_position_when_existing_line_is_shorter_then_the_new_one()
        {
            File.AppendAllText(_defaultPath, "other line\r\nsome line\r\none more line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(1);
            _textFileAccessor.UpdateLine("some new quite long line");

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some new quite long line", actual[1]);
            Assert.AreEqual("one more line", actual[2]);
        }

        [Test]
        public void UpdateLine_updates_the_first_line_when_existing_line_is_longer_then_the_new_one_and_seeks_to_the_beginning_of_the_next_line()
        {
            var newline = "test";
            File.AppendAllText(_defaultPath, "other line\r\nsome line\r\none more line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.UpdateLine(newline);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("test", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual("one more line", actual[2]);
            Assert.AreEqual((newline + Environment.NewLine).Length, curPos);
        }

        [Test]
        public void UpdateLine_updates_the_first_line_when_existing_line_is_shorter_then_the_new_one()
        {
            File.AppendAllText(_defaultPath, "other line\r\nsome line\r\none more line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.UpdateLine("some new quite long line");

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("some new quite long line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual("one more line", actual[2]);
        }

        [Test]
        public void UpdateLine_updates_the_last_line_when_existing_line_is_longer_then_the_new_one_and_seeks_to_the_end()
        {
            string line1 = "other line\r\nsome line\r\n", line2 = "one more line", newline = "test";
            File.AppendAllText(_defaultPath, line1 + line2);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(2);
            _textFileAccessor.UpdateLine(newline);
            var curPos = _textFileAccessor.CurrentPosition;

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual("test", actual[2]);
            Assert.AreEqual((line1 + newline).Length, curPos);
        }

        [Test]
        public void UpdateLine_updates_the_last_line_when_existing_line_is_shorter_then_the_new_one()
        {
            File.AppendAllText(_defaultPath, "other line\r\nsome line\r\none more line");
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(2);
            _textFileAccessor.UpdateLine("some new quite long line");

            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual("some new quite long line", actual[2]);
        }

        [Test]
        public void RemoveLine_does_nothing_when_the_file_is_empty()
        {
            File.AppendAllText(_defaultPath, String.Empty);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.RemoveLine();

            Assert.AreEqual(0, _textFileAccessor.CurrentPosition);
        }

        [Test]
        public void RemoveLine_does_nothing_when_current_position_is_in_the_end_of_the_file()
        {
            var init = "other line\r\nsome line\r\none more line";
            File.AppendAllText(_defaultPath, init);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(10);
            _textFileAccessor.RemoveLine();

            Assert.AreEqual(init.Length, _textFileAccessor.CurrentPosition);
        }

        [Test]
        public void RemoveLine_does_nothing_when_current_position_is_in_the_end_of_the_file_and_the_file_ends_with_eol()
        {
            var init = "other line\r\nsome line\r\none more line\r\n";
            File.AppendAllText(_defaultPath, init);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(10);
            _textFileAccessor.RemoveLine();

            Assert.AreEqual(init.Length, _textFileAccessor.CurrentPosition);
        }

        [Test]
        public void RemoveLine_removes_the_first_line_and_seeks_to_the_beginning()
        {
            var init = "other line\r\nsome line\r\none more line\r\n";
            File.AppendAllText(_defaultPath, init);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.RemoveLine();

            var curPos = _textFileAccessor.CurrentPosition;
            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("some line", actual[0]);
            Assert.AreEqual("one more line", actual[1]);
            Assert.AreEqual(0, curPos);
        }

        [Test]
        public void RemoveLine_removes_the_last_line_and_seeks_to_the_end()
        {
            var expected = "other line\r\nsome line\r\n";
            var init = expected + "one more line\r\n";
            File.AppendAllText(_defaultPath, init);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(2);
            _textFileAccessor.RemoveLine();

            var curPos = _textFileAccessor.CurrentPosition;
            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("some line", actual[1]);
            Assert.AreEqual(expected.Length, curPos);
        }

        [Test]
        public void RemoveLine_removes_a_line_at_the_current_position_and_seeks_to_the_beginning_of_the_next_line()
        {
            var expected = "other line\r\n";
            var init = expected + "some line\r\none more line\r\n";
            File.AppendAllText(_defaultPath, init);
            _textFileAccessor.Open(_defaultPath);

            _textFileAccessor.SeekLines(1);
            _textFileAccessor.RemoveLine();

            var curPos = _textFileAccessor.CurrentPosition;
            _textFileAccessor.Dispose();

            var actual = File.ReadAllLines(_defaultPath);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("other line", actual[0]);
            Assert.AreEqual("one more line", actual[1]);
            Assert.AreEqual(expected.Length, curPos);
        }
    }
}