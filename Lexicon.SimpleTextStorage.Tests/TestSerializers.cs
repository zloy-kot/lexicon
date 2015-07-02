using System;
using Lexicon.Common;

namespace Lexicon.SimpleTextStorage.Tests
{
    public class DummyEntity : IEquatable<DummyEntity>, IEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string PartOfSpeech { get; set; }

        public string Meaning { get; set; }

        public string Usage { get; set; }

        public bool Equals(DummyEntity other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id != other.Id) return false;
            if (Name != other.Name) return false;
            if (PartOfSpeech != other.PartOfSpeech) return false;
            if (Meaning != other.Meaning) return false;
            if (Usage != other.Usage) return false;
            return true;
        }
    }

    public class DummySerializer : TextSerializerBase<DummyEntity>
    {
        protected override DummyEntity CreateEntity(string[] raw)
        {
            return new DummyEntity
            {
                Name = raw[0],
                Meaning = raw[1],
                Usage = raw[2],
                PartOfSpeech = raw[3]
            };
        }

        protected override string[] CreateStringChain(DummyEntity obj)
        {
            return new[] {obj.Name, obj.Meaning, obj.Usage, obj.PartOfSpeech};
        }
    }

    public class EmptySerializer : TextSerializerBase<DummyEntity>
    {
        private readonly bool _onCreateStringChainReturnsEmptyArray;

        public EmptySerializer(bool onCreateStringChainReturnsEmptyArray = false)
        {
            _onCreateStringChainReturnsEmptyArray = onCreateStringChainReturnsEmptyArray;
        }

        protected override DummyEntity CreateEntity(string[] raw)
        {
            return null;
        }

        protected override string[] CreateStringChain(DummyEntity obj)
        {
            return _onCreateStringChainReturnsEmptyArray ? new string[0] : null;
        }
    }
}
