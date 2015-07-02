
using System;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal class FetchResult : IEquatable<FetchResult>
    {
        public FetchResult(long objectId, string objectBody)
        {
            ObjectId = objectId;
            ObjectBody = objectBody;
        }

        public long ObjectId { get; private set; }

        public string ObjectBody { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals((FetchResult)obj);
        }

        public bool Equals(FetchResult other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (ObjectId != other.ObjectId)
                return false;
            return (ObjectBody == other.ObjectBody);
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1}", ObjectId, ObjectBody);
        }
    }
}
