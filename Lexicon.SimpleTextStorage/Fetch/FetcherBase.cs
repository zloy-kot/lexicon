using System;
using System.Collections.Generic;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal abstract class FetcherBase
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileAccessor _textFileAccessor;

        protected FetcherBase(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileAccessor = Ensure.IsNotNull(textFileAccessor);
        }

        protected virtual IList<AccessItem> FetchInternal(object condition)
        {
            IList<AccessItem> result = new List<AccessItem>();
            try
            {
                _textFileAccessor.Open(_objectFilename);
            }
            catch (Exception ex)
            {
                _textFileAccessor.Close();
                throw new SimpleTextException(SimpleTextExceptionReason.FileOpenningFailure, "Failed to open the file", ex);
            }
            try
            {

                int lineNo = 0;
                string line;
                while ((line = _textFileAccessor.ReadLine()) != null)
                {
                    lineNo++;
                    if (String.IsNullOrWhiteSpace(line)) continue;

                    long readId = _objectStringParser.ExtractObjectId(line, lineNo);
                    var objStr = _objectStringParser.ExtractObjectBody(line, lineNo);

                    bool fetchComplete;
                    try
                    {
                        var matches = TestLine(readId, objStr, condition, out fetchComplete);
                        if (matches)
                            result.Add(new AccessItem(readId, objStr));
                    }
                    catch (Exception ex)
                    {
                        throw new SimpleTextException(SimpleTextExceptionReason.LineFetchingFailure, "Failed to fetch the object string", ex);
                    }
                    if (fetchComplete)
                        break;
                }
            }
            catch (SimpleTextException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SimpleTextException(SimpleTextExceptionReason.LineReadingFailure, "Failed to read the file", ex);
            }
            finally
            {
                _textFileAccessor.Close();
            }
            return result;
        }

        protected abstract bool TestLine(long objId, string objBody, object condition, out bool fetchComplete);
    }
}
