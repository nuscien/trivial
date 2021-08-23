using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Web
{
    /// <summary>
    /// Web format utility.
    /// </summary>
    public static partial class WebFormat
    {
        /// <summary>
        /// Gets the MIME value of document type definition format text.
        /// </summary>
        public const string DocumentTypeDefinitionMIME = "application/xml-dtd";

        /// <summary>
        /// Gets the MIME value of extensible markup language format text.
        /// </summary>
        public const string XmlMIME = "application/xml";

        /// <summary>
        /// Gets the MIME value of JSON format text.
        /// </summary>
        public const string JsonMIME = "application/json";

        /// <summary>
        /// Gets the MIME value of JavaScript (ECMAScript) format text.
        /// </summary>
        public const string JavaScriptMIME = "text/javascript";

        /// <summary>
        /// Gets the MIME value of YAML format text.
        /// </summary>
        public const string YamlMIME = "application/x-yaml";

        /// <summary>
        /// Gets the MIME value of the URL encoded.
        /// </summary>
        public const string FormUrlMIME = "application/x-www-form-urlencoded";

        /// <summary>
        /// Gets the MIME value of CSS format text.
        /// </summary>
        public const string CssMIME = "text/css";

        /// <summary>
        /// Gets the MIME value of HTML format text.
        /// </summary>
        public const string HtmlMIME = "text/html";

        /// <summary>
        /// Gets the MIME value of SVG format text.
        /// </summary>
        public const string SvgMIME = "image/svg+xml";

        /// <summary>
        /// Gets the MIME value of markdown format text.
        /// </summary>
        public const string MarkdownMIME = "text/markdown";

        /// <summary>
        /// Gets the MIME value of web assembly format text.
        /// </summary>
        public const string WebAssemblyMIME = "application/wasm";

        /// <summary>
        /// Gets the MIME value of octet stream.
        /// </summary>
        public const string StreamMIME = "application/octet-stream";

        /// <summary>
        /// The MIME mapping.
        /// </summary>
        internal static Collection.KeyedDataMapping<string> MimeMapping { get; } = new();

        /// <summary>
        /// Gets the MIME.
        /// </summary>
        /// <param name="file">The file information.</param>
        /// <returns>A MIME value.</returns>
        /// <remarks>This just contains the most useful MIMEs.</remarks>
        internal static string GetMime(FileInfo file)
            => GetMime(file?.Extension);

        /// <summary>
        /// Gets the MIME.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>A MIME value.</returns>
        /// <remarks>This just contains the most useful MIMEs.</remarks>
        internal static string GetMime(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension)) return null;
            fileExtension = fileExtension.Trim().ToLowerInvariant();
            if (MimeMapping.TryGetValue(fileExtension, out var s) && !string.IsNullOrEmpty(s)) return s;

            // http://www.iana.org/assignments/media-types/media-types.xhtml
            return fileExtension.Remove(0, 1) switch
            {
                // Office document
                "cat" => "application/vnd.ms-pki.seccat",
                "doc" => "application/msword",
                "docm" => "application/vnd.ms-word.document.macroenabled.12",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "dotx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "mda" or "mdb" or "mde" or "accdb" => "application/x-msaccess",
                "mpp" or "mpt" => "application/vnd.ms-project",
                "msg" => "application/vnd.ms-outlook",
                "ppt" => "application/vnd.ms-powerpoint",
                "pptm" => "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "ppsx" => "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                "sldx" => "application/vnd.openxmlformats-officedocument.presentationml.slide",
                "potx" => "application/vnd.openxmlformats-officedocument.presentationml.template",
                "xls" => "application/vnd.ms-excel",
                "xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xltx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                "onetoc" or "onetok2" or "onetmp" or "onepkg" => "application/onenote",
                "xps" => "application/vnd.ms-xpsdocument",
                "oxps" => "application/oxps",
                "vsd" or "vst" or "vss" or "vsw" => "application/vnd.visio",

                // Open Document
                "odc" => "application/vnd.oasis.opendocument.chart",
                "otc" => "application/vnd.oasis.opendocument.chart-template",
                "odb" => "application/vnd.oasis.opendocument.database",
                "odf" => "application/vnd.oasis.opendocument.formula",
                "odft" => "application/vnd.oasis.opendocument.formula-template",
                "odg" => "application/vnd.oasis.opendocument.graphics",
                "otg" => "application/vnd.oasis.opendocument.graphics-template",
                "odi" => "application/vnd.oasis.opendocument.image",
                "oti" => "application/vnd.oasis.opendocument.image-template",
                "odp" => "application/vnd.oasis.opendocument.presentation",
                "otp" => "application/vnd.oasis.opendocument.presentation-template",
                "ods" => "application/vnd.oasis.opendocument.spreadsheet",
                "ots" => "application/vnd.oasis.opendocument.spreadsheet-template",
                "odt" => "application/vnd.oasis.opendocument.text",
                "odm" => "application/vnd.oasis.opendocument.text-master",
                "ott" => "application/vnd.oasis.opendocument.text-template",
                "oth" => "application/vnd.oasis.opendocument.text-web",

                // Image
                "apng" => "image/apng",
                "png" => "image/png",
                "bmp" or "dib" => "image/bmp",
                "dwg" => "image/vnd.dwg",
                "dxf" => "image/vnd.dxf",
                "gif" => "image/gif",
                "heif" or "hif" => "image/heif",
                "heic" => "image/heic",
                "ico" or "cur" => "image/x-icon",
                "jpe" or "jpeg" or "jpg" or "jfif" => "image/jpeg",
                "psd" => "image/vnd.adobe.photoshop",
                "ai" => "application/illustrator",
                "cdr" => "application/vnd.corel-draw",
                "tif" or "tiff" => "image/tiff",
                "svg" or "svgz" => SvgMIME,
                "webp" => "image/webp",
                "3ds" => "image/x-3ds",
                "wmf" or "emf" or "emz" => "application/x-msmetafile",

                // Audio
                "au" or "snd" => "audio/basic",
                "mid" or "midi" or "kar" or "rmi" => "audio/midi",
                "m4a" or "mp4a" => "audio/mp4",
                "mp3" or "mpga" or "mp2" or "mp2a" or "m2a" or "m3a" => "audio/mpeg",
                "ogg" or "oga" or "spx" => "audio/ogg",
                "pya" => "audio/vnd.ms-playready.media.pya",
                "weba" => "audio/webm",
                "aac" => "audio/x-aac",
                "aif" or "aiff" or "aifc" => "audio/x-aiff",
                "flac" => "audio/x-flac",
                "mka" => "audio/x-matroska",
                "m3u" => "audio/x-mpegurl",
                "wax" => "audio/x-ms-wax",
                "wma" => "audio/x-ms-wma",
                "wav" or "wave" => "audio/wav",

                // Video
                "3gp" or "3gpp" => "video/3gpp",
                "3g2" or "3gp2" => "video/3gpp2",
                "avi" => "video/x-msvideo",
                "h261" => "video/h261",
                "h263" => "video/h263",
                "h264" => "video/h264",
                "h265" or "hevc" => "video/h265",
                "h266" or "vvc" => "video/h266",
                "h267" => "video/h267",
                "h268" => "video/h268",
                "av1" => "video/av1",
                "av2" => "video/av2",
                "jpgv" => "video/jpeg",
                "jpm" or "jpgm" => "video/jpm",
                "mp4" or "mp4v" or "mpg4" => "video/mp4",
                "mpeg" or "mpg" or "mpe" or "m1v" or "m2v" => "video/mpeg",
                "ogv" => "video/ogg",
                "mov" => "video/quicktime",
                "dvd" => "video/vnd.dvb.file",
                "mxu" or "m4u" => "video/vnd.mpegurl",
                "pyv" => "video/vnd.ms-playready.media.pyv",
                "webm" => "video/webm",
                "flv" => "video/x-flv",
                "m4v" => "video/x-m4v",
                "mkv" or "mk3d" or "mks" => "video/x-matroska",
                "asf" or "asx" => "video/x-ms-asf",
                "wm" => "video/x-ms-wm",
                "wmv" => "video/x-ms-wm",
                "wmx" => "video/x-ms-wmx",

                // Text
                "txt" or "text" or "log" or "def" or "ini" or "gitignore" or "editorconfig" => Text.StringExtensions.PlainTextMIME,
                "csv" => Text.CsvParser.MIME,
                "tsv" => Text.TsvParser.MIME,
                "md" => MarkdownMIME,
                "mml" => "text/mathml",
                "rtx" => Text.StringExtensions.RichTextMIME,
                "sgml" or "sgm" => "text/sgml",
                "vcf" => "text/x-vcard",
                "vtt" => "text/vtt",
                "diff" or "patch" => "text/x-diff",

                // Web
                "ac" => "application/pkix-attr-cert",
                "cer" => "application/pkix-cert",
                "css" => CssMIME,
                "sass" => "text/x-sass",
                "scss" => "text/x-scss",
                "less" => "text/x-less",
                "crl" => "application/pkix-crl",
                "dtd" => DocumentTypeDefinitionMIME,
                "ecma" => "application/ecmascript",
                "epub" => "application/epub+zip",
                "htm" or "html" or "shtml" or "hta" => HtmlMIME,
                "ink" or "inkml" => "application/inkml+xml",
                "js" or "jsx" or "esm" => JavaScriptMIME,
                "json" or "map" => JsonMIME,
                "p8" => "application/pkcs8",
                "pem" => "application/x-x509-ca-cert",
                "pki" => "application/pkixcmp",
                "pkipath" => "application/pkix-pkipath",
                "uri" or "uris" or "urls" => "text/uri-list",
                "xaml" => "application/xaml+xml",
                "xml" or "xsl" or "config" => XmlMIME,
                "vbs" => "text/vbscript",
                "yaml" or "yml" => YamlMIME,

                // Font
                "eot" => "application/vnd.ms-fontobject",
                "pcf" => "application/x-font-pcf",
                "pfr" => "application/font-tdpfr",
                "snf" => "application/x-font-snf",
                "ttf" or "ttc" => "application/x-font-ttf",
                "otf" => "application/x-font-otf",
                "pfa" or "pfb" or "pfm" or "afm" => "application/x-font-type1",
                "woff" => "application/font-woff",

                // Programming
                "c" or "cc" or "cpp" or "cxx" or "dic" => "text/x-c",
                "h" => "text/x-chdr",
                "hh" or "hpp" => "text/x-c++hdr",
                "java" => "text/x-java-source",
                "cs" => "text/x-csharp",
                "qs" => "text/x-qsharp",
                "vb" => "text/x-vb",
                "csproj" or "csdproj" or "vbproj" or "vbdproj" => Text.StringExtensions.PlainTextMIME,
                "py" or "py2" or "py3" or "pyw" => "text/x-python",
                "go" => "text/x-golang",
                "settings" => XmlMIME,
                "sql" => "application/x-sql",
                "ps1" => Text.StringExtensions.PlainTextMIME,

                // Others
                "application" => "application/x-ms-application",
                "iso" => "application/x-iso9660-image",
                "ics" or "ifb" => "text/calendar",
                "pdf" => "application/pdf",
                "crd" => "application/x-mscardfile",
                "clp" => "application/x-msclip",
                "jar" => "application/java-archive",
                "wasm" => WebAssemblyMIME,
                "aspx" => "application/x-aspx",
                "dll" or "pdb" or "exe" or "bat" or "msi" or "msu" or "com" => "application/x-msdownload",
                "app" or "glif" or "resx" or "php" or "jsp" or "cshtml" or "vbhtml" or "razor" or "3mf" or "lib" or "bin" or "dat" or "data" or "db" or "dms" or "lrf" or "pkg" or "dump" or "deploy" or "vso" or "nupkg" or "xsn" or "sln" or "vsix" or "ts" or "tsx" or "usr" or "user" or "bson" => StreamMIME,
                "pqa" or "oprc" => "application/vnd.palm",
                "appx" => "application/appx",
                "appxbundle" => "application/appxbundle",
                "appinstaller" => "application/appinstaller",
                "msix" => "application/msix",
                "msixbundle" => "application/msixbundle",
                "apk" or "aab" => "application/vnd.android.package-archive",
                "lnk" => "application/x-ms-shortcut",
                "123" => "application/vnd.lotus-1-2-3",
                "o" => "application/x-object",
                "obj" => "application/x-tgif",
                "zip" => "application/zip",
                "7z" => "application/x-7z-compressed",
                "cab" => "application/vnd.ms-cab-compressed",
                "rar" => "application/x-rar-compressed",
                "tar" => "application/tar",
                "gz" => "application/gzip",
                "br" => "application/brotli",
                "tgz" or "tar.gz" => "application/tar+gzip",
                "z" => "application/x-compress",
                _ => null,
            };
        }
    }
}
