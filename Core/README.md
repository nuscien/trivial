# [Trivial](../docs/core)

This library includes utilities and services for tasks, security, JSON, etc.

## [Tasks](../docs/tasks)

```csharp
using Trivial.Tasks;
```

### Hit task

You can create a task controller to manage when a handler should be raised.

- `HitTask.Debounce`:
  You may request to call a specific action several times in a short time but only the last one should be processed and previous ones should be ignored.
  A sample is real-time search suggestion.
- `HitTask.Throttle`:
  You may want to request to call an action only once in a short time even if you request to call several times.
  A sample is the submit button in a form.
- `HitTask.Times`:
  You can define an action can be only processed only when request to call in the specific times range and others will be ignored.
  A sample is double click.
- `HitTask.Multiple`:
  A handler to process for the specific times and the state will be reset after a while.

Following is an example for debounce.

```csharp
// Create a task.
var task = HitTask.Debounce(() =>
{
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Raise somewhere.
task.ProcessAsync();
```

### Retry

You can create a linear retry policy by `LinearRetryPolicy` or a customized one to process an action with the specific retry policy.
And you can use `ObservableTask` to observe the state of an action processing.

## [Network](../docs/net)

Contains the helper functions and extension functions for network, such as HTTP web client and its content.

```csharp
using Trivial.Net;
```

And you can also use `JsonHttpClient` to serialize the JSON format response with retry policy supports.
And `HttpUri` for HTTP URI fields accessing.

## [Security](../docs/security)

```csharp
using Trivial.Security;
```

### RSA

You can convert a PEM (OpenSSL RSA key) or an XML string to the `RSAParameters` struct.

```csharp
var parameters = RSAParametersConvert.Parse(pem);
```

And you can convert back by using the extension method `ToPrivatePEMString` or `ToPublicPEMString`.
And the extension method `ToXElement` to XML.

### Symmetric & Hash

You can use a symmetric algorithm to encrypt and decrypt a string by calling `SymmetricUtilities.Encrypt` and `SymmetricUtilities.DecryptText` functions.

For hash algorithm, you can call `HashUtilities.ToHashString` function to get hash from a plain string and call `HashUtilities.Verify` to verify.

### Access token

We also provide a set of tools for OAuth including following models.

- `TokenInfo` The access token and other properties.
- `AppAccessingKey` The app identifier and secret key.
- `OAuthClient` The token container with the ability to resolve the access token and create the JSON HTTP web client to access the resources required authentication.

And you can also implement the `OAuthBasedClient` base class to create your own business HTTP web client factory with OAuth supports.

### JWT

You can create a JSON web token to get the string encoded by initializing a new instance of the `JsonWebToken` class or the `JsonWebTokenParser` class.

```csharp
var sign = HashSignatureProvider.CreateHS512("a secret string");
var jwt = new JsonWebToken<JsonWebTokenPayload>(new JsonWebTokenPayload
{
    Id = Guid.NewGuid().ToString("n"),
    Issuer = "example"
}, sign);

// Get authenticiation header value.
var header = jwt.ToAuthenticationHeaderValue();

// Parse.
var jwtSame = JsonWebToken<Model>.Parse(jwtStr, sign); // jwtSame.ToEncodedString() == header.Parameter
```

### Secure string

You can use the extension methods in the `SecureStringExtensions` class to convert the secret between `SecureString` and `String`/`StringBuilder`/`Byte[]`.

You can also use the class `RSASecretExchange` to transfer the secret with RSA encryption.

## [Text](../docs/text)

```csharp
using Trivial.Text;
```

### JSON

Includes writable JSON DOM `JsonObject` and `JsonArray`.
And includes lots of useful converter like following.

- `JsonJavaScriptTicksConverter`, and its nullable value conveters and fallback converters, to convert `DateTime` or `DateTime?` from/to JavaScript ticks number in JSON.
- `JsonUnixTimestampConverter`, and its nullable value conveters and fallback converters, to convert `DateTime` or `DateTime?` from/to Unix timestamp number in JSON.
- `JsonNumberConverter` and `JsonNumberConverter.NumberStringConverter`, to read number string in JSON.
- `JsonStringListConverter` and its character separated converters (such as `JsonStringListConverter.WhiteSpaceSeparatedConverter`), to convert a string list from/to a string in JSON.
- `JsonObjectConverter`, to convert `JsonObject` and `JsonArray`.

### CSV

You can read CSV file into a list of the specific models.
For example, you have a model class `CsvModel` with string properties `A` and `B`, now you can map to the CSV file.

```csharp
var csv = new CsvParser("abcd,efg\nhijk,lmn");
foreach (var model in csv.ConvertTo<CsvModel>(new[] { "A", "B" }))
{
    Console.WriteLine("{0},{1}", model.A, model.B);
}
```


## [Data](../docs/data)

```csharp
using Trivial.Data;
```

### Cache

You can save a number of model in memory cache by generic class `DataCacheCollection`.

## [Mathematics](../docs/maths)

```csharp
using Trivial.Maths;
```

### Arithmetic

There a lot of arithmetic functions.

```csharp
Arithmetic.IsPrime(2147483647); // True
Arithmetic.Gcd(192, 128); // 64
```

### Numerals

You can get the number symbols as you want. And get the numerals in English.

```csharp
EnglishNumerals.Default.ToString(12345.67);
// twelve thousand three hundred and forty-five point six seven

EnglishNumerals.Default.ToApproximationString(1234567);
// 1.2M
```

And `ChineseNumerals` for Chinese and `JapaneseNumerals` for Japanese.

### Angle and polar point

- `Angle` Angle.
- `PolarPoint` The point in polar coordinates.
- `SphericalPoint` The point in spherical coordinates.

### Set

- `NullableValueSimpleInterval<T>` Interval, such as [20, 100).

### Rectangular coordinates

- `OneDimensionalPoint` The point in 1D (line) coordinates.
- `TwoDimensionalPoint` The point in 2D (flat) coordinates.
- `ThreeDimensionalPoint` The point in 3D (stereoscophic) coordinates.
- `FourDimensionalPoint` The point in 4D (spacetime) coordinates.

## Further

- [IO](../docs/io)
- [Geography](../docs/geo)
- [Reflection](../docs/reflection)
