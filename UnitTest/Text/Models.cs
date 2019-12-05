using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Text;

namespace Trivial.UnitTest.Text
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

    class JsonAttributeModel
    {
        [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
        public DateTime A { get; set; }

        [JsonConverter(typeof(JsonJavaScriptTicksConverter.NullableConverter))]
        public DateTime? B { get; set; }

        [JsonConverter(typeof(JsonJavaScriptTicksConverter.StringConverter))]
        public DateTime C { get; set; }

        [JsonConverter(typeof(JsonJavaScriptTicksConverter.NullableStringConverter))]
        public DateTime? D { get; set; }
    }
}
