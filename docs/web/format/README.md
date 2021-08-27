# Web Format

The format helper for web.

In `Trivial.Web` [namespace](../) of `Trivial.dll` [library](../../).

### Date and time

You can convert a JavaScript tick to or from a `DateTime` object.

```csharp
var time = WebFormat.ParseDate(1594958400000);
var tick = WebFormat.ParseDate(DateTime.Now);
```

You can also parse a string to a `DateTime` object.

```csharp
var d1 = WebFormat.ParseDate("2020W295");
var d2 = WebFormat.ParseDate("2020-7-17 12:00:00");
var d3 = WebFormat.ParseDate("2020-07-17T12:00:00Z");
var d4 = WebFormat.ParseDate("Fri, 17 Jul 2020 12:00:00 GMT");
var d5 = WebFormat.ParseDate("Fri, 17 Jul 2020 04:00:00 GMT-0800");
var d6 = WebFormat.ParseDate("Fri Jul 17 2020 12:00:00 GMT");
```

### Base64Url

- `WebFormat.Base64UrlEncode` Encode to Base64Url string.
- `WebFormat.Base64UrlDecode` Decode from a Base64Url string.
