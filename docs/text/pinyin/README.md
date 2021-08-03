# Chinese Pinyin

The Pinyin marks and formatter.

In `Trivial.Text` [namespace](./text) of `Trivial.dll` [library](../../).

## Finals and initials

The `enum`s for finals and initials of Pinyin.

- `PinyinFinals`
- `PinyinInitials`

And you can convert one of them to a string.

```csharp
var o = PinyinMarks.ToString(PinyinInitials.Zh);  // -> zh
o = PinyinMarks.ToString(PinyinFinals.Ang);  // -> ang
o = PinyinMarks.ToString(PinyinFinals.Ang, 1);  // -> āng
```

## Format

You can format a word or a sentence with Pinyin intials, finals and tones like following sample.

```csharp
var s = "Wo3men2 dou1zai4 yi1Qi3 wan2r, ni3ne?";
var output = PinyinMarks.Format(s);
// -> Wǒmén dōuzài yīqǐ wánr, nǐne?

s = "Wo3men2 doU1zai去 yīQǐ ㄨㄢˊ儿, ni上ne轻?";
output = PinyinMarks.Format(s); // Same as above output.
```
