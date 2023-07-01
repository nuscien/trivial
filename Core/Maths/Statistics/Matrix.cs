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
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[,] result = new double[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
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
        int A_rows = a.GetLength(0);
        int A_cols = a.GetLength(1);
        int B_rows = b.GetLength(0);
        int B_cols = b.GetLength(1);

        if (A_cols != B_rows)
        {
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        }

        double[,] result = new double[A_rows, B_cols];

        for (int i = 0; i < A_rows; i++)
        {
            for (int j = 0; j < B_cols; j++)
            {
                for (int k = 0; k < A_cols; k++)
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
        int A_rows = a.GetLength(0);
        int A_cols = a.GetLength(1);
        int B_len = b.Length;

        if (A_cols != B_len)
        {
            throw new ArgumentException("Matrix and vector dimensions do not match for multiplication.");
        }

        double[] result = new double[A_rows];

        for (int i = 0; i < A_rows; i++)
        {
            for (int j = 0; j < A_cols; j++)
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
        int n = matrix.GetLength(0);
        if (n != matrix.GetLength(1))
        {
            throw new ArgumentException("Matrix must be square.");
        }

        double[,] result = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            result[i, i] = 1;
        }

        for (int i = 0; i < n; i++)
        {
            double diag = matrix[i, i];
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] /= diag;
                result[i, j] /= diag;
            }

            for (int k = 0; k < n; k++)
            {
                if (k == i) continue;

                double factor = matrix[k, i];
                for (int j = 0; j < n; j++)
                {
                    matrix[k, j] -= factor * matrix[i, j];
                    result[k, j] -= factor * result[i, j];
                }
            }
        }

        return result;
    }
}
