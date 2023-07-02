using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The matrix calculation.
/// </summary>
public static class MatrixCalculation
{
    /// <summary>
    /// Transposes a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static int[,] Transpose(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var result = new int[cols, rows];
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
    /// Inverses a matrix.
    /// </summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    public static int[,] Inverse(int[,] matrix)
    {
        var n = matrix.GetLength(0);
        if (n != matrix.GetLength(1)) throw new ArgumentException("Matrix must be square.");
        var result = new int[n, n];
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

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static int[,] Plus(int[,] a, int[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new int[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <param name="c">The input matrix c.</param>
    /// <returns>The output matrix.</returns>
    public static int[,] Plus(int[,] a, int[,] b, int[,] c)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var cRows = c.GetLength(0);
        var cCols = c.GetLength(1);
        var rows = Math.Max(Math.Max(aRows, bRows), cRows);
        var cols = Math.Max(Math.Max(aCols, bCols), cCols);
        var result = new int[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0) + (i < cRows && j < cCols ? c[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Plus(double[,] a, double[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new double[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <param name="c">The input matrix c.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Plus(double[,] a, double[,] b, double[,] c)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var cRows = c.GetLength(0);
        var cCols = c.GetLength(1);
        var rows = Math.Max(Math.Max(aRows, bRows), cRows);
        var cols = Math.Max(Math.Max(aCols, bCols), cCols);
        var result = new double[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0) + (i < cRows && j < cCols ? c[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Plus(float[,] a, float[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new float[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Pluses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <param name="c">The input matrix c.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Plus(float[,] a, float[,] b, float[,] c)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var cRows = c.GetLength(0);
        var cCols = c.GetLength(1);
        var rows = Math.Max(Math.Max(aRows, bRows), cRows);
        var cols = Math.Max(Math.Max(aCols, bCols), cCols);
        var result = new float[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) + (i < bRows && j < bCols ? b[i, j] : 0) + (i < cRows && j < cCols ? c[i, j] : 0);
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
    /// Minuses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static int[,] Minus(int[,] a, int[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new int[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) - (i < bRows && j < bCols ? b[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Minuses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static double[,] Minus(double[,] a, double[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new double[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) - (i < bRows && j < bCols ? b[i, j] : 0);
            }
        }

        return result;
    }

    /// <summary>
    /// Minuses matrixes.
    /// </summary>
    /// <param name="a">The input matrix a.</param>
    /// <param name="b">The input matrix b.</param>
    /// <returns>The output matrix.</returns>
    public static float[,] Minus(float[,] a, float[,] b)
    {
        var aRows = a.GetLength(0);
        var aCols = a.GetLength(1);
        var bRows = b.GetLength(0);
        var bCols = b.GetLength(1);
        var rows = Math.Max(aRows, bRows);
        var cols = Math.Max(aCols, bCols);
        var result = new float[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                result[i, j] = (i < aRows && j < aCols ? a[i, j] : 0) - (i < bRows && j < bCols ? b[i, j] : 0);
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
}
