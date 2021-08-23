# MIME

Commonly used MIME content types and file extension mapping.

In `Trivial.Web` [namespace](../) of `Trivial.Mime.dll`.

### Constants

- `MimeConstants.Images`
- `MimeConstants.Audio`
- `MimeConstants.Videos`
- `MimeConstants.Documents`
- `MimeConstants.Web`
- `MimeConstants.Text`
- `MimeConstants.Packages`

### File extension mapping

You can get MIME by a specific file extension name.

```csharp
var av1 = MimeConstants.FromFileExtension(".av1");
var pptx = MimeConstants.FromFileExtension(".pptx");
```
