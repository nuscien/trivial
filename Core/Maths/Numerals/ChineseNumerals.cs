﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Trivial.Maths;

/// <summary>
/// The Chinese numerals.
/// </summary>
[Guid("2E96C2AC-A53D-46E9-825B-68E3F77A2A6C")]
public class ChineseNumerals : IIntegerSample, INumberLocalization
{
    private static readonly string digits1 = "零壹贰叄肆伍陆柒捌玖拾佰仟万亿";
    private static readonly string digits2 = "零一二三四五六七八九十百千万亿";
    private static readonly string digits3 = "零壹贰叄肆伍陆柒捌玖拾佰仟萬億";
    private static readonly string digits4 = "零一二三四五六七八九十百千萬億";
    private readonly string digits;

    /// <summary>
    /// The ten Heavenly Stems.
    /// </summary>
    public const string HeavenlyStems = "甲乙丙丁戊己庚辛壬癸";

    /// <summary>
    /// The ten Heavenly Stems.
    /// </summary>
    public const string HeavenlyStemsName = "天干";

    /// <summary>
    /// The twelve Earthly Branches.
    /// </summary>
    public const string EarthlyBranches = "子丑寅卯辰巳午未申酉戌亥";

    /// <summary>
    /// The twelve Earthly Branches.
    /// </summary>
    public const string EarthlyBranchesName = "地支";

    /// <summary>
    /// Initializes a new instance of the ChineseNumerals class.
    /// </summary>
    /// <param name="isTraditional">true if it set as Traditional Chinese; otherwise, false.</param>
    /// <param name="upperCase">true if use upper case; otherwise, false.</param>
    protected internal ChineseNumerals(bool isTraditional = false, bool upperCase = false)
    {
        IsUpperCase = upperCase;
        IsTraditional = isTraditional;
        digits = IsTraditional ? (IsUpperCase ? digits3 : digits4) : (IsUpperCase ? digits1 : digits2);
        PositiveSign = "正";
        NegativeSign = IsTraditional ? "負" : "负";
    }

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
    /// Gets a value indicating whether it is in lower case.
    /// </summary>
    public bool IsUpperCase { get; }

    /// <summary>
    /// Gets a value indicating whether it is Tranditional Chinese.
    /// </summary>
    public bool IsTraditional { get; }

    /// <summary>
    /// Gets a value indicating whether it is Simplified Chinese.
    /// </summary>
    public bool IsSimplified => !IsTraditional;

    /// <summary>
    /// Number 0.
    /// </summary>
    public string Zero => "零";

    /// <summary>
    /// Number 1.
    /// </summary>
    public string One => IsUpperCase ? "壹" : "一";

    /// <summary>
    /// Number 2.
    /// </summary>
    public string Two => IsUpperCase ? "贰" : "二";

    /// <summary>
    /// Number 2.
    /// </summary>
    private string Two2 => IsTraditional ? "兩" : "两";

    /// <summary>
    /// Number 3.
    /// </summary>
    public string Three => IsUpperCase ? "叄" : "三";

    /// <summary>
    /// Number 4.
    /// </summary>
    public string Four => IsUpperCase ? "肆" : "四";

    /// <summary>
    /// Number 5.
    /// </summary>
    public string Five => IsUpperCase ? "伍" : "五";

    /// <summary>
    /// Number 6.
    /// </summary>
    public string Six => IsUpperCase ? "陆" : "六";

    /// <summary>
    /// Number 7.
    /// </summary>
    public string Seven => IsUpperCase ? "柒" : "七";

    /// <summary>
    /// Number 8.
    /// </summary>
    public string Eight => IsUpperCase ? "捌" : "八";

    /// <summary>
    /// Number 9.
    /// </summary>
    public string Nine => IsUpperCase ? "玖" : "九";

    /// <summary>
    /// Number 10.
    /// </summary>
    public string Ten => TenClass;

    /// <summary>
    /// Number 11.
    /// </summary>
    public string Eleven => IsUpperCase ? "拾壹" : "十一";

    /// <summary>
    /// Number 12.
    /// </summary>
    public string Twelve => IsUpperCase ? "拾贰" : "十二";

    /// <summary>
    /// Number 13.
    /// </summary>
    public string Thirteen => IsUpperCase ? "拾叄" : "十三";

    /// <summary>
    /// Number 14.
    /// </summary>
    public string Fourteen => IsUpperCase ? "拾肆" : "十四";

    /// <summary>
    /// Number 15.
    /// </summary>
    public string Fifteen => IsUpperCase ? "拾伍" : "十五";

    /// <summary>
    /// Number 16.
    /// </summary>
    public string Sixteen => IsUpperCase ? "拾陆" : "十六";

    /// <summary>
    /// Number 17.
    /// </summary>
    public string Seventeen => IsUpperCase ? "拾柒" : "十七";

    /// <summary>
    /// Number 18.
    /// </summary>
    public string Eighteen => IsUpperCase ? "拾捌" : "十八";

    /// <summary>
    /// Number 19.
    /// </summary>
    public string Nineteen => IsUpperCase ? "拾玖" : "十九";

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
    /// Number 20.
    /// </summary>
    public string ShortTwenty => IsUpperCase ? Twenty : "廿";

    /// <summary>
    /// Number 30.
    /// </summary>
    public string ShortThirty => IsUpperCase ? Thirty : "卅";

    /// <summary>
    /// Number 40.
    /// </summary>
    public string ShortForty => IsUpperCase ? Forty : "卌";

    /// <summary>
    /// Number 50.
    /// </summary>
    public string ShortFifty => IsUpperCase ? Fifty : "圩";

    /// <summary>
    /// Number 60.
    /// </summary>
    public string ShortSixty => IsUpperCase ? Sixty : "圆";

    /// <summary>
    /// Number 70.
    /// </summary>
    public string ShortSeventy => IsUpperCase ? Seventy : "进";

    /// <summary>
    /// Number 80.
    /// </summary>
    public string ShortEighty => IsUpperCase ? Eighty : "枯";

    /// <summary>
    /// Number 90.
    /// </summary>
    public string ShortNinety => IsUpperCase ? Ninety : "枠";

    /// <summary>
    /// Number 100.
    /// </summary>
    public string OneHundred => One + HundredClass;

    /// <summary>
    /// Number 200.
    /// </summary>
    public string TwoHundred => (IsUpperCase ? Two : Two2) + HundredClass;

    /// <summary>
    /// Number 200.
    /// </summary>
    public string ShortTwoHundred => IsUpperCase ? TwoHundred : "皕";

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
    public string TwoThousand => (IsUpperCase ? Two : Two2) + ThousandClass;

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
    public string TenClass => IsUpperCase ? "拾" : "十";

    /// <summary>
    /// The order of magnitude 10e2.
    /// </summary>
    public string HundredClass => IsUpperCase ? "佰" : "百";

    /// <summary>
    /// The order of magnitude 10e3.
    /// </summary>
    public string ThousandClass => IsUpperCase ? "仟" : "千";

    /// <summary>
    /// The order of magnitude 10e4.
    /// </summary>
    public string TenThousandClass => IsTraditional ? "萬" : "万";
    
    /// <summary>
    /// The order of magnitude 10e8.
    /// </summary>
    public string HundredMillionClass => IsTraditional ? "億" : "亿";

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
        var levels = number.ToString(CultureInfo.InvariantCulture).Length / GroupLength;
        if (levels < 1) return number.ToString();
        var len = levels * GroupLength;
        var format = new StringBuilder("0.");
        format.Append('0', Math.Min(accuracy, len));
        var num = new StringBuilder((number * 1.0 / Math.Pow(10, len)).ToString(format.ToString(), CultureInfo.InvariantCulture));
        if (levels % 2 == 1) num.Append(digits[13]);
        num.Append(digits[14], levels / 2);
        return num.ToString();
    }

    /// <summary>
    /// Gets the main number digits.
    /// </summary>
    /// <returns>A string with main numbers.</returns>
    public override string ToString()
    {
        if (IsUpperCase) return (IsTraditional ? "中文大寫數字 " : "汉字大写数字 ") + digits;
        return (IsTraditional ? "中文數字 " : "汉字数字 ") + digits;
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
        var isZero = false;
        for (var i = 0; i < num.Length; i++)
        {
            var c = num[i].ToString();
            if (c == " ") continue;
            if (c == "0")
            {
                isZero = true;
            }
            else
            {
                if (isZero)
                {
                    str.Append(digits[0]);
                    isZero = false;
                }

                str.Append(digits[int.Parse(c)]);
                var j = 3 - i % 4;
                if (j > 0) str.Append(digits[9 + j]);
            }

            if (i % 4 != 3 || i == num.Length - 1) continue;
            classPos--;
            isZero = false;
            var lastChar = str[str.Length - 1];
            if (lastChar == digits[13] || lastChar == digits[14]) continue;
            if (classPos % 2 == 1)
            {
                str.Append(digits[13]);
            }
            else
            {
                str.Append(digits[14], classPos / 2);
            }
        }

        if (IsUpperCase) return str.ToString();
        str = str
            .Replace(Two + HundredClass, TwoHundred)
            .Replace(Two + ThousandClass, TwoThousand)
            .Replace(Zero + Two + TenThousandClass, Zero + Two2 + TenThousandClass);
        var result = str.ToString();
        return result.IndexOf(One + TenClass) == 0 ? result.Substring(1) : result;
    }

    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>A string for the number.</returns>
    public string ToString(double number)
    {
        if (double.IsNaN(number)) return IsTraditional ? "不是數字" : "不是数字";
        if (number > 0)
        {
            if (number <= ulong.MaxValue)
            {
                var numInt = (ulong)number;
                if (numInt == number) return ToString(numInt);
            }
            else if (double.IsPositiveInfinity(number))
            {
                return IsTraditional ? "正無窮" : "正无穷";
            }
        }
        else if (number > long.MinValue)
        {
            var numAbs = Math.Abs(number);
            var numInt = (ulong)numAbs;
            if (numInt == numAbs) return NegativeSign + ToString(numInt);
        }
        else if (number == 0)
        {
            return Zero;
        }
        else if (double.IsNegativeInfinity(number))
        {
            return IsTraditional ? "負無窮" : "负无穷";
        }

        var str = new StringBuilder();
        var (integerPart, fractionalPart, exponentialPart) = Numbers.SplitNumber(number);
        str.Append(ToString(integerPart, false));
        if (fractionalPart.Length > 0)
        {
            str.Append(IsTraditional ? "點" : "点");
            foreach (var item in fractionalPart)
            {
                str.Append(digits[int.Parse(item.ToString())]);
            }
        }

        if (exponentialPart > 0)
        {
            if (exponentialPart % 4 == 0 && exponentialPart < 41)
            {
                if (exponentialPart % 8 == 4)
                {
                    str.Append(digits[13]);
                }
                else
                {
                    str.Append(digits[14], exponentialPart / 8);
                }
            }
            else
            {
                str.Append("乘以");
                str.Append(TenClass);
                if (exponentialPart > 1)
                {
                    str.Append("的");
                    str.Append(ToString(exponentialPart));
                    str.Append("次方");
                }
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// Gets the string of a specific number.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>A string for the number.</returns>
    public string ToString(Fraction number)
    {
        if (number.IsNaN) return ToString(double.NaN);
        if (number.IsPositiveInfinity) return ToString(double.PositiveInfinity);
        if (number.IsNegativeInfinity) return ToString(double.NegativeInfinity);
        var sb = new StringBuilder();
        if (number.IsNegative) sb.Append(IsTraditional ? "負" : "负");
        sb.Append(ToString(number.LongDenominator));
        sb.Append("分之");
        sb.Append(ToString(number.LongNumerator));
        return sb.ToString();
    }

    /// <summary>
    /// Gets the Heavenly Stems, Earthly Branches and round.
    /// </summary>
    /// <param name="value">The input number.</param>
    /// <param name="start">The start number.</param>
    /// <returns>The Heavenly Stems, Earthly Branches and round.</returns>
    public static (char, char, int) ToHeavenlyStemsEarthlyBranchesAndRound(int value, int start = 0)
    {
        value -= start;
        return (HeavenlyStems[(value % 10 + 10) % 10], EarthlyBranches[(value % 12 + 12) % 12], value % 60);
    }

    /// <summary>
    /// Simplified Chinese number.
    /// </summary>
    /// <example>
    /// <code>
    /// // Get the string for a specific number. It should be following.
    /// // 一万两千三百四十五点六七
    /// ChineseNumber.Simplified.ToString(12345.67);
    /// 
    /// // Get the string of the digit one by one by setting the 2nd arg as true. It should be following.
    /// // 一二三四五
    /// ChineseNumber.Simplified.ToString(12345, true);
    /// 
    /// // Get the string of an approximation for a specific number. It should be following.
    /// // 123.5万
    /// var num3 = ChineseNumber.Simplified.ToApproximationString(1234567);
    /// </code>
    /// </example>
    public static readonly ChineseNumerals Simplified = new();

    /// <summary>
    /// Simplified Chinese uppercase number.
    /// </summary>
    /// <example>
    /// <code>
    /// // Get the uppercase number string. It should be following.
    /// // 壹萬贰仟叄佰肆拾伍
    /// ChineseNumber.SimplifiedUppercase.ToString(12345);
    /// </code>
    /// </example>
    public static readonly ChineseNumerals SimplifiedUppercase = new(false, true);

    /// <summary>
    /// Traditional Chinese number.
    /// </summary>
    public static readonly ChineseNumerals Traditional = new(true);

    /// <summary>
    /// Traditional Chinese uppercase number.
    /// </summary>
    public static readonly ChineseNumerals TraditionalUppercase = new(true, true);
}
