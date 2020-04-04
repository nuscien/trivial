# HTTP URI

You can access the parts of HTTP URI directly and convert from/to `Uri`.

In `Trivial.Net` [namespace](./net) of `Trivial.dll` [library](../README).

## Uri

You can create an HTTP URI instance by `HttpUri` to set the properties and convert from/to URI object.

```csharp
var url = "https://dotnet.microsoft.com/learn?q=abcd&from=#welcome";
var httpUri = HttpUri.Parse(url);

// httpUri.IsSecure == true
// httpUri.Host == "dotnet.microsoft.com"
// httpUri.Query.ToString() == "q=abcd&from="
// httpUri.Path == "/learn"
// httpUri.Hash == "#welcome"

// httpUri.ToString() == url
// ((Uri)httpUri).ToString() == url
// (HttpUri)(new Uri(url)) == httpUri
```

## Query

For accessing query, you can use the `QueryData` class.
