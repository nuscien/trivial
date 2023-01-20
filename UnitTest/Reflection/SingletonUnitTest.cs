using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.Reflection
{
    /// <summary>
    /// Singleton unit test.
    /// </summary>
    [TestClass]
    public class SingletonUnitTest
    {
        /// <summary>
        /// Tests singleton keeper.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Tests observable properties instance.
        /// </summary>
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
        
        /// <summary>
        /// Tests factory.
        /// </summary>
        [TestMethod]
        public void TestFactory()
        {
            var f = FactorySet.Instance();
            var i = 0;
            f.Register(() => i++);
            f.Register(() => Guid.NewGuid());
            f.Register<IJsonValueNode>(() => i % 2 == 0 ? new JsonObjectNode() : new JsonArrayNode());
            Assert.AreEqual(0, f.Create<int>());
            Assert.AreEqual(JsonValueKind.Array, f.Create<IJsonValueNode>().ValueKind);
            Assert.AreEqual(1, f.Create<int>());
            Assert.IsNotNull(f.Create<Guid>());
            Assert.AreEqual(JsonValueKind.Object, f.Create<IJsonValueNode>().ValueKind);
            Assert.IsNull(f.GetFactory<JsonObjectNode>());
            Assert.IsNull(f.GetFactory<JsonArrayNode>());
            Assert.IsNotNull(f.GetFactory<IJsonValueNode>());

            var jsonFactory = new RoutedFactory<IJsonValueNode>();
            jsonFactory.Register("obj", () => new JsonObjectNode());
            jsonFactory.Register("arr", () => new JsonArrayNode());
            var json = jsonFactory.Create("obj");
            Assert.AreEqual(JsonValueKind.Object, json.ValueKind);
            Assert.AreEqual(JsonValueKind.Array, jsonFactory.Create("arr").ValueKind);
        }
    }
}
