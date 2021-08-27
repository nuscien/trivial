# MIME

Commonly used MIME content types and its file extension part mapping.

In `Trivial.Web` [namespace](../) of `Trivial.Mime.dll` library.

### Constants

- `MimeConstants.Images`
- `MimeConstants.Audio`
- `MimeConstants.Videos`
- `MimeConstants.Documents`
- `MimeConstants.Web`
- `MimeConstants.Text`
- `MimeConstants.Packages`

### File extension mapping

You can get MIME content type by a specific file extension part.

```csharp
var av1 = MimeConstants.GetByFileExtension(".av1");
var pptx = MimeConstants.GetByFileExtension(".pptx");
```

### For Windows

You can load the specific MIME content type by the file extension part from Windows Register.
But note this is only applied for .NET Framework and .NET 6 on Windows NT OS.

```csharp
MimeConstants.Registry.RegisterFileExtensionMapping(".wma");
var wma = MimeConstants.GetByFileExtension(".wma");
```
