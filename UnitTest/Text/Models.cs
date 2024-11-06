using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Data;
using Trivial.Tasks;

namespace Trivial.Text;

[Guid("DD4F9F4E-D127-424A-B04A-696C5071EC7D")]
[DataContract]
class JsonModel
{
    [Description("Property A.")]
    [DataMember(Name = "str-a")]
    [JsonPropertyName("str-a")]
    public string A { get; set; }

    [Description("Property B.")]
    [DataMember(Name = "str-b")]
    [JsonPropertyName("str-b")]
    public string B { get; set; }

    [DataMember(Name = "str-c")]
    [JsonPropertyName("str-c")]
    public string C { get; set; }

    [Description("Property Number.")]
    [DataMember(Name = "num")]
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [Description("Property Collection.")]
    [DataMember(Name = "arr")]
    [JsonPropertyName("arr")]
    public IEnumerable<int> Col { get; set; }

    [Description("Create a new instance.")]
    public static JsonModel Create(JsonAttributeTestModel m)
        => new();

    [Description("Create a new instance.")]
    [JsonOperationDescriptive(typeof(JsonOperationDescriptiveHandler), "test")]
    public static JsonModel Create(string a, string b)
        => new()
        {
            A = a,
            B = b
        };
}

class JsonOperationDescriptiveHandler : IJsonOperationDescriptive<MethodInfo>
{
    public JsonOperationDescription CreateDescription(string id, MethodInfo info)
        => new JsonOperationDescription
        {
            Description = info.Name,
            Tag = id
        };
}

[Description("A test model.")]
class JsonAttributeTestModel
{
    [JsonConverter(typeof(JsonNumberConverter))]
    public DateTime A { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public DateTime? B { get; set; }

    [JsonConverter(typeof(JsonJavaScriptTicksConverter.FallbackConverter))]
    public DateTime C { get; set; }

    [JsonConverter(typeof(JsonJavaScriptTicksConverter.FallbackNullableConverter))]
    public DateTime? D { get; set; }

    [JsonConverter(typeof(JsonUnixTimestampConverter))]
    public DateTime E { get; set; }

    public JsonObjectNode F { get; set; }

    public JsonArrayNode G { get; set; }

    [JsonConverter(typeof(JsonStringListConverter))]
    public IList<string> H { get; set; }

    [JsonConverter(typeof(JsonStringListConverter.WhiteSpaceSeparatedConverter))]
    public HashSet<string> I { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public uint J { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public Maths.StructValueSimpleInterval<int> K { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public Maths.NullableValueSimpleInterval<long> L { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public Maths.StructValueSimpleInterval<double> M { get; set; }

    public Maths.VersionSimpleInterval N { get; set; }

    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Data.ChangeErrorKinds O { get; set; }

    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Data.ChangeErrorKinds P { get; set; }

    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Data.ChangeErrorKinds Q { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public System.Drawing.Color R { get; set; }

    [JsonConverter(typeof(JsonNumberConverter.NumberStringConverter))]
    public System.Drawing.Color S { get; set; }

    [JsonConverter(typeof(JsonNumberConverter))]
    public System.Drawing.Color T { get; set; }

    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Data.ChangeMethods? U { get; set; }

    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Data.ChangeMethods? V { get; set; }

    public Maths.Angle W { get; set; }
}

#if NET7_0_OR_GREATER
[JsonConverter(typeof(JsonObjectHostConverter))]
#endif
class JsonHostTestModel : IJsonObjectHost
{
    public JsonHostTestModel()
    {
    }

    public JsonHostTestModel(JsonObjectNode json)
    {
        if (json == null) return;
        Name = json.TryGetStringValue("n");
        Value = json.TryGetStringValue("v");
    }

    public JsonHostTestModel(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }

    public string Value { get; set; }

    public JsonObjectNode ToJson()
        => new()
        {
            { "n", Name },
            { "v", Value }
        };
}

class TestJsonSwitchCase : BaseJsonSwitchCase
{
    protected override void OnProcess()
        => TrySetArgs(Args is string s && s == "Hey!" ? "Hi!" : "Hey!");

    protected override bool Test()
    {
        if (Source is not JsonObjectNode) return false;
        return Source.Count > 1;
    }
}