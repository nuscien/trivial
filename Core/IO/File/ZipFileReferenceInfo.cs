using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.IO;

#if !NETFRAMEWORK

/// <summary>
/// The reference information of zip file.
/// </summary>
public class ZipFileReferenceInfo : LocalPackageFileReferenceInfo, IDirectoryHostReferenceInfo
{
    private readonly object locker = new();
    private readonly List<ZipArchiveEntryReferenceInfo> files = new();
    private readonly Dictionary<string, ZipArchiveDirectoryReferenceInfo> dirs = new();

    /// <summary>
    /// Initializes a new instance of the ZipFileReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    public ZipFileReferenceInfo(FileInfo file, LocalDirectoryReferenceInfo parent = null) : base(file, parent)
    {
    }

    /// <summary>
    /// Gets the specific sub-directory.
    /// </summary>
    /// <param name="path">The relative path.</param>
    /// <returns>The directory.</returns>
    public ZipArchiveDirectoryReferenceInfo GetDirectory(string path)
    {
        path = path?.Trim() ?? string.Empty;
        if (path.EndsWith('\\')) path = path.Substring(path.Length - 1).TrimEnd();
        if (string.IsNullOrEmpty(path)) return null;
        return dirs.TryGetValue(path, out var info) ? info : null;
    }

    /// <summary>
    /// Gets a value indiciating whether the instance has initialized.
    /// </summary>
    public bool HasInitialized { get; private set; }

    /// <summary>
    /// Gets the error message if has.
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public IReadOnlyList<ZipArchiveDirectoryReferenceInfo> GetDirectories()
    {
        EnsureInitialization();
        var col = new List<ZipArchiveDirectoryReferenceInfo>();
        foreach (var dir in dirs)
        {
            if (!dir.Key.Contains('\\')) col.Add(dir.Value);
        }

        return col;
    }

    /// <summary>
    /// Lists all sub-directories of the specific relative path.
    /// </summary>
    /// <param name="path">The relative path to list its sub-directories.</param>
    /// <returns>The directory collection.</returns>
    public IReadOnlyList<ZipArchiveDirectoryReferenceInfo> GetSubDirectories(string path)
    {
        EnsureInitialization();
        path = path?.Trim() ?? string.Empty;
        if (!path.EndsWith('\\')) path = string.Concat(path, '\\');
        if (path.Length < 2) return GetDirectories();
        var col = new List<ZipArchiveDirectoryReferenceInfo>();
        foreach (var dir in dirs)
        {
            if (!dir.Key.StartsWith(path)) continue;
            var name = dir.Key.Substring(path.Length);
            if (!name.Contains('\\')) col.Add(dir.Value);
        }

        return col;
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public IReadOnlyList<ZipArchiveEntryReferenceInfo> GetFiles()
    {
        EnsureInitialization();
        return files.AsReadOnly();
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public Task<IReadOnlyList<ZipArchiveDirectoryReferenceInfo>> GetDirectoriesAsync()
        => Task.FromResult(GetDirectories());

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public Task<IReadOnlyList<ZipArchiveEntryReferenceInfo>> GetFilesAsync()
        => Task.FromResult(GetFiles());

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    async Task<IReadOnlyList<IDirectoryReferenceInfo>> IDirectoryHostReferenceInfo.GetDirectoriesAsync()
    {
        var col = await GetDirectoriesAsync();
        return col;
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    async Task<IReadOnlyList<IFileReferenceInfo>> IDirectoryHostReferenceInfo.GetFilesAsync()
    {
        var col = await GetFilesAsync();
        return col;
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <returns>The parent.</returns>
    public Task<LocalDirectoryReferenceInfo> GetParentAsync()
        => Task.FromResult(GetParent());

    Task<IFileContainerReferenceInfo> IDirectoryHostReferenceInfo.GetParentAsync()
        => Task.FromResult<IFileContainerReferenceInfo>(GetParent());

    /// <summary>
    /// Ensures the item is initialized.
    /// </summary>
    public void EnsureInitialization()
    {
        if (HasInitialized) return;
        lock (locker)
        {
            if (HasInitialized) return;
            var file = Source;
            try
            {
                if (file == null || !file.Exists) return;
                using var stream = file.OpenRead();
                using var zip = new ZipArchive(stream);
                var col = zip.Entries.ToList();
                foreach (var entry in col)
                {
                    var path = entry.FullName.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (path.Length == 0) continue;
                    var dir = path.Take(path.Length - 1).ToList();
                    if (dir.Count == 0)
                    {
                        files.Add(new ZipArchiveEntryReferenceInfo(entry, this));
                        continue;
                    }

                    var dirName = StringExtensions.Join('\\', dir);
                    if (!dirs.TryGetValue(dirName, out var info))
                    {
                        info = new ZipArchiveDirectoryReferenceInfo(dir, this);
                        dirs[dirName] = info;
                    }

                    info.Add(entry);
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidDataException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (SecurityException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (IOException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NotSupportedException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (NullReferenceException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (ExternalException ex)
            {
                ErrorMessage = ex.Message;
            }

            HasInitialized = true;
        }
    }
}

/// <summary>
/// The reference information of zip archive entry.
/// </summary>
public class ZipArchiveDirectoryReferenceInfo : BaseDirectoryReferenceInfo<IReadOnlyList<string>>, IDirectoryHostReferenceInfo
{
    private readonly List<ZipArchiveEntryReferenceInfo> files = new();
    private readonly ZipFileReferenceInfo owner;

    /// <summary>
    /// Initializes a new instance of the ZipArchiveDirectoryReferenceInfo class.
    /// </summary>
    /// <param name="path">The relative directory path.</param>
    /// <param name="owner">The owner.</param>
    internal ZipArchiveDirectoryReferenceInfo(IReadOnlyList<string> path, ZipFileReferenceInfo owner) : base(path)
    {
        this.owner = owner;
        RelativePath = StringExtensions.Join('\\', path);
        Name = path.Count > 0 ? path[path.Count - 1] : string.Empty;
        LastModification = DateTime.Now;
        Exists = true;
    }

    /// <summary>
    /// Gets the relative path.
    /// </summary>
    public string RelativePath { get; private set; }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public IReadOnlyList<ZipArchiveDirectoryReferenceInfo> GetDirectories()
        => owner.GetSubDirectories(RelativePath);

    /// <summary>
    /// Lists all sub-directories of the specific relative path.
    /// </summary>
    /// <param name="path">The relative path to list its sub-directories.</param>
    /// <returns>The directory collection.</returns>
    public IReadOnlyList<ZipArchiveDirectoryReferenceInfo> GetSubDirectories(string path = null)
    {
        path = path?.Trim() ?? string.Empty;
        path = string.Concat(RelativePath, '\\', path);
        return owner.GetSubDirectories(path);
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public IReadOnlyList<ZipArchiveEntryReferenceInfo> GetFiles()
        => files.AsReadOnly();

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <return>The parent.</return>
    public IFileContainerReferenceInfo GetParent()
    {
        var path = Source;
        if (path == null || path.Count < 2) return owner;
        var parentPath = path.Take(path.Count - 1);
        var info = owner.GetDirectory(StringExtensions.Join('\\', parentPath));
        if (info == null) return owner;
        return info;
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    public Task<IReadOnlyList<ZipArchiveDirectoryReferenceInfo>> GetDirectoriesAsync()
        => Task.FromResult(GetDirectories());

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    public Task<IReadOnlyList<ZipArchiveEntryReferenceInfo>> GetFilesAsync()
        => Task.FromResult(GetFiles());

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <returns>The directory collection.</returns>
    async Task<IReadOnlyList<IDirectoryReferenceInfo>> IDirectoryHostReferenceInfo.GetDirectoriesAsync()
    {
        var col = await GetDirectoriesAsync();
        return col;
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>The file collection.</returns>
    async Task<IReadOnlyList<IFileReferenceInfo>> IDirectoryHostReferenceInfo.GetFilesAsync()
    {
        var col = await GetFilesAsync();
        return col;
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <return>The parent.</return>
    public Task<IFileContainerReferenceInfo> GetParentAsync()
        => Task.FromResult(GetParent());

    internal void Add(ZipArchiveEntry file)
    {
        if (file == null) return;
        var info = new ZipArchiveEntryReferenceInfo(file, this);
        files.Add(info);
        if (info.LastModification < LastModification) LastModification = info.LastModification;
    }
}

/// <summary>
/// The reference information of zip archive entry.
/// </summary>
public class ZipArchiveEntryReferenceInfo : BaseFileReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of the ZipArchiveEntryReferenceInfo class.
    /// </summary>
    /// <param name="file">The file item.</param>
    /// <param name="parent">The parent folder.</param>
    internal ZipArchiveEntryReferenceInfo(ZipArchiveEntry file, IFileContainerReferenceInfo parent) : base(parent, file)
    {
        Name = file.Name;
        Exists = true;
        LastModification = file.LastWriteTime.DateTime;
        Size = file.Length;
        CompressedSize = file.CompressedLength;
        Crc32 = file.Crc32;
    }

    /// <summary>
    /// Gets the 32-bit cyclic redundant check.
    /// </summary>
    public uint Crc32 { get; private set; }

    /// <summary>
    /// Gets the compressed size of the entry in the zip archive.
    /// </summary>
    public long CompressedSize { get; private set; }
}

#endif
