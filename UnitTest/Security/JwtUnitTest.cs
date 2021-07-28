using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.Security
{
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
            Assert.AreEqual(true, jwt.CanSign);
            Assert.AreEqual(payload, jwt.Payload);
            Assert.AreEqual(hash.Name, jwt.AlgorithmName);
            var header = jwt.ToAuthenticationHeaderValue();
            Assert.IsTrue(header.Parameter.Length > 10);
            var jwt2 = JsonWebToken<JsonObjectNode>.Parse(header.Parameter, hash);
            Assert.AreEqual(header.Parameter, jwt2.ToEncodedString());
            var parser = new JsonWebToken<JsonObjectNode>.Parser(header.Parameter);
            Assert.AreEqual(header.Parameter, parser.ToString());
        }
    }
}
