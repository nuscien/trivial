# OAuth

OAuth 2.0 客户端/服务器和 JWT 支持。

在 `Trivial.Security` [命名空间](../) 中。

## 认证

可以使用以下附加模型进行 OAuth 和应用密钥操作。

- `TokenInfo` 访问令牌和其他属性。
- `AppAccessingKey` 应用标识符和密钥。

可以使用 `OAuthClient` 类的实例以及 `AppAccessingKey` 实例、作用域和授权 URI 来访问需要访问令牌认证的资源。以下是 WNS 的示例。

```csharp
// 初始化一个新的 OAuth 客户端实例
// with client identifier, client secret, authorization URI and scope.
var oauth = new OAuthClient(
    "client_id",        // Client ID.
    "client_secret",    // Client secret.
    new Uri("https://login.live.com/accesstoken.srf"),
    "notify.windows.com");

// 获取访问令牌。
var token = await oauth.ResolveTokenAsync(new ClientTokenRequestBody());

// 然后你可以在需要时创建 JSON HTTP Web 客户端，
// 它会将访问令牌及其类型设置到 HTTP 请求的授权头中。
var httpClient = oauth.Create<ResponseBody>();

// 当然，你也可以通过以下属性获取访问令牌缓存。
token = oauth.Token;
```

## JWT

你可以通过初始化 `JsonWebToken` 类的新实例来创建一个 JSON Web Token，以在 HTTP 请求中获取授权头。

```csharp
// 创建一个哈希签名提供程序。
var sign = HashSignatureProvider.CreateHS512("a secret string");

// 创建一个有效负载。
// 支持任何类型。因此你可以定义你自己的自定义模型类来使用。
// 或者甚至使用 Trivial.Text.JsonObject 或 Newtonsoft.Json.Linq.JObject 类。
var model = new JsonWebTokenPayload
{
    Id = Guid.NewGuid().ToString("n"),
    Subject = "user-or-other-subject-id",
    Issuer = "example"
};

// 创建一个 JWT 实例
// 通过将一个 JsonWebTokenPayload（或一个 JsonObjectNode）
// 和签名提供程序实例相加。
var jwt = model + sign;

// 获取编码后的 JWT 字符串。
var jwtStr = jwt.ToEncodedString();

// 或者获取用于 HttpClient 类的认证头值。
var header = jwt.ToAuthenticationHeaderValue();
```

你可以通过以下方式解析 JWT 字符串。

```csharp
var jwtSame = JsonWebToken<JsonWebTokenPayload>.Parse(jwtStr, sign); // jwtSame.ToEncodedString() == jwtStr
```

或者使用解析器在签名验证之前获取详细信息。

```csharp
var parser = new JsonWebToken<Model>.Parser(jwtStr);

// Verify.
var isVerified = parser.Verify(sign);

// Get payload model.
var payload = parser.GetPayload();

// Convert to a JWT instance.
var jwt = parser.ToToken(sign, true);
```

以下是签名提供程序。你可以调用这些函数之一并传入密钥作为参数。

| 算法名称 | 函数名称 |
| -------------- | ------------------------- |
| HS512 | `HashSignatureProvider.CreateHS512(string pem)` |
| HS384 | `HashSignatureProvider.CreateHS384(string pem)` |
| HS256 | `HashSignatureProvider.CreateHS256(string pem)` |
| RS512 | `RSASignatureProvider.CreateRS512(string pem)` |
| RS384 | `RSASignatureProvider.CreateRS384(string pem)` |
| RS256 | `RSASignatureProvider.CreateRS256(string pem)` |
| ES512 | `ECDsaSignatureProvider.CreateES512(string pem)` |
| ES384 | `ECDsaSignatureProvider.CreateES384(string pem)` |
| ES256 | `ECDsaSignatureProvider.CreateES256(string pem)` |
| ES256K | `ECDsaSignatureProvider.CreateES256K(string pem)` |
| PS512 | `RSASignatureProvider.CreatePS512(string pem)` |
| PS384 | `RSASignatureProvider.CreatePS384(string pem)` |
| PS256 | `RSASignatureProvider.CreatePS256(string pem)` |
| ML-DSA-87 | `new MLDsaSignatureProvider(string pem)` |
| ML-DSA-65 | `new MLDsaSignatureProvider(string pem)` |
| ML-DSA-44 | `new MLDsaSignatureProvider(string pem)` |

你也可以初始化 `KeyedSignatureProvider` 类的新实例来创建你自己的签名提供程序。

请参阅 [JWT.IO](https://jwt.io/) 测试 JWT 或获取详细信息。

## Token request route

在服务器端，你可以使用或继承 `TokenRequestRoute<T>` 类来解析和处理令牌信息请求。

```csharp
// 创建一个路由并注册处理程序。
var route = new TokenRequestRoute<UserInfo>();
route.Register((PasswordTokenRequestBody req, CancellationToken cancellationToken)
    => UserManager.LoginByPasswordAsync(req.UserName, req.Password));
route.Register((RefreshTokenRequestBody req, CancellationToken cancellationToken)
    => UserManager.LoginByRefreshTokenAsync(req.RefreshToken));

// 然后你可以处理以下登录请求。
var resp = await route.SignInAsync(tokenReq);
```
