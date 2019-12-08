using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.UnitTest.Text
{
    [TestClass]
    public class JsonUnitTest
    {
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
            Assert.AreEqual("123", json.GetStringValue("num", true));
            Assert.AreEqual(now.Second, json.GetDateTimeValue("now").Second);
            Assert.AreEqual(now.Second, json.GetDateTimeValue("ticks").Second);
            Assert.AreEqual(JsonValueKind.String, json.GetValueKind("now"));
            Assert.AreEqual(JsonValueKind.Number, json.GetValueKind("ticks"));
            Assert.AreEqual(JsonValueKind.Object, json.GetValueKind("props"));
            Assert.AreEqual(JsonValueKind.Array, json.GetValueKind("arr"));

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
            Assert.AreEqual(jsonArray.GetInt32Value(0), 7);
            Assert.AreNotEqual(jsonArray.GetInt32Value(0), jsonArray.GetInt32Value(1));

            jsonArray.Remove(new Index(0, true));
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
                    "&*()_+-="
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

            str = @"{
    ""H"": "":,.;/| ""
}";
            model2 = JsonSerializer.Deserialize<JsonAttributeTestModel>(str);
            Assert.AreEqual(1, model2.H.Count);
            Assert.AreEqual(":,.;/| ", model2.H[0]);
        }
    }
}
