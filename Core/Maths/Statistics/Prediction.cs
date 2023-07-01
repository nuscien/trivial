using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var input = new List<string>(classes);
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

        return Enumerable.Range(0, xTest.GetLength(0)).Select(i => PredictSingle(input, classProb, wordProb, xTest, i));
    }

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

        double[,] Xt = Transpose(xExtended);
        double[,] XtX = Multiply(Xt, xExtended);
        double[,] XtX_inv = Inverse(XtX);
        double[,] XtX_inv_Xt = Multiply(XtX_inv, Xt);

        var weights = Multiply(XtX_inv_Xt, yTrain);
        int n2 = xTest.GetLength(0);
        int p2 = xTest.GetLength(1);

        double[,] xExtended2 = new double[n2, p2 + 1];
        for (int i = 0; i < n2; i++)
        {
            xExtended2[i, 0] = 1;
            for (int j = 0; j < p2; j++)
            {
                xExtended2[i, j + 1] = xTest[i, j];
            }
        }

        return Multiply(xExtended2, weights);
    }

    private static string PredictSingle(List<string> input, Dictionary<string, double> classProb, Dictionary<string, double[]> wordProb, int[,] matrix, int row)
    {
        var x = Enumerable.Range(0, matrix.GetLength(1)).Select(i => matrix[row, i]);
        return input.OrderByDescending(cls => classProb[cls] + x.Select((value, i) => value * wordProb[cls][i]).Sum()).First();
    }
}
