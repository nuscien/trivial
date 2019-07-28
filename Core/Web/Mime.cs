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
        /// Gets the MIME value of JSON format text.
        /// </summary>
        public const string JsonMIME = "application/json";

        /// <summary>
        /// Gets the MIME value of JavaScript (ECMAScript) format text.
        /// </summary>
        public const string JavaScriptMIME = "text/javascript";

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
        /// Gets or sets the MIME handler.
        /// </summary>
        internal static Func<FileInfo, string> GetMimeHandler { get; set; }

        /// <summary>
        /// Gets the MIME.
        /// </summary>
        /// <param name="file">The file information.</param>
        /// <returns>A MIME value.</returns>
        /// <remarks>This just contains the most useful MIMEs.</remarks>
        internal static string GetMime(FileInfo file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.Extension)) return null;
            var h = GetMimeHandler;
            if (h != null)
            {
                var result = h(file);
                if (!string.IsNullOrWhiteSpace(result)) return result;
            }

            switch (file.Extension.ToLowerInvariant().Remove(0))
            {
                // Office document
                case "cat":
                    return "application/vnd.ms-pki.seccat";
                case "doc":
                    return "application/msword";
                case "docm":
                    return "application/vnd.ms-word.document.macroenabled.12";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "dotx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case "mda":
                case "mdb":
                case "mde":
                case "accdb":
                    return "application/x-msaccess";
                case "mpp":
                case "mpt":
                    return "application/vnd.ms-project";
                case "ppt":
                    return "application/vnd.ms-powerpoint";
                case "pptm":
                    return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case "pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "ppsx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case "sldx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case "potx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case "xls":
                    return "application/vnd.ms-excel";
                case "xlsm":
                    return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xltx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case "onetoc":
                case "onetok2":
                case "onetmp":
                case "onepkg":
                    return "application/onenote";
                case "xps":
                    return "application/vnd.ms-xpsdocument";
                case "oxps":
                    return "application/oxps";
                case "vsd":
                case "vst":
                case "vss":
                case "vsw":
                    return "application/vnd.visio";

                // Open Document
                case "odc":
                    return "application/vnd.oasis.opendocument.chart";
                case "otc":
                    return "application/vnd.oasis.opendocument.chart-template";
                case "odb":
                    return "application/vnd.oasis.opendocument.database";
                case "odf":
                    return "application/vnd.oasis.opendocument.formula";
                case "odft":
                    return "application/vnd.oasis.opendocument.formula-template";
                case "odg":
                    return "application/vnd.oasis.opendocument.graphics";
                case "otg":
                    return "application/vnd.oasis.opendocument.graphics-template";
                case "odi":
                    return "application/vnd.oasis.opendocument.image";
                case "oti":
                    return "application/vnd.oasis.opendocument.image-template";
                case "odp":
                    return "application/vnd.oasis.opendocument.presentation";
                case "otp":
                    return "application/vnd.oasis.opendocument.presentation-template";
                case "ods":
                    return "application/vnd.oasis.opendocument.spreadsheet";
                case "ots":
                    return "application/vnd.oasis.opendocument.spreadsheet-template";
                case "odt":
                    return "application/vnd.oasis.opendocument.text";
                case "odm":
                    return "application/vnd.oasis.opendocument.text-master";
                case "ott":
                    return "application/vnd.oasis.opendocument.text-template";
                case "oth":
                    return "application/vnd.oasis.opendocument.text-web";

                // Image
                case "apng":
                    return "image/apng";
                case "png":
                    return "image/png";
                case "bmp":
                case "dib":
                    return "image/bmp";
                case "dwg":
                    return "image/vnd.dwg";
                case "gif":
                    return "image/gif";
                case "heic":
                    return "image/heic";
                case "ico":
                case "cur":
                    return "image/x-icon";
                case "jpe":
                case "jpeg":
                case "jpg":
                case "jfif":
                    return "image/jpeg";
                case "psd":
                    return "image/vnd.adobe.photoshop";
                case "tif":
                case "tiff":
                    return "image/tiff";
                case "svg":
                case "svgz":
                    return "image/svg+xml";
                case "webp":
                    return "image/webp";
                case "wmf":
                case "emf":
                case "emz":
                    return "application/x-msmetafile";

                // Audio
                case "au":
                case "snd":
                    return "audio/basic";
                case "mid":
                case "midi":
                case "kar":
                case "rmi":
                    return "audio/midi";
                case "m4a":
                case "mp4a":
                    return "audio/mp4";
                case "mp3":
                case "mpga":
                case "mp2":
                case "mp2a":
                case "m2a":
                case "m3a":
                    return "audio/mpeg";
                case "ogg":
                case "oga":
                case "spx":
                    return "audio/ogg";
                case "pya":
                    return "audio/vnd.ms-playready.media.pya";
                case "weba":
                    return "audio/webm";
                case "aac":
                    return "audio/x-aac";
                case "aif":
                case "aiff":
                case "aifc":
                    return "audio/x-aiff";
                case "flac":
                    return "audio/x-flac";
                case "mka":
                    return "audio/x-matroska";
                case "m3u":
                    return "audio/x-mpegurl";
                case "wax":
                    return "audio/x-ms-wax";
                case "wma":
                    return "audio/x-ms-wma";
                case "wav":
                case "wave":
                    return "audio/wav";

                // Video
                case "3gp":
                case "3gpp":
                    return "video/3gpp";
                case "3g2":
                case "3gp2":
                    return "video/3gpp2";
                case "avi":
                    return "video/x-msvideo";
                case "h261":
                    return "video/h261";
                case "h263":
                    return "video/h263";
                case "h264":
                    return "video/h264";
                case "h265":
                    return "video/h265";
                case "h266":
                    return "video/h266";
                case "jpgv":
                    return "video/jpeg";
                case "jpm":
                case "jpgm":
                    return "video/jpm";
                case "mp4":
                case "mp4v":
                case "mpg4":
                    return "video/mp4";
                case "mpeg":
                case "mpg":
                case "mpe":
                case "m1v":
                case "m2v":
                    return "video/mpeg";
                case "ogv":
                    return "video/ogg";
                case "mov":
                    return "video/quicktime";
                case "dvd":
                    return "video/vnd.dvb.file";
                case "mxu":
                case "m4u":
                    return "video/vnd.mpegurl";
                case "pyv":
                    return "video/vnd.ms-playready.media.pyv";
                case "webm":
                    return "video/webm";
                case "flv":
                    return "video/x-flv";
                case "m4v":
                    return "video/x-m4v";
                case "mkv":
                case "mk3d":
                case "mks":
                    return "video/x-matroska";
                case "asf":
                case "asx":
                    return "video/x-ms-asf";
                case "wm":
                    return "video/x-ms-wm";
                case "wmv":
                    return "video/x-ms-wm";
                case "wmx":
                    return "video/x-ms-wmx";

                // Text
                case "txt":
                case "text":
                case "log":
                case "def":
                    return "text/plain";
                case "csv":
                    return "text/csv";
                case "md":
                    return "text/markdown";
                case "mml":
                    return "text/mathml";
                case "rtx":
                    return "text/richtext";
                case "sgml":
                case "sgm":
                    return "text/sgml";
                case "vcf":
                    return "text/x-vcard";
                case "vtt":
                    return "text/vtt";

                // Web
                case "ac":
                    return "application/pkix-attr-cert";
                case "cer":
                    return "application/pkix-cert";
                case "css":
                    return CssMIME;
                case "crl":
                    return "application/pkix-crl";
                case "dtd":
                    return "application/xml-dtd";
                case "ecma":
                    return "application/ecmascript";
                case "epub":
                    return "application/epub+zip";
                case "htm":
                case "html":
                case "shtml":
                    return HtmlMIME;
                case "ink":
                case "inkml":
                    return "application/inkml+xml";
                case "js":
                    return JavaScriptMIME;
                case "json":
                case "map":
                    return JsonMIME;
                case "p8":
                    return "application/pkcs8";
                case "pem":
                    return "application/x-x509-ca-cert";
                case "pki":
                    return "application/pkixcmp";
                case "pkipath":
                    return "application/pkix-pkipath";
                case "uri":
                case "uris":
                case "urls":
                    return "text/uri-list";
                case "xml":
                case "xsl":
                    return "application/xml";
                case "vbs":
                    return "text/vbscript";

                // Font
                case "eot":
                    return "application/vnd.ms-fontobject";
                case "pcf":
                    return "application/x-font-pcf";
                case "pfr":
                    return "application/font-tdpfr";
                case "snf":
                    return "application/x-font-snf";
                case "ttf":
                case "ttc":
                    return "application/x-font-ttf";
                case "otf":
                    return "application/x-font-otf";
                case "pfa":
                case "pfb":
                case "pfm":
                case "afm":
                    return "application/x-font-type1";
                case "woff":
                    return "application/font-woff";

                // Programming
                case "c":
                case "cc":
                case "cpp":
                case "cxx":
                case "h":
                case "hh":
                case "dic":
                    return "text/x-c";
                case "java":
                    return "text/x-java-source";
                case "cs":
                case "csproj":
                case "csdproj":
                case "vb":
                case "vbproj":
                case "vbdproj":
                    return "text/plain";
                case "py":
                case "py2":
                case "py3":
                case "pyw":
                    return "text/x-python";
                case "settings":
                    return "application/xml";
                case "sql":
                    return "application/x-sql";
                case "ps1":
                    return "text/plain";

                // Others
                case "application":
                    return "application/x-ms-application";
                case "iso":
                    return "application/x-iso9660-image";
                case "ics":
                case "ifb":
                    return "text/calendar";
                case "pdf":
                    return "application/pdf";
                case "crd":
                    return "application/x-mscardfile";
                case "clp":
                    return "application/x-msclip";
                case "jar":
                    return "application/java-archive";
                case "dll":
                case "exe":
                case "bat":
                case "msi":
                case "msu":
                case "com":
                    return "application/x-msdownload";
                case "3mf":
                case "bin":
                case "dat":
                case "data":
                case "db":
                case "dms":
                case "lrf":
                case "msix":
                case "pkg":
                case "dump":
                case "deploy":
                case "vso":
                case "nupkg":
                case "xsn":
                    return "application/octet-stream";
                case "pdb":
                case "pqa":
                case "oprc":
                    return "application/vnd.palm";
                case "lnk":
                    return "application/x-ms-shortcut";
                case "zip":
                    return "application/zip";
                case "7z":
                    return "application/x-7z-compressed";
                case "cab":
                    return "application/vnd.ms-cab-compressed";
                case "rar":
                    return "application/x-rar-compressed";
                case "tar":
                    return "application/tar";
                case "gz":
                    return "application/gzip";
                case "tgz":
                case "tar.gz":
                    return "application/tar+gzip";
                case "z":
                    return "application/x-compress";
                default:
                    return null;
            }
        }
    }
}
