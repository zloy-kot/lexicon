using System;
using System.Linq;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal class GetByConditionFetcher : FetcherBase
    {
        public GetByConditionFetcher(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
            : base(objectFilename, textFileAccessor, objectStringParser)
        {
        }

        public long FetchMaxId()
        {
            var res = FetchInternal(new __state { current = 0, testFunc = (cur, id, body) => id > cur });
            return res.Select(x => x.ObjectId).LastOrDefault();
        }

        protected override bool TestLine(long objId, string objBody, object condition, out bool fetchComplete)
        {
            fetchComplete = false;
            long current = ((__state)condition).current;
            if (((__state)condition).testFunc(current, objId, objBody))
            {
                ((__state)condition).current = objId;
                return true;
            }
            return false;
        }

        private class __state
        {
            public long current { get; set; }

            public Func<long, long, string, bool> testFunc { get; set; }
        }
    }
}