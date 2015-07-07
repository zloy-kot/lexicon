using System;

namespace Lexicon.SimpleTextStorage
{
    public class SimpleTextException : Exception
    {
        public SimpleTextExceptionReason Reason { get; private set; }

        public SimpleTextException(SimpleTextExceptionReason reason)
        {
            Reason = reason;
        }

        public SimpleTextException(SimpleTextExceptionReason reason, string message)
            :  base(message)
        {
            Reason = reason;
        }

        public SimpleTextException(SimpleTextExceptionReason reason, string message, Exception innerException)
            :  base(message, innerException)
        {
            Reason = reason;
        }
    }

    public enum SimpleTextExceptionReason
    {
        FileOpenningFailure,
        LineReadingFailure,
        LineModificationFailure,
        LinePersistenceFailure,
        LineFetchingFailure,
        MissedObjectId,
        CorruptedObjectId,
        MissedObjectData
    }
}
