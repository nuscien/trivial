# CSV

In `Trivial.Text` [namespace](../).

## CSV parser

You can parse a Comma-Separated Values (CSV) text by following way.

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

If you have a model like following.

```csharp
class Model
{
    public string FieldText { get; set; }
    public int FieldNumber { get; set; }
}
```

Now you can map to the CSV file.

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

And you can also send this instance into `StringTableDataReader` construct with field names to load it as a `DbDataReader` object.

## TSV parser

Parse Tab-Separated Values (TSV) text is similar to CSV.

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
