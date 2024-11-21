using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Security
{
    class TokenClientVerb : CommandLine.BaseCommandVerb
    {
        public static string Description => "Access token";

        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            var codeTokenReq = new TokenRequest<CodeTokenRequestBody>(new CodeTokenRequestBody
            {
                Code = "hijklmn\r\nopq\trst"
            }, "abcd", "efg")
            {
                ScopeString = "test plain"
            };
            await Task.CompletedTask;
            cancellationToken.ThrowIfCancellationRequested();
            var tokenUrl = codeTokenReq.ToJsonString();
            codeTokenReq = CodeTokenRequestBody.Parse(tokenUrl);
            tokenUrl = codeTokenReq.ToQueryData().ToString();
            codeTokenReq = CodeTokenRequestBody.Parse(tokenUrl);
            tokenUrl = codeTokenReq.ToJsonString();
            codeTokenReq = CodeTokenRequestBody.Parse(tokenUrl);
            Console.WriteLine(codeTokenReq.ToQueryData().ToString());
            Console.WriteLine();

            // JWT HS512
            var hs = HashSignatureProvider.CreateHS512("a secret string");
            var jwt = new JsonWebToken<Net.HttpClientVerb.NameAndDescription>(new Net.HttpClientVerb.NameAndDescription
            {
                Name = "abcd",
                Description = "efg"
            }, hs);
            var header = jwt.ToAuthenticationHeaderValue();
            jwt = JsonWebToken<Net.HttpClientVerb.NameAndDescription>.Parse(header.ToString(), hs);
            var jwtStr = jwt.ToEncodedString();
            Console.WriteLine(jwtStr != header.Parameter ? "Failed JWT HS512 testing." : jwtStr);
            Console.WriteLine();

            // RSA.
            var rsa = RSA.Create();
            var privateKey = rsa.ExportParameters(true).ToPrivatePEMString(true);
            Console.WriteLine(privateKey);
            var publicKey = rsa.ExportParameters(false).ToPublicPEMString();
            Console.WriteLine(publicKey);
            var privateKeyP = RSAParametersConvert.Parse(privateKey).Value;
            var privateKeyS = privateKeyP.ToPrivatePEMString(true);
            var publicKeyP = RSAParametersConvert.Parse(publicKey).Value;
            var publicKeyS = publicKeyP.ToPublicPEMString();
            Console.WriteLine("They are {0}.", (privateKey == privateKeyS) && (publicKey == publicKeyS) ? "same" : "different");
            Console.WriteLine();

            // JWT RS512
            var rs = RSASignatureProvider.CreateRS512(rsa);
            jwt = new JsonWebToken<Net.HttpClientVerb.NameAndDescription>(jwt.Payload, rs);
            header = jwt.ToAuthenticationHeaderValue();
            jwt = JsonWebToken<Net.HttpClientVerb.NameAndDescription>.Parse(header.ToString(), rs);
            jwtStr = jwt.ToEncodedString();
            Console.WriteLine(jwtStr != header.Parameter ? "Failed JWT RS512 testing." : header.Parameter);
            Console.WriteLine();
        }
    }
}
