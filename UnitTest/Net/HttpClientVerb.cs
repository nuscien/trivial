using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Trivial.Net
{
    class HttpClientVerb : Console.AsyncVerb
    {
        [DataContract]
        internal class NameAndDescription
        {
            [DataMember(Name = "name")]
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [DataMember(Name = "description")]
            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        public override string Description => "HTTP client";

        public override async Task ProcessAsync()
        {
            // HTTP URI.
            var url = "http://www.kingcean.net:8080/test/path?a=123&b=hello#nothing/all";
            var uri = HttpUri.Parse(url);
            ConsoleLine.WriteLine(((Uri)uri).ToString());
            ConsoleLine.WriteLine();

            // Query data.
            var query = "{ str: \"abcdefg\", b: true, \"name\": \"hijklmn\", // abcd: efg\n \"value\": \"012345\", \"num\": 67, null: undefined, \"props\": { \"x\": \"o\\tp\\tq\", \"y\": [ 8, 9, { \"z\": \"rst\" } ] } }";
            var q = QueryData.Parse(query);
            ConsoleLine.WriteLine(q.ToString());
            ConsoleLine.WriteLine();

            // JSON HTTP web client.
            url = "https://github.com/compositejs/datasense/raw/master/package.json";
            var webClient = new JsonHttpClient<NameAndDescription>();
            var resp = await webClient.SendAsync(HttpMethod.Get, url);
            ConsoleLine.WriteLine(resp.Name);
            resp = await webClient.SendAsync(HttpMethod.Get, url);
            ConsoleLine.WriteLine(resp.Name);

            //"{ \"access_token\": \"abc\", \"token_type\": \"Bearer\" }"
        }
    }
}
