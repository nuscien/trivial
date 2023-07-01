using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The item of Gaussian Mixture Model.
/// </summary>
public struct GaussianMixtureModelItem
{
    /// <summary>
    /// Initializes a new instance of the GaussianMixtureModelItem class.
    /// </summary>
    /// <param name="weight">The weight,</param>
    /// <param name="mean">The mean.</param>
    /// <param name="variance">The variance.</param>
    public GaussianMixtureModelItem(double weight, double mean, double variance)
    {
        Weight = weight;
        Mean = mean;
        Variance = variance;
    }

    /// <summary>
    /// Gets the weights.
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Gets the mean.
    /// </summary>
    public double Mean { get; }

    /// <summary>
    /// Gets the variances.
    /// </summary>
    public double Variance { get; }

    /// <summary>
    /// Calculates the Gaussian Mixture Model.
    /// </summary>
    /// <param name="count">The count of the component.</param>
    /// <param name="data">The input data.</param>
    /// <param name="maxIterations">The max count of iterations.</param>
    /// <param name="tolerance">The tolerance.</param>
    /// <param name="random">An optional random object.</param>
    /// <returns>A collection of Gaussian Mixture Model.</returns>
    public static List<GaussianMixtureModelItem> Calculate(int count, double[] data, int maxIterations = 100, double tolerance = 1e-6, Random random = null)
    {
        var n = data.Length;
        random ??= new();
        var weights = Enumerable.Range(0, count).Select(_ => 1.0 / count).ToArray();
        var means = Enumerable.Range(0, count).Select(_ => data[random.Next(n)]).ToArray();
        var variances = Enumerable.Range(0, count).Select(_ => data.Select(x => Math.Pow(x - means[0], 2)).Average()).ToArray();
        var newWeights = new double[count];
        var newMeans = new double[count];
        var newVariances = new double[count];
        var responsibilities = new double[n, count];
        var prevLogLikelihood = double.NegativeInfinity;
        for (var iter = 0; iter < maxIterations; iter++)
        {
            for (var i = 0; i < n; i++)
            {
                var probs = new double[count];
                for (var j = 0; j < count; j++)
                {
                    probs[j] = weights[j] * StatisticalMethod.GaussianMixture(data[i], means[j], variances[j]);
                }
                var sumProbs = probs.Sum();
                for (var j = 0; j < count; j++)
                {
                    responsibilities[i, j] = probs[j] / sumProbs;
                }
            }

            for (var j = 0; j < count; j++)
            {
                var respSum = 0.0;
                for (var i = 0; i < n; i++)
                {
                    respSum += responsibilities[i, j];
                }

                newWeights[j] = respSum / n;
                newMeans[j] = 0;
                for (var i = 0; i < n; i++)
                {
                    newMeans[j] += responsibilities[i, j] * data[i];
                }

                newMeans[j] /= respSum;
                newVariances[j] = 0;
                for (var i = 0; i < n; i++)
                {
                    newVariances[j] += responsibilities[i, j] * Math.Pow(data[i] - newMeans[j], 2);
                }

                newVariances[j] /= respSum;
            }

            weights = newWeights;
            means = newMeans;
            variances = newVariances;
            var logLikelihood = 0.0;
            for (var i = 0; i < n; i++)
            {
                var probSum = 0.0;
                for (var j = 0; j < count; j++)
                {
                    probSum += weights[j] * StatisticalMethod.GaussianMixture(data[i], means[j], variances[j]);
                }

                logLikelihood += Math.Log(probSum);
            }

            if (Math.Abs(logLikelihood - prevLogLikelihood) < tolerance) break;
            prevLogLikelihood = logLikelihood;
        }

        var list = new List<GaussianMixtureModelItem>();
        for (var i = 0; i < count; i++)
        {
            list.Add(new(weights[i], means[i], variances[i]));
        }

        return list;
    }
}
