using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths;

/// <summary>
/// Arithmetic unit test.
/// </summary>
[TestClass]
public class StatisticalTest
{
    /// <summary>
    /// Tests min and max.
    /// </summary>
    [TestMethod]
    public void TestMinMax()
    {
        var col = new List<int> { 1, 2, 3, 9, 8, 7, 4, 6, 5 };
        var list = StatisticalMethod.Softmax(col);
        Assert.AreEqual(3, StatisticalMethod.IndexOfMax(list));
        Assert.AreEqual(0, StatisticalMethod.IndexOfMin(list));
        var arr = StatisticalMethod.Softmax(col.ToArray());
        Assert.AreEqual(3, StatisticalMethod.IndexOfMax(arr));
        Assert.AreEqual(0, StatisticalMethod.IndexOfMin(arr));
        list = StatisticalMethod.Hardmax(col, false);
        Assert.AreEqual(1, list[3]);
        Assert.AreEqual(0, list[8]);
        Assert.AreEqual(0, list[0]);
        arr = StatisticalMethod.Hardmax(col.ToArray(), true);
        Assert.AreEqual(1, arr[3]);
        Assert.AreEqual(0, arr[8]);
        Assert.AreEqual(0, list[0]);
        list = StatisticalMethod.Hardmin(col, false);
        Assert.AreEqual(0, list[3]);
        Assert.AreEqual(0, list[8]);
        Assert.AreEqual(1, list[0]);
        arr = StatisticalMethod.Hardmin(col.ToArray(), true);
        Assert.AreEqual(0, arr[3]);
        Assert.AreEqual(0, list[8]);
        Assert.AreEqual(1, arr[0]);
        var sample = StatisticalMethod.SampleDeviation(col);
        Assert.IsTrue(sample > 0);
        sample = StatisticalMethod.StandardDeviation(col);
        Assert.IsTrue(sample > 0);
        sample = StatisticalMethod.SampleVariance(col);
        Assert.IsTrue(sample > 0);
        sample = StatisticalMethod.Variance(col);
        Assert.IsTrue(sample > 0);
        Assert.AreEqual(5, StatisticalMethod.Mean(col));
        Assert.AreEqual(5, StatisticalMethod.Median(col));
        col = StatisticalMethod.Mode(col).ToList();
        Assert.IsTrue(col.Count == 9);
    }
}
