using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;

namespace Trivial.Text;

/// <summary>
/// The symbols of Hanyu Pinyin (Chinese tonal marks).
/// </summary>
public static partial class PinyinMarks
{
    // Hanyu Pinyin (汉语拼音)
    // http://www.moe.gov.cn/jyb_sjzl/ziliao/A19/195802/t19580201_186000.html

    /// <summary>
    /// The Pinyin name.
    /// </summary>
    public const string Name = "Hànyŭ Pīnyīn";

    /// <summary>
    /// The Pinyin name in Chinese simplified.
    /// </summary>
    public const string ChineseName = "汉语拼音";

    /// <summary>
    /// The Pinyin name in English.
    /// </summary>
    public const string EnglishName = "Chinese Pinyin";

    /// <summary>
    /// The capital letter A (ㄚ a).
    /// </summary>
    public const string CapitalA = "A";

    /// <summary>
    /// The capital letter A with high and level tone (ㄚ a).
    /// </summary>
    public const string CapitalA1 = "Ā";

    /// <summary>
    /// The capital letter A with rising tone (ㄚˊ a).
    /// </summary>
    public const string CapitalA2 = "Á";

    /// <summary>
    /// The capital letter A with falling-rising tone (ㄚˇ aa).
    /// </summary>
    public const string CapitalA3 = "Ǎ";

    /// <summary>
    /// The capital letter A with falling tone (ㄚˋ ah).
    /// </summary>
    public const string CapitalA4 = "À";

    /// <summary>
    /// The capital letter O (ㄛ o).
    /// </summary>
    public const string CapitalO = "O";

    /// <summary>
    /// The capital letter O with high and level tone (ㄛ o).
    /// </summary>
    public const string CapitalO1 = "Ō";

    /// <summary>
    /// The capital letter O with rising tone (ㄛˊ o).
    /// </summary>
    public const string CapitalO2 = "Ó";

    /// <summary>
    /// The capital letter O with falling-rising tone (ㄛˇ o).
    /// </summary>
    public const string CapitalO3 = "Ǒ";

    /// <summary>
    /// The capital letter O with falling tone (ㄛˋ oh).
    /// </summary>
    public const string CapitalO4 = "Ò";

    /// <summary>
    /// The capital letter Oe.
    /// </summary>
    public const string CapitalOe = "Œ";

    /// <summary>
    /// The capital letter E (ㄜ e).
    /// </summary>
    public const string CapitalE = "E";

    /// <summary>
    /// The capital letter E with high and level tone (ㄜ e).
    /// </summary>
    public const string CapitalE1 = "Ē";

    /// <summary>
    /// The capital letter E with rising tone (ㄜˊ e).
    /// </summary>
    public const string CapitalE2 = "É";

    /// <summary>
    /// The capital letter E with falling-rising tone (ㄜˇ e).
    /// </summary>
    public const string CapitalE3 = "Ě";

    /// <summary>
    /// The capital letter E with falling tone (ㄜˋ e).
    /// </summary>
    public const string CapitalE4 = "È";

    /// <summary>
    /// The capital letter Eh (ㄝ eh).
    /// </summary>
    public const string CapitalEh = "Ê";

    /// <summary>
    /// The capital letter Eh with rising tone (ㄝˊ eh).
    /// </summary>
    public const string CapitalEh2 = "Ế";

    /// <summary>
    /// The capital letter Eh with falling tone (ㄝˋ eh).
    /// </summary>
    public const string CapitalEh4 = "Ề";

    /// <summary>
    /// The capital letter Er (ㄦ erh).
    /// </summary>
    public const string CapitalEr = "Er";

    /// <summary>
    /// The capital letter Eo.
    /// </summary>
    public const string CapitalEo = "Ɵ";

    /// <summary>
    /// The capital letter I (丨 yi).
    /// </summary>
    public const string CapitalI = "I";

    /// <summary>
    /// The capital letter I with high and level tone (丨 yi).
    /// </summary>
    public const string CapitalI1 = "Ī";

    /// <summary>
    /// The capital letter I with rising tone (丨ˊ yi).
    /// </summary>
    public const string CapitalI2 = "Í";

    /// <summary>
    /// The capital letter I with falling-rising tone (丨ˇ yi).
    /// </summary>
    public const string CapitalI3 = "Ǐ";

    /// <summary>
    /// The capital letter I with falling tone (丨ˋ yee).
    /// </summary>
    public const string CapitalI4 = "Ì";

    /// <summary>
    /// The capital letter U (ㄨ wu).
    /// </summary>
    public const string CapitalU = "U";

    /// <summary>
    /// The capital letter U with high and level tone (ㄨ wu).
    /// </summary>
    public const string CapitalU1 = "Ū";

    /// <summary>
    /// The capital letter U with rising tone (ㄨˊ wu).
    /// </summary>
    public const string CapitalU2 = "Ú";

    /// <summary>
    /// The capital letter U with falling-rising tone (ㄨˇ wu).
    /// </summary>
    public const string CapitalU3 = "Ǔ";

    /// <summary>
    /// The capital letter U with falling tone (ㄨˋ woo).
    /// </summary>
    public const string CapitalU4 = "Ù";

    /// <summary>
    /// The capital letter V (Yu) (ㄩ yu).
    /// </summary>
    public const string CapitalV = "Ü";

    /// <summary>
    /// The capital letter V (Yu) with high and level tone (ㄩ yu).
    /// </summary>
    public const string CapitalV1 = "Ǖ";

    /// <summary>
    /// The capital letter V (Yu) with rising tone (ㄩˊ yu).
    /// </summary>
    public const string CapitalV2 = "Ǘ";

    /// <summary>
    /// The capital letter V (Yu) with falling-rising tone (ㄩˇ yu).
    /// </summary>
    public const string CapitalV3 = "Ǚ";

    /// <summary>
    /// The capital letter V (Yu) with falling tone (ㄩˋ yu).
    /// </summary>
    public const string CapitalV4 = "Ǜ";

    /// <summary>
    /// The capital letter N (兀 Mmm).
    /// </summary>
    public const string CapitalN = "N";

    /// <summary>
    /// The capital letter N with rising tone (兀ˊ Mmm).
    /// </summary>
    public const string CapitalN2 = "Ń";

    /// <summary>
    /// The capital letter N with falling-rising tone (兀ˇ Mmm).
    /// </summary>
    public const string CapitalN3 = "Ň";

    /// <summary>
    /// The capital letter N with falling tone (兀ˋ Mmm).
    /// </summary>
    public const string CapitalN4 = "Ǹ";

    /// <summary>
    /// The small letter A (ㄚ a).
    /// </summary>
    public const string SmallA = "a";

    /// <summary>
    /// The small letter A with high and level tone (ㄚ a).
    /// </summary>
    public const string SmallA1 = "ā";

    /// <summary>
    /// The small letter A with rising tone (ㄚˊ a).
    /// </summary>
    public const string SmallA2 = "á";

    /// <summary>
    /// The small letter A with falling-rising tone (ㄚˇ aa).
    /// </summary>
    public const string SmallA3 = "ǎ";

    /// <summary>
    /// The small letter A with falling tone (ㄚˋ ah).
    /// </summary>
    public const string SmallA4 = "à";

    /// <summary>
    /// The small letter O (ㄛ o).
    /// </summary>
    public const string SmallO = "o";

    /// <summary>
    /// The small letter O with high and level tone (ㄛ o).
    /// </summary>
    public const string SmallO1 = "ō";

    /// <summary>
    /// The small letter O with rising tone (ㄛˊ o).
    /// </summary>
    public const string SmallO2 = "ó";

    /// <summary>
    /// The small letter O with falling-rising tone (ㄛˇ o).
    /// </summary>
    public const string SmallO3 = "ǒ";

    /// <summary>
    /// The small letter O with falling tone (ㄛˋ oh).
    /// </summary>
    public const string SmallO4 = "ò";

    /// <summary>
    /// The small letter Oe.
    /// </summary>
    public const string SmallOe = "œ";

    /// <summary>
    /// The small letter E (ㄜ e).
    /// </summary>
    public const string SmallE = "e";

    /// <summary>
    /// The small letter E with high and level tone (ㄜ e).
    /// </summary>
    public const string SmallE1 = "ē";

    /// <summary>
    /// The small letter E with rising tone (ㄜˊ e).
    /// </summary>
    public const string SmallE2 = "é";

    /// <summary>
    /// The small letter E with falling-rising tone (ㄜˇ e).
    /// </summary>
    public const string SmallE3 = "ě";

    /// <summary>
    /// The small letter E with falling tone (ㄜˋ e).
    /// </summary>
    public const string SmallE4 = "è";

    /// <summary>
    /// The small letter Eh (ㄝ eh).
    /// </summary>
    public const string SmallEh = "ê";

    /// <summary>
    /// The small letter Eh with rising tone (ㄝˊ eh).
    /// </summary>
    public const string SmallEh2 = "ế";

    /// <summary>
    /// The small letter Eh with falling tone (ㄝˋ eh).
    /// </summary>
    public const string SmallEh4 = "ề";

    /// <summary>
    /// The small letter Er (ㄦ erh).
    /// </summary>
    public const string SmallEr = "er";

    /// <summary>
    /// The small letter Er short (ㄦ -l).
    /// </summary>
    public const string SmallErShort = "r";

    /// <summary>
    /// The small letter Eo.
    /// </summary>
    public const string SmallEo = "ɵ";

    /// <summary>
    /// The small letter I (丨 yi).
    /// </summary>
    public const string SmallI = "i";

    /// <summary>
    /// The small letter I with high and level tone (丨 yi).
    /// </summary>
    public const string SmallI1 = "ī";

    /// <summary>
    /// The small letter I with rising tone (丨ˊ yi).
    /// </summary>
    public const string SmallI2 = "í";

    /// <summary>
    /// The small letter I with falling-rising tone (丨ˇ yi).
    /// </summary>
    public const string SmallI3 = "ǐ";

    /// <summary>
    /// The small letter I with falling tone (丨ˋ yee).
    /// </summary>
    public const string SmallI4 = "ì";

    /// <summary>
    /// The small letter U (ㄨ wu).
    /// </summary>
    public const string SmallU = "u";

    /// <summary>
    /// The small letter U with high and level tone (ㄨ wu).
    /// </summary>
    public const string SmallU1 = "ū";

    /// <summary>
    /// The small letter U with rising tone (ㄨˊ wu).
    /// </summary>
    public const string SmallU2 = "ú";

    /// <summary>
    /// The small letter U with falling-rising tone (ㄨˇ wu).
    /// </summary>
    public const string SmallU3 = "ŭ";

    /// <summary>
    /// The small letter U with falling tone (ㄨˋ woo).
    /// </summary>
    public const string SmallU4 = "ù";

    /// <summary>
    /// The small letter V (Yu) (ㄩ yu).
    /// </summary>
    public const string SmallV = "ü";

    /// <summary>
    /// The small letter V (Yu) with high and level tone (ㄩ yu).
    /// </summary>
    public const string SmallV1 = "ǖ";

    /// <summary>
    /// The small letter V (Yu) with rising tone (ㄩˊ yu).
    /// </summary>
    public const string SmallV2 = "ǘ";

    /// <summary>
    /// The small letter V (Yu) with falling-rising tone (ㄩˇ yu).
    /// </summary>
    public const string SmallV3 = "ǚ";

    /// <summary>
    /// The small letter V (Yu) with falling tone (ㄩˋ yu).
    /// </summary>
    public const string SmallV4 = "ǜ";

    /// <summary>
    /// The small letter N (兀 Mmm).
    /// </summary>
    public const string SmallN = "n";

    /// <summary>
    /// The small letter N with rising tone (兀ˊ Mmm).
    /// </summary>
    public const string SmallN2 = "ń";

    /// <summary>
    /// The small letter N with falling-rising tone (兀ˇ Mmm).
    /// </summary>
    public const string SmallN3 = "ň";

    /// <summary>
    /// The small letter N with falling tone (兀ˋ Mmm).
    /// </summary>
    public const string SmallN4 = "ǹ";

    /// <summary>
    /// Checked tone p.
    /// </summary>
    public const string CheckedToneP = "p̚";

    /// <summary>
    /// Checked tone t.
    /// </summary>
    public const string CheckedToneT = "t̚";

    /// <summary>
    /// Checked tone k.
    /// </summary>
    public const string CheckedToneK = "k̚";

    /// <summary>
    /// Checked tone merged.
    /// </summary>
    public const string CheckedToneMerged = "ʔ";

    /// <summary>
    /// The high and level tone.
    /// </summary>
    public const string Tone1 = "¯";

    /// <summary>
    /// The rising tone.
    /// </summary>
    public const string Tone2 = "ˊ";

    /// <summary>
    /// The falling-rising tone.
    /// </summary>
    public const string Tone3 = "ˇ";

    /// <summary>
    /// The falling tone.
    /// </summary>
    public const string Tone4 = "ˋ";

    /// <summary>
    /// The oblique tone.
    /// </summary>
    public const string Tone5 = "˙";

    /// <summary>
    /// The seperator before sub-sequent final-only character in a word.
    /// </summary>
    public const string Seperator = "'";
}
