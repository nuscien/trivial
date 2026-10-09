# 汉语拼音

位于 `Trivial.Text` [命名空间](../)中。

## 声母和韵母

以下是声母枚举和韵母枚举。

- `PinyinFinals`
- `PinyinInitials`

你可以将它们中的一个转换为字符串。

```csharp
var o = PinyinMarks.ToString(PinyinInitials.Zh);  // -> zh
o = PinyinMarks.ToString(PinyinFinals.Ang);  // -> ang
o = PinyinMarks.ToString(PinyinFinals.Ang, 1);  // -> āng
```

## 格式化

你可以使用拼音的声母、韵母和声调来格式化一个单词或句子，如下示例所示。

```csharp
var s = "Wo3men2 dou1zai4 yi1Qi3 wan2r, ni3ne?";
var output = PinyinMarks.Format(s);
// -> Wǒmén dōuzài yīqǐ wánr, nǐne?

s = "Wo3men2 doU1zai去 yīQǐ ㄨㄢˊ儿, ni上ne轻?";
output = PinyinMarks.Format(s); // 输出同上方输出。
```
