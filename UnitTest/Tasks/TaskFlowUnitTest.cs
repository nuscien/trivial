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
/// Task flow unit test.
/// </summary>
[TestClass]
public class TaskFlowUnitTest
{
    /// <summary>
    /// Tests equipment task.
    /// </summary>
    [TestMethod]
    public async Task TestTaskAsync()
    {
        var task = Task.Run(async () =>
        {
            await Task.Delay(100);
            return 100;
        });
        var flow1 = new TaskFlow<int>(task);
        flow1.ResultTo(r =>
        {
            Assert.AreEqual(100, r);
        });
        flow1.ResultTo(r =>
        {
            Assert.AreEqual(100, r);
        });
        flow1.Catch<InvalidOperationException>(ex =>
        {
            Assert.Fail("It should not throw any exception.");
        });
        var flow2 = flow1.Then(r =>
        {
            throw new InvalidOperationException("test");
            #pragma warning disable CS0162
            return string.Empty;
            #pragma warning restore CS0162
        });
        flow2.Catch<InvalidOperationException>(ex =>
        {
            Assert.AreEqual("test", ex.Message);
        });
        flow2.ResultTo(r =>
        {
            Assert.Fail("It should not return any result.");
        });
        flow2.Catch<InvalidOperationException>(ex =>
        {
            Assert.AreEqual("test", ex.Message);
        });
        Assert.AreEqual(100, await flow1.ResultAsync());
        try
        {
            await flow2.WaitAsync();
            Assert.Fail("It should throw an exception.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("test", ex.Message);
        }
    }
}
