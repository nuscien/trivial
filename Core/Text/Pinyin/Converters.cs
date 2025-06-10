using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using Trivial.Maths;
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// The symbols of Hanyu Pinyin (Chinese tonal marks).
/// </summary>
public static partial class PinyinMarks
{
    #pragma warning disable IDE0056, IDE0057

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="initial">The Pinyin initial.</param>
    /// <param name="capitalize">true if returns capitalized; otherwise, false.</param>
    /// <param name="appendDefaultFinal">true if need to append default final; otherwise, false.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    public static string ToString(PinyinInitials initial, bool capitalize = false, bool appendDefaultFinal = false)
    {
        var s = capitalize ? initial switch
        {
            PinyinInitials.B => "B",
            PinyinInitials.P => "P",
            PinyinInitials.M => "M",
            PinyinInitials.F => "F",
            PinyinInitials.D => "D",
            PinyinInitials.T => "T",
            PinyinInitials.N => "N",
            PinyinInitials.L => "L",
            PinyinInitials.G => "G",
            PinyinInitials.K => "K",
            PinyinInitials.H => "H",
            PinyinInitials.J => "J",
            PinyinInitials.Q => "Q",
            PinyinInitials.X => "X",
            PinyinInitials.Zh => "Zh",
            PinyinInitials.Ch => "Ch",
            PinyinInitials.Sh => "Sh",
            PinyinInitials.R => "R",
            PinyinInitials.Z => "Z",
            PinyinInitials.C => "C",
            PinyinInitials.S => "S",
            PinyinInitials.Y => "Y",
            PinyinInitials.W => "W",
            _ => string.Empty
        } : initial switch
        {
            PinyinInitials.B => "b",
            PinyinInitials.P => "p",
            PinyinInitials.M => "m",
            PinyinInitials.F => "f",
            PinyinInitials.D => "d",
            PinyinInitials.T => "t",
            PinyinInitials.N => "n",
            PinyinInitials.L => "l",
            PinyinInitials.G => "g",
            PinyinInitials.K => "k",
            PinyinInitials.H => "h",
            PinyinInitials.J => "j",
            PinyinInitials.Q => "q",
            PinyinInitials.X => "x",
            PinyinInitials.Zh => "zh",
            PinyinInitials.Ch => "ch",
            PinyinInitials.Sh => "sh",
            PinyinInitials.R => "r",
            PinyinInitials.Z => "z",
            PinyinInitials.C => "c",
            PinyinInitials.S => "s",
            PinyinInitials.Y => "y",
            PinyinInitials.W => "w",
            _ => string.Empty
        };
        return appendDefaultFinal ? (s + initial switch
        {
            PinyinInitials.B or PinyinInitials.P or PinyinInitials.M or PinyinInitials.F => "o",
            PinyinInitials.D or PinyinInitials.T or PinyinInitials.N or PinyinInitials.L => "e",
            PinyinInitials.G or PinyinInitials.K or PinyinInitials.H => "e",
            PinyinInitials.J or PinyinInitials.Q or PinyinInitials.X => "i",
            PinyinInitials.Zh or PinyinInitials.Ch or PinyinInitials.Sh or PinyinInitials.R or PinyinInitials.Z or PinyinInitials.C or PinyinInitials.S => "i",
            PinyinInitials.Y => "i",
            PinyinInitials.W => "u",
            _ => string.Empty
        }) : s;
    }

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="initial">The Pinyin initial.</param>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="caseOptions">Optional case.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    public static string ToString(PinyinInitials initial, PinyinFinals final, int tone = 0, Cases caseOptions = Cases.Lower)
    {
        var sb = new StringBuilder(ToString(initial, caseOptions == Cases.Upper || caseOptions == Cases.Capitalize));
        if (sb.Length == 0) return ToString(final, tone, caseOptions);
        sb.Append(final switch
        {
            PinyinFinals.V => ToFinalString(initial switch
            {
                PinyinInitials.J or PinyinInitials.Q or PinyinInitials.X or PinyinInitials.Y => 'u',
                _ => 'v'
            }, tone, caseOptions, string.Empty),
            _ => ToString(final, tone, caseOptions == Cases.Upper ? Cases.Upper : Cases.Lower, true)
        });
        if (sb.Length > 2)
        {
            switch (initial)
            {
                case PinyinInitials.Y:
                    if ((sb[1] == 'i' || sb[1] == 'I') && final != PinyinFinals.I)
                        sb.Remove(1, 1);
                    break;
                case PinyinInitials.W:
                    if ((sb[1] == 'u' || sb[1] == 'U') && final != PinyinFinals.U && final != PinyinFinals.V)
                        sb.Remove(1, 1);
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="caseOptions">Optional case.</param>
    /// <param name="forFinalOnly">true if returns only for final; otherwise, false.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    public static string ToString(PinyinFinals final, int tone = 0, Cases caseOptions = Cases.Lower, bool forFinalOnly = false)
    {
        return final switch
        {
            PinyinFinals.A => ToFinalString('a', tone, caseOptions, string.Empty),
            PinyinFinals.Ai => ToFinalString('a', tone, caseOptions, "i"),
            PinyinFinals.Ao => ToFinalString('a', tone, caseOptions, "o"),
            PinyinFinals.An => ToFinalString('a', tone, caseOptions, "n"),
            PinyinFinals.Ang => ToFinalString('a', tone, caseOptions, "ng"),
            PinyinFinals.O => ToFinalString('o', tone, caseOptions, string.Empty),
            PinyinFinals.Ou => ToFinalString('o', tone, caseOptions, "u"),
            PinyinFinals.Ong => ToFinalString('o', tone, caseOptions, "ng"),
            PinyinFinals.E => ToFinalString('e', tone, caseOptions, string.Empty),
            PinyinFinals.Er => ToFinalString('e', tone, caseOptions, "r"),
            PinyinFinals.Ei => ToFinalString('e', tone, caseOptions, "i"),
            PinyinFinals.En => ToFinalString('e', tone, caseOptions, "n"),
            PinyinFinals.Eng => ToFinalString('e', tone, caseOptions, "ng"),
            PinyinFinals.I => forFinalOnly ? ToFinalString('i', tone, caseOptions, string.Empty) : ToFinalString("Y", 'i', tone, caseOptions, string.Empty),
            PinyinFinals.Ia => forFinalOnly ? ToFinalString("i", 'a', tone, caseOptions, string.Empty) : ToFinalString("Y", 'a', tone, caseOptions, string.Empty),
            PinyinFinals.Iao => forFinalOnly ? ToFinalString("i", 'a', tone, caseOptions, "o") : ToFinalString("Y", 'a', tone, caseOptions, "o"),
            PinyinFinals.Ian => forFinalOnly ? ToFinalString("i", 'a', tone, caseOptions, "n") : ToFinalString("Y", 'a', tone, caseOptions, "n"),
            PinyinFinals.Iang => forFinalOnly ? ToFinalString("i", 'a', tone, caseOptions, "ng") : ToFinalString("Y", 'a', tone, caseOptions, "ng"),
            PinyinFinals.Ie => forFinalOnly ? ToFinalString("i", 'e', tone, caseOptions, string.Empty) : ToFinalString("Y", 'e', tone, caseOptions, string.Empty),
            PinyinFinals.Iou => forFinalOnly ? ToFinalString("i", 'u', tone, caseOptions, string.Empty) : ToFinalString("Y", 'o', tone, caseOptions, "u"),
            PinyinFinals.Iong => forFinalOnly ? ToFinalString("i", 'o', tone, caseOptions, "ng") : ToFinalString("Y", 'o', tone, caseOptions, "ng"),
            PinyinFinals.In => forFinalOnly ? ToFinalString('i', tone, caseOptions, "n") : ToFinalString("Y", 'i', tone, caseOptions, "n"),
            PinyinFinals.Ing => forFinalOnly ? ToFinalString('i', tone, caseOptions, "ng") : ToFinalString("Y", 'i', tone, caseOptions, "ng"),
            PinyinFinals.U => forFinalOnly ? ToFinalString('u', tone, caseOptions, string.Empty) : ToFinalString("W", 'u', tone, caseOptions, string.Empty),
            PinyinFinals.Ua => forFinalOnly ? ToFinalString("u", 'a', tone, caseOptions, string.Empty) : ToFinalString("W", 'a', tone, caseOptions, string.Empty),
            PinyinFinals.Uai => forFinalOnly ? ToFinalString("u", 'a', tone, caseOptions, "i") : ToFinalString("W", 'a', tone, caseOptions, "i"),
            PinyinFinals.Uan => forFinalOnly ? ToFinalString("u", 'a', tone, caseOptions, "n") : ToFinalString("W", 'a', tone, caseOptions, "n"),
            PinyinFinals.Uang => forFinalOnly ? ToFinalString("u", 'a', tone, caseOptions, "ng") : ToFinalString("W", 'a', tone, caseOptions, "ng"),
            PinyinFinals.Uei => forFinalOnly ? ToFinalString("u", 'i', tone, caseOptions, string.Empty) : ToFinalString("W", 'e', tone, caseOptions, "i"),
            PinyinFinals.Uen => forFinalOnly ? ToFinalString('u', tone, caseOptions, "n") : ToFinalString("W", 'e', tone, caseOptions, "n"),
            PinyinFinals.Ueng => forFinalOnly ? ToFinalString("u", 'e', tone, caseOptions, "ng") : ToFinalString("W", 'e', tone, caseOptions, "ng"),
            PinyinFinals.Uo => forFinalOnly ? ToFinalString("u", 'o', tone, caseOptions, string.Empty) : ToFinalString("W", 'o', tone, caseOptions, string.Empty),
            PinyinFinals.V => forFinalOnly ? ToFinalString('v', tone, caseOptions, string.Empty) : ToFinalString("Y", 'u', tone, caseOptions, string.Empty),
            PinyinFinals.Van => forFinalOnly ? ToFinalString('v', tone, caseOptions, "an") : ToFinalString("Y", 'u', tone, caseOptions, "an"),
            PinyinFinals.Ve => forFinalOnly ? ToFinalString('u', tone, caseOptions, "e") : ToFinalString("Y", 'u', tone, caseOptions, "e"),
            PinyinFinals.Vn => forFinalOnly ? ToFinalString('u', tone, caseOptions, "n") : ToFinalString("Y", 'u', tone, caseOptions, "n"),
            PinyinFinals.Ng => ToFinalString('n', tone, caseOptions, "g"),
            _ => (int)final == 7
            ? (tone == 0 ? (caseOptions == Cases.Upper || caseOptions == Cases.Capitalize ? CapitalEh : SmallEh) : ToFinalString('e', tone, caseOptions, "i"))
            : string.Empty
        };
    }

    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
    /// <returns>A string for the number.</returns>
    public static string ToString(int number, bool digitOnly = false)
        => ToNumberString(ChineseNumerals.Simplified.ToString(number, digitOnly));

    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
    /// <returns>A string for the number.</returns>
    public static string ToString(long number, bool digitOnly = false)
        => ToNumberString(ChineseNumerals.Simplified.ToString(number, digitOnly));

    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>A string for the number.</returns>
    public static string ToString(double number)
        => ToNumberString(ChineseNumerals.Simplified.ToString(number));

    /// <summary>
    /// Parses a Pinyin initial.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>The Pinyin initial parsed; or null, if parse failed.</returns>
    /// <exception cref="FormatException">s was not in correct format.</exception>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty.</exception>
    public static PinyinInitials ParseInitial(string s)
    {
        if (s == null) throw ObjectConvert.ArgumentNull(nameof(s));
        if (s.Length < 1) throw new ArgumentException("s should not be empty.", nameof(s));
        var r = TryParseInitial(s);
        if (r.HasValue) return r.Value;
        throw new FormatException("s was not in correct format.", new ArgumentException("s should be a Pinyin initial.", nameof(s)));
    }

    /// <summary>
    /// Tries to parse a Pinyin initial.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>The Pinyin initial parsed; or null, if parse failed.</returns>
    public static PinyinInitials? TryParseInitial(string s)
    {
        if (s == null) return null;
        s = s.Trim().ToLowerInvariant();
        if (s.Length < 1) return null;
        if (Enum.TryParse(s, true, out PinyinInitials result)) return result;
        return s.Length switch
        {
            1 => s switch
            {
                "ㄅ" => PinyinInitials.B,
                "ㄆ" => PinyinInitials.P,
                "ㄇ" => PinyinInitials.M,
                "ㄈ" => PinyinInitials.F,
                "ㄉ" => PinyinInitials.D,
                "ㄊ" => PinyinInitials.T,
                "ㄋ" => PinyinInitials.N,
                "ㄌ" => PinyinInitials.L,
                "ㄍ" => PinyinInitials.G,
                "ㄎ" => PinyinInitials.K,
                "ㄏ" => PinyinInitials.H,
                "ㄐ" => PinyinInitials.J,
                "ㄑ" => PinyinInitials.Q,
                "ㄒ" => PinyinInitials.X,
                "ㄓ" or "ẑ" or "Ẑ" => PinyinInitials.Zh,
                "ㄔ" or "ĉ" or "Ĉ" => PinyinInitials.Ch,
                "ㄕ" or "ŝ" or "Ŝ" => PinyinInitials.Sh,
                "ㄖ" => PinyinInitials.R,
                "ㄗ" => PinyinInitials.Z,
                "ㄘ" => PinyinInitials.C,
                "ㄙ" => PinyinInitials.S,
                "丨" or "ㄧ" or "i" or "I" => PinyinInitials.Y,
                "ㄨ" or "u" or "U" => PinyinInitials.W,
                _ => null
            },
            2 => s.ToLowerInvariant() switch
            {
                "bo" => PinyinInitials.B,
                "po" => PinyinInitials.P,
                "mo" => PinyinInitials.M,
                "fo" => PinyinInitials.F,
                "de" => PinyinInitials.D,
                "te" => PinyinInitials.T,
                "ne" => PinyinInitials.N,
                "le" => PinyinInitials.L,
                "ge" => PinyinInitials.G,
                "ke" => PinyinInitials.K,
                "he" => PinyinInitials.H,
                "ji" => PinyinInitials.J,
                "qi" => PinyinInitials.Q,
                "xi" or "hs" => PinyinInitials.X,
                "ri" => PinyinInitials.R,
                "zi" or "tz" => PinyinInitials.Z,
                "ci" or "ts" => PinyinInitials.C,
                "si" or "ss" => PinyinInitials.S,
                "yi" => PinyinInitials.Y,
                "wu" => PinyinInitials.W,
                _ => null
            },
            3 => s.ToLowerInvariant() switch
            {
                "zhi" => PinyinInitials.Zh,
                "chi" => PinyinInitials.Ch,
                "shi" => PinyinInitials.Sh,
                _ => null
            },
            _ => null,
        };
    }

    /// <summary>
    /// Tries to parse a Pinyin initial.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="result">A Pinyin initial parsed</param>
    /// <returns>true parse succeeded; otherwise, false.</returns>
    public static bool TryParseInitial(string s, out PinyinInitials result)
    {
        var r = TryParseInitial(s);
        if (r.HasValue)
        {
            result = r.Value;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Parses a Pinyin final.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>The Pinyin final parsed; or null, if parse failed.</returns>
    /// <exception cref="FormatException">s was not in correct format.</exception>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty.</exception>
    public static PinyinFinals ParseFinal(string s)
    {
        if (s == null) throw ObjectConvert.ArgumentNull(nameof(s));
        if (s.Length < 1) throw new ArgumentException("s should not be empty.", nameof(s));
        var r = TryParseFinal(s);
        if (r.HasValue) return r.Value;
        throw new FormatException("s was not in correct format.", new ArgumentException("s should be a Pinyin final.", nameof(s)));
    }

    /// <summary>
    /// Tries to parse a Pinyin final.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>The Pinyin final parsed; or null, if parse failed.</returns>
    public static PinyinFinals? TryParseFinal(string s)
    {
        if (s == null) return null;
        s = s.Trim().ToLowerInvariant();
        if (s.Length < 1) return null;
        if (Enum.TryParse(s, true, out PinyinFinals result)) return result;
        return s.Length switch
        {
            1 => s switch
            {
                "ㄚ" => PinyinFinals.A,
                "ㄛ" => PinyinFinals.O,
                "ㄜ" or "ㄝ" or "Ê" or "ê" => PinyinFinals.E, // "Ê" or "ê" => PinyinFinals.Eh,
                "丨" or "ㄧ" or "一" => PinyinFinals.I,
                "ㄨ" => PinyinFinals.U,
                "ㄩ" or "Ü" or "ü" => PinyinFinals.V,
                "ㄦ" or "儿" or "r" => PinyinFinals.Er,
                "ㄞ" or "哀" => PinyinFinals.Ai,
                "ㄟ" or "欸" => PinyinFinals.Ei,
                "ㄠ" or "熬" => PinyinFinals.Ao,
                "ㄡ" or "欧" => PinyinFinals.Ou,
                "ㄢ" or "安" => PinyinFinals.An,
                "ㄣ" or "恩" => PinyinFinals.En,
                "ㄤ" or "昂" => PinyinFinals.Ang,
                "ㄥ" or "鞥" or "亨" => PinyinFinals.Eng,
                "ā" or "á" or "ǎ" or "à" or "Ā" or "Á" or "Ǎ" or "À" or "啊" or "ɑ" => PinyinFinals.A,
                "ō" or "ó" or "ò" or "ò" or "Ō" or "Ó" or "Ò" or "Ò" or "喔" or "ɔ" => PinyinFinals.O,
                "ē" or "é" or "ě" or "è" or "Ē" or "É" or "Ě" or "È" or "鹅" or "ɜ" => PinyinFinals.E,
                "ī" or "í" or "ǐ" or "ì" or "Ī" or "Í" or "Ǐ" or "Ì" or "衣" or "ɪ" => PinyinFinals.I,
                "ū" or "ú" or "ǔ" or "ù" or "Ū" or "Ú" or "Ǔ" or "Ù" or "乌" or "ʊ" => PinyinFinals.U,
                "ǖ" or "ǘ" or "ǚ" or "ǜ" or "Ǖ" or "Ǘ" or "Ǚ" or "Ǜ" or "迂" => PinyinFinals.V,
                "ń" or "ň" or "ǹ" or "Ń" or "Ň" or "Ǹ" => PinyinFinals.Ng,
                "呀" => PinyinFinals.Ia,
                "耶" => PinyinFinals.Ie,
                "蛙" => PinyinFinals.Ua,
                "窝" => PinyinFinals.Uo,
                "约" => PinyinFinals.Ve,
                "腰" => PinyinFinals.Iao,
                "忧" => PinyinFinals.Iou,
                "歪" => PinyinFinals.Uai,
                "威" => PinyinFinals.Uei,
                "烟" => PinyinFinals.Ian,
                "弯" => PinyinFinals.Uan,
                "冤" => PinyinFinals.Van,
                "因" => PinyinFinals.In,
                "温" => PinyinFinals.Uen,
                "晕" => PinyinFinals.Vn,
                "央" => PinyinFinals.Iang,
                "汪" => PinyinFinals.Uang,
                "翁" => PinyinFinals.Ueng,
                "英" => PinyinFinals.Ing,
                "轰" => PinyinFinals.Ong,
                "雍" => PinyinFinals.Iong,
                "嗯" => PinyinFinals.Ng,
                _ => null
            },
            2 => s.ToLowerInvariant() switch
            {
                "丨ㄚ" or "ㄧㄚ" => PinyinFinals.Ia,
                "丨ㄝ" or "ㄧㄝ" => PinyinFinals.Ie,
                "ㄨㄚ" => PinyinFinals.Ua,
                "ㄨㄛ" => PinyinFinals.Uo,
                "ㄩㄝ" => PinyinFinals.Ve,
                "丨ㄠ" or "ㄧㄠ" => PinyinFinals.Iao,
                "丨ㄡ" or "ㄧㄡ" => PinyinFinals.Iou,
                "ㄨㄞ" => PinyinFinals.Uai,
                "ㄨㄟ" => PinyinFinals.Uei,
                "丨ㄢ" or "ㄧㄢ" => PinyinFinals.Ian,
                "ㄨㄢ" => PinyinFinals.Uan,
                "ㄩㄢ" => PinyinFinals.Van,
                "丨ㄣ" or "ㄧㄣ" => PinyinFinals.In,
                "ㄨㄣ" => PinyinFinals.Uen,
                "ㄩㄣ" => PinyinFinals.Vn,
                "丨ㄤ" or "ㄧㄤ" => PinyinFinals.Iang,
                "ㄨㄤ" => PinyinFinals.Uang,
                "丨ㄥ" or "ㄧㄥ" => PinyinFinals.Ing,
                "ㄨㄥ" => PinyinFinals.Ong,
                "ㄩㄥ" => PinyinFinals.Iong,
                "aa" => PinyinFinals.A,
                "aŋ" => PinyinFinals.Ang,
                "eh" => PinyinFinals.E,
                "eŋ" => PinyinFinals.Eng,
                "iu" => PinyinFinals.Iou,
                "iŋ" => PinyinFinals.Ing,
                "oŋ" => PinyinFinals.Ong,
                "vŋ" or "üŋ" => PinyinFinals.Iong,
                "yi" => PinyinFinals.I,
                "yu" => PinyinFinals.V,
                "ui" => PinyinFinals.Uei,
                "un" => PinyinFinals.Uen,
                "üe" => PinyinFinals.Ve,
                "ün" => PinyinFinals.Vn,
                "wu" => PinyinFinals.U,
                "ng" => PinyinFinals.Ng,
                _ => null
            },
            3 => s.ToLowerInvariant() switch
            {
                "ioŋ" or "vng" or "üng" => PinyinFinals.Iong,
                "üan" => PinyinFinals.Van,
                "yin" => PinyinFinals.In,
                "yue" => PinyinFinals.Ve,
                "yun" => PinyinFinals.Vn,
                _ => null
            },
            4 => s.ToLowerInvariant() switch
            {
                "weng" => PinyinFinals.Ueng,
                "ying" => PinyinFinals.Ing,
                "yong" => PinyinFinals.Iong,
                "yuan" => PinyinFinals.Van,
                _ => null
            },
            _ => null,
        };
    }

    /// <summary>
    /// Tries to parse a Pinyin final.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="result">A Pinyin final parsed</param>
    /// <returns>true parse succeeded; otherwise, false.</returns>
    public static bool TryParseFinal(string s, out PinyinFinals result)
    {
        var r = TryParseFinal(s);
        if (r.HasValue)
        {
            result = r.Value;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Formats the Pinyin of a Chinese character.
    /// </summary>
    /// <param name="s">The Pinyin of a Chinese character to format.</param>
    /// <returns>A string formatted.</returns>
    /// <exception cref="FormatException">s was not in correct format.</exception>
    public static string Format(string s)
    {
        if (s == null) return string.Empty;
        if (s.Length < 2) return s;
        var e = s.GetEnumerator();
        var i = -1;
        PinyinInitials? initial = null;
        var final = new StringBuilder();
        var result = new StringBuilder();
        var lastKind = 0;
        var ignoreMove = false;
        var tone = 0;
        var cap = char.IsUpper(s[0]);
        while (true)
        {
            if (!ignoreMove)
            {
                if (!e.MoveNext()) break;
                i++;
            }

            ignoreMove = false;
            var c = e.Current;
            var k = GetKind(c);
            i++;
            if (k == 0)
            {
                tone = 0;
                if (c == '\0') break;
                if (c == '\b')
                {
                    if (final.Length > 0) final.Remove(final.Length - 1, 1);
                    continue;
                }

                if (c == '\a' && final.Length == 0 && !initial.HasValue)
                {
                    if (lastKind > 0) result.Append(' ');
                    lastKind = 8;
                    continue;
                }

                throw new FormatException($"s contains an unsupported character at position {i} which is {c}.");
            }

            if (k == 8)
            {
                if (lastKind != 8) Append(result, initial, final.ToString(), tone, i, c, cap);
                final.Clear();
                tone = 0;
                if (lastKind == 8 && (c == ' ' || c == '　') && result.Length > 0)
                {
                    var last = result[result.Length - 1];
                    if (last != ' ' && last != '　' && last != '\t')
                        result.Append(c);
                }
                else
                {
                    result.Append(c);
                }

                initial = null;
                if (" 　\t\n\r\"“‘《<（(【[*！？!?·]】)）>》’”".Contains(c))
                {
                    if (!e.MoveNext()) break;
                    i++;
                    ignoreMove = true;
                    var next = e.Current;
                    if (GetKind(next) < 8) cap = char.IsUpper(next);
                }
                else if (lastKind != 8)
                {
                    cap = false;
                }

                lastKind = k;
                continue;
            }

            if (k == 5)
            {
                tone = 0;
                if (initial.HasValue || final.Length > 0)
                    throw new FormatException($"s contains an obsolete initial notation at position {i} which is {c}.");
                if (lastKind == 0)
                {
                    result.Append(c switch
                    {
                        '兀' => "Ng",
                        '广' => "Gn",
                        _ => string.Empty
                    });
                    continue;
                }

                var w = c switch
                {
                    '兀' => "ng",
                    '广' => "gn",
                    _ => null
                };
                if (w == null) continue;
                if (lastKind > 0 && lastKind < 8) result.Append(' ');
                result.Append(w);
                lastKind = 8;
                continue;
            }

            if (k == 4)
            {
                if (lastKind == 4)
                    throw new FormatException($"s contains an initial following another at position {i} which is {c}.");
                Append(result, initial, final.ToString(), tone, i, c, cap);
                final.Clear();
                tone = 0;
                if (lastKind != 8 && lastKind != 0) cap = false;
                lastKind = k;
                if (c == 'z' || c == 'c' || c == 's' || c == 'Z' || c == 'C' || c == 'S')
                {
                    if (!e.MoveNext()) break;
                    i++;
                    var next = e.Current;
                    if (next == 'h' || next == 'H')
                    {
                        i++;
                        initial = c switch
                        {
                            'z' or 'Z' => PinyinInitials.Zh,
                            'c' or 'C' => PinyinInitials.Ch,
                            's' or 'S' => PinyinInitials.Sh,
                            _ => PinyinInitials.H,
                        };
                    }
                    else
                    {
                        initial = c switch
                        {
                            'z' or 'Z' => PinyinInitials.Z,
                            'c' or 'C' => PinyinInitials.C,
                            _ => PinyinInitials.S,
                        };
                        ignoreMove = true;
                    }

                    continue;
                }

                var iniParsed = TryParseInitial(c.ToString());
                if (!iniParsed.HasValue)
                    throw new FormatException($"s contains an incorrect initial at position {i} which is {c}.");
                initial = iniParsed.Value;
                continue;
            }

            if (k == 1)
            {
                if (lastKind == 8)
                {
                    tone = 0;
                    final.Clear();
                }

                final.Append(c);
                lastKind = k;
                continue;
            }

            if (k == 2)
            {
                var j = "āáǎàōóòòēéěèīíǐìūúǔùǖǘǚǜĀÁǍÀŌÓÒÒĒÉĚÈĪÍǏÌŪÚǓÙǕǗǙǛ".IndexOf(c);
                tone = j % 4 + 1;
                c = (j / 4 % 6) switch
                {
                    0 => 'a',
                    1 => 'o',
                    2 => 'e',
                    3 => 'i',
                    4 => 'u',
                    5 => 'v',
                    _ => ' '
                };
                final.Append(c);
                lastKind = k;
                continue;
            }

            if (k == 3)
            {
                tone = GetTone(c);
                if (lastKind == 4 && final.Length == 0)
                {
                    if (!initial.HasValue)
                        throw new FormatException($"s contains a tone unexpected at position {i} which is {c}.");
                    final.Append(initial.Value switch
                    {
                        PinyinInitials.B or PinyinInitials.P or PinyinInitials.M or PinyinInitials.F => 'o',
                        PinyinInitials.D or PinyinInitials.T or PinyinInitials.N or PinyinInitials.L or PinyinInitials.G or PinyinInitials.K or PinyinInitials.H => 'e',
                        PinyinInitials.J or PinyinInitials.Q or PinyinInitials.X or PinyinInitials.Zh or PinyinInitials.Ch or PinyinInitials.Sh or PinyinInitials.R or PinyinInitials.Z or PinyinInitials.C or PinyinInitials.S or PinyinInitials.Y => 'i',
                        PinyinInitials.W => 'w',
                        _ => throw new FormatException($"s contains a tone after an initial at position {i} which is {c}.")
                    });
                }
                else if (lastKind == 8)
                {
                    tone = 0;
                    result.Append(c);
                }
                else if (lastKind == 5)
                {
                    throw new FormatException($"s contains a tone after an invalid character at position {i} which is {c}.");
                }

                lastKind = 3;
                continue;
            }

            if (c == 'ㄨ')
            {
                if (lastKind == 4 || lastKind == 6 || lastKind == 7)
                {
                    final.Append('u');
                    lastKind = 1;
                }
                else
                {
                    Append(result, initial, final.ToString(), tone, i, c, cap);
                    final.Clear();
                    tone = 0;
                    initial = PinyinInitials.W;
                    cap = false;
                    lastKind = 4;
                }

                continue;
            }

            if (c == 'r' || c == 'R' || c == 'ㄖ')
            {
                if (lastKind > 3 || lastKind == 0)
                {
                    Append(result, initial, final.ToString(), tone, i, c, cap);
                    tone = 0;
                    final.Clear();
                    initial = PinyinInitials.R;
                    lastKind = 4;
                }
                else
                {
                    if (!e.MoveNext()) break;
                    i++;
                    ignoreMove = true;
                    var next = e.Current;
                    var nextKind = GetKind(next);
                    if (nextKind == 3)
                    {
                        Append(result, initial, final.ToString(), GetTone(next), i, c, cap);
                        tone = 0;
                        final.Clear();
                        result.Append('r');
                        lastKind = 1;
                        initial = null;
                    }
                    else if (nextKind > 3)
                    {
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        tone = 0;
                        final.Clear();
                        result.Append('r');
                        lastKind = 1;
                        initial = null;
                    }
                    else
                    {
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        tone = 0;
                        final.Clear();
                        initial = PinyinInitials.R;
                        lastKind = 4;
                    }
                }

                cap = false;
                continue;
            }

            if (c == '声' || c == '平') continue;
            if (k == 7 && final.Length < 1)
            {
                ignoreMove = true;
                if (c == '兀')
                {
                    final.Append("ng");
                    if (!e.MoveNext()) break;
                    i++;
                    var nextKind = GetKind(e.Current);
                    if (nextKind == 3)
                    {
                        tone = GetTone(e.Current);
                        ignoreMove = false;
                    }
                    else
                    {
                        tone = 0;
                    }

                    Append(result, initial, final.ToString(), tone, i, c, cap);
                    final.Clear();
                    tone = 0;
                    continue;
                }

                tone = c switch
                {
                    'ń' or 'Ń' => 2,
                    'ň' or 'Ň' => 3,
                    'ǹ' or 'Ǹ' => 4,
                    _ => 0
                };
                if (!e.MoveNext())
                {
                    final.Append('n');
                    break;
                }

                i++;
                var next = e.Current;
                if (next == 'g' || next == 'G')
                {
                    final.Append("ng");
                    if (!e.MoveNext()) break;
                    i++;
                    next = e.Current;
                    var nextKind = GetKind(next);
                    if (nextKind == 3)
                    {
                        tone = GetTone(next);
                        ignoreMove = false;
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        lastKind = 8;
                        initial = null;
                    }
                    else if (nextKind < 3)
                    {
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        lastKind = 4;
                        initial = PinyinInitials.G;
                    }
                    else
                    {
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        lastKind = 8;
                        initial = null;
                    }

                    final.Clear();
                    tone = 0;
                    continue;
                }
            }

            if (lastKind == 3 || lastKind == 6 || lastKind == 7)
            {
                Append(result, initial, final.ToString(), tone, i, c, cap);
                final.Clear();
                tone = 0;
                cap = false;
                var iniParsed = TryParseInitial(c.ToString());
                if (!iniParsed.HasValue)
                    throw new FormatException($"s contains an invalid character at position {i} which is {c}.");
                initial = iniParsed.Value;
                lastKind = 4;
                continue;
            }

            if (lastKind == 8 || lastKind == 0)
            {
                if (c == 'ŋ' || c == 'Ŋ')
                {
                    lastKind = 0;
                    result.Append("ng");
                    continue;
                }

                lastKind = 4;
                var iniParsed = TryParseInitial(c.ToString());
                if (!iniParsed.HasValue)
                    throw new FormatException($"s contains an invalid character at position {i} which is {c}.");
                initial = iniParsed.Value;
                continue;
            }

            if (c == 'n' || c == 'N' || c == 'ㄋ')
            {
                ignoreMove = true;
                if (!e.MoveNext())
                {
                    final.Append('n');
                    break;
                }

                i++;
                var next = e.Current;
                if (next == 'g' || next == 'G' || next == 'ㄍ')
                {
                    if (!e.MoveNext())
                    {
                        final.Append("ng");
                        break;
                    }

                    i++;
                    next = e.Current;
                    var nextKind = GetKind(next);
                    if (nextKind == 3 && final.Length < 1)
                    {
                        final.Append("ng");
                        tone = GetTone(next);
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        final.Clear();
                        tone = 0;
                        lastKind = 1;
                    }
                    else if (nextKind > 2)
                    {
                        final.Append("ng");
                        lastKind = 1;
                    }
                    else
                    {
                        final.Append('n');
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        tone = 0;
                        final.Clear();
                        initial = PinyinInitials.G;
                        cap = false;
                        lastKind = 4;
                    }
                }
                else if (lastKind == 8 || lastKind == 0)
                {
                    lastKind = 4;
                    ignoreMove = true;
                    initial = PinyinInitials.N;
                    continue;
                }
                else
                {
                    var nextKind = GetKind(next);
                    if (nextKind > 2)
                    {
                        final.Append('n');
                        lastKind = 1;
                    }
                    else
                    {
                        Append(result, initial, final.ToString(), tone, i, c, cap);
                        tone = 0;
                        final.Clear();
                        initial = PinyinInitials.N;
                        cap = false;
                        lastKind = 4;
                    }
                }

                continue;
            }

            if (c == 'y' || c == 'Y' || c == '丨' || c == 'ㄧ')
            {
                if (lastKind == 4)
                {
                    final.Append('y');
                    lastKind = 1;
                }
                else
                {
                    Append(result, initial, final.ToString(), tone, i, c, cap);
                    tone = 0;
                    final.Clear();
                    initial = PinyinInitials.Y;
                    cap = false;
                    lastKind = 4;
                }

                continue;
            }

            if (c == 'ŋ' || c == 'Ŋ')
            {
                if (lastKind > 2)
                    throw new FormatException($"s contains an invalid character at position {i} which is {c}.");
                final.Append("ng");
                lastKind = 1;
                continue;
            }
        }

        Append(result, initial, final.ToString(), tone, s.Length - 1, s[s.Length - 1], cap);
        if (result.Length > 0 && result.Length < 8 && s.Length > 0 && "abcdefghijklmnopqrstuvwxyzāáǎàōóòòēéěèêīíǐìūúǔùǖǘǚǜŋẑĉŝ".Contains(s[0]))
        {
            i = "ABCDEFGHIJKLMNOPQRSTUVWXYZĀÁǍÀŌÓÒÒĒÉĚÈÊĪÍǏÌŪÚǓÙǕǗǙǛ".IndexOf(result[0]);
            if (i >= 0)
            {
                result.Remove(0, 1);
                result.Insert(0, "abcdefghijklmnopqrstuvwxyzāáǎàōóòòēéěèêīíǐìūúǔùǖǘǚǜ"[i]);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts the Han character to a short number.
    /// </summary>
    /// <param name="initial">The Pinyin initial.</param>
    /// <param name="final">The Pinyin final.</param>
    /// <param name="tone">The tone.</param>
    /// <returns>A short number with initial and final information.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The initial, final or tone is out of range.</exception>
    public static short ZipToShort(PinyinInitials initial, PinyinFinals final, int tone = 0)
    {
        if (tone < 0 || tone > 7) throw new ArgumentOutOfRangeException(nameof(tone), "tone should be in 0 and 4.");
        var a = (byte)initial;
        var b = (byte)final;
        if (a > 63) throw new ArgumentOutOfRangeException(nameof(initial), "initial is not supported.");
        if (b > 63) throw new ArgumentOutOfRangeException(nameof(final), "final is not supported.");
        var r = a * 512 + tone * 64 + b;
        if (r < 0 || r > short.MaxValue) throw new ArgumentOutOfRangeException(nameof(tone), "initial or final is not supported.");
        return (short)r;
    }

    /// <summary>
    /// Converts the Han character to a short number.
    /// </summary>
    /// <param name="final">The Pinyin final.</param>
    /// <param name="tone">The tone.</param>
    /// <returns>A short number with initial and final information.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The initial, final or tone is out of range.</exception>
    public static short ZipToShort(PinyinFinals final, int tone = 0)
    {
        if (tone < 0 || tone > 7) throw new ArgumentOutOfRangeException(nameof(tone), "tone should be in 0 and 4.");
        var b = (byte)final;
        if (b > 63) throw new ArgumentOutOfRangeException(nameof(final), "final is not supported.");
        var r = tone * 64 + b;
        if (r < 0 || r > short.MaxValue) throw new ArgumentOutOfRangeException(nameof(tone), "initial or tone is not supported.");
        return (short)r;
    }

    /// <summary>
    /// Converts a short number back to Pinyin.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="initial">The Pinyin initial.</param>
    /// <param name="final">The Pinyin final.</param>
    /// <param name="tone">The tone.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool Unzip(short value, out PinyinInitials initial, out PinyinFinals final, out int tone)
    {
        int a = value;
        try
        {
            if (a > 0)
            {
                final = (PinyinFinals)(a % 64);
                a >>= 6;
                tone = a % 8;
                a >>= 3;
                initial = (PinyinInitials)a;
                return a > 63;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (ArithmeticException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (InvalidCastException)
        {
        }

        initial = 0;
        final = 0;
        tone = 0;
        return false;
    }

    /// <summary>
    /// Gets kind of the specific character.
    /// </summary>
    /// <param name="c">The character to test.</param>
    /// <returns>
    /// <list>
    /// <item><term>0</term><description>Unknown</description></item>
    /// <item><term>1</term><description>Vowels</description></item>
    /// <item><term>2</term><description>Finals with tone.</description></item>
    /// <item><term>3</term><description>Tones</description></item>
    /// <item><term>4</term><description>Initials and letter w but except letters rgny</description></item>
    /// <item><term>5</term><description>Obsolete initial notations</description></item>
    /// <item><term>6</term><description>Letters grŋy</description></item>
    /// <item><term>7</term><description>Letter n</description></item>
    /// <item><term>8</term><description>Seperators</description></item>
    /// </list>
    /// </returns>
    private static int GetKind(char c)
    {
        if ("aoeêiuvüAOEÊIUVÜㄚㄛㄜㄝㄩㄦ儿ㄞㄟㄠㄡㄢㄣㄤㄥ".Contains(c)) return 1;
        if ("āáǎàōóòòēéěèīíǐìūúǔùǖǘǚǜĀÁǍÀŌÓÒÒĒÉĚÈĪÍǏÌŪÚǓÙǕǗǙǛ".Contains(c)) return 2;
        if ("012345¯ˊˇˋ˙轻阴阳上去仄".Contains(c)) return 3;
        if ("bpmfdtlgkhjqxzcsẑĉŝwBPMFDTLGKHJQXZCSẐĈŜWㄅㄆㄇㄈㄉㄊㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙ".Contains(c)) return 4;
        if ("广万".Contains(c)) return 5;
        if ("rŋyRŊY丨ㄧㄋㄖㄨ".Contains(c)) return 6;
        if ("nńňǹNŃŇǸ兀".Contains(c)) return 7;
        if ("' 　\\\t\r\n\"`“‘’”-,.;?¿!&^(（【[{《<￥$%@/·|}>》]】）)_—=+#*，。゜、；？！~…".Contains(c)) return 8;
        return 0;
    }

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="caseOptions">Optional case.</param>
    /// <param name="rest">The rest string.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    private static string ToFinalString(char final, int tone, Cases caseOptions, string rest)
        => AppendFinal(new StringBuilder(), final, tone, caseOptions, rest).ToString();

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="prefix">The first characters.</param>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="caseOptions">Optional case.</param>
    /// <param name="rest">The rest string.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    private static string ToFinalString(string prefix, char final, int tone, Cases caseOptions, string rest)
    {
        var sb = new StringBuilder(prefix.ToSpecificCaseInvariant(caseOptions));
        return AppendFinal(sb, final, tone, caseOptions == Cases.Upper ? Cases.Upper : Cases.Lower, rest).ToString();
    }

    /// <summary>
    /// Gets the tone.
    /// </summary>
    /// <param name="c">The tone.</param>
    /// <returns>The tone.</returns>
    private static int GetTone(char c)
        => "012345 ¯ˊˇˋ˙轻阴阳上去仄".IndexOf(c) % 6;

    /// <summary>
    /// Returns a System.String that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="sb">The string builder to append.</param>
    /// <param name="initial">The Pinyin initial.</param>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="offset">The offset to read.</param>
    /// <param name="c">The character to read.</param>
    /// <param name="cap">true if the first letter should be upper case; otherwise, false.</param>
    /// <returns>A string that represents the specific character in Pinyin.</returns>
    private static void Append(StringBuilder sb, PinyinInitials? initial, string final, int tone, int offset, char c, bool cap)
    {
        var f = TryParseFinal(final);
        var caseOptions = cap ? Cases.Capitalize : Cases.Lower;
        if (cap)
        {
        }
        else if (sb.Length > 1)
        {
            var last = sb[sb.Length - 1];
            switch (last)
            {
                case '。':
                case '？':
                case '！':
                case '…':
                case '\r':
                case '\n':
                case '\t':
                case '\0':
                    caseOptions = Cases.Capitalize;
                    break;
                case ' ':
                case '　':
                case '.':
                case '?':
                case '¿':
                case '!':
                    last = sb[sb.Length - 2];
                    switch (last)
                    {
                        case '。':
                        case '？':
                        case '！':
                        case '…':
                        case '.':
                        case '?':
                        case '¿':
                        case '!':
                        case '\r':
                        case '\n':
                        case '\t':
                        case '\0':
                            caseOptions = Cases.Capitalize;
                            break;
                    }

                    break;
            }
        }
        else if (sb.Length < 1)
        {
            caseOptions = Cases.Capitalize;
        }

        if (tone < 0) tone = 0;
        if (f.HasValue)
        {
            var s = initial.HasValue ? ToString(initial.Value, f.Value, tone, caseOptions) : ToString(f.Value, tone, caseOptions);
            sb.Append(s);
            return;
        }
        else
        {
            if (string.IsNullOrEmpty(final) && !initial.HasValue) return;
        }

        for (var i = final.Length - 1; i > 0; i--)
        {
            var finalA = final.Substring(0, i);
            f = TryParseFinal(finalA);
            if (f.HasValue)
            {
                var s = initial.HasValue ? ToString(initial.Value, f.Value, 0) : ToString(f.Value, 0, caseOptions);
                sb.Append(s);
                final = final.Substring(i);
                Append(sb, null, final, tone, offset, c, false);
                return;
            }
        }

        if (string.IsNullOrEmpty(final)) final = c.ToString();
        throw new FormatException($"s contains an incorrect final before position {offset} which is {final}.");
    }

    /// <summary>
    /// Append the string that represents the specific character in Pinyin.
    /// </summary>
    /// <param name="sb">The string builder instance.</param>
    /// <param name="final">The pinyin final.</param>
    /// <param name="tone">Optional tone.</param>
    /// <param name="caseOptions">Optional case.</param>
    /// <param name="rest">The rest string.</param>
    /// <returns>A string builder that represents the specific character in Pinyin.</returns>
    private static StringBuilder AppendFinal(StringBuilder sb, char final, int tone, Cases caseOptions, string rest)
    {
        if (caseOptions == Cases.Upper || caseOptions == Cases.Capitalize)
        {
            sb.Append(final switch
            {
                'a' => tone switch
                {
                    1 => CapitalA1,
                    2 => CapitalA2,
                    3 => CapitalA3,
                    4 => CapitalA4,
                    _ => CapitalA
                },
                'o' => tone switch
                {
                    1 => CapitalO1,
                    2 => CapitalO2,
                    3 => CapitalO3,
                    4 => CapitalO4,
                    _ => CapitalO
                },
                'e' => tone switch
                {
                    1 => CapitalE1,
                    2 => CapitalE2,
                    3 => CapitalE3,
                    4 => CapitalE4,
                    _ => CapitalE
                },
                'i' => tone switch
                {
                    1 => CapitalI1,
                    2 => CapitalI2,
                    3 => CapitalI3,
                    4 => CapitalI4,
                    _ => CapitalI
                },
                'u' => tone switch
                {
                    1 => CapitalU1,
                    2 => CapitalU2,
                    3 => CapitalU3,
                    4 => CapitalU4,
                    _ => CapitalU
                },
                'v' => tone switch
                {
                    1 => CapitalV1,
                    2 => CapitalV2,
                    3 => CapitalV3,
                    4 => CapitalV4,
                    _ => CapitalV
                },
                'n' => tone switch
                {
                    1 => CapitalN,
                    2 => CapitalN2,
                    3 => CapitalN3,
                    4 => CapitalN4,
                    _ => CapitalN
                },
                _ => string.Empty
            });
        }
        else
        {
            sb.Append(final switch
            {
                'a' => tone switch
                {
                    1 => SmallA1,
                    2 => SmallA2,
                    3 => SmallA3,
                    4 => SmallA4,
                    _ => SmallA
                },
                'o' => tone switch
                {
                    1 => SmallO1,
                    2 => SmallO2,
                    3 => SmallO3,
                    4 => SmallO4,
                    _ => SmallO
                },
                'e' => tone switch
                {
                    1 => SmallE1,
                    2 => SmallE2,
                    3 => SmallE3,
                    4 => SmallE4,
                    _ => SmallE
                },
                'i' => tone switch
                {
                    1 => SmallI1,
                    2 => SmallI2,
                    3 => SmallI3,
                    4 => SmallI4,
                    _ => SmallI
                },
                'u' => tone switch
                {
                    1 => SmallU1,
                    2 => SmallU2,
                    3 => SmallU3,
                    4 => SmallU4,
                    _ => SmallU
                },
                'v' => tone switch
                {
                    1 => SmallV1,
                    2 => SmallV2,
                    3 => SmallV3,
                    4 => SmallV4,
                    _ => SmallV
                },
                'n' => tone switch
                {
                    1 => SmallN,
                    2 => SmallN2,
                    3 => SmallN3,
                    4 => SmallN4,
                    _ => SmallN
                },
                _ => string.Empty
            });
        }

        sb.Append(caseOptions == Cases.Upper ? rest?.ToUpperInvariant() : rest);
        return sb;
    }


    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="s">The number string.</param>
    /// <returns>A string for the number.</returns>
    private static string ToNumberString(string s)
        => s.Replace("空", "kōng")
            .Replace("正", "zhèng")
            .Replace("负", "fù")
            .Replace("点", "diǎn")
            .Replace("无穷", "wúqióng")
            .Replace("半", "bàn")
            .Replace("零", "lǐng")
            .Replace("一", "yī")
            .Replace("两", "liǎng")
            .Replace("二", "èr")
            .Replace("三", "sān")
            .Replace("四", "sì")
            .Replace("五", "wŭ")
            .Replace("六", "liù")
            .Replace("七", "qī")
            .Replace("八", "bā")
            .Replace("九", "jiŭ")
            .Replace("十", "shí")
            .Replace("百", "bǎi")
            .Replace("千", "qiān")
            .Replace("万", "wàn")
            .Replace("亿", "yì")
            .Replace("兆", "zhào")
            .Replace("吉", "jí")
            .Replace("太", "tài")
            .Replace("整", "zhěng")
            .Replace("的", "de")
            .Replace("乘", "chéng")
            .Replace("以", "yǐ")
            .Replace("幂", "mì")
            .Replace("次", "cì")
            .Replace("方", "fāng")
            .Replace("分之", "fēnzhī")
            .Replace("不是数字", "būshìshùzì");
    
    #pragma warning restore IDE0056, IDE0057
}
