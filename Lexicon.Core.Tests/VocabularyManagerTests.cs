using System;
using System.Diagnostics.CodeAnalysis;
using Lexicon.Common;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class VocabularyManagerTests
    {
        private VocabularyManager _appManager;
        private IImporter _importer;
        private IStorage _storage;

        [SetUp]
        public void SetUp()
        {
            _storage = Substitute.For<IStorage>();
            _importer = Substitute.For<IImporter>();

            _appManager = new VocabularyManager(_storage, _importer);
        }

        [Test]
        public void ctor_throws_ArgumentNullException_if_nulls_passed()
        {
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IStorage>(), null));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(null, Substitute.For<IImporter>()));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IStorage>(), new IImporter[]{ null }));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IStorage>(), Substitute.For<IImporter>(), null, Substitute.For<IImporter>()));
        }

        [Test]
        public void AddLesson_throws_ArgumentNullException_if_null_passed()
        {
            Assert.Throws<ArgumentNullException>(() => _appManager.AddLesson(null));
        }

        [Test]
        public void AddLesson_adds_the_lesson_into_cache_and_notifies_storage()
        {
            Lesson lesson = new Lesson("Test lesson");

            _appManager.AddLesson(lesson);

            Assert.IsTrue(_appManager.Lessons.Contains(lesson));
            _storage.Received(1).SaveLesson(lesson);
        }

        [Test]
        public void When_current_lesson_found_GetCurrentLesson_returns_that_lesson()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _appManager.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = true };
            _appManager.AddLesson(lesson2);

            var actual = _appManager.GetCurrentLesson();

            Assert.AreEqual(name2, actual.Name);
        }

        [Test]
        public void When_current_lesson_not_found_GetCurrentLesson_returns_first_one_of_the_list()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _appManager.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = false };
            _appManager.AddLesson(lesson2);

            var actual = _appManager.GetCurrentLesson();

            Assert.AreEqual(name1, actual.Name);
        }

        [Test]
        public void When_the_lesson_list_is_empty_GetCurrentLesson_returns_null()
        {
            var actual = _appManager.GetCurrentLesson();

            Assert.IsNull(actual);
        }

        [Test]
        public void GetAllLessons_returns_all_lessons_of_the_list()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _appManager.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = false };
            _appManager.AddLesson(lesson2);

            var actual = _appManager.GetAllLessons();

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(name1, actual[0].Name);
            Assert.AreEqual(name2, actual[1].Name);
        }

        [Test]
        public void When_the_lesson_list_is_empty_GetAllLessons_returns_empty_list()
        {
            var actual = _appManager.GetAllLessons();

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void AcceptAnswer(Exercise exercise)
        {
            
        }
    }
}
