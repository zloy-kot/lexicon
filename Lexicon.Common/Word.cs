using System;

namespace Lexicon.Common
{
    public class Word : IEntity
    {
        public Word(string value)
        {
            Value = value;
        }

        public long Id { get; set; }

        public string Value { get; set; }

        public string LangCode { get; set; }

        public override string ToString()
        {
            return String.IsNullOrWhiteSpace(LangCode) ? Value : String.Format("[{0}] {1}", LangCode, Value);
        }
    }
}