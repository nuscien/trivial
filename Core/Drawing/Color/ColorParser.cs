using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Trivial.Maths;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Drawing;

/// <summary>
/// Color calculator.
/// </summary>
public static partial class ColorCalculator
{
#pragma warning disable IDE0057, CA1846

    #region Parse

    /// <summary>
    /// Tries to parse a color.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public static bool TryParse(string s, out Color result)
    {
        s = s?.Trim();
        if (string.IsNullOrEmpty(s))
        {
            result = Color.Transparent;
            return false;
        }

        if (s.StartsWith('#'))
            return TryParseHexColor(s.Substring(1), out result);
        var pos = s.IndexOf('(');
        if (pos >= 0)
        {
            var type = s.Substring(0, pos).Trim().ToLowerInvariant();
            var value = s.Substring(pos + 1).Trim();
            pos = value.IndexOf(')');
            if (pos >= 0) value = value.Substring(0, pos).Trim();
            var isError = false;
            var arr = (value.Contains(',')
                ? value.Split(new[] { ',' })
                : value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Select(ele =>
            {
                var item = ele?.Trim();
                if (string.IsNullOrEmpty(item)) return 0;
                if (float.TryParse(item, out var r)) return r;
                if (item.Length > 1 && float.TryParse(item.Substring(0, item.Length - 1), out r))
                {
                    if (item.EndsWith('%')) return r * 0.01f;
                    if (item.EndsWith('‰')) return r * 0.001f;
                    if (item.EndsWith('‱')) return r * 0.0001f;
                    if (item.EndsWith('°')) return r;
                }

                if (item.Contains('x') && Numbers.TryParseToInt32(item, 16, out var i))
                    return i;
                if (item.EndsWith("deg") && float.TryParse(item.Substring(0, item.Length - 3).Trim(), out r))
                    return r;
                if (item.EndsWith("degree") && float.TryParse(item.Substring(0, item.Length - 6).Trim(), out r))
                    return r;
                isError = true;
                return 0;
            }).ToList();
            if (isError)
            {
                result = default;
                return false;
            }

            if (string.IsNullOrEmpty(type)) type = "rgba";
            try
            {
                switch (type)
                {
                    case "rgba":
                    case "rgb":
                        if (arr.Count == 4)
                        {
                            result = Color.FromArgb(ToChannel(arr[3] * 255), ToChannel(arr[0]), ToChannel(arr[1]), ToChannel(arr[2]));
                            return true;
                        }

                        if (arr.Count == 3)
                        {
                            result = Color.FromArgb(ToChannel(arr[0]), ToChannel(arr[1]), ToChannel(arr[2]));
                            return true;
                        }

                        break;
                    case "argb":
                        if (arr.Count == 4)
                        {
                            result = Color.FromArgb(ToChannel(arr[0] * 255), ToChannel(arr[1]), ToChannel(arr[2]), ToChannel(arr[3]));
                            return true;
                        }

                        if (arr.Count == 3)
                        {
                            result = Color.FromArgb(ToChannel(arr[0]), ToChannel(arr[1]), ToChannel(arr[2]));
                            return true;
                        }

                        break;
                    case "hsl":
                    case "hsla":
                        if (arr.Count < 3)
                            break;
                        if (arr[0] < 0 || arr[0] > 360) break;
                        if (arr.Count == 4)
                        {
                            result = FromHSL(arr[0], arr[1], arr[2], arr[3]);
                            return true;
                        }

                        if (arr.Count == 3)
                        {
                            result = FromHSL(arr[0], arr[1], arr[2]);
                            return true;
                        }

                        break;
                    case "cmyk":
                    case "cmyka":
                        if (arr.Count < 4)
                            break;
                        if (arr.Count == 5)
                        {
                            result = FromCMYK(arr[0], arr[1], arr[2], arr[3], arr[4]);
                            return true;
                        }

                        if (arr.Count == 4)
                        {
                            result = FromCMYK(arr[0], arr[1], arr[2], arr[3]);
                            return true;
                        }

                        break;
                }
            }
            catch (ArgumentException)
            {
                result = default;
                return false;
            }

            result = default;
            return false;
        }

        if (Enum.TryParse<KnownColor>(s, true, out var kc))
        {
            result = Color.FromKnownColor(kc);
            return true;
        }

        return TryParseHexColor(s, out result);
    }

    /// <summary>
    /// Parses a color.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <returns>The result color parsed.</returns>
    /// <exception cref="FormatException">s was incorrect to parse as a color.</exception>
    public static Color Parse(string s)
    {
        s = s.Trim();
        if (string.IsNullOrEmpty(s)) return Color.Transparent;
        if (TryParse(s, out var r)) return r;
        throw new FormatException("s is not a color.", new ArgumentException("s is not supported.", nameof(s)));
    }

    /// <summary>
    /// Converts a color to hex format string.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <returns>A hex format string.</returns>
    public static string ToHexString(Color value)
        => value.A == 255 ? $"#{value.R:x2}{value.G:x2}{value.B:x2}" : $"#{value.A:x2}{value.R:x2}{value.G:x2}{value.B:x2}";

    /// <summary>
    /// Converts a color to hex format string.
    /// </summary>
    /// <param name="value">The source color value.</param>
    /// <returns>A hex format string.</returns>
    public static string ToRgbaString(Color value)
        => $"rgba({value.R}, {value.G}, {value.B}, {value.A / 255d:0.######})";

    /// <summary>
    /// Parses the color from a JSON token.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The color read from JSON token.</returns>
    /// <exception cref="JsonException">The token type is unexpected or the value was invalid.</exception>
    internal static Color ParseValue(ref Utf8JsonReader reader)
    {
        try
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                case JsonTokenType.False:
                    return default;
                case JsonTokenType.Number:
                    return Parse("#" + reader.GetInt32().ToString("x8"));
                case JsonTokenType.String:
                    return Parse(reader.GetString());
                case JsonTokenType.StartObject:
                    var json = JsonObjectNode.ParseValue(ref reader);
                    if (json == null) break;
                    var a = TryGetSingleFromMulti(json, "a", "A", "alpha", "Alpha", "ALPHA", 1);
                    var r = json.TryGetInt32Value("r") ?? json.TryGetInt32Value("R") ?? json.TryGetInt32Value("red") ?? json.TryGetInt32Value("Red") ?? json.TryGetInt32Value("RED");
                    var g = json.TryGetInt32Value("g") ?? json.TryGetInt32Value("G") ?? json.TryGetInt32Value("green") ?? json.TryGetInt32Value("Green") ?? json.TryGetInt32Value("GREEN");
                    var b = json.TryGetInt32Value("b") ?? json.TryGetInt32Value("B") ?? json.TryGetInt32Value("blue") ?? json.TryGetInt32Value("Blue") ?? json.TryGetInt32Value("BLUE");
                    if (r.HasValue || g.HasValue || b.HasValue)
                        return Color.FromArgb(ToChannel(a * 255), r ?? 0, g ?? 0, b ?? 0);
                    var h = json.TryGetInt32Value("h") ?? json.TryGetInt32Value("H") ?? json.TryGetInt32Value("hue") ?? json.TryGetInt32Value("Hue") ?? json.TryGetInt32Value("HUE");
                    if (h.HasValue)
                    {
                        var s = TryGetSingleFromMulti(json, "s", "S", "saturation", "Saturation", "SATURATION", 0);
                        var l = TryGetSingleFromMulti(json, "l", "L", "lightness", "Lightness", "LIGHTNESS", 0);
                        return FromHSL(h.Value, s, l, a);
                    }

                    var c = json.TryGetInt32Value("c") ?? json.TryGetInt32Value("C") ?? json.TryGetInt32Value("cyan") ?? json.TryGetInt32Value("Cyan") ?? json.TryGetInt32Value("CYAN");
                    var m = json.TryGetInt32Value("m") ?? json.TryGetInt32Value("M") ?? json.TryGetInt32Value("magenta") ?? json.TryGetInt32Value("Magenta") ?? json.TryGetInt32Value("MAGENTA");
                    var y = json.TryGetInt32Value("y") ?? json.TryGetInt32Value("Y") ?? json.TryGetInt32Value("yellow") ?? json.TryGetInt32Value("Yellow") ?? json.TryGetInt32Value("YELLOW");
                    var k = json.TryGetInt32Value("k") ?? json.TryGetInt32Value("K") ?? json.TryGetInt32Value("black") ?? json.TryGetInt32Value("Black") ?? json.TryGetInt32Value("BLACK");
                    if (c.HasValue && m.HasValue && y.HasValue && k.HasValue)
                        return FromCMYK(c.Value, m.Value, y.Value, k.Value, a);
                    break;
                case JsonTokenType.StartArray:
                    var arr = JsonArrayNode.ParseValue(ref reader);
                    if (arr == null) break;
                    if (arr.Count < 3 || arr.Count > 4) break;
                    var i = 0;
                    var alpha = 255;
                    if (arr.Count == 4)
                    {
                        i = 1;
                        alpha = arr.TryGetInt32Value(0) ?? 255;
                    }

                    var item0 = arr.TryGetInt32Value(i);
                    var item1 = arr.TryGetInt32Value(i + 1);
                    var item2 = arr.TryGetInt32Value(i + 2);
                    if (!item0.HasValue || !item1.HasValue || !item2.HasValue) break;
                    return Color.FromArgb(alpha, item0.Value, item1.Value, item2.Value);
            }
        }
        catch (ArgumentException)
        {
        }
        catch (FormatException ex)
        {
            throw new JsonException($"Cannot parse the value.", ex);
        }

        throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or a color format string.");
    }

    #endregion
    #region HSL

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(double hue, double saturation, double lightness)
        => FromHSL(hue, saturation, lightness, 255);

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(double hue, double saturation, double lightness, double alpha)
        => FromHSL(hue, saturation, lightness, (byte)ToChannel(alpha * 255));

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 255.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(double hue, double saturation, double lightness, byte alpha)
    {
        double i, j;
        if (saturation == 0)
        {
            var channel = ToChannel(lightness * 255d);
            return Color.FromArgb(alpha, channel, channel, channel);
        }

        if (lightness < 0.5) j = lightness * (1 + saturation);
        else j = lightness + saturation - (saturation * lightness);
        i = 2d * lightness - j;
        var h = hue / 360d;
        var aThird = 1d / 3d;
        return Color.FromArgb(
            alpha,
            ToChannel(255d * FromHue(i, j, h + aThird)),
            ToChannel(255d * FromHue(i, j, h)),
            ToChannel(255d * FromHue(i, j, h - aThird)));
    }

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(float hue, float saturation, float lightness)
        => FromHSL(hue, saturation, lightness, 255);

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(float hue, float saturation, float lightness, float alpha)
        => FromHSL(hue, saturation, lightness, (byte)ToChannel(alpha * 255));

    /// <summary>
    /// Creates color from HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="hue">The hue. Value is from 0 to 360.</param>
    /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
    /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 255.</param>
    /// <returns>A color.</returns>
    public static Color FromHSL(float hue, float saturation, float lightness, byte alpha)
    {
        float i, j;
        if (saturation == 0)
        {
            var channel = ToChannel(lightness * 255f);
            return Color.FromArgb(alpha, channel, channel, channel);
        }

        if (lightness < 0.5) j = lightness * (1 + saturation);
        else j = lightness + saturation - (saturation * lightness);
        i = 2f * lightness - j;
        var h = hue / 360f;
        var aThird = 1f / 3f;
        return Color.FromArgb(
            alpha,
            ToChannel(255f * FromHue(i, j, h + aThird)),
            ToChannel(255f * FromHue(i, j, h)),
            ToChannel(255f * FromHue(i, j, h - aThird)));
    }

    private static double FromHue(double v1, double v2, double vH)
    {
        if (vH < 0) vH += 1;
        if (vH > 1) vH -= 1;
        if (6 * vH < 1) return v1 + (v2 - v1) * 6 * vH;
        if (2 * vH < 1) return v2;
        if (3 * vH < 2) return v1 + (v2 - v1) * ((2d / 3) - vH) * 6;
        return v1;
    }

    private static float FromHue(float v1, float v2, float vH)
    {
        if (vH < 0) vH += 1;
        if (vH > 1) vH -= 1;
        if (6 * vH < 1) return v1 + (v2 - v1) * 6 * vH;
        if (2 * vH < 1) return v2;
        if (3 * vH < 2) return v1 + (v2 - v1) * ((2f / 3) - vH) * 6;
        return v1;
    }

    /// <summary>
    /// Converts a color to HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="value">The color to get HSL values.</param>
    /// <returns>The HSL tuple: hue [0°..360°], saturation [0..1] and lightness [0..1].</returns>
    public static (double h, double s, double l) ToHSL(Color value)
    {
        var min = Arithmetic.Min(value.R, value.G, value.B) / 255d;
        var max = Arithmetic.Max(value.R, value.G, value.B) / 255d;
        var lightness = (max + min) / 2d;
        if (lightness == 0d || min == max)
            return (value.GetHue(), 0d, lightness);
        if (lightness > 0d && lightness <= 0.5d)
            return (value.GetHue(), (max - min) / (max + min), lightness);
        return (value.GetHue(), (max - min) / (2d - (max + min)), lightness);
    }

    /// <summary>
    /// Converts a color to HSL (hue-saturation-lightness).
    /// </summary>
    /// <param name="value">The color to get HSL values.</param>
    /// <returns>The HSL tuple: hue [0°..360°], saturation [0..1] and lightness [0..1].</returns>
    private static (float, float, float) ToSingleHSL(Color value)
    {
        var min = Arithmetic.Min(value.R, value.G, value.B) / 255f;
        var max = Arithmetic.Max(value.R, value.G, value.B) / 255f;
        var lightness = (max + min) / 2f;
        if (lightness == 0f || min == max)
            return (value.GetHue(), 0f, lightness);
        if (lightness > 0f && lightness <= 0.5f)
            return (value.GetHue(), (max - min) / (max + min), lightness);
        return (value.GetHue(), (max - min) / (2f - (max + min)), lightness);
    }

    #endregion
    #region HSV & HSI & HSB

    /// <summary>
    /// Converts a color to HSV (hue-saturation-value).
    /// </summary>
    /// <param name="value">The color to get HSV values.</param>
    /// <returns>The HSV tuple: hue [0°..360°], saturation [0..1] and value [0..1].</returns>
    public static (double h, double s, double v) ToHSV(Color value)
    {
        var min = Arithmetic.Min(value.R, value.G, value.B) / 255d;
        var max = Arithmetic.Max(value.R, value.G, value.B) / 255d;
        return (value.GetHue(), max == 0d ? 0d : (max - min) / max, max);
    }

    /// <summary>
    /// Converts a color to HSI (hue-saturation-intensity).
    /// </summary>
    /// <param name="value">The color to get HSI values.</param>
    /// <returns>The HSI tuple: hue [0°..360°], saturation [0..1] and value [0..1].</returns>
    public static (double h, double s, double i) ToHSI(Color value)
    {
        if (value.R == 0 && value.G == 0 && value.B == 0)
            return (0d, 0d, 0d);
        var intensity = (value.R / 255d + value.G / 255d + value.B / 255d) / 3d;
        var min = Arithmetic.Min(value.R, value.G, value.B) / 255d;
        return (value.GetHue(), 1d - (min / intensity), intensity);
    }

    /// <summary>
    /// Converts a color to HSI (hue-saturation-brightness).
    /// </summary>
    /// <param name="value">The color to get HSB values.</param>
    /// <returns>The HSI tuple: hue [0°..360°], saturation [0..1] and value [0..1].</returns>
    public static (double h, double s, double b) ToHSB(Color value)
        => (value.GetHue(), value.GetSaturation(), value.GetBrightness());

    #endregion
    #region CMYK

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(double cyan, double magenta, double yellow, double black)
        => FromCMYK(cyan, magenta, yellow, black, 255);

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(double cyan, double magenta, double yellow, double black, double alpha)
        => FromCMYK(cyan, magenta, yellow, black, (byte)ToChannel(alpha * 255));

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 255.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(double cyan, double magenta, double yellow, double black, byte alpha)
        => Color.FromArgb(
            alpha,
            ToChannel(255 * (1 - cyan) * (1 - black)),
            ToChannel(255 * (1 - magenta) * (1 - black)),
            ToChannel(255 * (1 - yellow) * (1 - black)));

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(float cyan, float magenta, float yellow, float black)
        => FromCMYK(cyan, magenta, yellow, black, 255);

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 1.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(float cyan, float magenta, float yellow, float black, float alpha)
        => FromCMYK(cyan, magenta, yellow, black, (byte)ToChannel(alpha * 255));

    /// <summary>
    /// Creates color from CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
    /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
    /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
    /// <param name="black">The black. Value is from 0 to 1.</param>
    /// <param name="alpha">The alpha. Value is from 0 to 255.</param>
    /// <returns>A color.</returns>
    public static Color FromCMYK(float cyan, float magenta, float yellow, float black, byte alpha)
        => Color.FromArgb(
            alpha,
            ToChannel(255 * (1 - cyan) * (1 - black)),
            ToChannel(255 * (1 - magenta) * (1 - black)),
            ToChannel(255 * (1 - yellow) * (1 - black)));

    /// <summary>
    /// Converts a color to CMYK (cyan-magenta-yellow-black).
    /// </summary>
    /// <param name="value">The color to get CMYK values.</param>
    /// <returns>The CMYK tuple: cyan[0..1], magenta[0..1], yellow[0..1] and black key[0..1].</returns>
    public static (double c, double m, double y, double k) ToCMYK(Color value)
    {
        if (value.R == 0 && value.G == 0 && value.B == 0)
            return (0d, 0d, 0d, 1d);
        var red = value.R / 255d;
        var green = value.G / 255d;
        var blue = value.B / 255d;
        var black = 1d - Arithmetic.Max(red, green, blue);
        if (1d - black == 0d)
            return (0d, 0d, 0d, 1d);
        return ((1d - red - black) / (1d - black), (1d - green - black) / (1d - black), (1d - blue - black) / (1d - black), black);
    }

    #endregion
    #region LAB & LXY

    /// <summary>
    /// Convert a color to a CIE XYZ color.
    /// </summary>
    /// <param name="value">The color to get XYZ values.</param>
    /// <returns>The CIE XYZ tuple: x [0..1], y [0..1] and z [0..1].</returns>
    public static (double x, double y, double z) ToCIEXYZ(Color value)
    {
        var red = value.R / 255d;
        var green = value.G / 255d;
        var blue = value.B / 255d;
        var redLinear = (red > 0.04045) ? Math.Pow((red + 0.055) / 1.055, 2.4) : (red / 12.92);
        var greenLinear = (green > 0.04045) ? Math.Pow((green + 0.055) / 1.055, 2.4) : (green / 12.92);
        var blueLinear = (blue > 0.04045) ? Math.Pow((blue + 0.055) / 1.055, 2.4) : (blue / 12.92);
        return (
            (redLinear * 0.4124) + (greenLinear * 0.3576) + (blueLinear * 0.1805),
            (redLinear * 0.2126) + (greenLinear * 0.7152) + (blueLinear * 0.0722),
            (redLinear * 0.0193) + (greenLinear * 0.1192) + (blueLinear * 0.9505));
    }

    /// <summary>
    /// Convert a color to a CIE LAB (lightness and 2 chromaticities) color.
    /// </summary>
    /// <param name="value">The color to get CMYK values.</param>
    /// <returns>The CIE LAB tuple: lightness [0..100] and 2 chromaticities (a and b) [-128..127].</returns>
    public static (double l, double a, double b) ToCIELAB(Color value)
    {
        var xyz = ToCIEXYZ(value);
        var x = xyz.Item1 * 100 / 95.0489;
        var y = xyz.Item2 * 100 / 100;
        var z = xyz.Item3 * 100 / 108.8840;
        var delta = 6d / 29;
        var m = 1d / 3 * Math.Pow(delta, -2);
        var t = Math.Pow(delta, 3);
        var fa = 16d / 116;
        var fx = (x > t) ? Math.Pow(x, 1d / 3d) : (x * m) + fa;
        var fy = (y > t) ? Math.Pow(y, 1d / 3d) : (y * m) + fa;
        var fz = (z > t) ? Math.Pow(z, 1d / 3d) : (z * m) + fa;
        return ((116 * fy) - 16, 500 * (fx - fy), 200 * (fy - fz));
    }

    #endregion
    #region Helpers

    private static float TryGetSingleFromMulti(JsonObjectNode node, string keyA, string keyB, string keyC, string keyD, string keyE, float defaultValue)
    {
        var v = node.TryGetSingleValue(keyA, false);
        if (!float.IsNaN(v)) return v;
        v = node.TryGetSingleValue(keyB, false);
        if (!float.IsNaN(v)) return v;
        v = node.TryGetSingleValue(keyC, false);
        if (!float.IsNaN(v)) return v;
        v = node.TryGetSingleValue(keyD, false);
        if (!float.IsNaN(v)) return v;
        v = node.TryGetSingleValue(keyE, false);
        if (!float.IsNaN(v)) return v;
        return defaultValue;
    }

    private static bool TryParseHexColor(string s, out Color result)
    {
        if (s.Length == 6)
        {
            var r = GetDigit(s, 0) * 16 + GetDigit(s, 1);
            var g = GetDigit(s, 2) * 16 + GetDigit(s, 3);
            var b = GetDigit(s, 4) * 16 + GetDigit(s, 5);
            if (r < 0 || g < 0 || b < 0)
            {
                result = default;
                return false;
            }

            result = Color.FromArgb(r, g, b);
            return true;
        }

        if (s.Length == 8)
        {
            var a = GetDigit(s, 0) * 16 + GetDigit(s, 1);
            var r = GetDigit(s, 2) * 16 + GetDigit(s, 3);
            var g = GetDigit(s, 4) * 16 + GetDigit(s, 5);
            var b = GetDigit(s, 6) * 16 + GetDigit(s, 7);
            if (a < 0 || r < 0 || g < 0 || b < 0)
            {
                result = default;
                return false;
            }

            result = Color.FromArgb(a, r, g, b);
            return true;
        }

        if (s.Length == 3)
        {
            var r = GetDigit(s, 0);
            var g = GetDigit(s, 1);
            var b = GetDigit(s, 2);
            if (r < 0 || g < 0 || b < 0)
            {
                result = default;
                return false;
            }

            result = Color.FromArgb(r * 16 + r, g * 16 + g, b * 16 + b);
            return true;
        }

        result = default;
        return false;
    }

    private static int GetDigit(string s, int position)
        => s[position] switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'A' or 'a' => 10,
            'B' or 'b' => 11,
            'C' or 'c' => 12,
            'D' or 'd' => 13,
            'E' or 'e' => 14,
            'F' or 'f' => 15,
            _ => -1
        };
    
    #endregion
#pragma warning restore IDE0057, CA1846
}
