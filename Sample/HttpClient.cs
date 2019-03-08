using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Net;
using System.Runtime.Serialization;
using System.Net.Http;

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
