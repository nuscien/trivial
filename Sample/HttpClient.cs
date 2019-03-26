using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Trivial.Net;
using Trivial.Reflection;
using Trivial.Tasks;

namespace Trivial.Sample
{
    public class HttpClientVerb : Trivial.Console.AsyncVerb
    {
        [DataContract]
        internal class NameAndDescription
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "description")]
            public string Description { get; set; }
        }

        public override string Description => "HTTP client";

        public override async Task ProcessAsync()
        {
            var url = "http://www.kingcean.net:8080/test/path?a=123&b=hello#nothing/all";
            var uri = HttpUri.Parse(url);
            ConsoleLine.WriteLine(((Uri)uri).ToString());
            ConsoleLine.WriteLine();

            // Query.
            var query = "{ str: \"abcdefg\", \"name\": \"hijklmn\", \"value\": \"012345\", \"num\": 67, \"props\": { \"x\": \"opq\", \"y\": [ 8, 9, { \"z\": \"rst\" } ] }, null: undefined }";
            var q = QueryData.Parse(query);

            // JSON HTTP web client.
            //"{ \"access_token\": \"abc\", \"token_type\": \"Bearer\" }"
            url = "https://github.com/compositejs/datasense/raw/master/package.json";
            var webClient = new JsonHttpClient<NameAndDescription>();
            var resp = await webClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            ConsoleLine.WriteLine(resp.Name);
            resp = await webClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            ConsoleLine.WriteLine(resp.Name);
        }
    }
}
