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
public interface IFileSystemReferenceInfo
{
    /// <summary>
    /// Gets the file name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the item exists.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets the file size.
    /// </summary>
    DateTime LastModification { get; }

    /// <summary>
    /// Gets the instance source for reference.
    /// </summary>
    object Source { get; }
}

/// <summary>
/// The reference information of file.
/// </summary>
public interface IFileReferenceInfo : IFileSystemReferenceInfo
{
    /// <summary>
    /// Gets or sets the file size.
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <returns>The parent reference information instance.</returns>
    IFileContainerReferenceInfo GetParent();
}

/// <summary>
/// The reference information of file container.
/// </summary>
public interface IFileContainerReferenceInfo : IFileSystemReferenceInfo
{
}

/// <summary>
/// The reference information of directory.
/// </summary>
public interface IDirectoryReferenceInfo : IFileContainerReferenceInfo
{
}

/// <summary>
/// The reference information of directory with host.
/// </summary>
public interface IDirectoryHostReferenceInfo : IDirectoryReferenceInfo
{
    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    Task<IReadOnlyList<IDirectoryReferenceInfo>> GetDirectoriesAsync();

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync();

    /// <summary>
    /// Gets the parent.
    /// </summary>
    Task<IFileContainerReferenceInfo> GetParentAsync();
}

/// <summary>
/// The reference information of file.
/// </summary>
public class BaseFileSystemReferenceInfo : IFileSystemReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the BaseFileSystemReferenceInfo class.
    /// </summary>
    protected BaseFileSystemReferenceInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileSystemReferenceInfo class.
    /// </summary>
    /// <param name="source">The instance source for reference.</param>
    protected BaseFileSystemReferenceInfo(object source)
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileSystemReferenceInfo class.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="lastModification">The last modification time.</param>
    /// <param name="exists">true if exists; otherwise, false.</param>
    /// <param name="source">The instance source for reference.</param>
    public BaseFileSystemReferenceInfo(string name, DateTime lastModification, object source = null, bool exists = true)
    {
        Name = name;
        LastModification = lastModification;
        Source = source;
        Exists = exists;
    }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the item exists.
    /// </summary>
    public bool Exists { get; protected set; }

    /// <summary>
    /// Gets the file size.
    /// </summary>
    public DateTime LastModification { get; protected set; }

    /// <summary>
    /// Gets the instance source for reference.
    /// </summary>
    public object Source { get; internal set; }
}

/// <summary>
/// The reference information of directory.
/// </summary>
public class BaseDirectoryReferenceInfo : BaseFileSystemReferenceInfo, IDirectoryReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    protected BaseDirectoryReferenceInfo() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="source">The instance source for reference.</param>
    protected BaseDirectoryReferenceInfo(object source) : base(source)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="lastModification">The last modification time.</param>
    /// <param name="source">The instance source for reference.</param>
    /// <param name="exists">true if exists; otherwise, false.</param>
    public BaseDirectoryReferenceInfo(string name, DateTime lastModification, object source = null, bool exists = true)
        : base(name, lastModification, source, exists)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="directory">The directory item.</param>
    public BaseDirectoryReferenceInfo(DirectoryInfo directory) : base()
    {
        Source = directory;
        if (directory == null) return;
        if (!directory.Exists)
        {
            try
            {
                Name = directory.Name;
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
            catch (NullReferenceException)
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

            return;
        }

        Name = directory.Name;
        Exists = true;
        try
        {
            LastModification = directory.LastWriteTime;
            return;
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

        Exists = false;
    }
}

/// <summary>
/// The reference information of file.
/// </summary>
public class BaseFileReferenceInfo : BaseFileSystemReferenceInfo, IFileReferenceInfo
{
    private IFileContainerReferenceInfo parent;

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    protected BaseFileReferenceInfo() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="parent">The parent directory.</param>
    /// <param name="source">The instance source for reference.</param>
    protected BaseFileReferenceInfo(IFileContainerReferenceInfo parent, object source) : base(source)
    {
        SetParent(parent);
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="lastModification">The last modification time.</param>
    /// <param name="size">The file size.</param>
    /// <param name="parent">The parent directory.</param>
    /// <param name="source">The instance source for reference.</param>
    /// <param name="exists">true if exists; otherwise, false.</param>
    public BaseFileReferenceInfo(string name, DateTime lastModification, long size, BaseDirectoryReferenceInfo parent, object source = null, bool exists = true)
        : base(name, lastModification, source, exists)
    {
        Size = size;
        SetParent(parent);
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    public BaseFileReferenceInfo(FileInfo file, LocalDirectoryReferenceInfo parent = null) : base()
    {
        Source = file;
        if (file == null) return;
        if (!file.Exists)
        {
            try
            {
                Name = file.Name;
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
            catch (NullReferenceException)
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

            Exists = false;
            return;
        }

        Name = file.Name;
        try
        {
            LastModification = file.LastWriteTime;
            Size = file.Length;
        }
        catch (IOException)
        {
            return;
        }
        catch (NotSupportedException)
        {
            return;
        }
        catch (InvalidOperationException)
        {
            return;
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }
        catch (SecurityException)
        {
            return;
        }
        catch (ExternalException)
        {
            return;
        }

        Exists = true;
        if (parent != null)
        {
            SetParent(parent);
            return;
        }

        try
        {
            var dir = file.Directory;
            if (dir == null || !dir.Exists) return;
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
    }

    /// <summary>
    /// Gets the file size.
    /// </summary>
    public long Size { get; protected set; }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <return>The parent.</return>
    public BaseDirectoryReferenceInfo GetParent()
        => GetParentInfo() as BaseDirectoryReferenceInfo;

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="info">The parent.</param>
    protected virtual void SetParent(IFileContainerReferenceInfo info)
        => parent = info;

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <returns>The parent reference information instance.</returns>
    protected virtual IFileContainerReferenceInfo GetParentInfo()
        => parent;

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <returns>The parent reference information instance.</returns>
    IFileContainerReferenceInfo IFileReferenceInfo.GetParent()
        => GetParentInfo();
}

/// <summary>
/// The reference information of file.
/// </summary>
/// <typeparam name="T">The type of instance source.</typeparam>
public class BaseDirectoryReferenceInfo<T> : BaseDirectoryReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    protected BaseDirectoryReferenceInfo() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="source">The instance source for reference.</param>
    protected BaseDirectoryReferenceInfo(T source) : base(source)
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of the BaseDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="lastModification">The last modification time.</param>
    /// <param name="source">The instance source for reference.</param>
    /// <param name="exists">true if exists; otherwise, false.</param>
    public BaseDirectoryReferenceInfo(string name, DateTime lastModification, T source = default, bool exists = true)
        : base(name, lastModification, source, exists)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="directory">The directory item.</param>
    internal BaseDirectoryReferenceInfo(DirectoryInfo directory) : base(directory)
    {
        if (directory is T s) Source = s;
    }

    /// <summary>
    /// Gets the directory information instance.
    /// </summary>
    public new T Source { get; private set; }
}

/// <summary>
/// The reference information of file.
/// </summary>
/// <typeparam name="T">The type of instance source.</typeparam>
public class BaseFileReferenceInfo<T> : BaseFileReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    protected BaseFileReferenceInfo() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="parent">The parent directory.</param>
    /// <param name="source">The instance source for reference.</param>
    protected BaseFileReferenceInfo(IFileContainerReferenceInfo parent, T source) : base(parent, source)
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="lastModification">The last modification time.</param>
    /// <param name="size">The file size.</param>
    /// <param name="parent">The parent directory.</param>
    /// <param name="source">The instance source for reference.</param>
    /// <param name="exists">true if exists; otherwise, false.</param>
    public BaseFileReferenceInfo(string name, DateTime lastModification, long size, BaseDirectoryReferenceInfo parent, T source = default, bool exists = true)
        : base(name, lastModification, size, parent, source, exists)
    {
        Source = source;
    }

    /// <summary>
    /// Initializes a new instance of the BaseFileReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    internal BaseFileReferenceInfo(FileInfo file, LocalDirectoryReferenceInfo parent = null) : base(file, parent)
    {
        if (file is T s) Source = s;
    }

    /// <summary>
    /// Gets the directory information instance.
    /// </summary>
    public new T Source { get; private set; }
}
