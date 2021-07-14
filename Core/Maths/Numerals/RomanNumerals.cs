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
        public string Eleven => IsLowerCase ? "ⅹⅰ" : "Ⅺ";

        /// <summary>
        /// Number 12.
        /// </summary>
        public string Twelve => IsLowerCase ? "ⅹⅱ" : "Ⅻ";

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
        public string Forty => IsLowerCase ? "xl" : "XL";

        /// <summary>
        /// Number 50.
        /// </summary>
        public string Fifty => IsLowerCase ? "l" : "L";

        /// <summary>
        /// Number 60.
        /// </summary>
        public string Sixty => IsLowerCase ? "lx" : "LX";

        /// <summary>
        /// Number 70.
        /// </summary>
        public string Seventy => IsLowerCase ? "lxx" : "LXX";

        /// <summary>
        /// Number 80.
        /// </summary>
        public string Eighty => IsLowerCase ? "lxxx" : "LXXX";

        /// <summary>
        /// Number 90.
        /// </summary>
        public string Ninety => IsLowerCase ? "xc" : "XC";

        /// <summary>
        /// Number 100.
        /// </summary>
        public string OneHundred => IsLowerCase ? "c" : "C";

        /// <summary>
        /// Number 500.
        /// </summary>
        public string FiveHundred => IsLowerCase ? "d" : "D";

        /// <summary>
        /// Number 1000.
        /// </summary>
        public string OneThousand => IsLowerCase ? "m" : "M";

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
        {
            var sb = new StringBuilder(number < 0 ? "-" : string.Empty);
            number = Math.Abs(number);
            var s = number.ToString("g", System.Globalization.CultureInfo.InvariantCulture).Trim();
            if (number < 11)
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
                    _ => string.Empty
                });
                return sb.ToString();
            }
            else if (!IsLowerCase && number < 13)
            {
                sb.Append(number == 11 ? Eleven : Twelve);
                return sb.ToString();
            }

            if (number >= 4000)
            {
                sb.Append(s);
                return sb.ToString();
            }

            var i = 0;
            if (s.Length == 4)
            {
                var c = s[0] switch
                {
                    '0' or ',' => string.Empty,
                    '1' => "M",
                    '2' => "MM",
                    '3' => "MMM",
                    _ => s[0].ToString()
                };
                sb.Append(c);
                i = 1;
            }

            if (s.Length > 2)
            {
                var c = s[i] switch
                {
                    '0' => string.Empty,
                    '1' => "C",
                    '2' => "CC",
                    '3' => "CCC",
                    '4' => "CD",
                    '5' => "D",
                    '6' => "DC",
                    '7' => "DCC",
                    '8' => "DCCC",
                    '9' => "CM",
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
                    '1' => "X",
                    '2' => "XX",
                    '3' => "XXX",
                    '4' => "XL",
                    '5' => "L",
                    '6' => "LX",
                    '7' => "LXX",
                    '8' => "LXXX",
                    '9' => "XC",
                    _ => s[0].ToString()
                };
                sb.Append(c);
                i++;
            }

            {
                var c = s[i] switch
                {
                    '0' => string.Empty,
                    '1' => "I",
                    '2' => "II",
                    '3' => "III",
                    '4' => "IV",
                    '5' => "V",
                    '6' => "VI",
                    '7' => "VII",
                    '8' => "VIII",
                    '9' => "IX",
                    _ => s[0].ToString()
                };
                sb.Append(c);
            }

            return IsLowerCase ? sb.ToString().ToLowerInvariant() : sb.ToString();
        }
    }
}
