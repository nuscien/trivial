using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trivial.UnitTest.Text
{
    [DataContract]
    public class JsonModel
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
}
