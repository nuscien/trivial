using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// Tests JSON object node access, mutation and serialization.
/// </summary>
[TestClass]
public class JsonObjectNodeTest
{
    private static readonly DateTime SampleDate = new(2024, 2, 29, 12, 34, 56);
    private static readonly Guid SampleGuid = new("f9524ddf-a080-44a4-b8b9-c1994946c620");

    [TestMethod]
    public void ConstructorAndMetadata()
    {
        var json = new JsonObjectNode();
        Assert.AreEqual(JsonValueKind.Object, json.ValueKind);
        Assert.AreEqual(0, json.Count);
        Assert.IsFalse(json.Any());
        Assert.IsFalse(json.IsReadOnly);
        Assert.IsNotNull(json.RevisionToken);
        Assert.IsNull(json.Id);
        Assert.IsNull(json.Schema);
        Assert.IsNull(json.TypeDiscriminator);
        Assert.IsNull(json.LocalDefinitions);
        Assert.IsNull(json.CommentValue);
        json.Id = "id";
        json.Schema = "schema";
        json.TypeDiscriminator = "kind";
        json.LocalDefinitions = new JsonObjectNode { { "entry", new JsonObjectNode { { "v", 1 } } } };
        json.CommentValue = "comment";
        Assert.AreEqual("id", json.Id);
        Assert.AreEqual("schema", json.Schema);
        Assert.AreEqual("kind", json.TypeDiscriminator);
        Assert.AreEqual("comment", json.CommentValue);
        Assert.AreEqual(1, json.GetLocalDefinition("entry").GetInt32Value("v"));
        json.Id = null;
        json.Schema = null;
        json.TypeDiscriminator = null;
        json.LocalDefinitions = null;
        json.CommentValue = "  ";
        Assert.AreEqual(0, json.Count);
        Assert.IsNull(json.GetLocalDefinition("missing"));
    }

    [TestMethod]
    public void SerializationConstructorAndObjectData()
    {
        Assert.AreEqual(0, new SerializableObject(null).Count);
        var info = NewSerializationInfo();
        info.AddValue("null", null, typeof(string));
        info.AddValue("long", 42L);
        info.AddValue("int", 42);
        info.AddValue("uint", 42U);
        info.AddValue("double", 4.5D);
        info.AddValue("float", 4.5F);
        info.AddValue("decimal", 4.5M);
        info.AddValue("date", SampleDate);
        info.AddValue("guid", SampleGuid);
        info.AddValue("string", "text");
        info.AddValue("object", new JsonObjectNode { { "v", 1 } });
        info.AddValue("array", new JsonArrayNode { 1 });
        info.AddValue("systemObject", new JsonObject { ["v"] = 1 });
        info.AddValue("systemArray", new JsonArray(1));
        info.AddValue("host", new ObjectHost());
        using var doc = JsonDocument.Parse("{\"v\":1}");
        info.AddValue("element", doc.RootElement);
        info.AddValue("unsupported", new object());
        var json = new SerializableObject(info);
        Assert.IsTrue(json.IsNull("null"));
        foreach (var key in new[] { "long", "int", "uint" }) Assert.AreEqual(42L, json.GetInt64Value(key));
        foreach (var key in new[] { "double", "float", "decimal" }) Assert.AreEqual(4.5M, json.GetDecimalValue(key));
        Assert.AreEqual(SampleDate, json.GetDateTimeValue("date"));
        Assert.AreEqual(SampleGuid, json.GetGuidValue("guid"));
        Assert.AreEqual("text", json.GetStringValue("string"));
        foreach (var key in new[] { "object", "systemObject", "host", "element" }) Assert.AreEqual(1, json.GetObjectValue(key).GetInt32Value("v"));
        foreach (var key in new[] { "array", "systemArray" }) Assert.AreEqual(1, json.GetArrayValue(key).Count);
        Assert.IsFalse(json.ContainsKey("unsupported"));
        json.SetValue("true", true);
        json.SetValue("false", false);
        var output = NewSerializationInfo();
        json.GetObjectData(output, default);
        Assert.AreEqual("text", output.GetString("string"));
        Assert.AreEqual(42L, output.GetInt64("long"));
        Assert.AreEqual(4.5D, output.GetDouble("double"));
        Assert.AreEqual(4.5M, output.GetDecimal("decimal"));
        Assert.IsTrue(output.GetBoolean("true"));
        Assert.IsFalse(output.GetBoolean("false"));
        Assert.IsInstanceOfType<JsonObjectNode>(output.GetValue("object", typeof(JsonObjectNode)));
        Assert.IsInstanceOfType<JsonArrayNode>(output.GetValue("array", typeof(JsonArrayNode)));
        Assert.ThrowsExactly<SerializationException>(() => output.GetValue("null", typeof(object)));
    }

    [TestMethod]
    public void KeyValidationAndKinds()
    {
        var json = JsonObjectNode.Parse("{\"n\":null,\"i\":1,\"l\":2147483648,\"d\":1.5,\"s\":\"text\",\"t\":true,\"f\":false,\"o\":{},\"a\":[]}");
        Assert.IsTrue(json.IsNull("n"));
        try
        {
            json.IsNull("missing");
            Assert.Fail("Should throw exception.");
        }
        catch (KeyNotFoundException)
        {
        }

        Assert.IsTrue(json.IsNullOrUndefined("missing"));
        Assert.IsFalse(json.IsNullOrUndefined("i"));
        Assert.IsTrue(json.IsValueKind("i", JsonValueKind.Number));
        Assert.IsTrue(json.ContainsKey("i".AsSpan()));
        Assert.AreEqual(JsonValueKind.Undefined, json.GetValueKind("missing"));
        Assert.AreEqual(JsonValueKind.Number, json.GetValueKind("i".AsSpan(), true));
        Assert.AreEqual("1", json.GetRawText("i"));
        Assert.AreEqual("\"text\"", json.GetRawText("s".AsSpan()));
        Assert.AreSame(json["i"], json.GetValue("i", false));
        Assert.AreSame(json["i"], json.GetValue("i".AsSpan()));
        Assert.AreEqual(JsonValueKind.Undefined, json.GetValue("missing", true).ValueKind);
        Assert.ThrowsExactly<ArgumentNullException>(() => json.GetValue((string)null));
        Assert.ThrowsExactly<ArgumentException>(() => json.GetValue(" "));
        Assert.ThrowsExactly<KeyNotFoundException>(() => json.GetValue("missing"));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => json.GetValueKind("missing", true));
        foreach (var key in json.Keys)
        {
            Assert.AreEqual(json[key].ValueKind, json.GetValueKind(key, out var value));
            if (key == "n") Assert.IsNull(value);
            else Assert.IsNotNull(value);
        }
        Assert.AreEqual(JsonValueKind.Undefined, json.GetValueKind("missing", out var missing));
        Assert.IsNull(missing);
        Assert.IsTrue(json.TryGetValue("i".AsSpan(), out var spanValue));
        Assert.AreSame(json["i"], spanValue);
        Assert.AreSame(spanValue, json.TryGetValue("i".AsSpan()));
        Assert.IsFalse(json.TryGetValue("missing", out _));
        json.SetValue("0", "zero");
        Assert.AreEqual("zero", json.TryGetValue(0).TryConvert<string>());
    }

    [TestMethod]
    public void ContainsOnlyAndLocalizedKeys()
    {
        var json = new JsonObjectNode();
        Assert.IsTrue(json.ContainsOnlyKeys(null));
        Assert.IsTrue(json.ContainsOnlyKeys(null, out var empty));
        Assert.AreEqual(0, empty.Count);
        Assert.IsTrue(json.ContainsOnlyKeys(null, out _, out _));
        json.SetValue("v", 42);
        Assert.IsTrue(json.ContainsOnlyKey("v"));
        Assert.IsFalse(json.ContainsOnlyKey("missing"));
        Assert.IsTrue(json.ContainsOnlyKey("v", out int number));
        Assert.AreEqual(42, number);
        Assert.IsTrue(json.ContainsOnlyKey("v", out long longNumber));
        Assert.AreEqual(42L, longNumber);
        Assert.IsTrue(json.ContainsOnlyKey("v", out string text));
        Assert.AreEqual("42", text);
        Assert.IsFalse(json.ContainsOnlyKeys(null));
        Assert.IsTrue(json.ContainsOnlyKeys(new[] { "v", "extra" }));
        Assert.IsFalse(json.ContainsOnlyKeys(new[] { "extra" }, out var matched, out var rest));
        Assert.AreEqual(0, matched.Count);
        CollectionAssert.AreEqual(new[] { "v" }, rest);
        Assert.IsTrue(json.ContainsOnlyKeys(new[] { "v" }, out matched));
        CollectionAssert.AreEqual(new[] { "v" }, matched);
        json.SetValue("v", true);
        Assert.IsTrue(json.ContainsOnlyKey("v", out bool? boolean));
        Assert.AreEqual(true, boolean);
        json.SetValue("v", new JsonObjectNode());
        Assert.IsTrue(json.ContainsOnlyKey("v", out JsonObjectNode obj));
        Assert.IsNotNull(obj);
        json.SetValue("v", new JsonArrayNode());
        Assert.IsTrue(json.ContainsOnlyKey("v", out JsonArrayNode arr));
        Assert.IsNotNull(arr);
        json.SetValue("extra", 1);
        Assert.IsFalse(json.ContainsOnlyKey("v", out int _));
        Assert.IsFalse(json.ContainsOnlyKey("v", out long _));
        Assert.IsFalse(json.ContainsOnlyKey("v", out string _));
        Assert.IsFalse(json.ContainsOnlyKey("v", out bool? _));
        Assert.IsFalse(json.ContainsOnlyKey("v", out JsonObjectNode _));
        Assert.IsFalse(json.ContainsOnlyKey("v", out JsonArrayNode _));
        json = JsonObjectNode.Parse("{\"title\":\"default\",\"title#en\":\" English \",\"title#en-US\":\" US \",\"title#zh\":\"中文\"}");
        Assert.AreEqual(0, json.GetKey(null).Count());
        CollectionAssert.AreEquivalent(new[] { "title", "title#en", "title#en-US", "title#zh" }, json.GetKey("title").ToArray());
        Assert.AreEqual(" US ", json.TryGetStringValueByCulture("title", CultureInfo.GetCultureInfo("en-US")));
        Assert.AreEqual("US", json.TryGetStringTrimmedValueByCulture("title", false, CultureInfo.GetCultureInfo("en-US")));
        Assert.AreEqual("default", json.TryGetStringValueByCulture("title", CultureInfo.GetCultureInfo("fr-FR")));
        Assert.AreEqual(" English ", json.TryGetValueByCulture("title#en").TryConvert<string>());
    }

    [TestMethod]
    public void PathsAndIndexers()
    {
        var json = JsonObjectNode.Parse("{\"a\":[{\"v\":7},[8],\"ab\",9],\"o\":{\"0\":10,\"child\":{\"v\":11}},\"s\":\"xy\",\"n\":null,\"v\":12}");
        Assert.AreEqual(7, json["a", 0, "v"].TryConvert<int>());
        Assert.AreEqual(8, json["a", 1, 0].TryConvert<int>());
        Assert.AreEqual("b", json["a", 2, 1].TryConvert<string>());
        Assert.AreEqual(10, json["o", 0].TryConvert<int>());
        Assert.AreEqual("y", json["s", 1].TryConvert<string>());
        Assert.AreEqual(12, json["v", 0].TryConvert<int>());
        Assert.AreEqual(11, json["o", "child", "v"].TryConvert<int>());
        Assert.AreSame(json["a", 0], json["a", 0, (string[])null]);
        Assert.AreSame(json["a", 1], json["a", 1, new string[] { null }]);
        Assert.AreEqual(8, json["a", 1, new[] { "0" }].TryConvert<int>());
        Assert.AreEqual(8, json["a", 1, new[] { null, "0" }].TryConvert<int>());
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _ = json["a", -1]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _ = json["a", 0, -1]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _ = json["a", 1, 99]);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = json["v", 1]);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = json["a", 3, 0]);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = json["a", 1, "bad"]);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = json["a", 3, "bad"]);
        var path = new[] { "a", "0", "v" };
        Assert.AreEqual(7, json.GetValue<int>(path));
        Assert.AreEqual(7, json.GetValue<int>("a", "0", "v"));
        Assert.AreEqual(7, json.GetValue("a", "0", "v").TryConvert<int>());
        Assert.AreEqual(7, json.TryGetValue("a", "0", "v").TryConvert<int>());
        Assert.AreEqual(7, json.TryGetValue<int>(path));
        Assert.AreEqual(7, json.TryGetValue("$.a[0].v", true).TryConvert<int>());
        Assert.AreEqual(7, json.TryGetValue("$.a.0.v", true).TryConvert<int>());
        Assert.AreEqual(12, json.TryGetValue("v", false).TryConvert<int>());
        Assert.AreSame(json, json.TryGetValue((string)null, true));
        Assert.AreSame(json, json.GetValue((IEnumerable<string>)null));
        Assert.AreSame(json, json.GetValue(Array.Empty<string>()));
        Assert.AreSame(json, json.TryGetValue((IEnumerable<string>)null));
        Assert.AreSame(json, json.GetValue<JsonObjectNode>(Array.Empty<string>()));
        Assert.AreSame(json, json.GetValue<BaseJsonValueNode>(Array.Empty<string>()));
        Assert.AreSame(json, json.GetValue<IJsonValueNode>(Array.Empty<string>()));
        Assert.AreEqual(json.ToString(), json.GetValue<string>(Array.Empty<string>()));
        Assert.ThrowsExactly<ArgumentException>(() => json.GetValue<int>(Array.Empty<string>()));
        foreach (var invalid in new[] { new[] { "missing" }, new[] { "n", "x" }, new[] { "v", "x" }, new[] { "a", "999", "v" } })
        {
            Assert.IsNull(json.TryGetValue(invalid));
            Assert.IsFalse(json.TryGetValue(invalid, out _));
            Assert.ThrowsExactly<InvalidOperationException>(() => json.GetValue(invalid));
        }
        Assert.AreEqual(12, json.GetValue(new[] { null, "", "v" }).TryConvert<int>());
        Assert.AreEqual(11, json.GetObjectValue("o", "child").GetInt32Value("v"));
        Assert.AreEqual(11, json.GetObjectValue(new[] { "o", "child" }).GetInt32Value("v"));
        Assert.AreEqual(7, json.TryGetObjectValue("a", 0).GetInt32Value("v"));
        Assert.IsNotNull(json.TryGetObjectValue("o", "child"));
        Assert.IsTrue(json.TryGetObjectValue(new[] { "o", "child" }, out _));
        Assert.IsFalse(json.TryGetObjectValue("v", out _));
        Assert.IsTrue(json.TryGetArrayValue("a", out _));
        Assert.IsTrue(json.TryGetArrayValue(new[] { "a" }, out _));
        Assert.IsNull(json.TryGetArrayValue("v"));
        json.SetValue("self", json);
        Assert.AreSame(json, json.GetObjectValue("self"));
        Assert.AreSame(json, json.TryGetObjectValue("self"));
        Assert.AreEqual(12, json.GetValue<int>(new[] { "self", "v" }));
        Assert.AreEqual(12, json.TryGetValue<int>("self", "v"));
    }

    public static IEnumerable<object[]> NumericGetterCases()
    {
        foreach (var name in new[] { "UInt16", "UInt32", "Int16", "Int32", "Int64", "Single", "Double", "Decimal", "Boolean" })
        {
            foreach (var method in typeof(JsonObjectNode).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.Name == "TryGet" + name + "Value" && !m.IsGenericMethod))
            {
                var parameters = method.GetParameters();
                if (parameters[0].ParameterType != typeof(string) && parameters[0].ParameterType != typeof(IEnumerable<string>)) continue;
                if (parameters.Any(p => p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IJsonPropertyResolver<>))) continue;
                foreach (var json in new[] { name == "Boolean" ? "true" : "42", name == "Boolean" ? "\"true\"" : "\"42\"", "null", "{}", "\"bad\"", "missing" })
                    yield return new object[] { method.ToString(), name, json };
            }
        }
    }

    //[TestMethod]
    //[DynamicData(nameof(NumericGetterCases))]
    //public void NumericTryGetterOverloads(string signature, string name, string value)
    //{
    //    var method = typeof(JsonObjectNode).GetMethods().Single(m => m.ToString() == signature);
    //    var json = value == "missing" ? new JsonObjectNode() : JsonObjectNode.Parse("{\"v\":" + value + "}");
    //    var parameters = method.GetParameters();
    //    var args = parameters.Select(p => p.IsOut ? null : p.ParameterType == typeof(string) ? (object)"v"
    //        : p.ParameterType == typeof(IEnumerable<string>) ? new[] { "v" }
    //        : p.ParameterType == typeof(bool) ? false
    //        : p.HasDefaultValue ? p.DefaultValue : Activator.CreateInstance(p.ParameterType)).ToArray();
    //    var result = Invoke(method, json, args);
    //    var success = value == "42" || value == "\"42\"" || value == "true" || value == "\"true\"";
    //    var outIndex = Array.FindIndex(parameters, p => p.IsOut && p.ParameterType != typeof(JsonValueKind).MakeByRefType());
    //    var kindIndex = Array.FindIndex(parameters, p => p.ParameterType == typeof(JsonValueKind).MakeByRefType());
    //    if (kindIndex >= 0) Assert.AreEqual(json.GetValueKind("v"), args[kindIndex], signature);
    //    if (outIndex >= 0)
    //    {
    //        Assert.AreEqual(success, result, signature);
    //        if (success) Assert.AreEqual(name == "Boolean" ? true : Convert.ChangeType(42, parameters[outIndex].ParameterType.GetElementType(), CultureInfo.InvariantCulture), args[outIndex], signature);
    //    }
    //    else if (success)
    //    {
    //        Assert.AreEqual(name == "Boolean" ? true : Convert.ChangeType(42, Nullable.GetUnderlyingType(method.ReturnType) ?? method.ReturnType, CultureInfo.InvariantCulture), result, signature);
    //    }
    //    else if (method.ReturnType == typeof(float))
    //    {
    //        Assert.IsTrue(float.IsNaN((float)result) || (float)result == 0F, signature);
    //    }
    //    else if (method.ReturnType == typeof(double))
    //    {
    //        Assert.IsTrue(double.IsNaN((double)result) || (double)result == 0D, signature);
    //    }
    //    else Assert.IsNull(result, signature);
    //}

    [TestMethod]
    public void TypedGettersAndConversions()
    {
        var json = JsonObjectNode.Parse("{\"v\":42,\"s\":\"42\",\"bad\":{},\"n\":null,\"b\":true,\"str\":\"abcdef\",\"o\":{\"Value\":42},\"a\":[1,2]}");
        CheckConversion(json, 42);
        CheckConversion(json, 42L);
        CheckConversion(json, 42U);
        CheckConversion(json, (short)42);
        CheckConversion(json, (ushort)42);
        CheckConversion(json, 42F);
        CheckConversion(json, 42D);
        CheckConversion(json, 42M);
        Assert.AreEqual(42UL, json.GetValue<ulong>("v"));
        Assert.AreEqual("42", json.GetValue<string>("v"));
        Assert.IsTrue(json.GetBooleanValue("b"));
        Assert.IsTrue(json.GetValue<bool>("b"));
        Assert.AreEqual("bcd", json.GetSubstringValue("str", 1, 3));
        Assert.AreEqual("cdef", json.GetSubstringValue("str", 2));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => json.GetSubstringValue("str", -1));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetStringValue("v", true));
        Assert.AreEqual("abcdef", json.GetStringValue("str", true));
        Assert.IsNull(json.GetStringValue("n"));
        Assert.IsNull(json.GetValue(typeof(int?), "n"));
        Assert.IsNull(json.GetValue((Type)null, "v"));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetValue(typeof(TimeSpan), "v"));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetValue<int>("n"));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetObjectValue("v"));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetArrayValue("v"));
        Assert.AreEqual(JsonValueKind.Number, json.GetValue<JsonValueKind>("v"));
        Assert.AreEqual(42, json.GetValue<ValueModel>("o").Value);
        Assert.AreEqual(42, json.TryGetValue<ValueModel>("o").Value);
        Assert.AreEqual("42", json.TryGetValue<StringBuilder>("v").ToString());
        Assert.IsInstanceOfType<JsonIntegerNode>(json.GetValue(typeof(JsonIntegerNode), "v"));
        Assert.IsInstanceOfType<IJsonNumberNode>(json.GetValue(typeof(IJsonNumberNode), "v"));
        Assert.IsInstanceOfType<JsonStringNode>(json.GetValue(typeof(JsonStringNode), "str"));
        Assert.IsInstanceOfType<JsonBooleanNode>(json.GetValue(typeof(JsonBooleanNode), "b"));
        Assert.AreSame(json["o"], json.GetValue(typeof(IJsonValueNode), "o"));
        Assert.AreSame(json["o"], json.GetValue(typeof(BaseJsonValueNode), "o"));
        Assert.IsInstanceOfType<JsonObject>(json.GetValue(typeof(JsonObject), "o"));
        Assert.IsInstanceOfType<JsonArray>(json.GetValue(typeof(JsonArray), "a"));
        using var doc = json.GetValue<JsonDocument>("o");
        Assert.AreEqual(42, doc.RootElement.GetProperty("Value").GetInt32());
        foreach (var name in new[] { "UInt32", "Int32", "Int64", "Single", "Double", "Decimal", "Boolean" })
        {
            var method = typeof(JsonObjectNode).GetMethod("Get" + name + "Value", new[] { typeof(string) });
            Assert.ThrowsExactly<InvalidOperationException>(() => Invoke(method, json, new object[] { "bad" }));
        }
    }

    [TestMethod]
    public void StringsIdentifiersListsAndResolvers()
    {
        var json = JsonObjectNode.Parse("{\"text\":\" AbC \",\"empty\":\"  \",\"t\":true,\"f\":false,\"n\":42,\"null\":null,\"obj\":{\"v\":1},\"list\":[\"a\",1,true,null,{}]}");
        Assert.IsNull(json.TryGetId(out _));
        foreach (var key in new[] { "$id", "id", "ID", "Id", "_id", "uuid" })
        {
            json.SetValue(key, "identifier");
            Assert.AreEqual("identifier", json.TryGetId(out var actual, out var kind));
            Assert.AreEqual(key, actual);
            Assert.AreEqual(JsonValueKind.String, kind);
            json.Remove(key);
        }
        Assert.AreEqual(" AbC ", json.TryGetStringValue("text"));
        Assert.AreEqual("AbC", json.TryGetStringTrimmedValue("text"));
        Assert.IsNull(json.TryGetStringTrimmedValue("empty", true));
        Assert.AreEqual(string.Empty, json.TryGetStringTrimmedValue("empty"));
        Assert.IsTrue(json.TryGetStringTrimmedValue("text", out var trimmed));
        Assert.AreEqual("AbC", trimmed);
        Assert.IsTrue(json.TryGetStringTrimmedValue("text", out trimmed, out var textKind));
        Assert.AreEqual(JsonValueKind.String, textKind);
        Assert.IsTrue(json.TryGetStringValue("n", out var number, out var numberKind));
        Assert.AreEqual("42", number);
        Assert.AreEqual(JsonValueKind.Number, numberKind);
        Assert.IsFalse(json.TryGetStringValue("n", true, out _));
        Assert.IsTrue(json.TryGetStringValue("text", true, out _));
        Assert.IsFalse(json.TryGetStringValue("obj", out _, out _));
        Assert.AreEqual(" AbC ", json.TryGetStringValue(new[] { "text" }));
        Assert.IsTrue(json.TryGetStringValue(new[] { "text" }, out _));
        Assert.AreEqual("resolved", json.TryGetStringValue("obj", _ => "resolved"));
        Assert.IsNull(json.TryGetStringValue("obj", (Func<JsonObjectNode, string>)null));
        var resolver = new PropertyResolver<string>("resolved");
        Assert.AreEqual("resolved", json.TryGetStringValue("obj", resolver));
        Assert.IsTrue(json.TryGetStringValue("obj", resolver, out _));
        Assert.AreEqual("yes", json.TryGetBooleanStringValue("t", "yes", "no"));
        Assert.AreEqual("no", json.TryGetBooleanStringValue("f", "yes", "no", out var isBoolean));
        Assert.IsTrue(isBoolean);
        Assert.AreEqual(" AbC ", json.TryGetBooleanStringValue("text", "yes", "no"));
        Assert.IsNull(json.TryGetBooleanStringValue("text", "yes", "no", true));
        Assert.AreEqual(5, json.TryGetStringListValue("list").Count);
        Assert.AreEqual(1, json.TryGetStringListValue("list", true).Count);
        Assert.AreEqual(1, json.TryGetStringListValue("text").Count);
        Assert.IsNull(json.TryGetStringListValue("missing"));
        Assert.AreEqual(1, json.TryGetObjectListValue("obj").Count);
        Assert.AreEqual(5, json.TryGetObjectListValue("list").Count);
        Assert.AreEqual(1, json.TryGetObjectListValue("list", true).Count);
        var map = new Dictionary<string, int> { ["AbC"] = 7 };
        Assert.IsTrue(json.TryGetStringMappedValue("text", map, out var mapped));
        Assert.AreEqual(7, mapped);
        Assert.IsFalse(json.TryGetStringMappedValue("missing", map, out _));
        Assert.IsFalse(json.TryGetStringMappedValue("text", (IDictionary<string, int>)null, out _));
        Assert.IsTrue(json.TryGetValue(resolver, out var resolved));
        Assert.AreEqual("resolved", resolved);
        Assert.IsFalse(json.TryGetValue((IJsonPropertyResolver<string>)null, out _));
        var keyRef = "obj";
        Assert.AreSame(json["obj"], json.TryGetObjectValue(ref keyRef, null));
        Assert.AreSame(json["obj"], json.TryGetObjectValue("obj", (IJsonPropertyRoutePolicy)null));
        Assert.IsTrue(json.TryGetObjectValue("obj", null, out _, out var exact));
        Assert.AreEqual("obj", exact);
        Assert.IsTrue(json.TryGetObjectValue("alias", new RoutePolicy(), out var routed, out exact));
        Assert.AreSame(json["obj"], routed);
        Assert.AreEqual("obj", exact);
    }

    [TestMethod]
    public void DatesGuidsUrisAndBase64()
    {
        var json = new JsonObjectNode();
        json.SetValue("guid", SampleGuid);
        json.SetValue("date", SampleDate);
        json.SetValue("uri", new Uri("https://example.invalid/path"));
        json.SetValue("bad", "not a date or guid");
        json.SetValue("blank", DBNull.Value);
        json.SetBase64("bytes", new byte[] { 0, 1, 254, 255 });
        Assert.AreEqual(SampleGuid, json.GetGuidValue("guid"));
        Assert.AreEqual(SampleGuid, json.TryGetGuidValue("guid"));
        Assert.IsTrue(json.TryGetGuidValue("guid", out _));
        Assert.IsFalse(json.TryGetGuidValue("bad", out _));
        Assert.IsNull(json.TryGetGuidValue("missing"));
        Assert.AreEqual(SampleDate, json.GetDateTimeValue("date"));
        Assert.AreEqual(SampleDate, json.GetValue<DateTime>("date"));
        Assert.AreEqual(SampleDate, json.TryGetDateTimeValue("date"));
        Assert.IsTrue(json.TryGetDateTimeValue("date", out var date, out var kind));
        Assert.AreEqual(SampleDate, date);
        Assert.AreEqual(JsonValueKind.String, kind);
        Assert.IsTrue(json.TryGetDateTimeValue("date", out _));
        Assert.IsFalse(json.TryGetDateTimeValue("bad", out _));
        Assert.IsNull(json.TryGetDateTimeValue("bad"));
        Assert.AreEqual(SampleDate, json.TryGetDateTimeValue("bad", _ => SampleDate));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetDateTimeValue("bad"));
        Assert.AreEqual("https://example.invalid/path", json.TryGetUriValue("uri").OriginalString);
        Assert.IsTrue(json.TryGetUriValue("uri", out _));
        Assert.IsNotNull(json.TryGetUriValue("uri", UriKind.Absolute));
        Assert.IsNull(json.TryGetUriValue("blank"));
        Assert.IsNull(json.TryGetUriValue("missing"));
        Assert.IsFalse(json.TryGetUriValue("missing", out _));
        Assert.AreEqual(SampleGuid, json.TryGetValue<Guid>("guid"));
        Assert.AreEqual(SampleDate, json.TryGetValue<DateTime>("date"));
        Assert.IsNotNull(json.GetValue<Uri>("uri"));
        Assert.IsNotNull(json.TryGetValue<Uri>("uri"));
        CollectionAssert.AreEqual(new byte[] { 0, 1, 254, 255 }, json.GetBytesFromBase64("bytes"));
        json.SetBase64("span", new byte[] { 1, 2 }.AsSpan());
        CollectionAssert.AreEqual(new byte[] { 1, 2 }, json.GetBytesFromBase64("span"));
        Assert.ThrowsExactly<FormatException>(() => json.GetBytesFromBase64("bad"));
#if !NETFRAMEWORK
        var buffer = new byte[4];
        Assert.IsTrue(json.TryGetBytesFromBase64("bytes", buffer, out var written));
        Assert.AreEqual(4, written);
        Assert.IsFalse(json.TryGetBytesFromBase64("bytes", new byte[1], out written));
        Assert.AreEqual(0, written);
        Assert.IsFalse(json.TryGetBytesFromBase64("blank", buffer, out _));
        Assert.IsFalse(json.TryGetBytesFromBase64("bad", buffer, out _));
#endif
        json.SetJavaScriptDateTicksValue("ticks", SampleDate);
        Assert.AreEqual(SampleDate, json.GetDateTimeValue("ticks"));
        json.SetUnixTimestampValue("unix", SampleDate);
        Assert.AreEqual(SampleDate, json.GetDateTimeValue("unix", true));
        json.SetWindowsFileTimeUtcValue("file", SampleDate);
        Assert.AreEqual(SampleDate.ToFileTimeUtc(), json.GetInt64Value("file"));
        json.SetDateTimeStringValue("iso", SampleDate);
        Assert.AreEqual(SampleDate, json.GetDateTimeValue("iso"));
    }

    [TestMethod]
    public void EnumOverloads()
    {
        var json = JsonObjectNode.Parse("{\"number\":1,\"name\":\"Friday\",\"lower\":\"friday\",\"bad\":\"not-an-enum\"}");
        Assert.AreEqual(DayOfWeek.Monday, json.GetEnumValue<DayOfWeek>("number"));
        Assert.AreEqual(DayOfWeek.Friday, json.GetEnumValue<DayOfWeek>("lower", true));
        Assert.AreEqual(DayOfWeek.Friday, json.GetEnumValue(typeof(DayOfWeek), "name"));
        Assert.AreEqual(DayOfWeek.Friday, json.GetEnumValue(typeof(DayOfWeek), "lower", true));
        Assert.AreEqual(DayOfWeek.Friday, json.TryGetEnumValue<DayOfWeek>("name"));
        Assert.AreEqual(DayOfWeek.Friday, json.TryGetEnumValue<DayOfWeek>("lower", true));
        Assert.IsTrue(json.TryGetEnumValue("number", out DayOfWeek numeric));
        Assert.AreEqual(DayOfWeek.Monday, numeric);
        Assert.IsTrue(json.TryGetEnumValue("lower", true, out DayOfWeek _));
        Assert.AreEqual(DayOfWeek.Saturday, json.TryGetEnumValue("bad", DayOfWeek.Saturday));
        Assert.AreEqual(DayOfWeek.Saturday, json.TryGetEnumValue("bad", true, DayOfWeek.Saturday));
        Assert.IsFalse(json.TryGetEnumValue("bad", out DayOfWeek _));
        Assert.IsNull(json.TryGetEnumValue<DayOfWeek>("missing"));
        Assert.IsTrue(json.TryGetEnumValue(typeof(DayOfWeek), "name", out var boxed));
        Assert.AreEqual(DayOfWeek.Friday, boxed);
        Assert.IsTrue(json.TryGetEnumValue(typeof(DayOfWeek), "lower", true, out _));
        Assert.IsFalse(json.TryGetEnumValue(typeof(DayOfWeek), "bad", out _));
        Assert.IsFalse(json.TryGetEnumValue(typeof(int), "name", out _));
        Assert.IsTrue(json.TryGetEnumValue(null, "number", out boxed));
        Assert.AreEqual(1, boxed);
        Assert.IsTrue(json.TryGetEnumValue(null, "name", out boxed));
        Assert.AreEqual("Friday", boxed);
        Assert.AreEqual(DayOfWeek.Friday, json.GetValue<DayOfWeek>("name"));
        Assert.AreEqual(DayOfWeek.Monday, json.TryGetValue<DayOfWeek>("number"));
    }

    public static IEnumerable<object[]> SetterCases()
    {
        foreach (var method in typeof(JsonObjectNode).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (method.IsGenericMethod || !new[] { "Add", "SetValue", "SetValueIfNotNull", "SetValueIfNotEmpty", "SetValueOrRemove" }.Contains(method.Name)) continue;
            var p = method.GetParameters();
            if (p.Length != 2 || p[0].ParameterType != typeof(string) || !IsSampleType(p[1].ParameterType)) continue;
            yield return new object[] { method.ToString() };
        }
    }

    [TestMethod]
    [DynamicData(nameof(SetterCases))]
    public void SetterAndAddOverloads(string signature)
    {
        var method = typeof(JsonObjectNode).GetMethods().Single(m => m.ToString() == signature);
        var type = method.GetParameters()[1].ParameterType;
        var sample = CreateSample(type, out var expected);
        try
        {
            var json = new JsonObjectNode();
            Invoke(method, json, new[] { "v", sample });
            Assert.AreEqual(expected, json.GetRawText("v"), signature);
            if (method.Name == "Add")
                Assert.ThrowsExactly<ArgumentException>(() => Invoke(method, json, new[] { "v", sample }));
            else
            {
                Invoke(method, json, new[] { "v", sample });
                Assert.AreEqual(1, json.Count);
            }

            if ((!type.IsValueType || Nullable.GetUnderlyingType(type) != null) && method.Name != "Add" && method.Name != "SetValue")
            {
                Invoke(method, json, new object[] { "v", null });
                if (method.Name == "SetValueOrRemove") Assert.IsFalse(json.ContainsKey("v"));
                else Assert.AreEqual(expected, json.GetRawText("v"));
                Invoke(method, json, new object[] { "missing", null });
                Assert.IsFalse(json.ContainsKey("missing"));
            }

            if (method.Name == "SetValueIfNotEmpty")
            {
                object empty = type == typeof(string) ? string.Empty : type == typeof(StringBuilder) ? new StringBuilder()
                    : type == typeof(JsonObjectNode) ? new JsonObjectNode() : type == typeof(JsonArrayNode) ? new JsonArrayNode()
                    : type.IsGenericType ? Array.CreateInstance(type.GetGenericArguments()[0], 0) : null;
                if (empty != null)
                {
                    Invoke(method, json, new[] { "empty", empty });
                    Assert.IsFalse(json.ContainsKey("empty"));
                }
            }
        }
        finally
        {
            (sample as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    public void NullableAndFormattedSetters()
    {
        foreach (var type in new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) })
        {
            var json = new JsonObjectNode();
            var nullable = typeof(Nullable<>).MakeGenericType(type);
            var setter = typeof(JsonObjectNode).GetMethod("SetValue", new[] { typeof(string), nullable, typeof(bool) });
            var value = Convert.ChangeType(42, type, CultureInfo.InvariantCulture);
            Invoke(setter, json, new[] { "v", value, false });
            Assert.AreEqual(42M, json.GetDecimalValue("v"));
            Invoke(setter, json, new object[] { "v", null, false });
            Assert.IsTrue(json.IsNull("v"));
            Invoke(setter, json, new object[] { "v", null, true });
            Assert.IsFalse(json.ContainsKey("v"));
            Invoke(setter, json, new[] { "v", value, true });
            Assert.AreEqual(42M, json.GetDecimalValue("v"));
            var format = typeof(JsonObjectNode).GetMethod("SetValue", new[] { typeof(string), type, typeof(string), typeof(IFormatProvider) });
            Invoke(format, json, new[] { "format", value, "F2", CultureInfo.InvariantCulture });
            Assert.AreEqual("42.00", json.GetStringValue("format", true));
        }

        var node = new JsonObjectNode();
        node.SetValue("v", (int current, bool found) => found ? current + 1 : 5);
        Assert.AreEqual(5, node.GetInt32Value("v"));
        node.SetValue("v", (int current, bool found) => found ? current + 1 : 5);
        Assert.AreEqual(6, node.GetInt32Value("v"));
        node.SetValue("v", (int? current) => current + 1);
        Assert.AreEqual(7, node.GetInt32Value("v"));
        node.SetValue("v", (int? _) => null);
        Assert.AreEqual(7, node.GetInt32Value("v"));
        node.SetValue("null", (bool?)null);
        Assert.IsTrue(node.IsNull("null"));
        node.SetValue("nanF", float.NaN);
        node.SetValue("nanD", double.NaN);
        node.Add("addNanF", float.NaN);
        node.Add("addNanD", double.NaN);
        foreach (var key in new[] { "nanF", "nanD", "addNanF", "addNanD" }) Assert.IsTrue(node.IsNull(key));
        node.SetFormatValue("format", "{0}:{1}", new object[] { "a", 2 });
        Assert.AreEqual("a:2", node.GetStringValue("format"));
        node.SetValue("formattedDate", SampleDate, "yyyy-MM-dd");
        Assert.AreEqual("2024-02-29", node.GetStringValue("formattedDate"));
        node.SetValue("numericDate", 0L);
        node.SetValue("numericDate", SampleDate);
        Assert.AreEqual(JsonValueKind.Number, node.GetValueKind("numericDate"));
        Assert.AreEqual(SampleDate, node.GetDateTimeValue("numericDate"));
        node.SetValue("objectDate", new JsonObjectNode { { "day", 1 } });
        node.SetValue("objectDate", SampleDate);
        Assert.AreEqual(29, node.GetObjectValue("objectDate").GetInt32Value("day"));
        Assert.AreEqual(2024, node.GetObjectValue("objectDate").GetInt32Value("year"));
#if NET10_0_OR_GREATER
        node.SetFormatValue("spanFormat", "{0}", new object[] { 42 }.AsSpan());
        Assert.AreEqual("42", node.GetStringValue("spanFormat"));
#endif
    }

    [TestMethod]
    public void ObjectArrayFactoriesAndConverters()
    {
        var json = new JsonObjectNode();
        json.SetValue("o", out JsonObjectNode obj);
        Assert.AreSame(obj, json.GetObjectValue("o"));
        json.SetValue("o", true, out JsonObjectNode sameObj);
        Assert.AreSame(obj, sameObj);
        json.SetValue("o", false, out JsonObjectNode replacement);
        Assert.AreNotSame(obj, replacement);
        json.SetValue("newO", true, out JsonObjectNode newObj);
        Assert.AreSame(newObj, json.GetObjectValue("newO"));
        json.SetValue("parent", "child", out JsonObjectNode child);
        Assert.AreSame(child, json.GetObjectValue("parent", "child"));
        json.SetValue("a", out JsonArrayNode arr);
        Assert.AreSame(arr, json.GetArrayValue("a"));
        json.SetValue("a", true, out JsonArrayNode sameArr);
        Assert.AreSame(arr, sameArr);
        json.SetValue("a", false, out JsonArrayNode replacementArr);
        Assert.AreNotSame(arr, replacementArr);
        json.SetValue("newA", true, out JsonArrayNode newArr);
        Assert.AreSame(newArr, json.GetArrayValue("newA"));
        var source = new JsonObjectNode { { "v", 42 } };
        json.SetValue("copy", source, "v");
        Assert.AreEqual(42, json.GetInt32Value("copy"));
        json.SetValue("copy", source, "missing");
        json.SetValue("items", new[] { 1, 2 }, i => new JsonObjectNode { { "v", i } });
        Assert.AreEqual(2, json.GetArrayValue("items").Count);
        Assert.AreEqual(2, json["items", 1, "v"].TryConvert<int>());
        json.SetValueIfNotNull("items", (IEnumerable<int>)null, i => new JsonObjectNode());
        Assert.AreEqual(2, json.GetArrayValue("items").Count);
        json.SetValueIfNotNull("items", new[] { 3 }, i => new JsonObjectNode { { "v", i } });
        Assert.AreEqual(3, json["items", 0, "v"].TryConvert<int>());
        json.SetValueIfNotEmpty("items", Array.Empty<int>(), i => new JsonObjectNode());
        Assert.AreEqual(1, json.GetArrayValue("items").Count);
        json.SetValueIfNotEmpty("items", new[] { 4 }, i => new JsonObjectNode { { "v", i } });
        Assert.AreEqual(4, json["items", 0, "v"].TryConvert<int>());
        Assert.ThrowsExactly<ArgumentNullException>(() => json.SetValue("items", new[] { 1 }, (Func<int, JsonObjectNode>)null));
        using var doc = JsonDocument.Parse("{\"fromProperty\":42}");
        json.SetValue(doc.RootElement.EnumerateObject().Single());
        Assert.AreEqual(42, json.GetInt32Value("fromProperty"));
        json.Add("self", json);
        Assert.AreSame(json, json.GetObjectValue("self"));
        json.SetValue("setSelf", json);
        Assert.AreSame(json, json.GetObjectValue("setSelf"));
        Assert.AreEqual(1, ((JsonObjectNode)json["self"]).Count);
    }

    public static IEnumerable<object[]> RangeCases()
    {
        foreach (var method in typeof(JsonObjectNode).GetMethods().Where(m => m.Name == "SetRange" && !m.IsGenericMethod))
        {
            var p = method.GetParameters();
            if (p.Length != 2 || p[1].ParameterType != typeof(bool) || !p[0].ParameterType.IsGenericType) continue;
            var sequence = p[0].ParameterType;
            if (sequence.GetGenericTypeDefinition() != typeof(IEnumerable<>)) continue;
            var pair = sequence.GetGenericArguments()[0];
            if (pair.IsGenericType && pair.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) && IsSampleType(pair.GetGenericArguments()[1]))
                yield return new object[] { method.ToString() };
        }
    }

    [TestMethod]
    [DynamicData(nameof(RangeCases))]
    public void EnumerableRangeOverloads(string signature)
    {
        var method = typeof(JsonObjectNode).GetMethods().Single(m => m.ToString() == signature);
        var pairType = method.GetParameters()[0].ParameterType.GetGenericArguments()[0];
        var sample = CreateSample(pairType.GetGenericArguments()[1], out var expected);
        try
        {
            var pairs = Array.CreateInstance(pairType, 3);
            pairs.SetValue(Activator.CreateInstance(pairType, "v", sample), 0);
            pairs.SetValue(Activator.CreateInstance(pairType, " ", sample), 1);
            pairs.SetValue(Activator.CreateInstance(pairType, "extra", sample), 2);
            var json = new JsonObjectNode();
            Assert.AreEqual(0, Invoke(method, json, new object[] { null, false }));
            Assert.AreEqual(2, Invoke(method, json, new object[] { pairs, false }));
            Assert.AreEqual(expected, json.GetRawText("v"));
            Assert.AreEqual(0, Invoke(method, json, new object[] { pairs, true }));
            json.SetValue("v", "old");
            Assert.AreEqual(0, Invoke(method, json, new object[] { pairs, true }));
            Assert.AreEqual("old", json.GetStringValue("v"));
            Assert.AreEqual(2, Invoke(method, json, new object[] { pairs, false }));
            Assert.AreEqual(expected, json.GetRawText("v"));
        }
        finally
        {
            (sample as IDisposable)?.Dispose();
        }
    }

    [TestMethod]
    public void PositionalRangesAndMappings()
    {
        foreach (var method in typeof(JsonObjectNode).GetMethods().Where(m => m.Name == "SetRange" && m.GetParameters()[0].ParameterType == typeof(string)))
        {
            var parameters = method.GetParameters();
            var json = new JsonObjectNode();
            var args = new object[parameters.Length];
            for (var i = 0; i < args.Length; i += 2)
            {
                args[i] = "key" + i;
                args[i + 1] = CreateSample(parameters[i + 1].ParameterType, out _);
            }

            Assert.AreEqual(args.Length / 2, Invoke(method, json, args));
            Assert.AreEqual(args.Length / 2, json.Count);
            for (var i = 0; i < args.Length; i += 2)
            {
                CreateSample(parameters[i + 1].ParameterType, out var expected);
                Assert.AreEqual(expected, json.GetRawText((string)args[i]));
            }

            for (var i = 4; i < args.Length; i++) if (parameters[i].HasDefaultValue) args[i] = parameters[i].DefaultValue;
            json.Clear();
            var requiredCount = parameters.Count(p => !p.HasDefaultValue) / 2;
            Assert.AreEqual(requiredCount, Invoke(method, json, args));
        }

        var source = JsonObjectNode.Parse("{\"a\":1,\"b\":2}");
        var target = new JsonObjectNode { { "a", 9 } };
        Assert.AreEqual(1, target.SetRange(source, true));
        Assert.AreEqual(9, target.GetInt32Value("a"));
        Assert.AreEqual(2, target.SetRange(source));
        Assert.AreEqual(1, target.GetInt32Value("a"));
        Assert.AreEqual(2, target.SetRange(target));
        Assert.AreEqual(0, target.SetRange((JsonObjectNode)null));
        target.SetRange(source, new[] { "a", "missing" });
        Assert.IsFalse(target.ContainsKey("missing"));
        target.SetRange(source, new Dictionary<string, string> { ["a"] = "renamed", ["absent"] = "removed" });
        Assert.IsFalse(target.ContainsKey("removed"));
        var array = new JsonArrayNode { 3, 4 };
        Assert.AreEqual(2, target.SetRange(array));
        Assert.AreEqual(3, target.GetInt32Value("0"));
        Assert.AreEqual(2, target.SetRange(array, new[] { "x", "y", "unused" }));
        Assert.AreEqual(4, target.GetInt32Value("y"));
        target.SetRange(array, new[] { "x", "new" }, true);
        Assert.AreEqual(4, target.GetInt32Value("new"));
        Assert.AreEqual(2, target.SetRange(new[] { "one", "two" }, new[] { "x", "y" }));
        Assert.AreEqual("one", target.GetStringValue("x"));
        target.SetRange(new[] { "changed", "new" }, new[] { "x", "newString" }, true);
        Assert.AreEqual("one", target.GetStringValue("x"));
        Assert.AreEqual("new", target.GetStringValue("newString"));
    }

    [TestMethod]
    public void EnsureAndAppendBranches()
    {
        var json = new JsonObjectNode();
        Assert.IsTrue(json.EnsureObjectValue("o", out var old));
        Assert.AreEqual(JsonValueKind.Undefined, old);
        var original = json.GetObjectValue("o");
        Assert.IsTrue(json.EnsureObjectValue("o", false, out old));
        Assert.AreSame(original, json.GetObjectValue("o"));
        Assert.AreEqual(JsonValueKind.Object, old);
        json.SetValue("o", 1);
        Assert.IsFalse(json.EnsureObjectValue("o", out old));
        Assert.AreEqual(JsonValueKind.Number, old);
        Assert.IsTrue(json.EnsureObjectValue("o", true, out _));
        Assert.IsTrue(json.EnsureArrayValue("a", false, out old));
        Assert.AreEqual(JsonValueKind.Undefined, old);
        Assert.IsTrue(json.EnsureArrayValue("a", false, out old));
        Assert.AreEqual(JsonValueKind.Array, old);
        json.SetValue("a", 1);
        Assert.IsFalse(json.EnsureArrayValue("a", out _));
        Assert.IsFalse(json.EnsureArrayValue("a", false, out _));
        Assert.IsTrue(json.EnsureArrayValue("a", true, out _));
        Assert.IsTrue(json.EnsureValue("v", new JsonIntegerNode(42), out old));
        Assert.AreEqual(JsonValueKind.Undefined, old);
        Assert.IsTrue(json.EnsureValue("v", new JsonIntegerNode(99), false, out _));
        Assert.AreEqual(42, json.GetInt32Value("v"));
        Assert.IsFalse(json.EnsureValue("v", new JsonStringNode("text"), out _));
        Assert.IsTrue(json.EnsureValue("v", new JsonStringNode("text"), true, out _));
        Assert.AreEqual("text", json.GetStringValue("v"));
        Assert.AreSame(json["v"], json.EnsureValue("v", (Func<BaseJsonValueNode, bool>)null, JsonValues.Null));
        Assert.AreSame(json["v"], json.EnsureValue("v", _ => false, JsonValues.Null));
        json.EnsureValue("v", _ => true, JsonValues.Undefined);
        Assert.IsFalse(json.ContainsKey("v"));
        json.EnsureValue("v", (key, value) => key == "v" && value == null, new JsonIntegerNode(7));
        Assert.AreEqual(7, json.GetInt32Value("v"));
        json.EnsureValue("v", (key, value) => false, JsonValues.Null);
        Assert.AreEqual(7, json.GetInt32Value("v"));
        json.EnsureValue("v", (key, value) => true, null);
        Assert.IsFalse(json.ContainsKey("v"));
        Assert.AreEqual("a", json.AppendValue("text", "a"));
        Assert.AreEqual("ab", json.AppendValue("text", new StringBuilder("b")));
        Assert.AreEqual("ab2", json.AppendValueFormat("text", "{0}", 2));
        json.SetNullValue("null");
        Assert.AreEqual("x", json.AppendValue("null", "x"));
        json.SetValue("n", 42);
        Assert.AreEqual("42x", json.AppendValue("n", "x"));
        json.SetValue("empty", new JsonArrayNode());
        Assert.AreEqual("x", json.AppendValue("empty", "x"));
        Assert.AreEqual(1, json.GetArrayValue("empty").Count);
        Assert.ThrowsExactly<InvalidOperationException>(() => json.AppendValue("empty", "x"));
        Assert.ThrowsExactly<InvalidOperationException>(() => json.AppendValue("o", "x"));
    }

    [TestMethod]
    [DataRow("int")]
    [DataRow("long")]
    [DataRow("double")]
    public void ArithmeticBranches(string type)
    {
        var valueType = type == "int" ? typeof(int) : type == "long" ? typeof(long) : typeof(double);
        var increase = typeof(JsonObjectNode).GetMethod("IncreaseValue", new[] { typeof(string), valueType });
        var decrease = typeof(JsonObjectNode).GetMethod("DecreaseValue", new[] { typeof(string), valueType });
        var operand = Convert.ChangeType(2, valueType, CultureInfo.InvariantCulture);
        foreach (var initial in new[] { "missing", "null", "40", "40.5", "\"40\"", "\"40.5\"", "\"\"" })
        {
            var json = initial == "missing" ? new JsonObjectNode() : JsonObjectNode.Parse("{\"v\":" + initial + "}");
            var baseline = initial.Contains("40.5") ? 40.5M : initial.Contains("40") ? 40M : 0M;
            Invoke(increase, json, new[] { "v", operand });
            Assert.AreEqual(baseline + 2, json.GetDecimalValue("v"), initial);
            Invoke(decrease, json, new[] { "v", operand });
            Assert.AreEqual(baseline, json.GetDecimalValue("v"), initial);
        }

        var decimalNode = new JsonObjectNode();
        decimalNode.SetValue("v", 40.5M);
        Invoke(increase, decimalNode, new[] { "v", operand });
        Assert.AreEqual(42.5M, decimalNode.GetDecimalValue("v"));
        var arrayNode = new JsonObjectNode { { "v", new JsonArrayNode() } };
        Invoke(increase, arrayNode, new[] { "v", operand });
        Assert.AreEqual(2, arrayNode["v", 0].TryConvert<int>());
        foreach (var initial in new[] { "{}", "[1]", "true", "\"bad\"" })
        {
            var json = JsonObjectNode.Parse("{\"v\":" + initial + "}");
            Assert.ThrowsExactly<InvalidOperationException>(() => Invoke(increase, json, new[] { "v", operand }), initial);
        }
    }

    [TestMethod]
    public void DictionaryInterfacesAndEvents()
    {
        var json = new JsonObjectNode();
        var changed = new List<string>();
        var notified = new List<string>();
        json.PropertyChanged += (_, e) => changed.Add(e.Key);
        PropertyChangedEventHandler notify = (_, e) => notified.Add(e.PropertyName);
        ((INotifyPropertyChanged)json).PropertyChanged += notify;
        var revision = json.RevisionToken;
        IDictionary<string, BaseJsonValueNode> dict = json;
        IDictionary<string, IJsonValueNode> iface = json;
        IReadOnlyDictionary<string, BaseJsonValueNode> readOnly = json;
        IReadOnlyDictionary<string, IJsonValueNode> readOnlyIface = json;
        dict.Add("a", new JsonIntegerNode(1));
        Assert.AreNotSame(revision, json.RevisionToken);
        iface.Add("b", new JsonStringNode("two"));
        iface["c"] = JsonBooleanNode.True;
        Assert.AreSame(iface["c"], readOnlyIface["c"]);
        Assert.AreSame(dict["a"], readOnly["a"]);
        Assert.AreEqual(3, dict.Values.Count);
        Assert.AreEqual(3, iface.Values.Count);
        Assert.AreEqual(3, readOnly.Keys.Count());
        Assert.AreEqual(3, readOnly.Values.Count());
        Assert.AreEqual(3, readOnlyIface.Keys.Count());
        Assert.AreEqual(3, readOnlyIface.Values.Count());
        Assert.IsTrue(iface.TryGetValue("a", out var value));
        Assert.AreSame(dict["a"], value);
        Assert.IsFalse(iface.TryGetValue("missing", out _));
        Assert.IsTrue(readOnlyIface.TryGetValue("a", out _));
        Assert.IsFalse(readOnlyIface.TryGetValue("missing", out _));
        var pairs = new KeyValuePair<string, BaseJsonValueNode>[4];
        json.CopyTo(pairs, 1);
        Assert.AreEqual("a", pairs[1].Key);
        var interfacePairs = new KeyValuePair<string, IJsonValueNode>[3];
        json.CopyTo(interfacePairs, 0);
        Assert.AreEqual("a", interfacePairs[0].Key);
        Assert.ThrowsExactly<ArgumentNullException>(() => json.CopyTo((KeyValuePair<string, BaseJsonValueNode>[])null, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => json.CopyTo(pairs, -1));
        Assert.ThrowsExactly<ArgumentException>(() => json.CopyTo(new KeyValuePair<string, BaseJsonValueNode>[1], 0));
        Assert.IsTrue(json.Contains(new KeyValuePair<string, BaseJsonValueNode>("a", new JsonIntegerNode(1))));
        Assert.IsTrue(json.Contains(new KeyValuePair<string, IJsonValueNode>("a", new JsonIntegerNode(1))));
        Assert.IsTrue(json.Contains("a", new JsonIntegerNode(1)));
        Assert.IsFalse(json.Contains("a", new JsonIntegerNode(2)));
        Assert.IsFalse(json.Contains(null, new JsonIntegerNode(1)));
        Assert.IsTrue(json.Contains(new JsonObjectNode { { "a", 1 } }));
        Assert.IsFalse(json.Contains((JsonObjectNode)null));
        Assert.IsFalse(json.Contains(new JsonObjectNode { { "a", 9 } }));
        Assert.IsTrue(json.Remove(new KeyValuePair<string, BaseJsonValueNode>("a", new JsonIntegerNode(1))));
        Assert.IsFalse(json.Remove(new KeyValuePair<string, BaseJsonValueNode>("a", new JsonIntegerNode(99))));
        Assert.IsTrue(json.Remove(new KeyValuePair<string, IJsonValueNode>("b", new JsonStringNode("two"))));
        Assert.IsFalse(json.Remove(new KeyValuePair<string, IJsonValueNode>("b", new JsonStringNode("absent"))));
        revision = json.RevisionToken;
        Assert.IsFalse(json.Remove("missing"));
        Assert.AreSame(revision, json.RevisionToken);
        json.Clear();
        Assert.AreNotSame(revision, json.RevisionToken);
        CollectionAssert.AreEqual(new[] { "a", "b", "c", "a", "b", "c" }, changed);
        CollectionAssert.AreEqual(changed, notified);
        ((INotifyPropertyChanged)json).PropertyChanged -= notify;
        json.SetValue("silent", 1);
        Assert.AreEqual(6, notified.Count);
        json.Clear();
        Assert.AreEqual(0, json.Count);
    }

    [TestMethod]
    public void RemoveRangesAndThreadSafeClone()
    {
        var json = JsonObjectNode.Parse("{\"a\":1,\"b\":2,\"c\":3,\"d\":4,\"e\":5}");
        Assert.AreEqual(0, json.Remove((IEnumerable<string>)null));
        Assert.AreEqual(1, json.Remove(new[] { "a", null, " ", "missing" }));
        Assert.AreEqual(1, json.Remove((IEnumerable<string>)new[] { "b", "b" }));
        Assert.IsTrue(json.Remove("c".AsSpan()));
        Assert.AreEqual(1, json.RemoveRange("d", "missing"));
        Assert.AreEqual(1, json.Remove(new[] { "e" }.AsSpan()));
        Assert.IsFalse(json.Any());
#if NET10_0_OR_GREATER
        json.SetValue("span", 1);
        Assert.AreEqual(1, json.RemoveRange(new[] { "span" }.AsSpan()));
#endif
        json = JsonObjectNode.Parse("{\"o\":{\"v\":1},\"a\":[{\"v\":2}],\"v\":3}");
        json.EnableThreadSafeMode(-1);
        json.EnableThreadSafeMode();
        json.EnableThreadSafeMode(2);
        json.EnableThreadSafeMode(2, true);
        var clone = json.Clone();
        Assert.AreEqual(json, clone);
        Assert.AreNotSame(json, clone);
        clone.SetValue("v", 4);
        Assert.AreEqual(3, json.GetInt32Value("v"));
        var selected = json.Clone(new[] { "v", "missing" });
        Assert.AreEqual(1, selected.Count);
        Assert.AreEqual(3, selected.GetInt32Value("v"));
        Assert.IsInstanceOfType<JsonObjectNode>(((ICloneable)json).Clone());
        Parallel.For(0, 20, i => json.SetValue("key" + i, i));
        for (var i = 0; i < 20; i++) Assert.AreEqual(i, json.GetInt32Value("key" + i));
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("{\"s\":\"中文\\n\\\"\\\\\",\"i\":1,\"l\":9223372036854775807,\"d\":1.25,\"t\":true,\"f\":false,\"n\":null,\"o\":{},\"a\":[1,{}]}")]
    public void ParseOverloadsAndJsonConversions(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var canonical = JsonObjectNode.Parse(text).ToString();
        Assert.AreEqual(canonical, JsonObjectNode.Parse(new ReadOnlyMemory<byte>(bytes)).ToString());
        Assert.AreEqual(canonical, JsonObjectNode.Parse(new ReadOnlyMemory<char>(text.ToCharArray())).ToString());
        Assert.AreEqual(canonical, JsonObjectNode.Parse(new ReadOnlySequence<byte>(bytes), new JsonDocumentOptions()).ToString());
        Assert.AreEqual(canonical, JsonObjectNode.Parse(new ReadOnlySequence<byte>(bytes), new JsonReaderOptions()).ToString());
        Assert.AreEqual(canonical, JsonObjectNode.Parse(new ReadOnlySpan<byte>(bytes)).ToString());
        var reader = new Utf8JsonReader(bytes);
        Assert.AreEqual(canonical, JsonObjectNode.ParseValue(ref reader).ToString());
        using var stream = new MemoryStream(bytes);
        Assert.AreEqual(canonical, JsonObjectNode.Parse(stream).ToString());
        stream.Position = 0;
        Assert.AreEqual(canonical, JsonObjectNode.TryParse(stream).ToString());
        Assert.AreEqual(canonical, JsonObjectNode.TryParse(text).ToString());
        using var doc = JsonDocument.Parse(text);
        JsonObjectNode fromDocument = doc;
        JsonObjectNode fromElement = doc.RootElement;
        JsonObjectNode fromObject = (JsonObject)JsonNode.Parse(text);
        JsonObjectNode fromNode = JsonNode.Parse(text);
        foreach (var node in new[] { fromDocument, fromElement, fromObject, fromNode })
        {
            Assert.AreEqual(canonical, node.ToString());
            using var convertedDocument = (JsonDocument)node;
            Assert.AreEqual(canonical, ((JsonObjectNode)convertedDocument).ToString());
            Assert.AreEqual(canonical, ((JsonObjectNode)(JsonObject)node).ToString());
            Assert.AreEqual(canonical, ((JsonObjectNode)(JsonNode)node).ToString());
            Assert.AreEqual(canonical, ((JsonObjectNode)node.ToJsonObject()).ToString());
            Assert.AreEqual(canonical, ((JsonObjectNode)node.ToJsonNode()).ToString());
        }
#if NET10_0_OR_GREATER
        Assert.AreEqual(canonical, ParseGeneric<JsonObjectNode>(text).ToString());
        Assert.IsTrue(TryParseGeneric<JsonObjectNode>(text, out var parsed));
        Assert.AreEqual(canonical, parsed.ToString());
        Assert.IsFalse(TryParseGeneric<JsonObjectNode>("bad", out _));
#endif
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("{")]
    [DataRow("{\"x\":}")]
    [DataRow("[]")]
    [DataRow("42")]
    [DataRow("true")]
    [DataRow("\"text\"")]
    public void InvalidJsonIsRejected(string text)
    {
        Assert.IsNull(JsonObjectNode.TryParse(text));
        Assert.Throws<JsonException>(() => JsonObjectNode.Parse(text));
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        Assert.IsNull(JsonObjectNode.TryParse(stream));
    }

    [TestMethod]
    public void NullParsingAndInvalidNodeConversions()
    {
        Assert.IsNull(JsonObjectNode.TryParse((string)null));
        Assert.IsNull(JsonObjectNode.TryParse((Stream)null));
        Assert.IsNull(JsonObjectNode.TryParse((FileInfo)null));
        Assert.IsNull(JsonObjectNode.Parse((FileInfo)null));
        Assert.IsNull(JsonObjectNode.Parse("null"));
        Assert.IsNull((JsonObjectNode)(JsonDocument)null);
        Assert.IsNull((JsonObjectNode)default(JsonElement));
        Assert.IsNull((JsonObjectNode)(JsonObject)null);
        Assert.IsNull((JsonObjectNode)(JsonNode)null);
        Assert.IsNull((JsonDocument)(JsonObjectNode)null);
        Assert.IsNull((JsonObject)(JsonObjectNode)null);
        Assert.IsNull((JsonNode)(JsonObjectNode)null);
        Assert.Throws<JsonException>(() => _ = (JsonObjectNode)(JsonNode)new JsonArray(1));
        using var doc = JsonDocument.Parse("[1]");
        Assert.Throws<JsonException>(() => _ = (JsonObjectNode)doc.RootElement);
        using var closed = new MemoryStream();
        closed.Close();
        Assert.IsNull(JsonObjectNode.TryParse(closed));
        Assert.IsNotNull(JsonObjectNode.TryParse("{}", new JsonDocumentOptions()));
    }

    [TestMethod]
    public void PatchAndReaderBranches()
    {
        var json = new JsonObjectNode { { "keep", 9 } };
        Assert.AreEqual(0, json.PatchParse((string)null));
        Assert.AreEqual(0, json.PatchParse(" "));
        Assert.AreEqual(0, json.PatchParse((Stream)null));
        Assert.AreEqual(0, json.SetRange((JsonDocument)null));
        Assert.AreEqual(0, json.SetRange(default(JsonElement)));
        using var nullDoc = JsonDocument.Parse("null");
        Assert.AreEqual(0, json.SetRange(nullDoc.RootElement));
        using var arrayDoc = JsonDocument.Parse("[]");
        Assert.Throws<JsonException>(() => json.SetRange(arrayDoc.RootElement));
        Assert.AreEqual(2, json.PatchParse("{\"a\":1,\"b\":null}"));
        Assert.AreEqual(9, json.GetInt32Value("keep"));
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"a\":2}"));
        Assert.AreEqual(1, json.PatchParse(stream));
        Assert.AreEqual(2, json.GetInt32Value("a"));
        using var objectDoc = JsonDocument.Parse("{\"a\":3}");
        Assert.AreEqual(1, json.SetRange(objectDoc));

        //var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes("{/*comment*/\"a\":4,\"s\":\"text\",\"l\":2147483648,\"d\":1.25,\"t\":true,\"f\":false,\"o\":{},\"arr\":[],\"n\":null}"), new JsonReaderOptions { CommentHandling = JsonCommentHandling.Allow });
        //json.SetRange(ref reader, true);
        //Assert.AreEqual(3, json.GetInt32Value("a"));
        //Assert.AreEqual("text", json.GetStringValue("s"));
        //Assert.AreEqual(2147483648L, json.GetInt64Value("l"));
        //Assert.AreEqual(1.25D, json.GetDoubleValue("d"));
        //Assert.IsTrue(json.GetBooleanValue("t"));
        //Assert.IsFalse(json.GetBooleanValue("f"));
        //Assert.IsNotNull(json.GetObjectValue("o"));
        //Assert.IsNotNull(json.GetArrayValue("arr"));
        //Assert.IsTrue(json.IsNull("n"));
    }

    [TestMethod]
    public void JsonFormattingWriterAndYaml()
    {
        var json = JsonObjectNode.Parse("{\"z\":1,\"$id\":\"id\",\"text\":\"line\\nnext\",\"n\":null,\"a\":[1,\"x\"],\"o\":{\"v\":true},\"special:key\":\"#value\"}");
        Assert.IsTrue(json.ToString().StartsWith("{\"$id\":"));
        foreach (IndentStyles style in Enum.GetValues(typeof(IndentStyles)))
        {
            var serialized = json.ToString(style);
            Assert.AreEqual(json.Count, JsonObjectNode.Parse(serialized).Count);
            Assert.AreEqual("line\nnext", JsonObjectNode.Parse(serialized).GetStringValue("text"));
            var filtered = JsonObjectNode.Parse(json.ToString(new[] { "z", "z", "missing", "a" }, true, style));
            Assert.AreEqual(2, filtered.Count);
            Assert.AreEqual(1, filtered.GetInt32Value("z"));
            Assert.AreEqual(json.Count, JsonObjectNode.Parse(json.ToString(new[] { "z" }, false, style)).Count);
            Assert.AreEqual(0, JsonObjectNode.Parse(new JsonObjectNode().ToString(style)).Count);
        }

        foreach (var only in new[] { false, true })
        {
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream)) json.WriteTo(writer, new[] { "z", "z", "missing", "a" }, only);
            var output = Encoding.UTF8.GetString(stream.ToArray());
            Assert.IsTrue(output.StartsWith("{\"z\":"));
            Assert.AreEqual(only ? 2 : json.Count, JsonObjectNode.Parse(output).Count);
        }

        using var defaultStream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(defaultStream)) json.WriteTo(writer, null);
        Assert.IsTrue(Encoding.UTF8.GetString(defaultStream.ToArray()).StartsWith("{\"$id\":"));
        json.WriteTo((Utf8JsonWriter)null);
        json.WriteTo((Utf8JsonWriter)null, new[] { "z" });
        var yaml = json.ToYamlString();
        StringAssert.Contains(yaml, "z: 1");
        StringAssert.Contains(yaml, "n: ~");
        StringAssert.Contains(yaml, "special:key");
        Assert.IsNotNull(new JsonObjectNode().ToYamlString());
        Assert.AreEqual(json.ToString(), JsonSerializer.Serialize(json));
        Assert.AreEqual(json.Count, JsonSerializer.Deserialize<JsonObjectNode>(json.ToString()).Count);
    }

    [TestMethod]
    public async Task FileAndAsyncStreamRoundTrips()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Trivial.JsonObjectNodeTest." + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var file = new FileInfo(Path.Combine(directory, "object.json"));
            var absent = new FileInfo(Path.Combine(directory, "absent.json"));
            var json = new JsonObjectNode { { "Value", 42 }, { "extra", "中文" } };
            Assert.IsNull(JsonObjectNode.Parse(absent));
            Assert.IsNull(JsonObjectNode.TryParse(absent));
            Assert.IsNull(await JsonObjectNode.ParseAsync(absent));
            Assert.IsNull(await JsonObjectNode.ParseAsync((FileInfo)null));
            json.WriteTo(file.FullName);
            file.Refresh();
            Assert.AreEqual(42, JsonObjectNode.Parse(file).GetInt32Value("Value"));
            json.WriteTo(file);
            Assert.AreEqual(42, JsonObjectNode.TryParse(file).GetInt32Value("Value"));
            json.WriteTo(file.FullName, new[] { "Value" }, true);
            Assert.AreEqual(1, JsonObjectNode.Parse(file).Count);
            json.WriteTo(file, new[] { "Value" }, true);
            Assert.AreEqual(1, JsonObjectNode.Parse(file).Count);
            Assert.IsTrue(json.TryWriteTo(file.FullName));
            Assert.IsTrue(json.TryWriteTo(file));
            Assert.IsFalse(json.TryWriteTo((string)null));
            Assert.IsFalse(json.TryWriteTo((FileInfo)null));
            Assert.IsFalse(json.TryWriteTo(directory));
            Assert.IsFalse(json.TryWriteTo(new FileInfo(Path.Combine(directory, "missing", "file.json"))));
            Assert.ThrowsExactly<ArgumentNullException>(() => json.WriteTo((FileInfo)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => json.WriteTo((FileInfo)null, new[] { "Value" }));
            Assert.AreEqual(42, (await JsonObjectNode.ParseAsync(file)).GetInt32Value("Value"));
            Assert.AreEqual(42, JsonObjectNode.ConvertFrom(file).GetInt32Value("Value"));
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json.ToString()));
            Assert.AreEqual(42, (await JsonObjectNode.ParseAsync(stream)).GetInt32Value("Value"));
            stream.Position = 0;
            Assert.AreEqual(42, (await JsonObjectNode.ParseAsync(stream, new JsonDocumentOptions())).GetInt32Value("Value"));
#if NET10_0_OR_GREATER
            await json.WriteToAsync(file.FullName);
            await json.WriteToAsync(file);
            await json.WriteToAsync(file.FullName, new[] { "Value" }, true);
            Assert.AreEqual(1, JsonObjectNode.Parse(file).Count);
            await json.WriteToAsync(file, new[] { "Value" }, true);
            Assert.AreEqual(1, JsonObjectNode.Parse(file).Count);
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => json.WriteToAsync((FileInfo)null));
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => json.WriteToAsync((FileInfo)null, new[] { "Value" }));
            await Assert.ThrowsAsync<OperationCanceledException>(() => json.WriteToAsync(file.FullName, cancellationToken: new CancellationToken(true)));
#endif
#if !NETFRAMEWORK
            using var archiveStream = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(archiveStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                using var entryStream = archive.CreateEntry("object.json").Open();
                var bytes = Encoding.UTF8.GetBytes(json.ToString());
                entryStream.Write(bytes, 0, bytes.Length);
            }

            archiveStream.Position = 0;
            using var zip = new System.IO.Compression.ZipArchive(archiveStream, System.IO.Compression.ZipArchiveMode.Read, true);
            Assert.AreEqual(42, (await JsonObjectNode.ParseAsync(zip, "object.json")).GetInt32Value("Value"));
            Assert.AreEqual(42, (await JsonObjectNode.ParseAsync(zip, "object.json", new JsonDocumentOptions())).GetInt32Value("Value"));
            Assert.IsNull(await JsonObjectNode.ParseAsync(zip, "absent"));
            Assert.IsNull(await JsonObjectNode.ParseAsync((System.IO.Compression.ZipArchive)null, "absent"));
#endif
            File.WriteAllText(file.FullName, "invalid");
            Assert.IsNull(JsonObjectNode.TryParse(file));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void DeserializeAndConvertFromBranches()
    {
        var json = JsonObjectNode.Parse("{\"Value\":42,\"o\":{\"Value\":7},\"s\":\"123\",\"null\":null,\"bad\":\"bad\"}");
        Assert.AreEqual(42, json.Deserialize<ValueModel>().Value);
        Assert.AreEqual(42, ((ValueModel)json.Deserialize(typeof(ValueModel))).Value);
        Assert.AreEqual(7, json.DeserializeValue<ValueModel>("o").Value);
        Assert.AreEqual(7, ((ValueModel)json.DeserializeValue("o", typeof(ValueModel))).Value);
        Assert.AreEqual(123, json.DeserializeValue("s", int.Parse));
        Assert.AreEqual(42, json.DeserializeValue("Value", (Func<string, int>)null));
        Assert.IsNull(json.DeserializeValue<ValueModel>("null"));
        Assert.IsNull(json.DeserializeValue("null", typeof(ValueModel)));
        Assert.IsNull(json.DeserializeValue("null", (Func<string, ValueModel>)(_ => new ValueModel())));
        Assert.IsTrue(json.TryDeserializeValue("o", null, out ValueModel result));
        Assert.AreEqual(7, result.Value);
        Assert.IsTrue(json.TryDeserializeValue("null", null, out result));
        Assert.IsNull(result);
        Assert.IsFalse(json.TryDeserializeValue("null", null, out int _));
        Assert.IsFalse(json.TryDeserializeValue("missing", null, out result));
        Assert.IsFalse(json.TryDeserializeValue("bad", null, out result));
        Assert.IsNull(JsonObjectNode.ConvertFrom(null));
        Assert.IsNull(JsonObjectNode.ConvertFrom(DBNull.Value));
        Assert.IsNull(JsonObjectNode.ConvertFrom(JsonValues.Null));
        Assert.IsNull(JsonObjectNode.ConvertFrom(JsonValues.Undefined));
        Assert.IsNull(JsonObjectNode.ConvertFrom(JsonBooleanNode.False));
        Assert.AreEqual(0, JsonObjectNode.ConvertFrom(JsonBooleanNode.True).Count);
        Assert.AreSame(json, JsonObjectNode.ConvertFrom(json));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(new ValueModel { Value = 42 }).GetInt32Value("Value"));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom("{\"Value\":42}").GetInt32Value("Value"));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(new StringBuilder("{\"Value\":42}")).GetInt32Value("Value"));
        Assert.AreEqual(1, JsonObjectNode.ConvertFrom(new ObjectHost()).GetInt32Value("v"));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(new JsonObject { ["Value"] = 42 }).GetInt32Value("Value"));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(new JsonArrayNode { 42 }).GetInt32Value("0"));
        using var doc = JsonDocument.Parse("{\"Value\":42}");
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(doc).GetInt32Value("Value"));
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"Value\":42}"));
        Assert.AreEqual(42, JsonObjectNode.ConvertFrom(stream).GetInt32Value("Value"));
        var dictionary = new Dictionary<string, object> { ["nested"] = new ValueModel { Value = 42 }, ["null"] = null, [" "] = new object() };
        var converted = JsonObjectNode.ConvertFrom(dictionary);
        Assert.AreEqual(2, converted.Count);
        Assert.AreEqual(42, converted.GetObjectValue("nested").GetInt32Value("Value"));
        Assert.IsTrue(converted.IsNull("null"));
    }

    [TestMethod]
    public void FilteringGroupingEnumerationAndEquality()
    {
        var json = JsonObjectNode.Parse("{\"a\":1,\"b\":2,\"s\":\"text\",\"n\":null,\"t\":true,\"f\":false,\"o\":{},\"arr\":[]}");
        var groups = json.GetJsonValueKindGroups();
        Assert.AreEqual(7, groups.Count);
        CollectionAssert.AreEqual(new[] { "a", "b" }, groups[JsonValueKind.Number]);
        Assert.AreEqual(json.Count, json.Select((kind, value, key) => key).Count());
        Assert.AreEqual(2, json.Where((kind, value, key, index) => kind == JsonValueKind.Number).Count);
        Assert.AreEqual(2, json.Where(new[] { "a", "n", "missing" }).Count);
        Assert.AreEqual(1, json.Where(new[] { "a", "n", "missing" }, true).Count);
        Assert.AreEqual(2, json.Where(JsonValueKind.Number).Count);
        Assert.AreEqual(1, json.Where(JsonValueKind.Number, (_, key, index) => index == 0).Count);
        Assert.AreEqual(1, json.Where(JsonValueKind.Null).Count);
        Assert.AreEqual(0, json.Where(JsonValueKind.Undefined).Count);
        Assert.AreEqual(json.Count, json.Where((Func<KeyValuePair<string, BaseJsonValueNode>, bool>)null).Count());
        Assert.AreEqual(json.Count, json.Where((Func<KeyValuePair<string, BaseJsonValueNode>, int, bool>)null).Count());
        Assert.AreEqual(1, json.Where(pair => pair.Key == "a").Count());
        Assert.AreEqual(1, json.Where((pair, index) => index == 0).Count());
        Assert.ThrowsExactly<ArgumentNullException>(() => json.Select<string>(null).ToList());
        Assert.ThrowsExactly<ArgumentNullException>(() => json.Where((Func<JsonValueKind, object, string, int, bool>)null));
        Assert.AreEqual(json.Count, json.ToDictionary().Count);
        Assert.AreEqual(2, json.ToLookup(p => p.Value.ValueKind)[JsonValueKind.Number].Count());
        Assert.AreEqual(2, json.ToLookup(p => p.Value.ValueKind.ToString(), StringComparer.OrdinalIgnoreCase)["number"].Count());
        Assert.AreEqual(2, json.ToLookup(p => p.Value.ValueKind, p => p.Key)[JsonValueKind.Number].Count());
        Assert.AreEqual(2, json.ToLookup(p => p.Value.ValueKind.ToString(), p => p.Key, StringComparer.OrdinalIgnoreCase)["number"].Count());
        Assert.AreEqual(json.Count, ((IEnumerable<KeyValuePair<string, IJsonValueNode>>)json).Count());
        Assert.AreEqual(json.Count, ((IEnumerable)json).Cast<object>().Count());
        Assert.AreEqual(json.Count, json.Clone((IEnumerable<string>)null).Count);
        var first = new JsonObjectNode { { "a", 1 } };
        var equal = new JsonObjectNode { { "a", 1 } };
        var different = new JsonObjectNode { { "a", 2 } };
        Assert.IsTrue(first.Equals(first));
        Assert.IsTrue(first.Equals(equal));
        Assert.IsTrue(first.Equals((IJsonValueNode)equal));
        Assert.IsTrue(first.Equals((object)equal));
        Assert.IsFalse(first.Equals((JsonObjectNode)null));
        Assert.IsFalse(first.Equals((IJsonValueNode)null));
        Assert.IsFalse(first.Equals(JsonBooleanNode.True));
        Assert.IsFalse(first.Equals(different));
        Assert.IsFalse(first.Equals(new JsonObjectNode()));
        Assert.AreEqual(first.GetHashCode(), first.GetHashCode());
        Assert.IsTrue(first == equal);
        Assert.IsFalse(first != equal);
        Assert.IsTrue(first != different);
        Assert.IsFalse(first == different);
        Assert.IsFalse(first == null);
        Assert.IsTrue(first != null);
        JsonObjectNode absent = null;
        Assert.IsTrue(absent == null);
        Assert.IsFalse(absent != null);
        Assert.IsFalse(absent == first);
        Assert.IsTrue(absent != first);
        var token = first + (Trivial.Security.ISignatureProvider)null;
        Assert.IsNotNull(token);
        Assert.IsNotNull(first.SwitchValue("a"));
        Assert.IsNotNull(first.SwitchValue("a", 42));
        Assert.IsNotNull(first.SwitchValue(true, "missing", 42));
        Assert.IsNull(first.SwitchValue(false, "missing", 42));
        Assert.IsNull(first.TryConvert<string>());
        var date = JsonObjectNode.Parse("{\"year\":2024,\"month\":2,\"day\":29}").TryConvert<DateTime>();
        Assert.AreEqual(2024, date.Year);
    }

    [TestMethod]
    public async Task SchemaAndLocalReferences()
    {
        var json = new JsonObjectNode();
        Assert.IsNull(await json.GetSchemaContentAsync());
        var schema = new JsonObjectNode { { "type", "object" } };
        json.SetValue("$schema", schema);
        Assert.AreSame(schema, await json.GetSchemaContentAsync(null));
        json.SetValue("$schema", 1);
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetSchemaContentAsync());
        json.SetValue("$schema", " ");
        Assert.ThrowsExactly<InvalidOperationException>(() => json.GetSchemaContentAsync());
        using var client = new System.Net.Http.HttpClient(new SchemaHandler());
        json.SetValue("$schema", "https://example.invalid/schema");
        var resolver = FactoryObjectResolver<System.Net.Http.HttpClient>.Create(client);
        var downloaded = await json.GetSchemaContentAsync(resolver);
        Assert.AreEqual("object", downloaded.GetStringValue("type"));
        var root = new JsonObjectNode { { "target", schema } };
        foreach (var reference in new[] { "", " ", "#", "$" })
        {
            json.SetValue("ref", new JsonObjectNode { { "$ref", reference } });
            Assert.AreSame(root, json.TryGetRefObjectValue("ref", root));
        }

        json.SetValue("ref", new JsonObjectNode { { "$ref", "@" } });
        Assert.AreSame(json, json.TryGetRefObjectValue("ref", root));
        json.SetValue("ref", new JsonObjectNode { { "$ref", "#/target" } });
        Assert.AreSame(schema, json.TryGetRefObjectValue("ref", root));
        json.SetValue("ref", new JsonObjectNode { { "$ref", "target" } });
        Assert.AreSame(schema, json.TryGetRefObjectValue("ref", root));
        json.SetValue("ref", schema);
        Assert.AreSame(schema, json.TryGetRefObjectValue("ref", root));
        Assert.IsNull(json.TryGetRefObjectValue("missing", root));
    }

    [TestMethod]
    [DataRow(typeof(ArgumentException))]
    [DataRow(typeof(InvalidOperationException))]
    [DataRow(typeof(JsonException))]
    [DataRow(typeof(FormatException))]
    [DataRow(typeof(InvalidCastException))]
    [DataRow(typeof(IOException))]
    [DataRow(typeof(SecurityException))]
    [DataRow(typeof(UnauthorizedAccessException))]
    [DataRow(typeof(NullReferenceException))]
    [DataRow(typeof(AggregateException))]
    [DataRow(typeof(System.Runtime.InteropServices.ExternalException))]
    public void TryParseHandlesStreamFailures(Type exceptionType)
    {
        using var stream = new FailingStream((Exception)Activator.CreateInstance(exceptionType));
        Assert.IsNull(JsonObjectNode.TryParse(stream));
    }

#if NET10_0_OR_GREATER
    private static T ParseGeneric<T>(string value) where T : IParsable<T> => T.Parse(value, CultureInfo.InvariantCulture);

    private static bool TryParseGeneric<T>(string value, out T result) where T : IParsable<T> => T.TryParse(value, CultureInfo.InvariantCulture, out result);
#endif

    private sealed class FailingStream(Exception exception) : MemoryStream
    {
        public override int Read(byte[] buffer, int offset, int count) => throw exception;
#if !NETFRAMEWORK
        public override int Read(Span<byte> buffer) => throw exception;
#endif
    }

    private sealed class SchemaHandler : System.Net.Http.HttpMessageHandler
    {
        protected override Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.AreEqual("https://example.invalid/schema", request.RequestUri.AbsoluteUri);
            return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.StringContent("{\"type\":\"object\"}", Encoding.UTF8, "application/json")
            });
        }
    }

    private static bool IsSampleType(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is Type underlying) return IsSampleType(underlying);
        if (new[] { typeof(string), typeof(char[]), typeof(StringBuilder), typeof(SecureString), typeof(bool), typeof(uint), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(Guid), typeof(DateTime), typeof(Uri), typeof(DBNull), typeof(JsonStringNode), typeof(JsonIntegerNode), typeof(JsonDoubleNode), typeof(JsonDecimalNode), typeof(JsonBooleanNode), typeof(JsonObjectNode), typeof(JsonArrayNode), typeof(BaseJsonValueNode), typeof(IJsonValueNode), typeof(IJsonObjectHost), typeof(JsonDocument), typeof(JsonElement), typeof(JsonNode), typeof(JsonObject), typeof(JsonArray) }.Contains(type)) return true;
        return type.IsGenericType && new[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IJsonValueNode<>) }.Contains(type.GetGenericTypeDefinition()) && IsSampleType(type.GetGenericArguments()[0]);
    }

    private static object CreateSample(Type type, out string expected)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        expected = "42";
        if (type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return Convert.ChangeType(42, type, CultureInfo.InvariantCulture);
        if (type == typeof(string)) { expected = "\"text\""; return "text"; }
        if (type == typeof(char[])) { expected = "\"text\""; return "text".ToCharArray(); }
        if (type == typeof(StringBuilder)) { expected = "\"text\""; return new StringBuilder("text"); }
        if (type == typeof(SecureString))
        {
            expected = "\"text\"";
            var secure = new SecureString();
            foreach (var c in "text") secure.AppendChar(c);
            return secure;
        }

        if (type == typeof(bool)) { expected = "true"; return true; }
        if (type == typeof(DBNull)) { expected = "null"; return DBNull.Value; }
        if (type == typeof(DateTime)) { expected = new JsonStringNode(SampleDate).ToString(); return SampleDate; }
        if (type == typeof(Guid)) { expected = new JsonStringNode(SampleGuid).ToString(); return SampleGuid; }
        if (type == typeof(Uri)) { expected = "\"https://example.invalid/path\""; return new Uri("https://example.invalid/path"); }
        if (type == typeof(JsonStringNode) || type == typeof(IJsonValueNode<string>)) { expected = "\"text\""; return new JsonStringNode("text"); }
        if (type == typeof(JsonBooleanNode) || type == typeof(IJsonValueNode<bool>)) { expected = "true"; return JsonBooleanNode.True; }
        if (type == typeof(JsonDoubleNode)) return new JsonDoubleNode(42D);
        if (type == typeof(JsonDecimalNode)) return new JsonDecimalNode(42M);
        if (type == typeof(JsonIntegerNode) || type == typeof(BaseJsonValueNode) || type == typeof(IJsonValueNode)) return new JsonIntegerNode(42);
        if (type == typeof(JsonArrayNode)) { expected = "[42]"; return new JsonArrayNode { 42 }; }
        if (type.IsGenericType && new[] { typeof(IEnumerable<>), typeof(ICollection<>) }.Contains(type.GetGenericTypeDefinition()))
        {
            var elementType = type.GetGenericArguments()[0];
            var sample = CreateSample(elementType, out expected);
            var array = Array.CreateInstance(elementType, 1);
            array.SetValue(sample, 0);
            expected = "[" + expected + "]";
            return array;
        }

        expected = "{\"v\":1}";
        if (type == typeof(JsonObjectNode)) return new JsonObjectNode { { "v", 1 } };
        if (type == typeof(IJsonObjectHost)) return new ObjectHost();
        if (type == typeof(JsonDocument)) return JsonDocument.Parse(expected);
        if (type == typeof(JsonElement)) { using var document = JsonDocument.Parse(expected); return document.RootElement.Clone(); }
        if (type == typeof(JsonNode) || type == typeof(JsonObject)) return new JsonObject { ["v"] = 1 };
        if (type == typeof(JsonArray)) { expected = "[42]"; return new JsonArray(42); }
        throw new NotSupportedException(type.FullName);
    }

    private static void CheckConversion<T>(JsonObjectNode json, T expected)
    {
        Assert.AreEqual(expected, json.GetValue<T>("v"));
        Assert.AreEqual(expected, json.GetValue<T>("v".AsSpan()));
        Assert.AreEqual(expected, json.GetValue(typeof(T), "s"));
        Assert.IsTrue(json.TryGetValue(new[] { "v" }, out T result, out var kind));
        Assert.AreEqual(expected, result);
        Assert.AreEqual(JsonValueKind.Number, kind);
        Assert.IsTrue(json.TryGetValue(new[] { "s" }, out result));
        Assert.AreEqual(expected, result);
        Assert.IsFalse(json.TryGetValue(new[] { "bad" }, out result));
        Assert.IsFalse(json.TryGetValue(new[] { "missing" }, out result, out kind));
        Assert.AreEqual(JsonValueKind.Undefined, kind);
        Assert.IsFalse(json.TryGetValue(new[] { "n" }, out result, out kind));
        Assert.AreEqual(JsonValueKind.Null, kind);
    }

    private static object Invoke(MethodInfo method, JsonObjectNode json, object[] args)
    {
        try
        {
            return method.Invoke(json, args);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }

#pragma warning disable SYSLIB0050
    private static SerializationInfo NewSerializationInfo() => new(typeof(JsonObjectNode), new FormatterConverter());
#pragma warning restore SYSLIB0050

    private sealed class SerializableObject : JsonObjectNode
    {
        public SerializableObject(SerializationInfo info) : base(info, default) { }
    }

    private sealed class ObjectHost : IJsonObjectHost
    {
        public JsonObjectNode ToJson() => new() { { "v", 1 } };
    }

    private sealed class PropertyResolver<T>(T value) : IJsonPropertyResolver<T>
    {
        public bool TryGetValue(JsonObjectNode source, out T result)
        {
            result = value;
            return source != null;
        }
    }

    private sealed class RoutePolicy : IJsonPropertyRoutePolicy
    {
        public bool TryGetObjectValue(JsonObjectNode source, string key, out JsonObjectNode value, out string exactKey)
        {
            exactKey = "obj";
            return source.TryGetObjectValue(exactKey, out value);
        }
    }

    public sealed class ValueModel
    {
        public int Value { get; set; }
    }
}
