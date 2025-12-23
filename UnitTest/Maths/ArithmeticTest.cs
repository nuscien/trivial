using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;

namespace Trivial.Maths;

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
        Assert.IsTrue(await Arithmetic.IsPrimeAsync(2147483647L));
#if NETCOREAPP
        Assert.IsTrue(await Arithmetic.IsPrimeAsync((Int128)2147483647));
#endif
        Assert.AreEqual(17, await Arithmetic.PreviousPrimeAsync(19));
        Assert.IsLessThan(968455, await Arithmetic.PreviousPrimeAsync(968455));
        Assert.IsGreaterThan(968455, await Arithmetic.NextPrimeAsync(968455));

        // GCD & LCM
        Assert.AreEqual(64, Arithmetic.Gcd(192, 128));
        Assert.AreEqual(1, Arithmetic.Gcd(67, 31));
        Assert.AreEqual(384, Arithmetic.Lcm(192, 128));
        Assert.AreEqual(2077, Arithmetic.Lcm(67, 31));

        // Factorial.
        Assert.AreEqual(2432902008176640000, Arithmetic.Factorial(20));
        Assert.AreEqual(2432902008176640000.0, Arithmetic.FactorialApproximate(20));

        // Positional notation.
        Assert.AreEqual("120", Numbers.ToPositionalNotationString(168, 12));
        Assert.AreEqual("120", Numbers.ToPositionalNotationString((uint)168, 12));
        Assert.AreEqual("8a", Numbers.ToPositionalNotationString(170.0, 20));
        Assert.AreEqual("8a.2", Numbers.ToPositionalNotationString(170.1, 20));
        Assert.AreEqual("3.47d01bpf", Numbers.ToPositionalNotationString(3.14159265, 30));
        Assert.AreEqual("0.6204620462", Numbers.ToPositionalNotationString(0.9, 7));
        Assert.AreEqual((short)168, Numbers.ParseToInt16("120", 12));
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
#if NETCOREAPP
        Assert.AreEqual((Int128)17_000_000_000L, Numbers.ParseToInt64("17十亿", 10));
        Assert.AreEqual((Int128)(-200_000_000_000_000), Numbers.ParseToInt64("-200T", 10));
#endif
        Assert.AreEqual(1_000_000L, Numbers.ParseToInt64("百万", 10));
        Assert.AreEqual(1_000_000L, Numbers.ParseToInt64("mega", 10));

        // Compare
        Assert.AreEqual(1, Arithmetic.Min(1, 2, 3));
        Assert.AreEqual(1, Arithmetic.Min(1, 2, 3, 4, 5, 6));
        Assert.AreEqual(1, Arithmetic.Min(2, 1, 3));
        Assert.AreEqual(1, Arithmetic.Min(2, 1, 3, 4, 5, 6));
        Assert.AreEqual(1, Arithmetic.Min(8, 7, 1));
        Assert.AreEqual(1, Arithmetic.Min(2, 7, 3, 4, 1, 2));
        Assert.AreEqual(1L, Arithmetic.Min(1, 2L, 3));
        Assert.AreEqual(1L, Arithmetic.Min(1, 2L, 3, 4, 5, 6));
        Assert.AreEqual(1L, Arithmetic.Min(2, 1L, 3));
        Assert.AreEqual(1L, Arithmetic.Min(2, 1L, 3, 4, 5, 6));
        Assert.AreEqual(1L, Arithmetic.Min(8, 7L, 1));
        Assert.AreEqual(1L, Arithmetic.Min(2, 7L, 3, 4, 1, 2));
        Assert.AreEqual(1.0, Arithmetic.Min(1, 2.0, 3));
        Assert.AreEqual(1.0, Arithmetic.Min(1, 2.0, 3, 4, 5, 6));
        Assert.AreEqual(1.0, Arithmetic.Min(2, 1, 3.0));
        Assert.AreEqual(1.0, Arithmetic.Min(2, 1, 3.0, 4, 5, 6));
        Assert.AreEqual(1.0, Arithmetic.Min(8, 7, 1.0));
        Assert.AreEqual(1.0, Arithmetic.Min(2, 7, 3.0, 4, 1, 2));
        Assert.AreEqual(9, Arithmetic.Max(7, 8, 9));
        Assert.AreEqual(9, Arithmetic.Max(5, 6, 7, 8, 9));
        Assert.AreEqual(9, Arithmetic.Max(2, 9, 3));
        Assert.AreEqual(9, Arithmetic.Max(2, 9, 3, 4, 5, 6));
        Assert.AreEqual(9, Arithmetic.Max(9, 7, 1));
        Assert.AreEqual(9, Arithmetic.Max(9, 7, 3, 4, 1, 2));
        Assert.AreEqual(9L, Arithmetic.Max(7, 8L, 9));
        Assert.AreEqual(9L, Arithmetic.Max(5, 6L, 7, 8, 9));
        Assert.AreEqual(9L, Arithmetic.Max(2, 9L, 3));
        Assert.AreEqual(9L, Arithmetic.Max(2, 9L, 3, 4, 5, 6));
        Assert.AreEqual(9L, Arithmetic.Max(9, 7L, 1));
        Assert.AreEqual(9L, Arithmetic.Max(9, 7L, 3, 4, 1, 2));
        Assert.AreEqual(9.0, Arithmetic.Max(7, 8.0, 9));
        Assert.AreEqual(9.0, Arithmetic.Max(5, 6.0, 7, 8, 9));
        Assert.AreEqual(9.0, Arithmetic.Max(2, 9.0, 3));
        Assert.AreEqual(9.0, Arithmetic.Max(2, 9.0, 3, 4, 5, 6));
        Assert.AreEqual(9.0, Arithmetic.Max(9, 7.0, 1));
        Assert.AreEqual(9.0, Arithmetic.Max(9, 7.0, 3, 4, 1, 2));

        // Sign
        Assert.AreEqual(1, Arithmetic.Hardsign(100));
        Assert.AreEqual(-1, Arithmetic.Hardsign(-200));
        Assert.AreEqual(0, Arithmetic.Hardsign(0));
        Assert.AreEqual(0d, Arithmetic.Softsign(0));
        Assert.AreEqual(0f, Arithmetic.SoftsignF(0));
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

    /// <summary>
    /// Tests numerals.
    /// </summary>
    [TestMethod]
    public void TestArray()
    {
        var a = new[] { 1, 2, 3, 4 };
        var b = new[] { 5, 6, 7, 8, 9 };
        var c = Arithmetic.Plus(a, b);
        Assert.HasCount(5, c);
        Assert.AreEqual(6, c[0]);
        Assert.AreEqual(9, c[4]);
        c = Arithmetic.Minus(a, b);
        Assert.HasCount(5, c);
        Assert.AreEqual(-4, c[0]);
        Assert.AreEqual(-9, c[4]);
    }

    /// <summary>
    /// Tests numerals.
    /// </summary>
    [TestMethod]
    public void TestCollection()
    {
        var a = new List<int> { 1, 2, 3, 4 };
        var b = new List<int> { 5, 6, 7 };
        var c = Arithmetic.Plus(a, b).ToList();
        Assert.HasCount(4, c);
        Assert.AreEqual(6, c[0]);
        Assert.AreEqual(4, c[3]);
        c = Arithmetic.Plus(a, a, b).ToList();
        Assert.HasCount(4, c);
        Assert.AreEqual(7, c[0]);
        Assert.AreEqual(8, c[3]);
        c = Arithmetic.Plus(a, 8).ToList();
        Assert.HasCount(4, c);
        Assert.AreEqual(9, c[0]);
        Assert.AreEqual(12, c[3]);
        c = Arithmetic.Plus(a, b, 8).ToList();
        Assert.HasCount(4, c);
        Assert.AreEqual(14, c[0]);
        Assert.AreEqual(12, c[3]);
        c = Arithmetic.Minus(a, b).ToList();
        Assert.HasCount(4, c);
        Assert.AreEqual(-4, c[0]);
        Assert.AreEqual(4, c[3]);
        var d = Random(1000009);
        var e = Random(1000007);
        var f = Arithmetic.Plus(d, e);
        Assert.HasCount(d.Length, f);
        Assert.AreEqual(d.First() + e.First(), f.First());
        Assert.AreEqual(d.Last(), f.Last());
    }

    /// <summary>
    /// Tests boolean formular.
    /// </summary>
    [TestMethod]
    public void TestFormula()
    {
        var f = new BooleanBinaryOperationFormula(false, BinaryBooleanOperator.And, true);
        var s = JsonSerializer.Serialize(f);
        f = JsonSerializer.Deserialize<BooleanBinaryOperationFormula>(s);
        Assert.AreEqual(BinaryBooleanOperator.And, f.Operator);
        Assert.IsTrue(f.IsValid);
        Assert.IsFalse(f.Result);
        f = new BooleanBinaryOperationFormula(f.LeftValue, BinaryBooleanOperator.Xor, f.RightValue);
        s = JsonSerializer.Serialize(f);
        f = JsonSerializer.Deserialize<BooleanBinaryOperationFormula>(s);
        Assert.AreEqual(BinaryBooleanOperator.Xor, f.Operator);
        Assert.IsTrue(f.IsValid);
        Assert.IsTrue(f.Result);
        s = BooleanOperations.ToString(new List<BooleanBinaryOperationFormula>()
        {
            f, f, f
        });
        Assert.Contains(Environment.NewLine, s);
        Assert.IsGreaterThan(20, s.Length);

        Assert.IsFalse(BooleanOperations.Calculate(true, BinaryBooleanOperator.Nand, false, BinaryBooleanOperator.Xnor, false));
        var c = BooleanOperations.Calculate(BinaryBooleanOperator.Nor, [true, false, true, true], [false, false, false]).ToList();
        Assert.HasCount(4, c);
        Assert.IsFalse(c[0]);
        Assert.IsTrue(c[1]);
        Assert.IsFalse(c[2]);
        Assert.IsTrue(c[3]);
        c = BooleanOperations.Calculate(BinaryBooleanOperator.Or, [false, false, false], [true, false, true, true], true).ToList();
        Assert.HasCount(4, c);
        Assert.IsTrue(c[0]);
        Assert.IsFalse(c[1]);
        Assert.IsTrue(c[2]);
        Assert.IsTrue(c[3]);
        c = BooleanOperations.Calculate(BinaryBooleanOperator.Xor, [false], [true]).ToList();
        Assert.HasCount(1, c);
        Assert.IsTrue(c[0]);
        c = BooleanOperations.Calculate(BinaryBooleanOperator.Xnor, [], []).ToList();
        Assert.IsEmpty(c);

        var col = new List<bool>()
        {
            true, false, false, true, false, true, false, true, false
        };
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.And, col, true));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Or, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Nand, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Nor, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.First, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Last, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Half, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Nhalf, col, false));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Most, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Least, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.MostOrFirst, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.MostOrLast, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.LeastOrFirst, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.LeastOrLast, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Positive, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Negative, col));
        col.Add(true);
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.And, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Or, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Nand, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Nor, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.First, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Last, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Half, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Nhalf, col));
        Assert.IsNull(BooleanOperations.Calculate(SequenceBooleanOperator.Most, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.Least, col, false));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.MostOrFirst, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.MostOrLast, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.LeastOrFirst, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.LeastOrLast, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Positive, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Negative, col));
        col.Clear();
        Assert.IsNull(BooleanOperations.Calculate(SequenceBooleanOperator.And, col));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Or, col, true));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.Half, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.First, col, false));
        Assert.IsNull(BooleanOperations.Calculate(SequenceBooleanOperator.First, null));
        Assert.IsTrue(BooleanOperations.Calculate(SequenceBooleanOperator.True, col));
        Assert.IsFalse(BooleanOperations.Calculate(SequenceBooleanOperator.False, col));
    }

    private static int[] Random(int count)
    {
        var random = new Random();
        var nums = new int[count];
        for (int i = 0; i < count; i++)
        {
            nums[i] = random.Next(1_000_000_000);
        }

        return nums;
    }
}
