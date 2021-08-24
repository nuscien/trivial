using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;

using Microsoft.Win32;
using Trivial.Data;
using Trivial.IO;

namespace Trivial.Web
{
    /// <summary>
    /// The MIME constants.
    /// </summary>
    public static partial class MimeConstants
    {
#if NETFRAMEWORK
        /// <summary>
        /// The registry access.
        /// </summary>
        public static class Win32Registry
        {
            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="file">The file information.</param>
            /// <returns>The MIME content type.</returns>
            public static string ByFileExtension(FileInfo file)
                => ByFileExtension(file?.Extension, StreamMIME);

            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="file">The file information.</param>
            /// <param name="defaultMime">The default MIME content type.</param>
            /// <returns>The MIME content type.</returns>
            public static string ByFileExtension(FileInfo file, string defaultMime)
                => ByFileExtension(file?.Extension, defaultMime);

            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="fileExtension">The file extension.</param>
            /// <returns>The MIME content type.</returns>
            public static string ByFileExtension(string fileExtension)
                => ByFileExtension(fileExtension, StreamMIME);

            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="fileExtension">The file extension.</param>
            /// <param name="returnNullIfUnsupported">true if returns null if not supported; otherwise, false.</param>
            /// <returns>The MIME content type.</returns>
            public static string ByFileExtension(string fileExtension, bool returnNullIfUnsupported)
                => ByFileExtension(fileExtension, returnNullIfUnsupported ? null : StreamMIME);

            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="fileExtension">The file extension.</param>
            /// <param name="defaultMime">The default MIME content type.</param>
            /// <returns>The MIME content type.</returns>
            public static string ByFileExtension(string fileExtension, string defaultMime)
            {
                using var regKey = GetRegistryKeyByFileExtension(fileExtension, true);
                return Win32RegistryUtility.TryGetStringValue(regKey, "Content Type", defaultMime);
            }

            /// <summary>
            /// Registers file extension mapping.
            /// </summary>
            /// <param name="file">The file information.</param>
            /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
            /// <returns>true if registers succeeded; otherwise, false.</returns>
            public static bool RegisterFileExtensionMapping(FileInfo file, bool overrideIfExist = false)
                => RegisterFileExtensionMapping(file?.Extension, overrideIfExist);

            /// <summary>
            /// Registers file extension mapping.
            /// </summary>
            /// <param name="fileExtension">The file extension to register.</param>
            /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
            /// <returns>true if registers succeeded; otherwise, false.</returns>
            public static bool RegisterFileExtensionMapping(string fileExtension, bool overrideIfExist = false)
            {
                if (!overrideIfExist && FileExtensionMapping.ContainsKey(fileExtension)) return false;
                var r = GetRegistryKeyByFileExtension(fileExtension, false);
                string v;
                if (r != null)
                {
                    v = Win32RegistryUtility.TryGetStringValue(r, "Content Type");
                    if (string.IsNullOrWhiteSpace(v)) return false;
                    return FileExtensionMapping.Set(fileExtension, v, overrideIfExist);
                }

                if (fileExtension.IndexOf('/') < 1) return false;
                v = fileExtension;
                using var mime = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, "MIME\\Database\\Content Type\\" + fileExtension);
                fileExtension = Win32RegistryUtility.TryGetStringValue(mime, "Extension");
                return FileExtensionMapping.Set(fileExtension, v, overrideIfExist);
            }

            /// <summary>
            /// Gets the file association information.
            /// </summary>
            /// <param name="file">The file information.</param>
            /// <returns>The file association information; or null, if not found or failure.</returns>
            public static FileAssociationInfo GetFileAssociationInfo(FileInfo file)
                => GetFileAssociationInfo(file?.Extension);

            /// <summary>
            /// Gets the file association information.
            /// </summary>
            /// <param name="fileExtension">The file extension part.</param>
            /// <returns>The file association information; or null, if not found or failure.</returns>
            public static FileAssociationInfo GetFileAssociationInfo(string fileExtension)
            {
                fileExtension = fileExtension?.Trim()?.ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension)) return null;
                var info = new FileAssociationInfo
                {
                    FileExtension = fileExtension
                };
                using var ext = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, fileExtension);
                string h;
                if (ext == null)
                {
                    if (fileExtension.IndexOf('/') < 1) return info;
                    using var mime = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, "MIME\\Database\\Content Type\\" + fileExtension);
                    if (mime == null) return info;
                    info.ContentType = fileExtension;
                    fileExtension = Win32RegistryUtility.TryGetStringValue(mime, "Extension");
                    info.FileExtension = fileExtension;
                    using var ext2 = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, fileExtension);
                    if (ext2 == null) return info;
                    h = Win32RegistryUtility.TryGetStringValue(ext2, null);
                }
                else
                {
                    info.ContentType = Win32RegistryUtility.TryGetStringValue(ext, "Content Type");
                    h = Win32RegistryUtility.TryGetStringValue(ext, null);
                }

                if (string.IsNullOrEmpty(h))
                {
                    using var ext2 = Win32RegistryUtility.TryOpenSubKey(ext, "OpenWithProgids");
                    if (ext2 == null) return info;
                    h = Win32RegistryUtility.TryGetValueNames(ext2).FirstOrDefault(ele => !string.IsNullOrWhiteSpace(ele));
                    if (string.IsNullOrEmpty(h)) return info;
                }

                using var assoc = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, h);
                if (assoc == null) return info;
                info.Name = Win32RegistryUtility.TryGetStringValue(assoc, null) ?? Win32RegistryUtility.TryGetStringValue(assoc, "FriendlyTypeName");
                using var icon = Win32RegistryUtility.TryOpenSubKey(assoc, "DefaultIcon");
                if (icon != null) info.Icon = Win32RegistryUtility.TryGetStringValue(icon, null);
                using var shell = Win32RegistryUtility.TryOpenSubKey(assoc, "shell");
                if (shell == null) return info;
                var defaultCommand = Win32RegistryUtility.TryGetStringValue(shell, null);
                var commands = Win32RegistryUtility.TryOpenSubKeys(shell)?.ToList();
                if (commands.Count < 1) return info;
                foreach (var command in commands)
                {
                    var name = command.Name;
                    var pos = name?.LastIndexOf('\\') ?? -1;
                    if (pos >= 0) name = name.Substring(pos + 1);
                    var cmd = new FileOpenCommandInfo
                    {
                        Key = name,
                        Name = Win32RegistryUtility.TryGetStringValue(command, null)
                    };
                    var exe = Win32RegistryUtility.TryOpenSubKey(command, "command");
                    if (exe == null) continue;
                    var c = Win32RegistryUtility.TryGetStringValue(exe, null);
                    if (string.IsNullOrWhiteSpace(c)) continue;
                    cmd.Command = c;
                    info.Commands.Add(cmd);
                }

                if (!string.IsNullOrWhiteSpace(defaultCommand))
                {
                    info.DefaultCommand = info.Commands.FirstOrDefault(ele => defaultCommand.Equals(ele.Key, StringComparison.OrdinalIgnoreCase));
                }

                if (info.DefaultCommand == null) info.DefaultCommand = info.Commands.FirstOrDefault();
                return info;
            }

            /// <summary>
            /// Gets the MIME content type by file extension part.
            /// </summary>
            /// <param name="fileExtension">The file extension part.</param>
            /// <param name="throwExceptionForNullOrEmpty">true if throw an argument exception if the file extension is null or empty; otherwise, false.</param>
            /// <returns>The registry key.</returns>
            /// <exception cref="ArgumentNullException">fileExtension was null.</exception>
            /// <exception cref="ArgumentException">fileExtension was empty.</exception>
            private static RegistryKey GetRegistryKeyByFileExtension(string fileExtension, bool throwExceptionForNullOrEmpty)
            {
                if (fileExtension == null && throwExceptionForNullOrEmpty) throw new ArgumentNullException(nameof(fileExtension), "fileExtension should not be null.");
                fileExtension = fileExtension.Trim().ToLowerInvariant();
                if (fileExtension.Length < 1)
                {
                    if (throwExceptionForNullOrEmpty)
                        throw new ArgumentException("fileExtension should not be empty.", nameof(fileExtension));
                    return null;
                }

                var r = Win32RegistryUtility.TryOpenSubKey(Registry.ClassesRoot, fileExtension);
                return r;
            }
        }
#endif
    }
}
