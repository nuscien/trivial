using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using Microsoft.Win32;

namespace Trivial.Data;

/// <summary>
/// The Windows Registry utility.
/// </summary>
#if NET6_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
public static class RegistryUtility
{
    /// <summary>
    /// Tries to get the sub registry key of another specific one.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="path">The path of the sub key.</param>
    /// <param name="writable">true if need write access to the key; otherwise false.</param>
    /// <returns>The registry key.</returns>
    public static RegistryKey TryOpenSubKey(RegistryKey regKey, string path, bool writable = false)
    {
        path = path?.Trim();
        if (string.IsNullOrEmpty(path)) return regKey;
        try
        {
            if (regKey != null) return regKey.OpenSubKey(path, writable);
            var i = path.IndexOf('\\', 0);
            if (i == 0) i = path.TrimStart('\\').IndexOf('\\');
            var key = (i < 0 ? path : path.Substring(0, i)).Trim().ToUpperInvariant();
            switch (key)
            {
                case "HKEY_CLASSES_ROOT":
                    regKey = Registry.ClassesRoot;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    regKey = Registry.CurrentConfig;
                    break;
                case "HKEY_CURRENT_USER":
                    regKey = Registry.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    regKey = Registry.LocalMachine;
                    break;
                case "HKEY_PERFORMANCE_DATA":
                    regKey = Registry.PerformanceData;
                    break;
                case "HKEY_USERS":
                    regKey = Registry.Users;
                    break;
                default:
                    return null;
            }

            if (i < 0) return regKey;
            path = path.Substring(i + 1);
            if (string.IsNullOrEmpty(path)) return regKey;
            return regKey.OpenSubKey(path, writable);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (ExternalException)
        {
        }
        catch (AggregateException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to get the string value.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The string value; or the default value if not found or failure.</returns>
    public static string TryGetStringValue(RegistryKey regKey, string name, string defaultValue = null)
        => TryGetValue<string>(regKey, name, out var r) ? r : defaultValue;

    /// <summary>
    /// Tries to get the integer value.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The integer value; or the default value if not found or failure.</returns>
    public static int TryGetInt32Value(RegistryKey regKey, string name, int defaultValue)
        => TryGetValue<int>(regKey, name, out var r) ? r : defaultValue;

    /// <summary>
    /// Tries to get the integer value.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <returns>The integer value; or the null if not found or failure.</returns>
    public static int? TryGetInt32Value(RegistryKey regKey, string name)
        => TryGetValue<int>(regKey, name, out var r) ? r : null;

    /// <summary>
    /// Tries to get the integer value.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The integer value; or the default value if not found or failure.</returns>
    public static long TryGetInt64Value(RegistryKey regKey, string name, long defaultValue)
        => TryGetValue<long>(regKey, name, out var r) ? r : defaultValue;

    /// <summary>
    /// Tries to get the integer value.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <returns>The integer value; or the null if not found or failure.</returns>
    public static long? TryGetInt64Value(RegistryKey regKey, string name)
        => TryGetValue<long>(regKey, name, out var r) ? r : null;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value to cast.</typeparam>
    /// <param name="regKey">The registry key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="value">The value output.</param>
    /// <returns>The value to get.</returns>
    public static bool TryGetValue<T>(RegistryKey regKey, string name, out T value)
        => TryGetValue(regKey, null, name, out value);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value to cast.</typeparam>
    /// <param name="path">The path of the sub key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="value">The value output.</param>
    /// <returns>The value to get.</returns>
    public static bool TryGetValue<T>(string path, string name, out T value)
        => TryGetValue(null, path, name, out value);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value to cast.</typeparam>
    /// <param name="regKey">The registry key.</param>
    /// <param name="path">The path of the sub key.</param>
    /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
    /// <param name="value">The value output.</param>
    /// <returns>The value to get.</returns>
    public static bool TryGetValue<T>(RegistryKey regKey, string path, string name, out T value)
    {
        regKey = TryOpenSubKey(regKey, path);
        if (regKey == null)
        {
            value = default;
            return false;
        }

        try
        {
            var r = regKey.GetValue(name);
            if (r is T result)
            {
                value = result;
                return true;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (IOException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (ExternalException)
        {
        }
        catch (AggregateException)
        {
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the specific values.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="names">The names of the value to retrieve. This string is not case-sensitive.</param>
    /// <returns>All values to get.</returns>
    public static Dictionary<string, object> TryGetValues(RegistryKey regKey, IEnumerable<string> names = null)
    {
        if (regKey == null) return null;
        var dict = new Dictionary<string, object>();
        if (names == null) names = TryGetValueNames(regKey);
        foreach (var name in names)
        {
            try
            {
                var r = regKey.GetValue(name);
                if (r is not null) dict[name] = r;
            }
            catch (SecurityException)
            {
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            catch (Win32Exception)
            {
            }
        }

        return dict;
    }

    /// <summary>
    /// Tries to get the value names.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <returns>All value names.</returns>
    public static string[] TryGetValueNames(RegistryKey regKey)
    {
        if (regKey == null) return null;
        try
        {
            return regKey.GetValueNames() ?? Array.Empty<string>();
        }
        catch (SecurityException)
        {
        }
        catch (IOException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (Win32Exception)
        {
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Tries to get the sub registry key names.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <returns>All sub registry key names.</returns>
    public static string[] TryGetSubKeyNames(RegistryKey regKey)
    {
        if (regKey == null) return null;
        try
        {
            return regKey.GetSubKeyNames() ?? Array.Empty<string>();
        }
        catch (SecurityException)
        {
        }
        catch (IOException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (Win32Exception)
        {
        }

        return Array.Empty<string>();
    }

    /// <summary>
    /// Tries to get the sub registry keys.
    /// </summary>
    /// <param name="regKey">The registry key.</param>
    /// <param name="writable">true if need write access to the key; otherwise false.</param>
    /// <returns>All sub registry keys.</returns>
    public static IEnumerable<RegistryKey> TryOpenSubKeys(RegistryKey regKey, bool writable = false)
    {
        var names = TryGetSubKeyNames(regKey);
        if (names == null) yield break;
        foreach (var name in names)
        {
            var r = TryOpenSubKey(regKey, name, writable);
            if (r != null) yield return r;
        }
    }
}
