using System;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage
{
    public interface IObjectStringParser
    {
        long ExtractObjectId(string line, int lineNo);
        string ExtractObjectString(string line, int lineNo);
    }

    public class ObjectStringParser : IObjectStringParser
    {

        public long ExtractObjectId(string line, int lineNo)
        {
            Ensure.IsNotNullNorWhiteSpace(line);

            if (!line.StartsWith("["))
                throw new SimpleTextException(SimpleTextExceptionReason.MissedObjectId, String.Format("Cannot find object id at line {0}", lineNo));
            var closedAt = line.IndexOf("]", StringComparison.Ordinal);
            if (closedAt == -1)
                throw new SimpleTextException(SimpleTextExceptionReason.CorruptedObjectId, String.Format("Cannot read object id at line {0}", lineNo));
            var strId = line.Substring(1, closedAt - 1);
            long readId;
            if (!Int64.TryParse(strId, out readId))
                throw new SimpleTextException(SimpleTextExceptionReason.CorruptedObjectId, String.Format("Cannot parse object id at line {0}", lineNo));
            return readId;
        }

        public string ExtractObjectString(string line, int lineNo)
        {
            Ensure.IsNotNullNorWhiteSpace(line);

            var closedAt = line.IndexOf("]", StringComparison.Ordinal);
            if (closedAt == -1)
                throw new SimpleTextException(SimpleTextExceptionReason.CorruptedObjectId, String.Format("Cannot read object id at line {0}", lineNo));
            string res = line.Substring(closedAt + 1);
            if (String.IsNullOrWhiteSpace(res))
                throw new SimpleTextException(SimpleTextExceptionReason.MissedObjectData, String.Format("Cannot find object data at line {0}", lineNo));
            return res;
        } 
    }
}