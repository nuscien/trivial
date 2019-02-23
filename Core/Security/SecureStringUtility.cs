using System;
using System.Collections.Generic;
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
        public static void AppendString(this SecureString obj, string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(value)) return;
            foreach (var c in value)
            {
                obj.AppendChar(c);
            }
        }

        /// <summary>
        /// Appends a string into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The string to append.</param>
        public static void AppendString(this SecureString obj, StringBuilder value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (value == null) return;
            for (var i = 0; i < value.Length; i++)
            {
                obj.AppendChar(value[i]);
            }
        }

        /// <summary>
        /// Appends a string into a secure string instance.
        /// </summary>
        /// <param name="obj">The secure string instance.</param>
        /// <param name="value">The string to append.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void AppendFormat(this SecureString obj, string value, params object[] args)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(value)) return;
            value = string.Format(value, args);
            foreach (var c in value)
            {
                obj.AppendChar(c);
            }
        }

        /// <summary>
        /// Converts a secure string instance to a string.
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
        /// Converts a secure string instance to a string.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The secure string instance.</returns>
        public static SecureString ToSecure(this StringBuilder value)
        {
            var obj = new SecureString();
            AppendString(obj, value.ToString());
            return obj;
        }

        /// <summary>
        /// Sets the new value.
        /// </summary>
        /// <param name="value">The secure string instance.</param>
        /// <param name="newValue">The new string to set.</param>
        /// <param name="old">The optional old string to validate.</param>
        /// <param name="confirm">The confirm string for the new one.</param>
        /// <returns>true if set succeeded; otherwise, false.</returns>
        public static bool Set(this SecureString value, string newValue, string old = null, string confirm = null)
        {
            if (confirm != null && newValue != confirm) return false;
            if (value.IsReadOnly()) return false;
            if (old != null)
            {
                if (value.ToString() != old) return false;
                if (newValue == old) return true;
            }

            value.Clear();
            AppendString(value, newValue);
            return true;
        }
    }
}
