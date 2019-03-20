# [Trivial](https://github.com/nuscien/trivial/wiki/core)

This library includes utilities and services for tasks, IO, security, etc.

## [Tasks](https://github.com/nuscien/trivial/wiki/tasks)

Just add following namespace to your code file to use.

```csharp
using Trivial.Tasks;
```

### Debounce

Maybe a handler will be asked to process several times in a short time but you just want to process once at the last time because the previous ones are obsolete. A sample is real-time search. You can use following method to do so.

```csharp
var task = HitTask.Debound(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

### Throttle

A handler to be frozen for a while after it has processed.

```csharp
var task = HitTask.Throttle(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(10000));

// Somewhere to raise.
task.ProcessAsync();
```

### Multiple

A handler to process for the specific times and it will be reset after a while.

```csharp
var task = HitTask.Multiple(() => {
    // Do something...
}, 10, null, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

### Times

A handler to process for the specific times only and it will be reset after a while. A sample is double click.

```csharp
var task = HitTask.Times(() => {
    // Do something...
}, 2, 2, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

### Retry

You can create a linear retry policy or even a customized to process an action with the specific retry policy.
And you can use `ObservableTask` to observe the state of an action processing.

## [Mathematics](https://github.com/nuscien/trivial/wiki/maths)

Just add following namespace to your code file to use.

```csharp
using Trivial.Maths;
```

### Arithmetic

There a lot of arithmetic functions.

```csharp
Arithmetic.IsPrime(2147483647); // True
Arithmetic.IsPrime(21474836479); // False
await Arithmetic.IsPrimeAsync(2305843009213693951); // False

Arithmetic.Factorial(10); // 3628800

Arithmetic.Gcd(192, 128); // 64
Arithmetic.Lcm(192, 128); // 384
```

### Numbers

You can get the number symbols as you want.

You can also get the number string in English words.

```csharp
EnglishNumber.Default.ToString(12345.67);
// twelve thousand three hundred and forty-five point six seven

EnglishNumber.Default.ToString(12345, true);
// one two three four five

EnglishNumber.Default.ToApproximationString(1234567);
// 1.2M
```

And also for Chinese and Japanese.

```csharp
ChineseNumber.Simplified.ToString(12345.67);
// 一万两千三百四十五点六七

ChineseNumber.SimplifiedUppercase.ToString(12345, true);
// 壹贰叄肆伍

JapaneseNumber.Default.ToApproximationString(1234567);
// 123.5万
```

### Angle and polar point

- `Angle` Angle.
- `PolarPoint` The point in polar coordinates.

### Set

- `NullableValueSimpleInterval<T>` Interval, such as [20, 100).

### Rectangular coordinates

- `OneDimensionalPoint` The point in 1D (line) coordinates.
- `TwoDimensionalPoint` The point in 2D (flat) coordinates.
- `ThreeDimensionalPoint` The point in 3D (stereoscophic) coordinates.
- `FourDimensionalPoint` The point in 4D (spacetime) coordinates.

## [Network](https://github.com/nuscien/trivial/wiki/net)

Contains the helper functions and extension functions for network, such as HTTP web client and its content.

```csharp
using Trivial.Net;
```

And you can also use `JsonHttpClient` to serialize the JSON format response with retry policy supports.
And `HttpUri` for HTTP URI fields accessing.

## [Security](https://github.com/nuscien/trivial/wiki/security)

Just add following namespace to your code file to use.

```csharp
using Trivial.Security;
```
### RSA

You can convert a PEM (OpenSSL RSA key) or an XML string to the `RSAParameters` class.

```csharp
var parameters = RSAUtility.Parse(pem);
```

And you can convert back by using the extension method `ToPrivatePEMString` or `ToPublicPEMString`.
And also you can use the extension method `ToXElement` to export the XML.

### Symmetric

You can encrypt and decrypt a string by symmetric algorithm.

```csharp
// AES sample.
var original = "Original secret string";
var cipher = SymmetricUtilities.Encrypt(Aes.Create, original, key, iv);
var back = SymmetricUtilities.Decrypt(Aes.Create, cipher, key, iv); // back == original
```

### Hash

For hash algorithm, you can call `HashUtilities.ToHashString` function to get hash from a plain string and call `HashUtilities.Verify` to verify.

```csharp
var original = "The string to hash";

// SHA-512 (of SHA-2 family)
var sha512Hash = HashUtilities.ToHashString(SHA512.Create, original);
var isVerified = HashUtilities.Verify(SHA512.Create, original, sha512Hash); // --> true

// SHA-3-512
var sha3512Name = new HashAlgorithmName("SHA3512");
var sha3512Hash = HashUtilities.ToHashString(sha3512Name, original);
var isVerified3512 = HashUtilities.Verify(sha3512Name, original, sha3512Hash); // --> true
```

### Access token

We also provide a set of tools for OAuth including following models.

- `TokenInfo` The access token and other properties.
- `AppAccessingKey` The app identifier and secret key.

And you can implement the `OpenIdTokenClient` abstract class or `TokenResolver` abstract class to get and maintain the access token.

### Secure string utiltiy

You can use the extension methods `SecureStringUtiltiy` to convert between `SecureString` and `String`/`StringBuilder`/`Byte[]`.

## [IO](https://github.com/nuscien/trivial/wiki/io)

Contains the helper functions and extension methods for `FileInfo` and `Stream`.

```csharp
using Trivial.IO;
```

## [Data](https://github.com/nuscien/trivial/wiki/data)

Contains lots of data readers, parsers and utilities.

```csharp
using Trivial.Data;
```

### CSV

You can parse a CSV text by following way.

```csharp
var csv = new CsvParser("ab,cd,efg\nhijk,l,mn");
foreach (var item in csv)
{
    Console.WriteLine("{0},{1},{2}", item[0], item[1], item[2]);
}
```

If you have a model like following.

```csharp
class Model
{
    public string FieldA { get; set; }
    public string FieldB { get; set; }
    public string FieldC { get; set; }
}
```

Now you can map to the CSV file.

```csharp
var csv = new CsvParser("ab,cd,efg\nhijk,l,mn");
foreach (var model in csv.ConvertTo<Model>(new[] { "FieldA", "FieldB", "FieldC" }))
{
    Console.WriteLine("{0},{1},{2}", model.FieldA, model.FieldB, model.FieldC);
}
```

And you can also send this instance into `StringTableDataReader` construct with field names to load it as a `DbDataReader` object.

### Data cache collection

You can create a collection to cache data with expiration and count limitation by following way.

```csharp
var cache = new DataCacheCollection<Model>
{
    MaxCount = 1000,
    Expiration = TimeSpan.FromSeconds(100)
};
```

So that you can get the data from the cache if has and or add new one if necessary.

```csharp
if (!cache.TryGet("abcd", out item))
{
    item = new Model();
}
```

## [Geography](https://github.com/nuscien/trivial/wiki/geo)

Contains the models of geography.

```csharp
using Trivial.Geography;
```

### Geolocation

You can use `Geolocation`, `Latitude`, `Longitude` and other models for geolocation information.
