using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lexicon.Common;

namespace Lexicon.Core
{
    public class LessonDispatcher
    {
        Random _random = new Random();
        private readonly ILessonRepository _lessonRepository;
        ExerciseDirection _exerciseDirection = ExerciseDirection.NativeToForeign;

        public LessonDispatcher(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;

            Lessons = new List<Lesson>();
        }

        internal IList<Lesson> Lessons { get; set; }

        public void AddLesson(Lesson lesson)
        {
            Ensure.IsNotNull(lesson);

            Lessons.Add(lesson);

            _lessonRepository.Save(lesson);
        }

        public Lesson GetCurrentLesson()
        {
            var current = Lessons.FirstOrDefault(x => x.IsCurrent);
            return current ?? Lessons.FirstOrDefault();
        }

        public IList<Lesson> GetAllLessons()
        {
            return Lessons;
        }

        public bool AcceptAnswer(Exercise exercise)
        {
            var pair = findWordPair(exercise.LessonId, exercise.WordPairId);

            var testedWord = getCorrectAnswer(exercise.Direction, pair);

            var correct = testedWord.Equals(exercise.Answer);

            return correct;
        }

        private WordPair findWordPair(long lessonId, long pairId)
        {
            return Lessons.Single(x => x.Id == lessonId).Words.Single(x => x.PairId == pairId);
        }

        private string getCorrectAnswer(ExerciseDirection direction, WordPair pair)
        {
            if (direction == ExerciseDirection.NativeToForeign)
                return pair.ForeignWord.Value;
            else
                return pair.NativeWord.Value;
        }

        public Exercise GetNextExercise()
        {
            var current = GetCurrentLesson();
            if(current == null)
                throw new ExerciseSelectionException(ExerciseSelectionExceptionReason.NoCurrentLesson);

            if (current.Words.Count == 0)
                throw new ExerciseSelectionException(ExerciseSelectionExceptionReason.NoAvailableWord);

            var rndPair = selectWordPair(current.Words);
            var exDir = selectDirection(_exerciseDirection);
            var word = exDir == ExerciseDirection.NativeToForeign ? rndPair.NativeWord.Value : rndPair.ForeignWord.Value;
            Exercise exercise = new Exercise(current.Id, rndPair.PairId, word, exDir);
            return exercise;
        }

        private T selectWordPair<T>(IList<T> list)
        {
            var rnd = _random.Next(list.Count);
            return list[rnd];
        }

        private ExerciseDirection selectDirection(ExerciseDirection exerciseDirection)
        {
            if (exerciseDirection == ExerciseDirection.BothDirections)
            {
                var rnd = _random.Next(2);
                return rnd == 0 ? ExerciseDirection.NativeToForeign : ExerciseDirection.ForeignToNative;
            }
            return exerciseDirection;
        }
    }
}
