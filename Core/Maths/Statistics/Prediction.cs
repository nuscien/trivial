using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;

namespace Trivial.Maths;

public static partial class StatisticalMethod
{
    /// <summary>
    /// Predicts texts by Naive Bayes classification algorithm.
    /// </summary>
    /// <param name="classes">The clases.</param>
    /// <param name="xTrain">The train data set x.</param>
    /// <param name="yTrain">The train data set y.</param>
    /// <param name="xTest">The test data set x.</param>
    /// <returns>The data set predicted.</returns>
    public static IEnumerable<string> NaiveBayes(IEnumerable<string> classes, int[,] xTrain, string[] yTrain, int[,] xTest)
    {
        var input = ListExtensions.ToList(classes, false);
        var classProb = input.ToDictionary(cls => cls, cls => Math.Log(yTrain.Count(label => label == cls) / (double)yTrain.Length));
        var wordProb = new Dictionary<string, double[]>();
        foreach (var cls in input)
        {
            var xcls = yTrain.Select((label, i) => new { label, i }).Where(item => item.label == cls).Select(item => item.i).ToArray();
            var clsWordProb = new double[xTrain.GetLength(1)];
            for (int j = 0; j < xTrain.GetLength(1); j++)
            {
                clsWordProb[j] = xcls.Sum(i => xTrain[i, j]) + 1;
            }

            wordProb[cls] = clsWordProb.Select(p => Math.Log(p / (clsWordProb.Sum() + xTrain.GetLength(1)))).ToArray();
        }

        return Enumerable.Range(0, xTest.GetLength(0)).Select(i => PredictSingleByNaiveBayes(input, classProb, wordProb, xTest, i));
    }

    /// <summary>
    /// Predicts texts by Naive Bayes classification algorithm.
    /// </summary>
    /// <param name="classes">The clases.</param>
    /// <param name="xTrain">The train data set x.</param>
    /// <param name="yTrain">The train data set y.</param>
    /// <param name="xTest">The test data set x.</param>
    /// <returns>The data set predicted.</returns>
    public static string[] NaiveBayes(string[] classes, int[,] xTrain, string[] yTrain, int[,] xTest)
        => NaiveBayes(classes as IEnumerable<string>, xTrain, yTrain, xTest).ToArray();

    /// <summary>
    /// Predicts by linear regression.
    /// </summary>
    /// <param name="xTrain">The train data set x.</param>
    /// <param name="yTrain">The train data set y.</param>
    /// <param name="xTest">The test data set x.</param>
    /// <returns>The data set predicted.</returns>
    public static double[] LinearRegression(double[,] xTrain, double[] yTrain, double[,] xTest)
    {
        var n = xTrain.GetLength(0);
        var p = xTrain.GetLength(1);
        var xExtended = new double[n, p + 1];
        for (var i = 0; i < n; i++)
        {
            xExtended[i, 0] = 1;
            for (int j = 0; j < p; j++)
            {
                xExtended[i, j + 1] = xTrain[i, j];
            }
        }

        var xt = MatrixCalculation.Transpose(xExtended);
        var xtx = MatrixCalculation.Multiply(xt, xExtended);
        var xtxInv = MatrixCalculation.Inverse(xtx);
        var xtxInvXt = MatrixCalculation.Multiply(xtxInv, xt);
        var weights = MatrixCalculation.Multiply(xtxInvXt, yTrain);
        var n2 = xTest.GetLength(0);
        var p2 = xTest.GetLength(1);
        var xExtended2 = new double[n2, p2 + 1];
        for (int i = 0; i < n2; i++)
        {
            xExtended2[i, 0] = 1;
            for (int j = 0; j < p2; j++)
            {
                xExtended2[i, j + 1] = xTest[i, j];
            }
        }

        return MatrixCalculation.Multiply(xExtended2, weights);
    }

    /// <summary>
    /// Predicts by linear regression.
    /// </summary>
    /// <param name="xTrain">The train data set x.</param>
    /// <param name="yTrain">The train data set y.</param>
    /// <param name="xTest">The test data set x.</param>
    /// <returns>The data set predicted.</returns>
    public static float[] LinearRegression(float[,] xTrain, float[] yTrain, float[,] xTest)
    {
        var n = xTrain.GetLength(0);
        var p = xTrain.GetLength(1);
        var xExtended = new float[n, p + 1];
        for (var i = 0; i < n; i++)
        {
            xExtended[i, 0] = 1;
            for (int j = 0; j < p; j++)
            {
                xExtended[i, j + 1] = xTrain[i, j];
            }
        }

        var xt = MatrixCalculation.Transpose(xExtended);
        var xtx = MatrixCalculation.Multiply(xt, xExtended);
        var xtxInv = MatrixCalculation.Inverse(xtx);
        var xtxInvXt = MatrixCalculation.Multiply(xtxInv, xt);
        var weights = MatrixCalculation.Multiply(xtxInvXt, yTrain);
        var n2 = xTest.GetLength(0);
        var p2 = xTest.GetLength(1);
        var xExtended2 = new float[n2, p2 + 1];
        for (int i = 0; i < n2; i++)
        {
            xExtended2[i, 0] = 1;
            for (int j = 0; j < p2; j++)
            {
                xExtended2[i, j + 1] = xTest[i, j];
            }
        }

        return MatrixCalculation.Multiply(xExtended2, weights);
    }

    /// <summary>
    /// Generates sample by Metropolis-Hastings algorithm.
    /// </summary>
    /// <param name="n">The sample count.</param>
    /// <param name="mu">The means.</param>
    /// <param name="sigma">Sigma.</param>
    /// <param name="proposalSigma">Proposal sigma.</param>
    /// <param name="burnIn">The count to burn in.</param>
    /// <param name="random">An optional random object.</param>
    /// <returns>The sample data set;</returns>
    public static List<double> MetropolisHastings(int n, double mu, double sigma, double proposalSigma, int burnIn = 1000, Random random = null)
    {
        var samples = new List<double>();
        var current = mu;
        random ??= new();
        for (var i = 0; i < n + burnIn; i++)
        {
            var proposal = current + proposalSigma * random.NextDouble();
            var acceptanceRatio = Math.Exp((-0.5 * Math.Pow((proposal - mu) / sigma, 2)) + (0.5 * Math.Pow((current - mu) / sigma, 2)));
            if (random.NextDouble() < acceptanceRatio)
            {
                current = proposal;
            }

            if (i >= burnIn)
            {
                samples.Add(current);
            }
        }

        return samples;
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Generates sample by Metropolis-Hastings algorithm.
    /// </summary>
    /// <param name="n">The sample count.</param>
    /// <param name="mu">The mean.</param>
    /// <param name="sigma">Sigma.</param>
    /// <param name="proposalSigma">Proposal sigma.</param>
    /// <param name="burnIn">The count to burn in.</param>
    /// <param name="random">An optional random.</param>
    /// <returns>The sample data set;</returns>
    public static List<float> MetropolisHastings(int n, float mu, float sigma, float proposalSigma, int burnIn = 1000, Random random = null)
    {
        var samples = new List<float>();
        var current = mu;
        random ??= new Random();
        for (var i = 0; i < n + burnIn; i++)
        {
            var proposal = current + proposalSigma * random.NextSingle();
            var acceptanceRatio = MathF.Exp((-0.5f * MathF.Pow((proposal - mu) / sigma, 2)) + (0.5f * MathF.Pow((current - mu) / sigma, 2)));
            if (random.NextSingle() < acceptanceRatio)
            {
                current = proposal;
            }

            if (i >= burnIn)
            {
                samples.Add(current);
            }
        }

        return samples;
    }

    /// <summary>
    /// Calculates by Gaussian Mixture algorithm.
    /// </summary>
    /// <param name="x">The input data.</param>
    /// <param name="mean">The mean.</param>
    /// <param name="variance">The variance.</param>
    /// <returns>A number calculated by Gaussian Mixture algorithm.</returns>
    public static float GaussianMixture(float x, float mean, float variance)
        => MathF.Exp(-0.5f * MathF.Pow((x - mean) / MathF.Sqrt(variance), 2)) / MathF.Sqrt(2 * MathF.PI * variance);
#endif

    /// <summary>
    /// Calculates by Gaussian Mixture algorithm.
    /// </summary>
    /// <param name="x">The input data.</param>
    /// <param name="mean">The mean.</param>
    /// <param name="variance">The variance.</param>
    /// <returns>A number calculated by Gaussian Mixture algorithm.</returns>
    public static double GaussianMixture(double x, double mean, double variance)
        => Math.Exp(-0.5 * Math.Pow((x - mean) / Math.Sqrt(variance), 2)) / Math.Sqrt(2 * Math.PI * variance);

    private static string PredictSingleByNaiveBayes(List<string> input, Dictionary<string, double> classProb, Dictionary<string, double[]> wordProb, int[,] matrix, int row)
    {
        var x = Enumerable.Range(0, matrix.GetLength(1)).Select(i => matrix[row, i]);
        return input.OrderByDescending(cls => classProb[cls] + x.Select((value, i) => value * wordProb[cls][i]).Sum()).First();
    }
}
