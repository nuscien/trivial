using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Trivial.Security
{
    /// <summary>
    /// The helper of secure string.
    /// </summary>
    public static class SecureStringUtility
    {
        /// <summary>
        /// Appends a string into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The string to append.</param>
        /// <exception cref="ArgumentNullException">the secure string instance was null.</exception>
        public static void AppendString(this SecureString obj, string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "obj should not be null.");
            if (string.IsNullOrEmpty(value)) return;
            foreach (var c in value)
            {
                obj.AppendChar(c);
            }
        }

        /// <summary>
        /// Appends a string builder content into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The string to append.</param>
        /// <exception cref="ArgumentNullException">the secure string instance was null.</exception>
        public static void AppendString(this SecureString obj, StringBuilder value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "obj should not be null.");
            if (value == null) return;
            for (var i = 0; i < value.Length; i++)
            {
                obj.AppendChar(value[i]);
            }
        }

        /// <summary>
        /// Appends a string by a byte array with character encoding into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The byte array to append.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <exception cref="ArgumentNullException">the secure string instance was null.</exception>
        public static void AppendString(this SecureString obj, byte[] value, Encoding encoding = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "obj should not be null.");
            if (value == null) return;
            foreach (var c in (encoding ?? Encoding.UTF8).GetChars(value))
            {
                obj.AppendChar(c);
            }
        }

        /// <summary>
        /// Appends a string into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The string to append.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException">the secure string instance was null.</exception>
        public static void AppendFormat(this SecureString obj, string value, params object[] args)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "obj should not be null.");
            if (string.IsNullOrEmpty(value)) return;
            value = string.Format(value, args);
            foreach (var c in value)
            {
                obj.AppendChar(c);
            }
        }

        /// <summary>
        /// Converts a string to a secure string instance.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The secure string instance.</returns>
        public static SecureString ToSecure(this string value)
        {
            var obj = new SecureString();
            AppendString(obj, value);
            return obj;
        }

        /// <summary>
        /// Converts a string builder instance to a secure string instance.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The secure string instance.</returns>
        public static SecureString ToSecure(this StringBuilder value)
        {
            if (value == null) return null;
            var obj = new SecureString();
            AppendString(obj, value.ToString());
            return obj;
        }

        /// <summary>
        /// Converts a byte array instance to a secure string instance.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>The secure string instance.</returns>
        public static SecureString ToSecure(this byte[] value, Encoding encoding = null)
        {
            if (value == null) return null;
            var obj = new SecureString();
            AppendString(obj, value, encoding);
            return obj;
        }

        /// <summary>
        /// Sets the new value into the secure string instance.
        /// </summary>
        /// <param name="value">The secure string instance.</param>
        /// <param name="newValue">The new string to set.</param>
        /// <param name="old">The optional old string to validate.</param>
        /// <param name="confirm">The confirm string for the new one.</param>
        /// <returns>true if set succeeded; otherwise, false.</returns>
        public static bool Set(this SecureString value, string newValue, string old = null, string confirm = null)
        {
            if (value == null) return false;
            if (confirm != null && newValue != confirm) return false;
            if (value.IsReadOnly()) return false;
            if (old != null)
            {
                if (ToUnsecureString(value) != old) return false;
                if (newValue == old) return true;
            }

            value.Clear();
            AppendString(value, newValue);
            return true;
        }

        /// <summary>
        /// Converts a secure string to unsecure string.
        /// </summary>
        /// <param name="value">The secure string to convert.</param>
        /// <returns>The unsecure string.</returns>
        public static string ToUnsecureString(this SecureString value)
        {
            if (value == null) return null;
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
