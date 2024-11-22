using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.CommandLine;
using Trivial.Security;
using Trivial.Text;

namespace Trivial.Net;

class HttpClientVerb : BaseCommandVerb
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

    sealed class WhoAmI
    {
        public sealed class GeoInfo
        {
            [JsonPropertyName("country")]
            public string RegionName { get; set; }


            [JsonPropertyName("country_iso")]
            public string RegionCode { get; set; }
        }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("geo")]
        public GeoInfo Geo { get; set; }

        [JsonIgnore]
        public string RegionCode => Geo?.RegionCode;
    }

    public static string Description => "HTTP client";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        // HTTP URI.
        var url = "http://www.kingcean.net:8080/test/path?a=123&b=hello#nothing/all";
        var uri = HttpUri.Parse(url);
        Console.WriteLine(((Uri)uri).ToString());
        Console.WriteLine();

        // Query data.
        var query = "{ str: \"abcdefg\", b: true, \"name\": \"hijklmn\", // abcd: efg\n \"value\": \"012345\", \"num\": 67, null: undefined, \"props\": { \"x\": \"o\\tp\\tq\", \"y\": [ 8, 9, { \"z\": \"rst\" } ] } }";
        var q = QueryData.Parse(query);
        Console.WriteLine(q.ToString());
        Console.WriteLine();

        // JSON HTTP web client.
        url = "https://developer.mozilla.org/api/v1/whoami";
        var who = new JsonHttpClient<WhoAmI>();
        var me = await who.SendAsync(HttpMethod.Get, url, cancellationToken);
        Console.WriteLine(me.RegionCode);
        me = await who.SendAsync(HttpMethod.Get, url, cancellationToken);
        Console.WriteLine(me.RegionCode);

        // JSON HTTP web client.
        url = "https://github.com/compositejs/datasense/raw/main/package.json";
        var webClient = new JsonHttpClient<NameAndDescription>();
        var resp = await webClient.SendAsync(HttpMethod.Get, url, cancellationToken);
        Console.WriteLine(resp.Name);
        resp = await webClient.SendAsync(HttpMethod.Get, url, cancellationToken);
        Console.WriteLine(resp.Name);

        //"{ \"access_token\": \"abc\", \"token_type\": \"Bearer\" }"
    }
}

class DemoServerClientVerb : BaseCommandVerb
{
    public const string url = "http://localhost:5090/Test/";

    public static string Description => "HTTP client";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var oauth = new OAuthClient("trivial.cli", "demo-used")
        {
            TokenResolverUri = new(url + "Login")
        };
        const string name = "admin";
        const string password = "P@ssw0rd";
        DefaultConsole.Write("Signing in...");
        var client = oauth.Create<JsonObjectNode>();
        var resp = await client.PostAsync(url + "Register", new JsonObjectNode
        {
            { "name", name },
            { "password", password }
        }, cancellationToken);
        await oauth.ResolveTokenAsync(new PasswordTokenRequestBody(name, password), cancellationToken);
        DefaultConsole.BackspaceToBeginning();
        DefaultConsole.WriteLine($"Signed in succeeded by account {name}.");
        resp = await client.GetAsync(url + "Data");
        DefaultConsole.WriteLine();
        DefaultConsole.WriteLine(resp);
        DefaultConsole.WriteLine();
        DefaultConsole.WriteLine("SSE test");
        DefaultConsole.WriteLine();
        var sse = await oauth.Create<IEnumerable<ServerSentEventInfo>>().GetAsync(url + "Stream");
        foreach (var item in sse)
        {
            DefaultConsole.WriteLine(item.Id);
            DefaultConsole.WriteLine(item.DataString);
            DefaultConsole.WriteLine();
        }

        DefaultConsole.WriteLine("Done!");
    }
}
