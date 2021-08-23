using System;

namespace Trivial.Web
{
    /// <summary>
    /// The MIME constants.
    /// </summary>
    public static partial class MimeConstants
    {
        /// <summary>
        /// The popular MIME constants of text.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// The MIME of plain text.
            /// </summary>
            public const string Plain = "text/plain";

            /// <summary>
            /// The MIME of rich text.
            /// </summary>
            public const string Rtf = "text/richtext";

            /// <summary>
            /// The MIME of comma-separated values.
            /// </summary>
            public const string Csv = "text/csv";

            /// <summary>
            /// The MIME of tab-separated values.
            /// </summary>
            public const string Tsv = "text/tsv";

            /// <summary>
            /// The MIME of vCard (Versitcard).
            /// </summary>
            public const string VCard = "text/x-vcard";

            /// <summary>
            /// The MIME of diff and patch.
            /// </summary>
            public const string Diff = "text/x-diff";

            /// <summary>
            /// The MIME of markdown.
            /// </summary>
            public const string Markdown = "text/markdown";

            /// <summary>
            /// The MIME of math markup.
            /// </summary>
            public const string Math = "text/mathml";

            /// <summary>
            /// The MIME of Standard Generalized Markup Language.
            /// </summary>
            public const string Sgml = "text/sgml";

            /// <summary>
            /// The MIME of ink markup.
            /// </summary>
            public const string Ink = "application/inkml+xml";

            /// <summary>
            /// The MIME of iCalendar.
            /// </summary>
            public const string Calendar = "text/calendar";
        }
    }
}
