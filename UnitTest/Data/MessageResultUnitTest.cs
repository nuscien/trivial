using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Reflection;

namespace Trivial.Data;

[TestClass]
public class MessageResultUnitTest
{
    public class SampleItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void DataResultConstructorsAndAccessors()
    {
        var empty = new DataResult<SampleItem>();
        Assert.IsNull(empty.Data);
        Assert.IsTrue(empty.IsNull);
        Assert.AreEqual(typeof(SampleItem), empty.GetDataType());
        Assert.AreEqual("null", empty.SerializeData());

        var item = new SampleItem { Id = 7, Name = "alpha" };
        var result = new DataResult<SampleItem>(item, "ready");
        Assert.AreSame(item, result.Data);
        Assert.AreEqual("ready", result.Message);
        Assert.IsFalse(result.IsNull);
        Assert.AreEqual(typeof(SampleItem), result.GetDataType());
        Assert.IsTrue(((IObjectRef)result).IsValueCreated);
        Assert.AreSame(item, ((IObjectRef)result).Value);
        Assert.AreSame(item, ((IObjectRef<SampleItem>)result).Value);
        using var dataJson = JsonDocument.Parse(result.SerializeData());
        Assert.AreEqual(7, dataJson.RootElement.GetProperty("Id").GetInt32());
        Assert.AreEqual("alpha", dataJson.RootElement.GetProperty("Name").GetString());
        Assert.IsFalse(dataJson.RootElement.TryGetProperty("data", out _));

        var polymorphic = new DataResult<object>(item);
        Assert.AreEqual(typeof(SampleItem), polymorphic.GetDataType());
        var fromEvent = new DataResult<SampleItem>(new DataEventArgs<SampleItem>(item, "event"));
        Assert.AreSame(item, fromEvent.Data);
        Assert.AreEqual("event", fromEvent.Message);
        var nullEvent = new DataResult<SampleItem>((DataEventArgs<SampleItem>)null);
        Assert.IsNull(nullEvent.Data);
        Assert.IsNull(nullEvent.Message);
    }

    [TestMethod]
    public void DataResultJsonRoundTripAndClientResponse()
    {
        var result = new DataResult<SampleItem>(new SampleItem { Id = 7, Name = "alpha" }, "ready")
        {
            ErrorCode = "not_found",
            TrackingId = "trace-1",
            Tag = "local only"
        };
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(result));
        var root = document.RootElement;
        Assert.AreEqual(7, root.GetProperty("data").GetProperty("Id").GetInt32());
        Assert.AreEqual("alpha", root.GetProperty("data").GetProperty("Name").GetString());
        Assert.AreEqual("ready", root.GetProperty("message").GetString());
        Assert.AreEqual("not_found", root.GetProperty("error").GetString());
        Assert.AreEqual("trace-1", root.GetProperty("track").GetString());
        Assert.IsFalse(root.TryGetProperty("IsNull", out _));
        Assert.IsFalse(root.TryGetProperty("IsError", out _));
        Assert.IsFalse(root.TryGetProperty("Tag", out _));

        var copy = JsonSerializer.Deserialize<DataResult<SampleItem>>(root.GetRawText());
        Assert.AreEqual(7, copy.Data.Id);
        Assert.AreEqual("alpha", copy.Data.Name);
        Assert.AreEqual("ready", copy.Message);
        Assert.AreEqual("not_found", copy.ErrorCode);
        Assert.AreEqual("trace-1", copy.TrackingId);
        Assert.IsTrue(copy.IsError);
        Assert.IsNull(copy.Tag);

        var client = JsonSerializer.Deserialize<DataResult<SampleItem>>("{\"data\":{\"Id\":9,\"Name\":\"beta\"},\"message\":\"ok\",\"extra\":{\"version\":2}}");
        Assert.AreEqual(9, client.Data.Id);
        Assert.AreEqual("beta", client.Data.Name);
        Assert.AreEqual("ok", client.Message);
        Assert.AreEqual(2, client.AdditionalProperties["extra"].GetProperty("version").GetInt32());
        using var clientJson = JsonDocument.Parse(JsonSerializer.Serialize(client));
        Assert.AreEqual(2, clientJson.RootElement.GetProperty("extra").GetProperty("version").GetInt32());
        Assert.IsFalse(clientJson.RootElement.TryGetProperty("error", out _));
        Assert.IsFalse(clientJson.RootElement.TryGetProperty("track", out _));

        using var emptyJson = JsonDocument.Parse(JsonSerializer.Serialize(new DataResult<SampleItem>()));
        Assert.AreEqual(JsonValueKind.Null, emptyJson.RootElement.GetProperty("data").ValueKind);
        Assert.IsFalse(emptyJson.RootElement.TryGetProperty("message", out _));
    }

    [TestMethod]
    public void MessageResultErrorAndTrackingBehavior()
    {
        var message = new MessageResult("notice");
        Assert.AreEqual("notice", message.Message);
        Assert.IsFalse(message.IsError);
        message.ErrorCode = "  ";
        Assert.IsFalse(message.IsError);
        message.ErrorCode = "failure";
        Assert.IsTrue(message.IsError);
        var id = Guid.Parse("12ab34cd-5678-90ab-cdef-123456789012");
        message.CreateNewTrackingId(id);
        Assert.AreEqual("12ab34cd567890abcdef123456789012", message.TrackingId);
        message.CreateNewTrackingId();
        Assert.IsTrue(Guid.TryParseExact(message.TrackingId, "N", out _));

        MessageResult objectResult = new DataResult<int>(42);
        MessageResult collectionResult = new CollectionResult<int>(new[] { 42 });
        objectResult.ErrorCode = "object_error";
        collectionResult.ErrorCode = "collection_error";
        Assert.IsTrue(objectResult.IsError);
        Assert.IsTrue(collectionResult.IsError);
        using var collectionJson = JsonDocument.Parse(JsonSerializer.Serialize((CollectionResult<int>)collectionResult));
        Assert.AreEqual("collection_error", collectionJson.RootElement.GetProperty("error").GetString());
    }

    [TestMethod]
    public void ConstructorsAndCollectionOperations()
    {
        var empty = new CollectionResult<int>();
        Assert.IsNull(empty.Data);
        Assert.AreEqual(0, empty.CurrentCount);
        Assert.IsNull(empty.Offset);
        Assert.IsNull(empty.TotalCount);
        empty.AddItem(1);
        empty.AddRange(new[] { 2, 3 });
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, empty.Data.ToArray());
        Assert.AreEqual(3, empty.CurrentCount);

        var source = new[] { 4, 5 };
        var complete = new CollectionResult<int>(source, all: true, offset: 2);
        source[0] = 99;
        CollectionAssert.AreEqual(new[] { 4, 5 }, complete.Data.ToArray());
        Assert.AreEqual(2, complete.Offset);
        Assert.AreEqual(4, complete.TotalCount);
        Assert.IsNull(new CollectionResult<int>(source, all: false).TotalCount);
        Assert.AreEqual(0, new CollectionResult<int>((int[])null, all: true).CurrentCount);

        var observable = new ObservableCollection<int> { 7 };
        var shared = new CollectionResult<int>(observable);
        Assert.AreSame(observable, shared.Data);
        observable.Add(8);
        Assert.AreEqual(2, shared.CurrentCount);
    }

    [TestMethod]
    public void JsonRoundTripPreservesCollectionAndMetadata()
    {
        var result = new CollectionResult<int>("trace-1", new[] { 10, 20 }, 2, 6, "partial");
        result.AdditionalInfo = new() { { "source", "example" } };
        var json = JsonSerializer.Serialize(result);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        CollectionAssert.AreEqual(new[] { 10, 20 }, root.GetProperty("items").EnumerateArray().Select(item => item.GetInt32()).ToArray());
        Assert.AreEqual(2, root.GetProperty("offset").GetInt32());
        Assert.AreEqual(6, root.GetProperty("count").GetInt32());
        Assert.AreEqual("partial", root.GetProperty("message").GetString());
        Assert.AreEqual("trace-1", root.GetProperty("track").GetString());
        Assert.AreEqual("example", root.GetProperty("info").GetProperty("source").GetString());
        Assert.IsFalse(root.TryGetProperty("CurrentCount", out _));
        Assert.IsFalse(root.TryGetProperty("Data", out _));

        var copy = JsonSerializer.Deserialize<CollectionResult<int>>(json);
        CollectionAssert.AreEqual(new[] { 10, 20 }, copy.Data.ToArray());
        Assert.AreEqual(2, copy.Offset);
        Assert.AreEqual(6, copy.TotalCount);
        Assert.AreEqual("partial", copy.Message);
        Assert.AreEqual("trace-1", copy.TrackingId);
        Assert.AreEqual("example", copy.AdditionalInfo.GetStringValue("source"));
    }

    [TestMethod]
    public void ClientDeserializesUnpagedAndPagedResponses()
    {
        var unpaged = JsonSerializer.Deserialize<CollectionResult<string>>("{\"items\":[\"a\",\"b\"]}");
        CollectionAssert.AreEqual(new[] { "a", "b" }, unpaged.Data.ToArray());
        Assert.IsNull(unpaged.Offset);
        Assert.IsNull(unpaged.TotalCount);
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(unpaged));
        Assert.IsFalse(document.RootElement.TryGetProperty("offset", out _));
        Assert.IsFalse(document.RootElement.TryGetProperty("count", out _));

        var page = JsonSerializer.Deserialize<CollectionResult<int>>("{\"items\":[30,40],\"offset\":2,\"count\":5,\"extra\":true}");
        CollectionAssert.AreEqual(new[] { 30, 40 }, page.Data.ToArray());
        Assert.AreEqual(2, page.Offset);
        Assert.AreEqual(5, page.TotalCount);
        Assert.IsTrue(page.AdditionalProperties["extra"].GetBoolean());
        Assert.IsTrue(page.TryGetValue(2, true, out var item));
        Assert.AreEqual(30, item);
        Assert.IsFalse(page.TryGetValue(1, true, out item));
        Assert.AreEqual(0, item);
        Assert.IsFalse(page.TryGetValue(2, out item));
    }

    [TestMethod]
    public void AddRangeAppendsContiguousPagesAndPreservesMetadata()
    {
        var first = new CollectionResult<int>("first-trace", new[] { 1, 2 }, 0, 5, "first");
        var next = new CollectionResult<int>("next-trace", new[] { 3, 4 }, 2, 7, "next");
        first.AddRange(next);
        first.AddRange(new CollectionResult<int>(new[] { 5 }, offset: 4, count: 5));
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, first.Data.ToArray());
        CollectionAssert.AreEqual(new[] { 3, 4 }, next.Data.ToArray());
        Assert.AreEqual(0, first.Offset);
        Assert.AreEqual(5, first.TotalCount);
        Assert.AreEqual("first", first.Message);
        Assert.AreEqual("first-trace", first.TrackingId);
        Assert.AreEqual(5, first.CurrentCount);

        var later = new CollectionResult<int>(new[] { 3, 4 }, offset: 2, count: 6);
        later.AddRange(new CollectionResult<int>(new[] { 5, 6 }, offset: 4, count: 6));
        CollectionAssert.AreEqual(new[] { 3, 4, 5, 6 }, later.Data.ToArray());
        Assert.AreEqual(2, later.Offset);
        Assert.AreEqual(6, later.TotalCount);
        Assert.AreEqual(5, later.TryGetValue(4, startFromOffset: true));
    }

    [TestMethod]
    public void AddRangeReplacesOverlapsAndSupportsEarlierPages()
    {
        var result = new CollectionResult<string>(new[] { "c", "d", "e" }, offset: 2, count: 6);
        result.AddRange(new CollectionResult<string>(new[] { "D", "E", "f" }, offset: 3));
        CollectionAssert.AreEqual(new[] { "c", "D", "E", "f" }, result.Data.ToArray());
        result.AddRange(new CollectionResult<string>(new[] { "a", "b", "C" }, offset: 0));
        CollectionAssert.AreEqual(new[] { "a", "b", "C", "D", "E", "f" }, result.Data.ToArray());
        Assert.AreEqual(0, result.Offset);
        Assert.AreEqual(6, result.TotalCount);
    }

    [TestMethod]
    public void AddRangeFillsSkippedPositionsAndAllowsLaterReplacement()
    {
        var result = new CollectionResult<int>(new[] { 3, 4 }, offset: 2, count: 8);
        result.AddRange(new CollectionResult<int>(new[] { 7, 8 }, offset: 6));
        CollectionAssert.AreEqual(new[] { 3, 4, 0, 0, 7, 8 }, result.Data.ToArray());
        Assert.AreEqual(2, result.Offset);
        Assert.AreEqual(8, result.TotalCount);
        Assert.AreEqual(0, result.TryGetValue(4, startFromOffset: true));
        Assert.AreEqual(7, result.TryGetValue(6, startFromOffset: true));

        result.AddRange(new CollectionResult<int>(new[] { 5, 6 }, offset: 4));
        CollectionAssert.AreEqual(new[] { 3, 4, 5, 6, 7, 8 }, result.Data.ToArray());
    }

    [TestMethod]
    public void AddRangeFillsSkippedPositionsBeforeExistingPage()
    {
        var result = new CollectionResult<string>(new[] { "e", "f" }, offset: 4, count: 6);
        result.AddRange(new CollectionResult<string>(new[] { "a", "b" }, offset: 0));
        CollectionAssert.AreEqual(new[] { "a", "b", null, null, "e", "f" }, result.Data.ToArray());
        Assert.AreEqual(0, result.Offset);
        Assert.AreEqual(6, result.TotalCount);
        Assert.IsTrue(result.TryGetValue(2, startFromOffset: true, out var missing));
        Assert.IsNull(missing);
        Assert.AreEqual("e", result.TryGetValue(4, startFromOffset: true));
    }

    [TestMethod]
    public void AddRangeHandlesMissingPageAndMissingOffset()
    {
        var result = new CollectionResult<int>();
        result.AddRange((CollectionResult<int>)null);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(0, result.CurrentCount);
        result.AddRange(new CollectionResult<int>());
        Assert.AreEqual(0, result.CurrentCount);
        result.AddRange(new CollectionResult<int>(new[] { 1, 2 }));
        result.AddRange(new CollectionResult<int>(new[] { 3 }));
        result.AddRange(new CollectionResult<int>(new int[0], offset: 3));
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.Data.ToArray());
        Assert.IsNull(result.Offset);
        Assert.IsNull(result.TotalCount);

        var later = new CollectionResult<int>(new[] { 3, 4 }, offset: 2, count: 6);
        later.AddRange(new CollectionResult<int>(new[] { 5, 6 }));
        CollectionAssert.AreEqual(new[] { 3, 4, 5, 6 }, later.Data.ToArray());
        Assert.AreEqual(2, later.Offset);
        Assert.AreEqual(6, later.TotalCount);
        later.AddRange(new CollectionResult<int>(new int[0], offset: 10));
        CollectionAssert.AreEqual(new[] { 3, 4, 5, 6 }, later.Data.ToArray());
    }
}
