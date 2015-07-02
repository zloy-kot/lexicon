using System;
using System.Collections.Generic;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage.Fetch
{
    internal abstract class FetcherBase
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileModifier _textFileReader;

        protected FetcherBase(string objectFilename, ITextFileModifier textFileReader, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileReader = Ensure.IsNotNull(textFileReader);
        }

        protected virtual IList<FetchResult> FetchInternal(object condition)
        {
            IList<FetchResult> result = new List<FetchResult>();
            try
            {
                _textFileReader.Open(_objectFilename);
            }
            catch (Exception ex)
            {
                _textFileReader.Close();
                throw new SimpleTextException(SimpleTextExceptionReason.FileOpenningFailure, "Failed to open the file", ex);
            }
            try
            {

                int lineNo = 0;
                string line;
                while (!String.IsNullOrWhiteSpace(line = _textFileReader.ReadLine()))
                {
                    lineNo++;
                    long readId = _objectStringParser.ExtractObjectId(line, lineNo);
                    var objStr = _objectStringParser.ExtractObjectString(line, lineNo);

                    bool fetchComplete;
                    try
                    {
                        var matches = TestLine(readId, objStr, condition, out fetchComplete);
                        if (matches)
                            result.Add(new FetchResult(readId, objStr));
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
                _textFileReader.Close();
            }
            return result;
        }

        protected abstract bool TestLine(long objId, string objBody, object condition, out bool fetchComplete);
    }
}
