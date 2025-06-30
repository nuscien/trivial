using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.IO;

/// <summary>
/// File system info utility.
/// </summary>
public static class FileSystemInfoUtility
{
    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The destination directory.</returns>
    private static DirectoryInfo CopyTo(DirectoryInfo source, string destPath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(destPath);
        foreach (var item in source.GetFiles())
        {
            item.CopyTo(Path.Combine(destPath, item.Name), true);
        }

        cancellationToken.ThrowIfCancellationRequested();
        foreach (var item in source.GetDirectories())
        {
            source = item;
            CopyTo(item, Path.Combine(destPath, item.Name), cancellationToken);
        }

        return new DirectoryInfo(destPath);
    }

    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <returns>The destination directory.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static DirectoryInfo CopyTo(this DirectoryInfo source, string destPath)
        => CopyTo(source, destPath, CancellationToken.None);

    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <param name="cancellationToken">The opitional cancellation token.</param>
    /// <returns>The destination directory.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static Task<DirectoryInfo> CopyToAsync(this DirectoryInfo source, string destPath, CancellationToken cancellationToken = default)
        => Task.Run(() =>
        {
            return CopyTo(source, destPath, cancellationToken);
        });

    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>true if copy succeeded; otherwise, false.</returns>
    private static bool TryCopyTo(DirectoryInfo source, string destPath, CancellationToken cancellationToken)
    {
        try
        {
            if (!source.Exists) return false;
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (var item in source.GetFiles())
            {
                item.CopyTo(Path.Combine(destPath, item.Name), true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var item in source.GetDirectories())
            {
                source = item;
                TryCopyTo(item, Path.Combine(destPath, item.Name), cancellationToken);
            }

            return true;
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return false;
    }

    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <returns>true if copy succeeded; otherwise, false.</returns>
    public static bool TryCopyTo(this DirectoryInfo source, string destPath)
        => TryCopyTo(source, destPath, CancellationToken.None);

    /// <summary>
    /// Copies a directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="destPath">The destinate directory path.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>true if copy succeeded; otherwise, false.</returns>
    public static Task<bool> TryCopyToAsync(this DirectoryInfo source, string destPath, CancellationToken cancellationToken = default)
        => Task.Run(() =>
        {
            return TryCopyTo(source, destPath, cancellationToken);
        });

    /// <summary>
    /// Tries to write the specific string to file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The path of file to write.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public static bool TryWriteAllTextAsync(string path, string content, Encoding encoding = null)
    {
        try
        {
            if (encoding == null) File.WriteAllText(path, content);
            else File.WriteAllText(path, content, encoding);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (IOException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (AggregateException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Tries to write the specific string to file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The path of file to write.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public static async Task<bool> TryWriteAllTextAsync(string path, string content, Encoding encoding = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (encoding == null) await File.WriteAllTextAsync(path, content, cancellationToken);
            else await File.WriteAllTextAsync(path, content, encoding, cancellationToken);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (IOException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (AggregateException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }
#endif

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    public static IEnumerable<string> ReadLines(this FileInfo file, Encoding encoding, bool removeEmptyLine = false)
        => CharsReader.ReadLines(file, encoding, removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    public static IEnumerable<string> ReadLines(this FileInfo file, bool detectEncodingFromByteOrderMarks, bool removeEmptyLine = false)
        => CharsReader.ReadLines(file, detectEncodingFromByteOrderMarks, removeEmptyLine);

    /// <summary>
    /// Gets the string of the file size.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="unit">The unit.</param>
    /// <returns>A string.</returns>
    public static string ToFileSizeString(this FileInfo file, string unit = "B")
        => ToFileSizeString(file.Length, unit);

    /// <summary>
    /// Gets the string of the file size.
    /// </summary>
    /// <param name="size">The file size.</param>
    /// <param name="unit">The unit.</param>
    /// <returns>A string.</returns>
    public static string ToFileSizeString(int size, string unit = "B")
        => ToFileSizeString((long)size, unit);

    /// <summary>
    /// Gets the approximation string of the file size.
    /// </summary>
    /// <param name="size">The file size.</param>
    /// <param name="unit">The unit.</param>
    /// <returns>A string.</returns>
    public static string ToFileSizeString(long size, string unit = "B")
    {
        var prefix = string.Empty;
        if (size < 0)
        {
            prefix = "-";
            size = -size;
        }

        return prefix + ToFileSizeString((ulong)size, unit);
    }

    /// <summary>
    /// Gets the approximation string of the file size.
    /// </summary>
    /// <param name="size">The file size.</param>
    /// <param name="unit">The unit.</param>
    /// <returns>A string.</returns>
    public static string ToFileSizeString(ulong size, string unit = "B")
    {
        if (unit == null) unit = string.Empty;
        switch (unit.Trim())
        {
            case "":
            case "B":
            case "Byte":
            case "Bytes":
            case "BYTE":
            case "BYTES":
            case "Octet":
            case "Octets":
            case "OCTET":
            case "OCTETS":
            case "字节":
            case "位元組":
                break;
            case "k":
            case "K":
            case "KB":
            case "千":
            case "千字节":
                return (size / 1024.0).ToString("F1") + unit;
            case "M":
            case "MB":
            case "兆":
            case "兆字节":
                return (size / 1048576.0).ToString("F1") + unit;
            case "G":
            case "GB":
            case "吉":
            case "吉字节":
                return (size / 1073741824.0).ToString("F1") + unit;
            case "T":
            case "TB":
            case "太":
            case "太字节":
                return (size / 1099511627776.0).ToString("F1") + unit;
            case "P":
            case "PB":
                return (size / 1125899906842624.0).ToString("F1") + unit;
            case "E":
            case "EB":
                return (size / 1152921504606846976.0).ToString("F1") + unit;
        }

        if (size > 1125336956000000000) return (size / 1152921504606846976.0).ToString("F1") + "E" + unit;
        if (size > 1098961000000000) return (size / 1125899906842624.0).ToString("F1") + "P" + unit;
        if (size > 1073204000000) return (size / 1099511627776.0).ToString("F1") + "T" + unit;
        if (size > 1048000000) return (size / 1073741824.0).ToString("F1") + "G" + unit;
        if (size > 1023400) return (size / 1048576.0).ToString("F1") + "M" + unit;
        if (size > 999) return (size / 1024.0).ToString("F1") + "K" + unit;
        return size.ToString() + unit;
    }

#pragma warning disable IDE0057
    /// <summary>
    /// Gets the directory information instance by relative path.
    /// </summary>
    /// <param name="root">The root path.</param>
    /// <param name="relative">The relative path.</param>
    /// <returns>The directory.</returns>
    public static DirectoryInfo GetDirectoryInfoByRelative(DirectoryInfo root, string relative)
    {
        if (string.IsNullOrEmpty(relative))
            return root;
        if (relative.EndsWith('/') || relative.EndsWith('\\'))
            relative = relative.Substring(0, relative.Length - 1);
        if (relative.Length < 1 || relative == "." || relative == "~")
            return root;
        if (relative.StartsWith("./") || relative.StartsWith(".\\"))
            relative = relative.Substring(2);
        while (relative.StartsWith("../") || relative.StartsWith("..\\"))
        {
            root = root.Parent;
            relative = relative.Substring(3);
        }

        if (relative == "..")
            return root.Parent;
        return relative == "." ? root : TryGetDirectoryInfo(root.FullName, relative);
    }

    /// <summary>
    /// Gets the file information instance by relative path.
    /// </summary>
    /// <param name="root">The root path.</param>
    /// <param name="relative">The relative path.</param>
    /// <returns>The file.</returns>
    public static FileInfo GetFileInfoByRelative(DirectoryInfo root, string relative)
    {
        if (string.IsNullOrEmpty(relative))
            return null;
        if (relative.EndsWith('/') || relative.EndsWith('\\'))
            relative = relative.Substring(0, relative.Length - 1);
        if (relative.Length < 1 || relative == "." || relative == "~")
            return null;
        if (relative.StartsWith("./") || relative.StartsWith(".\\"))
            relative = relative.Substring(2);
        while (relative.StartsWith("../") || relative.StartsWith("..\\"))
        {
            root = root.Parent;
            relative = relative.Substring(3);
        }

        return relative == ".." || relative == "." ? null : TryGetFileInfo(root.FullName, relative);
    }
#pragma warning restore IDE0057

    /// <summary>
    /// Gets the specific directory path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="path2">The second path to combine.</param>
    /// <param name="path3">The third path to combine.</param>
    /// <param name="path4">The forth path to combine.</param>
    /// <returns>The path.</returns>
    /// <remarks>For Windows OS only.</remarks>
    public static string GetLocalPath(string path, string path2 = null, string path3 = null, string path4 = null)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        if (!string.IsNullOrWhiteSpace(path2)) path = Path.Combine(path, path2);
        if (!string.IsNullOrWhiteSpace(path3)) path = Path.Combine(path, path3);
        if (!string.IsNullOrWhiteSpace(path4)) path = Path.Combine(path, path4);
        if (path.IndexOf("%") != 0) return path;
        var arr = new Dictionary<string, Environment.SpecialFolder>
        {
            { "AppData", Environment.SpecialFolder.ApplicationData },
            { "appdata", Environment.SpecialFolder.ApplicationData },
            { "LocalAppData", Environment.SpecialFolder.LocalApplicationData },
            { "localappdata", Environment.SpecialFolder.LocalApplicationData },
            { "ProgramFiles", Environment.SpecialFolder.ProgramFiles },
            { "programfiles", Environment.SpecialFolder.ProgramFiles },
            { "ProgramFiles(x86)", Environment.SpecialFolder.ProgramFilesX86 },
            { "programfiles(x86)", Environment.SpecialFolder.ProgramFilesX86 },
            { "CommonProgramFiles", Environment.SpecialFolder.CommonProgramFiles },
            { "CommonProgramFiles(x86)", Environment.SpecialFolder.CommonProgramFilesX86 },
            { "CommonProgramFilesW6432", Environment.SpecialFolder.CommonProgramFiles },
            { "commonprogramFiles", Environment.SpecialFolder.CommonProgramFiles },
            { "commonprogramfiles(x86)", Environment.SpecialFolder.CommonProgramFilesX86 },
            { "commonprogramfilesw6432", Environment.SpecialFolder.CommonProgramFiles },
            { "WinDir", Environment.SpecialFolder.Windows },
            { "windir", Environment.SpecialFolder.Windows },
            { "SystemRoot", Environment.SpecialFolder.Windows },
            { "systemroot", Environment.SpecialFolder.Windows },
            { "UserProfile", Environment.SpecialFolder.UserProfile },
            { "userprofile", Environment.SpecialFolder.UserProfile }
        };
        var endSign = path.IndexOf("%\\", 2) - 1;
        if (endSign < 3) return null;
        var sign = path.Substring(1, endSign);
        if (!arr.TryGetValue(sign, out Environment.SpecialFolder value)) return null;
        try
        {
            return string.Concat(Environment.GetFolderPath(value), path.Substring(sign.Length + 2));
        }
        catch (ArgumentException)
        {
        }
        catch (PlatformNotSupportedException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets a directory information instance of the specific path.
    /// </summary>
    /// <param name="folder">The folder path.</param>
    /// <param name="folderName">The sub-folder name.</param>
    /// <param name="folderName2">The sub-sub-folder name.</param>
    /// <param name="folderName3">The sub-sub-sub-folder name.</param>
    /// <returns>The directory information instance.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static DirectoryInfo GetDirectoryInfo(string folder, string folderName = null, string folderName2 = null, string folderName3 = null)
    {
        var path = GetLocalPath(folder, folderName, folderName2, folderName3);
        return new DirectoryInfo(path);
    }

    /// <summary>
    /// Gets a directory information instance of the specific path.
    /// </summary>
    /// <param name="folder">The folder path.</param>
    /// <param name="folderName">The sub-folder name.</param>
    /// <param name="folderName2">The sub-sub-folder name.</param>
    /// <param name="folderName3">The sub-sub-sub-folder name.</param>
    /// <returns>The file information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetDirectoryInfo(string folder, string folderName = null, string folderName2 = null, string folderName3 = null)
    {
        var path = GetLocalPath(folder, folderName, folderName2, folderName3);
        if (string.IsNullOrWhiteSpace(path)) return null;
        try
        {
            return new DirectoryInfo(path);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets a file information instance of the specific path.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The file information instance.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static FileInfo GetFileInfo(string fileName)
    {
        var path = GetLocalPath(fileName);
        if (string.IsNullOrWhiteSpace(path)) throw ObjectConvert.ArgumentNull(nameof(fileName));
        path = Path.Combine(path, fileName);
        return new FileInfo(path);
    }

    /// <summary>
    /// Gets a file information instance of the specific path.
    /// </summary>
    /// <param name="folder">The folder path.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The file information instance.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static FileInfo GetFileInfo(string folder, string fileName)
    {
        var path = GetLocalPath(folder);
        if (string.IsNullOrWhiteSpace(path)) throw ObjectConvert.ArgumentNull(nameof(folder));
        if (string.IsNullOrWhiteSpace(fileName)) throw ObjectConvert.ArgumentNull(nameof(fileName));
        path = Path.Combine(path, fileName);
        return new FileInfo(path);
    }

    /// <summary>
    /// Tries to get a file information instance of the specific path.
    /// </summary>
    /// <param name="file">The file path.</param>
    /// <returns>A file info instance; or null, if accesses failed.</returns>
    public static FileInfo TryGetFileInfo(string file)
    {
        file = GetLocalPath(file);
        if (string.IsNullOrWhiteSpace(file)) return null;
        try
        {
            return new FileInfo(file);
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to get a file information instance of the specific path.
    /// </summary>
    /// <param name="folder">The folder path.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The file information instance; or null, if accesses failed.</returns>
    public static FileInfo TryGetFileInfo(string folder, string fileName)
    {
        var path = GetLocalPath(folder);
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(fileName)) return null;
        path = Path.Combine(path, fileName);
        return TryGetFileInfo(path);
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static FileInfo TryGetFileInfo(DirectoryInfo dir, string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;
        try
        {
            if (dir == null || !dir.Exists) return null;
            return dir.EnumerateFiles(fileName).FirstOrDefault(ele => fileName.Equals(ele.Name, StringComparison.OrdinalIgnoreCase));
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        try
        {
            return TryGetFileInfo(dir.FullName, fileName);
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static FileInfo TryGetFileInfo(DirectoryInfo dir, IEnumerable<string> folderNamePath, string fileName)
    {
        dir = TryGetSubDirectory(dir, folderNamePath);
        if (dir == null) return null;
        return TryGetFileInfo(dir, fileName);
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static FileInfo TryGetFileInfo(DirectoryInfo dir, ReadOnlySpan<string> folderNamePath, string fileName)
    {
        dir = TryGetSubDirectory(dir, folderNamePath);
        if (dir == null) return null;
        return TryGetFileInfo(dir, fileName);
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderName">The folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(DirectoryInfo dir, string folderName)
    {
        if (string.IsNullOrEmpty(folderName)) return dir;
        try
        {
            if (dir == null || !dir.Exists) return null;
            return dir.EnumerateDirectories(folderName).FirstOrDefault(ele => folderName.Equals(ele.Name, StringComparison.OrdinalIgnoreCase));
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        try
        {
            return TryGetDirectoryInfo(dir.FullName, folderName);
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderName">The folder name.</param>
    /// <param name="folderName2">The folder name 2.</param>
    /// <param name="folderName3">The folder name 3.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(DirectoryInfo dir, string folderName, string folderName2, string folderName3 = null)
    {
        var folder = TryGetSubDirectory(dir, folderName);
        if (folder == null || !folder.Exists) return null;
        folder = TryGetSubDirectory(folder, folderName2);
        return string.IsNullOrEmpty(folderName3) ? folder : TryGetSubDirectory(folder, folderName3);
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(DirectoryInfo dir, IEnumerable<string> folderNamePath)
    {
        if (folderNamePath == null) return dir;
        foreach (var folder in folderNamePath)
        {
            dir = TryGetSubDirectory(dir, folder);
            if (dir == null) return null;
        }

        return dir;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(DirectoryInfo dir, ReadOnlySpan<string> folderNamePath)
    {
        foreach (var folder in folderNamePath)
        {
            dir = TryGetSubDirectory(dir, folder);
            if (dir == null) return null;
        }

        return dir;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="createIfNonExist">true if create one if the directory does not exist; otherwise, false.</param>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderName">The folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(bool createIfNonExist, DirectoryInfo dir, string folderName)
    {
        var d = TryGetSubDirectory(dir, folderName);
        try
        {
            if (!dir.Exists) dir.Create();
            if (d == null)
            {
                if (!createIfNonExist) return null;
                d.CreateSubdirectory(folderName);
            }
            else if (!d.Exists)
            {
                d.Create();
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        return d;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="createIfNonExist">true if create one if the directory does not exist; otherwise, false.</param>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderName">The folder name.</param>
    /// <param name="folderName2">The folder name 2.</param>
    /// <param name="folderName3">The folder name 3.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(bool createIfNonExist, DirectoryInfo dir, string folderName, string folderName2, string folderName3 = null)
    {
        var folder = TryGetSubDirectory(createIfNonExist, dir, folderName);
        if (folder == null || !folder.Exists) return null;
        folder = TryGetSubDirectory(createIfNonExist, folder, folderName2);
        return string.IsNullOrEmpty(folderName3) ? folder : TryGetSubDirectory(createIfNonExist, folder, folderName3);
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="createIfNonExist">true if create one if the directory does not exist; otherwise, false.</param>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(bool createIfNonExist, DirectoryInfo dir, IEnumerable<string> folderNamePath)
    {
        if (folderNamePath == null) return dir;
        foreach (var folder in folderNamePath)
        {
            dir = TryGetSubDirectory(createIfNonExist, dir, folder);
            if (dir == null) return null;
        }

        return dir;
    }

    /// <summary>
    /// Gets the directory information instance of the sub-folder.
    /// </summary>
    /// <param name="createIfNonExist">true if create one if the directory does not exist; otherwise, false.</param>
    /// <param name="dir">The directory information.</param>
    /// <param name="folderNamePath">The path of sub-directory folder name.</param>
    /// <returns>The directory information instance; or null, if accesses failed.</returns>
    public static DirectoryInfo TryGetSubDirectory(bool createIfNonExist, DirectoryInfo dir, ReadOnlySpan<string> folderNamePath)
    {
        foreach (var folder in folderNamePath)
        {
            dir = TryGetSubDirectory(createIfNonExist, dir, folder);
            if (dir == null) return null;
        }

        return dir;
    }

    /// <summary>
    /// Gets the relative path.
    /// </summary>
    /// <param name="dir">The directory to get.</param>
    /// <param name="root">The root directory.</param>
    /// <returns>The relative path.</returns>
    public static string GetRelativePath(DirectoryInfo dir, DirectoryInfo root)
    {
        string path = null;
        while (dir != null && dir != root && dir.FullName != root.FullName)
        {
            path = path == null ? dir.Name : Path.Combine(dir.Name, path);
            if (dir == dir.Parent) break;
            dir = dir.Parent;
        }

        return path;
    }

    /// <summary>
    /// Gets the relative path.
    /// </summary>
    /// <param name="file">The file to get.</param>
    /// <param name="root">The root directory.</param>
    /// <returns>The relative path.</returns>
    public static string GetRelativePath(FileInfo file, DirectoryInfo root)
    {
        var parent = file.Directory;
        var path = file.Name;
        while (parent != null && parent != root && parent.FullName != root.FullName)
        {
            path = Path.Combine(parent.Name, path);
            if (parent == parent.Parent) break;
            parent = parent.Parent;
        }

        return path;
    }
}
