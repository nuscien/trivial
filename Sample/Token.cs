using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Cryptography;

using Trivial.Net;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Tasks;
using Trivial.Web;

namespace Trivial.Sample
{
    public class TokenClientVerb : Trivial.Console.Verb
    {
        public override string Description => "Access token";

        public override void Process()
        {
            var codeTokenReq = new CodeTokenRequest(new CodeTokenRequestBody
            {
                Code = "hijklmn"
            }, "abcd", "efg")
            {
                ScopeString = "test plain"
            };
            var tokenUrl = codeTokenReq.ToJsonString();
            codeTokenReq = CodeTokenRequest.Parse(tokenUrl);
            tokenUrl = codeTokenReq.ToQueryData().ToString();
            codeTokenReq = CodeTokenRequest.Parse(tokenUrl);
            tokenUrl = codeTokenReq.ToJsonString();
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

            // RSA.
            var rsa = RSA.Create();
            var privateKey = rsa.ExportParameters(true).ToPrivatePEMString(true);
            ConsoleLine.WriteLine(privateKey);
            var publicKey = rsa.ExportParameters(false).ToPublicPEMString();
            ConsoleLine.WriteLine(publicKey);
            var privateKeyP = RSAParametersConvert.Parse(privateKey).Value;
            var privateKeyS = privateKeyP.ToPrivatePEMString(true);
            var publicKeyP = RSAParametersConvert.Parse(publicKey).Value;
            var publicKeyS = publicKeyP.ToPublicPEMString();
            ConsoleLine.WriteLine("They are {0}.", (privateKey == privateKeyS) && (publicKey == publicKeyS) ? "same" : "different");
            ConsoleLine.WriteLine();

            // JWT RS512
            using (var rs = RSASignatureProvider.CreateRS512(rsa))
            {
                jwt = new JsonWebToken<HttpClientVerb.NameAndDescription>(jwt.Payload, rs);
                header = jwt.ToAuthenticationHeaderValue();
                jwt = JsonWebToken<HttpClientVerb.NameAndDescription>.Parse(header.ToString(), rs);
                jwtStr = jwt.ToEncodedString();
                ConsoleLine.WriteLine(jwtStr != header.Parameter ? "Failed JWT RS512 testing." : header.Parameter);
                ConsoleLine.WriteLine();
            }
        }
    }
}
