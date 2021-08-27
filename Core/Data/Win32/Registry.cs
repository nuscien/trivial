using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security;

using Microsoft.Win32;

namespace Trivial.Data
{
#if NETFRAMEWORK
    /// <summary>
    /// The Windows Registry utility.
    /// </summary>
    public static class RegistryUtility
    {
        /// <summary>
        /// Tries to get the sub registry key of another specific one.
        /// </summary>
        /// <param name="regKey">The registry key.</param>
        /// <param name="path">The path of the sub key.</param>
        /// <param name="writable">true if need write access to the key; otherwise false.</param>
        /// <returns>The registry key.</returns>
        /// <exception cref="ArgumentNullException">fileExtension was null.</exception>
        /// <exception cref="ArgumentException">fileExtension was empty.</exception>
        public static RegistryKey TryOpenSubKey(RegistryKey regKey, string path, bool writable = false)
        {
            path = path?.Trim();
            if (string.IsNullOrEmpty(path)) return regKey;
            if (regKey == null) return null;
            try
            {
                return regKey.OpenSubKey(path, writable);
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
            catch (Win32Exception)
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
        {
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
#endif
}
