using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trivial.Net;

namespace Trivial.Sample
{
    public class HttpClientVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "HTTP client";

        public override async Task ProcessAsync()
        {
            await Task.Delay(0);
            var url = "http://www.kingcean.net:8080/test/path?a=123&b=hello#nothing/all";
            var uri = HttpUri.Parse(url);
            ConsoleLine.Write(((Uri)uri).ToString());
        }
    }
}
