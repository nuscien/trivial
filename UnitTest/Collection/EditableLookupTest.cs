using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Collection
{
    [TestClass]
    public class EditableLookupTest
    {
        [TestMethod]
        public void AddItemOverloadsAppendAndReplaceNullGroups()
        {
            var lookup = new EditableLookup<string, int>();
            lookup.AddItem("a", 1);
            lookup.AddItem("a", new[] { 2, 3 });
            lookup.AddItem(new KeyValuePair<string, int>("a", 4));
            lookup.AddItem(new KeyValuePair<string, IEnumerable<int>>("a", new[] { 5, 6 }));
            lookup.AddItem(new[] { new KeyValuePair<string, int>("a", 7) }.GroupBy(p => p.Key, p => p.Value).Single());
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 }, lookup["a"]);

            lookup["b"] = null;
            lookup.AddItem("b", 8);
            lookup["c"] = null;
            lookup.AddItem("c", new[] { 9 });
            lookup["d"] = null;
            lookup.AddItem(new KeyValuePair<string, int>("d", 10));
            lookup["e"] = null;
            lookup.AddItem(new KeyValuePair<string, IEnumerable<int>>("e", new[] { 11 }));
            lookup["f"] = null;
            lookup.AddItem(new[] { 12 }.GroupBy(x => "f").Single());
            Assert.AreEqual(8, lookup["b"].Single());
            Assert.AreEqual(9, lookup["c"].Single());
            Assert.AreEqual(10, lookup["d"].Single());
            Assert.AreEqual(11, lookup["e"].Single());
            Assert.AreEqual(12, lookup["f"].Single());
            Assert.IsNull(lookup.TryGetValue("missing"));
            Assert.AreSame(lookup["a"], lookup.TryGetValue("a"));
        }

        [TestMethod]
        public void LookupInterfaceUsesGroupsAndComparer()
        {
            var lookup = new EditableLookup<string, int>(StringComparer.OrdinalIgnoreCase);
            lookup.AddItem("Apple", 1);
            lookup.AddItem("apple", 2);
            ILookup<string, int> view = lookup;
            Assert.IsTrue(view.Contains("APPLE"));
            Assert.IsFalse(view.Contains("other"));
            CollectionAssert.AreEqual(new[] { 1, 2 }, view["APPLE"].ToArray());
            var groups = view.ToArray();
            Assert.AreEqual(1, groups.Length);
            Assert.AreEqual("Apple", groups[0].Key);
            CollectionAssert.AreEqual(new[] { 1, 2 }, groups[0].ToArray());
        }

        [TestMethod]
        public void ToEditableLookupGroupsAllInputForms()
        {
            var source = new[] { "a", "A", "bb" };
            var basic = source.ToEditableLookup(x => x.Length);
            CollectionAssert.AreEqual(new[] { "a", "A" }, basic[1]);
            var compared = source.ToEditableLookup(x => x, StringComparer.OrdinalIgnoreCase);
            CollectionAssert.AreEqual(new[] { "a", "A" }, compared["A"]);

            var pairs = new[] { new KeyValuePair<string, int>("x", 1), new KeyValuePair<string, int>("x", 2) };
            CollectionAssert.AreEqual(new[] { 1, 2 }, pairs.ToEditableLookup()["x"]);
            var batches = new[]
            {
                new KeyValuePair<string, IEnumerable<int>>("x", new[] { 1, 2 }),
                new KeyValuePair<string, IEnumerable<int>>("x", new[] { 3 })
            };
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, batches.ToEditableLookup()["x"]);

            Assert.IsNull(ListExtensions.ToEditableLookup<int, string>((IEnumerable<string>)null, x => x.Length));
            Assert.IsNull(ListExtensions.ToEditableLookup<int, string>((IEnumerable<string>)null, x => x.Length, EqualityComparer<int>.Default));
            Assert.IsNull(ListExtensions.ToEditableLookup<string, int>((IEnumerable<KeyValuePair<string, int>>)null));
            Assert.IsNull(ListExtensions.ToEditableLookup<string, int>((IEnumerable<KeyValuePair<string, IEnumerable<int>>>)null));
        }
    }
}
