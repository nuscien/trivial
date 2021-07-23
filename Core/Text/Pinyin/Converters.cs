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

namespace Trivial.Text
{
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
                _ => (int)final == 6
                ? (tone == 0 ? (caseOptions == Cases.Upper || caseOptions == Cases.Capitalize ? CapitalEh : SmallEh) : ToFinalString('e', tone, caseOptions, "i"))
                : string.Empty
            };
        }

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
            if (s == null) throw new ArgumentNullException(nameof(s), "s was null.");
            if (s.Length < 1) throw new ArgumentNullException(nameof(s), "s was empty.");
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
            if (s == null) throw new ArgumentNullException(nameof(s), "s should not be null.");
            if (s.Length < 1) throw new ArgumentException(nameof(s), "s should not be  empty.");
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
                    "ㄜ" or "ㄝ" or "Ê" or "ê" => PinyinFinals.E,
                    "丨" or "ㄧ" => PinyinFinals.I,
                    "ㄨ" => PinyinFinals.U,
                    "ㄩ" or "Ü" or "ü" => PinyinFinals.V,
                    "ㄦ" or "儿" or "r" => PinyinFinals.Er,
                    "ㄞ" => PinyinFinals.Ai,
                    "ㄟ" => PinyinFinals.Ei,
                    "ㄠ" => PinyinFinals.Ao,
                    "ㄡ" => PinyinFinals.Ou,
                    "ㄢ" => PinyinFinals.An,
                    "ㄣ" => PinyinFinals.En,
                    "ㄤ" => PinyinFinals.Ang,
                    "ㄥ" => PinyinFinals.Eng,
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
            while (true)
            {
                if (!ignoreMove)
                {
                    if (!e.MoveNext()) break;
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
                        lastKind = 7;
                        continue;
                    }

                    throw new FormatException($"s contains an unsupported character at position {i} which is {c}.");
                }

                if (k == 7)
                {
                    Append(result, initial, final.ToString(), tone, i, c);
                    final.Clear();
                    tone = 0;
                    if (lastKind == 7 && (c == ' ' || c == '　') && result.Length > 0)
                    {
                        var last = result[result.Length - 1];
                        if (last != ' ' && last != '　' && last != '\t')
                            result.Append(c);
                    }
                    else
                    {
                        result.Append(c);
                    }

                    lastKind = k;
                    initial = null;
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
                    if (lastKind > 0 && lastKind < 7) result.Append(' ');
                    result.Append(w);
                    lastKind = 7;
                    continue;
                }

                if (k == 4)
                {
                    if (lastKind == 4)
                        throw new FormatException($"s contains an initial following another at position {i} which is {c}.");
                    Append(result, initial, final.ToString(), tone, i, c);
                    final.Clear();
                    tone = 0;
                    lastKind = k;
                    if (c == 'z' || c == 'c' || c == 's' || c == 'Z' || c == 'C' || c == 'S')
                    {
                        if (!e.MoveNext()) break;
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
                    if (lastKind == 7)
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
                    final.Append((j / 4 % 6) switch
                    {
                        0 => 'a',
                        1 => 'o',
                        2 => 'e',
                        3 => 'i',
                        4 => 'u',
                        5 => 'v',
                        _ => ' '
                    });
                    lastKind = k;
                    continue;
                }

                if (k == 3)
                {
                    tone = "012345 ¯ˊˇˋ˙轻阴阳上去仄".IndexOf(c) % 6;
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
                    else if (lastKind == 7)
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
                    if (lastKind == 4 || lastKind == 6)
                    {
                        final.Append('u');
                        lastKind = 1;
                    }
                    else
                    {
                        Append(result, initial, final.ToString(), tone, i, c);
                        final.Clear();
                        tone = 0;
                        initial = PinyinInitials.W;
                        lastKind = 4;
                    }

                    continue;
                }

                if (c == 'r' || c == 'R' || c == 'ㄖ')
                {
                    if (lastKind > 3)
                    {
                        Append(result, initial, final.ToString(), tone, i, c);
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
                            Append(result, initial, final.ToString(), "012345 ¯ˊˇˋ˙轻阴阳上去仄".IndexOf(next) % 6, i, c);
                            tone = 0;
                            final.Clear();
                            result.Append('r');
                            lastKind = 1;
                            initial = null;
                        }
                        else if (nextKind > 3)
                        {
                            Append(result, initial, final.ToString(), tone, i, c);
                            tone = 0;
                            final.Clear();
                            result.Append('r');
                            lastKind = 1;
                            initial = null;
                        }
                        else
                        {
                            Append(result, initial, final.ToString(), tone, i, c);
                            tone = 0;
                            final.Clear();
                            initial = PinyinInitials.R;
                            lastKind = 4;
                        }
                    }

                    continue;
                }

                if (c == '声' || c == '平') continue;
                if (lastKind == 3 || lastKind == 6)
                {
                    Append(result, initial, final.ToString(), tone, i, c);
                    final.Clear();
                    tone = 0;
                    var iniParsed = TryParseInitial(c.ToString());
                    if (!iniParsed.HasValue)
                        throw new FormatException($"s contains an invalid character at position {i} which is {c}.");
                    initial = iniParsed.Value;
                    lastKind = 4;
                    continue;
                }

                if (lastKind == 7)
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
                    if (!e.MoveNext()) break;
                    var next = e.Current;
                    if (next == 'g' || next == 'G' || next == 'ㄍ')
                    {
                        if (!e.MoveNext()) break;
                        i++;
                        next = e.Current;
                        var nextKind = GetKind(next);
                        if (nextKind > 2)
                        {
                            final.Append("ng");
                            lastKind = 1;
                        }
                        else
                        {
                            final.Append('n');
                            Append(result, initial, final.ToString(), tone, i, c);
                            tone = 0;
                            final.Clear();
                            initial = PinyinInitials.G;
                            lastKind = 4;
                        }
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
                            Append(result, initial, final.ToString(), tone, i, c);
                            tone = 0;
                            final.Clear();
                            initial = PinyinInitials.N;
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
                        Append(result, initial, final.ToString(), tone, i, c);
                        tone = 0;
                        final.Clear();
                        initial = PinyinInitials.Y;
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

            Append(result, initial, final.ToString(), tone, s.Length - 1, s[s.Length - 1]);
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
        /// <item><term>6</term><description>Letters ngrŋy</description></item>
        /// <item><term>7</term><description>Seperators</description></item>
        /// </list>
        /// </returns>
        private static int GetKind(char c)
        {
            if ("aoeêiuvüAOEÊIUVÜㄚㄛㄜㄝㄩㄦ儿ㄞㄟㄠㄡㄢㄣㄤㄥ".Contains(c)) return 1;
            if ("āáǎàōóòòēéěèīíǐìūúǔùǖǘǚǜĀÁǍÀŌÓÒÒĒÉĚÈĪÍǏÌŪÚǓÙǕǗǙǛ".Contains(c)) return 2;
            if ("012345¯ˊˇˋ˙轻阴阳上去仄".Contains(c)) return 3;
            if ("bpmfdtlgkhjqxzcsẑĉŝwBPMFDTLGKHJQXZCSẐĈŜWㄅㄆㄇㄈㄉㄊㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙ".Contains(c)) return 4;
            if ("兀广万".Contains(c)) return 5;
            if ("nrŋyNRŊY丨ㄧㄋㄖㄨ".Contains(c)) return 6;
            if ("' 　\\\t\r\n\"`“‘’”-,.;?¿!&^(（【[{《<￥$%@/·|}>》]】）)_—=+，。゜、；？！~…".Contains(c)) return 7;
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
        /// Returns a System.String that represents the specific character in Pinyin.
        /// </summary>
        /// <param name="sb">The string builder to append.</param>
        /// <param name="initial">The Pinyin initial.</param>
        /// <param name="final">The pinyin final.</param>
        /// <param name="tone">Optional tone.</param>
        /// <param name="offset">The offset to read.</param>
        /// <param name="c">The character to read.</param>
        /// <returns>A string that represents the specific character in Pinyin.</returns>
        private static void Append(StringBuilder sb, PinyinInitials? initial, string final, int tone, int offset, char c)
        {
            var f = TryParseFinal(final);
            var caseOptions = Cases.Lower;
            if (sb.Length > 1)
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
                    Append(sb, null, final, tone, offset, c);
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
                    _ => string.Empty
                });
            }

            sb.Append(caseOptions == Cases.Upper ? rest?.ToUpperInvariant() : rest);
            return sb;
        }

        #pragma warning restore IDE0056, IDE0057
    }
}
