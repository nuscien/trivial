using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Tasks;

/// <summary>
/// Hit task unit test.
/// </summary>
[TestClass]
public class InterceptorUnitTest
{
    /// <summary>
    /// Tests debounce hit task.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [TestMethod]
    public async Task TestDebounceAsync()
    {
        var result = string.Empty;
        var task = new Interceptor<string>(
            v => result = v,
            InterceptorPolicy.Debounce(TimeSpan.FromMilliseconds(100))
            );
        Assert.IsFalse(task.IsWorking);
        _ = task.InvokeAsync("abc");
        Assert.IsTrue(task.IsWorking);
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("defg");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("hijk");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("lmn");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        await Task.Delay(90);
        if (string.IsNullOrEmpty(result)) await Task.Delay(20);
        Assert.AreEqual("lmn", result);
        await task.WaitAsync();
        Assert.IsFalse(task.IsWorking);
    }

    /// <summary>
    /// Tests throttle hit task.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [TestMethod]
    public async Task TestThrottleAsync()
    {
        var taskTokens = new List<Task>();
        var result = string.Empty;
        var task = new Interceptor<string>(
            v => result = v,
            InterceptorPolicy.Throttle(TimeSpan.FromMilliseconds(100))
            );
        Assert.IsFalse(task.IsWorking);
        _ = task.InvokeAsync("opq");
        await Task.Delay(10);
        Assert.AreEqual("opq", result);
        _ = task.InvokeAsync("rst");
        await Task.Delay(10);
        Assert.AreEqual("opq", result);
        _ = task.InvokeAsync("uvw");
        await Task.Delay(110);
        Assert.AreEqual("opq", result);
        _ = task.InvokeAsync("xyz");
        Assert.AreEqual("xyz", result);
        await task.WaitAsync();
        Assert.IsFalse(task.IsWorking);
    }

    /// <summary>
    /// Tests multiple hit task.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [TestMethod]
    public async Task TestMutlipleHitsAsync()
    {
        var taskTokens = new List<Task>();
        var result = string.Empty;
        var task = new Interceptor<string>(
            v => result = v,
            InterceptorPolicy.Mutliple(2, 4, TimeSpan.FromMilliseconds(100))
            );
        Assert.IsFalse(task.IsWorking);
        _ = task.InvokeAsync("abc");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("defg");
        await Task.Delay(20);
        Assert.AreEqual("defg", result);
        _ = task.InvokeAsync("hijk");
        await Task.Delay(20);
        Assert.AreEqual("hijk", result);
        _ = task.InvokeAsync("lmn");
        await Task.Delay(20);
        Assert.AreEqual("lmn", result);
        _ = task.InvokeAsync("opq");
        await Task.Delay(110);
        Assert.AreEqual("lmn", result);
        _ = task.InvokeAsync("rst");
        await Task.Delay(20);
        Assert.AreEqual("lmn", result);
        _ = task.InvokeAsync("uvw");
        await Task.Delay(20);
        Assert.AreEqual("uvw", result);
        _ = task.InvokeAsync("xyz");
        await Task.Delay(20);
        Assert.AreEqual("xyz", result);
        await task.WaitAsync();
        Assert.IsFalse(task.IsWorking);
    }

    /// <summary>
    /// Tests times hit task.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [TestMethod]
    public async Task TestTimesHitsAsync()
    {
        var taskTokens = new List<Task>();
        var result = string.Empty;
        var task = new Interceptor<string>(
            v => result = v,
            InterceptorPolicy.Times(2, 3, TimeSpan.FromMilliseconds(100))
            );
        Assert.IsFalse(task.IsWorking);
        _ = task.InvokeAsync("abc");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("defg");
        Assert.IsTrue(task.IsWorking);
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("hijk");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("lmn");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        await Task.Delay(90);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("opq");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        _ = task.InvokeAsync("rst");
        await Task.Delay(20);
        Assert.AreEqual(string.Empty, result);
        await Task.Delay(90);
        if (string.IsNullOrEmpty(result)) await Task.Delay(20);
        Assert.AreEqual("rst", result);
        _ = task.InvokeAsync("uvw");
        await Task.Delay(20);
        Assert.AreEqual("rst", result);
        _ = task.InvokeAsync("xyz");
        await Task.Delay(140);
        if (result == "rst") await Task.Delay(20);
        Assert.AreEqual("xyz", result);
        await task.WaitAsync();
        Assert.IsFalse(task.IsWorking);
    }
}
