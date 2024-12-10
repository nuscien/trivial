using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Security;

/// <summary>
/// Cryptography unit test.
/// </summary>
[TestClass]
public class CryptographyUnitTest
{
    /// <summary>
    /// Tests RSA converter.
    /// </summary>
    [TestMethod]
    public void TestRsa()
    {
        var source = "Test RSA data";
        var key = RSA.Create().ExportParameters(true);
        var pem = key.ToPublicPEMString();
        var param = RSAParametersConvert.Parse(pem);
        Assert.IsNotNull(param);
        var rsa = RSA.Create(param.Value);
        var encrypt = rsa.Encrypt(source, RSAEncryptionPadding.Pkcs1);
        Assert.IsNotNull(encrypt);
        var str = ObjectConvert.ToHexString(encrypt);
        Assert.IsNotNull(str);
        pem = key.ToPrivatePEMString(true);
        param = RSAParametersConvert.Parse(pem);
        Assert.IsTrue(param.HasValue);
        rsa = RSA.Create(param.Value);
        var bytes = ObjectConvert.FromHexString(str);
        var value = rsa.DecryptText(bytes, RSAEncryptionPadding.Pkcs1);
        Assert.AreEqual(source, value);
        var sign = RSASignatureProvider.CreateRS512(param.Value);
        str = sign.SignToHex(source);
        Assert.IsTrue(sign.VerifyFromHex(source, str));
        str = sign.SignToBase64Url(source);
        Assert.IsTrue(sign.VerifyFromBase64Url(source, str));
    }
}
