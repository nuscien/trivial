using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths;

/// <summary>
/// Statistical method unit tests.
/// </summary>
[TestClass]
public class StatisticalTest
{
    [TestMethod]
    public void TestDescriptiveStatistics()
    {
        var values = new[] { 1, 2, 3 };
        Assert.AreEqual(2d, StatisticalMethod.Mean(values), 1e-12);
        Assert.AreEqual(2d / 3, StatisticalMethod.Variance(values), 1e-12);
        Assert.AreEqual(1d, StatisticalMethod.SampleVariance(values), 1e-12);
        Assert.AreEqual(Math.Sqrt(2d / 3), StatisticalMethod.StandardDeviation(values), 1e-12);
        Assert.AreEqual(1d, StatisticalMethod.SampleDeviation(values), 1e-12);
        Assert.AreEqual(2d, StatisticalMethod.Mean(new long[] { 1, 2, 3 }), 1e-12);
        Assert.AreEqual(2f, StatisticalMethod.Mean(new float[] { 1, 2, 3 }), 1e-6f);
        Assert.AreEqual(2d, StatisticalMethod.Mean(new double[] { 1, 2, 3 }), 1e-12);
        Assert.AreEqual(2m, StatisticalMethod.Mean(new decimal[] { 1, 2, 3 }));
        Assert.AreEqual(2d / 3, StatisticalMethod.Variance(new long[] { 1, 2, 3 }), 1e-12);
        Assert.AreEqual(2d / 3, StatisticalMethod.Variance(new float[] { 1, 2, 3 }), 1e-6);
        Assert.AreEqual(2d / 3, StatisticalMethod.Variance(new double[] { 1, 2, 3 }), 1e-12);
        Assert.AreEqual(1d, StatisticalMethod.SampleVariance(new double[] { 1, 2, 3 }), 1e-12);

        Assert.IsTrue(double.IsNaN(StatisticalMethod.Mean(Array.Empty<double>())));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.Variance(Array.Empty<int>())));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.SampleVariance(new[] { 5 })));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.Variance((IEnumerable<double>)null)));
        var deferredValues = Enumerable.Range(1, 3).Select(value => (double)value);
        Assert.AreEqual(2d / 3, StatisticalMethod.Variance(deferredValues), 1e-12);
        Assert.AreEqual(1d, StatisticalMethod.SampleVariance(deferredValues), 1e-12);
        Assert.IsTrue(double.IsNaN(StatisticalMethod.Variance(Enumerable.Empty<double>())));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.SampleVariance(Enumerable.Empty<double>())));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.Variance(Enumerable.Empty<float>())));
        Assert.IsTrue(double.IsNaN(StatisticalMethod.SampleVariance(Enumerable.Empty<int>())));
        Assert.AreEqual(0m, StatisticalMethod.Mean(Array.Empty<decimal>()));

        Assert.AreEqual(3, StatisticalMethod.Median(new[] { 5, 1, 3 }, out var medianCount));
        Assert.AreEqual(1, medianCount);
        Assert.AreEqual(3, StatisticalMethod.Median(new[] { 4, 1, 3, 2 }, out medianCount));
        Assert.AreEqual(2, medianCount);
        Assert.AreEqual(0, StatisticalMethod.Median(Array.Empty<int>(), out medianCount));
        Assert.AreEqual(0, medianCount);
        Assert.AreEqual("cc", StatisticalMethod.Median(new[] { "aaa", "b", "cc" }, value => value.Length));

        var modes = StatisticalMethod.Mode(new[] { 1, 1, 2, 2, 3 });
        Assert.HasCount(2, modes);
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, modes.ToArray());
        Assert.HasCount(0, StatisticalMethod.Mode(Array.Empty<int>()));
        Assert.IsNull(StatisticalMethod.Mode<int>(null));
    }

    [TestMethod]
    public void TestScoreTransforms()
    {
        var softmax = StatisticalMethod.Softmax(new[] { 1, 2, 3 });
        Assert.HasCount(3, softmax);
        Assert.AreEqual(1d, softmax.Sum(), 1e-12);
        Assert.IsTrue(softmax[2] > softmax[1] && softmax[1] > softmax[0]);
        Assert.AreEqual(softmax[1], StatisticalMethod.Softmax(new[] { 1, 2, 3 }.AsSpan())[1], 1e-12);

        var softmin = StatisticalMethod.Softmin(new[] { 1, 2, 3 });
        Assert.AreEqual(1d, softmin.Sum(), 1e-12);
        Assert.IsTrue(softmin[0] > softmin[1] && softmin[1] > softmin[2]);
        Assert.AreEqual(softmin[1], StatisticalMethod.Softmin(new[] { 1, 2, 3 }.AsSpan())[1], 1e-12);
        Assert.IsNull(StatisticalMethod.Softmax((IEnumerable<double>)null));
        Assert.IsNull(StatisticalMethod.Softmin((IEnumerable<double>)null));

        var tied = new[] { 1, 5, 5, 2 };
        CollectionAssert.AreEqual(new[] { 0, 1, 1, 0 }, StatisticalMethod.Hardmax(tied));
        CollectionAssert.AreEqual(new[] { 0, 1, 1, 0 }, StatisticalMethod.Hardmax(tied.AsSpan()));
        CollectionAssert.AreEqual(new[] { 0, 0.5, 0.5, 0 }, StatisticalMethod.Hardmax(tied, true));
        CollectionAssert.AreEqual(new[] { 0, 0, 0, 1 }, StatisticalMethod.Hardmin(new[] { 3, 5, 4, 1 }));
        CollectionAssert.AreEqual(new[] { 0, 0, 0, 1 }, StatisticalMethod.Hardmin(new[] { 3, 5, 4, 1 }.AsSpan()));
        CollectionAssert.AreEqual(new[] { 0.5, 0, 0.5 }, StatisticalMethod.Hardmin(new[] { 1, 2, 1 }, true));

#if !NETFRAMEWORK
        var softmaxF = StatisticalMethod.SoftmaxF(new[] { 1, 2, 3 });
        Assert.AreEqual(1f, softmaxF.Sum(), 1e-6f);
        var softminF = StatisticalMethod.SoftminF(new[] { 1, 2, 3 });
        Assert.AreEqual(1f, softminF.Sum(), 1e-6f);
        Assert.AreEqual(2f / 3, StatisticalMethod.VarianceF(new[] { 1f, 2f, 3f }), 1e-6f);
        Assert.AreEqual(1f, StatisticalMethod.SampleVarianceF(new[] { 1f, 2f, 3f }), 1e-6f);
        Assert.AreEqual(MathF.Sqrt(2f / 3), StatisticalMethod.StandardDeviationF(new[] { 1f, 2f, 3f }), 1e-6f);
        Assert.AreEqual(1f, StatisticalMethod.SampleDeviationF(new[] { 1f, 2f, 3f }), 1e-6f);
        Assert.IsTrue(float.IsNaN(StatisticalMethod.VarianceF(Array.Empty<float>())));
        Assert.IsTrue(float.IsNaN(StatisticalMethod.SampleVarianceF(Array.Empty<float>())));
#endif
    }

    [TestMethod]
    public void TestLargeArrayVariance()
    {
        var doubles = Enumerable.Range(0, 10003).Select(i => (double)(i % 101) - 50).ToArray();
        var doubleAverage = doubles.Average();
        var expectedVariance = doubles.Sum(value => Math.Pow(value - doubleAverage, 2)) / doubles.Length;
        var expectedSampleVariance = doubles.Sum(value => Math.Pow(value - doubleAverage, 2)) / (doubles.Length - 1);
        Assert.AreEqual(expectedVariance, StatisticalMethod.Variance(doubles), 1e-10);
        Assert.AreEqual(expectedSampleVariance, StatisticalMethod.SampleVariance(doubles), 1e-10);
        Assert.AreEqual(Math.Sqrt(expectedVariance), StatisticalMethod.StandardDeviation(doubles), 1e-10);
        Assert.AreEqual(Math.Sqrt(expectedSampleVariance), StatisticalMethod.SampleDeviation(doubles), 1e-10);

        var floats = Enumerable.Range(0, 10003).Select(i => (float)(i % 101) - 50).ToArray();
        var floatAverage = floats.Average();
        var expectedFloatVariance = floats.Sum(value => Math.Pow(value - floatAverage, 2)) / floats.Length;
        var expectedFloatSampleVariance = floats.Sum(value => Math.Pow(value - floatAverage, 2)) / (floats.Length - 1);
        Assert.AreEqual(expectedFloatVariance, StatisticalMethod.Variance(floats), 1e-8);
        Assert.AreEqual(expectedFloatSampleVariance, StatisticalMethod.SampleVariance(floats), 1e-8);
        Assert.AreEqual(Math.Sqrt(expectedFloatVariance), StatisticalMethod.StandardDeviation(floats), 1e-8);
        Assert.AreEqual(Math.Sqrt(expectedFloatSampleVariance), StatisticalMethod.SampleDeviation(floats), 1e-8);

#if !NETFRAMEWORK
        var expectedVarianceF = floats.Sum(value => MathF.Pow(value - floatAverage, 2)) / floats.Length;
        var expectedSampleVarianceF = floats.Sum(value => MathF.Pow(value - floatAverage, 2)) / (floats.Length - 1);
        Assert.AreEqual(expectedVarianceF, StatisticalMethod.VarianceF(floats), 1e-3f);
        Assert.AreEqual(expectedSampleVarianceF, StatisticalMethod.SampleVarianceF(floats), 1e-3f);
        Assert.AreEqual(MathF.Sqrt(expectedVarianceF), StatisticalMethod.StandardDeviationF(floats), 1e-3f);
        Assert.AreEqual(MathF.Sqrt(expectedSampleVarianceF), StatisticalMethod.SampleDeviationF(floats), 1e-3f);
#endif
    }

    [TestMethod]
    public void TestMinMax()
    {
        var values = new[] { 4, 1, 9, 9, 2 };
        Assert.AreEqual(1, StatisticalMethod.Min(values, out var minIndex));
        Assert.AreEqual(1, minIndex);
        Assert.AreEqual(9, StatisticalMethod.Max(values, out var maxIndex));
        Assert.AreEqual(2, maxIndex);
        var list = new List<int>(values);
        Assert.AreEqual(1, StatisticalMethod.Min(list, out var listMinIndex));
        Assert.AreEqual(1, listMinIndex);
        Assert.AreEqual(9, StatisticalMethod.Max(list, out var listMaxIndex));
        Assert.AreEqual(2, listMaxIndex);
        Assert.AreEqual(-1, StatisticalMethod.Min(Array.Empty<int>(), -1));
        Assert.AreEqual(-1, StatisticalMethod.Max(Array.Empty<int>(), -1));
        Assert.ThrowsExactly<ArgumentNullException>(() => StatisticalMethod.Min<int>(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => StatisticalMethod.Max<int>(null));
        Assert.ThrowsExactly<InvalidOperationException>(() => StatisticalMethod.Min(Array.Empty<int>()));
        Assert.ThrowsExactly<InvalidOperationException>(() => StatisticalMethod.Max(Array.Empty<int>()));
    }

    [TestMethod]
    public void TestDifferences()
    {
        CollectionAssert.AreEqual(new[] { 3, -1 }, StatisticalMethod.Difference(new[] { 10, 13, 12 }).ToArray());
        CollectionAssert.AreEqual(new[] { 3L, -1L }, StatisticalMethod.Difference(new[] { 10L, 13L, 12L }.AsSpan()));
        CollectionAssert.AreEqual(new short[] { 3, -1 }, StatisticalMethod.Difference(new short[] { 10, 13, 12 }).ToArray());
        CollectionAssert.AreEqual(new short[] { 3, -1 }, StatisticalMethod.Difference(new short[] { 10, 13, 12 }.AsSpan()));
        CollectionAssert.AreEqual(new[] { 3d, -1d }, StatisticalMethod.Difference(new[] { 10d, 13d, 12d }.AsSpan()));
        CollectionAssert.AreEqual(new[] { 1.5f, -0.5f }, StatisticalMethod.Difference(new[] { 1f, 2.5f, 2f }).ToArray());
        CollectionAssert.AreEqual(new[] { 1.5m, -0.5m }, StatisticalMethod.Difference(new[] { 1m, 2.5m, 2m }.AsSpan()));
        var start = new DateTime(2000, 1, 1);
        CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) }, StatisticalMethod.Difference(new[] { start, start.AddSeconds(2), start.AddSeconds(5) }).ToArray());
        var durations = new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(2) };
        CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(-1) }, StatisticalMethod.Difference(durations).ToArray());
        CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(-1) }, StatisticalMethod.Difference(durations.AsSpan()));
        Assert.IsFalse(StatisticalMethod.Difference((IEnumerable<int>)null).Any());
    }

    [TestMethod]
    public void TestPredictionAndSampling()
    {
        var classes = new[] { "A", "B" };
        var xTrain = new[,] { { 3, 0 }, { 2, 0 }, { 0, 3 }, { 0, 2 } };
        var yTrain = new[] { "A", "A", "B", "B" };
        var predictions = StatisticalMethod.NaiveBayes(classes, xTrain, yTrain, new[,] { { 1, 0 }, { 0, 1 } });
        CollectionAssert.AreEqual(new[] { "A", "B" }, predictions);

        var regression = StatisticalMethod.LinearRegression(
            new[,] { { 1d }, { 2d }, { 3d } },
            new[] { 3d, 5d, 7d },
            new[,] { { 4d }, { 5d } });
        Assert.AreEqual(9d, regression[0], 1e-10);
        Assert.AreEqual(11d, regression[1], 1e-10);

        var regressionF = StatisticalMethod.LinearRegression(
            new[,] { { 1f }, { 2f }, { 3f } },
            new[] { 3f, 5f, 7f },
            new[,] { { 4f } });
        Assert.AreEqual(9f, regressionF[0], 1e-5f);
        Assert.AreEqual(1 / Math.Sqrt(2 * Math.PI), StatisticalMethod.GaussianMixture(0d, 0d, 1d), 1e-12);
#if !NETFRAMEWORK
        Assert.AreEqual(1 / MathF.Sqrt(2 * MathF.PI), StatisticalMethod.GaussianMixture(0f, 0f, 1f), 1e-6f);
#endif

        var samples = StatisticalMethod.MetropolisHastings(20, 0, 1, 0.5, 10, new Random(1234));
        var repeatedSamples = StatisticalMethod.MetropolisHastings(20, 0, 1, 0.5, 10, new Random(1234));
        Assert.HasCount(20, samples);
        CollectionAssert.AreEqual(samples, repeatedSamples);
        Assert.IsTrue(samples.All(value => !double.IsNaN(value) && !double.IsInfinity(value)));
    }
}
