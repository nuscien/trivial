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
            Assert.IsTrue(await Arithmetic.IsPrimeAsync(2147483647));
            Assert.IsFalse(await Arithmetic.IsPrimeAsync(21474836477));

            // GCD & LCM
            Assert.AreEqual(64, Arithmetic.Gcd(192, 128));
            Assert.AreEqual(1, Arithmetic.Gcd(67, 31));
            Assert.AreEqual(384, Arithmetic.Lcm(192, 128));
            Assert.AreEqual(2077, Arithmetic.Lcm(67, 31));

            // Factorial.
            Assert.AreEqual(2432902008176640000, Arithmetic.Factorial(20));

            // Positional notation.
            Assert.AreEqual("120", Arithmetic.ToPositionalNotationString(168, 12));
            Assert.AreEqual("8a", Arithmetic.ToPositionalNotationString(170.0, 20));
            Assert.AreEqual("8a.2", Arithmetic.ToPositionalNotationString(170.1, 20));
            Assert.AreEqual("3.47d01bpf", Arithmetic.ToPositionalNotationString(3.14159265, 30));
            Assert.AreEqual("0.6204620462", Arithmetic.ToPositionalNotationString(0.9, 7));
            Assert.AreEqual(168, Arithmetic.ParseToInt32("120", 12));
            Assert.AreEqual(-168L, Arithmetic.ParseToInt64("-120", 12));
            Assert.AreEqual(170, Arithmetic.ParseToInt32("8a", 20));
            Assert.IsFalse(Arithmetic.TryParseToInt32("8a", 9, out _));
            Assert.IsFalse(Arithmetic.TryParseToInt32("8a", 8, out _));
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
        }
    }
}
