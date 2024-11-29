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
    /// The Esperanto alphabet.
    /// </summary>
    public static class Esperanto
    {
        /// <summary>
        /// The name of Esperanto alphabet in Esperanto.
        /// </summary>
        public const string FullName = "Esperanto Alphabet";

        /// <summary>
        /// The name of Esperanto in Esperanto.
        /// </summary>
        public const string LanguageName = "Esperanto";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "epo";

        /// <summary>
        /// The count of the letter.
        /// </summary>
        public const int Count = 28;

        /// <summary>
        /// The capital letter cx (/tʃ/).
        /// </summary>
        public const string CapitalCx = "Ĉ";

        /// <summary>
        /// The small letter cx (/tʃ/).
        /// </summary>
        public const string SmallCx = "ĉ";

        /// <summary>
        /// The capital letter gx (/dʒ/).
        /// </summary>
        public const string CapitalGx = "Ĝ";

        /// <summary>
        /// The small letter gx (/dʒ/).
        /// </summary>
        public const string SmallGx = "ĝ";

        /// <summary>
        /// The capital letter hx (/x/).
        /// </summary>
        public const string CapitalHx = "Ĥ";

        /// <summary>
        /// The small letter hx (/x/).
        /// </summary>
        public const string SmallHx = "ĥ";

        /// <summary>
        /// The capital letter jx (/ʒ/).
        /// </summary>
        public const string CapitalJx = "Ĵ";

        /// <summary>
        /// The small letter jx (/ʒ/).
        /// </summary>
        public const string SmallJx = "ĵ";

        /// <summary>
        /// The capital letter sx (/ʃ/).
        /// </summary>
        public const string CapitalSx = "Ŝ";

        /// <summary>
        /// The small letter sx (/ʃ/).
        /// </summary>
        public const string SmallSx = "ŝ";

        /// <summary>
        /// The capital letter ux (/w/).
        /// </summary>
        public const string CapitalUx = "Ŭ";

        /// <summary>
        /// The small letter ux (/w/).
        /// </summary>
        public const string SmallUx = "ŭ";

        /// <summary>
        /// The number 0.
        /// </summary>
        public const string Zero = "nul";

        /// <summary>
        /// The number 1.
        /// </summary>
        public const string One = "unu";

        /// <summary>
        /// The number 2.
        /// </summary>
        public const string Two = "du";

        /// <summary>
        /// The number 3.
        /// </summary>
        public const string Three = "tri";

        /// <summary>
        /// The number 4.
        /// </summary>
        public const string Four = "kvar";

        /// <summary>
        /// The number 5.
        /// </summary>
        public const string Five = "kvin";

        /// <summary>
        /// The number 6.
        /// </summary>
        public const string Six = "ses";

        /// <summary>
        /// The number 7.
        /// </summary>
        public const string Seven = "sep";

        /// <summary>
        /// The number 8.
        /// </summary>
        public const string Eight = "ok";

        /// <summary>
        /// The number 9.
        /// </summary>
        public const string Nine = "naŭ";

        /// <summary>
        /// The number 10.
        /// </summary>
        public const string Ten = "dek";

        /// <summary>
        /// Lists the capital letters.
        /// </summary>
        /// <returns>All captial letters.</returns>
        public static IEnumerable<string> CapitalLetters()
        {
            yield return CapitalA;
            yield return CapitalB;
            yield return CapitalC;
            yield return CapitalCx;
            yield return CapitalD;
            yield return CapitalE;
            yield return CapitalF;
            yield return CapitalG;
            yield return CapitalGx;
            yield return CapitalH;
            yield return CapitalHx;
            yield return CapitalI;
            yield return CapitalJ;
            yield return CapitalJx;
            yield return CapitalK;
            yield return CapitalL;
            yield return CapitalM;
            yield return CapitalN;
            yield return CapitalO;
            yield return CapitalP;
            yield return CapitalR;
            yield return CapitalS;
            yield return CapitalSx;
            yield return CapitalT;
            yield return CapitalU;
            yield return CapitalUx;
            yield return CapitalV;
            yield return CapitalZ;
        }

        /// <summary>
        /// Lists the small letters.
        /// </summary>
        /// <returns>All small letters.</returns>
        public static IEnumerable<string> SmallLetters()
        {
            yield return SmallA;
            yield return SmallB;
            yield return SmallC;
            yield return SmallCx;
            yield return SmallD;
            yield return SmallE;
            yield return SmallF;
            yield return SmallG;
            yield return SmallGx;
            yield return SmallH;
            yield return SmallHx;
            yield return SmallI;
            yield return SmallJ;
            yield return SmallJx;
            yield return SmallK;
            yield return SmallL;
            yield return SmallM;
            yield return SmallN;
            yield return SmallO;
            yield return SmallP;
            yield return SmallR;
            yield return SmallS;
            yield return SmallSx;
            yield return SmallT;
            yield return SmallU;
            yield return SmallUx;
            yield return SmallV;
            yield return SmallZ;
        }
    }
}
