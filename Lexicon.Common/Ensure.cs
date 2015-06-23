using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon.Common
{
    public static class Ensure
    {
        public static T IsNotNull<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException();

            return value;
        }

        public static T[] IsNotNull<T>(T[] value)
        {
            if (value == null)
                throw new ArgumentNullException();

            if (value.Any(x => x == null))
                throw new ArgumentNullException();

            return value;
        }

        public static string IsNullOrWhiteSpace(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException();

            return value;
        }
    }
}
