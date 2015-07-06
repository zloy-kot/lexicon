using System;

namespace Lexicon.SimpleTextStorage
{
    internal class AccessItem : IEquatable<AccessItem>
    {
        public AccessItem(long objectId, string objectBody)
        {
            ObjectId = objectId;
            ObjectBody = objectBody;
        }

        public long ObjectId { get; private set; }

        public string ObjectBody { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals((AccessItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ObjectId.GetHashCode() * 397) ^ (ObjectBody != null ? ObjectBody.GetHashCode() : 0);
            }
        }

        public bool Equals(AccessItem other)
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
