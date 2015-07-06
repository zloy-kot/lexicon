using System;
using Lexicon.Common;
using Lexicon.SimpleTextStorage.Fetch;
using Lexicon.SimpleTextStorage.Persistence;

namespace Lexicon.SimpleTextStorage.Impl
{
    internal class AccessorRegistry : IAccessorRegistry
    {
        private readonly string _objectFilename;
        private readonly IObjectStringParser _objectStringParser;
        private readonly ITextFileAccessor _textFileAccessor;

        private Lazy<GetByIdFetcher> _getByIdFetcher;
        private Lazy<GetAllFetcher> _getAllFetcher;
        private Lazy<GetByConditionFetcher> _getByConditionFetcher;

        private Lazy<ModifyByIdPersister> _byIdPersister;
        private Lazy<AddItemPersister> _addItemPersister;

        public AccessorRegistry(string objectFilename, ITextFileAccessor textFileAccessor, IObjectStringParser objectStringParser)
        {
            _objectFilename = Ensure.IsNotNullNorWhiteSpace(objectFilename);
            _objectStringParser = Ensure.IsNotNull(objectStringParser);
            _textFileAccessor = Ensure.IsNotNull(textFileAccessor);

            Init();
        }

        public ITextFileAccessor TextFileAccessor
        {
            get { return _textFileAccessor; }
        }

        private void Init()
        {
            _getByIdFetcher = new Lazy<GetByIdFetcher>(() => new GetByIdFetcher(_objectFilename, _textFileAccessor, _objectStringParser));
            _getAllFetcher = new Lazy<GetAllFetcher>(() => new GetAllFetcher(_objectFilename, _textFileAccessor, _objectStringParser));
            _getByConditionFetcher = new Lazy<GetByConditionFetcher>(() => new GetByConditionFetcher(_objectFilename, _textFileAccessor, _objectStringParser));

            _byIdPersister = new Lazy<ModifyByIdPersister>(() => new ModifyByIdPersister(_objectFilename, _textFileAccessor, _objectStringParser));
            _addItemPersister = new Lazy<AddItemPersister>(() => new AddItemPersister(_objectFilename, _textFileAccessor, _objectStringParser));
        }

        public GetByIdFetcher GetById
        {
            get { return _getByIdFetcher.Value; }
        }

        public GetAllFetcher GetAll
        {
            get { return _getAllFetcher.Value; }
        }

        public GetByConditionFetcher GetByCondition
        {
            get { return _getByConditionFetcher.Value; }
        }

        public ModifyByIdPersister ModifyById
        {
            get { return _byIdPersister.Value; }
        }

        public AddItemPersister CreateNew
        {
            get { return _addItemPersister.Value; }
        }
    }
}