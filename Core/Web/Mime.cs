using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Web;

/// <summary>
/// Web format utility.
/// </summary>
public static partial class WebFormat
{
    /// <summary>
    /// Gets the MIME content type of document type definition format text.
    /// </summary>
    public const string DocumentTypeDefinitionMIME = "application/xml-dtd";

    /// <summary>
    /// Gets the MIME content type of extensible markup language format text.
    /// </summary>
    public const string XmlMIME = "application/xml";

    /// <summary>
    /// Gets the MIME content type of JavaScript (ECMAScript) format text.
    /// </summary>
    public const string JavaScriptMIME = "application/javascript";

    /// <summary>
    /// Gets the MIME content type of YAML format text.
    /// </summary>
    public const string YamlMIME = "application/x-yaml";

    /// <summary>
    /// Gets the MIME content type of the URL encoded.
    /// </summary>
    public const string FormUrlMIME = "application/x-www-form-urlencoded";

    /// <summary>
    /// Gets the MIME content type of multipart form data.
    /// </summary>
    public const string FormDataMIME = "multipart/form-data";

    /// <summary>
    /// Gets the MIME content type of CSS format text.
    /// </summary>
    public const string CssMIME = "text/css";

    /// <summary>
    /// Gets the MIME content type of HTML format text.
    /// </summary>
    public const string HtmlMIME = "text/html";

    /// <summary>
    /// Gets the MIME content type of SVG format text.
    /// </summary>
    public const string SvgMIME = "image/svg+xml";

    /// <summary>
    /// Gets the MIME content type of markdown format text.
    /// </summary>
    public const string MarkdownMIME = "text/markdown";

    /// <summary>
    /// Gets the MIME content type of web assembly format text.
    /// </summary>
    public const string WebAssemblyMIME = "application/wasm";

    /// <summary>
    /// Gets the MIME content type of MPEG transport stream.
    /// </summary>
    public const string MpegTransportStream = "video/mp2t";

    /// <summary>
    /// Gets the MIME content type of extended M3U playlist.
    /// </summary>
    public const string ExtendedPlaylist = "application/x-mpegurl";

    /// <summary>
    /// Gets the MIME content type of Server-Sent Events response.
    /// </summary>
    public const string ServerSentEventsMIME = "text/event-stream";

    /// <summary>
    /// Gets the MIME content type of octet stream.
    /// </summary>
    public const string StreamMIME = "application/octet-stream";

    /// <summary>
    /// Gets the content type with UTF-8 character set.
    /// </summary>
    /// <param name="contentType">The content type (MIME).</param>
    /// <returns>A string with the specific content type and character set UTF-8.</returns>
    public static string GetUtf8ContentType(string contentType)
        => string.Concat(contentType, "; charset=utf-8");

    /// <summary>
    /// Gets the content type of multiple part form data.
    /// </summary>
    /// <param name="boundary">The boundary.</param>
    /// <returns>The content type of multiple part form data with boundary.</returns>
    public static string GetFormDataContentType(string boundary)
        => string.Concat(FormDataMIME, "; boundary=", boundary);

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
            "doc" => "application/msword",
            "docm" => "application/vnd.ms-word.document.macroenabled.12",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "dotx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
            "mda" or "mdb" or "mde" or "accdb" or "accde" or "accdt" => "application/x-msaccess",
            "mpp" or "mpt" => "application/vnd.ms-project",
            "msg" => "application/vnd.ms-outlook",
            "ppt" => "application/vnd.ms-powerpoint",
            "pptm" => "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
            "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "ppsx" => "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
            "sldx" => "application/vnd.openxmlformats-officedocument.presentationml.slide",
            "potx" => "application/vnd.openxmlformats-officedocument.presentationml.template",
            "ppam" => "application/vnd.ms-powerpoint.addin.macroEnabled.12",
            "pps" => "application/vnd.ms-powerpoint",
            "ppsm" => "application/vnd.ms-powerpoint.slideshow.macroEnabled.12",
            "sldm" => "application/vnd.ms-powerpoint.slide.macroEnabled.12",
            "xls" or "xla" or "xlc" or "xlm" or "xlt" => "application/vnd.ms-excel",
            "xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "xltx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
            "xlam" => "application/vnd.ms-excel.addin.macroEnabled.12",
            "xlsb" => "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
            "xlw" => "application/vnd.ms-excel",
            "onetoc" or "onetok2" or "onetmp" or "onepkg" or "one" or "onea" or "onetoc2" => "application/onenote",
            "xps" => "application/vnd.ms-xpsdocument",
            "oxps" => "application/oxps",
            "vsd" or "vst" or "vss" or "vsw" or "vsx" or "vtx" => "application/vnd.visio",
            "vdx" => "application/vnd.ms-visio.viewer",
            "vsto" => "application/x-ms-vsto",
            "pub" => "application/x-mspublisher",
            "wcm" or "wdb" or "wks" => "application/vnd.ms-works",
            "thmx" => "application/vnd.ms-officetheme",
            "wri" => "application/x-mswrite",
            "lit" => "application/x-ms-reader",
            "calx" => "application/vnd.ms-office.calx",

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
            "oxt" => "application/vnd.openofficeorg.extension",

            // Image
            "apng" => "image/apng",
            "png" or "pnz" => "image/png",
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
            "xbm" => "image/x-xbitmap",
            "ief" => "image/ief",
            "ppm" => "image/x-portable-pixmap",
            "art" => "image/x-jg",
            "cmx" => "image/x-cmx",
            "cod" => "image/cis-cod",
            "ras" => "image/x-cmu-raster",
            "rgb" => "image/x-rgb",
            "xpm" => "image/x-xpixmap",

            // Audio
            "flac" => "audio/x-flac",
            "mp3" or "mpga" or "mp2" or "mp2a" or "m2a" or "m3a" => "audio/mpeg",
            "weba" => "audio/webm",
            "wma" => "audio/x-ms-wma",
            "au" or "snd" or "smd" or "smx" or "smz" => "audio/basic",
            "wav" or "wave" => "audio/wav",
            "aac" => "audio/x-aac",
            "mid" or "midi" or "kar" or "rmi" => "audio/midi",
            "m4a" or "mp4a" => "audio/mp4",
            "ogg" or "oga" or "spx" => "audio/ogg",
            "pya" => "audio/vnd.ms-playready.media.pya",
            "aif" or "aiff" or "aifc" => "audio/x-aiff",
            "mka" => "audio/x-matroska",
            "m3u" => "audio/x-mpegurl",
            "wax" => "audio/x-ms-wax",
            "mlp" => "application/vnd.dolby.mlp",

            // Video
            "m3u8" => ExtendedPlaylist,
            "mp4" or "mp4v" or "mpg4" => "video/mp4",
            "webm" => "video/webm",
            "wmv" => "video/x-ms-wm",
            "h268" => "video/h268",
            "h267" => "video/h267",
            "h266" or "vvc" => "video/h266",
            "h265" or "hevc" => "video/h265",
            "h264" => "video/h264",
            "h263" => "video/h263",
            "h261" => "video/h261",
            "mpeg" or "mpg" or "mpe" or "m1v" or "m2v" => "video/mpeg",
            "avi" => "video/x-msvideo",
            "3gp" or "3gpp" => "video/3gpp",
            "3g2" or "3gp2" => "video/3gpp2",
            "av1" => "video/av1",
            "av2" => "video/av2",
            "av3" => "video/av3",
            "avs3" => "video/avs3",
            "avs4" => "video/avs4",
            "jpgv" => "video/jpeg",
            "jpm" or "jpgm" => "video/jpm",
            "ogv" => "video/ogg",
            "mov" or "qt" or "qtl" => "video/quicktime",
            "movie" => "video/x-sgi-movie",
            "dvd" => "video/vnd.dvb.file",
            "mxu" or "m4u" => "video/vnd.mpegurl",
            "pyv" => "video/vnd.ms-playready.media.pyv",
            "m4v" => "video/x-m4v",
            "mkv" or "mk3d" or "mks" => "video/x-matroska",
            "asf" or "asx" => "video/x-ms-asf",
            "wm" => "video/x-ms-wm",
            "wmx" => "video/x-ms-wmx",
            "wmp" => "video/x-ms-wmp",
            "mpa" => "video/mpeg",
            "mpv2" => "video/mpeg",
            "nsc" => "video/x-ms-asf",
            "wvx" => "video/x-ms-wvx",
            "flv" => "video/x-flv",
            "rm" or "rmvb" => "application/vnd.rn-realmedia",
            "vp9" => "video/vp9",

            // Text
            "txt" or "text" or "log" or "def" or "ini" or "gitignore" or "editorconfig" or "bas" or "cnf" or "xdr" or "asm" or "vcs" or "lrc" => Text.StringExtensions.PlainTextMIME,
            "csv" => Text.CsvParser.MIME,
            "tsv" => Text.TsvParser.MIME,
            "md" => MarkdownMIME,
            "mml" => "text/mathml",
            "rtx" => Text.StringExtensions.RichTextMIME,
            "sgml" or "sgm" => "text/sgml",
            "vcf" => "text/x-vcard",
            "jcf" => "text/x-jcard",
            "vtt" => "text/vtt",
            "diff" or "patch" => "text/x-diff",
            "323" => "text/h323",
            "dlm" => "text/dlm",
            "etx" => "text/x-setext",
            "hdml" => "text/x-hdml",
            "uls" => "text/iuls",
            "pml" => "application/vnd.ctc-posml",

            // Web
            "json" or "map" or "jsonc" => Text.JsonValues.JsonMIME,
            "jsonl" => Text.JsonValues.JsonlMIME,
            "xml" or "xsl" or "xsf" or "xsd" or "config" or "xslt" or "mno" or "vml" or "wsdl" or "disco" => XmlMIME,
            "yaml" or "yml" => YamlMIME,
            "epub" => "application/epub+zip",
            "css" => CssMIME,
            "sass" => "text/x-sass",
            "scss" => "text/x-scss",
            "less" => "text/x-less",
            "crl" => "application/pkix-crl",
            "dtd" => DocumentTypeDefinitionMIME,
            "ecma" => "application/ecmascript",
            "htm" or "html" or "shtml" or "hta" or "hxt" => HtmlMIME,
            "htt" => "text/webviewhtml",
            "ink" or "inkml" => "application/inkml+xml",
            "js" or "jsx" or "mjs" or "esm" => JavaScriptMIME,
            "cjs" => "application/node",
            "uri" or "uris" or "urls" => "text/uri-list",
            "xaml" => "application/xaml+xml",
            "vbs" => "text/vbscript",
            "dart" => "application/vnd.dart",
            "chm" => "application/vnd.ms-htmlhelp",
            "acx" => "application/internet-property-stream",
            "osdx" => "application/opensearchdescription+xml",
            "atom" => "application/atom+xml",
            "ps" => "application/postscript",
            "wasm" => WebAssemblyMIME,
            "ics" or "ifb" => "text/calendar",
            "me" => "application/x-troff-me",
            "mht" or "mhtml" or "nws" or "eml" => "message/rfc822",
            "pcurl" => "application/vnd.curl.pcurl",
            "car" => "application/vnd.curl.car",
            "x3d" => "model/x3d+xml",
            "x3dv" => "model/x3d-vrml",
            "x3db" => "model/x3d+fastinfoset",

            // Credential
            "pem" => "application/x-x509-ca-cert",
            "cer" => "application/pkix-cert",
            "pki" => "application/pkixcmp",
            "pkipath" => "application/pkix-pkipath",
            "ac" => "application/pkix-attr-cert",
            "cat" => "application/vnd.ms-pki.seccat",
            "pfx" or "p12" => "application/x-pkcs12",
            "p10" => "application/pkcs10",
            "p8" => "application/pkcs8",
            "p7b" => "application/x-pkcs7-certificates",
            "p7c" => "application/pkcs7-mime",
            "p7m" => "application/pkcs7-mime",
            "p7r" => "application/x-pkcs7-certreqresp",
            "p7s" => "application/pkcs7-signature",
            "pko" => "application/vnd.ms-pki.pko",
            "sst" => "application/vnd.ms-pki.certstore",
            "stl" => "application/vnd.ms-pki.stl",

            // Font
            "ttf" or "ttc" => "font/ttf",
            "eot" => "application/vnd.ms-fontobject",
            "pcf" => "application/x-font-pcf",
            "pfr" => "application/font-tdpfr",
            "snf" => "application/x-font-snf",
            "otf" => "font/otf",
            "pfa" or "pfb" or "pfm" or "afm" => "application/x-font-type1",
            "woff" => "font/woff",
            "woff2" => "font/woff2",
            "ppd" => "application/vnd.cups-ppd",

            // Programming
            "java" => "text/x-java-source",
            "py" or "py2" or "py3" or "pyw" => "text/x-python",
            "go" => "text/x-golang",
            "cs" => "text/x-csharp",
            "qs" => "text/x-qsharp",
            "vb" => "text/x-vb",
            "csproj" or "csdproj" or "vbproj" or "vbdproj" => Text.StringExtensions.PlainTextMIME,
            "aspx" => "application/x-aspx",
            "c" or "cc" or "cpp" or "cxx" or "dic" => "text/x-c",
            "h" => "text/x-chdr",
            "hh" or "hpp" => "text/x-c++hdr",
            "settings" => XmlMIME,
            "sql" => "application/x-sql",
            "ps1" => Text.StringExtensions.PlainTextMIME,
            "class" => "application/x-java-applet",
            "x" => "application/directx",
            "sh" => "application/x-sh",
            "htc" => "text/x-component",
            "manifest" => "application/x-ms-manifest",

            // Compress
            "zip" => "application/zip",
            "7z" => "application/x-7z-compressed",
            "cab" => "application/vnd.ms-cab-compressed",
            "rar" => "application/x-rar-compressed",
            "tar" => "application/tar",
            "gz" => "application/gzip",
            "br" => "application/brotli",
            "tgz" or "tar.gz" => "application/tar+gzip",
            "xz" or "txz" => "application/x-xz-compressed-tar",
            "bz" or "tbz" => "application/x-bzip1-compressed-tar",
            "bz2" or "tbz2" or "tb2" => "application/x-bzip2-compressed-tar",
            "bz3" or "tbz3" => "application/x-bzip3-compressed-tar",
            "bz4" or "tbz4" => "application/x-bzip4-compressed-tar",
            "z" => "application/x-compress",

            // Others
            "pdf" => "application/pdf",
            "application" => "application/x-ms-application",
            "iso" => "application/x-iso9660-image",
            "123" => "application/vnd.lotus-1-2-3",
            "crd" => "application/x-mscardfile",
            "clp" => "application/x-msclip",
            "jar" => "application/java-archive",
            "dll" or "exe" => "application/vnd.microsoft.portable-executable",
            "pdb" or "bat" or "msi" or "msu" or "com" => "application/x-msdownload",
            "app" or "so" or "a" or "rpm" or "glif" or "resx" or "php" or "jsp" or "cshtml" or "vbhtml" or "razor" or "3mf" or "lib" or "bin" or "dat" or "data" or "db" or "dms" or "lrf" or "pkg" or "dump" or "deploy" or "vso" or "nupkg" or "xsn" or "sln" or "vsix" or "ts" or "tsx" or "usr" or "user" or "bson" or "aaf" or "aca" or "afm" or "deploy" or "dsp" or "mdp" or "xtp" or "xsn" => StreamMIME,
            "obj" => "model/obj",
            "appx" => "application/appx",
            "appxbundle" => "application/appxbundle",
            "appinstaller" => "application/appinstaller",
            "msix" => "application/msix",
            "msixbundle" => "application/msixbundle",
            "apk" or "aab" => "application/vnd.android.package-archive",
            "cdf" => "application/x-cdf",
            "lnk" => "application/x-ms-shortcut",
            "deb" => "application/vnd.debian.binary-package",
            "o" => "application/x-object",
            "wmz" => "application/x-ms-wmz",
            "hhc" => "application/x-oleobject",
            "scd" => "application/x-msschedule",
            "rtf" => "application/rtf",
            "iii" => "application/x-iphone",
            "asd" or "asi" or "dwp" or "hhp" or "toc" or "fla" or "hhk" or "inf" or "jpb" or "lpk" or "lzh" or "mix" or "mso" or "psm" or "prx" or "prm" or "psp" or "ocx" or "pcx" or "pcz" or "smi" => StreamMIME,
            "pqa" or "oprc" => "application/vnd.palm",
            "swf" => "application/x-shockwave-flash",
            "t" => "application/x-troff",
            "tex" => "application/x-tex",
            "texi" or "texinfo" => "application/x-texinfo",
            "tr" => "application/x-troff",
            "trm" => "application/x-msterminal",
            "ustar" => "application/x-ustar",
            "wmd" => "application/x-ms-wmd",
            "xap" => "application/x-silverlight-app",
            "xbap" => "application/x-ms-xbap",
            "mts" => "model/vnd.mts",
            "mtl" => "model/mtl",
            "fbx" => "application/fbx",
            _ => null
        };
    }
}
