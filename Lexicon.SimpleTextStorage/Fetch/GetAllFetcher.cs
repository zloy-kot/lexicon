using System.Collections.Generic;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal class GetAllFetcher : FetcherBase
    {
        public GetAllFetcher(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser) 
            : base(objectFilename, textFileAccessor, objectStringParser)
        {
            
        }

        public IList<AccessItem> Fetch()
        {
            return FetchInternal(null);
        }

        protected override bool TestLine(long objId, string objBody, object condition, out bool fetchComplete)
        {
            fetchComplete = false;
            return true;
        }
    }
}