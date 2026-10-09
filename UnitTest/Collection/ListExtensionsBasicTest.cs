using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Data;
using Trivial.Text;

namespace Trivial.Collection
{
    [TestClass]
    public class ListExtensionsBasicTest
    {
        private sealed class Item : IIdPropertyModel, INamePropertyModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void StringDictionaryAndConsoleTextOverloads()
        {
            IDictionary<string, string> dict = new Dictionary<string, string> { ["empty"] = "", ["full"] = "value" };
            Assert.IsTrue(dict.TryGetNotEmptyValue("full", out var value));
            Assert.AreEqual("value", value);
            Assert.IsFalse(dict.TryGetNotEmptyValue("empty", out value));
            Assert.IsNull(value);
            Assert.IsNull(dict.TryGetNotEmptyValue("missing"));
            Assert.AreEqual("value", dict.TryGetNotEmptyValue("full"));
            Assert.ThrowsExactly<ArgumentNullException>(() => ListExtensions.TryGetNotEmptyValue(null, "full"));

            IList<Trivial.CommandLine.ConsoleText> texts = new List<Trivial.CommandLine.ConsoleText>();
            texts.Add("text");
            texts.Add(new StringBuilder("builder"));
            texts.Add('x', 3);
            Assert.AreEqual(3, texts.Count);
        }

        [TestMethod]
        public void PaddingAndBooleanSelection()
        {
            CollectionAssert.AreEqual(new[] { 0, 0, 1, 2 }, ListExtensions.PadBegin(new[] { 1, 2 }, 2));
            CollectionAssert.AreEqual(new[] { 1, 2, 9, 9 }, ListExtensions.PadEnd(new[] { 1, 2 }, 2, 9));
            CollectionAssert.AreEqual(new[] { 1, 2 }, ListExtensions.PadBegin(new[] { 1, 2 }, -1));
            CollectionAssert.AreEqual(new[] { 1, 2, 1, 2 }, ListExtensions.PadEnd(new[] { 1, 2 }, -1));
            CollectionAssert.AreEqual(new[] { 7, 7 }, ListExtensions.PadBegin<int>(null, 2, 7));
            CollectionAssert.AreEqual(new[] { 7 }, ListExtensions.PadEnd<int>(null, 1, 7));
            IList<int> list = new List<int> { 1, 2 };
            ListExtensions.PadBeginTo(list, 2, 9);
            ListExtensions.PadEndTo(list, 1, 8);
            CollectionAssert.AreEqual(new[] { 9, 9, 1, 2, 8 }, list.ToArray());
            Assert.ThrowsExactly<ArgumentNullException>(() => ListExtensions.PadBeginTo<int>(null, 1));
            Assert.ThrowsExactly<ArgumentNullException>(() => ListExtensions.PadEndTo<int>(null, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ListExtensions.PadEndTo(new HashSet<int>(), -1));
            CollectionAssert.AreEqual(new[] { "yes", "no" }, ListExtensions.Select((IEnumerable<bool>)new List<bool> { true, false }, "yes", "no").ToArray());
            CollectionAssert.AreEqual(new[] { 1, 0 }, ListExtensions.Select(new[] { true, false }, 1, 0));
            Assert.IsNull(ListExtensions.Select((bool[])null, 1, 0));
            Assert.AreEqual(0, ListExtensions.Select((IEnumerable<bool>)null, 1, 0).Count());
        }

        [TestMethod]
        public void KeyValuePairOperations()
        {
            var pairs = new List<KeyValuePair<string, int>>();
            pairs.Add("a", 1);
            pairs.Add("a", 2);
            pairs.Insert(1, "b", 3);
            CollectionAssert.AreEqual(new[] { "a", "b" }, pairs.Keys().ToArray());
            CollectionAssert.AreEqual(new[] { 1, 2 }, pairs.GetValues("a").ToArray());
            Assert.AreEqual(2, pairs.GetValue("a", 1));
            Assert.AreEqual(3, pairs.Get("b").Single());
            Assert.IsNull(pairs.Get("absent"));
            Assert.IsTrue(pairs.ContainsKey("a"));
            Assert.IsFalse(pairs.ContainsKey("absent"));
            Assert.AreEqual(0, pairs.IndexOf("a"));
            Assert.AreEqual(0, pairs.LastIndexOf("a"));
            Assert.IsTrue(pairs.TryGetValue("a", 1, out var value));
            Assert.AreEqual(2, value);
            Assert.IsFalse(pairs.TryGetValue("a", 3, out value));
            Assert.AreEqual(0, value);
            pairs.Add("b", 4, true);
            CollectionAssert.AreEqual(new[] { 4 }, pairs.GetValues("b").ToArray());
            Assert.AreEqual(2, pairs.Remove(new[] { "a" }));
            Assert.AreEqual(1, pairs.Count);
            Assert.AreEqual(1, pairs.Remove((ReadOnlySpan<string>)new[] { "b" }));
            Assert.AreEqual(0, pairs.Count);
            var nested = new[] { new KeyValuePair<string, int[]>("x", new[] { 1, 2 }), new KeyValuePair<string, int[]>("x", new[] { 3 }) };
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, nested.GetValueItems<string, int, int[]>("x").ToArray());
            Assert.ThrowsExactly<ArgumentNullException>(() => ListExtensions.Keys((IEnumerable<KeyValuePair<string, int>>)null));
        }

        [TestMethod]
        public void ModelLookupsSkipNullsAndReturnDefaults()
        {
            var first = new Item { Id = "1", Name = "A" };
            var second = new Item { Id = "1", Name = "B" };
            var items = new[] { null, first, second };
            CollectionAssert.AreEqual(new[] { first, second }, ListExtensions.GetAllById(items, "1").ToArray());
            Assert.AreSame(first, ListExtensions.GetByIdOrDefault(items, "1"));
            Assert.IsTrue(ListExtensions.TryGetById(items, "1", out var found));
            Assert.AreSame(first, found);
            Assert.IsFalse(ListExtensions.TryGetById(items, "missing", out found));
            Assert.IsNull(found);
            Assert.AreEqual(0, ListExtensions.GetAllById<Item>(null, "1").Count());
            CollectionAssert.AreEqual(new[] { second }, ListExtensions.GetAllByName(items, "B").ToArray());
            Assert.AreSame(second, ListExtensions.GetByNameOrDefault(items, "B"));
            Assert.IsTrue(ListExtensions.TryGetByName(items, "B", out found));
            Assert.AreSame(second, found);
            Assert.IsFalse(ListExtensions.TryGetByName<Item>(null, "B", out found));
            Assert.IsNull(found);
            Assert.AreEqual(0, ListExtensions.GetAllByName<Item>(null, "B").Count());
        }
    }
}
