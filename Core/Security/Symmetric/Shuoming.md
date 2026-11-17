# 对称加密

在 `Trivial.Security` [命名空间](../)中。

<!-- Start -->

对称算法工具。

## 对称算法工具

可以使用对称算法加密一个秘密字符串或解密一个密文字符串，如下所示。

```csharp
// Encryption Key and IV.
var key = ...;
var iv = ...;

// AES sample.
var original = "Original secret string";
var cipher = SymmetricUtilities.Encrypt(Aes.Create, original, key, iv);
var back = SymmetricUtilities.Decrypt(Aes.Create, cipher, key, iv); // back == original
```
