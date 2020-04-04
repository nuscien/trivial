# OAuth

OAuth 2.0 client/server and JWT supports.

In `Trivial.Security` [namespace](./security) of `Trivial.dll` [library](../README).

## Authentication

You can use following additional models for OAuth and app key.

- `TokenInfo` The access token and other properties.
- `AppAccessingKey` The app identifier and secret key.

You can access the resources required access token authentication by using an instance of the `OAuthClient` class with the `AppAccessingKey` instance, scope and authorization URI. Following is an example for WNS.

```csharp
// Inialize a new instance of OAuth client
// with client identifier, client secret, authorization URI and scope.
var oauth = new OAuthClient(
    "client_id",        // Client ID.
    "client_secret",    // Client secret.
    new Uri("https://login.live.com/accesstoken.srf"),
    "notify.windows.com");

// Get access token.
var token = await oauth.ResolveTokenAsync(new ClientTokenRequestBody());

// Then you can create the JSON HTTP web client when you need,
// And it will set the access token and its type into the authorization header of HTTP request.
var httpClient = oauth.Create<ResponseBody>();

// And, of course, you can get the access token cache by following property.
token = oauth.Token;
```

## JWT

You can create a JSON web token to get an authorization header in HTTP request by initializing a new instance of the `JsonWebToken` class.

```csharp
// Create a hash signature provider.
var sign = HashSignatureProvider.CreateHS512("a secret string");

// Create a payload.
// Supports any type. So you can define your customized model class to use.
// Or even use Trivial.Text.JsonObject or Newtonsoft.Json.Linq.JObject class.
var model = new JsonWebTokenPayload
{
    Id = Guid.NewGuid().ToString("n"),
    Issuer = "example"
};

// Create a JWT instance.
var jwt = new JsonWebToken<JsonWebTokenPayload>(model, sign);

// Get the JWT string encoded.
var jwtStr = jwt.ToEncodedString();

// Or get authenticiation header value for HttpClient class using.
var header = jwt.ToAuthenticationHeaderValue();
```

You can parse a JWT string as following way.

```csharp
var jwtSame = JsonWebToken<JsonWebTokenPayload>.Parse(jwtStr, sign); // jwtSame.ToEncodedString() == jwtStr
```

Or use a parser to get details before sign validation.

```csharp
var parser = new JsonWebToken<Model>.Parser(jwtStr);

// Verify.
var isVerified = parser.Verify(sign);

// Get payload model.
var payload = parser.GetPayload();

// Convert to a JWT instance.
var jwt = parser.ToToken(sign, true);
```

Following are the signature providers. You can call one of these function and pass the secret as a parameter.

| Algorithm Name | Function Name |
| -------------- | ------------------------- |
| HS512 | `HashSignatureProvider.CreateHS512` |
| HS384 | `HashSignatureProvider.CreateHS384` |
| HS256 | `HashSignatureProvider.CreateHS256` |
| RS512 | `RSASignatureProvider.CreateRS512` |
| RS384 | `RSASignatureProvider.CreateRS384` |
| RS256 | `RSASignatureProvider.CreateRS256` |

You can also initialize a new instance of the `KeyedSignatureProvider` class for your own signature provider.

See [JWT.IO](https://jwt.io/) to test JWT or get details.

## Token request route

For server side, you can use or inherit class `TokenRequestRoute<T>` to parse and process the token info request.

```csharp
// Create a route and register the handlers.
var route = new TokenRequestRoute<UserInfo>();
PasswordTokenRequestBody.Register(route, async req =>
{
    return await UserManager.LoginByPasswordAsync(req.UserName, req.Password);
});
RefreshTokenRequestBody.Register(route, async req =>
{
    return await UserManager.LoginByRefreshTokenAsync(req.RefreshToken);
});

// Then you can handle following login request.
var resp = await route.SignInAsync(tokenReq);
```
