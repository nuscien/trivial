using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Data;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Collection
{
    [TestClass]
    public class ListExtensionsEventTest
    {
        private static async IAsyncEnumerable<ServerSentEventInfo> AsAsync(IEnumerable<ServerSentEventInfo> events)
        {
            foreach (var item in events)
            {
                yield return item;
                await Task.Yield();
            }
        }

        private static async Task<List<T>> Collect<T>(IAsyncEnumerable<T> source)
        {
            var result = new List<T>();
            await foreach (var item in source) result.Add(item);
            return result;
        }

        [TestMethod]
        public void UnionAndConciseModelConversion()
        {
            var json = JsonObjectNode.Parse("{\"items\":[\"a\",\"b\"]}");
            CollectionAssert.AreEqual(new[] { "start", "a", "b" }, ListExtensions.Union(new[] { "start" }, json, "items", false).ToArray());
            CollectionAssert.AreEqual(new[] { "start" }, ListExtensions.Union(new[] { "start" }, null, "items", false).ToArray());
            var models = new[] { new ConciseModel() };
            Assert.AreEqual(1, models.ToJsonObjectNodes().Count());
            Assert.AreEqual(1, models.ToJsonArrayNode().Count);
            Assert.AreEqual(0, ListExtensions.ToJsonObjectNodes((IEnumerable<ConciseModel>)null).Count());
            Assert.IsNull(ListExtensions.ToJsonArrayNode((IEnumerable<ConciseModel>)null));
        }

        [TestMethod]
        public void ServerSentEventsConvertAndDispatchSynchronously()
        {
            var events = new[]
            {
                new ServerSentEventInfo("1", "alpha", "{\"value\":1}"),
                new ServerSentEventInfo("2", "beta", "{\"value\":2}"),
                new ServerSentEventInfo("3", "alpha", "{\"value\":3}")
            };
            Assert.AreEqual(3, events.ToJsonObjectNodes().Count());
            Assert.AreEqual(2, events.ToJsonObjectNodes("alpha").Count());
            Assert.AreEqual(3, events.ToJsonArrayNode().Count);
            Assert.IsNull(ListExtensions.ToJsonArrayNode((IEnumerable<ServerSentEventInfo>)null));
            var called = 0;
            Assert.AreEqual(3, events.On("alpha", (Action<ServerSentEventInfo>)(_ => called++)).Count());
            Assert.AreEqual(2, called);
            var indexes = new List<int>();
            events.On("alpha", (item, index) => indexes.Add(index)).ToArray();
            CollectionAssert.AreEqual(new[] { 0, 2 }, indexes);
            events.On(new Dictionary<string, Action<ServerSentEventInfo>> { ["beta"] = _ => called++ }).ToArray();
            Assert.AreEqual(3, called);
            events.On(new Dictionary<string, Action<ServerSentEventInfo, int>> { ["beta"] = (_, index) => indexes.Add(index) }).ToArray();
            Assert.AreEqual(1, indexes.Last());
            Assert.AreEqual(0, ListExtensions.On((IEnumerable<ServerSentEventInfo>)null, "alpha", (Action<ServerSentEventInfo>)(_ => { })).Count());
        }

        [TestMethod]
        public async Task ServerSentEventsConvertAndDispatchAsynchronously()
        {
            var events = new[]
            {
                new ServerSentEventInfo("1", "alpha", "{\"value\":1}"),
                new ServerSentEventInfo("2", "beta", "{\"value\":2}")
            };
            Assert.AreEqual(2, (await Collect(AsAsync(events).ToJsonObjectNodesAsync())).Count);
            Assert.AreEqual(1, (await Collect(AsAsync(events).ToJsonObjectNodesAsync("alpha"))).Count);
            Assert.AreEqual(2, (await AsAsync(events).ToJsonArrayNodeAsync()).Count);
            Assert.IsNull(await ListExtensions.ToJsonArrayNodeAsync(null));
            var called = 0;
            await Collect(AsAsync(events).OnAsync("alpha", (Action<ServerSentEventInfo>)(_ => called++)));
            Assert.AreEqual(1, called);
            var indexes = new List<int>();
            await Collect(AsAsync(events).OnAsync("beta", (item, index) => indexes.Add(index)));
            CollectionAssert.AreEqual(new[] { 1 }, indexes);
            await Collect(AsAsync(events).OnAsync(new Dictionary<string, Action<ServerSentEventInfo>> { ["beta"] = _ => called++ }));
            await Collect(AsAsync(events).OnAsync(new Dictionary<string, Action<ServerSentEventInfo, int>> { ["alpha"] = (_, index) => indexes.Add(index) }));
            Assert.AreEqual(2, called);
            Assert.AreEqual(0, indexes.Last());
            Assert.AreEqual(2, await AsAsync(events).ProcessAsync());
            Assert.AreEqual(2, await AsAsync(events).ProcessAsync((item, index) => indexes.Add(index)));
            Assert.AreEqual(1, indexes.Last());
            Assert.AreEqual(0, await ListExtensions.ProcessAsync((IAsyncEnumerable<ServerSentEventInfo>)null));
        }
    }
}
