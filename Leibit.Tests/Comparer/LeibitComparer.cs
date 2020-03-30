using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.Tests.Comparer
{
    internal abstract class LeibitComparer<T>
    {
        abstract internal void Compare(T expected, T actual);
        abstract internal Identifier GetIdentifier(T value);

        protected void CompareScalar<TValue>(TValue expected, TValue actual, string propertyName)
        {
            if (expected == null && actual == null)
                return;
            if (expected == null)
                Assert.Fail("Comparison of property '{0}' failed. Expected: NULL Actual: '{1}'", propertyName, actual);
            if (actual == null)
                Assert.Fail("Comparison of property '{0}' failed. Expected: '{1}' Actual: NULL", propertyName, expected);

            if (!expected.Equals(actual))
                Assert.Fail("Comparison of property '{0}' failed. Expected: '{1}' Actual: '{2}'", propertyName, expected, actual);
        }

        internal static void CompareList<TValue>(IEnumerable<TValue> expected, IEnumerable<TValue> actual, LeibitComparer<TValue> comparer, string propertyName)
        {
            if (expected.Count() != actual.Count())
                Assert.Fail("Different list size of property '{0}'. Expected: '{1}' Actual: '{2}'", propertyName, expected.Count(), actual.Count());

            foreach (var exp in expected)
            {
                var identifier = comparer.GetIdentifier(exp);
                var match = actual.FirstOrDefault(x => comparer.GetIdentifier(x).Equals(identifier));

                if (match == null)
                    Assert.Fail("Element '{0}' could not be found in list of property '{1}'.", exp, propertyName);

                comparer.Compare(exp, match);
            }
        }

        internal static void CompareDictionary<TKey, TValue>(IDictionary<TKey, TValue> expected, IDictionary<TKey, TValue> actual, LeibitComparer<TValue> comparer, string propertyName)
        {
            if (expected.Count != actual.Count)
                Assert.Fail("Different dictionary size of property '{0}'. Expected: '{1}' Actual: '{2}'", propertyName, expected.Count, actual.Count);

            foreach (var exp in expected)
            {
                if (!actual.ContainsKey(exp.Key))
                    Assert.Fail("Element '{0}' could not be found in dictionary of property '{1}'.", exp.Key, propertyName);

                comparer.Compare(exp.Value, actual[exp.Key]);
            }
        }

        internal static void CompareDictionary<TKey, TValue>(IDictionary<TKey, List<TValue>> expected, IDictionary<TKey, List<TValue>> actual, LeibitComparer<TValue> comparer, string propertyName)
        {
            if (expected.Count != actual.Count)
                Assert.Fail("Different dictionary size of property '{0}'. Expected: '{1}' Actual: '{2}'", propertyName, expected.Count, actual.Count);

            foreach (var exp in expected)
            {
                if (!actual.ContainsKey(exp.Key))
                    Assert.Fail("Element '{0}' could not be found in dictionary of property '{1}'.", exp.Key, propertyName);

                CompareList(exp.Value, actual[exp.Key], comparer, propertyName);
            }
        }
    }
}
