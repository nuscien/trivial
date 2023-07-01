using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The result of simple linear regression.
/// </summary>
public struct SimpleLinearRegressionResult : IEquatable<SimpleLinearRegressionResult>, IEquatable<SimpleLinearRegressionResultF>
{
    /// <summary>
    /// Initializes a new instance of the SimpleLinearRegressionResult class.
    /// </summary>
    public SimpleLinearRegressionResult()
    {
        Slope = 0;
        Intercept = 0;
    }

    /// <summary>
    /// Initializes a new instance of the SimpleLinearRegressionResult class.
    /// </summary>
    /// <param name="intercept">The intercept.</param>
    /// <param name="slope">The slope.</param>
    [JsonConstructor]
    public SimpleLinearRegressionResult(double intercept, double slope)
    {
        Intercept = intercept;
        Slope = double.IsNaN(slope) ? 0 : slope;
    }

    /// <summary>
    /// Gets the intercept (beta 0).
    /// </summary>
    [JsonPropertyName("intercept")]
    public double Intercept { get; }

    /// <summary>
    /// Gets the slope (beta 1).
    /// </summary>
    [JsonPropertyName("slope")]
    public double Slope { get; }

    /// <summary>
    /// Gets a value indicating whether the result is not valid.
    /// </summary>
    [JsonIgnore]
    public bool IsInvalid => Slope == 0 || double.IsNaN(Intercept);

    /// <summary>
    /// Gets the value on X-axis.
    /// </summary>
    /// <param name="y">The value on Y-axis.</param>
    /// <returns>The value on X-axis.</returns>
    public double GetX(double y)
        => Slope == 0 ? double.NaN : ((y - Intercept) / Slope);

    /// <summary>
    /// Gets the value on Y-axis.
    /// </summary>
    /// <param name="x">The value on X-axis.</param>
    /// <returns>The value on Y-axis.</returns>
    public double GetY(double x)
        => Slope == 0 ? double.NaN : (Intercept + Slope * x);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
        => $"y = {Intercept} + {Slope} x".GetHashCode();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(SimpleLinearRegressionResult other)
        => Intercept == other.Intercept && Slope == other.Slope;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(SimpleLinearRegressionResultF other)
        => Intercept == other.Intercept && Slope == other.Slope;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is SimpleLinearRegressionResult r) return Equals(r);
        if (other is SimpleLinearRegressionResultF f) return Equals(f);
        return false;
    }

    /// <summary>
    /// Compares two multiple elements to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(SimpleLinearRegressionResult leftValue, SimpleLinearRegressionResult rightValue)
        => leftValue.Equals(rightValue);

    /// <summary>
    /// Compares two multiple elements to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(SimpleLinearRegressionResult leftValue, SimpleLinearRegressionResult rightValue)
        => !leftValue.Equals(rightValue);

    /// <summary>
    /// Gets the simple linear regression.
    /// </summary>
    /// <param name="values">The input data.</param>
    /// <returns>The result of simple linear regression.</returns>
    public static SimpleLinearRegressionResult From(IEnumerable<DoublePoint2D> values)
    {
        if (values == null) return new();
        var list = values.Where(ele => ele != null).ToList();
        var meanX = list.Average(ele => ele.X);
        var meanY = list.Average(ele => ele.Y);
        var numerators = new List<double>();
        var denominators = new List<double>();
        foreach (var item in list)
        {
            var deltaX = item.X - meanX;
            numerators.Add(deltaX * (item.Y - meanY));
            denominators.Add(deltaX * deltaX);
        }

        var denominator = denominators.Average();
        if (denominator == 0) return new();
        var slope = numerators.Average() / denominator;
        return new(meanY - slope * meanX, slope);
    }

    /// <summary>
    /// Gets the simple linear regression.
    /// </summary>
    /// <param name="values">The input data.</param>
    /// <returns>The result of simple linear regression.</returns>
    public static SimpleLinearRegressionResult From(IEnumerable<IntPoint2D> values)
    {
        if (values == null) return new();
        var list = values.Where(ele => ele != null).ToList();
        var meanX = list.Average(ele => ele.X);
        var meanY = list.Average(ele => ele.Y);
        var numerators = new List<double>();
        var denominators = new List<double>();
        foreach (var item in list)
        {
            var deltaX = item.X - meanX;
            numerators.Add(deltaX * (item.Y - meanY));
            denominators.Add(deltaX * deltaX);
        }

        var denominator = denominators.Average();
        if (denominator == 0) return new();
        var slope = numerators.Average() / denominator;
        return new(meanY - slope * meanX, slope);
    }
}

/// <summary>
/// The result of simple linear regression.
/// </summary>
public class SimpleLinearRegressionResultF : IEquatable<SimpleLinearRegressionResultF>, IEquatable<SimpleLinearRegressionResult>
{
    /// <summary>
    /// Initializes a new instance of the SimpleLinearRegressionResultF class.
    /// </summary>
    public SimpleLinearRegressionResultF()
    {
        Slope = 0;
        Intercept = 0;
    }

    /// <summary>
    /// Initializes a new instance of the SimpleLinearRegressionResultF class.
    /// </summary>
    /// <param name="slope">The slope.</param>
    /// <param name="intercept">The intercept.</param>
    [JsonConstructor]
    public SimpleLinearRegressionResultF(float slope, float intercept)
    {
        Slope = float.IsNaN(slope) ? 0 : slope;
        Intercept = intercept;
    }

    /// <summary>
    /// Gets the slope (beta 1).
    /// </summary>
    [JsonPropertyName("slope")]
    public float Slope { get; }

    /// <summary>
    /// Gets the intercept (beta 0).
    /// </summary>
    [JsonPropertyName("intercept")]
    public float Intercept { get; }

    /// <summary>
    /// Gets a value indicating whether the result is not valid.
    /// </summary>
    [JsonIgnore]
    public bool IsInvalid => Slope == 0 || float.IsNaN(Intercept);

    /// <summary>
    /// Gets the value on X-axis.
    /// </summary>
    /// <param name="y">The value on Y-axis.</param>
    /// <returns>The value on X-axis.</returns>
    public float GetX(float y)
        => Slope == 0 ? float.NaN : ((y - Intercept) / Slope);

    /// <summary>
    /// Gets the value on Y-axis.
    /// </summary>
    /// <param name="x">The value on X-axis.</param>
    /// <returns>The value on Y-axis.</returns>
    public float GetY(float x)
        => Slope == 0 ? float.NaN : (Intercept + Slope * x);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
        => $"y = {Intercept} + {Slope} x".GetHashCode();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(SimpleLinearRegressionResult other)
        => Intercept == other.Intercept && Slope == other.Slope;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(SimpleLinearRegressionResultF other)
        => Intercept == other.Intercept && Slope == other.Slope;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is SimpleLinearRegressionResultF r) return Equals(r);
        if (other is SimpleLinearRegressionResult d) return Equals(d);
        return false;
    }

    /// <summary>
    /// Compares two multiple elements to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(SimpleLinearRegressionResultF leftValue, SimpleLinearRegressionResultF rightValue)
        => leftValue.Equals(rightValue);

    /// <summary>
    /// Compares two multiple elements to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(SimpleLinearRegressionResultF leftValue, SimpleLinearRegressionResultF rightValue)
        => !leftValue.Equals(rightValue);

    /// <summary>
    /// Gets the simple linear regression.
    /// </summary>
    /// <param name="values">The input data.</param>
    /// <returns>The result of simple linear regression.</returns>
    public static SimpleLinearRegressionResultF SimpleLinearRegression(IEnumerable<System.Numerics.Vector2> values)
    {
        if (values == null) return new();
        var list = values as IList<System.Numerics.Vector2> ?? values.ToList();
        var meanX = list.Average(ele => ele.X);
        var meanY = list.Average(ele => ele.Y);
        var numerators = new List<float>();
        var denominators = new List<float>();
        foreach (var item in list)
        {
            var deltaX = item.X - meanX;
            numerators.Add(deltaX * (item.Y - meanY));
            denominators.Add(deltaX * deltaX);
        }

        var denominator = denominators.Average();
        if (denominator == 0) return new();
        var slope = numerators.Average() / denominator;
        return new(meanY - slope * meanX, slope);
    }
}
