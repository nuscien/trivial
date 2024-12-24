using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths;

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
        var a = IntervalUtility.ParseForInt32("(3.6, 100.9)", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(4, a.MinValue);
        Assert.AreEqual(100, a.MaxValue);
        Assert.IsFalse(a.LeftOpen);
        Assert.IsFalse(a.RightOpen);
        Assert.IsFalse(a.Contains(2));
        Assert.IsFalse(a.Contains(3));
        Assert.IsTrue(a.Contains(4));
        Assert.IsTrue(a.Contains(5));
        Assert.IsTrue(a.Contains(99));
        Assert.IsTrue(a.Contains(100));
        Assert.IsFalse(a.Contains(101));
        Assert.IsFalse(a.Contains(102));

        a = IntervalUtility.ParseForInt32("[4, 9)", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(4, a.MinValue);
        Assert.AreEqual(9, a.MaxValue);
        Assert.IsFalse(a.LeftOpen);
        Assert.IsTrue(a.RightOpen);
        Assert.IsFalse(a.Contains(2));
        Assert.IsTrue(a.Contains(4));
        Assert.IsTrue(a.Contains(8));
        Assert.IsFalse(a.Contains(9));
        Assert.IsFalse(a.Contains(10));

        a = IntervalUtility.ParseForInt32("(-20, 30]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(-20, a.MinValue);
        Assert.AreEqual(30, a.MaxValue);
        Assert.IsTrue(a.LeftOpen);
        Assert.IsFalse(a.RightOpen);

        a = IntervalUtility.ParseForInt32("(20, )", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(20, a.MinValue);
        Assert.AreEqual(int.MaxValue, a.MaxValue);
        Assert.IsTrue(a.LeftOpen);
        Assert.IsFalse(a.RightOpen);
        Assert.IsFalse(a.Contains(int.MinValue));
        Assert.IsFalse(a.Contains(4));
        Assert.IsFalse(a.Contains(20));
        Assert.IsTrue(a.Contains(21));
        Assert.IsTrue(a.Contains(100000));
        Assert.IsTrue(a.Contains(int.MaxValue));

        a = IntervalUtility.ParseForInt32($"(-Infinity, {Numbers.InfiniteSymbol}]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(int.MinValue, a.MinValue);
        Assert.AreEqual(int.MaxValue, a.MaxValue);
        Assert.IsFalse(a.LeftOpen);
        Assert.IsFalse(a.RightOpen);
        Assert.IsTrue(a.Contains(int.MinValue));
        Assert.IsTrue(a.Contains(-1024));
        Assert.IsTrue(a.Contains(0));
        Assert.IsTrue(a.Contains(int.MaxValue));

        var b = IntervalUtility.ParseForDouble($"(null, {Numbers.PositiveInfiniteSymbol}]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(double.NegativeInfinity, b.MinValue);
        Assert.AreEqual(double.PositiveInfinity, b.MaxValue);
        Assert.IsTrue(b.LeftOpen);
        Assert.IsTrue(b.RightOpen);
        Assert.IsTrue(b.Contains(int.MinValue));
        Assert.IsTrue(b.Contains(-1024));
        Assert.IsTrue(b.Contains(0));
        Assert.IsTrue(b.Contains(int.MaxValue));

        b = IntervalUtility.ParseForDouble("(3.1415926, 900]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(3.1415926, b.MinValue);
        Assert.AreEqual(900, b.MaxValue);
        Assert.IsTrue(b.LeftOpen);
        Assert.IsFalse(b.RightOpen);
        Assert.IsFalse(b.Contains(int.MinValue));
        Assert.IsFalse(b.Contains(3.1));
        Assert.IsTrue(b.Contains(3.2));
        Assert.IsTrue(b.Contains(900));
        Assert.IsFalse(b.Contains(1920));
        Assert.IsFalse(b.Contains(int.MaxValue));

        var c = IntervalUtility.ParseForNullableInt64($"(-1000, {Numbers.PositiveInfiniteSymbol}]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(-1000, c.MinValue);
        Assert.IsNull(c.MaxValue);
        Assert.IsTrue(c.LeftOpen);
        Assert.IsTrue(c.RightOpen);
        Assert.IsFalse(c.Contains(int.MinValue));
        Assert.IsTrue(c.Contains(-200));
        Assert.IsTrue(c.Contains(0));
        Assert.IsTrue(c.Contains(int.MaxValue));

        c = IntervalUtility.ParseForNullableInt64("[100, -2]", NumberStyles.Any, CultureInfo.InvariantCulture);
        Assert.AreEqual(100, c.MinValue);
        Assert.AreEqual(-2, c.MaxValue);
        Assert.IsFalse(c.LeftOpen);
        Assert.IsFalse(c.RightOpen);
        Assert.IsFalse(c.Contains(3));
        Assert.IsFalse(c.Contains(-200));

        var d = VersionSimpleInterval.Parse("[1.0.2, 3.4)");
        Assert.AreEqual("1.0.2", d.MinValue);
        Assert.AreEqual("3.4", d.MaxValue);
        Assert.IsFalse(d.LeftOpen);
        Assert.IsTrue(d.RightOpen);
        Assert.IsFalse(d.Contains("1.0.0"));
        Assert.IsFalse(d.Contains("1.0.1"));
        Assert.IsTrue(d.Contains("1.0.2"));
        Assert.IsTrue(d.Contains("1.0.2.0"));
        Assert.IsTrue(d.Contains("1.1"));
        Assert.IsTrue(d.Contains("1.1.0.0-beta2.717+abcdefg"));
        Assert.IsTrue(d.Contains("1.1.0.0-rc1.0+hijklmn"));
        Assert.IsTrue(d.Contains("2.0"));
        Assert.IsTrue(d.Contains("3"));
        Assert.IsTrue(d.Contains("3.3"));
        Assert.IsFalse(d.Contains("3.4"));
        Assert.IsFalse(d.Contains("3.4.0"));
        Assert.IsFalse(d.Contains("3.4-preview"));
        Assert.IsFalse(d.Contains("3.4.0-preview"));
        Assert.IsFalse(d.Contains("3.5.0.1000"));

        d = VersionSimpleInterval.Parse("(NaN, 6.0]");
        Assert.IsNull(d.MinValue);
        Assert.AreEqual("6.0", d.MaxValue);
        Assert.IsTrue(d.LeftOpen);
        Assert.IsFalse(d.RightOpen);
        Assert.IsTrue(d.Contains("0.1.0.0"));
        Assert.IsTrue(d.Contains("2.0"));
        Assert.IsTrue(d.Contains("6.0.0.0"));
        Assert.IsFalse(d.Contains("6.2.0.100"));

        Assert.IsTrue(Reflection.VersionComparer.Compare("1.0.0", "1.0.0", false) == 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("1.0.1", "1.0.0", false) > 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("1.0.0", "1.0.1", false) < 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("1.1.0", "1.0.0", false) > 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("1.0.0", "1.1.0", false) < 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("1.0.0.0", "1.0.0", false) > 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("10.0.0.0", "9.100.0", false) > 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("10.10.0.0", "9.0.0", false) > 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("9.0.0.0", "10.0.0", false) < 0);
        Assert.IsTrue(Reflection.VersionComparer.Compare("9.0.0.0", "10.20.0", false) < 0);
    }

    /// <summary>
    /// Tests writable JSON DOM.
    /// </summary>
    [TestMethod]
    public void TestDimensions()
    {
        var p1 = new IntPoint1D(1);
        Assert.AreEqual(2, (p1 + p1).X);
        Assert.AreEqual(0, (p1 - p1).X);
        Assert.AreEqual(-1, (-p1).X);
        var s = JsonSerializer.Serialize(p1);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<IntPoint1D>(s).X);
        var p2 = new IntPoint2D(1, 2);
        Assert.AreEqual(2, (p2 + p2).X);
        Assert.AreEqual(0, (p2 - p2).X);
        Assert.AreEqual(-1, (-p2).X);
        s = JsonSerializer.Serialize(p2);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<IntPoint2D>(s).X);
        var p3 = new IntPoint3D(1, 2, 3);
        Assert.AreEqual(2, (p3 + p3).X);
        Assert.AreEqual(0, (p3 - p3).X);
        Assert.AreEqual(-1, (-p3).X);
        s = JsonSerializer.Serialize(p3);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<IntPoint3D>(s).X);
        var p1d = new DoublePoint1D(1);
        Assert.AreEqual(2.0, (p1d + p1d).X);
        Assert.AreEqual(0d, (p1d - p1d).X);
        Assert.AreEqual(-1.0, (-p1d).X);
        s = JsonSerializer.Serialize(p1d);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<DoublePoint1D>(s).X);
        var p2d = new DoublePoint2D(1, 2);
        Assert.AreEqual(2.0, (p2d + p2d).X);
        Assert.AreEqual(0d, (p2d - p2d).X);
        Assert.AreEqual(-1.0, (-p2d).X);
        s = JsonSerializer.Serialize(p2d);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<DoublePoint2D>(s).X);
        var p3d = new DoublePoint3D(1, 2, 3);
        Assert.AreEqual(2.0, (p3d + p3d).X);
        Assert.AreEqual(0d, (p3d - p3d).X);
        Assert.AreEqual(-1.0, (-p3d).X);
        s = JsonSerializer.Serialize(p3d);
        Assert.IsNotNull(s);
        Assert.AreEqual(1, JsonSerializer.Deserialize<DoublePoint3D>(s).X);
    }
}
