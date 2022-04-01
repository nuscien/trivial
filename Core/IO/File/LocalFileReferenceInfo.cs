using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.IO;

/// <summary>
/// The reference information of directory.
/// </summary>
public class LocalDirectoryReferenceInfo : BaseDirectoryReferenceInfo<DirectoryInfo>, IDirectoryHostReferenceInfo
{
    IFileContainerReferenceInfo parent;

    /// <summary>
    /// Initializes a new instance of the LocalDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="directory">The directory instance.</param>
    /// <param name="parent">The parent folder.</param>
    public LocalDirectoryReferenceInfo(DirectoryInfo directory, LocalDirectoryReferenceInfo parent = null) : base(directory)
    {
        this.parent = parent;
        if (directory == null) return;
        try
        {
            Creation = directory.CreationTime;
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }
    }

    /// <summary>
    /// Initializes a new instance of the LocalDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="directory">The directory instance.</param>
    /// <param name="parent">The parent folder.</param>
    public LocalDirectoryReferenceInfo(DirectoryInfo directory, LocalPackageFileReferenceInfo parent) : base(directory)
    {
        this.parent = parent;
    }

    /// <summary>
    /// Gets the date created.
    /// </summary>
    public DateTime Creation { get; private set; }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public IEnumerable<LocalDirectoryReferenceInfo> GetDirectories()
        => GetDirectories(false, null);

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="showHidden">true if show hidden; otherwise, false.</param>
    /// <param name="predicate">An optional function to test each element for a condition.</param>
    /// <returns>The directory collection.</returns>
    public IEnumerable<LocalDirectoryReferenceInfo> GetDirectories(bool showHidden, Func<DirectoryInfo, bool> predicate = null)
    {
        var dir = Source;
        if (dir == null) return new List<LocalDirectoryReferenceInfo>();
        var col = dir.EnumerateDirectories();
        if (col == null) return new List<LocalDirectoryReferenceInfo>();
        if (!showHidden) col = col.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden));
        if (predicate != null) col = col.Where(predicate);
        return col.Select(ele => new LocalDirectoryReferenceInfo(ele, this));
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <returns>The directory collection.</returns>
    public IEnumerable<LocalDirectoryReferenceInfo> GetDirectories(string searchPattern)
    {
        var dir = Source;
        if (dir == null) return new List<LocalDirectoryReferenceInfo>();
        var col = string.IsNullOrEmpty(searchPattern) ? dir.EnumerateDirectories() : dir.EnumerateDirectories(searchPattern);
        if (col == null) return new List<LocalDirectoryReferenceInfo>();
        return col.Select(ele => new LocalDirectoryReferenceInfo(ele, this));
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public IEnumerable<LocalFileReferenceInfo> GetFiles()
        => GetFiles(false, null);

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="showHidden">true if show hidden; otherwise, false.</param>
    /// <param name="predicate">An optional function to test each element for a condition.</param>
    /// <returns>The file collection.</returns>
    public IEnumerable<LocalFileReferenceInfo> GetFiles(bool showHidden, Func<FileInfo, bool> predicate = null)
    {
        var dir = Source;
        if (dir == null) return new List<LocalFileReferenceInfo>();
        var col = dir.EnumerateFiles();
        if (col == null) return new List<LocalFileReferenceInfo>();
        if (!showHidden) col = col.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden));
        if (predicate != null) col = col.Where(predicate);
        return col.Select(ele => CreateFileReferenceInfo(ele, this));
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <returns>The directory collection.</returns>
    public IEnumerable<LocalFileReferenceInfo> GetFiles(string searchPattern)
    {
        var dir = Source;
        if (dir == null) return new List<LocalFileReferenceInfo>();
        var col = string.IsNullOrEmpty(searchPattern) ? dir.EnumerateFiles() : dir.EnumerateFiles(searchPattern);
        if (col == null) return new List<LocalFileReferenceInfo>();
        return col.Select(ele => CreateFileReferenceInfo(ele, this));
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public Task<IReadOnlyList<LocalDirectoryReferenceInfo>> GetDirectoriesAsync()
        => GetReadOnlyListAsync(GetDirectories(false, null));

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="showHidden">true if show hidden; otherwise, false.</param>
    /// <param name="predicate">An optional function to test each element for a condition.</param>
    /// <returns>The directory collection.</returns>
    public Task<IReadOnlyList<LocalDirectoryReferenceInfo>> GetDirectoriesAsync(bool showHidden, Func<DirectoryInfo, bool> predicate = null)
        => GetReadOnlyListAsync(GetDirectories(showHidden, predicate));

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <returns>The directory collection.</returns>
    public Task<IReadOnlyList<LocalDirectoryReferenceInfo>> GetDirectoriesAsync(string searchPattern)
        => GetReadOnlyListAsync(GetDirectories(searchPattern));

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public Task<IReadOnlyList<LocalFileReferenceInfo>> GetFilesAsync()
        => GetReadOnlyListAsync(GetFiles(false, null));

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="showHidden">true if show hidden; otherwise, false.</param>
    /// <param name="predicate">An optional function to test each element for a condition.</param>
    /// <returns>The file collection.</returns>
    public Task<IReadOnlyList<LocalFileReferenceInfo>> GetFilesAsync(bool showHidden, Func<FileInfo, bool> predicate = null)
        => GetReadOnlyListAsync(GetFiles(showHidden, predicate));

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <returns>The file collection.</returns>
    public Task<IReadOnlyList<LocalFileReferenceInfo>> GetFilesAsync(string searchPattern)
        => GetReadOnlyListAsync(GetFiles(searchPattern));

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    Task<IReadOnlyList<IFileReferenceInfo>> IDirectoryHostReferenceInfo.GetFilesAsync()
        => GetFilesAsync(Source, this);

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    Task<IReadOnlyList<IDirectoryReferenceInfo>> IDirectoryHostReferenceInfo.GetDirectoriesAsync()
        => GetDirectoriesAsync(Source, this);

    /// <summary>
    /// Refreshes the state of the object.
    /// </summary>
    public void Refresh()
    {
        var source = Source;
        if (source == null) return;
        try
        {
            source.Refresh();
            Name = source.Name;
            Creation = source.CreationTime;
            LastModification = source.LastWriteTime;
            if (GetParent().Source != Source.Parent)
            {
                parent = null;
                GetParent();
            }
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public LocalDirectoryReferenceInfo GetParent()
        => GetParentInternal() as LocalDirectoryReferenceInfo;

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Task<LocalDirectoryReferenceInfo> GetParentAsync()
        => Task.FromResult(GetParentInternal() as LocalDirectoryReferenceInfo);

    /// <summary>
    /// Gets the parent.
    /// </summary>
    Task<IFileContainerReferenceInfo> IDirectoryHostReferenceInfo.GetParentAsync()
        => Task.FromResult(GetParentInternal());

    /// <summary>
    /// Gets the parent.
    /// </summary>
    private IFileContainerReferenceInfo GetParentInternal()
    {
        if (parent != null) return parent;
        try
        {
            var dir = Source.Parent;
            if (dir == null || !dir.Exists) return null;
            parent = new LocalDirectoryReferenceInfo(dir);
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }

        return parent;
    }

    private static Task<IReadOnlyList<T>> GetReadOnlyListAsync<T>(IEnumerable<T> col)
        => Task.FromResult<IReadOnlyList<T>>(col.ToList());

    internal static Task<IReadOnlyList<IDirectoryReferenceInfo>> GetDirectoriesAsync(DirectoryInfo dir, LocalDirectoryReferenceInfo parent)
    {
        if (dir == null) return Task.FromResult<IReadOnlyList<IDirectoryReferenceInfo>>(new List<IDirectoryReferenceInfo>());
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateDirectories()?.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden))?.Select(ele => new LocalDirectoryReferenceInfo(ele, parent) as IDirectoryReferenceInfo)?.ToList() ?? new List<IDirectoryReferenceInfo>();
        return Task.FromResult<IReadOnlyList<IDirectoryReferenceInfo>>(col);
    }

    internal static Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync(DirectoryInfo dir, LocalDirectoryReferenceInfo parent)
    {
        if (dir == null) return Task.FromResult<IReadOnlyList<IFileReferenceInfo>>(new List<IFileReferenceInfo>());
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateFiles()?.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden))?.Select(ele => CreateFileReferenceInfo(ele, parent) as IFileReferenceInfo)?.ToList() ?? new List<IFileReferenceInfo>();
        return Task.FromResult<IReadOnlyList<IFileReferenceInfo>>(col);
    }

    private static LocalFileReferenceInfo CreateFileReferenceInfo(FileInfo ele, LocalDirectoryReferenceInfo parent)
    {
#if !NETFRAMEWORK
        if (ele.Extension?.Equals(".zip", StringComparison.OrdinalIgnoreCase) == true)
            return new ZipFileReferenceInfo(ele, parent);
        else
#endif
            return new LocalFileReferenceInfo(ele, parent);
    }
}

/// <summary>
/// The reference information of file.
/// </summary>
public class LocalFileReferenceInfo: BaseFileReferenceInfo<FileInfo>
{
    /// <summary>
    /// Initializes a new instance of the LocalFileReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    public LocalFileReferenceInfo(FileInfo file, LocalDirectoryReferenceInfo parent = null) : base(file, parent)
    {
        if (file == null) return;
        try
        {
            Extension = file.Extension;
            Creation = file.CreationTime;
            Attributes = file.Attributes;
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }
    }

    /// <summary>
    /// Gets the extension name.
    /// </summary>
    public string Extension { get; private set; }

    /// <summary>
    /// Gets the date created.
    /// </summary>
    public DateTime Creation { get; private set; }

    /// <summary>
    /// Gets the file attributes.
    /// </summary>
    public FileAttributes Attributes { get; private set; }

    /// <summary>
    /// Refreshes the state of the object.
    /// </summary>
    public void Refresh()
    {
        var source = Source;
        if (source == null) return;
        try
        {
            source.Refresh();
            Name = source.Name;
            Extension = source.Extension;
            Creation = source.CreationTime;
            LastModification = source.LastWriteTime;
            Size = source.Length;
            if (GetParent().Source != Source.Directory)
            {
                SetParent(null);
                GetParentInfo();
            }
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }
    }

    /// <summary>
    /// Opens a file in the specified mode and other options.
    /// </summary>
    /// <param name="mode">A constant specifying the mode (for example, Open or Append) in which to open the file.</param>
    /// <param name="access">A constant specifying whether to open the file with Read, Write, or ReadWrite file access.</param>
    /// <param name="share">A constant specifying the type of access other file stream objects have to this file.</param>
    /// <returns>A file opened in the specified mode, access options and shared options.</returns>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="FileNotFoundException">The file is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">The file is read-only.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="IOException">The file is already open.</exception>
    public FileStream Open(FileMode mode, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None)
    {
        var file = Source;
        if (file == null) throw new FileNotFoundException("The file does not specify.");
        return file.Open(mode, access, share);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Opens a file in the specified mode and other options.
    /// </summary>
    /// <param name="options">An object that describes optional file stream parameters to use.</param>
    /// <returns>A file opened in the specified mode, access options and shared options.</returns>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="FileNotFoundException">The file is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">The file is read-only.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="IOException">The file is already open.</exception>
    public FileStream Open(FileStreamOptions options)
    {
        var file = Source;
        if (file == null) throw new FileNotFoundException("The file does not specify.");
        return options == null ? file.Open(FileMode.OpenOrCreate) : file.Open(options);
    }
#endif

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <return>The parent.</return>
    public new LocalDirectoryReferenceInfo GetParent()
        => base.GetParent() as LocalDirectoryReferenceInfo;

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <return>The parent.</return>
    protected override IFileContainerReferenceInfo GetParentInfo()
    {
        var info = base.GetParentInfo();
        if (info != null) return info;
        try
        {
            var dir = Source.Directory;
            if (dir == null || !dir.Exists) return null;
            SetParent(new LocalDirectoryReferenceInfo(dir));
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ExternalException)
        {
        }

        return base.GetParentInfo();
    }
}

/// <summary>
/// The reference information of package (such as compressed) file.
/// </summary>
public class LocalPackageFileReferenceInfo : LocalFileReferenceInfo, IFileContainerReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the LocalPackageFileReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    public LocalPackageFileReferenceInfo(FileInfo file, LocalDirectoryReferenceInfo parent = null) : base(file, parent)
    {
    }
}
