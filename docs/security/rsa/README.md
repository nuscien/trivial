# RSA

RSA utility and related models.

In `Trivial.Security` [namespace](../) of `Trivial.dll` [library](../../).

## RSA parameters convert

You can parse a PEM string (OpenSSL RSA key) or an XML format string to the `RSAParameters` class.

```csharp
var parameters = RSAParametersConvert.Parse(pem);
```

And you can convert back by using the extension function `ToPrivatePEMString` or `ToPublicPEMString`. And also you can use the extension function `ToXElement` or `ToXmlDocument` to export as XML.

## RSA secret exchange

You can save the secret or other string in local and send it another side (client/server-side) after encryption by the public key required by that side. And send a public key registered in current container to the other side so that it can use the same mechanism to transfer the secret encrypted back and you can decrypt it by your private key.

Following is an example of flow, suppose one is server side and another is client side.

1. Client side asks for a server encrypt key to encrypt the authorization form filled in client.
2. Server side sends a server encrypt key and its identifier. This key is a public key (marked as S-Key-Public here) for encryption. Its private key (marked as S-Key-Private here) is used to decrypt. Both keys and its identifier (marked as S-Key-Id here) should cache in server.
3. Client creates a pair of private key and public key. The private key (marked as C-Key-Private here) is used to decrypt data and the public key (marked as C-Key-Public here) is used to encrypt. The key pair should cache in client with an identifier (marked as C-Key-Id here).
4. Client uses S-Key-Public to encrypt the form now and send it back to server with the S-Key-Id, C-Key-Public and C-Key-Id. Client also need cache S-Key-Id and S-Key-Public in local.
5. Server uses S-Key-Id to find the S-Key-Private to decrypt the form. Then generates a secret and encrypt it by C-Key-Public. Then send the secret encrypted back to client with C-Key-Id. Server also need cache C-Key-Id and C-Key-Public with a mapping relationship with the token.
6. Client uses C-Key-Id to find C-Key-Private to decrypt the secret. At next time, client sends the data and the secret encrypted by S-Key-Public with S-Key-Id.
7. Server uses S-Key-Id to find the S-Key-Private to decrypt the secret to authorize. Then process the business logic of request. Return the result and the secret encrypted by C-Key-Public with C-Key-Id.

```csharp
// Create a secret exchange instance and create a pair of RSA key.
var exchange = new RSASecretExchange();
exchange.CreateCrypto();

// Get the public key to send to the other side
// so that they can use this to encrypt the secret and send back to us.
var publicKey = exchange.PublicKey.ToPublicPEMString();

// Save the secret from the other side.
// The secret is encrypted by the current public key
// and we can decrypt by the private key stored in this instance to save.
exchange.DecryptSecret(secretReceived);

// Save the other side public key.
var otherSidePublicKey = ...; // An RSA public key from the other side.
exchange.EncryptKey = RSAUtility.Parse(otherSidePublicKey);

// Get the Base64 of the secret encrypted by the other side public key.
var secretToSend = exchange.EncryptSecret();

// Get the authentication header value in JSON web secret format
// using HMAC SHA-512 keyed hash algorithm to signature for example.
var sign = HashSignatureProvider.CreateHS512("a secret hash key");
var jwt = exchange.ToJsonWebTokenAuthenticationHeaderValue(sign);
```
