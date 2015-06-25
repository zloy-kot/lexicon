using System;

namespace Lexicon.Common
{
    public class ExerciseSelectionException : Exception
    {
        public ExerciseSelectionExceptionReason Reason { get; private set; }

        public ExerciseSelectionException(ExerciseSelectionExceptionReason reason)
        {
            Reason = reason;
        }

        public ExerciseSelectionException(ExerciseSelectionExceptionReason reason, string message)
            : base(message)
        {
            Reason = reason;
        }

        public ExerciseSelectionException(ExerciseSelectionExceptionReason reason, string message, Exception innerException)
            : base(message, innerException)
        {
            Reason = reason;
        }
    }

    public enum ExerciseSelectionExceptionReason
    {
        NoCurrentLesson,
        NoAvailableWord
    }
}
