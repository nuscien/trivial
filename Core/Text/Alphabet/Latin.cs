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
    /// The name of Latin in Latin.
    /// </summary>
    public const string LanguageName = "Lingua Latīna";

    /// <summary>
    /// The name of Latin in English.
    /// </summary>
    public const string EnglishLanguageName = "Latin";

    /// <summary>
    /// The count of the letter.
    /// </summary>
    public const int Count = 26;

    /// <summary>
    /// The capital letter a
    /// (/aː/ in Latin, /eɪ/ in English, /æ/ in Irish English, /ɑ/ in French, /aː/ in German, /a/ in Spanish &amp; Portuguese &amp; Italian &amp; Esperanto).
    /// </summary>
    public const string CapitalA = "A";

    /// <summary>
    /// The small letter a
    /// (/aː/ in Latin, /eɪ/ in English, /æ/ in Irish English, /ɑ/ in French, /aː/ in German, /a/ in Spanish &amp; Portuguese &amp; Italian &amp; Esperanto).
    /// </summary>
    public const string SmallA = "a";

    /// <summary>
    /// The capital letter ae
    /// (/æʃ/ in Danish &amp; Norwegian &amp; Icelandic, /æ/ in middle English).
    /// </summary>
    public const string CapitalAe = "Æ";

    /// <summary>
    /// The small letter ae
    /// (/æʃ/ in Danish &amp; Norwegian &amp; Icelandic, /æ/ in middle English).
    /// </summary>
    public const string SmallAe = "æ";

    /// <summary>
    /// The capital letter b
    /// (/beː/ in Latin, /biː/ in English, /be/ in French, /beː/ in German, /be/ in Spanish &amp; Portuguese, /bi/ in Italian, /b/ in Esperanto).
    /// </summary>
    public const string CapitalB = "B";

    /// <summary>
    /// The small letter b
    /// (/beː/ in Latin, /biː/ in English, /be/ in French, /beː/ in German, /be/ in Spanish &amp; Portuguese, /bi/ in Italian, /b/ in Esperanto).
    /// </summary>
    public const string SmallB = "b";

    /// <summary>
    /// The capital letter c
    /// (/keː/ in Latin, /siː/ in English, /se/ in French, /tseː/ in German, /se/ in Spanish &amp; Portuguese, /θe/ in Spanish, /tʃi/ in Italian, /ts/ in Esperanto).
    /// </summary>
    public const string CapitalC = "C";

    /// <summary>
    /// The small letter c
    /// (/keː/ in Latin, /siː/ in English, /se/ in French, /tseː/ in German,, /se/ in Spanish &amp; Portuguese, /θe/ in Spanish, /tʃi/ in Italian, /ts/ in Esperanto).
    /// </summary>
    public const string SmallC = "c";

    /// <summary>
    /// The capital letter d
    /// (/deː/ in Latin, /diː/ in English, /de/ in French, /deː/ in German, /de/ in Spanish &amp; Portuguese, /di/ in Italian, /d/ in Esperanto).
    /// </summary>
    public const string CapitalD = "D";

    /// <summary>
    /// The small letter d
    /// (/deː/ in Latin, /diː/ in English, /de/ in French, /deː/ in German, /de/ in Spanish &amp; Portuguese, /di/ in Italian, /d/ in Esperanto).
    /// </summary>
    public const string SmallD = "d";

    /// <summary>
    /// The capital letter eth (/ð/).
    /// </summary>
    public const string CapitalEth = "Ð";

    /// <summary>
    /// The small letter eth (/ð/).
    /// </summary>
    public const string SmallEth = "ð";

    /// <summary>
    /// The capital letter e
    /// (/eː/ in Latin, /iː/ in English, /ə/ in French, /eː/ in German, /e/ in Spanish &amp; Portuguese &amp; Italian, /ɛ/ in Portuguese, /e/ in Esperanto).
    /// </summary>
    public const string CapitalE = "E";

    /// <summary>
    /// The small letter e
    /// (/eː/ in Latin, /iː/ in English, /ə/ in French, /eː/ in German, /e/ in Spanish &amp; Portuguese &amp; Italian, /ɛ/ in Portuguese, /e/ in Esperanto).
    /// </summary>
    public const string SmallE = "e";

    /// <summary>
    /// The capital letter ezh.
    /// </summary>
    public const string CapitalEzh = "Ʒ";

    /// <summary>
    /// The small letter ezh.
    /// </summary>
    public const string SmallEzh = "ʒ";

    /// <summary>
    /// The capital letter f
    /// (/ɛf/ in Latin &amp; English &amp; French &amp; German, /ˈefe/ in Spanish, /ɛfɨ/ in Portuguese, /ˈɛffe/ in Italian, /f/ in Esperanto).
    /// </summary>
    public const string CapitalF = "F";

    /// <summary>
    /// The small letter f
    /// (/ɛf/ in Latin &amp; English &amp; French &amp; German, /ˈefe/ in Spanish, /ɛfɨ/ in Portuguese, /ˈɛffe/ in Italian, /f/ in Esperanto).
    /// </summary>
    public const string SmallF = "f";

    /// <summary>
    /// The capital letter g
    /// (/ɡeː/ in Latin, /dʒiː/ in English, /ʒe/ in French, /ɡeː/ in German, /xe/ in Spanish, /ʒe/ or /ɡe/ Portuguese, /dʒi/ in Italian, /g/ in Esperanto).
    /// </summary>
    public const string CapitalG = "G";

    /// <summary>
    /// The small letter g
    /// (/ɡeː/ in Latin, /dʒiː/ in English, /ʒe/ in French, /ɡeː/ in German, /xe/ in Spanish, /ʒe/ or /ɡe/ Portuguese, /dʒi/ in Italian, /g/ in Esperanto).
    /// </summary>
    public const string SmallG = "g";

    /// <summary>
    /// The capital letter h
    /// (/haː/ in Latin, /eɪtʃ/ in English, /heɪtʃ/ in Irish English, /hetʃ/ in Hinglish, /aʃ/ in French, /haː/ in German, /ˈatʃe/ in Spanish, /ɐˈɡa/ in Portuguese, /ˈakka/ in Italian, /h/ in Esperanto).
    /// </summary>
    public const string CapitalH = "H";

    /// <summary>
    /// The small letter h
    /// (/haː/ in Latin, /eɪtʃ/ in English, /heɪtʃ/ in Irish English, /hetʃ/ in Hinglish, /aʃ/ in French, /haː/ in German, /ˈatʃe/ in Spanish, /ɐˈɡa/ in Portuguese, /ˈakka/ in Italian, /h/ in Esperanto).
    /// </summary>
    public const string SmallH = "h";

    /// <summary>
    /// The capital letter hv.
    /// </summary>
    public const string CapitalHv = "Ƕ";

    /// <summary>
    /// The small letter hv.
    /// </summary>
    public const string SmallHv = "ƕ";

    /// <summary>
    /// The capital letter i
    /// (/iː/ in Latin, /aɪ/ in English, /iː/ in German, /i/ in French &amp; Spanish &amp; Portuguese &amp; Italian, /i/ in Esperanto).
    /// </summary>
    public const string CapitalI = "I";

    /// <summary>
    /// The small letter i
    /// (/iː/ in Latin, /aɪ/ in English, /iː/ in German, /i/ in French &amp; Spanish &amp; Portuguese &amp; Italian, /i/ in Esperanto).
    /// </summary>
    public const string SmallI = "i";

    /// <summary>
    /// The capital letter ij (/jeɪ/ in Dutch).
    /// </summary>
    public const string CapitalIj = "Ĳ";

    /// <summary>
    /// The small letter ij (/jeɪ/ in Dutch).
    /// </summary>
    public const string SmallIj = "ĳ";

    /// <summary>
    /// The capital letter iota.
    /// </summary>
    public const string CapitalIota = "Ɩ";

    /// <summary>
    /// The small letter iota.
    /// </summary>
    public const string SmallIota = "ɩ";

    /// <summary>
    /// The capital letter j
    /// (/iː/ in Latin, /dʒeɪ/ in English, /dʒaɪ/ in Scottish English, /ʒi/ in French, /jɔt/ in German, /ˈxota/ in Spanish, /ˈʒɔtɐ/ in Portuguese, /ilˈluŋga/ or /ˈjota/ in Italian, /j/ in Esperanto).
    /// </summary>
    public const string CapitalJ = "J";

    /// <summary>
    /// The small letter j
    /// (/iː/ in Latin, /dʒeɪ/ in English, /dʒaɪ/ in Scottish English, /ʒi/ in French, /jɔt/ in German, /ˈxota/ in Spanish, /ˈʒɔtɐ/ in Portuguese, /ilˈluŋga/ or /ˈjota/ in Italian, /j/ in Esperanto).
    /// </summary>
    public const string SmallJ = "j";

    /// <summary>
    /// The capital letter k
    /// (/kaː/ in Latin, /keɪ/ in English, /kɑ/ in French, /kaː/ in German, /ka/ in Spanish &amp; Portuguese, /ˈkapɐ/ in Portuguese, /ˈkappa/ in Italian, /k/ in Esperanto).
    /// </summary>
    public const string CapitalK = "K";

    /// <summary>
    /// The small letter k
    /// (/kaː/ in Latin, /keɪ/ in English, /kɑ/ in French, /kaː/ in German, /ka/ in Spanish &amp; Portuguese, /ˈkapɐ/ in Portuguese, /ˈkappa/ in Italian, /k/ in Esperanto).
    /// </summary>
    public const string SmallK = "k";

    /// <summary>
    /// The capital letter l
    /// (/ɛl/ in Latin &amp; English &amp; French &amp; in German, /ˈele/ in Spanish, /ˈɛlɨ/ in Portuguese, /ˈɛlle/ in Italian, /l/ in Esperanto).
    /// </summary>
    public const string CapitalL = "L";

    /// <summary>
    /// The small letter l
    /// (/ɛl/ in Latin &amp; English &amp; in French &amp; in German, /ˈele/ in Spanish, /ˈɛlɨ/ in Portuguese, /ˈɛlle/ in Italian, /l/ in Esperanto).
    /// </summary>
    public const string SmallL = "l";

    /// <summary>
    /// The capital letter m
    /// (/ɛm/ in Latin &amp; English &amp; in French &amp; in German, /ˈeme/ in Spanish, /ˈɛmɨ/ or /ˈẽmi/ in Portuguese, /ˈɛmme/ in Italian, /m/ in Esperanto).
    /// </summary>
    public const string CapitalM = "M";

    /// <summary>
    /// The small letter m
    /// (/ɛm/ in Latin &amp; English &amp; in French &amp; in German, /ˈeme/ in Spanish, /ˈɛmɨ/ or /ˈẽmi/ in Portuguese, /ˈɛmme/ in Italian, /m/ in Esperanto).
    /// </summary>
    public const string SmallM = "m";

    /// <summary>
    /// The capital letter n
    /// (/ɛn/ in Latin &amp; English &amp; in French &amp; in German, /ˈene/ in Spanish, /ˈɛnɨ/ or /ˈẽni/ in Portuguese, /ˈɛnne/ in Italian, /n/ in Esperanto).
    /// </summary>
    public const string CapitalN = "N";

    /// <summary>
    /// The small letter n
    /// (/ɛn/ in Latin &amp; English &amp; in French &amp; in German, /ˈene/ in Spanish, /ˈɛnɨ/ or /ˈẽni/ in Portuguese, /ˈɛnne/ in Italian, /n/ in Esperanto).
    /// </summary>
    public const string SmallN = "n";

    /// <summary>
    /// The capital letter engma (/ŋ/).
    /// </summary>
    public const string CapitalEng = "Ŋ";

    /// <summary>
    /// The small letter engma (/ŋ/).
    /// </summary>
    public const string SmallEng = "ŋ";

    /// <summary>
    /// The capital letter o
    /// (/oː/ in Latin, /əʊ/ in English, /oː/ in German, /o/ in French &amp; Spanish &amp; Italian, /ɔ/ in Portuguese, /o/ in Esperanto).
    /// </summary>
    public const string CapitalO = "O";

    /// <summary>
    /// The small letter o
    /// (/oː/ in Latin, /əʊ/ in English, /oː/ in German, /o/ in French &amp; Spanish &amp; Italian, /ɔ/ in Portuguese, /o/ in Esperanto).
    /// </summary>
    public const string SmallO = "o";

    /// <summary>
    /// The capital letter oe (/oe̯/).
    /// </summary>
    public const string CapitalOe = "Œ";

    /// <summary>
    /// The small letter oe (/oe̯/).
    /// </summary>
    public const string SmallOe = "œ";

    /// <summary>
    /// The capital letter oi.
    /// </summary>
    public const string CapitalOi = "Ƣ";

    /// <summary>
    /// The small letter oi.
    /// </summary>
    public const string SmallOi = "ƣ";

    /// <summary>
    /// The capital letter p
    /// (/peː/ in Latin, /piː/ in English, /peː/ in German, /pe/ in French &amp; Spanish &amp; Portuguese, /pi/ in Italian, /p/ in Esperanto).
    /// </summary>
    public const string CapitalP = "P";

    /// <summary>
    /// The small letter p
    /// (/peː/ in Latin, /piː/ in English, /peː/ in German, /pe/ in French &amp; Spanish &amp; Portuguese, /pi/ in Italian, /p/ in Esperanto).
    /// </summary>
    public const string SmallP = "p";

    /// <summary>
    /// The capital letter q
    /// (/kuː/ in Latin, /kjuː/ in English, /ky/ in French, /kuː/ in German, /ku/ in Spanish &amp; Italian, /ke/ in Portuguese).
    /// </summary>
    public const string CapitalQ = "Q";

    /// <summary>
    /// The small letter q
    /// (/kuː/ in Latin, /kjuː/ in English, /ky/ in French, /kuː/ in German, /ku/ in Spanish &amp; Italian, /ke/ in Portuguese).
    /// </summary>
    public const string SmallQ = "q";

    /// <summary>
    /// The capital letter r
    /// (/ɛr/ in Latin, /ɑːr/ in English, /ɑr/ in American English &amp; Canadian English, /ɔːr/ in Scottish English, /ɛʁ/ in French, /ɛʀ/ in German, /ˈeɾe/ in Spanish, /ˈɛʁɨ/ or /ʁe/ in Portuguese, /ˈɛrre/ in Italian, /r/ in Esperanto).
    /// </summary>
    public const string CapitalR = "R";

    /// <summary>
    /// The small letter r
    /// (/ɛr/ in Latin, /ɑːr/ in English, /ɑr/ in American English &amp; Canadian English, /ɔːr/ in Scottish English, /ɛʁ/ in French, /ɛʀ/ in German, /ˈeɾe/ in Spanish, /ˈɛʁɨ/ or /ʁe/ in Portuguese, /ˈɛrre/ in Italian, /r/ in Esperanto).
    /// </summary>
    public const string SmallR = "r";

    /// <summary>
    /// The capital letter s
    /// (/ɛs/ in Latin &amp; English &amp; French &amp; German, /ˈese/ in Spanish, /ˈɛsɨ/ in Portuguese, /ˈɛsse/ in Italian, /s/ in Esperanto).
    /// </summary>
    public const string CapitalS = "S";

    /// <summary>
    /// The small letter s
    /// (/ɛs/ in Latin &amp; English &amp; French &amp; German, /ˈese/ in Spanish, /ˈɛsɨ/ in Portuguese, /ˈɛsse/ in Italian, /s/ in Esperanto).
    /// </summary>
    public const string SmallS = "s";

    /// <summary>
    /// The capital letter t
    /// (/teː/ in Latin, /tiː/ in English, /teː/ in German, /te/ in French &amp; Spanish &amp; Portuguese, /ti/ in Italian, /t/ in Esperanto).
    /// </summary>
    public const string CapitalT = "T";

    /// <summary>
    /// The small letter t
    /// (/teː/ in Latin, /tiː/ in English, /teː/ in German, /te/ in French &amp; Spanish &amp; Portuguese, /ti/ in Italian, /t/ in Esperanto).
    /// </summary>
    public const string SmallT = "t";

    /// <summary>
    /// The capital letter thorn
    /// (/θ/ in middle English, icelandic, old norse and old Swedish).
    /// </summary>
    public const string CapitalTh = "Þ";

    /// <summary>
    /// The small letter thorn
    /// (/θ/ in middle English, icelandic, old norse and old Swedish).
    /// </summary>
    public const string SmallTh = "þ";

    /// <summary>
    /// The capital letter u
    /// (/uː/ in Latin, /juː/ in English, /y/ in French, /u/ in Spanish &amp; Portuguese &amp; Italian, /u/ in Esperanto).
    /// </summary>
    public const string CapitalU = "U";

    /// <summary>
    /// The small letter u
    /// (/uː/ in Latin, /juː/ in English, /y/ in French, /u/ in Spanish &amp; Portuguese &amp; Italian, /u/ in Esperanto).
    /// </summary>
    public const string SmallU = "u";

    /// <summary>
    /// The capital letter v
    /// (/uː/ in Latin, /viː/ in English, /ve/ in French, /faʊ/ in German, /ˈuβe/ in Spanish, /ve/ in Portuguese, /vu/ in Italian, /v/ in Esperanto).
    /// </summary>
    public const string CapitalV = "V";

    /// <summary>
    /// The small letter v
    /// (/uː/ in Latin, /viː/ in English, /ve/ in French, /faʊ/ in German, /ˈuβe/ in Spanish, /ve/ in Portuguese, /vu/ in Italian, /v/ in Esperanto).
    /// </summary>
    public const string SmallV = "v";

    /// <summary>
    /// The capital letter w
    /// (/ˈdʌbəl.juː/ in English, /dubləve/ in French, /veː/ in German, /uβeˈdoβle/ in Spanish, /ˈdɐ̃bliu/ in Portuguese, /doppjavu/ in Italian).
    /// </summary>
    public const string CapitalW = "W";

    /// <summary>
    /// The small letter w
    /// (/ˈdʌbəl.juː/ in English, /dubləve/ in French, /veː/ in German, /uβeˈdoβle/ in Spanish, /ˈdɐ̃bliu/ in Portuguese, /doppjavu/ in Italian).
    /// </summary>
    public const string SmallW = "w";

    /// <summary>
    /// The capital letter x
    /// (/ɛks/ in Latin &amp; English, /iks/ in Latin &amp; French &amp; German &amp; Italian, /ˈekis/ in Spanish, /ʃiʃ/ in Portuguese).
    /// </summary>
    public const string CapitalX = "X";

    /// <summary>
    /// The small letter x
    /// (/ɛks/ in Latin &amp; English, /iks/ in Latin &amp; French &amp; German &amp; Italian, /ˈekis/ in Spanish, /ʃiʃ/ in Portuguese).
    /// </summary>
    public const string SmallX = "x";

    /// <summary>
    /// The capital letter y
    /// (/yː/ in Latin, /waɪ/ in English, /igʁɛk/ in French, /ˈipsɪlɔn/ in German, /je/ or /'ipsilon/ or /iˈɡɾjeɣa/ in Spanish, /ˈipsɨlɔn/ or /ˈipsilõ/ in Portuguese, /iɡˈɡrɛka/ or /ˈipsilon/ in Italian).
    /// </summary>
    public const string CapitalY = "Y";

    /// <summary>
    /// The small letter y
    /// (/yː/ in Latin, /waɪ/ in English, /igʁɛk/ in French, /ˈipsɪlɔn/ in German, /je/ or /'ipsilon/ or /iˈɡɾjeɣa/ in Spanish, /ˈipsɨlɔn/ or /ˈipsilõ/ in Portuguese, /iɡˈɡrɛka/ or /ˈipsilon/ in Italian).
    /// </summary>
    public const string SmallY = "y";

    /// <summary>
    /// The capital letter yr.
    /// </summary>
    public const string CapitalYr = "Ʀ";

    /// <summary>
    /// The small letter yr.
    /// </summary>
    public const string SmallYr = "ʀ";

    /// <summary>
    /// The capital letter z
    /// (/ˈzeːta/ in Latin, /zɛd/ in British English, /ziː/ in Amercian English, /zɛd/ in French, /tsɛt/ in German, /ˈθeta/ or /ˈseta/ in Spanish, /ze/ in Portuguese, /ˈzɛta/ in Italian, /z/ in Esperanto).
    /// </summary>
    public const string CapitalZ = "Z";

    /// <summary>
    /// The small letter z
    /// (/ˈzeːta/ in Latin, /zɛd/ in British English, /ziː/ in Amercian English, /zɛd/ in French, /tsɛt/ in German, /ˈθeta/ or /ˈseta/ in Spanish, /ze/ in Portuguese, /ˈzɛta/ in Italian, /z/ in Esperanto).
    /// </summary>
    public const string SmallZ = "z";

    /// <summary>
    /// The capital letter ss (/ɛsˈtsɛt/ in German).
    /// </summary>
    public const string CapitalSs = "ẞ";

    /// <summary>
    /// The small letter ss (/ɛsˈtsɛt/ in German).
    /// </summary>
    public const string SmallSs = "ß";

    /// <summary>
    /// The capital letter esh (/ɛʃ/ in Shona).
    /// </summary>
    public const string CapitalEsh = "Ʃ";

    /// <summary>
    /// The small letter esh (/ɛʃ/ in Shona).
    /// </summary>
    public const string SmallEsh = "ʃ";

    /// <summary>
    /// The capital letter schwa (/æ/ in Azerbaijani &amp; Chechen).
    /// </summary>
    public const string CapitalSchwa = "Ə";

    /// <summary>
    /// The small letter schwa (/æ/ in Azerbaijani &amp; Chechen).
    /// </summary>
    public const string SmallSchwa = "ə";

    /// <summary>
    /// The capital inverse letter of e.
    /// </summary>
    public const string CapitalInverseE = "Ǝ";

    /// <summary>
    /// The small inverse letter of e.
    /// </summary>
    public const string SmallInverseE = "ǝ";

    /// <summary>
    /// The capital inverse letter of ezh.
    /// </summary>
    public const string CapitalInverseEzh = "Ƹ";

    /// <summary>
    /// The small inverse letter of ezh.
    /// </summary>
    public const string SmallInverseEzh = "ƹ";

    /// <summary>
    /// The capital inverse letter of m.
    /// </summary>
    public const string CapitalInverseM = "Ɯ";

    /// <summary>
    /// The small inverse letter of m.
    /// </summary>
    public const string SmallInverseM = "ɯ";

    /// <summary>
    /// The capital inverse letter of s and tone 2.
    /// </summary>
    public const string CapitalInverseS = "Ƨ";

    /// <summary>
    /// The small inverse letter of s and tone 2.
    /// </summary>
    public const string SmallInverseS = "ƨ";

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
    }
}
