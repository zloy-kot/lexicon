using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon.Common
{
    public class WordPair
    {
        public WordPair(Word native, Word foreign)
        {
            NativeWord = native;
            ForeignWord = foreign;
        }

        public long PairId { get; set; }

        public Word NativeWord { get; private set; }

        public Word ForeignWord { get; private set; }

        //add some stats of learning here
    }
}
