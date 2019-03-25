using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Trivial.Net;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Tasks;
using System.Security.Cryptography;

namespace Trivial.Sample
{
    public class TokenClientVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "HTTP client";

        public override async Task ProcessAsync()
        {
            var codeTokenReq = new CodeTokenRequest("abcd", "efg")
            {
                Code = "hijklmn",
                ScopeString = "test plain"
            };
            var tokenUrl = codeTokenReq.ToJson();
            codeTokenReq = CodeTokenRequest.Parse(tokenUrl);
            tokenUrl = codeTokenReq.ToQueryData().ToString();
            codeTokenReq = CodeTokenRequest.Parse(tokenUrl);
            ConsoleLine.WriteLine(codeTokenReq.ToQueryData().ToString());
            ConsoleLine.WriteLine();

            // JWT HS512
            var hs = HashSignatureProvider.CreateHS512("a secret string");
            var jwt = new JsonWebToken<HttpClientVerb.NameAndDescription>(new HttpClientVerb.NameAndDescription
            {
                Name = "abcd",
                Description = "efg"
            }, hs);
            var header = jwt.ToAuthenticationHeaderValue();
            jwt = JsonWebToken<HttpClientVerb.NameAndDescription>.Parse(header.ToString(), hs);
            var jwtStr = jwt.ToEncodedString();
            ConsoleLine.WriteLine(jwtStr != header.Parameter ? "Failed JWT HS512 testing." : jwtStr);
            ConsoleLine.WriteLine();

            // JWT RS512
            var rsa = RSA.Create();
            ConsoleLine.WriteLine(rsa.ExportParameters(true).ToPrivatePEMString(true));
            ConsoleLine.WriteLine(rsa.ExportParameters(false).ToPublicPEMString());
            var rs = RSASignatureProvider.CreateRS512(RSA.Create());
            jwt = new JsonWebToken<HttpClientVerb.NameAndDescription>(jwt.Payload, rs);
            header = jwt.ToAuthenticationHeaderValue();
            jwt = JsonWebToken<HttpClientVerb.NameAndDescription>.Parse(header.ToString(), rs);
            jwtStr = jwt.ToEncodedString();
            ConsoleLine.WriteLine(jwtStr != header.Parameter ? "Failed JWT RS512 testing." : header.Parameter);
            ConsoleLine.WriteLine();
        }
    }
}
