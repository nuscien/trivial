using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Trivial.Maths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Text
{
    /// <summary>
    /// JSON unit test.
    /// </summary>
    [TestClass]
    public class JsonUnitTest
    {
        /// <summary>
        /// Tests writable JSON DOM.
        /// </summary>
        [TestMethod]
        public void TestJsonObject()
        {
            var now = DateTime.Now;
            var json = new JsonObject();
            json.SetValue("now", now);
            json.SetValue("str-a", "abcdefg");
            json.SetValue("str-a", "hijklmn");
            json.SetValue("str-b", "opq");
            json.SetRange(new Dictionary<string, string>
            {
                { "str-b", "rst" },
                { "str-c", "uvw" },
                { "str-d", "xyz" },
                { "str-e", "$$$" }
            });
            json.Remove("str-e");
            json.SetValue("str-d", "0123456789");
            json.SetValue("num", 123);
            json.SetJavaScriptDateTicksValue("ticks", now);
            json.SetValue("props", json);
            json.SetValue("arr", new JsonArray());
            json.SetRange(json);

            Assert.AreEqual(9, json.Keys.Count());
            Assert.AreEqual("hijklmn", json.GetStringValue("str-a"));
            Assert.AreEqual("rst", json.GetStringValue("str-b"));
            Assert.AreEqual("uvw", json.GetStringValue("str-c"));
            Assert.AreEqual("0123456789", json.GetStringValue("str-d"));
            Assert.AreEqual(123, json.GetInt32Value("num"));
            Assert.AreEqual("123", json.GetStringValue("num"));
            Assert.AreEqual(now.Second, json.GetDateTimeValue("now").Second);
            Assert.AreEqual(now.Second, json.GetDateTimeValue("ticks").Second);
            Assert.AreEqual(JsonValueKind.String, json.GetValueKind("now"));
            Assert.AreEqual(JsonValueKind.Number, json.GetValueKind("ticks"));
            Assert.AreEqual(JsonValueKind.Object, json.GetValueKind("props"));
            Assert.AreEqual(JsonValueKind.Array, json.GetValueKind("arr"));
            Assert.AreEqual(json, json.Clone());

            var props = json.GetObjectValue("props");
            var p1 = new JsonObject
            {
                { "p6", "()()" },
                { "p7", 4567 },
                { "p9", "2020W295" },
            };
            p1.SetValue("p2", true);
            p1.SetValue("p3", p1);
            props.SetValue("p1", p1);
            props.SetValue("p4", "p5");
            Assert.AreNotEqual(json, p1);
            Assert.IsNotNull(json.GetObjectValue("props", "p1", "p3"));
            Assert.IsTrue(json.GetObjectValue("props", "p1", "p3").GetBooleanValue("p2"));
            Assert.IsFalse(json.TryGetObjectValue("props", "p1").ContainsKey("p8"));
            Assert.AreEqual(17, json.GetObjectValue("props", "p1").GetDateTimeValue("p9").Day);
            Assert.IsNull(json.TryGetObjectValue("props", "p1", "p3", "p6"));
            Assert.AreEqual(4567, p1.GetInt32Value("p7"));

            var jsonArray = json.GetArrayValue("arr");
            Assert.AreEqual(0, jsonArray.Count);
            jsonArray.Add("*+-\"\'\\");
            jsonArray.Add(456);
            jsonArray.AddRange(jsonArray);
            jsonArray.Remove(2);
            Assert.AreEqual(456, jsonArray.GetInt32Value(1));
            jsonArray.InsertRange(1, jsonArray);
            Assert.AreEqual(6, jsonArray.Count);
            Assert.AreEqual("*+-\"\'\\", jsonArray.GetStringValue(0));
            Assert.AreEqual(jsonArray.GetStringValue(0), jsonArray.GetStringValue(1));
            Assert.AreEqual(JsonValueKind.Number, jsonArray.GetValueKind(2));
            Assert.AreEqual(jsonArray.GetStringValue(2, true), jsonArray.GetStringValue(3, true));
            Assert.AreEqual(jsonArray.GetInt32Value(2), jsonArray.GetInt32Value(4));
            Assert.AreEqual(jsonArray.GetInt32Value(2), jsonArray.GetInt32Value(5));
            jsonArray.SetValue(0, 7);
            jsonArray.SetValue(1, 0);
            jsonArray.AddRange(new[] { 8, 9 });
            jsonArray.AddNull();
            Assert.AreEqual(9, jsonArray.Count);
            Assert.AreEqual(7, jsonArray.GetInt32Value(0));
            Assert.AreNotEqual(jsonArray.GetInt32Value(0), jsonArray.GetInt32Value(1));
            Assert.AreEqual(7, json.GetValue("arr", "0").GetInt32());
            Assert.AreEqual(7, json.GetValue("arr", null, null, "0", string.Empty).GetInt32());
            Assert.IsNull(json.TryGetValue(new[] { "arr", "0", "x" }));
            Assert.AreEqual(9, jsonArray.GetStringCollection().ToList().Count);

            jsonArray.Remove(jsonArray.Count - 1);
            var m = json.Deserialize<JsonModel>();
            Assert.AreEqual(json.GetStringValue("str-a"), m.A);
            Assert.AreEqual(json.GetStringValue("str-b"), m.B);
            Assert.AreEqual(json.GetStringValue("str-c"), m.C);
            Assert.AreEqual(json.GetInt32Value("num"), m.Num);
            Assert.IsInstanceOfType(m.Col, typeof(IEnumerable<int>));
            var list = m.Col.ToList();
            Assert.AreEqual(jsonArray.Count, list.Count);
            Assert.AreEqual(0, list[1]);
            Assert.AreEqual(456, list[2]);
            Assert.AreEqual(8, list[6]);
            Assert.AreEqual(9, list[7]);
            jsonArray.AddNull();
            Assert.AreEqual(JsonValues.Null, jsonArray[jsonArray.Count - 1]);
            jsonArray.Add(new JsonArray
            {
                8, 9, 0
            });
            jsonArray.Add(null as string);
            jsonArray.AddNull();
            jsonArray.RemoveNull();
            Assert.AreEqual(JsonValueKind.Array, jsonArray[jsonArray.Count - 1].ValueKind);

            var jsonDoc = (JsonDocument)json;
            var jsonStr = json.ToString();
            Assert.IsTrue(jsonStr.StartsWith("{") && jsonStr.Length > 20);
            Assert.AreEqual(jsonStr, jsonDoc.RootElement.ToString());
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            json.WriteTo(writer);
            writer.Flush();
            Assert.AreEqual(jsonStr, Encoding.UTF8.GetString(stream.ToArray()));
            stream.Position = 0;
            json = JsonObject.Parse(stream);
            Assert.AreEqual(9, json.Keys.Count());
            Assert.AreEqual(jsonStr, json.ToString());
        }

        /// <summary>
        /// Tests JSON attributes.
        /// </summary>
        [TestMethod]
        public void TestJsonAttribute()
        {
            var jArr = new JsonArray
            {
                "abc",
                true,
                "defg"
            };
            jArr.AddNull();
            jArr.Add(1234);
            var jObj = new JsonObject();
            jObj.SetValue("hijk", jArr);
            jObj.SetValue("lmn", "opq");
            jObj.SetValue("rst", 56789);
            jObj.SetValue("uvw", false);
            jObj.SetNullValue("x");
            jObj.SetValue("y", new JsonObject());
            jObj.SetValue("z", 0);

            var model = new JsonAttributeTestModel
            {
                A = new DateTime(2020, 1, 1),
                B = new DateTime(2020, 1, 2),
                C = new DateTime(2020, 1, 3),
                D = new DateTime(2020, 1, 4),
                E = new DateTime(2020, 1, 5),
                F = jObj,
                G = jArr,
                H = new List<string>
                {
                    "!@#$%^",
                    null,
                    "&()_+-="
                },
                I = new HashSet<string>
                {
                    "***",
                    string.Empty,
                    "|||||||"
                }
            };
            var str = JsonSerializer.Serialize(model);
            var model2 = JsonSerializer.Deserialize<JsonAttributeTestModel>(str);
            Assert.AreEqual(model.A, model2.A);
            Assert.AreEqual(model.B, model2.B);
            Assert.AreEqual(model.C, model2.C);
            Assert.AreEqual(model.D, model2.D);
            Assert.AreEqual(model.E, model2.E);
            Assert.AreEqual(model.F.Count, model2.F.Count);
            Assert.AreEqual(model.G.Count, model2.G.Count);
            Assert.AreEqual(model.H.Count, model2.H.Count);
            Assert.AreEqual(model.I.Count - 1, model2.I.Count);

            model = new JsonAttributeTestModel
            {
                A = new DateTime(2020, 1, 1),
                B = null,
                C = new DateTime(2020, 1, 3),
                D = null,
                E = new DateTime(2020, 1, 5)
            };
            str = JsonSerializer.Serialize(model);
            model2 = JsonSerializer.Deserialize<JsonAttributeTestModel>(str);
            Assert.AreEqual(model.A, model2.A);
            Assert.AreEqual(model.B, model2.B);
            Assert.AreEqual(model.C, model2.C);
            Assert.AreEqual(model.D, model2.D);
            Assert.AreEqual(model.E, model2.E);
            Assert.AreEqual(null, model2.F);
            Assert.AreEqual(null, model2.G);
            Assert.AreEqual(null, model2.H);
            Assert.AreEqual(null, model2.I);

            str = @"{
    ""H"": "":,.;/| "",
    ""I"": ""abcdefg hijklmn    opq\trst\n\nuvw\rxyz"",
    ""J"": ""123456"",
    ""K"": ""[11.1, 76.9)"",
    ""L"": ""[, 999999999999)"",
    ""M"": ""[3.1415926, ¡Þ]"",
    ""N"": ""[3.6.0, 5.0.0)""
}";
            model2 = JsonSerializer.Deserialize<JsonAttributeTestModel>(str);
            Assert.AreEqual(1, model2.H.Count);
            Assert.AreEqual(":,.;/| ", model2.H[0]);
            Assert.AreEqual(6, model2.I.Count);
            Assert.AreEqual((uint)123456, model2.J);
            Assert.IsNotNull(model2.K);
            Assert.AreEqual(12, model2.K.MinValue);
            Assert.IsFalse(model2.K.LeftOpen);
            Assert.AreEqual(76, model2.K.MaxValue);
            Assert.IsFalse(model2.K.RightOpen);
            Assert.IsFalse(model2.K.IsInInterval(11));
            Assert.IsTrue(model2.K.IsInInterval(12));
            Assert.IsTrue(model2.K.IsInInterval(76));
            Assert.IsFalse(model2.K.IsInInterval(77));
            Assert.IsNotNull(model2.L);
            Assert.IsNull(model2.L.MinValue);
            Assert.IsTrue(model2.L.LeftOpen);
            Assert.IsFalse(model2.L.LeftBounded);
            Assert.AreEqual(999999999999, model2.L.MaxValue);
            Assert.IsTrue(model2.L.RightOpen);
            Assert.IsTrue(model2.L.RightBounded);
            Assert.IsNotNull(model2.M);
            Assert.AreEqual(3.1415926, model2.M.MinValue);
            Assert.IsFalse(model2.M.LeftOpen);
            Assert.IsTrue(model2.M.LeftBounded);
            Assert.IsTrue(model2.M.RightOpen);
            Assert.IsFalse(model2.M.RightBounded);
            Assert.IsFalse(model2.M.IsInInterval(3));
            Assert.IsTrue(model2.M.IsInInterval(3.1415926));
            Assert.IsTrue(model2.M.IsInInterval(200000000000000));
            Assert.IsNotNull(model2.N);
            Assert.AreEqual("3.6.0", model2.N.MinValue);
            Assert.IsFalse(model2.N.LeftOpen);
            Assert.IsTrue(model2.N.LeftBounded);
            Assert.AreEqual("5.0.0", model2.N.MaxValue);
            Assert.IsTrue(model2.N.RightOpen);
            Assert.IsTrue(model2.N.RightBounded);
            Assert.IsFalse(model2.N.IsInInterval("3.0.0.0"));
            Assert.IsTrue(model2.N.IsInInterval("3.6.0.0"));
            Assert.IsTrue(model2.N.IsInInterval("4.2.0.0"));
            Assert.IsFalse(model2.N.IsInInterval("5.0.0.0"));
            Assert.IsFalse(model2.N.IsInInterval("6.0.0.0"));
            Assert.IsNotNull(model2.N.MinVersion);
            Assert.AreEqual(6, model2.N.MinVersion.Minor);
        }
    }
}
