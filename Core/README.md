# Trivial

Includes utilities and services for tasks, security, JSON, etc.

## [Tasks](https://trivial.kingcean.net/tasks)

```csharp
using Trivial.Tasks;
```

### Interceptor

Set an action with a specific interceptor to control its execution.

- `Interceptor.Debounce`:
  Request to invoke a specific action several times in a short time but only the last one should be executed and previous ones should be ignored.
  A sample is real-time search suggestion.
- `Interceptor.Throttle`:
  Raise actually only once in a short time even if invoke several times.
  A sample is the submit button in a form.
- `Interceptor.Times`:
  The action can be only executed only when invoke in the specific times range and others will be ignored.
  A sample is double click.
- `Interceptor.Multiple`:
  A handler to execute for the specific times and the state will be reset after a while.

```csharp
// Set an action with interceptor.
var action = Interceptor.Debounce(() =>
{
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Invoke somewhere.
action();
```

### Retry

Create a linear retry policy by `LinearRetryPolicy` or a customized one to process an action with the specific retry policy.
And you can use `ObservableTask` to observe the state of an action processing.

## [Network](https://trivial.kingcean.net/net)

Contains the helper functions and extension functions for network, such as HTTP web client and its content.

```csharp
using Trivial.Net;
```

Use `JsonHttpClient` to serialize the JSON format response with retry policy supports.
And `HttpUri` for HTTP URI fields accessing.

## [Security](https://trivial.kingcean.net/security)

```csharp
using Trivial.Security;
```

### RSA

Convert a PEM (OpenSSL RSA key) or an XML string to the `RSAParameters` struct.

```csharp
var parameters = RSAParametersConvert.Parse(pem);
```

And you can convert back by using the extension method `ToPrivatePEMString` or `ToPublicPEMString`.
And the extension method `ToXElement` to XML.

### Symmetric & Hash

Use a symmetric algorithm to encrypt and decrypt a string by calling `SymmetricUtilities.Encrypt` and `SymmetricUtilities.DecryptText` functions.

For hash algorithm, you can call `HashUtilities.ToHashString` function to get hash from a plain string and call `HashUtilities.Verify` to verify.

### Access token

Provide a set of tools for OAuth including following models.

- `TokenInfo` The access token and other properties.
- `AppAccessingKey` The app identifier and secret key.
- `OAuthClient` The token container with the ability to resolve the access token and create the JSON HTTP web client to access the resources required authentication.

Optional to implement the `OAuthBasedClient` base class to create your own business HTTP web client factory with OAuth supports.

### JWT

Create a JSON web token to get the string encoded by initializing a new instance of the `JsonWebToken` class or the `JsonWebTokenParser` class.

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

Use the extension methods in the `SecureStringExtensions` class to convert the secret between `SecureString` and `String`/`StringBuilder`/`Byte[]`.

And class `RSASecretExchange` is used to transfer the secret with RSA encryption.

## [Text](https://trivial.kingcean.net/text)

```csharp
using Trivial.Text;
```

### JSON

Includes writable JSON DOM `JsonObjectNode` and `JsonArrayNode`.
And includes lots of useful converter like following.

- `JsonJavaScriptTicksConverter`, and its fallback converters, to convert `DateTime` from/to JavaScript ticks number in JSON.
- `JsonUnixTimestampConverter`, and its fallback converters, to convert `DateTime` from/to Unix timestamp number in JSON.
- `JsonNumberConverter` and `JsonNumberConverter.NumberStringConverter`, to read number string in JSON.
- `JsonStringListConverter` and its character separated converters (such as `JsonStringListConverter.WhiteSpaceSeparatedConverter`), to convert a string list from/to a string in JSON.

### CSV

Read CSV or TSV file into a list of the specific models.
For example, you have a model class `CsvModel` with string properties `A` and `B`, now you can map to the CSV file.

```csharp
var csv = new CsvParser("abcd,efg\nhijk,lmn");
foreach (var model in csv.ConvertTo<CsvModel>(new[] { "A", "B" }))
{
    Console.WriteLine("{0},{1}", model.A, model.B);
}
```

## [Data](https://trivial.kingcean.net/data)

```csharp
using Trivial.Data;
```

### Cache

Save a number of model in memory cache by generic class `DataCacheCollection`.

## [Mathematics](https://trivial.kingcean.net/maths)

```csharp
using Trivial.Maths;
```

### Arithmetic

There are a lot of arithmetic functions.

```csharp
Arithmetic.IsPrime(2147483647); // True
Arithmetic.Gcd(192, 128); // 64
```

### Numerals

Get the number symbols as you want. And get the numerals in English.

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

- `NullableValueSimpleInterval<T>` Interval, such as `[20, 100)`.

### Rectangular coordinates

- `OneDimensionalPoint` The point in 1D (line) coordinates.
- `TwoDimensionalPoint` The point in 2D (flat) coordinates.
- `ThreeDimensionalPoint` The point in 3D (stereoscophic) coordinates.
- `FourDimensionalPoint` The point in 4D (spacetime) coordinates.

## [Drawing](https://trivial.kingcean.net/drawing)

```csharp
using Trivial.Drawing;
```

### Color calculator

Color adjustment, converter, parser and mixer.

```csharp
var color = ColorCalculator.Parse("rgb(226, 37, 0xA8)");
color = ColorCalculator.Opacity(color, 0.9);
color = ColorCalculator.Saturate(color, RelativeSaturationLevels.High);
color = ColorCalculator.Mix(ColorMixTypes.Lighten, color, Color.FromArgb(0, 240, 0));
```

## Further

- [IO](https://trivial.kingcean.net/io)
- [Geography](https://trivial.kingcean.net/geo)
- [Reflection](https://trivial.kingcean.net/reflection)
- [CommandLine](https://trivial.kingcean.net/cmdline)
