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
            Name = Ensure.IsNotNullNorWhiteSpace(name);
            Words = new List<WordPair>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsCurrent { get; set; }

        public IList<WordPair> Words { get; set; }
    }
}
