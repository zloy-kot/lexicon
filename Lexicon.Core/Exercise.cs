namespace Lexicon.Core
{
    public class Exercise
    {
        public Exercise(long lessonId, long wordPairId, string word, ExerciseDirection direction)
        {
            LessonId = lessonId;
            WordPairId = wordPairId;
            Word = word;
            Direction = direction;
        }

        internal long LessonId { get; private set; }

        internal long WordPairId { get; private set; }

        public string Word { get; private set; }

        public ExerciseDirection Direction { get; private set; }

        public string Answer { get; set; }
    }

    public enum ExerciseDirection
    {
        NativeToForeign,
        ForeignToNative,
        BothDirections
    }
}