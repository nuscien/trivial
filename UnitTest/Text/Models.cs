using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trivial.Text
{
    [DataContract]
    class JsonModel
    {
        [DataMember(Name = "str-a")]
        [JsonPropertyName("str-a")]
        public string A { get; set; }

        [DataMember(Name = "str-b")]
        [JsonPropertyName("str-b")]
        public string B { get; set; }

        [DataMember(Name = "str-c")]
        [JsonPropertyName("str-c")]
        public string C { get; set; }

        [DataMember(Name = "num")]
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [DataMember(Name = "arr")]
        [JsonPropertyName("arr")]
        public IEnumerable<int> Col { get; set; }
    }

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
}
