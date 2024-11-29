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
    /// German (Deutsch) alphabet.
    /// </summary>
    public static class German
    {
        /// <summary>
        /// The name of German in German.
        /// </summary>
        public const string LanguageName = "Deutsch";

        /// <summary>
        /// The language name in English.
        /// </summary>
        public const string EnglishLanguageName = "German";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "de";

        /// <summary>
        /// The count of the letter.
        /// </summary>
        public const int Count = 30;

        /// <summary>
        /// The capital letter ae (/εː/ or /ε/).
        /// </summary>
        public const string CapitalAe = "Ä";

        /// <summary>
        /// The small letter ae (/εː/ or /ε/).
        /// </summary>
        public const string SmallAe = "ä";

        /// <summary>
        /// The capital letter oe (/øː/ or /œ/).
        /// </summary>
        public const string CapitalOe = "Ö";

        /// <summary>
        /// The small letter oe (/øː/ or /œ/).
        /// </summary>
        public const string SmallOe = "ö";

        /// <summary>
        /// The capital letter ue (/yː/ or /ʏ/).
        /// </summary>
        public const string CapitalUe = "Ü";

        /// <summary>
        /// The small letter ue (/yː/ or /ʏ/).
        /// </summary>
        public const string SmallUe = "ü";

        /// <summary>
        /// Lists the capital letters.
        /// </summary>
        /// <returns>All captial letters.</returns>
        public static IEnumerable<string> CapitalLetters()
        {
            yield return CapitalA;
            yield return CapitalB;
            yield return CapitalC;
            yield return CapitalD;
            yield return CapitalE;
            yield return CapitalF;
            yield return CapitalG;
            yield return CapitalH;
            yield return CapitalI;
            yield return CapitalJ;
            yield return CapitalK;
            yield return CapitalL;
            yield return CapitalM;
            yield return CapitalN;
            yield return CapitalO;
            yield return CapitalP;
            yield return CapitalQ;
            yield return CapitalR;
            yield return CapitalS;
            yield return CapitalT;
            yield return CapitalU;
            yield return CapitalV;
            yield return CapitalW;
            yield return CapitalX;
            yield return CapitalY;
            yield return CapitalZ;
            yield return CapitalAe;
            yield return CapitalOe;
            yield return CapitalUe;
            yield return CapitalSs;
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
            yield return SmallD;
            yield return SmallE;
            yield return SmallF;
            yield return SmallG;
            yield return SmallH;
            yield return SmallI;
            yield return SmallJ;
            yield return SmallK;
            yield return SmallL;
            yield return SmallM;
            yield return SmallN;
            yield return SmallO;
            yield return SmallP;
            yield return SmallQ;
            yield return SmallR;
            yield return SmallS;
            yield return SmallT;
            yield return SmallU;
            yield return SmallV;
            yield return SmallW;
            yield return SmallX;
            yield return SmallY;
            yield return SmallZ;
            yield return SmallAe;
            yield return SmallOe;
            yield return SmallUe;
            yield return SmallSs;
        }
    }
}
