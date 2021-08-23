# [Trivial.Mime](../docs/web/mime)

Commonly used MIME content types and file extension mapping.

## Import

Just add following namespace to your code file to use.

```csharp
using Trivial.Web;
```

## Constants

- `MimeConstants.Images`
- `MimeConstants.Audio`
- `MimeConstants.Videos`
- `MimeConstants.Documents`
- `MimeConstants.Web`
- `MimeConstants.Text`
- `MimeConstants.Packages`

## File extension mapping

You can get MIME by a specific file extension name.

```csharp
var av1 = MimeConstants.FromFileExtension(".av1");
var pptx = MimeConstants.FromFileExtension(".pptx");
```
