using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon.Common
{
    public class Exercise
    {
        public Exercise(string word, ExerciseDirection direction)
        {
            Word = word;
            Direction = direction;
        }

        public string Word { get; private set; }

        public ExerciseDirection Direction { get; private set; }

        public string Answer { get; set; }
    }

    public enum ExerciseDirection
    {
        NativeToForeign,
        ForeignToNative
    }
}
