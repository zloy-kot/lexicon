using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon.Common
{
    public class Lesson
    {
        public Lesson(string name)
        {
            Name = Ensure.IsNullOrWhiteSpace(name);
            Words = new List<Word>();
        }

        public string Name { get; set; }

        public bool IsCurrent { get; set; }

        public IList<Word> Words { get; set; }
    }
}
