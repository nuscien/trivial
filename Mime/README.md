# [Trivial.Mime](../docs/web/mime)

Commonly used MIME content types and its file extension part mapping.

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
- `MimeConstants.Multipart`

## File extension mapping

You can get MIME by a specific file extension name.

```csharp
var av1 = MimeConstants.GetByFileExtension(".av1");
var pptx = MimeConstants.GetByFileExtension(".pptx");
```
