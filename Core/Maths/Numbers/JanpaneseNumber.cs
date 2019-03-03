using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The Japanese number digits.
    /// </summary>
    public class JapaneseNumber : IIntegerSample, INumberLocalization
    {
        private static readonly string digits= "零一二三四五六七八九十百千万億兆京垓秭穰沟涧正载";

        /// <summary>
        /// Initializes a new instance of the JapaneseNumber class.
        /// </summary>
        protected internal JapaneseNumber()
        {
        }

        /// <summary>
        /// Gets a value inidcatint whether it is hiragana.
        /// </summary>
        public bool IsHiragana { get; }

        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => true;

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string PositiveSign => "正";

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string NegativeSign => "负";

        /// <summary>
        /// Gets the length of digit group.
        /// </summary>
        public int GroupLength => 4;

        /// <summary>
        /// Number 0.
        /// </summary>
        public string Zero => IsHiragana ? "れい" : "零";

        /// <summary>
        /// Number 1.
        /// </summary>
        public string One => IsHiragana ? "いち" : "一";

        /// <summary>
        /// Number 2.
        /// </summary>
        public string Two => IsHiragana ? "に" : "二";

        /// <summary>
        /// Number 3.
        /// </summary>
        public string Three => IsHiragana ? "さん" : "三";

        /// <summary>
        /// Number 4.
        /// </summary>
        public string Four => IsHiragana ? "し" : "四";

        /// <summary>
        /// Number 5.
        /// </summary>
        public string Five => IsHiragana ? "ご" : "五";

        /// <summary>
        /// Number 6.
        /// </summary>
        public string Six => IsHiragana ? "ろく" : "六";

        /// <summary>
        /// Number 7.
        /// </summary>
        public string Seven => IsHiragana ? "しち" : "七";

        /// <summary>
        /// Number 8.
        /// </summary>
        public string Eight => IsHiragana ? "はち" : "八";

        /// <summary>
        /// Number 9.
        /// </summary>
        public string Nine => IsHiragana ? "きゅう" : "九";

        /// <summary>
        /// Number 10.
        /// </summary>
        public string Ten => TenClass;

        /// <summary>
        /// Number 11.
        /// </summary>
        public string Eleven => IsHiragana ? "じゅういち" : "十一";

        /// <summary>
        /// Number 12.
        /// </summary>
        public string Twelve => IsHiragana ? "じゅうに" : "十二";

        /// <summary>
        /// Number 13.
        /// </summary>
        public string Thirteen => IsHiragana ? "じゅうさん" : "十三";

        /// <summary>
        /// Number 14.
        /// </summary>
        public string Fourteen => IsHiragana ? "じゅうし" : "十四";

        /// <summary>
        /// Number 15.
        /// </summary>
        public string Fifteen => IsHiragana ? "じゅうご" : "十五";

        /// <summary>
        /// Number 16.
        /// </summary>
        public string Sixteen => IsHiragana ? "じゅうろく" : "十六";

        /// <summary>
        /// Number 17.
        /// </summary>
        public string Seventeen => IsHiragana ? "じゅうしち" : "十七";

        /// <summary>
        /// Number 18.
        /// </summary>
        public string Eighteen => IsHiragana ? "じゅうはち" : "十八";

        /// <summary>
        /// Number 19.
        /// </summary>
        public string Nineteen => IsHiragana ? "じゅうきゅう" : "十九";

        /// <summary>
        /// Number 20.
        /// </summary>
        public string Twenty => Two + TenClass;

        /// <summary>
        /// Number 30.
        /// </summary>
        public string Thirty => Three + TenClass;

        /// <summary>
        /// Number 40.
        /// </summary>
        public string Forty => Four + TenClass;

        /// <summary>
        /// Number 50.
        /// </summary>
        public string Fifty => Five + TenClass;

        /// <summary>
        /// Number 60.
        /// </summary>
        public string Sixty => Six + TenClass;

        /// <summary>
        /// Number 70.
        /// </summary>
        public string Seventy => Seven + TenClass;

        /// <summary>
        /// Number 80.
        /// </summary>
        public string Eighty => Nine + TenClass;

        /// <summary>
        /// Number 90.
        /// </summary>
        public string Ninety => Nine + TenClass;

        /// <summary>
        /// Number 100.
        /// </summary>
        public string OneHundred => One + HundredClass;

        /// <summary>
        /// Number 200.
        /// </summary>
        public string TwoHundred => Two + HundredClass;

        /// <summary>
        /// Number 500.
        /// </summary>
        public string FiveHundred => Five + HundredClass;

        /// <summary>
        /// Number 1,000.
        /// </summary>
        public string OneThousand => One + ThousandClass;

        /// <summary>
        /// Number 2,000.
        /// </summary>
        public string TwoThousand => Two + ThousandClass;

        /// <summary>
        /// Number 5,000.
        /// </summary>
        public string FiveThousand => Five + ThousandClass;

        /// <summary>
        /// Number 10,000.
        /// </summary>
        public string TenThousand => TenThousandClass;

        /// <summary>
        /// Number 100,000.
        /// </summary>
        public string OneHundredThousand => Ten + TenThousandClass;

        /// <summary>
        /// Number 1,000,000.
        /// </summary>
        public string OneMillion => OneHundred + TenThousandClass;

        /// <summary>
        /// The order of magnitude 10e1.
        /// </summary>
        public string TenClass => IsHiragana ? "じゅう" : "十";

        /// <summary>
        /// The order of magnitude 10e2.
        /// </summary>
        public string HundredClass => IsHiragana ? "ひゃく" : "百";

        /// <summary>
        /// The order of magnitude 10e3.
        /// </summary>
        public string ThousandClass => IsHiragana ? "せん" : "千";

        /// <summary>
        /// The order of magnitude 10e4.
        /// </summary>
        public string TenThousandClass => IsHiragana ? "おく" : "万";

        /// <summary>
        /// The order of magnitude 10e8.
        /// </summary>
        public string HundredMillionClass => IsHiragana ? "おく" : "億";

        /// <summary>
        /// The order of magnitude 10e12.
        /// </summary>
        public string TrillionClass => IsHiragana ? "ちょう" : "兆";

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
            if (number < 0) return NegativeSign + str;
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
            var levels = number.ToString(CultureInfo.InvariantCulture).Length / GroupLength;
            if (levels < 1) return number.ToString();
            var len = levels * GroupLength;
            var format = new StringBuilder("0.");
            format.Append('0', Math.Min(accuracy, len));
            var num = new StringBuilder((number * 1.0 / Math.Pow(10, len)).ToString(format.ToString(), CultureInfo.InvariantCulture));
            if ((number / Math.Pow(10, len)).ToString(CultureInfo.InvariantCulture).Length == 4) num.Insert(1, ',');
            var digits = ToString();
            if (levels > 11)
            {
                num.AppendFormat("×10^{0}", levels);
            }
            else
            {
                num.Append(digits[12 + levels]);
            }

            return num.ToString();
        }

        /// <summary>
        /// Gets the main number digits.
        /// </summary>
        /// <returns>A string with main numbers.</returns>
        public override string ToString()
        {
            return digits;
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public string ToString(long number, bool digitOnly = false)
        {
            var prefix = number < 0 ? NegativeSign : string.Empty;
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
            var num = number.ToString(CultureInfo.InvariantCulture);
            var digits = ToString();
            var str = new StringBuilder();
            if (digitOnly)
            {
                foreach (var item in num)
                {
                    str.Append(digits[int.Parse(item.ToString())]);
                }

                return str.ToString();
            }

            if (number <= 10) return digits[(int)number].ToString();
            if (number < 20) return TenClass + digits[(int)number - 10].ToString();
            var append = 4 - num.Length % 4;
            if (append == 4) append = 0;
            for (var i = 0; i < append; i++)
            {
                num = " " + num;
            }

            var classPos = num.Length / 4;
            for (var i = 0; i < num.Length; i++)
            {
                var c = num[i].ToString();
                if (c == " ") continue;
                if (c != "0")
                {
                    str.Append(digits[int.Parse(c)]);
                    var j = 3 - i % 4;
                    if (j > 0) str.Append(digits[9 + j]);
                }

                if (i % 4 != 3 || i == num.Length - 1) continue;
                classPos--;
                if (classPos == 0) continue;
                str.Append(digits[12 + classPos]);
            }

            return str.ToString().Replace("一十", "十");
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
                if (number <= ulong.MaxValue)
                {
                    var numInt = (ulong)number;
                    if (numInt == number) return ToString(numInt);
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
            var digits = ToString();
            if (fractionalPart.Length > 0)
            {
                str.Append("点");
                foreach (var item in fractionalPart)
                {
                    str.Append(digits[int.Parse(item.ToString())]);
                }
            }

            if (exponentialPart > 0)
            {
                if (exponentialPart % 4 == 0 && exponentialPart < 45)
                {
                    str.Append(digits[12 + exponentialPart / 4]);
                }
                else
                {
                    str.Append("かける");
                    str.Append(TenClass);
                    if (exponentialPart > 1)
                    {
                        str.Append("の");
                        str.Append(ToString(exponentialPart));
                        str.Append("乗");
                    }
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Simplified Chinese number.
        /// </summary>
        public static readonly JapaneseNumber Default = new JapaneseNumber();
    }
}
