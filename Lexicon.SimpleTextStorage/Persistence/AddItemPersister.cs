namespace Lexicon.SimpleTextStorage.Persistence
{
    internal class AddItemPersister : PersisterBase<string>
    {
        public AddItemPersister(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser) 
            : base(objectFilename, textFileAccessor, objectStringParser)
        {
        }

        public void Add(string body)
        {
            PersistInternal(body);
        }

        protected override void ExecuteLogic(string condition)
        {
            TextFileAccessor.AddLine(condition);
        }

        protected override bool TestLine(long objId, string condition, out bool persistenceComplete)
        {
            throw new System.NotImplementedException();
        }

        protected override void PersistLine(ITextFileAccessor textFileAccessor, long objId, string condition)
        {
            throw new System.NotImplementedException();
        }
    }
}