using System;
using System.Collections.Generic;
using Lexicon.Common;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    [TestFixture]
    public class LessonDispatcherTests
    {
        private ILessonRepository _lessonRepository;
        private LessonDispatcher _exerciseDispatcher;

        [SetUp]
        public void SetUp()
        {
            _lessonRepository = Substitute.For<ILessonRepository>();

            _exerciseDispatcher = new LessonDispatcher(_lessonRepository);
        }

        [Test]
        public void AddLesson_throws_ArgumentNullException_if_null_passed()
        {
            Assert.Throws<ArgumentNullException>(() => _exerciseDispatcher.AddLesson(null));
        }

        [Test]
        public void AddLesson_adds_the_lesson_into_cache_and_notifies_storage()
        {
            Lesson lesson = new Lesson("Test lesson");

            _exerciseDispatcher.AddLesson(lesson);

            Assert.IsTrue(_exerciseDispatcher.Lessons.Contains(lesson));
            _lessonRepository.Received(1).Save(lesson);
        }

        [Test]
        public void When_current_lesson_found_GetCurrentLesson_returns_that_lesson()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _exerciseDispatcher.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = true };
            _exerciseDispatcher.AddLesson(lesson2);

            var actual = _exerciseDispatcher.GetCurrentLesson();

            Assert.AreEqual(name2, actual.Name);
        }

        [Test]
        public void When_current_lesson_not_found_GetCurrentLesson_returns_first_one_of_the_list()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _exerciseDispatcher.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = false };
            _exerciseDispatcher.AddLesson(lesson2);

            var actual = _exerciseDispatcher.GetCurrentLesson();

            Assert.AreEqual(name1, actual.Name);
        }

        [Test]
        public void When_the_lesson_list_is_empty_GetCurrentLesson_returns_null()
        {
            var actual = _exerciseDispatcher.GetCurrentLesson();

            Assert.IsNull(actual);
        }

        [Test]
        public void GetAllLessons_returns_all_lessons_of_the_list()
        {
            var name1 = "Test lesson 111";
            Lesson lesson1 = new Lesson(name1) { IsCurrent = false };
            _exerciseDispatcher.AddLesson(lesson1);

            var name2 = "Test lesson 222";
            Lesson lesson2 = new Lesson(name2) { IsCurrent = false };
            _exerciseDispatcher.AddLesson(lesson2);

            var actual = _exerciseDispatcher.GetAllLessons();

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(name1, actual[0].Name);
            Assert.AreEqual(name2, actual[1].Name);
        }

        [Test]
        public void When_the_lesson_list_is_empty_GetAllLessons_returns_empty_list()
        {
            var actual = _exerciseDispatcher.GetAllLessons();

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void When_direction_is_native_to_foreign_and_answer_to_the_exercise_is_correct_AcceptAnswer_returns_true()
        {
            _exerciseDispatcher.AddLesson(new Lesson("name") { Id = 1, 
                Words = new List<WordPair>
                {
                    new WordPair(new Word("native"), new Word("foreign")) { PairId = 1 }
                } 
            });
            var exercise = new Exercise(1, 1, "native", ExerciseDirection.NativeToForeign);

            exercise.Answer = "foreign";
            bool correct = _exerciseDispatcher.AcceptAnswer(exercise);

            Assert.IsTrue(correct);
        }
    }
}
