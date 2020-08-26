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

        [JsonConverter(typeof(JsonObjectConverter))]
        public JsonObject F { get; set; }

        [JsonConverter(typeof(JsonObjectConverter))]
        public JsonArray G { get; set; }

        [JsonConverter(typeof(JsonStringListConverter))]
        public IList<string> H { get; set; }

        [JsonConverter(typeof(JsonStringListConverter.WhiteSpaceSeparatedConverter))]
        public HashSet<string> I { get; set; }

        [JsonConverter(typeof(JsonNumberConverter))]
        public uint J { get; set; }

        [JsonConverter(typeof(JsonNumberConverter))]
        public Maths.StructValueSimpleInterval<int> K { get; set; }
    }
}
