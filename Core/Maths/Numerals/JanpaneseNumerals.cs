using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The Japanese numerals.
    /// </summary>
    public class JapaneseNumerals : IIntegerSample, INumberLocalization
    {
        private static readonly string digits = "零一二三四五六七八九十百千万億兆京垓秭穣溝澗正載極";

        /// <summary>
        /// Initializes a new instance of the JapaneseNumerals class.
        /// </summary>
        /// <param name="kana">true if use Kana; otherwise, false.</param>
        protected internal JapaneseNumerals(bool kana = false)
        {
            IsKana = kana;
            PositiveSign = "正";
            NegativeSign = "負";
        }

        /// <summary>
        /// Gets a value inidcatint whether it is based on Kana or Chinese.
        /// </summary>
        public bool IsKana { get; }

        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => true;

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string PositiveSign { get; }

        /// <summary>
        /// Gets the sign of negative.
        /// </summary>
        public string NegativeSign { get; }

        /// <summary>
        /// Gets the length of digit group.
        /// </summary>
        public int GroupLength => 4;

        /// <summary>
        /// Number 0.
        /// </summary>
        public string Zero => IsKana ? "れい" : "零";

        /// <summary>
        /// Number 1.
        /// </summary>
        public string One => IsKana ? "いち" : "一";

        /// <summary>
        /// Number 2.
        /// </summary>
        public string Two => IsKana ? "に" : "二";

        /// <summary>
        /// Number 3.
        /// </summary>
        public string Three => IsKana ? "さん" : "三";

        /// <summary>
        /// Number 4.
        /// </summary>
        public string Four => IsKana ? "し" : "四";

        /// <summary>
        /// Number 5.
        /// </summary>
        public string Five => IsKana ? "ご" : "五";

        /// <summary>
        /// Number 6.
        /// </summary>
        public string Six => IsKana ? "ろく" : "六";

        /// <summary>
        /// Number 7.
        /// </summary>
        public string Seven => IsKana ? "しち" : "七";

        /// <summary>
        /// Number 8.
        /// </summary>
        public string Eight => IsKana ? "はち" : "八";

        /// <summary>
        /// Number 9.
        /// </summary>
        public string Nine => IsKana ? "きゅう" : "九";

        /// <summary>
        /// Number 10.
        /// </summary>
        public string Ten => TenClass;

        /// <summary>
        /// Number 11.
        /// </summary>
        public string Eleven => IsKana ? "じゅういち" : "十一";

        /// <summary>
        /// Number 12.
        /// </summary>
        public string Twelve => IsKana ? "じゅうに" : "十二";

        /// <summary>
        /// Number 13.
        /// </summary>
        public string Thirteen => IsKana ? "じゅうさん" : "十三";

        /// <summary>
        /// Number 14.
        /// </summary>
        public string Fourteen => IsKana ? "じゅうし" : "十四";

        /// <summary>
        /// Number 15.
        /// </summary>
        public string Fifteen => IsKana ? "じゅうご" : "十五";

        /// <summary>
        /// Number 16.
        /// </summary>
        public string Sixteen => IsKana ? "じゅうろく" : "十六";

        /// <summary>
        /// Number 17.
        /// </summary>
        public string Seventeen => IsKana ? "じゅうしち" : "十七";

        /// <summary>
        /// Number 18.
        /// </summary>
        public string Eighteen => IsKana ? "じゅうはち" : "十八";

        /// <summary>
        /// Number 19.
        /// </summary>
        public string Nineteen => IsKana ? "じゅうきゅう" : "十九";

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
        public string TenClass => IsKana ? "じゅう" : "十";

        /// <summary>
        /// The order of magnitude 10e2.
        /// </summary>
        public string HundredClass => IsKana ? "ひゃく" : "百";

        /// <summary>
        /// The order of magnitude 10e3.
        /// </summary>
        public string ThousandClass => IsKana ? "せん" : "千";

        /// <summary>
        /// The order of magnitude 10e4.
        /// </summary>
        public string TenThousandClass => IsKana ? "まん" : "万";

        /// <summary>
        /// The order of magnitude 10e8.
        /// </summary>
        public string HundredMillionClass => IsKana ? "おく" : "億";

        /// <summary>
        /// The order of magnitude 10e12.
        /// </summary>
        public string TrillionClass => IsKana ? "ちょう" : "兆";

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
            var levels = number.ToString(CultureInfo.InvariantCulture).Length / GroupLength;
            if (levels < 1) return ConvertString(number.ToString());
            var len = levels * GroupLength;
            var format = new StringBuilder("0.");
            format.Append('0', Math.Min(accuracy, len));
            var num = new StringBuilder((number * 1.0 / Math.Pow(10, len)).ToString(format.ToString(), CultureInfo.InvariantCulture));
            if ((number / Math.Pow(10, len)).ToString(CultureInfo.InvariantCulture).Length == 4) num.Insert(1, ',');
            if (levels > 12)
            {
                num.AppendFormat("×10^{0}", levels);
            }
            else
            {
                num.Append(digits[12 + levels]);
            }

            return ConvertString(num);
        }

        /// <summary>
        /// Gets the main number digits.
        /// </summary>
        /// <returns>A string with main numbers.</returns>
        public override string ToString()
        {
            return "日本の数字 " + digits;
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
            var str = new StringBuilder();
            if (digitOnly)
            {
                foreach (var item in num)
                {
                    str.Append(digits[int.Parse(item.ToString())]);
                }

                return ConvertString(str);
            }

            if (number <= 10) return ConvertString(digits[(int)number].ToString());
            if (number < 20) return ConvertString(TenClass + digits[(int)number - 10].ToString());
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

            return ConvertString(str.ToString().Replace("一十", "十"));
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
            if (fractionalPart.Length > 0)
            {
                str.Append("点");
                foreach (var item in fractionalPart)
                {
                    if (IsKana) str.Append(ConvertString(digits[int.Parse(item.ToString())].ToString()));
                    else str.Append(digits[int.Parse(item.ToString())]);
                }
            }

            if (exponentialPart > 0)
            {
                if (exponentialPart % 4 == 0 && exponentialPart < 45)
                {
                    if (IsKana) str.Append(ConvertString(digits[12 + exponentialPart / 4].ToString()));
                    else str.Append(digits[12 + exponentialPart / 4]);
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

        private string ConvertString(StringBuilder value)
        {
            return ConvertString(value.ToString());
        }

        private string ConvertString(string value)
        {
            if (!IsKana) return value;
            return value
                .Replace("零", "れい")
                .Replace("一", "いち")
                .Replace("二", "に")
                .Replace("三", "さん")
                .Replace("四", "し")
                .Replace("五", "ご")
                .Replace("六", "ろく")
                .Replace("七", "しち")
                .Replace("八", "はち")
                .Replace("九", "きゅう")
                .Replace("十", "じゅう")
                .Replace("百", "ひゃく")
                .Replace("千", "せん")
                .Replace("万", "まん")
                .Replace("億", "おく")
                .Replace("兆", "ちょう")
                .Replace("京", "けい")
                .Replace("垓", "がい")
                .Replace("秭", "し")
                .Replace("穣", "じょう")
                .Replace("溝", "こう")
                .Replace("澗", "かん")
                .Replace("正", "せい")
                .Replace("載", "さい")
                .Replace("極", "ごく");
        }

        /// <summary>
        /// Japanese number.
        /// </summary>
        public static readonly JapaneseNumerals Default = new JapaneseNumerals();

        /// <summary>
        /// Japanese Kana number.
        /// </summary>
        public static readonly JapaneseNumerals Kana = new JapaneseNumerals(true);
    }
}
