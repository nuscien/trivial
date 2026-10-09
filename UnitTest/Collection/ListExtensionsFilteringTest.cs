using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Data;

namespace Trivial.Collection
{
    [TestClass]
    public class ListExtensionsFilteringTest
    {
        [TestMethod]
        public void NullFilteringAndSequenceEnds()
        {
            Assert.AreSequenceEqual(new[] { "x", "" }, ListExtensions.WhereNotNull(new[] { null, "x", "" }).ToArray());
            Assert.AreSequenceEqual(new[] { "x", " " }, new[] { null, "", "x", " " }.WhereNotEmpty().ToArray());
            Assert.AreSequenceEqual(new[] { "x" }, new[] { null, "", " ", "x" }.WhereNotNullNorWhiteSpace().ToArray());
            Assert.IsNull(ListExtensions.WhereNotNull<string>(null));
            Assert.IsNull(ListExtensions.WhereNotEmpty(null));
            Assert.IsNull(ListExtensions.WhereNotNullNorWhiteSpace(null));
            Assert.AreEqual(1, ListExtensions.FirstOrNull(new[] { 1, 2 }));
            Assert.AreEqual(2, ListExtensions.LastOrNull(new[] { 1, 2 }));
            Assert.AreEqual(2, ListExtensions.LastOrNull(Enumerable.Range(1, 2).Where(x => x > 0)));
            Assert.IsNull(ListExtensions.FirstOrNull<int>(null));
            Assert.IsNull(ListExtensions.LastOrNull(Array.Empty<int>()));
            Assert.AreEqual(9, ListExtensions.First(Array.Empty<int>(), 9));
            Assert.AreEqual(9, ListExtensions.Last<int>(null, 9));
        }

        [TestMethod]
        public void IndexesSetAndReplace()
        {
            var values = new[] { "a", null, "b", "a", null };
            Assert.AreSequenceEqual(new[] { 0, 3 }, values.AllIndexesOf("a").ToArray());
            Assert.AreSequenceEqual(new[] { 1, 4 }, values.AllIndexesOf((string)null).ToArray());
            Assert.AreSequenceEqual(new[] { 0 }, values.AllIndexesOf(x => x == "a", 3).ToArray());
            var pairs = new List<KeyValuePair<string, int>>
            {
                new("a", 1), new("b", 2), new("a", 1)
            };
            Assert.AreSequenceEqual(new[] { 0, 2 }, pairs.AllIndexesOf("a").ToArray());
            Assert.AreSequenceEqual(new[] { 0, 2 }, pairs.AllIndexesOf("a", 1).ToArray());
            pairs.Set("a", 3);
            Assert.AreEqual(3, pairs[0].Value);
            Assert.AreEqual(2, pairs.Count);
            pairs.Set("b", 4, true);
            Assert.AreEqual(4, pairs.Last().Value);
            var list = new List<int> { 1, 2, 1 };
            Assert.AreEqual(1, ListExtensions.Replace(list, 1, 3, 1, (a, b) => a == b));
            Assert.AreSequenceEqual(new[] { 3, 2, 1 }, list);
            Assert.AreEqual(1, ListExtensions.Replace(list, 1, 4, (a, b) => a == b));
            Assert.AreEqual(0, ListExtensions.Replace<int>(null, 1, 2));
        }

        [TestMethod]
        public void FlagFilteringInvokesCallbacks()
        {
            var seen = new List<int>();
            Assert.AreSequenceEqual(new[] { 1, -1, -1 }, ListExtensions.KeepWithDefault(new[] { 1, 2, 3 }, new[] { true, false }, -1, (item, keep, index) => seen.Add(index)).ToArray());
            Assert.AreSequenceEqual(new[] { 0, 1, 2 }, seen);
            seen.Clear();
            Assert.AreSequenceEqual(new[] { 1 }, ListExtensions.Keep(new[] { 1, 2, 3 }, new[] { false, true, false }, (item, keep, index) => seen.Add(index)).ToArray());
            Assert.AreSequenceEqual(new[] { 1, 2, 3 }, seen);
        }

        [TestMethod]
        public void GroupingEqualityAndContainsOnly()
        {
            var pairs = new[] { new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("a", 2) };
            Assert.AreEqual(2, pairs.ToGroups().Single().Count());
            Assert.AreSequenceEqual(new[] { 1, 2 }, ListExtensions.ToDictionary(pairs)["a"].ToArray());
            Assert.AreSequenceEqual(new[] { 1, 2 }, ListExtensions.ToDictionary(pairs.ToGroups())["a"].ToArray());
            Assert.IsNull(ListExtensions.ToGroups<string, int>(null));
            Assert.IsNull(ListExtensions.ToDictionary((IEnumerable<KeyValuePair<string, int>>)null));
            Assert.IsNull(ListExtensions.ToDictionary((IEnumerable<IGrouping<string, int>>)null));
            Assert.IsTrue(ListExtensions.Equals(new[] { 1, 2 }, new[] { 1, 2 }));
            Assert.IsFalse(ListExtensions.Equals(new[] { 1, 2 }, new[] { 2, 1 }));
            Assert.IsTrue(ListExtensions.Equals(new[] { "A" }, new[] { "a" }, StringComparer.OrdinalIgnoreCase.Equals));
            Assert.IsFalse(ListExtensions.Equals(new[] { 1 }, new[] { 1, 2 }, (a, b) => a == b));
            Assert.IsTrue(ListExtensions.Equals((IList<int>)new List<int> { 1 }, new List<int> { 1 }));
            Assert.IsFalse(ListExtensions.Equals((IList<int>)new List<int> { 1 }, new List<int> { 2 }));
            Assert.IsTrue(ListExtensions.Equals((IList<string>)new List<string> { "A" }, new List<string> { "a" }, StringComparer.OrdinalIgnoreCase.Equals));
            Assert.IsFalse(ListExtensions.Equals((IList<int>)new List<int> { 1 }, new List<int> { 1, 2 }, (a, b) => a == b));
            Assert.IsFalse(new[] { "a", "other" }.ContainsOnly(new[] { "a" }, out var matched, out var rest));
            Assert.AreSequenceEqual(new[] { "a" }, matched);
            Assert.AreSequenceEqual(new[] { "other" }, rest);
            Assert.IsTrue(new[] { "a" }.ContainsOnly(new[] { "a" }, out matched));
            Assert.IsFalse(new[] { "a" }.ContainsOnly(null, out matched));
            Assert.IsTrue(Array.Empty<string>().ContainsOnly(null, out matched, out rest));
        }

        [TestMethod]
        public void ConditionsAndSynchronizedListOverloads()
        {
            var strings = new[] { "a", "b" };
            var integers = new[] { 1, 2 };
            var longs = new[] { 1L, 2L };
            var singles = new[] { 1f, 2f };
            var doubles = new[] { 1d, 2d };
            var decimals = new[] { 1m, 2m };
            var dates = new[] { DateTime.MinValue, DateTime.MaxValue };
            Assert.AreSame(strings, ListExtensions.Where(strings, (StringCondition)null));
            Assert.AreSame(integers, ListExtensions.Where(integers, (Int32Condition)null));
            Assert.AreSame(longs, ListExtensions.Where(longs, (Int64Condition)null));
            Assert.AreSame(singles, ListExtensions.Where(singles, (SingleCondition)null));
            Assert.AreSame(doubles, ListExtensions.Where(doubles, (DoubleCondition)null));
            Assert.AreSame(decimals, ListExtensions.Where(decimals, (DecimalCondition)null));
            Assert.AreSame(dates, ListExtensions.Where(dates, (DateTimeCondition)null));
            Assert.AreEqual(2, ListExtensions.Where(integers, new Int32Condition { Value = 2 }).Single());
            var source = new List<int> { 1 };
            var copy = ListExtensions.ToSynchronizedList((IEnumerable<int>)source);
            source.Add(2);
            Assert.AreEqual(1, copy.Count);
            var shared = ListExtensions.ToSynchronizedList(source, true);
            source.Add(3);
            Assert.AreEqual(3, shared.Count);
#if NET10_0_OR_GREATER
            var root = new Lock();
#else
            var root = new object();
#endif
            var wrapped = ListExtensions.ToSynchronizedList((IEnumerable<int>)source, root);
            Assert.AreEqual(3, wrapped.Count);
            var wrappedSource = ListExtensions.ToSynchronizedList(source, root, true);
            source.Add(4);
            Assert.AreEqual(4, wrappedSource.Count);
        }
    }
}
