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
/// The reference information of file.
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
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateDirectories();
        if (col == null) return new List<LocalDirectoryReferenceInfo>();
        if (!showHidden) col = col.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden));
        if (predicate != null) col = col.Where(predicate);
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
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateFiles();
        if (col == null) return new List<LocalFileReferenceInfo>();
        if (!showHidden) col = col.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden));
        if (predicate != null) col = col.Where(predicate);
        return col.Select(ele => new LocalFileReferenceInfo(ele, this));
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
        if (dir == null)
            return Task.FromResult<IReadOnlyList<IDirectoryReferenceInfo>>(new List<IDirectoryReferenceInfo>());
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateDirectories()?.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden))?.Select(ele => new LocalDirectoryReferenceInfo(ele, parent) as IDirectoryReferenceInfo)?.ToList() ?? new List<IDirectoryReferenceInfo>();
        return Task.FromResult<IReadOnlyList<IDirectoryReferenceInfo>>(col);
    }

    internal static Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync(DirectoryInfo dir, LocalDirectoryReferenceInfo parent)
    {
        if (dir == null)
            return Task.FromResult<IReadOnlyList<IFileReferenceInfo>>(new List<IFileReferenceInfo>());
        if (parent == null) parent = new LocalDirectoryReferenceInfo(dir);
        var col = dir.EnumerateFiles()?.Where(ele => !ele.Attributes.HasFlag(FileAttributes.Hidden))?.Select(ele => new LocalFileReferenceInfo(ele, parent) as IFileReferenceInfo)?.ToList() ?? new List<IFileReferenceInfo>();
        return Task.FromResult<IReadOnlyList<IFileReferenceInfo>>(col);
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
    }

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
    /// Gets the parent.
    /// </summary>
    public new LocalDirectoryReferenceInfo GetParent()
        => base.GetParent() as LocalDirectoryReferenceInfo;

    /// <summary>
    /// Gets the parent.
    /// </summary>
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
