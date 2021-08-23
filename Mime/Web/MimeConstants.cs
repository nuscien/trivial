using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Trivial.Collection;

namespace Trivial.Web
{
    /// <summary>
    /// The MIME constants.
    /// </summary>
    public static partial class MimeConstants
    {
        private static MethodInfo method;
        private static KeyedDataMapping<string> fem;

        /// <summary>
        /// Gets the MIME value of octet stream.
        /// </summary>
        public const string StreamMIME = "application/octet-stream";

        /// <summary>
        /// Gets the MIME mapping of file extension.
        /// </summary>
        public static KeyedDataMapping<string> FileExtensionMapping
        {
            get
            {
                if (fem != null) return fem;
                var prop = typeof(WebFormat).GetProperty("MimeMapping", BindingFlags.Static | BindingFlags.NonPublic);
                if (prop?.CanRead == true) fem = prop.GetValue(null) as KeyedDataMapping<string>;
                return fem;
            }
        }

        /// <summary>
        /// Gets the MIME from file extension.
        /// </summary>
        /// <param name="file">The file information.</param>
        /// <returns>A MIME value.</returns>
        public static string FromFileExtension(FileInfo file)
            => FromFileExtension(file?.Extension, StreamMIME);

        /// <summary>
        /// Gets the MIME from file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>A MIME value.</returns>
        public static string FromFileExtension(string fileExtension)
            => FromFileExtension(fileExtension, StreamMIME);

        /// <summary>
        /// Gets the MIME from file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="returnNullIfUnsupported">true if returns null if not supported; otherwise, false.</param>
        /// <returns>A MIME value.</returns>
        public static string FromFileExtension(string fileExtension, bool returnNullIfUnsupported)
            => FromFileExtension(fileExtension, returnNullIfUnsupported ? null : StreamMIME);

        /// <summary>
        /// Gets the MIME from file extension.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="defaultMime">The default MIME.</param>
        /// <returns>A MIME value.</returns>
        public static string FromFileExtension(string fileExtension, string defaultMime)
        {
            if (string.IsNullOrWhiteSpace(fileExtension)) return null;
            if (method == null)
                method = typeof(WebFormat).GetMethod("GetMime", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
            if (method == null) return defaultMime;
            var r = method.Invoke(null, new object[] { fileExtension });
            if (r == null) return defaultMime;
            try
            {
                return (string)r;
            }
            catch (InvalidCastException)
            {
            }

            return defaultMime;
        }
    }
}
