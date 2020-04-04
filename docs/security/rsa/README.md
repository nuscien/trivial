# RSA

RSA utility and related models.

In `Trivial.Security` [namespace](../) of `Trivial.dll` [library](../../).

## RSA parameters convert

You can parse a PEM string (OpenSSL RSA key) or an XML format string to the `RSAParameters` class.

```csharp
var parameters = RSAParametersConvert.Parse(pem);
```

And you can convert back by using the extension function `ToPrivatePEMString` or `ToPublicPEMString`. And also you can use the extension function `ToXElement` or `ToXmlDocument` to export as XML.
