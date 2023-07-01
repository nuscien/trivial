using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

public static partial class StatisticalMethod
{
    /// <summary>
    /// Transposes a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Transpose(double[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var result = new double[cols, rows];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Transposes a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Transpose(float[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var result = new float[cols, rows];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Multiply(double[,] a, double[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        if (aCols != bRows) throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        var result = new double[aRows, bCols];

        for (var i = 0; i < aRows; i++)
        {
            for (var j = 0; j < bCols; j++)
            {
                for (var k = 0; k < aCols; k++)
                {
                    result[i, j] += a[i, k] * b[k, j];
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Multiply(float[,] a, float[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        if (aCols != bRows) throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        var result = new float[aRows, bCols];

        for (var i = 0; i < aRows; i++)
        {
            for (var j = 0; j < bCols; j++)
            {
                for (var k = 0; k < aCols; k++)
                {
                    result[i, j] += a[i, k] * b[k, j];
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static double[] Multiply(double[,] a, double[] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bLen = b.Length;
        if (aCols != bLen) throw new ArgumentException("Matrix and vector dimensions do not match for multiplication.");
        var result = new double[aRows];
        for (var i = 0; i < aRows; i++)
        {
            for (var j = 0; j < aCols; j++)
            {
                result[i] += a[i, j] * b[j];
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static float[] Multiply(float[,] a, float[] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bLen = b.Length;
        if (aCols != bLen) throw new ArgumentException("Matrix and vector dimensions do not match for multiplication.");
        var result = new float[aRows];
        for (var i = 0; i < aRows; i++)
        {
            for (var j = 0; j < aCols; j++)
            {
                result[i] += a[i, j] * b[j];
            }
        }

        return result;
    }

    /// <summary>
    /// Inverses a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Inverse(double[,] matrix)
    {
        var n = matrix.GetLength(0);
        if (n != matrix.GetLength(1)) throw new ArgumentException("Matrix must be square.");
        var result = new double[n, n];
        for (var i = 0; i < n; i++)
        {
            result[i, i] = 1;
        }

        for (var i = 0; i < n; i++)
        {
            var diag = matrix[i, i];
            for (var j = 0; j < n; j++)
            {
                matrix[i, j] /= diag;
                result[i, j] /= diag;
            }

            for (var k = 0; k < n; k++)
            {
                if (k == i) continue;
                var factor = matrix[k, i];
                for (var j = 0; j < n; j++)
                {
                    matrix[k, j] -= factor * matrix[i, j];
                    result[k, j] -= factor * result[i, j];
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Inverses a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Inverse(float[,] matrix)
    {
        var n = matrix.GetLength(0);
        if (n != matrix.GetLength(1)) throw new ArgumentException("Matrix must be square.");
        var result = new float[n, n];
        for (var i = 0; i < n; i++)
        {
            result[i, i] = 1;
        }

        for (var i = 0; i < n; i++)
        {
            var diag = matrix[i, i];
            for (var j = 0; j < n; j++)
            {
                matrix[i, j] /= diag;
                result[i, j] /= diag;
            }

            for (var k = 0; k < n; k++)
            {
                if (k == i) continue;
                var factor = matrix[k, i];
                for (var j = 0; j < n; j++)
                {
                    matrix[k, j] -= factor * matrix[i, j];
                    result[k, j] -= factor * result[i, j];
                }
            }
        }

        return result;
    }
}
