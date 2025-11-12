using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing;

/// <summary>
/// The relative levels of brightness.
/// </summary>
public enum RelativeBrightnessLevels : byte
{
    /// <summary>
    /// No change.
    /// </summary>
    Regular = 0,

    /// <summary>
    /// Toggle between light mode and dark mode.
    /// </summary>
    Switch = 1,

    /// <summary>
    /// Translate to high level.
    /// </summary>
    High = 2,

    /// <summary>
    /// Translate to middle level.
    /// </summary>
    Middle = 3,

    /// <summary>
    /// Translate to low level.
    /// </summary>
    Low = 4,

    /// <summary>
    /// Translate to exposure.
    /// </summary>
    Exposure = 5,

    /// <summary>
    /// Translate to shadow.
    /// </summary>
    Shadow = 6,
}

/// <summary>
/// Color calculator.
/// </summary>
public static partial class ColorCalculator
{
    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static Color Lighten(Color value, double ratio)
    {
        if (ratio == 0d || double.IsNaN(ratio)) return value;
        return LightenInternal(value.A, value.R, value.G, value.B, ratio);
    }

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static Color Lighten(byte alpha, byte red, byte green, byte blue, double ratio)
    {
        if (ratio == 0d || double.IsNaN(ratio)) return Color.FromArgb(alpha, red, green, blue);
        return LightenInternal(alpha, red, green, blue, ratio);
    }

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static Color Lighten(Color value, float ratio)
    {
        if (ratio == 0 || float.IsNaN(ratio)) return value;
        return LightenInternal(value.A, value.R, value.G, value.B, ratio);
    }

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static Color Lighten(byte alpha, byte red, byte green, byte blue, float ratio)
    {
        if (ratio == 0 || float.IsNaN(ratio)) return Color.FromArgb(alpha, red, green, blue);
        return LightenInternal(alpha, red, green, blue, ratio);
    }

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static IEnumerable<Color> Lighten(IEnumerable<Color> value, double ratio)
        => value?.Select(ele => Lighten(ele, ratio));

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    public static IEnumerable<Color> Lighten(IEnumerable<Color> value, float ratio)
        => value?.Select(ele => Lighten(ele, ratio));

#if NETFRAMEWORK
    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    public static void Lighten(Bitmap value, double ratio)
        => Filter(value, Lighten, ratio);

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    public static void Lighten(Bitmap value, float ratio)
        => Filter(value, Lighten, ratio);
#endif

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static Color Darken(Color value, double ratio)
        => Lighten(value, -ratio);

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static Color Darken(byte alpha, byte red, byte green, byte blue, double ratio)
        => Lighten(alpha, red, green, blue, -ratio);

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static Color Darken(Color value, float ratio)
        => Lighten(value, -ratio);

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static Color Darken(byte alpha, byte red, byte green, byte blue, float ratio)
        => Lighten(alpha, red, green, blue, -ratio);

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static IEnumerable<Color> Darken(IEnumerable<Color> value, double ratio)
        => value?.Select(ele => Darken(ele, ratio));

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    /// <returns>The color after darken.</returns>
    public static IEnumerable<Color> Darken(IEnumerable<Color> value, float ratio)
        => value?.Select(ele => Darken(ele, ratio));

#if NETFRAMEWORK
    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    public static void Darken(Bitmap value, double ratio)
        => Filter(value, Darken, ratio);

    /// <summary>
    /// Decreases brighness.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
    public static void Darken(Bitmap value, float ratio)
        => Filter(value, Darken, ratio);
#endif

    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <returns>The color toggled.</returns>
    public static Color ToggleBrightness(Color value)
    {
        var delta = 255
            - Maths.Arithmetic.Max(value.R, value.G, value.B)
            - Maths.Arithmetic.Min(value.R, value.G, value.B);
        return Color.FromArgb(value.A, value.R + delta, value.G + delta, value.B + delta);
    }

    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <param name="level">The relative saturation level.</param>
    /// <returns>The color toggled.</returns>
    public static Color ToggleBrightness(Color value, RelativeBrightnessLevels level)
    {
        var max = Maths.Arithmetic.Max(value.R, value.G, value.B);
        var min = Maths.Arithmetic.Min(value.R, value.G, value.B);
        var high = 255 - max;
        var delta = high - min;
        switch (level)
        {
            case RelativeBrightnessLevels.Switch:
                break;
            case RelativeBrightnessLevels.High:
                if (high <= min) return value;
                break;
            case RelativeBrightnessLevels.Low:
                if (high >= min) return value;
                break;
            case RelativeBrightnessLevels.Middle:
                {
                    if (high == min) return value;
                    return Color.FromArgb(
                        value.A,
                        ToChannel(value.R + delta / 2f),
                        ToChannel(value.G + delta / 2f),
                        ToChannel(value.B + delta / 2f));
                }
            case RelativeBrightnessLevels.Exposure:
                if (high == 0) return value;
                delta = high;
                break;
            case RelativeBrightnessLevels.Shadow:
                if (min == 0) return value;
                delta = -min;
                break;
            default:
                return value;
        }

        return Color.FromArgb(value.A, value.R + delta, value.G + delta, value.B + delta);
    }

    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <returns>The color toggled.</returns>
    public static IEnumerable<Color> ToggleBrightness(IEnumerable<Color> value)
        => value?.Select(ele => ToggleBrightness(ele));

    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="level">The relative saturation level.</param>
    /// <returns>The color toggled.</returns>
    public static IEnumerable<Color> ToggleBrightness(IEnumerable<Color> value, RelativeBrightnessLevels level)
        => value?.Select(ele => ToggleBrightness(ele, level));

#if NETFRAMEWORK
    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    public static void ToggleBrightness(Bitmap value)
        => Filter(value, ToggleBrightness);

    /// <summary>
    /// Toggles brightness between light mode and dark mode.
    /// </summary>
    /// <param name="value">The source color value collection.</param>
    /// <param name="level">The relative saturation level.</param>
    public static void ToggleBrightness(Bitmap value, RelativeBrightnessLevels level)
        => Filter(value, ToggleBrightness, level);
#endif

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    private static Color LightenInternal(byte alpha, byte red, byte green, byte blue, double ratio)
    {
        if (ratio > 1) return Color.FromArgb(alpha, 255, 255, 255);
        if (ratio < -1) return Color.FromArgb(alpha, 0, 0, 0);
        var bg = ratio > 0 ? 255 : 0;
        ratio = Math.Abs(ratio);
        return Color.FromArgb(
            alpha,
            ToChannel((bg - red) * ratio + red),
            ToChannel((bg - green) * ratio + green),
            ToChannel((bg - blue) * ratio + blue));
    }

    /// <summary>
    /// Increases brighness.
    /// </summary>
    /// <param name="alpha">The alpha channel of the source color.</param>
    /// <param name="red">The channel red of the source color.</param>
    /// <param name="green">The channel green of the source color.</param>
    /// <param name="blue">The channel blue of the source color.</param>
    /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
    /// <returns>The color after lighten.</returns>
    private static Color LightenInternal(byte alpha, byte red, byte green, byte blue, float ratio)
    {
        if (ratio > 1) return Color.FromArgb(alpha, 255, 255, 255);
        if (ratio < -1) return Color.FromArgb(alpha, 0, 0, 0);
        var bg = ratio > 0 ? 255 : 0;
        ratio = Math.Abs(ratio);
        return Color.FromArgb(
            alpha,
            ToChannel((bg - red) * ratio + red),
            ToChannel((bg - green) * ratio + green),
            ToChannel((bg - blue) * ratio + blue));
    }
}
