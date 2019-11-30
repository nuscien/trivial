using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Security;
using Trivial.Text;

namespace Trivial.UnitTest.Security
{
    [TestClass]
    public class JwtTest
    {
        [TestMethod]
        public void TestStandardJwt()
        {
            var payload = new JsonWebTokenPayload
            {
                Issuer = "Someone",
                Subject = "Local Test Auth",
                Audience = new[] { "BackupOperator", "PowerUser" }
            };
            var hash = HashSignatureProvider.CreateHS512("key");
            var jwt = new JsonWebToken<JsonWebTokenPayload>(payload, hash);
            Assert.AreEqual(true, jwt.CanSign);
            Assert.AreEqual(payload, jwt.Payload);
            Assert.AreEqual(hash.Name, jwt.AlgorithmName);
            var header = jwt.ToAuthenticationHeaderValue();
            Assert.IsTrue(header.Parameter.Length > 10);
            var jwt2 = JsonWebToken<JsonWebTokenPayload>.Parse(header.Parameter, hash);
            Assert.AreEqual(header.Parameter, jwt2.ToEncodedString());
            var parser = new JsonWebToken<JsonWebTokenPayload>.Parser(header.Parameter);
            Assert.AreEqual(header.Parameter, parser.ToString());
        }

        [TestMethod]
        public void TestCustomizedJwt()
        {
            var payload = new JsonObject();
            payload.SetValue("abcdefg", "hijklmn");
            payload.SetValue("opqrst", "uvwxyz");
            var hash = HashSignatureProvider.CreateHS512("key");
            var jwt = new JsonWebToken<JsonObject>(payload, hash);
            Assert.AreEqual(true, jwt.CanSign);
            Assert.AreEqual(payload, jwt.Payload);
            Assert.AreEqual(hash.Name, jwt.AlgorithmName);
            var header = jwt.ToAuthenticationHeaderValue();
            Assert.IsTrue(header.Parameter.Length > 10);
            var jwt2 = JsonWebToken<JsonObject>.Parse(header.Parameter, hash);
            Assert.AreEqual(header.Parameter, jwt2.ToEncodedString());
            var parser = new JsonWebToken<JsonObject>.Parser(header.Parameter);
            Assert.AreEqual(header.Parameter, parser.ToString());
        }
    }
}
