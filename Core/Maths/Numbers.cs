// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimeHelper.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The number theory utility.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Trivial.Maths
{
    /// <summary>
    /// The class for prime number.
    /// </summary>
    public static partial class NumberUtility
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
        /// Upper Roman number of 1.
        /// </summary>
        public const string RomanNumberOne = "Ⅰ";

        /// <summary>
        /// Upper Roman number of 2.
        /// </summary>
        public const string RomanNumberTwo = "Ⅱ";

        /// <summary>
        /// Upper Roman number of 3.
        /// </summary>
        public const string RomanNumberThree = "Ⅲ";

        /// <summary>
        /// Upper Roman number of 4.
        /// </summary>
        public const string RomanNumberFour = "Ⅳ";

        /// <summary>
        /// Upper Roman number of 5.
        /// </summary>
        public const string RomanNumberFive = "Ⅴ";

        /// <summary>
        /// Upper Roman number of 6.
        /// </summary>
        public const string RomanNumberSix = "Ⅵ";

        /// <summary>
        /// Upper Roman number of 7.
        /// </summary>
        public const string RomanNumberSeven = "Ⅶ";

        /// <summary>
        /// Upper Roman number of 8.
        /// </summary>
        public const string RomanNumberEight = "Ⅷ";

        /// <summary>
        /// Upper Roman number of 9.
        /// </summary>
        public const string RomanNumberNine = "Ⅸ";

        /// <summary>
        /// Upper Roman number of 10.
        /// </summary>
        public const string RomanNumberTen = "Ⅹ";

        /// <summary>
        /// Upper Roman number of 11.
        /// </summary>
        public const string RomanNumberEleven = "Ⅺ";

        /// <summary>
        /// Upper Roman number of 12.
        /// </summary>
        public const string RomanNumberTwelve = "Ⅻ";

        /// <summary>
        /// Lower Roman number of 1.
        /// </summary>
        public const string RomanLowerNumberOne = "ⅰ";

        /// <summary>
        /// Lower Roman number of 2.
        /// </summary>
        public const string RomanLowerNumberTwo = "ⅱ";

        /// <summary>
        /// Lower Roman number of 3.
        /// </summary>
        public const string RomanLowerNumberThree = "ⅲ";

        /// <summary>
        /// Lower Roman number of 4.
        /// </summary>
        public const string RomanLowerNumberFour = "ⅳ";

        /// <summary>
        /// Lower Roman number of 5.
        /// </summary>
        public const string RomanLowerNumberFive = "ⅴ";

        /// <summary>
        /// Lower Roman number of 6.
        /// </summary>
        public const string RomanLowerNumberSix = "ⅵ";

        /// <summary>
        /// Lower Roman number of 7.
        /// </summary>
        public const string RomanLowerNumberSeven = "ⅶ";

        /// <summary>
        /// Lower Roman number of 8.
        /// </summary>
        public const string RomanLowerNumberEight = "ⅷ";

        /// <summary>
        /// Lower Roman number of 9.
        /// </summary>
        public const string RomanLowerNumberNine = "ⅸ";

        /// <summary>
        /// Lower Roman number of 10.
        /// </summary>
        public const string RomanLowerNumberTen = "ⅹ";

        /// <summary>
        /// Upper Chinese number of 0.
        /// </summary>
        public const string ChineseUpperNumberZero = "零";

        /// <summary>
        /// Upper Chinese number of 1.
        /// </summary>
        public const string ChineseUpperNumberOne = "壹";

        /// <summary>
        /// Upper Chinese number of 2.
        /// </summary>
        public const string ChineseUpperNumberTwo = "贰";

        /// <summary>
        /// Upper Chinese number of 3.
        /// </summary>
        public const string ChineseUpperNumberThree = "叄";

        /// <summary>
        /// Upper Chinese number of 4.
        /// </summary>
        public const string ChineseUpperNumberFour = "肆";

        /// <summary>
        /// Upper Chinese number of 5.
        /// </summary>
        public const string ChineseUpperNumberFive = "伍";

        /// <summary>
        /// Upper Chinese number of 6.
        /// </summary>
        public const string ChineseUpperNumberSix = "陆";

        /// <summary>
        /// Upper Chinese number of 7.
        /// </summary>
        public const string ChineseUpperNumberSeven = "柒";

        /// <summary>
        /// Upper Chinese number of 8.
        /// </summary>
        public const string ChineseUpperNumberEight = "捌";

        /// <summary>
        /// Upper Chinese number of 9.
        /// </summary>
        public const string ChineseUpperNumberNine = "玖";

        /// <summary>
        /// Upper Chinese number of 10.
        /// </summary>
        public const string ChineseUpperNumberTen = "拾";

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
}
