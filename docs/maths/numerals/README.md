# Numerals

The numerals, mathematics symbols and number utilities.

In `Trivial.Maths` [namespace](../) of `Trivial.dll` [library](../../).

## Symbols

You can get some number symbols in string by the constants fields of the `NumberSymbols` class and the `BooleanSymbols` class.

## Roman numerals

You can get the Roman numerals in `RomanNumerals.Uppercase` static property and `RomanNumerals.Lowercase` for lowercase.

## English numerals

For English speakers, you can get the English words for a specific number.

```csharp
// Get the string for a specific number. It should be following.
// twelve thousand three hundred and forty-five point six seven
var num1 = EnglishNumerals.Default.ToString(12345.67);

// Get the string of the digit one by one by setting the 2nd arg as true. It should be following.
// one two three four five
var num2 = EnglishNumerals.Default.ToString(12345, true);

// Get the string of an approximation for a specific number. It should be following.
// 1.2M
var num3 = EnglishNumerals.Default.ToApproximationString(1234567);
```

## Chinese and Japanese numerals

For Chinese and Japanese, you can get by following way.

- `ChineseNumerals.Simplified` Simplified Chinese numerals.
- `ChineseNumerals.SimplifiedUppercase` Simplified Chinese uppercase numerals.
- `ChineseNumerals.Traditional` Traditional Chinese numerals.
- `ChineseNumerals.TraditionalUppercase` Traditional Chinese uppercase numerals.
- `JapaneseNumerals.Default` Japanese numerals.

```csharp
// Get the string for a specific number. It should be following.
// 一万两千三百四十五点六七
ChineseNumerals.Simplified.ToString(12345.67);

// Get the string of the digit one by one by setting the 2nd arg as true. It should be following.
// 一二三四五
ChineseNumerals.Simplified.ToString(12345, true);

// Get the uppercase number string. It should be following.
// 壹萬贰仟叄佰肆拾伍
ChineseNumerals.SimplifiedUppercase.ToString(12345);

// Get the string of an approximation for a specific number. It should be following.
// 123.5万
var num3 = ChineseNumerals.Simplified.ToApproximationString(1234567);
```

## Positional notation

You can convert a number to a specific radix. The radix should be one of 2-36.

```csharp
var num = Numbers.ToPositionalNotationString(365, 24); // => f5
```

And you can parse it back.

```csharp
var i = Numbers.ParseToInt32("f5", 24); // => 365
```
