using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths
{
    /// <summary>
    /// JSON unit test.
    /// </summary>
    [TestClass]
    public class SetUnitTest
    {
        /// <summary>
        /// Tests writable JSON DOM.
        /// </summary>
        [TestMethod]
        public void TestSimpleInterval()
        {
            var a = IntervalUtility.ParseForInt32("(3.6, 100.9)");
            Assert.AreEqual(4, a.MinValue);
            Assert.AreEqual(100, a.MaxValue);
            Assert.IsFalse(a.LeftOpen);
            Assert.IsFalse(a.RightOpen);
            Assert.IsFalse(a.IsInInterval(2));
            Assert.IsFalse(a.IsInInterval(3));
            Assert.IsTrue(a.IsInInterval(4));
            Assert.IsTrue(a.IsInInterval(5));
            Assert.IsTrue(a.IsInInterval(99));
            Assert.IsTrue(a.IsInInterval(100));
            Assert.IsFalse(a.IsInInterval(101));
            Assert.IsFalse(a.IsInInterval(102));

            a = IntervalUtility.ParseForInt32("[4, 9)");
            Assert.AreEqual(4, a.MinValue);
            Assert.AreEqual(9, a.MaxValue);
            Assert.IsFalse(a.LeftOpen);
            Assert.IsTrue(a.RightOpen);
            Assert.IsFalse(a.IsInInterval(2));
            Assert.IsTrue(a.IsInInterval(4));
            Assert.IsTrue(a.IsInInterval(8));
            Assert.IsFalse(a.IsInInterval(9));
            Assert.IsFalse(a.IsInInterval(10));

            a = IntervalUtility.ParseForInt32("(-20, 30]");
            Assert.AreEqual(-20, a.MinValue);
            Assert.AreEqual(30, a.MaxValue);
            Assert.IsTrue(a.LeftOpen);
            Assert.IsFalse(a.RightOpen);

            a = IntervalUtility.ParseForInt32("(20, )");
            Assert.AreEqual(20, a.MinValue);
            Assert.AreEqual(int.MaxValue, a.MaxValue);
            Assert.IsTrue(a.LeftOpen);
            Assert.IsFalse(a.RightOpen);
            Assert.IsFalse(a.IsInInterval(int.MinValue));
            Assert.IsFalse(a.IsInInterval(4));
            Assert.IsFalse(a.IsInInterval(20));
            Assert.IsTrue(a.IsInInterval(21));
            Assert.IsTrue(a.IsInInterval(100000));
            Assert.IsTrue(a.IsInInterval(int.MaxValue));

            a = IntervalUtility.ParseForInt32("(-Infinity, ¡Þ]");
            Assert.AreEqual(int.MinValue, a.MinValue);
            Assert.AreEqual(int.MaxValue, a.MaxValue);
            Assert.IsFalse(a.LeftOpen);
            Assert.IsFalse(a.RightOpen);
            Assert.IsTrue(a.IsInInterval(int.MinValue));
            Assert.IsTrue(a.IsInInterval(-1024));
            Assert.IsTrue(a.IsInInterval(0));
            Assert.IsTrue(a.IsInInterval(int.MaxValue));

            var b = IntervalUtility.ParseForDouble("(null, +¡Þ]");
            Assert.AreEqual(double.NegativeInfinity, b.MinValue);
            Assert.AreEqual(double.PositiveInfinity, b.MaxValue);
            Assert.IsTrue(b.LeftOpen);
            Assert.IsTrue(b.RightOpen);
            Assert.IsTrue(b.IsInInterval(int.MinValue));
            Assert.IsTrue(b.IsInInterval(-1024));
            Assert.IsTrue(b.IsInInterval(0));
            Assert.IsTrue(b.IsInInterval(int.MaxValue));

            b = IntervalUtility.ParseForDouble("(3.1415926, 900]");
            Assert.AreEqual(3.1415926, b.MinValue);
            Assert.AreEqual(900, b.MaxValue);
            Assert.IsTrue(b.LeftOpen);
            Assert.IsFalse(b.RightOpen);
            Assert.IsFalse(b.IsInInterval(int.MinValue));
            Assert.IsFalse(b.IsInInterval(3.1));
            Assert.IsTrue(b.IsInInterval(3.2));
            Assert.IsTrue(b.IsInInterval(900));
            Assert.IsFalse(b.IsInInterval(1920));
            Assert.IsFalse(b.IsInInterval(int.MaxValue));

            var c = IntervalUtility.ParseForNullableInt64("(-1000, +¡Þ]");
            Assert.AreEqual(-1000, c.MinValue);
            Assert.IsNull(c.MaxValue);
            Assert.IsTrue(c.LeftOpen);
            Assert.IsTrue(c.RightOpen);
            Assert.IsFalse(c.IsInInterval(int.MinValue));
            Assert.IsTrue(c.IsInInterval(-200));
            Assert.IsTrue(c.IsInInterval(0));
            Assert.IsTrue(c.IsInInterval(int.MaxValue));

            c = IntervalUtility.ParseForNullableInt64("[100, -2]");
            Assert.AreEqual(100, c.MinValue);
            Assert.AreEqual(-2, c.MaxValue);
            Assert.IsFalse(c.LeftOpen);
            Assert.IsFalse(c.RightOpen);
            Assert.IsFalse(c.IsInInterval(3));
            Assert.IsFalse(c.IsInInterval(-200));

            var d = VersionSimpleInterval.Parse("[1.0.2, 3.4)");
            Assert.AreEqual("1.0.2", d.MinValue);
            Assert.AreEqual("3.4", d.MaxValue);
            Assert.IsFalse(d.LeftOpen);
            Assert.IsTrue(d.RightOpen);
            Assert.IsFalse(d.IsInInterval("1.0.0"));
            Assert.IsFalse(d.IsInInterval("1.0.1"));
            Assert.IsTrue(d.IsInInterval("1.0.2"));
            Assert.IsTrue(d.IsInInterval("1.0.2.0"));
            Assert.IsTrue(d.IsInInterval("1.1"));
            Assert.IsTrue(d.IsInInterval("1.1.0.0-beta2.717+abcdefg"));
            Assert.IsTrue(d.IsInInterval("1.1.0.0-rc1.0+hijklmn"));
            Assert.IsTrue(d.IsInInterval("2.0"));
            Assert.IsTrue(d.IsInInterval("3"));
            Assert.IsTrue(d.IsInInterval("3.3"));
            Assert.IsFalse(d.IsInInterval("3.4"));
            Assert.IsFalse(d.IsInInterval("3.4.0"));
            Assert.IsFalse(d.IsInInterval("3.4-preview"));
            Assert.IsFalse(d.IsInInterval("3.4.0-preview"));
            Assert.IsFalse(d.IsInInterval("3.5.0.1000"));

            d = VersionSimpleInterval.Parse("(NaN, 6.0]");
            Assert.IsNull(d.MinValue);
            Assert.AreEqual("6.0", d.MaxValue);
            Assert.IsTrue(d.LeftOpen);
            Assert.IsFalse(d.RightOpen);
            Assert.IsTrue(d.IsInInterval("0.1.0.0"));
            Assert.IsTrue(d.IsInInterval("2.0"));
            Assert.IsTrue(d.IsInInterval("6.0.0.0"));
            Assert.IsFalse(d.IsInInterval("6.2.0.100"));
        }
    }
}
