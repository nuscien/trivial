using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.Net;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// HTTP URI unit test.
/// </summary>
[TestClass]
public class DataResultUnitTest
{
    /// <summary>
    /// Tests server-sent event.
    /// </summary>
    [TestMethod]
    public async Task TestServerSentEventAsync()
    {
        using var stream = new MemoryStream();
        using var builder = new CollectionResultWriter<JsonObjectNode>(stream);
        Assert.IsFalse(builder.IsEnd);

        // 1
        builder.Add(new()
        {
            { "index", 0 },
            { "value", "abc" }
        });
        Assert.AreEqual(1, builder.Result.CurrentCount);
        Assert.IsNull(builder.Result.Offset);
        Assert.AreEqual(0, builder.Result.Data[0].TryGetInt32Value("index"));

        // 2
        builder.SetOffset(0, 10);
        Assert.IsNull(builder.Result.Message);
        Assert.AreEqual(0, builder.Result.Offset);
        Assert.AreEqual(10, builder.Result.TotalCount);

        // 3
        builder.PatchAdditionalInfo(new()
        {
            { "name", "Test data" }
        });
        Assert.AreEqual("Test data", builder.Result.AdditionalInfo.GetStringValue("name"));

        // 4
        builder.Add(new()
        {
            { "index", 1 },
            { "value", "defg" }
        });
        Assert.AreEqual(2, builder.Result.CurrentCount);
        Assert.AreEqual(0, builder.Result.Data[0].TryGetInt32Value("index"));
        Assert.AreEqual(1, builder.Result.Data[1].TryGetInt32Value("index"));

        // 5
        builder.ClearItems();
        Assert.AreEqual(0, builder.Result.CurrentCount);

        // 6
        builder.ClearAdditionalInfo();
        Assert.AreEqual(0, builder.Result.AdditionalInfo.Count);

        // 7
        builder.AddRange(new List<JsonObjectNode>()
        {
            new(),
            null,
            new()
            {
                { "char", "hijklmn " }
            },
        });
        Assert.AreEqual(3, builder.Result.CurrentCount);
        Assert.AreEqual(0, builder.Result.Data[0].Count);
        Assert.IsNull(builder.Result.Data[1]);
        Assert.AreEqual("hijklmn", builder.Result.Data[2].TryGetStringTrimmedValue("char"));

        // 8
        builder.SetMessage("OK!");
        Assert.AreEqual("OK!", builder.Result.Message);
        Assert.IsFalse(builder.IsEnd);

        // 9
        builder.End();
        Assert.IsTrue(builder.IsEnd);

        // Transfer
        stream.Seek(0, SeekOrigin.Begin);
        var result = new StreamingCollectionResult<JsonObjectNode>(stream, AssertResult);
        //Assert.AreEqual(TaskStates.Working, result.State);
        await result.WaitAsync();
        Assert.AreEqual(TaskStates.Done, result.State);
        Assert.AreEqual(8, result.Tag);
        builder.Dispose();
    }

    private static void AssertResult(StreamingCollectionResult<JsonObjectNode> result, ServerSentEventInfo info)
    {
        if (result.Tag is not int i)
        {
            if (result.Tag is not null) Assert.Fail();
            i = 0;
        }

        i++;
        if (i != result.EventItemCount) Assert.Fail();
        result.Tag = result.EventItemCount;
        switch (i)
        {
            case 0:
                Assert.Fail();
                break;
            case 1:
                Assert.AreEqual(TaskStates.Working, result.State);
                Assert.AreEqual(1, result.CurrentCount);
                Assert.IsNull(result.Offset);
                Assert.AreEqual(0, result.Data[0].TryGetInt32Value("index"));
                break;
            case 2:
                Assert.IsNull(result.Message);
                Assert.AreEqual(0, result.Offset);
                Assert.AreEqual(10, result.TotalCount);
                break;
            case 3:
                Assert.AreEqual("Test data", result.AdditionalInfo.GetStringValue("name"));
                break;
            case 4:
                Assert.AreEqual(2, result.CurrentCount);
                Assert.AreEqual(0, result.Data[0].TryGetInt32Value("index"));
                Assert.AreEqual(1, result.Data[1].TryGetInt32Value("index"));
                break;
            case 5:
                Assert.AreEqual(0, result.CurrentCount);
                break;
            case 6:
                Assert.AreEqual(0, result.AdditionalInfo.Count);
                break;
            case 7:
                Assert.AreEqual(3, result.CurrentCount);
                Assert.AreEqual(0, result.Data[0].Count);
                Assert.IsNull(result.Data[1]);
                Assert.AreEqual("hijklmn", result.Data[2].TryGetStringTrimmedValue("char"));
                break;
            case 8:
                Assert.AreEqual("OK!", result.Message);
                Assert.AreEqual(TaskStates.Working, result.State);
                break;
            case 9:
                Assert.Fail();
                break;
        }
    }
}
