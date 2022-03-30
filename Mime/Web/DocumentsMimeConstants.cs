using System;

namespace Trivial.Web;

/// <summary>
/// The MIME constants.
/// </summary>
public static partial class MimeConstants
{
    /// <summary>
    /// The popular MIME constants of documents.
    /// </summary>
    public static class Documents
    {
        /// <summary>
        /// The MIME content type of Portable Document Format.
        /// </summary>
        public const string Pdf = "application/pdf";

        /// <summary>
        /// The MIME content type of ePub.
        /// </summary>
        public const string EPub = "application/epub+zip";

        /// <summary>
        /// The MIME content type of lotus 123.
        /// </summary>
        public const string Lotus123 = "vnd.lotus-1-2-3";

        /// <summary>
        /// The MIME content type of Microsoft Office Clip.
        /// </summary>
        public const string OfficeClip = "application/x-msclip";

        /// <summary>
        /// The MIME content type of Microsoft card file.
        /// </summary>
        public const string Card = "application/x-mscardfile";

        /// <summary>
        /// The MIME content type of Microsoft Office Word old binary document.
        /// </summary>
        public const string Doc = "application/msword";

        /// <summary>
        /// The MIME content type of Microsoft Office Word document.
        /// </summary>
        public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// The MIME content type of Microsoft Office Word template.
        /// </summary>
        public const string Dotx = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";

        /// <summary>
        /// The MIME content type of Microsoft Office Excel old binary document.
        /// </summary>
        public const string Xls = "application/vnd.ms-excel";

        /// <summary>
        /// The MIME content type of Microsoft Office Excel document.
        /// </summary>
        public const string Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// The MIME content type of Microsoft Office Excel template.
        /// </summary>
        public const string Xltx = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";

        /// <summary>
        /// The MIME content type of Microsoft Office PowerPoint old binary document.
        /// </summary>
        public const string Ppt = "application/vnd.ms-powerpoint";

        /// <summary>
        /// The MIME content type of Microsoft Office PowerPoint document.
        /// </summary>
        public const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        /// <summary>
        /// The MIME content type of Microsoft Office PowerPoint SlideShow.
        /// </summary>
        public const string Ppsx = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";

        /// <summary>
        /// The MIME content type of Microsoft Office PowerPoint slide.
        /// </summary>
        public const string Sldx = "application/vnd.openxmlformats-officedocument.presentationml.slide";

        /// <summary>
        /// The MIME content type of Microsoft Office PowerPoint template.
        /// </summary>
        public const string Potx = "application/vnd.openxmlformats-officedocument.presentationml.template";

        /// <summary>
        /// The MIME content type of Microsoft Office OneNote document.
        /// </summary>
        public const string OneNote = "application/onenote";

        /// <summary>
        /// The MIME content type of Microsoft Office Visio document.
        /// </summary>
        public const string Visio = "application/vnd.visio";

        /// <summary>
        /// The MIME content type of Microsoft Office Project document.
        /// </summary>
        public const string Project = "application/vnd.ms-project";

        /// <summary>
        /// The MIME content type of Microsoft Office Access database.
        /// </summary>
        public const string Access = "application/x-msaccess";

        /// <summary>
        /// The MIME content type of Microsoft Office Outlook file.
        /// </summary>
        public const string Outlook = "application/vnd.ms-outlook";

        /// <summary>
        /// The MIME content type of XML Paper Specification document.
        /// </summary>
        public const string Xps = "application/vnd.ms-xpsdocument";

        /// <summary>
        /// The MIME content type of Open XML Paper Specification document.
        /// </summary>
        public const string Oxps = "application/oxps";

        /// <summary>
        /// The MIME content type of Open Document Chart.
        /// </summary>
        public const string Odc = "application/vnd.oasis.opendocument.chart";

        /// <summary>
        /// The MIME content type of Open Document Chart Template.
        /// </summary>
        public const string Otc = "application/vnd.oasis.opendocument.chart-template";

        /// <summary>
        /// The MIME content type of Open Document Database.
        /// </summary>
        public const string Odb = "application/vnd.oasis.opendocument.database";

        /// <summary>
        /// The MIME content type of Open Document Formula.
        /// </summary>
        public const string Odf = "application/vnd.oasis.opendocument.formula";

        /// <summary>
        /// The MIME content type of Open Document Formula Template.
        /// </summary>
        public const string Odft = "application/vnd.oasis.opendocument.formula-template";

        /// <summary>
        /// The MIME content type of Open Document Graphics.
        /// </summary>
        public const string Odg = "application/vnd.oasis.opendocument.graphics";

        /// <summary>
        /// The MIME content type of Open Document Graphics Template.
        /// </summary>
        public const string Otg = "application/vnd.oasis.opendocument.graphics-template";

        /// <summary>
        /// The MIME content type of Open Document Image.
        /// </summary>
        public const string Odi = "application/vnd.oasis.opendocument.image";

        /// <summary>
        /// The MIME content type of Open Document Image Template.
        /// </summary>
        public const string Oti = "application/vnd.oasis.opendocument.image-template";

        /// <summary>
        /// The MIME content type of Open Document Presentation.
        /// </summary>
        public const string Odp = "application/vnd.oasis.opendocument.presentation";

        /// <summary>
        /// The MIME content type of Open Document Presentation Template.
        /// </summary>
        public const string Otp = "application/vnd.oasis.opendocument.presentation-template";

        /// <summary>
        /// The MIME content type of Open Document Spreadsheet.
        /// </summary>
        public const string Ods = "application/vnd.oasis.opendocument.spreadsheet";

        /// <summary>
        /// The MIME content type of Open Document Spreadsheet Template.
        /// </summary>
        public const string Ots = "application/vnd.oasis.opendocument.spreadsheet-template";

        /// <summary>
        /// The MIME content type of Open Document Text.
        /// </summary>
        public const string Odt = "application/vnd.oasis.opendocument.text";

        /// <summary>
        /// The MIME content type of Open Document Text Master.
        /// </summary>
        public const string Odm = "application/vnd.oasis.opendocument.text-master";

        /// <summary>
        /// The MIME content type of Open Document Text Template.
        /// </summary>
        public const string Ott = "application/vnd.oasis.opendocument.text-template";

        /// <summary>
        /// The MIME content type of Open Document Text Web.
        /// </summary>
        public const string Oth = "application/vnd.oasis.opendocument.text-web";
    }
}
