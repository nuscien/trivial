# Hash

Hash algorithm utility.

In `Trivial.Security` [namespace](./README) of `Trivial.dll` [library](../README).

### Hash utility

You can hash a string and validate it.

```csharp
var original = "The string to hash";

// SHA-256
var sha256Hash = HashUtilities.ComputeHashString(SHA256.Create, original);
var isVerified = HashUtilities.Verify(SHA256.Create, original, sha256Hash); // --> true

// SHA-512
var sha512Hash = HashUtilities.ComputeHashString(SHA512.Create, original);
var isVerified = HashUtilities.Verify(SHA512.Create, original, sha256Hash); // --> true

// SHA-3-512
var sha3512Name = new HashAlgorithmName("SHA3512");
var sha3512Hash = HashUtilities.ComputeHashString(sha3512Name, original);
var isVerified3512 = HashUtilities.Verify(sha3512Name, original, sha3512Hash); // --> true
```
