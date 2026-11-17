# String Utilities

在 `Trivial.Text` [命名空间](../)中。

<!-- Start -->

提供一些文本的辅助函数和扩展函数。

## 字符串大小写

你可以将字符串转换为特定的大小写。

- 全部大写。
- 全部小写。
- 仅首字母大写。
- 仅首字母小写。

```csharp
var a = " how are you?".ToSpecificCase(Cases.Capitalize);
// a -> " How are you?"
```

## 换行

你可以将字符串按特定长度换行。

```csharp
var lines = StringExtensions.BreakLines("A very long text...", 70);
```

## Yield 分割

你可以将字符串按指定分隔符分割，并以 `IEnumerable<string>` 输出。

```csharp
var words = "Some words here to split".YieldSplit(' ');
```

## 查找

你可以在字符串中查找子字符串。

```csharp
const string Test = "<a href=\"https://trivial.kingcean.net\" >Trivial official website</a>";
var span = StringExtensions.After(Test, '\"'); // => https://trivial.kingcean.net\" >Trivial…
span = StringExtensions.Before(Test, '\"'); // => https://trivial.kingcean.net
```
