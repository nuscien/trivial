# 数字和进制

数字、数学符号和数字工具。

在 `Trivial.Maths` [命名空间](../) 中。

## 符号

可以通过 `NumberSymbols` 类和 `BooleanSymbols` 类的常量字段获取一些数字符号的字符串。

## 中文和日文数字

可以通过以下方式获取中文和日文数字。

- `ChineseNumerals.Simplified` 简体中文数字。
- `ChineseNumerals.SimplifiedUppercase` 简体中文大写数字。
- `ChineseNumerals.Traditional` 繁体中文数字。
- `ChineseNumerals.TraditionalUppercase` 繁体中文大写数字。
- `JapaneseNumerals.Default` 日文数字。

```csharp
// 获取特定数字的字符串。结果应如下所示。
// 一万两千三百四十五点六七
ChineseNumerals.Simplified.ToString(12345.67);

// 获取每个数字的字符串，通过将第二个参数设置为 true。结果应如下所示。
// 一二三四五
ChineseNumerals.Simplified.ToString(12345, true);

// 获取大写数字字符串。结果应如下所示。
// 壹萬贰仟叄佰肆拾伍
ChineseNumerals.SimplifiedUppercase.ToString(12345);

// 获取特定数字的近似字符串。结果应如下所示。
// 123.5万
var num3 = ChineseNumerals.Simplified.ToApproximationString(1234567);
```

## 罗马数字

可以通过 `RomanNumerals.Uppercase` 静态属性获取大写罗马数字，通过 `RomanNumerals.Lowercase` 获取小写罗马数字。

## 英文数字

可以获取特定数字的英文单词形式呈现方式。

```csharp
// 获取特定数字的字符串。结果应如下所示。
// twelve thousand three hundred and forty-five point six seven
var num1 = EnglishNumerals.Default.ToString(12345.67);

// 获取每个数字的字符串，通过将第二个参数设置为 true。结果应如下所示。
// one two three four five
var num2 = EnglishNumerals.Default.ToString(12345, true);

// 获取特定数字的近似字符串。结果应如下所示。
// 1.2M
var num3 = EnglishNumerals.Default.ToApproximationString(1234567);
```

## 进制

可以将一个数字转换为特定的进制。进制应为 2-36 之间的一个数字。

```csharp
var num = Numbers.ToPositionalNotationString(365, 24); // => f5
```

也可以将其解析回原始数字。

```csharp
var i = Numbers.ParseToInt32("f5", 24); // => 365
```
