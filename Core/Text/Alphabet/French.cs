using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The Latin alphabet (a.k.a Roman alphabet) which is used in English, French, Italian, etc.
/// </summary>
public static partial class LatinAlphabet
{
    /// <summary>
    /// Frensh (Français) alphabet.
    /// </summary>
    public static class French
    {
        /// <summary>
        /// The name of French in French.
        /// </summary>
        public const string LanguageName = "Français";

        /// <summary>
        /// The language name in English.
        /// </summary>
        public const string EnglishLanguageName = "French";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "fr";

        /// <summary>
        /// The count of the letter.
        /// </summary>
        public const int Count = 26;

        /// <summary>
        /// The capital letter a-grave.
        /// </summary>
        public const string CapitalAGrave = "À";

        /// <summary>
        /// The small letter a-grave.
        /// </summary>
        public const string SmallAGrave = "à";

        /// <summary>
        /// The capital letter a-circumflex.
        /// </summary>
        public const string CapitalACircumflex = "Â";

        /// <summary>
        /// The small letter a-circumflex.
        /// </summary>
        public const string SmallACircumflex = "â";

        /// <summary>
        /// The capital letter ae.
        /// </summary>
        public const string CapitalAe = "Æ";

        /// <summary>
        /// The small letter ae.
        /// </summary>
        public const string SmallAe = "æ";

        /// <summary>
        /// The capital letter c-cedilla.
        /// </summary>
        public const string CapitalCCedilla = "Ç";

        /// <summary>
        /// The small letter c-cedilla.
        /// </summary>
        public const string SmallCCedilla = "ç";

        /// <summary>
        /// The capital letter e-acute.
        /// </summary>
        public const string CapitalEAcute = "É";

        /// <summary>
        /// The small letter e-acute.
        /// </summary>
        public const string SmallEAcute = "é";

        /// <summary>
        /// The capital letter e-grave.
        /// </summary>
        public const string CapitalEGrave = "È";

        /// <summary>
        /// The small letter e-grave.
        /// </summary>
        public const string SmallEGrave = "è";

        /// <summary>
        /// The capital letter e-circumflex.
        /// </summary>
        public const string CapitalECircumflex = "Ê";

        /// <summary>
        /// The small letter e-circumflex.
        /// </summary>
        public const string SmallECircumflex = "ê";

        /// <summary>
        /// The capital letter e-diaeresis.
        /// </summary>
        public const string CapitalEDiaeresis = "Ë";

        /// <summary>
        /// The small letter e-diaeresis.
        /// </summary>
        public const string SmallEDiaeresis = "ë";

        /// <summary>
        /// The capital letter i-circumflex.
        /// </summary>
        public const string CapitalICircumflex = "Î";

        /// <summary>
        /// The small letter i-circumflex.
        /// </summary>
        public const string SmallICircumflex = "î";

        /// <summary>
        /// The capital letter i-diaeresis.
        /// </summary>
        public const string CapitalIDiaeresis = "Ï";

        /// <summary>
        /// The small letter i-diaeresis.
        /// </summary>
        public const string SmallIDiaeresis = "ï";

        /// <summary>
        /// The capital letter o-circumflex.
        /// </summary>
        public const string CapitalOCircumflex = "Ô";

        /// <summary>
        /// The small letter o-circumflex.
        /// </summary>
        public const string SmallOCircumflex = "ô";

        /// <summary>
        /// The capital letter oe.
        /// </summary>
        public const string CapitalOe = "Œ";

        /// <summary>
        /// The small letter oe.
        /// </summary>
        public const string SmallOe = "œ";

        /// <summary>
        /// The capital letter u-grave.
        /// </summary>
        public const string CapitalUGrave = "Ù";

        /// <summary>
        /// The small letter u-grave.
        /// </summary>
        public const string SmallUGrave = "ù";

        /// <summary>
        /// The capital letter u-circumflex.
        /// </summary>
        public const string CapitalUCircumflex = "Û";

        /// <summary>
        /// The small letter u-circumflex.
        /// </summary>
        public const string SmallUCircumflex = "û";

        /// <summary>
        /// The capital letter u-diaeresis.
        /// </summary>
        public const string CapitalUDiaeresis = "Ü";

        /// <summary>
        /// The small letter u-diaeresis.
        /// </summary>
        public const string SmallUDiaeresis = "ü";

        /// <summary>
        /// The capital letter y-diaeresis.
        /// </summary>
        public const string CapitalYDiaeresis = "Ÿ";

        /// <summary>
        /// The small letter y-diaeresis.
        /// </summary>
        public const string SmallYDiaeresis = "ÿ";

        /// <summary>
        /// Lists the capital letters.
        /// </summary>
        /// <returns>All captial letters.</returns>
        public static IEnumerable<string> CapitalLetters()
            => LatinAlphabet.CapitalLetters();

        /// <summary>
        /// Lists the small letters.
        /// </summary>
        /// <returns>All small letters.</returns>
        public static IEnumerable<string> SmallLetters()
            => LatinAlphabet.SmallLetters();
    }
}
