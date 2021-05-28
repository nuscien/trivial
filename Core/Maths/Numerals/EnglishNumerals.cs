using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Trivial.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The English numerals.
    /// </summary>
    public class EnglishNumerals : IIntegerSample, INumberLocalization
    {
        /// <summary>
        /// Initializes a new instance of the EnglishNumerals class.
        /// </summary>
        /// <param name="standard">true if use standard mode style; otherwise, false.</param>
        protected internal EnglishNumerals(bool standard)
        {
            IsStandard = standard;
        }

        /// <summary>
        /// Gets a value indicating whether it is standard mode style.
        /// </summary>
        public bool IsStandard { get; }

        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => true;

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string PositiveSign => "positive";

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string NegativeSign => "negative";

        /// <summary>
        /// Gets the length of digit group.
        /// </summary>
        public int GroupLength => 3;

        /// <summary>
        /// Number 0.
        /// </summary>
        public string Zero => "zero";

        /// <summary>
        /// Number 1.
        /// </summary>
        public string One => "one";

        /// <summary>
        /// Number 2.
        /// </summary>
        public string Two => "two";

        /// <summary>
        /// Number 3.
        /// </summary>
        public string Three => "three";

        /// <summary>
        /// Number 4.
        /// </summary>
        public string Four => "four";

        /// <summary>
        /// Number 5.
        /// </summary>
        public string Five => "five";

        /// <summary>
        /// Number 6.
        /// </summary>
        public string Six => "six";

        /// <summary>
        /// Number 7.
        /// </summary>
        public string Seven => "seven";

        /// <summary>
        /// Number 8.
        /// </summary>
        public string Eight => "eight";

        /// <summary>
        /// Number 9.
        /// </summary>
        public string Nine => "nine";

        /// <summary>
        /// Number 10.
        /// </summary>
        public string Ten => "ten";

        /// <summary>
        /// Number 11.
        /// </summary>
        public string Eleven => "eleven";

        /// <summary>
        /// Number 12.
        /// </summary>
        public string Twelve => "twelve";

        /// <summary>
        /// Number 13.
        /// </summary>
        public string Thirteen => "thirteen";

        /// <summary>
        /// Number 14.
        /// </summary>
        public string Fourteen => "fourteen";

        /// <summary>
        /// Number 15.
        /// </summary>
        public string Fifteen => "fifteen";

        /// <summary>
        /// Number 16.
        /// </summary>
        public string Sixteen => "sixteen";

        /// <summary>
        /// Number 17.
        /// </summary>
        public string Seventeen => "seventeen";

        /// <summary>
        /// Number 18.
        /// </summary>
        public string Eighteen => "eighteen";

        /// <summary>
        /// Number 19.
        /// </summary>
        public string Nineteen => "nineteen";

        /// <summary>
        /// Number 20.
        /// </summary>
        public string Twenty => "twenty";

        /// <summary>
        /// Number 30.
        /// </summary>
        public string Thirty => "thirty";

        /// <summary>
        /// Number 40.
        /// </summary>
        public string Forty => "forty";

        /// <summary>
        /// Number 50.
        /// </summary>
        public string Fifty => "fifty";

        /// <summary>
        /// Number 60.
        /// </summary>
        public string Sixty => "sixty";

        /// <summary>
        /// Number 70.
        /// </summary>
        public string Seventy => "seventy";

        /// <summary>
        /// Number 80.
        /// </summary>
        public string Eighty => "eighty";

        /// <summary>
        /// Number 90.
        /// </summary>
        public string Ninety => "ninety";

        /// <summary>
        /// Number 100.
        /// </summary>
        public string OneHundred => IsStandard ? "one hundred" : "a hundred";

        /// <summary>
        /// Number 200.
        /// </summary>
        public string TwoHundred => "two hundred";

        /// <summary>
        /// Number 500.
        /// </summary>
        public string FiveHundred => "five hundred";

        /// <summary>
        /// Number 1,000.
        /// </summary>
        public string OneThousand => IsStandard ? "one thousand" : "a thousand";

        /// <summary>
        /// Number 2,000.
        /// </summary>
        public string TwoThousand => "two thousand";

        /// <summary>
        /// Number 5,000.
        /// </summary>
        public string FiveThousand => "five thousand";

        /// <summary>
        /// Number 10,000.
        /// </summary>
        public string TenThousand => "ten thousand";

        /// <summary>
        /// Number 100,000.
        /// </summary>
        public string OneHundredThousand => IsStandard ? "one hundred thousand" : "a hundred thousand";

        /// <summary>
        /// Number 1,000,000.
        /// </summary>
        public string OneMillion => IsStandard ? "one million" : "a million";

        /// <summary>
        /// The order of magnitude 10e2.
        /// </summary>
        public string HundredClass => "hundred";

        /// <summary>
        /// The order of magnitude 10e3.
        /// </summary>
        public string ThousandClass => "thousand";

        /// <summary>
        /// The order of magnitude 10e6.
        /// </summary>
        public string MillionClass => "million";

        /// <summary>
        /// The order of magnitude 10e9.
        /// </summary>
        public string BillionClass => "billion";

        /// <summary>
        /// The order of magnitude 10e12.
        /// </summary>
        public string TrillionClass => "trillion";

        /// <summary>
        /// Converts a number to the string of approximation.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <param name="accuracy">A nature number for accuracy of the number.</param>
        /// <returns>A string format number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">accuracy is less than 0 or is greater than 32.</exception>
        public string ToApproximationString(long number, int accuracy = 1)
        {
            var str = ToApproximationString((ulong)Math.Abs(number), accuracy);
            if (number < 0) return Numbers.NegativeSign + str;
            return str;
        }

        /// <summary>
        /// Converts a number to the string of approximation.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <param name="accuracy">A nature number for accuracy of the number.</param>
        /// <returns>A string format number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">accuracy is less than 0 or is greater than 32.</exception>
        public string ToApproximationString(ulong number, int accuracy = 1)
        {
            if (accuracy < 0 || accuracy > 32) throw new ArgumentOutOfRangeException(nameof(accuracy));
            var levels = Math.Abs(number.ToString(CultureInfo.InvariantCulture).Length) / GroupLength;
            if (levels < 1) return number.ToString();
            var len = levels * GroupLength;
            var format = new StringBuilder("0.");
            format.Append('0', Math.Min(accuracy, len));
            var num = (number * 1.0 / Math.Pow(10, len)).ToString(format.ToString(), CultureInfo.InvariantCulture);
            return levels switch
            {
                1 => num + "K",
                2 => num + "M",
                3 => num + "G",
                4 => num + "T",
                5 => num + "P",
                6 => num + "E",
                7 => num + "Z",
                8 => num + "Y",
                _ => string.Format("{0}×10{1}", num, Numbers.ToExponentString(levels * GroupLength)),
            };
        }

        /// <summary>
        /// Gets the a part of numbers.
        /// </summary>
        /// <returns>A string with key numbers.</returns>
        public override string ToString()
        {
            return string.Format("English numerals {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                Zero,
                One,
                Two,
                Three,
                Four,
                Five,
                Six,
                Seven,
                Eight,
                Nine,
                Ten);
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(long number, bool digitOnly = false)
        {
            var prefix = number < 0 ? (NegativeSign + " ") : string.Empty;
            return prefix + ToString((ulong)Math.Abs(number), digitOnly);
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(ulong number, bool digitOnly = false)
        {
            var str = new StringBuilder();
            if (digitOnly)
            {
                var numStr = number.ToString(CultureInfo.InvariantCulture);
                if (numStr.Length < 6) return GetAllDigitsString(numStr, 6);
                if (numStr.Length == 6 || numStr.Length == 9) return GetAllDigitsString(numStr, 3);
                return (numStr.Length % 4) switch
                {
                    2 => string.Format("{0}, {1}, {2}",
                                               GetAllDigitsString(numStr.Substring(0, 3), 3),
                                               GetAllDigitsString(numStr.SubRangeString(3, 3, true), 4),
                                               GetAllDigitsString(numStr.Substring(numStr.Length - 3), 3)),
                    3 => GetAllDigitsString(numStr.Substring(0, 3), 3) + ", " + GetAllDigitsString(numStr.Substring(4), 4),
                    _ => GetAllDigitsString(numStr, 4),
                };
            }

            if (number < 1000)
            {
                if (number == 100) return OneHundred;
                var remainder = number % 100;
                if (remainder > 10 && !IsStandard)
                {
                    var a = (int)(number - remainder) / 100;
                    if (a > 0) str.Append(GetDigitString(a) + " ");
                    str.Append(Get2DigitsString((int)remainder));
                    return str.ToString();
                }

                return Get3DigitsString((int)number);
            }

            if (number < 10000)
            {
                if (number % 1000 == 0)
                {
                    if (number == 1000) return OneThousand;
                    return GetDigitString((int)number / 1000) + " " + ThousandClass;
                }

                var b = number % 100;
                var a = (number - b) / 100;
                if (a == 10)
                {
                    str.Append(OneThousand + " and ");
                }
                else if (IsStandard)
                {
                    var c = (int)a % 10;
                    str.Append(GetDigitString(((int)a - c) / 10));
                    str.Append(" thousand ");
                    if (c > 0)
                    {
                        str.Append(GetDigitString(c));
                        str.Append(" hundred ");
                    }

                    str.Append("and ");
                }
                else if (a % 10 > 0)
                {
                    str.Append(Get2DigitsString((int)a));
                    str.Append(b < 10 ? " zero " : " ");
                }
                else
                {
                    str.Append(GetDigitString((int)a / 10));
                    str.Append(" k");
                    str.Append(b < 10 ? " and " : " ");
                }

                str.Append(Get2DigitsString((int)b));
                return str.ToString();
            }

            if (number >= 1_000_000_000_000_000)
            {
                var numStr = number.ToString(CultureInfo.InvariantCulture);
                var last = numStr.Length - 1;
                for (var i = 0; i < last; i++)
                {
                    str.Append(GetDigitString(numStr, i));
                    if ((numStr.Length - i) % 3 == 1) str.Append(", ");
                    else str.Append(" ");
                }

                str.Append(GetDigitString(numStr, last));
                return str.ToString();
            }

            var arr = new List<string>();
            var highNum = number;
            while (highNum > 0)
            {
                var remainder = highNum % 1000;
                highNum = (highNum - remainder) / 1000;
                if (remainder == 0)
                {
                    arr.Add(string.Empty);
                    continue;
                }

                var partStr = Get3DigitsString((int)remainder);
                if (remainder < 100 && highNum > 0) partStr = "and " + partStr;
                arr.Add(partStr);
            }

            for (var i = arr.Count - 1; i > 0; i--)
            {
                var item = arr[i];
                if (string.IsNullOrWhiteSpace(item)) continue;
                str.Append(item);
                str.Append(" ");
                switch (i)
                {
                    case 4:
                        str.Append(TrillionClass);
                        break;
                    case 3:
                        str.Append(BillionClass);
                        break;
                    case 2:
                        str.Append(MillionClass);
                        break;
                    case 1:
                        str.Append(ThousandClass);
                        break;
                }

                str.Append(" ");
            }

            str.Append(arr[0]);
            return str.ToString();
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(double number)
        {
            if (double.IsNaN(number)) return "not a number";
            if (number > 0)
            {
                if (number < 1)
                {
                    if (number == 0.5) return "a half";
                    if (number == 1 / 3) return "a third";
                    if (number == 0.25) return "a quarter";
                    if (number == 0.2) return "a fifth";
                    if (number == 0.1) return "ten percents";
                    if (number == 0.01) return "a percent";
                }

                if (number <= ulong.MaxValue)
                {
                    var numInt = (ulong)number;
                    if (numInt == number) return ToString(numInt);
                    if (number == 1.5) return "a half and one";
                    if (number == 1.25) return "a quarter and one";
                    if (number == 1.2) return "a fifth and one";
                }
                else if (double.IsPositiveInfinity(number))
                {
                    return "positive infinity";
                }
            }
            else if (number > long.MinValue)
            {
                var numAbs = Math.Abs(number);
                var numInt = (ulong)numAbs;
                if (numInt == numAbs) return NegativeSign + ToString(numInt);
            }
            else if (double.IsNegativeInfinity(number))
            {
                return "negative infinity";
            }

            var str = new StringBuilder();
            var (integerPart, fractionalPart, exponentialPart) = Numbers.SplitNumber(number);
            str.Append(ToString(integerPart, false));
            if (fractionalPart.Length > 0)
            {
                str.Append(" point ");
                str.Append(GetAllDigitsString(fractionalPart, fractionalPart.Length));
            }

            if (exponentialPart > 0)
            {
                switch (exponentialPart)
                {
                    case 1:
                        str.Append(" times ten");
                        break;
                    case 2:
                        str.Append(" hundred");
                        break;
                    case 3:
                        str.Append(" thousand");
                        break;
                    case 6:
                        str.Append(" million");
                        break;
                    case 9:
                        str.Append(" billion");
                        break;
                    case 12:
                        str.Append(" trillion");
                        break;
                    default:
                        str.Append(" times ten of ");
                        str.Append(ToString(exponentialPart));
                        str.Append(" power");
                        break;
                }
            }

            return str.ToString();
        }

        private string GetDigitString(int number)
        {
            if (number < 0) number = Math.Abs(number);
            if (number == 10) return Ten;
            if (number > 10) number %= 10;
            return number switch
            {
                9 => Nine,
                8 => Eight,
                7 => Seven,
                6 => Six,
                5 => Five,
                4 => Four,
                3 => Three,
                2 => Two,
                1 => One,
                _ => Zero,
            };
        }

        private string Get2DigitsString(int number)
        {
            if (number < 0) number = Math.Abs(number);
            if (number <= 10) return GetDigitString(number);
            if (number < 20)
            {
                return number switch
                {
                    19 => Nineteen,
                    18 => Eighteen,
                    17 => Seventeen,
                    16 => Sixteen,
                    15 => Fifteen,
                    14 => Fourteen,
                    13 => Thirteen,
                    12 => Twelve,
                    11 => Eleven,
                    _ => GetDigitString(number),
                };
            }

            string str;
            if (number > 100) number %= 100;
            var remainder = number % 10;
            switch ((number - remainder) / 10)
            {
                case 9:
                    str = Ninety;
                    break;
                case 8:
                    str = Eighty;
                    break;
                case 7:
                    str = Seventy;
                    break;
                case 6:
                    str = Sixty;
                    break;
                case 5:
                    str = Fifty;
                    break;
                case 4:
                    str = Forty;
                    break;
                case 3:
                    str = Thirty;
                    break;
                case 2:
                    str = Twenty;
                    break;
                default:
                    return string.Empty;
            }

            if (remainder > 0) str += "-" + GetDigitString(remainder);
            return str;
        }

        private string Get3DigitsString(int number)
        {
            if (number < 100) return Get2DigitsString(number);
            var remainder = number % 100;
            var str = new StringBuilder();
            str.Append(GetDigitString((number - remainder) / 100));
            str.Append(" ");
            str.Append(HundredClass);
            if (remainder == 0) return str.ToString();
            str.Append(" and ");
            str.Append(Get2DigitsString(remainder));
            return str.ToString();
        }

        private string GetAllDigitsString(string number, int count)
        {
            var str = new StringBuilder();
            var last = number.Length - 1;
            var split = count - 1;
            for (var i = 0; i < last; i++)
            {
                str.Append(GetDigitString(number, i));
                if (i % count == split) str.Append(", ");
                else str.Append(" ");
            }

            str.Append(GetDigitString(number, last));
            return str.ToString();
        }

        private string GetDigitString(string number, int index)
        {
            return GetDigitString(int.Parse(number[index].ToString()));
        }

        /// <summary>
        /// English number.
        /// </summary>
        /// <example>
        /// <code>
        /// // Get the string for a specific number. It should be following.
        /// // twelve thousand three hundred and forty-five point six seven
        /// var num1 = EnglishNumber.Default.ToString(12345.67);
        /// 
        /// // Get the string of the digit one by one by setting the 2nd arg as true. It should be following.
        /// // one two three four five
        /// var num2 = EnglishNumber.Default.ToString(12345, true);
        /// 
        /// // Get the string of an approximation for a specific number. It should be following.
        /// // 1.2M
        /// var num3 = EnglishNumber.Default.ToApproximationString(1234567);
        /// </code>
        /// </example>
        public static EnglishNumerals Default = new EnglishNumerals(false);

        /// <summary>
        /// English number in standard mode style.
        /// </summary>
        /// <example>
        /// <code>
        /// // Get the string for a specific number. It should be following.
        /// // twelve thousand three hundred and forty-five point six seven
        /// var num1 = EnglishNumber.Default.ToString(12345.67);
        /// 
        /// // Get the string of the digit one by one by setting the 2nd arg as true. It should be following.
        /// // one two three four five
        /// var num2 = EnglishNumber.Default.ToString(12345, true);
        /// 
        /// // Get the string of an approximation for a specific number. It should be following.
        /// // 1.2M
        /// var num3 = EnglishNumber.Default.ToApproximationString(1234567);
        /// </code>
        /// </example>
        public static EnglishNumerals Standard = new EnglishNumerals(true);
    }
}
