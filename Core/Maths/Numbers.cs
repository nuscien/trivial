// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimeHelper.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The number theory utility.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The integer sample.
    /// </summary>
    public interface IIntegerSample
    {
        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        bool IsZeroSupported { get; }

        /// <summary>
        /// Number 0.
        /// </summary>
        string Zero { get; }

        /// <summary>
        /// Number 1.
        /// </summary>
        string One { get; }

        /// <summary>
        /// Number 2.
        /// </summary>
        string Two { get; }

        /// <summary>
        /// Number 3.
        /// </summary>
        string Three { get; }

        /// <summary>
        /// Number 4.
        /// </summary>
        string Four { get; }

        /// <summary>
        /// Number 5.
        /// </summary>
        string Five { get; }

        /// <summary>
        /// Number 6.
        /// </summary>
        string Six { get; }

        /// <summary>
        /// Number 7.
        /// </summary>
        string Seven { get; }

        /// <summary>
        /// Number 8.
        /// </summary>
        string Eight { get; }

        /// <summary>
        /// Number 9.
        /// </summary>
        string Nine { get; }

        /// <summary>
        /// Number 10.
        /// </summary>
        string Ten { get; }

        /// <summary>
        /// Number 11.
        /// </summary>
        string Eleven { get; }

        /// <summary>
        /// Number 12.
        /// </summary>
        string Twelve { get; }

        /// <summary>
        /// Number 13.
        /// </summary>
        string Thirteen { get; }

        /// <summary>
        /// Number 14.
        /// </summary>
        string Fourteen { get; }

        /// <summary>
        /// Number 15.
        /// </summary>
        string Fifteen { get; }

        /// <summary>
        /// Number 16.
        /// </summary>
        string Sixteen { get; }

        /// <summary>
        /// Number 17.
        /// </summary>
        string Seventeen { get; }

        /// <summary>
        /// Number 18.
        /// </summary>
        string Eighteen { get; }

        /// <summary>
        /// Number 19.
        /// </summary>
        string Nineteen { get; }

        /// <summary>
        /// Number 20.
        /// </summary>
        string Twenty { get; }

        /// <summary>
        /// Number 30.
        /// </summary>
        string Thirty { get; }

        /// <summary>
        /// Number 40.
        /// </summary>
        string Forty { get; }

        /// <summary>
        /// Number 50.
        /// </summary>
        string Fifty { get; }

        /// <summary>
        /// Number 60.
        /// </summary>
        string Sixty { get; }

        /// <summary>
        /// Number 70.
        /// </summary>
        string Seventy { get; }

        /// <summary>
        /// Number 80.
        /// </summary>
        string Eighty { get; }

        /// <summary>
        /// Number 90.
        /// </summary>
        string Ninety { get; }

        /// <summary>
        /// Number 100.
        /// </summary>
        string OneHundred { get; }

        /// <summary>
        /// Number 500.
        /// </summary>
        string FiveHundred { get; }

        /// <summary>
        /// Number 1000.
        /// </summary>
        string OneThousand { get; }
    }

    /// <summary>
    /// The local string resolver for number.
    /// </summary>
    public interface IIntegerLocalization
    {
        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        string ToString(long number, bool digitOnly = false);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        string ToString(ulong number, bool digitOnly = false);
    }

    /// <summary>
    /// The Roman number.
    /// </summary>
    public class RomanNumber : IIntegerSample
    {
        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => false;

        /// <summary>
        /// Initializes a new instance of the RomanNumber class.
        /// </summary>
        /// <param name="lowerCase">true if use lower case; otherwise, false.</param>
        protected internal RomanNumber(bool lowerCase = false)
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
    }

    /// <summary>
    /// The base number digits.
    /// </summary>
    public abstract class LocalNumber : IIntegerSample, IIntegerLocalization
    {
        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => true;

        /// <summary>
        /// Sign of negative.
        /// </summary>
        public abstract string PositiveSign { get; }

        /// <summary>
        /// Sign of negative.
        /// </summary>
        public abstract string NegativeSign { get; }

        /// <summary>
        /// Number 0.
        /// </summary>
        public virtual string Zero => ToString(0);

        /// <summary>
        /// Number 1.
        /// </summary>
        public virtual string One => ToString(1);

        /// <summary>
        /// Number 2.
        /// </summary>
        public virtual string Two => ToString(2);

        /// <summary>
        /// Number 3.
        /// </summary>
        public virtual string Three => ToString(3);

        /// <summary>
        /// Number 4.
        /// </summary>
        public virtual string Four => ToString(4);

        /// <summary>
        /// Number 5.
        /// </summary>
        public virtual string Five => ToString(5);

        /// <summary>
        /// Number 6.
        /// </summary>
        public virtual string Six => ToString(6);

        /// <summary>
        /// Number 7.
        /// </summary>
        public virtual string Seven => ToString(7);

        /// <summary>
        /// Number 8.
        /// </summary>
        public virtual string Eight => ToString(8);

        /// <summary>
        /// Number 9.
        /// </summary>
        public virtual string Nine => ToString(9);

        /// <summary>
        /// Number 10.
        /// </summary>
        public virtual string Ten => ToString(10);

        /// <summary>
        /// Number 11.
        /// </summary>
        public virtual string Eleven => ToString(11);

        /// <summary>
        /// Number 12.
        /// </summary>
        public virtual string Twelve => ToString(12);

        /// <summary>
        /// Number 13.
        /// </summary>
        public virtual string Thirteen => ToString(13);

        /// <summary>
        /// Number 14.
        /// </summary>
        public virtual string Fourteen => ToString(14);

        /// <summary>
        /// Number 15.
        /// </summary>
        public virtual string Fifteen => ToString(15);

        /// <summary>
        /// Number 16.
        /// </summary>
        public virtual string Sixteen => ToString(16);

        /// <summary>
        /// Number 17.
        /// </summary>
        public virtual string Seventeen => ToString(17);

        /// <summary>
        /// Number 18.
        /// </summary>
        public virtual string Eighteen => ToString(18);

        /// <summary>
        /// Number 19.
        /// </summary>
        public virtual string Nineteen => ToString(19);

        /// <summary>
        /// Number 20.
        /// </summary>
        public virtual string Twenty => ToString(20);

        /// <summary>
        /// Number 30.
        /// </summary>
        public virtual string Thirty => ToString(30);

        /// <summary>
        /// Number 40.
        /// </summary>
        public virtual string Forty => ToString(40);

        /// <summary>
        /// Number 50.
        /// </summary>
        public virtual string Fifty => ToString(50);

        /// <summary>
        /// Number 60.
        /// </summary>
        public virtual string Sixty => ToString(60);

        /// <summary>
        /// Number 70.
        /// </summary>
        public virtual string Seventy => ToString(70);

        /// <summary>
        /// Number 80.
        /// </summary>
        public virtual string Eighty => ToString(80);

        /// <summary>
        /// Number 90.
        /// </summary>
        public virtual string Ninety => ToString(90);

        /// <summary>
        /// Number 100.
        /// </summary>
        public virtual string OneHundred => ToString(100);

        /// <summary>
        /// Number 500.
        /// </summary>
        public virtual string FiveHundred => ToString(500);

        /// <summary>
        /// Number 1,000.
        /// </summary>
        public virtual string OneThousand => ToString(1000);

        /// <summary>
        /// Number 5,000.
        /// </summary>
        public virtual string FiveThousand => ToString(5000);

        /// <summary>
        /// Number 10,000.
        /// </summary>
        public virtual string TenThousand => ToString(10000);

        /// <summary>
        /// Number 100,000.
        /// </summary>
        public virtual string OneHundredThousand => ToString(100000);

        /// <summary>
        /// Number 1,000,000.
        /// </summary>
        public virtual string OneMillion => ToString(1000000);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public abstract string ToString(long number, bool digitOnly = false);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public abstract string ToString(ulong number, bool digitOnly = false);
    }

    /// <summary>
    /// The English number digits.
    /// </summary>
    public class EnglishNumber : IIntegerSample, IIntegerLocalization
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
                var numStr = number.ToString();
                if (numStr.Length < 6) return GetAllDigitsString(number.ToString(), 6);
                if (numStr.Length == 6 || numStr.Length == 9) return GetAllDigitsString(number.ToString(), 3);
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
                var numStr = number.ToString();
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
    }

    /// <summary>
    /// The Chinese number digits.
    /// </summary>
    public class ChineseNumber : IIntegerSample, IIntegerLocalization
    {
        private static readonly string digits1 = "零壹贰叄肆伍陆柒捌玖拾佰仟萬亿";
        private static readonly string digits2 = "零一二三四五六七八九十百千万亿";

        /// <summary>
        /// Initializes a new instance of the ChineseNumber class.
        /// </summary>
        /// <param name="upperCase">true if use upper case; otherwise, false.</param>
        protected internal ChineseNumber(bool upperCase = false)
        {
            IsUpperCase = upperCase;
        }

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
        /// Gets a value indicating whether it is in lower case.
        /// </summary>
        public bool IsUpperCase { get; }

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
        /// Number 100.
        /// </summary>
        public string OneHundred => One + HundredClass;

        /// <summary>
        /// Number 200.
        /// </summary>
        public string TwoHundred => "两" + HundredClass;

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
        public string TwoThousand => "两" + ThousandClass;

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
        public string TenThousandClass => IsUpperCase ? "萬" : "万";

        /// <summary>
        /// Gets the main number digits.
        /// </summary>
        /// <returns>A string with main numbers.</returns>
        public override string ToString()
        {
            return IsUpperCase ? digits1 : digits2;
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
            var num = number.ToString();
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

            var result = str
                .Replace(Two + HundredClass, TwoHundred)
                .Replace(Two + ThousandClass, TwoThousand)
                .Replace(Zero + Two + TenThousandClass, Zero + "两" + TenThousandClass)
                .ToString();
            return result.IndexOf(One + TenClass) == 0 ? result.Substring(1) : result;
        }
    }

    /// <summary>
    /// The class for numbers.
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// Number symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// Sign of plus minus.
            /// </summary>
            public const string PlusMinusSign = "±";

            /// <summary>
            /// Sign of positive.
            /// </summary>
            public const string PositiveSign = "+";

            /// <summary>
            /// Sign of negative.
            /// </summary>
            public const string NegativeSign = "-";

            /// <summary>
            /// The positive infinite.
            /// </summary>
            public const string PositiveInfiniteSymbol = "+∞";

            /// <summary>
            /// The negative infinite.
            /// </summary>
            public const string NegativeInfiniteSymbol = "-∞";

            /// <summary>
            /// Number of 0.
            /// </summary>
            public const string NumberZero = "0";

            /// <summary>
            /// Number of 1/8.
            /// </summary>
            public const string NumberOneEighth = "⅛";

            /// <summary>
            /// Number of 1/4.
            /// </summary>
            public const string NumberQuarter = "¼";

            /// <summary>
            /// Number of 1/3.
            /// </summary>
            public const string NumberOneThrid = "⅓";

            /// <summary>
            /// Number of 3/8.
            /// </summary>
            public const string NumberThreeEighths = "⅜";

            /// <summary>
            /// Number of 1/2.
            /// </summary>
            public const string NumberHalf = "½";

            /// <summary>
            /// Number of 5/8.
            /// </summary>
            public const string NumberFiveEighths = "⅝";

            /// <summary>
            /// Number of 2/3.
            /// </summary>
            public const string NumberTwoThrids = "⅔";

            /// <summary>
            /// Number of 3/4.
            /// </summary>
            public const string NumberThreeQuarters = "¾";

            /// <summary>
            /// Number of 7/8.
            /// </summary>
            public const string NumberSevenEighths = "⅞";

            /// <summary>
            /// Number of 1.
            /// </summary>
            public const string NumberOne = "1";

            /// <summary>
            /// Number of 2.
            /// </summary>
            public const string NumberTwo = "2";

            /// <summary>
            /// Number of 3.
            /// </summary>
            public const string NumberThree = "3";

            /// <summary>
            /// Number of 4.
            /// </summary>
            public const string NumberFour = "4";

            /// <summary>
            /// Number of 5.
            /// </summary>
            public const string NumberFive = "5";

            /// <summary>
            /// Number of 6.
            /// </summary>
            public const string NumberSix = "6";

            /// <summary>
            /// Number of 7.
            /// </summary>
            public const string NumberSeven = "7";

            /// <summary>
            /// Number of 8.
            /// </summary>
            public const string NumberEight = "8";

            /// <summary>
            /// Number of 9.
            /// </summary>
            public const string NumberNine = "9";

            /// <summary>
            /// Number of 10 in hexadecimal.
            /// </summary>
            public const string NumberA = "A";

            /// <summary>
            /// Number of 11 in hexadecimal.
            /// </summary>
            public const string NumberB = "B";

            /// <summary>
            /// Number of 12 in hexadecimal.
            /// </summary>
            public const string NumberC = "C";

            /// <summary>
            /// Number of 13 in hexadecimal.
            /// </summary>
            public const string NumberD = "D";

            /// <summary>
            /// Number of 14 in hexadecimal.
            /// </summary>
            public const string NumberE = "E";

            /// <summary>
            /// Number of 15 in hexadecimal.
            /// </summary>
            public const string NumberF = "F";

            /// <summary>
            /// The symbol of PI.
            /// </summary>
            public const string PiSymbol = "π";

            /// <summary>
            /// The symbol of natural logarithmic base.
            /// </summary>
            public const string NaturalLogarithmicBaseSymbol = "e";

            /// <summary>
            /// The symbol of complex base.
            /// </summary>
            public const string ComplexBaseSymbol = "i";

            /// <summary>
            /// The sign of empty set of nothing.
            /// </summary>
            public const string EmptySetSymbol = "ø";
        }

        /// <summary>
        /// Roman number digits.
        /// </summary>
        public static readonly RomanNumber Roman = new RomanNumber();

        /// <summary>
        /// Roman number digits.
        /// </summary>
        public static readonly RomanNumber LowerCaseRoman = new RomanNumber(true);

        /// <summary>
        /// English number digits.
        /// </summary>
        public static readonly EnglishNumber English = new EnglishNumber();

        /// <summary>
        /// Chinese number digits.
        /// </summary>
        public static readonly ChineseNumber Chinese = new ChineseNumber();

        /// <summary>
        /// Chinese number digits.
        /// </summary>
        public static readonly ChineseNumber UpperCaseChinese = new ChineseNumber(true);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="numbers">The number localization instance.</param>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public static string ToString(this IIntegerLocalization numbers, int number, bool digitOnly = false)
        {
            if (numbers == null) return number.ToString();
            return numbers.ToString((long)number, digitOnly);
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="numbers">The number localization instance.</param>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public static string ToString(this IIntegerLocalization numbers, uint number, bool digitOnly = false)
        {
            if (numbers == null) return number.ToString();
            return numbers.ToString((ulong)number, digitOnly);
        }
    }
}
