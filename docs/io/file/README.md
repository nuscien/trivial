# File and directory

Provide the helper functions and extension functions for file and directory.

In `Trivial.IO` [namespace](../) of `Trivial.dll` [library](../../).

## Directory copy

You can copy a directory by following way.

```csharp
var folder = new DirectoryInfo(@"C:\Temp");
var newFolder = await folder.CopyToAsync("C:\Temp2");
```

## Special path

You can get the path which is in a special folder.

```csharp
var msoDir = FileSystemInfoUtility.GetLocalDir(@"%ProgramFiles%\Microsoft Office");
var explorerFile = FileSystemInfoUtility.GetLocalFile(@"%windir%\explorer.exe");
```

## File size string

You can get a string for the file size.

```csharp
var size = FileSystemInfoUtility.ToFileSizeString(12345); // 12.1KB
```
