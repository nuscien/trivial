# Hash

在 `Trivial.Security` [命名空间](../)中。

<!-- Start -->

哈希算法工具。

### 哈希工具

可以对字符串进行哈希并验证它。

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
