using System.Collections.Generic;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal class GetAllFetcher : FetcherBase
    {
        public GetAllFetcher(string objectFilename, ITextFileModifier textFileReader, IObjectStringParser objectStringParser) 
            : base(objectFilename, textFileReader, objectStringParser)
        {
            
        }

        public IList<FetchResult> Fetch()
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