# 文件和目录

在 `Trivial.IO` [命名空间](../)中。

<!-- Start -->

提供文件和目录的辅助函数和扩展函数。

## 目录复制

可以通过以下方式复制一个目录。

```csharp
var folder = new DirectoryInfo(@"C:\Temp");
var newFolder = await folder.CopyToAsync("C:\Temp2");
```

## 特殊路径

可以获取位于特殊文件夹中的路径。

```csharp
var msoDir = FileSystemInfoUtility.GetLocalDir(@"%ProgramFiles%\Microsoft Office");
var explorerFile = FileSystemInfoUtility.GetLocalFile(@"%windir%\explorer.exe");
```

## 文件大小字符串

可以获取文件大小的字符串表示。

```csharp
var size = FileSystemInfoUtility.ToFileSizeString(12345); // 12.1KB
```
