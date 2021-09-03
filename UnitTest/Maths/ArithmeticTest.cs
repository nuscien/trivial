using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths
{
    /// <summary>
    /// Arithmetic unit test.
    /// </summary>
    [TestClass]
    public class ArithmeticTest
    {
        /// <summary>
        /// Tests arithmetic.
        /// </summary>
        [TestMethod]
        public async Task TestArithmetic()
        {
            // Prime
            Assert.IsTrue(Arithmetic.IsPrime(524287));
            Assert.IsFalse(Arithmetic.IsPrime(968455));
            Assert.IsFalse(Arithmetic.IsPrime(21474836477));
            Assert.IsFalse(await Arithmetic.IsPrimeAsync(21474836477));
            Assert.IsTrue(await Arithmetic.IsPrimeAsync(2147483647));
            Assert.AreEqual(17, await Arithmetic.PreviousPrimeAsync(19));
            Assert.IsTrue(await Arithmetic.PreviousPrimeAsync(968455) < 968455);
            Assert.IsTrue(await Arithmetic.NextPrimeAsync(968455) > 968455);

            // GCD & LCM
            Assert.AreEqual(64, Arithmetic.Gcd(192, 128));
            Assert.AreEqual(1, Arithmetic.Gcd(67, 31));
            Assert.AreEqual(384, Arithmetic.Lcm(192, 128));
            Assert.AreEqual(2077, Arithmetic.Lcm(67, 31));

            // Factorial.
            Assert.AreEqual(2432902008176640000, Arithmetic.Factorial(20));

            // Positional notation.
            Assert.AreEqual("120", Numbers.ToPositionalNotationString(168, 12));
            Assert.AreEqual("8a", Numbers.ToPositionalNotationString(170.0, 20));
            Assert.AreEqual("8a.2", Numbers.ToPositionalNotationString(170.1, 20));
            Assert.AreEqual("3.47d01bpf", Numbers.ToPositionalNotationString(3.14159265, 30));
            Assert.AreEqual("0.6204620462", Numbers.ToPositionalNotationString(0.9, 7));
            Assert.AreEqual(168, Numbers.ParseToInt32("120", 12));
            Assert.AreEqual(-168L, Numbers.ParseToInt64("-120", 12));
            Assert.AreEqual(170, Numbers.ParseToInt32("8a", 20));
            Assert.AreEqual(17, Numbers.ParseToInt32("17", 10));
            Assert.AreEqual(17000, Numbers.ParseToInt32("17K", 10));
            Assert.AreEqual(170000, Numbers.ParseToInt32("17万", 10));
            Assert.AreEqual(17000000, Numbers.ParseToInt32("17百w", 10));
            Assert.AreEqual(-1000000000, Numbers.ParseToInt32("-1G", 10));
            Assert.AreEqual(1000000, Numbers.ParseToInt32("百万", 10));
            Assert.AreEqual(17, Numbers.ParseToInt32("seventeen", 10));
            Assert.IsFalse(Numbers.TryParseToInt32("8a", 9, out _));
            Assert.IsFalse(Numbers.TryParseToInt32("8a", 8, out _));
            Assert.AreEqual(17_000L, Numbers.ParseToInt64("17K", 10));
            Assert.AreEqual(170_000L, Numbers.ParseToInt64("17万", 10));
            Assert.AreEqual(17_000_000L, Numbers.ParseToInt64("17百w", 10));
            Assert.AreEqual(17_000_000_000L, Numbers.ParseToInt64("17十亿", 10));
            Assert.AreEqual(-200_000_000_000_000, Numbers.ParseToInt64("-200T", 10));
            Assert.AreEqual(1_000_000L, Numbers.ParseToInt64("百万", 10));
            Assert.AreEqual(1_000_000L, Numbers.ParseToInt64("mega", 10));

            // Compare
            Assert.AreEqual(1, Arithmetic.Min(1, 2, 3));
            Assert.AreEqual(1, Arithmetic.Min(1, 2, 3, 4, 5));
            Assert.AreEqual(9, Arithmetic.Max(7, 8, 9));
            Assert.AreEqual(9, Arithmetic.Max(5, 6, 7, 8, 9));
        }

        /// <summary>
        /// Tests numerals.
        /// </summary>
        [TestMethod]
        public void TestNumerals()
        {
            // English.
            Assert.AreEqual("9.9G", EnglishNumerals.Default.ToApproximationString(9876543210));
            Assert.AreEqual(
                "negative nine billion eight hundred and seventy-six million five hundred and forty-three thousand two hundred and ten",
                EnglishNumerals.Default.ToString(-9876543210));
            Assert.AreEqual(
                "nine eight seven, six five four three, two one zero",
                EnglishNumerals.Default.ToString(9876543210, true));
            Assert.AreEqual(
                "three point one four one five nine two six five",
                EnglishNumerals.Default.ToString(3.14159265));
            Assert.AreEqual(
                "one point two three times ten of forty-five power",
                EnglishNumerals.Default.ToString(1.23e45));
            Assert.AreEqual(
                "负九十八亿七千六百五十四万三千两百一十",
                ChineseNumerals.Simplified.ToString(-9876543210));
            Assert.AreEqual(
                "玖捌柒陆伍肆叄贰壹零",
                ChineseNumerals.SimplifiedUppercase.ToString(9876543210, true));
            Assert.AreEqual(
                "三點一四一五九二六五",
                ChineseNumerals.Traditional.ToString(3.14159265));
            Assert.AreEqual(
                "一点二三乘以十的四十五次方",
                ChineseNumerals.Simplified.ToString(1.23e45));

            // Fraction
            var f1 = new Fraction(100, 200);
            var f2 = new Fraction(2L, 4L);
            Assert.AreEqual(1, f1.Numerator);
            Assert.AreEqual(2, f1.Denominator);
            Assert.AreEqual(0, f1.IntegerPart);
            Assert.AreEqual(1, f1.NumeratorOfDecimalPart);
            Assert.AreEqual(1L, f1.LongNumerator);
            Assert.AreEqual(2L, f1.LongDenominator);
            Assert.AreEqual(0L, f1.LongIntegerPart);
            Assert.AreEqual(1L, f1.LongNumeratorOfDecimalPart);
            Assert.IsTrue(f1.IsPositive);
            Assert.IsFalse(f1.IsNegative);
            Assert.IsFalse(f1.IsInteger);
            Assert.IsFalse(f1.IsInfinity);
            Assert.IsFalse(f1.IsNaN);
            Assert.AreEqual(0.5, (double)f1);
            Assert.AreEqual(f1, f2);
            Assert.IsFalse(f1 > f2);
            Assert.IsTrue(f1 >= f2);
            f1 += f2;
            Assert.AreEqual(1, f1.Numerator);
            Assert.AreEqual(1, f1.Denominator);
            Assert.IsTrue(f1.IsPositive);
            Assert.IsFalse(f1.IsNegative);
            Assert.IsTrue(f1.IsInteger);
            Assert.IsFalse(f1.IsInfinity);
            Assert.IsFalse(f1.IsNaN);
            Assert.AreEqual(1, (int)f1);
            Assert.AreNotEqual(f1, f2);
            Assert.IsTrue(f1 > f2);
            Assert.IsTrue(f1 >= f2);
            f2 = new Fraction(1);
            Assert.AreEqual(f1, f2);
            Assert.AreEqual(1, (int)f2);
            Assert.AreEqual(1L, (long)f2);
            Assert.AreEqual(-1, (int)-f2);
            f1 /= 3;
            f1 += f2;
            f1 += 1;
            Assert.AreEqual(7, f1.Numerator);
            Assert.AreEqual(3, f1.Denominator);
            Assert.AreEqual(2, f1.IntegerPart);
            Assert.AreEqual(1, f1.NumeratorOfDecimalPart);
            Assert.IsTrue(f1.IsPositive);
            Assert.IsFalse(f1.IsNegative);
            Assert.IsFalse(f1.IsInteger);
            Assert.IsFalse(f1.IsInfinity);
            Assert.IsFalse(f1.IsNaN);
            Assert.IsFalse(f1 < f2);
            Assert.IsFalse(f1 <= f2);
            f2 = new Fraction(100, 0);
            Assert.IsTrue(f2.IsInfinity);
            Assert.IsTrue(f2.IsPositive);
            Assert.IsTrue(f2.IsPositiveInfinity);
            Assert.IsFalse(f2.IsNegative);
            Assert.IsFalse(f2.IsNegativeInfinity);
            Assert.IsFalse(f2.IsNaN);
            Assert.AreNotEqual(f1, f2);
            f2 = new Fraction(0, 0);
            Assert.IsFalse(f2.IsInfinity);
            Assert.IsFalse(f2.IsPositive);
            Assert.IsFalse(f2.IsPositiveInfinity);
            Assert.IsFalse(f2.IsNegative);
            Assert.IsFalse(f2.IsNegativeInfinity);
            Assert.IsTrue(f2.IsNaN);
            Assert.AreNotEqual(f1, f2);
        }
    }
}
