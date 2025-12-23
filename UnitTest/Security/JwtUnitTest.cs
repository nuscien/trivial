using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// JWT unit test.
/// </summary>
[TestClass]
public class JwtUnitTest
{
    /// <summary>
    /// Tests standard JWT payload.
    /// </summary>
    [TestMethod]
    public void TestStandardJwt()
    {
        var payload = new JsonWebTokenPayload
        {
            Issuer = "Someone",
            Subject = "Local Test Auth",
            Audience = new() { "BackupOperator", "PowerUser" }
        };
        var hash = HashSignatureProvider.CreateHS512("key");
        var jwt = payload + hash;
        Assert.IsTrue(jwt.CanSign);
        Assert.AreEqual(payload, jwt.Payload);
        Assert.AreEqual(hash.Name, jwt.AlgorithmName);
        var header = jwt.ToAuthenticationHeaderValue();
        Assert.IsGreaterThan(10, header.Parameter.Length);
        var jwt2 = JsonWebToken<JsonWebTokenPayload>.Parse(header.Parameter, hash);
        Assert.AreEqual(header.Parameter, jwt2.ToEncodedString());
        var parser = new JsonWebToken<JsonWebTokenPayload>.Parser(header.Parameter);
        Assert.AreEqual(header.Parameter, parser.ToString());
    }

    /// <summary>
    /// Tests customized JWT payload.
    /// </summary>
    [TestMethod]
    public void TestCustomizedJwt()
    {
        var payload = new JsonObjectNode();
        payload.SetValue("abcdefg", "hijklmn");
        payload.SetValue("opqrst", HashUtility.ComputeSHA3512String("uvwxyz"));
        var hash = HashSignatureProvider.CreateHS512("key");
        var jwt = new JsonWebToken<JsonObjectNode>(payload, hash);
        Assert.IsTrue(jwt.CanSign);
        Assert.AreEqual(payload, jwt.Payload);
        Assert.AreEqual(hash.Name, jwt.AlgorithmName);
        var header = jwt.ToAuthenticationHeaderValue();
        Assert.IsGreaterThan(10, header.Parameter.Length);
        var jwt2 = JsonWebToken<JsonObjectNode>.Parse(header.Parameter, hash);
        Assert.AreEqual(header.Parameter, jwt2.ToEncodedString());
        var parser = new JsonWebToken<JsonObjectNode>.Parser(header.Parameter);
        Assert.AreEqual(header.Parameter, parser.ToString());
    }

    /// <summary>
    /// Tests protected resource metadata.
    /// </summary>
    [TestMethod]
    public async Task TestProtectedResourceMetadataAsync()
    {
        var uri = StringExtensions.TryCreateUri("https://www.kingcean.net");
        var realm = "Test \"Asset, Resource, \"\", etc\"";
        var req = new ProtectedResourceMetadataLink(uri)
        {
            Realm = realm,
            Scopes = new() { "read", "list" }
        };
        var s = req.ToString();
        req = ProtectedResourceMetadataLink.TryParse(s);
        Assert.AreEqual(TokenInfo.BearerTokenType, req.ToAuthenticationHeader()?.Scheme);
        Assert.IsNotNull(req?.Uri);
        Assert.AreEqual(uri.OriginalString, req.Url);
        Assert.AreEqual(realm, req.Realm);
        Assert.IsNotNull(req.Scopes);
        Assert.HasCount(2, req.Scopes);
        Assert.AreEqual("read", req.Scopes[0]);
        Assert.AreEqual("list", req.Scopes[1]);
        req.Scopes = null;
        s = req.ToString();
        req = ProtectedResourceMetadataLink.TryParse(s);
        Assert.AreEqual(uri.OriginalString, req.Url);
        Assert.AreEqual(realm, req.Realm);
        Assert.IsNull(req.Scopes);
        var meta = new ProtectedResourceMetadataResponse
        {
            Id = uri.OriginalString,
            Scopes = new() { "read", "list", "write" }
        };
        meta.AddBearerMethodHeader();
        meta.AddDPoPSigningAlgorithm("ES256");
        meta.AddDPoPSigningAlgorithm("ES512");
        meta.AddJwtFormat();
        var cache = new JsonHttpClientCache<JsonObjectNode>();
        cache.Collection.Add(uri.OriginalString, JsonObjectNode.ConvertFrom(meta));
        var http = new JsonHttpClient<JsonObjectNode>
        {
            Cache = cache
        };
        meta = await req.GetMetadataAsync(http);
        Assert.IsNotNull(meta);
        Assert.AreEqual(uri.OriginalString, meta.Id);
        Assert.HasCount(3, meta.Scopes);
        Assert.HasCount(2, meta.DPoPSigningAlgorithms);
        Assert.AreEqual("ES256", meta.DPoPSigningAlgorithms[0]);
        Assert.AreEqual("ES512", meta.DPoPSigningAlgorithms[1]);
        Assert.HasCount(1, meta.TokenFormats);
        Assert.AreEqual("jwt", meta.TokenFormats[0]);
        Assert.IsNull(meta.JsonWebKeysUrl);
    }
}
