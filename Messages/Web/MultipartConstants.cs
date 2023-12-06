using System;

namespace Trivial.Web;

/// <summary>
/// The MIME constants.
/// </summary>
public static partial class MimeConstants
{
    /// <summary>
    /// The popular MIME constants of multipart.
    /// </summary>
    public static class Multipart
    {
        /// <summary>
        /// The MIME content type of form data.
        /// </summary>
        public const string FormDataMIME = "multipart/form-data";

        /// <summary>
        /// The MIME content type of byte ranges.
        /// </summary>
        public const string ByteRanges = "multipart/byteranges";

        /// <summary>
        /// The MIME content type of encrypted.
        /// </summary>
        public const string Encrypted = "multipart/encrypted";

        /// <summary>
        /// The MIME content type of multilingual.
        /// </summary>
        public const string Multilingual = "multipart/multilingual";

        /// <summary>
        /// The MIME content type of signed.
        /// </summary>
        public const string Signed = "multipart/signed";

        /// <summary>
        /// The MIME content type of voice message.
        /// </summary>
        public const string VoiceMessage = "multipart/voice-message";

        /// <summary>
        /// The MIME content type of mixed replace.
        /// </summary>
        public const string MixedReplace = "multipart/x-mixed-replace";
    }
}
