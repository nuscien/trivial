using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The Roman numerals.
    /// </summary>
    public class RomanNumerals : IIntegerSample
    {
        /// <summary>
        /// Roman numbers.
        /// </summary>
        public enum Numbers
        {
            /// <summary>
            /// One, I.
            /// </summary>
            I = 1,

            /// <summary>
            /// Five, V.
            /// </summary>
            V = 5,

            /// <summary>
            /// Ten, X.
            /// </summary>
            X = 10,

            /// <summary>
            /// Fifty, L.
            /// </summary>
            L = 50,

            /// <summary>
            /// One hundred, C.
            /// </summary>
            C = 100,

            /// <summary>
            /// Five hundred, D.
            /// </summary>
            D = 500,

            /// <summary>
            /// Ten hundred, M.
            /// </summary>
            M = 1000
        }

        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => false;

        /// <summary>
        /// Initializes a new instance of the RomanNumber class.
        /// </summary>
        /// <param name="lowerCase">true if use lower case; otherwise, false.</param>
        protected internal RomanNumerals(bool lowerCase = false)
        {
            IsLowerCase = lowerCase;
        }

        /// <summary>
        /// Gets a value indicating whether it is in lower case.
        /// </summary>
        public bool IsLowerCase { get; }

        /// <summary>
        /// Number 0.
        /// </summary>
        public string Zero => " ";

        /// <summary>
        /// Number 1.
        /// </summary>
        public string One => IsLowerCase ? "ⅰ" : "Ⅰ";

        /// <summary>
        /// Number 2.
        /// </summary>
        public string Two => IsLowerCase ? "ⅱ" : "Ⅱ";

        /// <summary>
        /// Number 3.
        /// </summary>
        public string Three => IsLowerCase ? "ⅲ" : "Ⅲ";

        /// <summary>
        /// Number 4.
        /// </summary>
        public string Four => IsLowerCase ? "ⅳ" : "Ⅳ";

        /// <summary>
        /// Number 5.
        /// </summary>
        public string Five => IsLowerCase ? "ⅴ" : "Ⅴ";

        /// <summary>
        /// Number 6.
        /// </summary>
        public string Six => IsLowerCase ? "ⅵ" : "Ⅵ";

        /// <summary>
        /// Number 7.
        /// </summary>
        public string Seven => IsLowerCase ? "ⅶ" : "Ⅶ";

        /// <summary>
        /// Number 8.
        /// </summary>
        public string Eight => IsLowerCase ? "ⅷ" : "Ⅷ";

        /// <summary>
        /// Number 9.
        /// </summary>
        public string Nine => IsLowerCase ? "ⅸ" : "Ⅸ";

        /// <summary>
        /// Number 10.
        /// </summary>
        public string Ten => IsLowerCase ? "ⅹ" : "Ⅹ";

        /// <summary>
        /// Number 11.
        /// </summary>
        public string Eleven => IsLowerCase ? "ⅺ" : "Ⅺ";

        /// <summary>
        /// Number 12.
        /// </summary>
        public string Twelve => IsLowerCase ? "ⅻ" : "Ⅻ";

        /// <summary>
        /// Number 13.
        /// </summary>
        public string Thirteen => IsLowerCase ? "ⅹⅲ" : "ⅩⅢ";

        /// <summary>
        /// Number 14.
        /// </summary>
        public string Fourteen => IsLowerCase ? "ⅹⅳ" : "ⅩⅣ";

        /// <summary>
        /// Number 15.
        /// </summary>
        public string Fifteen => IsLowerCase ? "ⅹⅴ" : "ⅩⅤ";

        /// <summary>
        /// Number 16.
        /// </summary>
        public string Sixteen => IsLowerCase ? "ⅹⅵ" : "ⅩⅥ";

        /// <summary>
        /// Number 17.
        /// </summary>
        public string Seventeen => IsLowerCase ? "ⅹⅶ" : "ⅩⅦ";

        /// <summary>
        /// Number 18.
        /// </summary>
        public string Eighteen => IsLowerCase ? "ⅹⅷ" : "ⅩⅧ";

        /// <summary>
        /// Number 19.
        /// </summary>
        public string Nineteen => IsLowerCase ? "ⅹⅸ" : "ⅩⅨ";

        /// <summary>
        /// Number 20.
        /// </summary>
        public string Twenty => IsLowerCase ? "ⅹⅹ" : "ⅩⅩ";

        /// <summary>
        /// Number 30.
        /// </summary>
        public string Thirty => IsLowerCase ? "ⅹⅹⅹ" : "ⅩⅩⅩ";

        /// <summary>
        /// Number 40.
        /// </summary>
        public string Forty => IsLowerCase ? "ⅹⅼ" : "ⅩⅬ";

        /// <summary>
        /// Number 50.
        /// </summary>
        public string Fifty => IsLowerCase ? "ⅼ" : "Ⅼ";

        /// <summary>
        /// Number 60.
        /// </summary>
        public string Sixty => IsLowerCase ? "ⅼⅹ" : "ⅬⅩ";

        /// <summary>
        /// Number 70.
        /// </summary>
        public string Seventy => IsLowerCase ? "ⅼⅹⅹ" : "ⅬⅩⅩ";

        /// <summary>
        /// Number 80.
        /// </summary>
        public string Eighty => IsLowerCase ? "ⅼⅹⅹⅹ" : "ⅬⅩⅩⅩ";

        /// <summary>
        /// Number 90.
        /// </summary>
        public string Ninety => IsLowerCase ? "ⅹⅽ" : "ⅩⅭ";

        /// <summary>
        /// Number 100.
        /// </summary>
        public string OneHundred => IsLowerCase ? "ⅽ" : "Ⅽ";

        /// <summary>
        /// Number 500.
        /// </summary>
        public string FiveHundred => IsLowerCase ? "ⅾ" : "Ⅾ";

        /// <summary>
        /// Number 1000.
        /// </summary>
        public string OneThousand => IsLowerCase ? "ⅿ" : "Ⅿ";

        /// <summary>
        /// Roman number digits.
        /// </summary>
        public static readonly RomanNumerals Uppercase = new();

        /// <summary>
        /// Roman number digits.
        /// </summary>
        public static readonly RomanNumerals Lowercase = new(true);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(int number)
            => ToString(number);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="useLatinAlphabet">true if uses lower-character Unicode Latin letter; otherwise false.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(int number, bool useLatinAlphabet)
        {
            var sb = new StringBuilder();
            if (number < 0) sb.Append('-');
            number = Math.Abs(number);
            var s = number.ToString("g", System.Globalization.CultureInfo.InvariantCulture).Trim();
            if (number < 13)
            {
                sb.Append(number switch
                {
                    0 => "0",
                    1 => One,
                    2 => Two,
                    3 => Three,
                    4 => Four,
                    5 => Five,
                    6 => Six,
                    7 => Seven,
                    8 => Eight,
                    9 => Nine,
                    10 => Ten,
                    11 => Eleven,
                    12 => Twelve,
                    _ => string.Empty
                });
                return useLatinAlphabet ? ConvertToLatinAlphabet(sb.ToString()) : sb.ToString();
            }

            var i = 0;
            if (number >= 10000)
            {
                if (number < 100000)
                {
                    i++;
                    int decadeCount;
                    if (number < 20000)
                        decadeCount = 10;
                    else if (number < 30000)
                        decadeCount = 20;
                    else if (number < 40000)
                        decadeCount = 30;
                    else if (number < 50000)
                        decadeCount = 40;
                    else if (number < 60000)
                        decadeCount = 50;
                    else if (number < 70000)
                        decadeCount = 60;
                    else if (number < 80000)
                        decadeCount = 70;
                    else if (number < 90000)
                        decadeCount = 80;
                    else
                        decadeCount = 90;
                    sb.Append(IsLowerCase ? 'ⅿ' : 'Ⅿ', decadeCount);
                }
                else
                {
                    sb.Append(s);
                    return sb.ToString();
                }
            }

            if (s.Length > 3)
            {
                var c = s[i] switch
                {
                    '0' or ',' => string.Empty,
                    '1' => IsLowerCase ? "ⅿ" : "Ⅿ",
                    '2' => IsLowerCase ? "ⅿⅿ" : "ⅯⅯ",
                    '3' => IsLowerCase ? "ⅿⅿⅿ" : "ⅯⅯⅯ",
                    '4' => IsLowerCase ? "ⅿⅿⅿⅿ" : "ⅯⅯⅯⅯ",
                    '5' => IsLowerCase ? "ⅿⅿⅿⅿⅿ" : "ⅯⅯⅯⅯⅯ",
                    '6' => IsLowerCase ? "ⅿⅿⅿⅿⅿⅿ" : "ⅯⅯⅯⅯⅯⅯ",
                    '7' => IsLowerCase ? "ⅿⅿⅿⅿⅿⅿⅿ" : "ⅯⅯⅯⅯⅯⅯⅯ",
                    '8' => IsLowerCase ? "ⅿⅿⅿⅿⅿⅿⅿⅿ" : "ⅯⅯⅯⅯⅯⅯⅯⅯ",
                    '9' => IsLowerCase ? "ⅿⅿⅿⅿⅿⅿⅿⅿⅿ" : "ⅯⅯⅯⅯⅯⅯⅯⅯⅯ",
                    _ => s[0].ToString()
                };
                sb.Append(c);
                i++;
            }

            if (s.Length > 2)
            {
                var c = s[i] switch
                {
                    '0' => string.Empty,
                    '1' => IsLowerCase ? "ⅽ" : "Ⅽ",
                    '2' => IsLowerCase ? "ⅽⅽ" : "ⅭⅭ",
                    '3' => IsLowerCase ? "ⅽⅽⅽ" : "ⅭⅭⅭ",
                    '4' => IsLowerCase ? "ⅽⅾ" : "ⅭⅮ",
                    '5' => IsLowerCase ? "ⅾ" : "Ⅾ",
                    '6' => IsLowerCase ? "ⅾⅽ" : "ⅮⅭ",
                    '7' => IsLowerCase ? "ⅾⅽⅽ" : "ⅮⅭⅭ",
                    '8' => IsLowerCase ? "ⅾⅽⅽⅽ" : "ⅮⅭⅭⅭ",
                    '9' => IsLowerCase ? "ⅽⅿ" : "ⅭⅯ",
                    _ => s[0].ToString()
                };
                sb.Append(c);
                i++;
            }

            if (s.Length > 1)
            {
                var c = s[i] switch
                {
                    '0' => string.Empty,
                    '1' => IsLowerCase ? "ⅹ" : "Ⅹ",
                    '2' => IsLowerCase ? "ⅹⅹ" : "ⅩⅩ",
                    '3' => IsLowerCase ? "ⅹⅹⅹ" : "ⅩⅩⅩ",
                    '4' => IsLowerCase ? "ⅹⅼ" : "ⅩⅬ",
                    '5' => IsLowerCase ? "ⅼ" : "Ⅼ",
                    '6' => IsLowerCase ? "ⅼⅹ" : "ⅬⅩ",
                    '7' => IsLowerCase ? "ⅼⅹⅹ" : "ⅬⅩⅩ",
                    '8' => IsLowerCase ? "ⅼⅹⅹⅹ" : "ⅬⅩⅩⅩ",
                    '9' => IsLowerCase ? "ⅹⅽ" : "ⅩⅭ",
                    _ => s[0].ToString()
                };
                sb.Append(c);
                i++;
            }

            {
                var c = s[i] switch
                {
                    '0' => string.Empty,
                    '1' => One,
                    '2' => Two,
                    '3' => Three,
                    '4' => Four,
                    '5' => Five,
                    '6' => Six,
                    '7' => Seven,
                    '8' => Eight,
                    '9' => Nine,
                    _ => s[0].ToString()
                };
                sb.Append(c);
            }

            return useLatinAlphabet ? ConvertToLatinAlphabet(sb.ToString()) : sb.ToString();
        }

        private string ConvertToLatinAlphabet(string s)
        {
            s = s.Replace(One, "I")
                .Replace(Two, "II")
                .Replace(Three, "III")
                .Replace(Four, "IV")
                .Replace(Five, "V")
                .Replace(Six, "VI")
                .Replace(Seven, "VII")
                .Replace(Eight, "VII")
                .Replace(Nine, "IX")
                .Replace(Ten, "X")
                .Replace(Eleven, "XI")
                .Replace(Twelve, "XII")
                .Replace(Fifty, "L")
                .Replace(OneHundred, "C")
                .Replace(FiveHundred, "D")
                .Replace(OneThousand, "M")
                .Trim();
            return IsLowerCase ? s.ToLowerInvariant() : s;
        }
    }
}
