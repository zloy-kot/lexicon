using System;
using System.Collections.Generic;

namespace Lexicon.Core
{
    public class WordDefinition
    {
        public WordDefinition(string native)
        {
            NativeWord = native;
            Translations = new List<string>();
        }

        public string NativeWord { get; set; }

        public IList<string> Translations { get; private set; }
    }
}
