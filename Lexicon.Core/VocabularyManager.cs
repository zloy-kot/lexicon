using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexicon.Common;

namespace Lexicon.Core
{
    public class VocabularyManager
    {
        private readonly IStorage _storage;
        private readonly IImporter[] _importers;

        public VocabularyManager(IStorage storage, params IImporter[] importers)
        {
            _storage = Ensure.IsNotNull(storage);
            _importers = Ensure.IsNotNull(importers);

            Lessons = new List<Lesson>();
        }

        internal IList<Lesson> Lessons { get; set; }

        public void AddLesson(Lesson lesson)
        {
            Ensure.IsNotNull(lesson);

            Lessons.Add(lesson);

            _storage.SaveLesson(lesson);
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
    }
}
