# 网络常用格式

```csharp
using Trivial.Web;
```

该格式化助手用于 Web。

## 日期和时间

可以将 JavaScript 的时间戳转换为 `DateTime` 对象，反之亦然。

```csharp
var time = WebFormat.ParseDate(1594958400000);
var tick = WebFormat.ParseDate(DateTime.Now);
```

也可以将字符串解析为 `DateTime` 对象。

```csharp
var d1 = WebFormat.ParseDate("2020W295");
var d2 = WebFormat.ParseDate("2020-7-17 12:00:00");
var d3 = WebFormat.ParseDate("2020-07-17T12:00:00Z");
var d4 = WebFormat.ParseDate("Fri, 17 Jul 2020 12:00:00 GMT");
var d5 = WebFormat.ParseDate("Fri, 17 Jul 2020 04:00:00 GMT-0800");
var d6 = WebFormat.ParseDate("Fri Jul 17 2020 12:00:00 GMT");
```

## Base64Url

- `WebFormat.Base64UrlEncode` 编码为 Base64Url 字符串。
- `WebFormat.Base64UrlDecode` 从 Base64Url 字符串解码。
