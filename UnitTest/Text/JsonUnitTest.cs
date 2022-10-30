using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Trivial.Maths;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Trivial.Text;

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
        var json = CreateModel(now);

        Assert.AreEqual(9, json.Keys.Count());
        Assert.AreEqual("hijklmn", json.GetStringValue("str-a"));
        Assert.AreEqual("hijklmn", json.GetValue<string>("str-a"));
        Assert.AreEqual("rst", json.GetStringValue("str-b"));
        Assert.AreEqual("uvw", json.GetStringValue("str-c"));
        Assert.AreEqual("0123456789", json.GetStringValue("str-d"));
        Assert.AreEqual(123, json.GetInt32Value("num"));
        Assert.AreEqual("123", json.GetStringValue("num"));
        Assert.AreEqual(now.Second, json.GetDateTimeValue("now").Second);
        Assert.AreEqual(now.Second, json.GetValue<DateTime>("now").Second);
        Assert.AreEqual(now.Second, json.GetDateTimeValue("ticks").Second);
        Assert.AreEqual(JsonValueKind.String, json.GetValueKind("now"));
        Assert.AreEqual(JsonValueKind.Number, json.GetValueKind("ticks"));
        Assert.AreEqual(JsonValueKind.Object, json.GetValueKind("props"));
        Assert.AreEqual(JsonValueKind.Array, json.GetValueKind("arr"));
        Assert.AreEqual(json, json.Clone());

        var props = json.GetObjectValue("props");
        var p1 = new JsonObjectNode
        {
            { "p6", "()()" },
            { "p7", 4567 },
            { "p9", "2020W295" },
        };
        p1.SetValue("p2", true);
        p1.SetValue("p3", p1);
        props.SetValue("p1", p1);
        Assert.AreEqual(props, json.GetValue<JsonObjectNode>("props"));
        Assert.AreEqual("@", (json.GetValue<IJsonValueNode>("props") as JsonObjectNode).GetStringValue("$ref"));
        props = props.Clone();
        json.Remove("p1");
        Assert.IsTrue(props.EnsureObjectValue("p4", out _));
        Assert.AreEqual(JsonValueKind.Object, props.GetValueKind("p4"));
        props.SetValue("p4", "p5");
        Assert.IsFalse(props.EnsureArrayValue("p4", out _));
        Assert.AreEqual(JsonValueKind.String, props.GetValueKind("p4"));
        json.SetValue("props", props);
        Assert.AreEqual(props, json.GetValue<IJsonValueNode>("props"));
        Assert.AreNotEqual(json, p1);
        Assert.IsNotNull(json.GetObjectValue("props", "p1", "p3"));
        Assert.IsTrue(json.GetObjectValue("props", "p1", "p3").GetBooleanValue("p2"));
        Assert.IsFalse(json.TryGetObjectValue("props", "p1").ContainsKey("p8"));
        Assert.AreEqual(17, json.GetObjectValue("props", "p1").GetDateTimeValue("p9").Day);
        Assert.AreEqual(2020, json.GetValue<DateTime>("props", "p1", "p9").Year);
        Assert.AreEqual(0, json.GetValue<DateTime>("props", "p1", "p9").Second);
        Assert.AreEqual(2020, json.TryGetValue<DateTime>("props", "p1", "p9").Year);
        json.SetValue("dateObj", new JsonObjectNode
        {
            { "year", 2000 },
            { "month", 12 },
            { "day", 24 },
            { "hour", 20 }
        });
        Assert.AreEqual(2000, json.TryGetDateTimeValue("dateObj").Value.Year);
        Assert.AreEqual(20, json.TryGetDateTimeValue("dateObj").Value.Hour);
        json.SetValue("dateObj", DateTime.Now);
        Assert.AreEqual(JsonValueKind.Object, json.GetValueKind("dateObj"));
        Assert.IsTrue(json.TryGetDateTimeValue("dateObj").HasValue);
        json.SetValue("dateObj", DateTime.Now, null);
        Assert.AreEqual(JsonValueKind.String, json.GetValueKind("dateObj"));
        Assert.IsTrue(json.TryGetDateTimeValue("dateObj").HasValue);
        json.Remove("dateObj");
        Assert.IsFalse(json.TryGetDateTimeValue("dateObj").HasValue);
        Assert.IsNull(json.TryGetObjectValue("props", "p1", "p3", "p6"));
        Assert.AreEqual(4567, p1.GetInt32Value("p7"));
        Assert.AreEqual(4567, p1.GetValue<int>("p7"));
        p1.SetValue("p9", "Wed, 21 Oct 2015 07:28:00 GMT");
        Assert.AreEqual(28, json.GetObjectValue("props", "p1").GetDateTimeValue("p9").Minute);
        Assert.AreEqual(10, json.GetValue<DateTime>("props", "p1", "p9").Month);
        Assert.AreEqual(2015, json.GetValue<DateTime>("props", "p1", "p9").Year);
        p1.SetValue("p9", "2022-01-03T04:56:00");
        Assert.AreEqual(56, json.GetObjectValue("props", "p1").GetDateTimeValue("p9").Minute);
        Assert.AreEqual(1, json.GetValue<DateTime>("props", "p1", "p9").Month);
        Assert.AreEqual(2022, json.GetValue<DateTime>("props", "p1", "p9").Year);
        p1.SetValue("p9", "2010-1-2");
        Assert.AreEqual(2010, json.GetValue<DateTime>("props", "p1", "p9").Year);
        Assert.AreEqual(2, json.GetValue<DateTime>("props", "p1", "p9").Day);
        p1.SetValue("p9", "7月17日");
        Assert.AreEqual(DateTime.UtcNow.Year, json.GetValue<DateTime>("props", "p1", "p9").Year);
        Assert.AreEqual(17, json.GetValue<DateTime>("props", "p1", "p9").Day);
        p1.SetValue("p9", "сегодня");
        Assert.AreEqual(DateTime.UtcNow.Day, json.GetValue<DateTime>("props", "p1", "p9").Day);
        Assert.AreEqual(0, json.GetValue<DateTime>("props", "p1", "p9").Minute);
        p1.SetValue("p9", "сейчас");
        Assert.AreEqual(DateTime.UtcNow.Day, json.GetValue<DateTime>("props", "p1", "p9").Day);
        p1.SetValue("p9", "어제");
        Assert.AreEqual((DateTime.UtcNow - TimeSpan.FromDays(1)).Day, json.GetValue<DateTime>("props", "p1", "p9").Day);
        p1.SetValue("p9", "mañana");
        Assert.AreEqual((DateTime.UtcNow + TimeSpan.FromDays(1)).Day, json.GetValue<DateTime>("props", "p1", "p9").Day);
        p1.SetValue("p9", "2020W295");
        Assert.AreEqual(7, json.GetValue<DateTime>("props", "p1", "p9").Month);
        Assert.AreEqual("2020W295", json.GetValue<string>("props", "p1", "p9"));

        json.EnableThreadSafeMode(3);
        Assert.IsTrue(json.GetObjectValue("props", "p1", "p3").GetBooleanValue("p2"));
        Assert.IsTrue(json["props", "p1", "p3", "p2"].GetBoolean());
        Assert.IsTrue(json.TryGetObjectValue("props", "p1", "p3").GetBooleanValue("p2"));
        Assert.IsTrue(json.TryGetValue("props.p1.p3.p2", true).GetBoolean());
        Assert.IsTrue(json.TryGetValue("props. 'p1'.'p3'.p2", true).GetBoolean());
        Assert.IsTrue(json.TryGetValue("[props]['p1'][p3][ 'p2' ]", true).GetBoolean());
        Assert.IsTrue(json.GetValue<bool>("props", "p1", "p3", "p2"));
        try
        {
            var testError = json["props", "p1", "q", "p3"];
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
        }
        catch (ArgumentException)
        {
        }

        try
        {
            var testError = json["props", "p1", "p3", "q"];
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
        }
        catch (ArgumentException)
        {
        }

        json["dot.net"] = new JsonArrayNode
        {
            "Text",
            new JsonObjectNode
            {
                { "name", "value" },
                { ".[][][]....\"\'\\.", "test" }
            }
        };
        Assert.AreEqual(4567, p1.GetInt32Value("p7"));
        Assert.IsTrue(json.ContainsKey("dot.net"));
        Assert.AreEqual("Text", json["dot.net", 0].GetString());
        Assert.AreEqual(JsonValueKind.Array, json["dot.net"].ValueKind);
        Assert.AreEqual("e", json.TryGetValue("[dot.net][1]['.[][][]....\\\"\\\'\\\\.'][1]", true).GetString());
        Assert.AreEqual(4, json.TryGetValue("'dot.net'.1.\".[][][]....\\\"\\\'\\\\.\".length", true).GetInt32());
        json.GetArrayValue("dot.net").ReplaceValue(json.GetArrayValue("dot.net").GetObjectValue(1), new JsonObjectNode
        {
            { "then", "true" }
        });
        Assert.IsTrue(json.TryGetBooleanValue(new[] { "dot.net", "1", "then" }));
        json.Remove("dot.net");
        Assert.IsFalse(json.ContainsKey("dot.net"));
        Assert.IsNull(json.TryGetValue("dot.net"));

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
        Assert.IsTrue(json.TryGetValue(new List<string> { "arr" }, out _));
        Assert.IsFalse(json.TryGetValue(new List<string> { "arr", "some" }, out _));
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
        jsonArray.Add(new JsonArrayNode
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
        json = JsonObjectNode.Parse(stream);
        Assert.AreEqual(9, json.Keys.Count);
        Assert.AreEqual(jsonStr, json.ToString());
        var jsonNode = (System.Text.Json.Nodes.JsonNode)json;
        Assert.IsNotNull(jsonNode);
        json = jsonNode;
        Assert.IsNotNull(json);
        Assert.AreEqual(9, json.Keys.Count);
        Assert.AreEqual(jsonStr, json.ToString());

        Assert.IsFalse(jsonArray.IsNull(0));
        Assert.IsTrue(jsonArray.IsNullOrUndefined(100));
        Assert.IsTrue(jsonArray.EnsureCount(20) > 0);
        jsonArray.SetValue(20, "str");
        Assert.AreEqual("str", jsonArray.GetStringValue(20));
        jsonArray.SetValue(20, DateTime.Now);
        jsonArray.SetValue(20, Guid.NewGuid());
        jsonArray.SetValue(20, 1);
        Assert.AreEqual(1, jsonArray.GetInt32Value(20));
        jsonArray.SetValue(20, 1L);
        Assert.AreEqual(1L, jsonArray.GetInt64Value(20));
        jsonArray.SetValue(20, 1.0);
        Assert.AreEqual(1.0, jsonArray.GetDoubleValue(20));
        jsonArray.SetValue(20, 1.0F);
        Assert.AreEqual(1.0F, jsonArray.GetSingleValue(20));
        jsonArray.SetValue(20, true);
        Assert.IsTrue(jsonArray.GetBooleanValue(20));
        jsonArray.SetValue(20, false);
        Assert.IsFalse(jsonArray.GetBooleanValue(20));
        Assert.AreEqual(JsonValueKind.False, (jsonArray as IJsonContainerNode).GetValue("20").ValueKind);
        jsonArray.Remove(20);

        Assert.IsFalse(json.IsNull("props"));
        Assert.IsTrue(json.IsNullOrUndefined("test"));
        json.SetValue("test", "str");
        Assert.AreEqual("str", json.GetStringValue("test"));
        json.SetValue("test", Guid.NewGuid());
        json.SetValue("test", DateTime.Now);
        json.SetUnixTimestampValue("test", DateTime.Now);
        json.SetJavaScriptDateTicksValue("test", DateTime.Now);
        json.SetDateTimeStringValue("test", DateTime.Now);
        json.SetValue("test", 1);
        Assert.AreEqual(1, json.GetInt32Value("test"));
        json.SetValue("test", 1L);
        Assert.AreEqual(1L, json.GetInt64Value("test"));
        json.SetValue("test", 1.0);
        Assert.AreEqual(1.0, json.GetDoubleValue("test"));
        json.SetValue("test", 1.0F);
        Assert.AreEqual(1.0F, json.GetSingleValue("test"));
        json.SetValue("test", true);
        Assert.IsTrue(json.GetBooleanValue("test"));
        json.SetValue("test", false);
        Assert.IsFalse(json.GetBooleanValue("test"));
        json.Remove("test");
        Assert.IsTrue(json.IsNullOrUndefined("test"));
        json.Add("1", 70);
        json.Add("2", "Greetings!");
        json.Add("3", true);
        json.Add("4", 1.0);
        json.Add("5", 600L);
        json.Add("6", DateTime.Now);
        Assert.IsTrue(json.Count > 6);

        json.Clear();
        Assert.AreEqual(0, json.Count);
        json.SetRange(new Dictionary<string, int>
        {
            { "number", 100 }
        });
        json.SetRange(new Dictionary<string, int>
        {
            { "number", 200 },
            { "another", 300 }
        }, true);
        json.SetRange(new Dictionary<string, JsonObjectNode>
        {
            { "duplicate", json }
        });
        json.SetRange(new Dictionary<string, JsonObjectNode>
        {
            { "another", new JsonObjectNode() }
        }, true);
        json.SetRange(new Dictionary<string, JsonArrayNode>
        {
            { "arr", new JsonArrayNode() }
        });
        json.SetRange(new Dictionary<string, JsonArrayNode>
        {
            { "arr", null }
        }, true);
        Assert.AreEqual(4, json.Count);
        json.IncreaseValue("number");
        json.IncreaseValue("number", 7L);
        Assert.AreEqual(108, json.GetValue<int>("number"));
        json.DecreaseValue("number");
        json.DecreaseValue("number", 7L);
        Assert.AreEqual(100L, json.GetInt64Value("number"));
        json.IncreaseValue("number", 1.2);
        json.DecreaseValue("number", 0.3);
        Assert.IsTrue(json.GetDoubleValue("number") > 100);

        var j1 = Serialize<System.Text.Json.Nodes.JsonObject>("{ \"a\": \"bcdefg\", \"h\": \"ijklmn\" }");
        Assert.AreEqual(2, j1.Count);
        var j2 = Serialize<System.Text.Json.Nodes.JsonArray>("[ 123, 456 ]");
        Assert.AreEqual(2, j2.Count);
        var j3 = Serialize<System.Text.Json.Nodes.JsonNode>("{ \"a\": \"bcdefg\", \"h\": \"ijklmn\" }");
        Assert.AreEqual(2, j3.AsObject().Count);
        j3 = Serialize<System.Text.Json.Nodes.JsonNode>("[ 123, 456 ]");
        Assert.AreEqual(2, j3.AsArray().Count);
    }

    /// <summary>
    /// Tests JSON attributes.
    /// </summary>
    [TestMethod]
    public void TestJsonAttribute()
    {
        var jArr = new JsonArrayNode
        {
            "abc",
            true,
            "defg"
        };
        jArr.AddNull();
        jArr.Add(1234);
        Assert.AreEqual(4, jArr.OfType<string>().Count());
        Assert.AreEqual("true", jArr.OfType<string>().ToList()[1]);
        Assert.AreEqual(1, jArr.OfType<long>().Count());
        Assert.AreEqual(0, jArr.OfType<JsonObjectNode>().Count());
        Assert.AreEqual(2, jArr.OfType<IJsonStringNode>().Count());
        Assert.AreEqual("defg", jArr.OfType<IJsonStringNode>().ToList()[1].StringValue);
        var jObj = new JsonObjectNode();
        jObj.SetValue("hijk", jArr);
        jObj.SetValue("lmn", "opq");
        jObj.SetValue("rst", "56789K");
        Assert.AreEqual(56789000, jObj.TryGetInt32Value("rst"));
        Assert.AreEqual(56789000, jObj["rst"].GetInt64());
        jObj.SetValue("rst", 56789);
        jObj.SetValue("uvw", false);
        jObj.SetNullValue("x");
        jObj.SetValue("y", new JsonObjectNode());
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
""M"": ""[3.1415926, " + Numbers.InfiniteSymbol + @"]"",
""N"": ""[3.6.0, 5.0.0)"",
""O"": 2,
""P"": ""unauthorized"",
""R"": ""#CC3333"",
""S"": ""rgba(240, 240, 16, 0.8)"",
""T"": { ""b"": 255 },
""V"": ""unknown"",
""W"": 17.24
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
        Assert.AreEqual(Data.ChangeErrorKinds.Unauthorized, model2.O);
        Assert.AreEqual(Data.ChangeErrorKinds.Unauthorized, model2.P);
        Assert.AreEqual(Data.ChangeErrorKinds.None, model2.Q);
        Assert.AreEqual(0xCC, model2.R.R);
        Assert.AreEqual(240, model2.S.G);
        Assert.IsTrue(model.S.A < 0.9 && model2.S.A > 0.7);
        Assert.AreEqual(255, model2.T.B);
        Assert.IsNull(model2.U);
        Assert.AreEqual(Data.ChangeMethods.Unknown, model2.V);
        Assert.AreEqual(17, model2.W.Degree);
        Assert.IsTrue(model2.W.Arcminute > 0);
        str = JsonSerializer.Serialize(model2);
        Assert.IsNotNull(str);
    }

    private static T Serialize<T>(string json) where T : System.Text.Json.Nodes.JsonNode
    {
        var t = typeof(T);
        var n = t;
        if (string.IsNullOrEmpty(json)) return null;
        if (t.Name.Equals("JsonNode", StringComparison.InvariantCulture))
        {
        }
        else if (t.Name.Equals("JsonObject", StringComparison.InvariantCulture) || t.Name.Equals("JsonArray", StringComparison.InvariantCulture))
        {
            n = t.Assembly.GetType("System.Text.Json.Nodes.JsonNode", false);
            if (n == null) return null;
        }
        else
        {
            return null;
        }

        var parser = n.GetMethod("Parse", new[] { typeof(string) });
        if (parser != null && parser.IsStatic)
        {
            return (T)parser.Invoke(null, new object[] { json });
        }

        var nullable = typeof(Nullable<>);
        var optionsType = t.Assembly.GetType("System.Text.Json.Nodes.JsonNodeOptions", false);
        if (optionsType == null) return null;
        nullable = nullable.MakeGenericType(optionsType);
        parser = n.GetMethod("Parse", new[] { typeof(string), nullable, typeof(JsonDocumentOptions) });
        if (parser != null && parser.IsStatic)
        {
            return (T)parser.Invoke(null, new object[] { json, null, default(JsonDocumentOptions) });
        }

        return null;
    }

    internal static JsonObjectNode CreateModel(DateTime now)
    {
        var json = new JsonObjectNode();
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
        json.SetValue("arr", new JsonArrayNode());
        json.SetRange(json);
        return json;
    }
}
