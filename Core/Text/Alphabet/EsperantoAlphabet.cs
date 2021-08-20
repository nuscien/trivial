using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// The Esperanto alphabet.
    /// </summary>
    public static class EsperantoAlphabet
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
        /// The capital letter a (/a/).
        /// </summary>
        public const string CapitalA = "A";

        /// <summary>
        /// The small letter a (/a/).
        /// </summary>
        public const string SmallA = "a";

        /// <summary>
        /// The capital letter b (/b/).
        /// </summary>
        public const string CapitalB = "B";

        /// <summary>
        /// The small letter b (/b/).
        /// </summary>
        public const string SmallB = "b";

        /// <summary>
        /// The capital letter c (/ts/).
        /// </summary>
        public const string CapitalC = "C";

        /// <summary>
        /// The small letter c (/ts/).
        /// </summary>
        public const string SmallC = "c";

        /// <summary>
        /// The capital letter cx (/tʃ/).
        /// </summary>
        public const string CapitalCx = "Ĉ";

        /// <summary>
        /// The small letter cx (/tʃ/).
        /// </summary>
        public const string SmallCx = "ĉ";

        /// <summary>
        /// The capital letter d (/d/).
        /// </summary>
        public const string CapitalD = "D";

        /// <summary>
        /// The small letter d (/d/).
        /// </summary>
        public const string SmallD = "d";

        /// <summary>
        /// The capital letter e (/e/).
        /// </summary>
        public const string CapitalE = "E";

        /// <summary>
        /// The small letter e (/e/).
        /// </summary>
        public const string SmallE = "e";

        /// <summary>
        /// The capital letter f (/f/).
        /// </summary>
        public const string CapitalF = "F";

        /// <summary>
        /// The small letter f (/f/).
        /// </summary>
        public const string SmallF = "f";

        /// <summary>
        /// The capital letter g (/g/).
        /// </summary>
        public const string CapitalG = "G";

        /// <summary>
        /// The small letter g (/g/).
        /// </summary>
        public const string SmallG = "g";

        /// <summary>
        /// The capital letter gx (/dʒ/).
        /// </summary>
        public const string CapitalGx = "Ĝ";

        /// <summary>
        /// The small letter gx (/dʒ/).
        /// </summary>
        public const string SmallGx = "ĝ";

        /// <summary>
        /// The capital letter h (/h/).
        /// </summary>
        public const string CapitalH = "H";

        /// <summary>
        /// The small letter h (/h/).
        /// </summary>
        public const string SmallH = "h";

        /// <summary>
        /// The capital letter hx (/x/).
        /// </summary>
        public const string CapitalHx = "Ĥ";

        /// <summary>
        /// The small letter hx (/x/).
        /// </summary>
        public const string SmallHx = "ĥ";

        /// <summary>
        /// The capital letter i (/i/).
        /// </summary>
        public const string CapitalI = "I";

        /// <summary>
        /// The small letter i (/i/).
        /// </summary>
        public const string SmallI = "i";

        /// <summary>
        /// The capital letter j (/j/).
        /// </summary>
        public const string CapitalJ = "J";

        /// <summary>
        /// The small letter j (/j/).
        /// </summary>
        public const string SmallJ = "j";

        /// <summary>
        /// The capital letter jx (/ʒ/).
        /// </summary>
        public const string CapitalJx = "Ĵ";

        /// <summary>
        /// The small letter jx (/ʒ/).
        /// </summary>
        public const string SmallJx = "ĵ";

        /// <summary>
        /// The capital letter k (/k/).
        /// </summary>
        public const string CapitalK = "K";

        /// <summary>
        /// The small letter k (/k/).
        /// </summary>
        public const string SmallK = "k";

        /// <summary>
        /// The capital letter l (/l/).
        /// </summary>
        public const string CapitalL = "L";

        /// <summary>
        /// The small letter l (/l/).
        /// </summary>
        public const string SmallL = "l";

        /// <summary>
        /// The capital letter m (/m/).
        /// </summary>
        public const string CapitalM = "M";

        /// <summary>
        /// The small letter m (/m/).
        /// </summary>
        public const string SmallM = "m";

        /// <summary>
        /// The capital letter n (/n/).
        /// </summary>
        public const string CapitalN = "N";

        /// <summary>
        /// The small letter n (/n/).
        /// </summary>
        public const string SmallN = "n";

        /// <summary>
        /// The capital letter o (/o/).
        /// </summary>
        public const string CapitalO = "O";

        /// <summary>
        /// The small letter o (/o/).
        /// </summary>
        public const string SmallO = "o";

        /// <summary>
        /// The capital letter p (/p/).
        /// </summary>
        public const string CapitalP = "P";

        /// <summary>
        /// The small letter p (/p/).
        /// </summary>
        public const string SmallP = "p";

        /// <summary>
        /// The capital letter r (/r/).
        /// </summary>
        public const string CapitalR = "R";

        /// <summary>
        /// The small letter r (/r/).
        /// </summary>
        public const string SmallR = "r";

        /// <summary>
        /// The capital letter s (/s/).
        /// </summary>
        public const string CapitalS = "S";

        /// <summary>
        /// The small letter s (/s/).
        /// </summary>
        public const string SmallS = "s";

        /// <summary>
        /// The capital letter sx (/ʃ/).
        /// </summary>
        public const string CapitalSx = "Ŝ";

        /// <summary>
        /// The small letter sx (/ʃ/).
        /// </summary>
        public const string SmallSx = "ŝ";

        /// <summary>
        /// The capital letter t (/t/).
        /// </summary>
        public const string CapitalT = "T";

        /// <summary>
        /// The small letter t (/t/).
        /// </summary>
        public const string SmallT = "t";

        /// <summary>
        /// The capital letter u (/u/).
        /// </summary>
        public const string CapitalU = "U";

        /// <summary>
        /// The small letter u (/u/).
        /// </summary>
        public const string SmallU = "u";

        /// <summary>
        /// The capital letter ux (/w/).
        /// </summary>
        public const string CapitalUx = "Ŭ";

        /// <summary>
        /// The small letter ux (/w/).
        /// </summary>
        public const string SmallUx = "ŭ";

        /// <summary>
        /// The capital letter v (/v/).
        /// </summary>
        public const string CapitalV = "V";

        /// <summary>
        /// The small letter v (/v/).
        /// </summary>
        public const string SmallV = "v";

        /// <summary>
        /// The capital letter z (/z/).
        /// </summary>
        public const string CapitalZ = "Z";

        /// <summary>
        /// The small letter z (/z/).
        /// </summary>
        public const string SmallZ = "z";

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
