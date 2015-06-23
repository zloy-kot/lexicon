using System;
using System.Collections.Generic;
using System.Linq;
using Lexicon.Common;

namespace Lexicon.Core
{
    public class LessonDispatcher
    {
        private readonly ILessonRepository _lessonRepository;

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
    }
}
