using System;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage.Persistence
{
    public abstract class PersisterBase<T>
    {
        protected readonly string ObjectFilename;
        protected readonly IObjectStringParser ObjectStringParser;
        protected readonly ITextFileAccessor TextFileAccessor;

        protected PersisterBase(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
        {
            ObjectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            ObjectStringParser = Ensure.IsNotNull(objectStringParser);
            TextFileAccessor = Ensure.IsNotNull(textFileAccessor);
        }

        public void PersistInternal(T condition)
        {
            try
            {
                TextFileAccessor.Open(ObjectFilename);
            }
            catch (Exception ex)
            {
                TextFileAccessor.Close();
                throw new SimpleTextException(SimpleTextExceptionReason.FileOpenningFailure, "Failed to open the file", ex);
            }
            try
            {
                ExecuteLogic(condition);
            }
            catch (SimpleTextException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SimpleTextException(SimpleTextExceptionReason.LineModificationFailure, "Failed to modify the file", ex);
            }
            finally
            {
                TextFileAccessor.Close();
            }   
        }

        protected virtual void ExecuteLogic(T condition)
        {
            int lineNo = 0;
            string line;
            while ((line = TextFileAccessor.ReadLine()) != null)
            {
                lineNo++;
                if (String.IsNullOrWhiteSpace(line)) continue;

                long readId = ObjectStringParser.ExtractObjectId(line, lineNo);

                bool persistenceComplete;
                try
                {
                    var matches = TestLine(readId, condition, out persistenceComplete);
                    if (matches)
                    {
                        TextFileAccessor.SeekLines(-1);
                        PersistLine(TextFileAccessor, readId, condition);
                    }
                }
                catch (Exception ex)
                {
                    throw new SimpleTextException(SimpleTextExceptionReason.LinePersistenceFailure, "Failed to persist the object string", ex);
                }
                if (persistenceComplete)
                    break;
            }
        }

        protected abstract bool TestLine(long objId, T condition, out bool persistenceComplete);

        protected abstract void PersistLine(ITextFileAccessor textFileAccessor, long objId, T condition);
    }
}