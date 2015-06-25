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
            _exerciseDispatcher.AddLesson(createLesson(1, "name", new { nw = "native", fw = "foreign", id = 1 }));
            var exercise = new Exercise(1, 1, "native", ExerciseDirection.NativeToForeign);

            //act
            exercise.Answer = "foreign";
            bool correct = _exerciseDispatcher.AcceptAnswer(exercise);

            Assert.IsTrue(correct);
        }

        [Test]
        public void When_direction_is_foreign_to_native_and_answer_to_the_exercise_is_correct_AcceptAnswer_returns_true()
        {
            _exerciseDispatcher.AddLesson(createLesson(1, "name", new { nw = "native", fw = "foreign", id = 1 }));
            var exercise = new Exercise(1, 1, "foreign", ExerciseDirection.ForeignToNative);

            //act
            exercise.Answer = "native";
            bool correct = _exerciseDispatcher.AcceptAnswer(exercise);

            Assert.IsTrue(correct);
        }

        [Test]
        public void When_direction_is_native_to_foreign_and_answer_to_the_exercise_is_NOT_correct_AcceptAnswer_returns_false()
        {
            _exerciseDispatcher.AddLesson(createLesson(1, "name", new { nw = "native", fw = "foreign", id = 1 }));
            var exercise = new Exercise(1, 1, "native", ExerciseDirection.NativeToForeign);

            //act
            exercise.Answer = "unknown";
            bool correct = _exerciseDispatcher.AcceptAnswer(exercise);

            Assert.IsFalse(correct);
        }

        [Test]
        public void When_direction_is_foreign_to_native_and_answer_to_the_exercise_is_NOT_correct_AcceptAnswer_returns_false()
        {
            _exerciseDispatcher.AddLesson(createLesson(1, "name", new { nw = "native", fw = "foreign", id = 1 }));
            var exercise = new Exercise(1, 1, "foreign", ExerciseDirection.ForeignToNative);

            //act
            exercise.Answer = "unknown";
            bool correct = _exerciseDispatcher.AcceptAnswer(exercise);

            Assert.IsFalse(correct);
        }

        [Test]
        public void GetNextExercise_returns_an_exercise_from_the_current_lesson()
        {
            _exerciseDispatcher.AddLesson(createLesson(3, "name", new { nw = "проблема", fw = "problem", id = 1 }));

            Exercise exercise = _exerciseDispatcher.GetNextExercise();

            Assert.AreEqual(3, exercise.LessonId);
            Assert.AreEqual(1, exercise.WordPairId);
        }

        [Test]
        public void GetNextExercise_returns_one_of_exercises_from_the_current_lesson()
        {
            _exerciseDispatcher.AddLesson(createLesson(2, "name", new { nw = "проблема", fw = "problem", id = 1 }, new { nw = "задача", fw = "task", id = 2 }));

            Exercise exercise = _exerciseDispatcher.GetNextExercise();

            Assert.AreEqual(2, exercise.LessonId);
            Assert.That(exercise.WordPairId == 1 || exercise.WordPairId == 2);
        }

        [Test]
        public void When_no_lesson_created_GetNextExercise_throws_ExerciseSelectionException_with_NoCurrentLesson_reason()
        {
            var ex = Assert.Throws<ExerciseSelectionException>(() => _exerciseDispatcher.GetNextExercise());
            Assert.AreEqual(ExerciseSelectionExceptionReason.NoCurrentLesson, ex.Reason);
        }

        [Test]
        public void When_current_lesson_has_no_words_GetNextExercise_throws_ExerciseSelectionException_with_NoAvailableWord_reason()
        {
            _exerciseDispatcher.AddLesson(createLesson(2, "name"));

            var ex = Assert.Throws<ExerciseSelectionException>(() => _exerciseDispatcher.GetNextExercise());
            Assert.AreEqual(ExerciseSelectionExceptionReason.NoAvailableWord, ex.Reason);
        }

        private Lesson createLesson(long lessonId, string lessonName, params dynamic[] wordPairs)
        {
            var lesson = new Lesson(lessonName) {Id = lessonId};
            foreach(var p in wordPairs)
                lesson.Words.Add(new WordPair(new Word(p.nw), new Word(p.fw)) { PairId = p.id });
            return lesson;
        }
    }
}
