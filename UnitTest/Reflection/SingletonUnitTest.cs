using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.UnitTest.Reflection
{
    [TestClass]
    public class SingletonUnitTest
    {
        [TestMethod]
        public async Task TestSingletonKeeperAsync()
        {
            var i = 0;
            var singleton = new SingletonKeeper<int>(async () =>
            {
                await Task.Delay(10);
                i++;
                return i;
            });
            Assert.AreEqual(0, singleton.Cache);
            var j = await singleton.RenewAsync(true);
            Assert.AreEqual(i, j);
            Assert.AreEqual(1, j);
        }

        [TestMethod]
        public void TestObservableProperties()
        {
            var i = 0;
            var obs = new NameValueObservableProperties<string>();
            obs.PropertyChanged += (sender, obj) =>
            {
                if (obj.PropertyName == "Value") i++;
            };
            obs.Name = "abcdefg";
            Assert.AreEqual(0, i);
            obs.Value = "hijklmn";
            Assert.AreEqual(1, i);
            obs.Name = "opqrst";
            Assert.AreEqual(1, i);
            obs.Value = "uvwxyz";
            Assert.AreEqual(2, i);
        }
    }
}
