namespace Lexicon.Common
{
    public class Word
    {
        public Word(string value)
        {
            Value = value;
        }

        public long Id { get; set; }

        public string Value { get; set; }

        public string LangCode { get; set; }
    }
}