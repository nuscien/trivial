using System;

namespace Trivial.Web
{
    /// <summary>
    /// The MIME constants.
    /// </summary>
    public static partial class MimeConstants
    {
        /// <summary>
        /// The popular MIME constants of package.
        /// </summary>
        public static class Packages
        {
            /// <summary>
            /// The MIME of Windows application program package.
            /// </summary>
            public const string Application = "application/x-ms-application";

            /// <summary>
            /// The MIME of ISO 9660 (disc image).
            /// </summary>
            public const string Iso = "application/x-iso9660-image";

            /// <summary>
            /// The MIME of Microsoft file required downloading.
            /// </summary>
            public const string DownloadToRun = "application/x-msdownload";

            /// <summary>
            /// The MIME of Windows App package.
            /// </summary>
            public const string Appx = "application/appx";

            /// <summary>
            /// The MIME of Windows App bundle package.
            /// </summary>
            public const string AppxBundle = "application/appxbundle";

            /// <summary>
            /// The MIME of Windows App installer.
            /// </summary>
            public const string AppInstaller = "application/appinstaller";

            /// <summary>
            /// The MIME of Microsoft Installer.
            /// </summary>
            public const string Msix = "application/msix";

            /// <summary>
            /// The MIME of Microsoft Installer bundler.
            /// </summary>
            public const string MsixBundle = "application/msixbundle";

            /// <summary>
            /// The MIME of Android App Package.
            /// </summary>
            public const string Apk = "application/vnd.android.package-archive";

            /// <summary>
            /// The MIME of object.
            /// </summary>
            public const string O = "application/x-object";

            /// <summary>
            /// The MIME of object.
            /// </summary>
            public const string Obj = "application/x-tgif";

            /// <summary>
            /// The MIME of Zip.
            /// </summary>
            public const string Zip = "application/zip";

            /// <summary>
            /// The MIME of 7z.
            /// </summary>
            public const string SevenZip = "application/x-7z-compressed";

            /// <summary>
            /// The MIME of cab.
            /// </summary>
            public const string Cab = "application/vnd.ms-cab-compressed";

            /// <summary>
            /// The MIME of WinRAR.
            /// </summary>
            public const string Rar = "application/x-rar-compressed";

            /// <summary>
            /// The MIME of Tape Archive.
            /// </summary>
            public const string Tar = "application/tar";

            /// <summary>
            /// The MIME of GNU Zip.
            /// </summary>
            public const string Gz = "application/gzip";

            /// <summary>
            /// The MIME of brotli.
            /// </summary>
            public const string Brotli = "application/brotli";

            /// <summary>
            /// The MIME of Tape Archive on GNU Zip.
            /// </summary>
            public const string Tgz = "application/tar+gzip";

            /// <summary>
            /// The MIME of z compress file.
            /// </summary>
            public const string Z = "application/x-compress";
        }
    }
}
