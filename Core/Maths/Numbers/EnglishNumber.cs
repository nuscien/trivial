using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The English number digits.
    /// </summary>
    public class EnglishNumber : IIntegerSample, INumberLocalization
    {
        /// <summary>
        /// Initializes a new instance of the EnglishNumber class.
        /// </summary>
        protected internal EnglishNumber()
        {
        }

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
        public string OneHundred => "a hundred";

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
        public string OneThousand => "a thousand";

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
        public string OneHundredThousand => "a hundred thousand";

        /// <summary>
        /// Number 1,000,000.
        /// </summary>
        public string OneMillion => "a million";

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
            if (number < 0) return NumberSymbols.NegativeSign + str;
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
            var prefix = number < 0 ? NegativeSign : string.Empty;
            var levels = Math.Abs(number.ToString(CultureInfo.InvariantCulture).Length) / GroupLength;
            if (levels < 1) return number.ToString();
            var len = levels * GroupLength;
            var format = new StringBuilder("0.");
            format.Append('0', Math.Min(accuracy, len));
            var num = (number * 1.0 / Math.Pow(10, len)).ToString(format.ToString(), CultureInfo.InvariantCulture);
            switch (levels)
            {
                case 1:
                    return num + "K";
                case 2:
                    return num + "M";
                case 3:
                    return num + "G";
                case 4:
                    return num + "T";
                case 5:
                    return num + "P";
                case 6:
                    return num + "E";
                case 7:
                    return num + "Z";
                case 8:
                    return num + "Y";
                default:
                    return string.Format("{0}×10^{1}", num, levels * GroupLength);
            }
        }

        /// <summary>
        /// Gets the main number digits.
        /// </summary>
        /// <returns>A string with main numbers.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
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
                switch (numStr.Length % 4)
                {
                    case 2:
                        return string.Format("{0}, {1}, {2}",
                            GetAllDigitsString(numStr.Substring(0, 3), 3),
                            GetAllDigitsString(numStr.Substring(3, numStr.Length - 6), 4),
                            GetAllDigitsString(numStr.Substring(numStr.Length - 3), 3));
                    case 3:
                        return GetAllDigitsString(numStr.Substring(0, 3), 3) + ", " + GetAllDigitsString(numStr.Substring(4), 4);
                    default:
                        return GetAllDigitsString(numStr, 4);
                }
            }

            if (number < 1000)
            {
                if (number == 100) return OneHundred;
                var remainder = number % 100;
                if (remainder > 10)
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
                else if (a % 10 > 0)
                {
                    str.Append(Get2DigitsString((int)a));
                    str.Append(b < 10 ? " zero " : " ");
                }
                else
                {
                    str.Append(GetDigitString((int)a / 10) + " k");
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
                    if (number == 1.25) return "a half and a quarter";
                    if (number == 1.21) return "a half and a fifth";
                }
            }
            else if (number > long.MinValue)
            {
                var numAbs = Math.Abs(number);
                var numInt = (ulong)numAbs;
                if (numInt == numAbs) return NegativeSign + ToString(numInt);
            }

            var str = new StringBuilder();
            var (integerPart, fractionalPart, exponentialPart) = NumberSymbols.SplitNumber(number);
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
            switch (number)
            {
                case 9:
                    return Nine;
                case 8:
                    return Eight;
                case 7:
                    return Seven;
                case 6:
                    return Six;
                case 5:
                    return Five;
                case 4:
                    return Four;
                case 3:
                    return Three;
                case 2:
                    return Two;
                case 1:
                    return One;
                default:
                    return Zero;
            }
        }

        private string Get2DigitsString(int number)
        {
            if (number < 0) number = Math.Abs(number);
            if (number <= 10) return GetDigitString(number);
            if (number < 20)
            {
                switch (number)
                {
                    case 19:
                        return Nineteen;
                    case 18:
                        return Eighteen;
                    case 17:
                        return Seventeen;
                    case 16:
                        return Sixteen;
                    case 15:
                        return Fifteen;
                    case 14:
                        return Fourteen;
                    case 13:
                        return Thirteen;
                    case 12:
                        return Twelve;
                    case 11:
                        return Eleven;
                    default:
                        return GetDigitString(number);
                }
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
        public static EnglishNumber Default = new EnglishNumber();
    }
}
