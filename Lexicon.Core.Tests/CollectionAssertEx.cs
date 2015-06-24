using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    public static class CollectionAssertEx
    {
        /// <summary>
        /// Asserts that collection contains actuals only
        /// </summary>
        /// <param name="actual">IEnumerable of objects to be considered</param>
        /// <param name="expected">IEnumerable of objects to be found within collection</param>
        public static void ContainsOnly<T>(IEnumerable<T> actual, params T[] expected)
        {
            Assert.IsNotNull(actual);
            CollectionAssert.AllItemsAreNotNull(actual);
            var equal = actual.SequenceEqual(expected);
            if (!equal)
                Assert.Fail("Actual and expected collections do not match");
        }
    }
}
