# CSV

在 `Trivial.Text` [命名空间](../)中。

<!-- Start -->

## CSV 解析器

你可以通过以下方式解析逗号分隔值 (CSV) 文本。

```csharp
var csv = new CsvParser("ab,cd,efg\nhijk,l,mn");
foreach (var item in csv)
{
    Console.WriteLine("{0}    {1}    {2}", item[0], item[1], item[2]);
}
// Output:
// ab    cd    efg
// hijk    l    mn
```

如果你有一个如下的模型。

```csharp
class Model
{
    public string FieldText { get; set; }
    public int FieldNumber { get; set; }
}
```

现在你可以将其映射到 CSV 文件。

```csharp
var csv = new CsvParser("abcdefg,123\n\"hijk,lmn\", 456");
foreach (var model in csv.ConvertTo<Model>(new[] { "FieldText", "FieldNumber" }))
{
    Console.WriteLine("{0}    {1}", model.FieldText, model.FieldNumber);
}
// Output:
// abcdefg    123
// hijk,lmn    456
```

你也可以将此实例发送到 `StringTableDataReader` 构造函数中，并使用字段名称将其加载为 `DbDataReader` 对象。

## TSV 解析器

解析制表符分隔值 (TSV) 文本与 CSV 类似。

```csharp
var tsv = new TsvParser("ab\tcd\tefg\nhijk\tl\tmn");
foreach (var item in tsv)
{
    Console.WriteLine("{0}    {1}    {2}", item[0], item[1], item[2]);
}
// Output:
// ab    cd    efg
// hijk    l    mn
```
