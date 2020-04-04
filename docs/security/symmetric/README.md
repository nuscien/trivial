# Symmetric

Symmetric algorithm utility.

In `Trivial.Security` [namespace](../) of `Trivial.dll` [library](../../).

## Symmetric utility

You can encrypt a secret string or decrypt a cipher string by symmetric algorithm as following.

```csharp
// Encryption Key and IV.
var key = ...;
var iv = ...;

// AES sample.
var original = "Original secret string";
var cipher = SymmetricUtilities.Encrypt(Aes.Create, original, key, iv);
var back = SymmetricUtilities.Decrypt(Aes.Create, cipher, key, iv); // back == original
```
