using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Net;

class HttpClientVerb : CommandLine.BaseCommandVerb
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
