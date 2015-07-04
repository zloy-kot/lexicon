using System;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage.Persistence
{
    internal class Persister
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileAccessor _textFileAccessor;

        public Persister(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileAccessor = Ensure.IsNotNull(textFileAccessor);
        }

        public void Persist(long id, string objString)
        {
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
                    if (String.IsNullOrWhiteSpace(line)) continue;
                    lineNo++;
                    long readId = _objectStringParser.ExtractObjectId(line, lineNo);
                    if (readId == id)
                    {
                        _textFileAccessor.UpdateLine(objString);
                        break;
                    }
                }
            }
            catch (SimpleTextException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SimpleTextException(SimpleTextExceptionReason.LineReadingFailure, "Failed to modify the file", ex);
            }
            finally
            {
                _textFileAccessor.Close();
            }   
        }
    }
}