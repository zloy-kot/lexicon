using System;
using System.Diagnostics.CodeAnalysis;

namespace Lexicon.SimpleTextStorage.Persistence
{
    internal class ModifyByIdPersister : PersisterBase<ModifyByIdPersister.__state>
    {
        public ModifyByIdPersister(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser) 
            : base(objectFilename, textFileAccessor, objectStringParser)
        {
        }

        public void Update(long id, string body)
        {
            var pia = new __state
            {
                action = (a, b) => a.UpdateLine(b),
                id = id,
                body = body
            };
            PersistInternal(pia);
        }

        public void Remove(long id)
        {
            var pia = new __state
            {
                action = (a, b) => a.RemoveLine(),
                id = id
            };
            PersistInternal(pia);
        }

        protected override bool TestLine(long objId, __state condition, out bool persistenceComplete)
        {
            return persistenceComplete = (objId == condition.id);
        }

        protected override void PersistLine(ITextFileAccessor textFileAccessor, long objId, __state condition)
        {
            condition.action(textFileAccessor, condition.body);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal class __state
        {
            public Action<ITextFileAccessor, string> action { get; set; }
            public long id { get; set; }
            public string body { get; set; }
        }
    }
}