using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing;

/// <summary>
/// The relative levels of saturation.
/// </summary>
public enum RelativeSaturationLevels : byte
{
    /// <summary>
    /// No change.
    /// </summary>
    Regular = 0,

    /// <summary>
    /// High saturation.
    /// </summary>
    High = 1,

    /// <summary>
    /// Low saturation.
    /// </summary>
    Low = 2,

    /// <summary>
    /// Adjust saturation to most.
    /// </summary>
    Most = 3,

    /// <summary>
    /// Grayscale.
    /// </summary>
    Gray = 4,

    /// <summary>
    /// Translate and scale to exposure.
    /// </summary>
    Exposure = 5,

    /// <summary>
    /// Translate and scale to shadow.
    /// </summary>
    Shadow = 6,
}

/// <summary>
/// Color calculator.
/// </summary>
public static partial class ColorCalculator
{
    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    public static Color Saturate(Color value, double ratio)
        => double.IsNaN(ratio) ? value : SaturateInternal(value, ratio);

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    public static Color Saturate(Color value, float ratio)
        => float.IsNaN(ratio) ? value : SaturateInternal(value, ratio);

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="level">The relative saturation level.</param>
    /// <returns>A new color with high saturation.</returns>
    public static Color Saturate(Color value, RelativeSaturationLevels level)
    {
        if (value.R == value.G && value.R == value.B) return value;
        var high = Maths.Arithmetic.Max(value.R, value.G, value.B);
        var low = Maths.Arithmetic.Min(value.R, value.G, value.B);
        switch (level)
        {
            case RelativeSaturationLevels.High:
                {
                    var highDistance = (byte)(255 - high);
                    if (highDistance == low) return value;
                    var diff = Math.Min(highDistance, low);
                    var ratio = diff / 255f;
                    return Color.FromArgb(
                        value.A,
                        Saturate(value.R, high, low, diff, ratio),
                        Saturate(value.G, high, low, diff, ratio),
                        Saturate(value.B, high, low, diff, ratio));
                }
            case RelativeSaturationLevels.Most:
                {
                    var diff = Math.Min((byte)(255 - high), low);
                    var ratio = diff / 255f;
                    diff = Math.Max((byte)(255 - high), low);
                    ratio -= ratio * diff / 255f;
                    return Color.FromArgb(
                        value.A,
                        Saturate(value.R, high, low, diff, ratio),
                        Saturate(value.G, high, low, diff, ratio),
                        Saturate(value.B, high, low, diff, ratio));
                }
            case RelativeSaturationLevels.Low:
                {
                    if ((high > 127 && low > 127) || (high < 128 && low < 128)) return value;
                    byte newHigh;
                    byte newLow;
                    float ratio;
                    var highDistance = (byte)(255 - high);
                    if (highDistance == low) return value;
                    if (highDistance < low)
                    {
                        newHigh = (byte)(255 - low);
                        ratio = (high - newHigh) / 255f;
                        newLow = (byte)(127 - (127 - low) * ratio);
                    }
                    else
                    {
                        newLow = highDistance;
                        ratio = (newLow - low) / 255f;
                        newHigh = (byte)(128 + (high - 128) * ratio);
                    }

                    return Color.FromArgb(
                        value.A,
                        Saturate(value.R, high, low, newHigh, newLow, ratio),
                        Saturate(value.G, high, low, newHigh, newLow, ratio),
                        Saturate(value.B, high, low, newHigh, newLow, ratio));
                }
            case RelativeSaturationLevels.Gray:
                return Grayscale(value);
            case RelativeSaturationLevels.Exposure:
                {
                    if (high == 255) return value;
                    var ratio = (255 - low) / (high - low);
                    return Color.FromArgb(
                        value.A,
                        SaturateExposure(value.R, high, low, ratio),
                        SaturateExposure(value.G, high, low, ratio),
                        SaturateExposure(value.B, high, low, ratio));
                }
            case RelativeSaturationLevels.Shadow:
                {
                    if (low == 0) return value;
                    var ratio = high / (high - low);
                    return Color.FromArgb(
                        value.A,
                        SaturateShadow(value.R, high, low, ratio),
                        SaturateShadow(value.G, high, low, ratio),
                        SaturateShadow(value.B, high, low, ratio));
                }
            default:
                return value;
        }
    }

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    public static IEnumerable<Color> Saturate(IEnumerable<Color> value, double ratio)
        => double.IsNaN(ratio) ? value : value?.Select(ele => SaturateInternal(ele, ratio));

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    public static IEnumerable<Color> Saturate(IEnumerable<Color> value, float ratio)
        => float.IsNaN(ratio) ? value : value?.Select(ele => SaturateInternal(ele, ratio));

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="level">The relative saturation level.</param>
    /// <returns>A new color with additional saturation.</returns>
    public static IEnumerable<Color> Saturate(IEnumerable<Color> value, RelativeSaturationLevels level)
        => value?.Select(ele => Saturate(ele, level));

#if NETFRAMEWORK
    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    public static void Saturate(Bitmap value, double ratio)
        => Filter(value, SaturateInternal, ratio);

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    public static void Saturate(Bitmap value, float ratio)
        => Filter(value, SaturateInternal, ratio);

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="level">The relative saturation level.</param>
    public static void Saturate(Bitmap value, RelativeSaturationLevels level)
        => Filter(value, Saturate, level);
#endif

    private static int Saturate(byte channel, byte high, byte low, byte diff, float ratio)
    {
        if (channel == high) return Math.Min(channel + diff, 255);
        if (channel == low) return Math.Max(channel - diff, 0);
        if (channel < 128) return ToChannel(channel - channel * ratio);
        else return ToChannel(channel + (255 - channel) * ratio);
    }

    private static int Saturate(byte channel, byte high, byte low, byte newHigh, byte newLow, float ratio)
    {
        if (channel == high) return newHigh;
        if (channel == low) return newLow;
        if (channel < 128) return ToChannel(127 - (127 - channel) * ratio);
        else return ToChannel(128 + (channel - 128) * ratio);
    }

    private static int SaturateExposure(byte channel, byte high, byte low, float ratio)
    {
        if (channel == high) return 255;
        if (channel == low) return low;
        return ToChannel((channel - low) * ratio + low);
    }

    private static int SaturateShadow(byte channel, byte high, byte low, float ratio)
    {
        if (channel == high) return high;
        if (channel == low) return 0;
        return ToChannel(channel * ratio);
    }

    /// <summary>
    /// Creates a new color by a given one in grayscale.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <returns>A new color with grayscale.</returns>
    public static Color Grayscale(Color value)
    {
        var gray = ToChannel((value.R + value.G + value.B) / 3f);
        return Color.FromArgb(value.A, gray, gray, gray);
    }

    /// <summary>
    /// Creates a new color by a given one with the specific grayscale.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The grayscale ratio to change. Value is from 0 to 1.</param>
    /// <returns>A new color with grayscale.</returns>
    /// <exception cref="ArgumentOutOfRangeException">ratio was less than 0.</exception>
    public static Color Grayscale(Color value, double ratio)
    {
        if (ratio < 0) throw new ArgumentOutOfRangeException(nameof(ratio), "ratio should not be less than 0.");
        if (ratio == 0) return value;
        var gray = (value.R + value.G + value.B) / 3d;
        if (ratio >= 1)
            return Color.FromArgb(
                value.A,
                ToChannel(gray),
                ToChannel(gray),
                ToChannel(gray));
        if (ratio > 1) ratio = 1;
        var grayRatio = 1 - ratio;
        return Color.FromArgb(
            value.A,
            ToChannel(value.R * ratio + gray * grayRatio),
            ToChannel(value.G * ratio + gray * grayRatio),
            ToChannel(value.B * ratio + gray * grayRatio));
    }

    /// <summary>
    /// Creates a new color by a given one with the specific grayscale.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The grayscale ratio to change. Value is from 0 to 1.</param>
    /// <returns>A new color with grayscale.</returns>
    /// <exception cref="ArgumentOutOfRangeException">ratio was less than 0.</exception>
    public static Color Grayscale(Color value, float ratio)
    {
        if (ratio < 0) throw new ArgumentOutOfRangeException(nameof(ratio), "ratio should not be less than 0.");
        if (ratio == 0) return value;
        var gray = (value.R + value.G + value.B) / 3f;
        if (ratio >= 1)
            return Color.FromArgb(
                value.A,
                ToChannel(gray),
                ToChannel(gray),
                ToChannel(gray));
        var grayRatio = 1 - ratio;
        return Color.FromArgb(
            value.A,
            ToChannel(value.R * ratio + gray * grayRatio),
            ToChannel(value.G * ratio + gray * grayRatio),
            ToChannel(value.B * ratio + gray * grayRatio));
    }
    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    private static Color SaturateInternal(Color value, double ratio)
    {
        var (h, s, l) = ToHSL(value);
        var saturate = s * ratio;
        if (saturate < 0) saturate = 0;
        else if (saturate > 1) saturate = 1;
        return FromHSL(h, saturate, l);
    }

    /// <summary>
    /// Adds saturate filter.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
    /// <returns>A new color with additional saturation.</returns>
    private static Color SaturateInternal(Color value, float ratio)
    {
        var hsl = ToSingleHSL(value);
        var saturate = hsl.Item2 * ratio;
        if (saturate < 0) saturate = 0;
        else if (saturate > 1) saturate = 1;
        return FromHSL(hsl.Item1, saturate, hsl.Item3);
    }
}
