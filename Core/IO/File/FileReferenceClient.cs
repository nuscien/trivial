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
/// The client for loading file reference information.
/// </summary>
public interface IFileReferenceClient
{
    /// <summary>
    /// Tests if supports the directory reference.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>true if supports; otherwise, false.</returns>
    bool Test(IFileContainerReferenceInfo directory);

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The file collection.</returns>
    Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync(IFileContainerReferenceInfo directory);

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The directory collection.</returns>
    Task<IReadOnlyList<IDirectoryReferenceInfo>> GetDirectoriesAsync(IFileContainerReferenceInfo directory);

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <param name="directory">The directory to get parent.</param>
    /// <return>The parent information; or null, if no parent.</return>
    Task<IFileContainerReferenceInfo> GetParentAsync(IFileContainerReferenceInfo directory);
}

/// <summary>
/// The client for loading file reference information.
/// </summary>
public abstract class BaseFileReferenceClient<T> : IFileReferenceClient where T : IDirectoryReferenceInfo
{
    /// <summary>
    /// Tests if supports the directory reference.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>true if supports; otherwise, false.</returns>
    public abstract bool Test(T directory);

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The file collection.</returns>
    public abstract Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync(T directory);

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The directory collection.</returns>
    public abstract Task<IReadOnlyList<T>> GetDirectoriesAsync(T directory);

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <param name="directory">The directory to get parent.</param>
    /// <return>The parent information; or null, if no parent.</return>
    public abstract Task<T> GetParentAsync(T directory);

    /// <summary>
    /// Tests if supports the directory reference.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>true if supports; otherwise, false.</returns>
    bool IFileReferenceClient.Test(IFileContainerReferenceInfo directory)
    {
        if (directory is not T info) return false;
        return Test(info);
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The file collection.</returns>
    async Task<IReadOnlyList<IFileReferenceInfo>> IFileReferenceClient.GetFilesAsync(IFileContainerReferenceInfo directory)
    {
        if (directory is not T info) return new List<IFileReferenceInfo>();
        return await GetFilesAsync(info) ?? new List<IFileReferenceInfo>();
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The directory collection.</returns>
    async Task<IReadOnlyList<IDirectoryReferenceInfo>> IFileReferenceClient.GetDirectoriesAsync(IFileContainerReferenceInfo directory)
    {
        var col = new List<IDirectoryReferenceInfo>();
        if (directory is not T info) return col;
        var list = await GetDirectoriesAsync(info);
        if (list == null) return col;
        foreach (var item in list)
        {
            col.Add(item);
        }

        return col;
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <param name="directory">The directory to get parent.</param>
    /// <return>The parent information; or null, if no parent.</return>
    async Task<IFileContainerReferenceInfo> IFileReferenceClient.GetParentAsync(IFileContainerReferenceInfo directory)
    {
        if (directory is not T info) return null;
        return await GetParentAsync(info);
    }
}

/// <summary>
/// The file reference client factory.
/// </summary>
[Guid("FC26FC1D-1ADE-4B5A-82D2-A94093D23D96")]
public class FileReferenceClientFactory : IFileReferenceClient
{
    private readonly Dictionary<Type, IFileReferenceClient> handlers = new();

    /// <summary>
    /// The singleton.
    /// </summary>
    public static readonly FileReferenceClientFactory Instance = new();

    /// <summary>
    /// Registers the client handler.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    /// <param name="client">The client.</param>
    public void Register<T>(BaseFileReferenceClient<T> client) where T : IDirectoryReferenceInfo
        => Register(typeof(T), client);

    /// <summary>
    /// Registers the client handler.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    /// <param name="client">The client.</param>
    public void Register<T>(IFileReferenceClient client) where T : IDirectoryReferenceInfo
        => Register(typeof(T), client);

    /// <summary>
    /// Registers the client handler.
    /// </summary>
    /// <param name="type">The type to register.</param>
    /// <param name="client">The client.</param>
    public void Register(Type type, IFileReferenceClient client)
    {
        if (client == null) handlers.Remove(type);
        else handlers[type] = client;
    }

    /// <summary>
    /// Removes the client handler.
    /// </summary>
    /// <typeparam name="T">The type of client.</typeparam>
    public void Remove<T>()
        => handlers.Remove(typeof(T));

    /// <summary>
    /// Removes the client handler.
    /// </summary>
    /// <param name="type">The type to unregister.</param>
    public void Remove(Type type)
        => handlers.Remove(type);

    /// <summary>
    /// Tests if supports the directory reference.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>true if supports; otherwise, false.</returns>
    public bool Test(IFileContainerReferenceInfo directory)
    {
        if (directory == null) return false;
        if (directory is IDirectoryHostReferenceInfo) return true;
        var type = directory.GetType();
        return handlers.ContainsKey(type) || directory.Source is DirectoryInfo;
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The file collection.</returns>
    public async Task<IReadOnlyList<IFileReferenceInfo>> GetFilesAsync(IFileContainerReferenceInfo directory)
    {
        if (directory == null) return new List<IFileReferenceInfo>();
        var type = directory.GetType();
        if (handlers.TryGetValue(type, out var h) && h.Test(directory))
            return await h.GetFilesAsync(directory) ?? new List<IFileReferenceInfo>();
        if (directory is IDirectoryHostReferenceInfo dir) return await dir.GetFilesAsync() ?? new List<IFileReferenceInfo>();
        return await LocalDirectoryReferenceInfo.GetFilesAsync(directory.Source as DirectoryInfo, null);
    }

    /// <summary>
    /// Lists all sub-directories.
    /// </summary>
    /// <param name="directory">The directory to load sub-directories or files.</param>
    /// <returns>The directory collection.</returns>
    public async Task<IReadOnlyList<IDirectoryReferenceInfo>> GetDirectoriesAsync(IFileContainerReferenceInfo directory)
    {
        if (directory == null) return new List<IDirectoryReferenceInfo>();
        var type = directory.GetType();
        if (handlers.TryGetValue(type, out var h) && h.Test(directory))
            return await h.GetDirectoriesAsync(directory) ?? new List<IDirectoryReferenceInfo>();
        if (directory is IDirectoryHostReferenceInfo dir) return await dir.GetDirectoriesAsync() ?? new List<IDirectoryReferenceInfo>();
        return await LocalDirectoryReferenceInfo.GetDirectoriesAsync(directory.Source as DirectoryInfo, null);
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <param name="directory">The directory to get parent.</param>
    /// <return>The parent information; or null, if no parent.</return>
    public async Task<IFileContainerReferenceInfo> GetParentAsync(IFileContainerReferenceInfo directory)
    {
        if (directory == null) return null;
        var type = directory.GetType();
        if (handlers.TryGetValue(type, out var h) && h.Test(directory))
            return await h.GetParentAsync(directory);
        if (directory is IDirectoryHostReferenceInfo dir) return await dir.GetParentAsync();
        if (directory.Source is not DirectoryInfo info) return null;
        return new LocalDirectoryReferenceInfo(info);
    }
}
