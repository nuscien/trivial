# RSA

RSA 工具和相关模型。

在 `Trivial.Security` [命名空间](../) 中。

## RSA parameters convert

你可以将 PEM 字符串（OpenSSL RSA 密钥）或 XML 格式字符串解析为 `RSAParameters` 类。

```csharp
var parameters = RSAParametersConvert.Parse(pem);
```

你也可以使用扩展函数 `ToPrivatePEMString` 或 `ToPublicPEMString` 将其转换回 PEM 字符串。你还可以使用扩展函数 `ToXElement` 或 `ToXmlDocument` 将其导出为 XML。

## RSA secret exchange

你可以将本地的密钥或其他字符串加密后发送到另一方（客户端/服务器端），然后使用该方所需的公钥进行加密。并将当前容器中注册的公钥发送给另一方，以便它可以使用相同的机制将加密的密钥传回，你可以使用你的私钥进行解密。

以下是一个流程示例，假设一方是服务器端，另一方是客户端。

1. 客户端请求服务器加密密钥以加密客户端填写的授权表单。
2. 服务器发送一个服务器加密密钥及其标识符。该密钥是用于加密的公钥（此处标记为 S-Key-Public）。其私钥（此处标记为 S-Key-Private）用于解密。两个密钥及其标识符（此处标记为 S-Key-Id）应缓存在服务器中。
3. 客户端创建一对私钥和公钥。私钥（此处标记为 C-Key-Private）用于解密数据，公钥（此处标记为 C-Key-Public）用于加密。密钥对应与标识符（此处标记为 C-Key-Id）一起缓存在客户端。
4. 客户端使用 S-Key-Public 加密表单，并将其与 S-Key-Id、C-Key-Public 和 C-Key-Id 一起发送回服务器。客户端还需要在本地缓存 S-Key-Id 和 S-Key-Public。
5. 服务器使用 S-Key-Id 查找 S-Key-Private 以解密表单。然后生成一个密钥并使用 C-Key-Public 加密。然后将加密的密钥与 C-Key-Id 一起发送回客户端。服务器还需要缓存 C-Key-Id 和 C-Key-Public，并与令牌建立映射关系。
6. 客户端使用 C-Key-Id 查找 C-Key-Private 以解密密钥。下次，客户端发送数据和使用 S-Key-Public 加密的密钥及 S-Key-Id。
7. 服务器使用 S-Key-Id 查找 S-Key-Private 以解密密钥以进行授权。然后处理请求的业务逻辑。返回结果和使用 C-Key-Public 加密的密钥及 C-Key-Id。

```csharp
// 创建一个密钥交换实例并生成一对 RSA 密钥。
var exchange = new RSASecretExchange();
exchange.CreateCrypto();

// 获取要发送给另一方的公钥，以便他们可以使用它来加密密钥并发送回我们。
var publicKey = exchange.PublicKey.ToPublicPEMString();

// 保存来自另一方的密钥。密钥是由当前公钥加密的，我们可以使用此实例中存储的私钥进行解密。
exchange.DecryptSecret(secretReceived);

// 保存另一方的公钥。
var otherSidePublicKey = ...; // 来自另一方的 RSA 公钥。
exchange.EncryptKey = RSAUtility.Parse(otherSidePublicKey);

// 获取由另一方公钥加密的密钥的 Base64。
var secretToSend = exchange.EncryptSecret();

// 获取 JSON Web 密钥格式的认证头值，例如使用 HMAC SHA-512 密钥哈希算法进行签名。
var sign = HashSignatureProvider.CreateHS512("一个秘密哈希密钥");
var jwt = exchange.ToJsonWebTokenAuthenticationHeaderValue(sign);
```
