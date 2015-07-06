using Lexicon.SimpleTextStorage.Fetch;
using Lexicon.SimpleTextStorage.Persistence;

namespace Lexicon.SimpleTextStorage
{
    internal interface IAccessorRegistry
    {
        ITextFileAccessor TextFileAccessor { get; }
        GetByIdFetcher GetById { get; }
        GetAllFetcher GetAll { get; }
        GetByConditionFetcher GetByCondition { get; }
        ModifyByIdPersister ModifyById { get; }
        AddItemPersister CreateNew { get; }
    }
}