using System.Linq;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal class GetByIdFetcher : FetcherBase
    {
        public GetByIdFetcher(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
            : base(objectFilename, textFileAccessor, objectStringParser)
        {
        }

        public FetchResult Fetch(long soughtId)
        {
            var result = FetchInternal(soughtId);
            return result.FirstOrDefault();
        }

        protected override bool TestLine(long objId, string objBody, object condition, out bool fetchComplete)
        {
            return fetchComplete = (objId == (long)condition);
        }
    }
}
