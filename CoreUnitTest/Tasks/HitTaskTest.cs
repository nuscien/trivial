﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Tasks;

namespace Trivial.UnitTest.Tasks
{
    [TestClass]
    public class HitTaskTest
    {
        [TestMethod]
        public async Task TestDebounceAsync()
        {
            var taskTokens = new List<Task>();
            var result = string.Empty;
            var task = HitTask.Debounce((HitTask<string> sender, HitEventArgs<string> ev) =>
            {
                result = ev.Argument;
            }, TimeSpan.FromMilliseconds(100));
            _ = ProcessHit(taskTokens, task, "abc");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "defg");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "hijk");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "lmn");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            await Task.Delay(90);
            Assert.AreEqual("lmn", result);
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
        }

        [TestMethod]
        public async Task TestThrottleAsync()
        {
            var taskTokens = new List<Task>();
            var result = string.Empty;
            var task = HitTask.Throttle((HitTask<string> sender, HitEventArgs<string> ev) =>
            {
                result = ev.Argument;
            }, TimeSpan.FromMilliseconds(100));
            _ = ProcessHit(taskTokens, task, "opq");
            await Task.Delay(20);
            Assert.AreEqual("opq", result);
            _ = ProcessHit(taskTokens, task, "rst");
            await Task.Delay(20);
            Assert.AreEqual("opq", result);
            _ = ProcessHit(taskTokens, task, "uvw");
            await Task.Delay(110);
            Assert.AreEqual("opq", result);
            _ = ProcessHit(taskTokens, task, "xyz");
            Assert.AreEqual("xyz", result);
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
        }

        [TestMethod]
        public async Task TestMutlipleHitsAsync()
        {
            var taskTokens = new List<Task>();
            var result = string.Empty;
            var task = HitTask.Mutliple((HitTask<string> sender, HitEventArgs<string> ev) =>
            {
                result = ev.Argument;
            }, 2, 4, TimeSpan.FromMilliseconds(100));
            _ = ProcessHit(taskTokens, task, "abc");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "defg");
            await Task.Delay(20);
            Assert.AreEqual("defg", result);
            _ = ProcessHit(taskTokens, task, "hijk");
            await Task.Delay(20);
            Assert.AreEqual("hijk", result);
            _ = ProcessHit(taskTokens, task, "lmn");
            await Task.Delay(20);
            Assert.AreEqual("lmn", result);
            _ = ProcessHit(taskTokens, task, "opq");
            await Task.Delay(110);
            Assert.AreEqual("lmn", result);
            _ = ProcessHit(taskTokens, task, "rst");
            await Task.Delay(20);
            Assert.AreEqual("lmn", result);
            _ = ProcessHit(taskTokens, task, "uvw");
            await Task.Delay(20);
            Assert.AreEqual("uvw", result);
            _ = ProcessHit(taskTokens, task, "xyz");
            await Task.Delay(20);
            Assert.AreEqual("xyz", result);
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
        }

        [TestMethod]
        public async Task TestTimesHitsAsync()
        {
            var taskTokens = new List<Task>();
            var result = string.Empty;
            var task = HitTask.Times((HitTask<string> sender, HitEventArgs<string> ev) =>
            {
                result = ev.Argument;
            }, 2, 3, TimeSpan.FromMilliseconds(100));
            _ = ProcessHit(taskTokens, task, "abc");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "defg");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "hijk");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "lmn");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            await Task.Delay(90);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "opq");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            _ = ProcessHit(taskTokens, task, "rst");
            await Task.Delay(20);
            Assert.AreEqual(string.Empty, result);
            await Task.Delay(90);
            Assert.AreEqual("rst", result);
            _ = ProcessHit(taskTokens, task, "uvw");
            await Task.Delay(20);
            Assert.AreEqual("rst", result);
            _ = ProcessHit(taskTokens, task, "xyz");
            await Task.Delay(120);
            Assert.AreEqual("xyz", result);
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
        }

        private Task<bool> ProcessHit<T>(IList<Task> taskTokens, HitTask<T> task, T value)
        {
            var t = task.ProcessAsync(value);
            taskTokens.Add(t);
            return t;
        }
    }
}