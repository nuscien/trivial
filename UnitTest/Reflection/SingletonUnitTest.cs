using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Data;
using Trivial.Maths;
using Trivial.Text;

namespace Trivial.Reflection;

/// <summary>
/// Singleton unit test.
/// </summary>
[TestClass]
public class SingletonUnitTest
{
    /// <summary>
    /// Tests object convert helper.
    /// </summary>
    [TestMethod]
    public void TestObjectConvert()
    {
        var s = ObjectConvert.Invoke<string>(123);
        Assert.AreEqual("123", s);
        var f = ObjectConvert.Invoke<float>("456.789");
        Assert.IsTrue(Math.Abs(f - 456.789) < 0.00001);
        var d = ObjectConvert.Invoke<DateTime>("Wed, 17 Jul 2024 11:20:38 GMT");
        Assert.AreEqual(2024, d.Year);
        Assert.AreEqual(7, d.Month);
        var a = ObjectConvert.Invoke<Angle>("7°17'");
        Assert.AreEqual(7, a.Degree);
        Assert.AreEqual(17, a.Arcminute);
        var b = ObjectConvert.Invoke<bool>(true);
        Assert.IsTrue(b);
        var t = new TypedNestedParameter();
        t.Register(s);
        t.Register(f);
        t.Register(d);
        t.Register(a);
        t.Register(b);
        t.Register(t);
        var n = new TestNestedParameter(new TestNestedParameter(t));
        Assert.AreEqual(s, ObjectConvert.TryGet<string>(n));
        Assert.AreEqual(f, ObjectConvert.TryGet<float>(n));
        Assert.AreEqual(d, ObjectConvert.TryGet<DateTime>(n));
        Assert.AreEqual(a, ObjectConvert.TryGet<Angle>(n));
        Assert.AreEqual(b, ObjectConvert.TryGet<bool>(n));
        Assert.AreEqual(t, ObjectConvert.TryGet<TypedNestedParameter>(n));
        Assert.IsTrue(t.Contains<TypedNestedParameter>());
        Assert.AreEqual(t, t.TryGet<TypedNestedParameter>());
        Assert.AreEqual(0, t.TryGet<int>());
        t.Remove<string>();
        t.Remove(typeof(TypedNestedParameter));
        Assert.IsFalse(t.Contains(typeof(string)));
        Assert.IsFalse(t.Contains<TypedNestedParameter>());
        Assert.IsNull(t.TryGet<string>());
        n = new(b);
        Assert.IsTrue(ObjectConvert.TryGet<bool>(n));
        Assert.AreEqual(1, ObjectConvert.TryGet<int>(n));
        n = new(f);
        Assert.AreEqual(f, ObjectConvert.TryGet<float>(n));
        Assert.AreEqual((double)f, ObjectConvert.TryGet<double>(n));
        Assert.IsNull(ObjectConvert.TryGet<Uri>(n));
    }

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
    /// Tests object reference.
    /// </summary>
    [TestMethod]
    public void TestObjectRef()
    {
        var s = "test text";
        IObjectRef r1 = new ObjectRef(s);
        Assert.IsTrue(r1.IsValueCreated);
        Assert.AreEqual(s, r1.Value);
        Assert.IsTrue(r1.IsValueCreated);
        IObjectRef<string> r2 = new ObjectRef<string>(s);
        Assert.IsTrue(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
        r1 = new ObjectRef(r1);
        Assert.IsTrue(r1.IsValueCreated);
        Assert.AreEqual(s, r1.Value);
        Assert.IsTrue(r1.IsValueCreated);
        r2 = new ObjectRef<string>(r2);
        Assert.IsTrue(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
        r1 = ObjectRef.Create(s);
        Assert.IsTrue(r1.IsValueCreated);
        Assert.AreEqual(s, r1.Value);
        Assert.IsTrue(r1.IsValueCreated);
        r2 = ObjectRef<string>.Create(s);
        Assert.IsTrue(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
        r2 = ObjectRef<string>.Create(new Lazy<string>(() => s));
        Assert.IsFalse(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
        r1 = ObjectRef.Create(() => s);
        Assert.IsFalse(r1.IsValueCreated);
        Assert.AreEqual(s, r1.Value);
        Assert.IsTrue(r1.IsValueCreated);
        r2 = ObjectRef<string>.Create(() => s);
        Assert.IsFalse(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
        r1 = new ObjectRef(r1);
        Assert.IsTrue(r1.IsValueCreated);
        Assert.AreEqual(s, r1.Value);
        Assert.IsTrue(r1.IsValueCreated);
        r2 = new ObjectRef<string>(r2);
        Assert.IsTrue(r2.IsValueCreated);
        Assert.AreEqual(s, r2.Value);
        Assert.IsTrue(r2.IsValueCreated);
    }

    /// <summary>
    /// Tests observable properties instance.
    /// </summary>
    [TestMethod]
    public void TestObservableProperties()
    {
        var i = 0;
        var obs = new NameValueObservableModel<string>();
        (obs as INotifyPropertyChanged).PropertyChanged += (sender, obj) =>
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
        var m = JsonSerializer.Deserialize<ConciseModel>("{ \"id\": \"9876543210\", \"keywords\": \"test;another\" }");
        Assert.AreEqual("9876543210", m.Id);
        Assert.IsNull(m.Title);
        Assert.IsNotNull(m.Keywords);
        Assert.AreEqual(2, m.Keywords.Count);
        m = JsonSerializer.Deserialize<ConciseModel>("{ \"id\": \"9876543210\", \"keywords\": [\"test\", \"another\"] }");
        Assert.AreEqual("9876543210", m.Id);
        Assert.IsNull(m.Title);
        Assert.IsNotNull(m.Keywords);
        Assert.AreEqual(2, m.Keywords.Count);
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

internal class TestNestedParameter(object parameter) : BaseNestedParameter(parameter)
{
}
