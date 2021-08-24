using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Trivial.Collection;
using Trivial.Text;

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
        /// The name of MIME.
        /// </summary>
        public const string Name = "Multipurpose Internet Mail Extensions";

        /// <summary>
        /// The MIME content type of octet stream.
        /// </summary>
        public const string StreamMIME = "application/octet-stream";

        /// <summary>
        /// Gets the MIME content type mapping of file extension.
        /// </summary>
        public static KeyedDataMapping<string> FileExtensionMapping
        {
            get
            {
                if (fem != null) return fem;
                var prop = typeof(WebFormat).GetProperty("MimeMapping", BindingFlags.Static | BindingFlags.NonPublic);
                if (prop == null) prop = typeof(WebFormat).GetProperty("MimeMapping", BindingFlags.Static | BindingFlags.Public);
                if (prop?.CanRead == true) fem = prop.GetValue(null) as KeyedDataMapping<string>;
                return fem;
            }
        }

        /// <summary>
        /// Gets the MIME content type by file extension part.
        /// </summary>
        /// <param name="file">The file information.</param>
        /// <returns>The MIME content type.</returns>
        public static string GetByFileExtension(FileInfo file)
            => GetByFileExtension(file?.Extension, StreamMIME);

        /// <summary>
        /// Gets the MIME content type by file extension part.
        /// </summary>
        /// <param name="file">The file information.</param>
        /// <param name="defaultMime">The default MIME content type.</param>
        /// <returns>The MIME content type.</returns>
        public static string GetByFileExtension(FileInfo file, string defaultMime)
            => GetByFileExtension(file?.Extension, defaultMime);

        /// <summary>
        /// Gets the MIME content type by file extension part.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>The MIME content type.</returns>
        public static string GetByFileExtension(string fileExtension)
            => GetByFileExtension(fileExtension, StreamMIME);

        /// <summary>
        /// Gets the MIME content type by file extension part.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="returnNullIfUnsupported">true if returns null if not supported; otherwise, false.</param>
        /// <returns>The MIME content type.</returns>
        public static string GetByFileExtension(string fileExtension, bool returnNullIfUnsupported)
            => GetByFileExtension(fileExtension, returnNullIfUnsupported ? null : StreamMIME);

        /// <summary>
        /// Gets the MIME content type by file extension part.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <param name="defaultMime">The default MIME content type.</param>
        /// <returns>The MIME content type.</returns>
        public static string GetByFileExtension(string fileExtension, string defaultMime)
        {
            if (string.IsNullOrWhiteSpace(fileExtension)) return null;
            if (method == null)
                method = typeof(WebFormat).GetMethod("GetMime", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
            if (method == null)
                method = typeof(WebFormat).GetMethod("GetMime", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
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

        /// <summary>
        /// Registers file extension mapping.
        /// </summary>
        /// <param name="json">The mapping source.</param>
        /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
        /// <returns>The count of item added or changed.</returns>
        public static int RegisterFileExtensionMapping(JsonObjectNode json, bool overrideIfExist = false)
        {
            var mapping = FileExtensionMapping;
            if (json == null || mapping == null) return 0;
            var arr = json.TryGetArrayValue("mime");
            if (arr != null) return RegisterFileExtensionMapping(arr, overrideIfExist);
            var body = json.TryGetObjectValue("mime");
            if (body != null) json = body;
            var i = 0;
            foreach (var item in json)
            {
                if (item.Value?.ValueKind != JsonValueKind.String) continue;
                if (!item.Value.TryGetString(out var s)) continue;
                if (mapping.Set(item.Key, s, overrideIfExist)) i++;
            }

            return i;
        }

        /// <summary>
        /// Registers file extension mapping.
        /// </summary>
        /// <param name="json">The source.</param>
        /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
        /// <returns>The count of item added or changed.</returns>
        public static int RegisterFileExtensionMapping(JsonArrayNode json, bool overrideIfExist = false)
        {
            var mapping = FileExtensionMapping;
            if (json == null || mapping == null) return 0;
            var i = 0;
            foreach (var item in json)
            {
                if (item is not JsonObjectNode ele) continue;
                var ext = ele.TryGetStringValue("extension") ?? ele.TryGetStringValue("ext");
                var mime = ele.TryGetStringValue("mime");
                if (mapping.Set(mime, ext, overrideIfExist)) i++;
            }

            return i;
        }

        /// <summary>
        /// Registers file extension mapping.
        /// </summary>
        /// <param name="source">The mapping source.</param>
        /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
        /// <returns>The count of item added or changed.</returns>
        public static int RegisterFileExtensionMapping(IDictionary<string, string> source, bool overrideIfExist = false)
        {
            var mapping = FileExtensionMapping;
            if (mapping == null || source == null) return 0;
            return mapping.Set(source, overrideIfExist);
        }

        /// <summary>
        /// Registers file extension mapping.
        /// </summary>
        /// <param name="fileExtension">The file extension part.</param>
        /// <param name="mime">The MIME content type.</param>
        /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
        /// <returns>true if registers succeeded; otherwise, false.</returns>
        public static bool RegisterFileExtensionMapping(string fileExtension, string mime, bool overrideIfExist = false)
        {
            var mapping = FileExtensionMapping;
            if (mapping == null || fileExtension == null) return false;
            return mapping.Set(fileExtension, mime, overrideIfExist);
        }
    }
}
