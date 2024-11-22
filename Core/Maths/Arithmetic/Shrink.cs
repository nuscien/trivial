// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Shrink.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The sign and shrink functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths;

/// <summary>
/// The utility for arithmetic.
/// </summary>
public static partial class Arithmetic
{
    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static double Softsign(int input)
        => input / (1d + Math.Abs(input));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static double Softsign(long input)
        => input / (1d + Math.Abs(input));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static float SoftsignF(int input)
        => input / (1f + Math.Abs(input));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static float SoftsignF(long input)
        => input / (1f + Math.Abs(input));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static double Softsign(double input)
        => double.IsNaN(input) ? double.NaN : (input / (1 + Math.Abs(input)));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static float Softsign(float input)
        => float.IsNaN(input) ? float.NaN : (input / (1 + Math.Abs(input)));

    /// <summary>
    /// Gets a floating number from -1 to 1 about sign of specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The result in scale of -1 and 1.</returns>
    public static decimal Softsign(decimal input)
        => input / (1 + Math.Abs(input));

    /// <summary>
    /// Returns an integer that indicates the sign of a specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>1 if input is greater than 0; -1 if input is less than 0; otherwise, 0, if input equals to 0.</returns>
    public static int Hardsign(int input)
    {
        if (input < 0) return -1;
        if (input > 0) return 1;
        return 0;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>1 if input is greater than 0; -1 if input is less than 0; otherwise, 0, if input equals to 0.</returns>
    public static int Hardsign(long input)
    {
        if (input < 0) return -1;
        if (input > 0) return 1;
        return 0;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>1 if input is greater than 0; -1 if input is less than 0; otherwise, 0, if input equals to 0 or is not a number.</returns>
    public static int Hardsign(double input)
    {
        if (double.IsNaN(input)) return 0;
        if (input < 0) return -1;
        if (input > 0) return 1;
        return 0;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>1 if input is greater than 0; -1 if input is less than 0; otherwise, 0, if input equals to 0 or is not a number.</returns>
    public static int Hardsign(float input)
    {
        if (float.IsNaN(input)) return 0;
        if (input < 0) return -1;
        if (input > 0) return 1;
        return 0;
    }

    /// <summary>
    /// Returns an integer that indicates the sign of a specific number.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>1 if input is greater than 0; -1 if input is less than 0; otherwise, 0, if input equals to 0 or is not a number.</returns>
    public static int Hardsign(decimal input)
    {
        if (input < 0) return -1;
        if (input > 0) return 1;
        return 0;
    }

    /// <summary>
    /// Computes by softshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>The result of softshrink.</returns>
    public static int Softshrink(int input, int lambda)
    {
        if (Math.Abs(input) < lambda) return 0;
        if (input > lambda) return input - lambda;
        return input + lambda;
    }

    /// <summary>
    /// Computes by softshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>The result of softshrink.</returns>
    public static long Softshrink(long input, long lambda)
    {
        if (Math.Abs(input) < lambda) return 0L;
        if (input > lambda) return input - lambda;
        return input + lambda;
    }

    /// <summary>
    /// Computes by softshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>The result of softshrink.</returns>
    public static double Softshrink(double input, double lambda)
    {
        if (Math.Abs(input) < lambda) return 0d;
        if (input > lambda) return input - lambda;
        return input + lambda;
    }

    /// <summary>
    /// Computes by softshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>The result of softshrink.</returns>
    public static float Softshrink(float input, float lambda)
    {
        if (Math.Abs(input) < lambda) return 0f;
        if (input > lambda) return input - lambda;
        return input + lambda;
    }

    /// <summary>
    /// Computes by softshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>The result of softshrink.</returns>
    public static decimal Softshrink(decimal input, decimal lambda)
    {
        if (Math.Abs(input) < lambda) return decimal.Zero;
        if (input > lambda) return input - lambda;
        return input + lambda;
    }

    /// <summary>
    /// Computes by hardshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>Zero (0) if the absolute input is less than lambda; otherwise, input returned.</returns>
    public static int Hardshrink(int input, int lambda)
        => Math.Abs(input) < lambda ? 0 : input;

    /// <summary>
    /// Computes by hardshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>Zero (0) if the absolute input is less than lambda; otherwise, input returned.</returns>
    public static long Hardshrink(long input, long lambda)
        => Math.Abs(input) < lambda ? 0L : input;

    /// <summary>
    /// Computes by hardshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>Zero (0) if the absolute input is less than lambda; otherwise, input returned.</returns>
    public static double Hardshrink(double input, double lambda)
        => Math.Abs(input) < lambda ? 0d : input;

    /// <summary>
    /// Computes by hardshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>Zero (0) if the absolute input is less than lambda; otherwise, input returned.</returns>
    public static float Hardshrink(float input, float lambda)
        => Math.Abs(input) < lambda ? 0f : input;

    /// <summary>
    /// Computes by hardshrink (a neural networks activation function).
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <param name="lambda">The lambda used to compare with absolute input.</param>
    /// <returns>Zero (0) if the absolute input is less than lambda; otherwise, input returned.</returns>
    public static decimal Hardshrink(decimal input, decimal lambda)
        => Math.Abs(input) < lambda ? decimal.Zero : input;
}
