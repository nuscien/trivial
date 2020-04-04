# Secure string

Secure string utility and secret exchange.

In `Trivial.Security` [namespace](../) of `Trivial.dll` [library](../../).

## Secure string extensions

You can use the extension methods in `SecureStringExtensions` class to convert secret between `SecureString` and `String`/`StringBuilder`/`Byte[]`.

## Secret exchange based on RSA

You can save the secret locally and send it to server after encryption by the public key from the other side. And send a public key registered in current container to the other side so that it can use the same mechanism to transfer the secret encrypted back and you can decrypt it by your private key.

Class `RSASecretExchange` will help you to do so in both sides.

```csharp
// Create a secret exchange instance and create a pair of RSA key.
var exchange = new RSASecretExchange();
exchange.CreateCrypto();

// Get the public key to send to the other side
// so that they can use this to encrypt the secret
// and send back to us.
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

// Get the authentication header value in JSON web token format
// using HMAC SHA-512 keyed hash algorithm to signature for example.
var sign = HashSignatureProvider.CreateHS512("a secret hash key");
var jwt = exchange.ToJsonWebTokenAuthenticationHeaderValue(sign);
```

So you can use this in both 2 sides.
