using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing
{
    /// <summary>
    /// Color calculator.
    /// </summary>
    public static partial class ColorCalculator
    {
        /// <summary>
        /// Calculates to get the color with opacity and a given color.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
        /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
        /// <returns>A color with new alpha channel value.</returns>
        public static Color Opacity(Color value, double alpha, bool resetOriginalAlpha = false)
            => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha * 255) : ToChannel(value.A * alpha), value.R, value.G, value.B);

        /// <summary>
        /// Calculates to get the color with opacity and a given color.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="alpha">The alpha channel. Value is from 0 to 1.</param>
        /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
        /// <returns>A color with new alpha channel value.</returns>
        public static Color Opacity(Color value, float alpha, bool resetOriginalAlpha = false)
            => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha * 255) : ToChannel(value.A * alpha), value.R, value.G, value.B);

        /// <summary>
        /// Calculates to get the color with opacity and a given color.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="alpha">The alpha channel. Value is from 0 to 255.</param>
        /// <param name="resetOriginalAlpha">true if use new alpha channel directly instead base the current one; otherwise, false.</param>
        /// <returns>A color with new alpha channel value.</returns>
        public static Color Opacity(Color value, byte alpha, bool resetOriginalAlpha = false)
            => Color.FromArgb(resetOriginalAlpha ? ToChannel(alpha) : ToChannel(alpha / 255d * value.A), value.R, value.G, value.B);

        /// <summary>
        /// Creates a new color by set a channel to a given color.
        /// </summary>
        /// <param name="value">The base color.</param>
        /// <param name="channel">The channel to set.</param>
        /// <param name="newValue">The new value of channel.</param>
        /// <returns>A color with new channel value.</returns>
        public static Color WithChannel(Color value, ColorChannels channel, byte newValue)
            => Color.FromArgb(
                channel.HasFlag((ColorChannels)8) ? newValue : value.A,
                channel.HasFlag(ColorChannels.Red) ? newValue : value.R,
                channel.HasFlag(ColorChannels.Green) ? newValue : value.G,
                channel.HasFlag(ColorChannels.Blue) ? newValue : value.B);

        /// <summary>
        /// Gets the channel value of a specific color.
        /// </summary>
        /// <param name="value">The color to get the channel.</param>
        /// <param name="channel">The channel to get.</param>
        /// <returns>The channel value.</returns>
        public static byte GetChannelValue(Color value, ColorChannels channel)
        {
            var arr = new List<byte>();
            if (channel.HasFlag((ColorChannels)8)) arr.Add(value.A);
            if (channel.HasFlag(ColorChannels.Red)) arr.Add(value.R);
            if (channel.HasFlag(ColorChannels.Green)) arr.Add(value.G);
            if (channel.HasFlag(ColorChannels.Blue)) arr.Add(value.B);
            if (arr.Count == 0) return 127;
            float count = 0;
            foreach (var n in arr)
            {
                count += n;
            }

            return (byte)Math.Round(count / arr.Count);
        }

        /// <summary>
        /// Increases brighness.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
        /// <returns>The color after lighten.</returns>
        public static Color Lighten(Color value, double ratio)
        {
            if (ratio == 0) return value;
            if (ratio > 1) return Color.FromArgb(value.A, 255, 255, 255);
            if (ratio < -1) return Color.FromArgb(value.A, 0, 0, 0);
            var bg = ratio > 0 ? 255 : 0;
            ratio = Math.Abs(ratio);
            return Color.FromArgb(
                value.A,
                ToChannel((bg - value.R) * ratio + value.R),
                ToChannel((bg - value.G) * ratio + value.G),
                ToChannel((bg - value.B) * ratio) + value.B);
        }

        /// <summary>
        /// Increases brighness.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="ratio">The brightness ratio to increase. Value is from -1 to 1.</param>
        /// <returns>The color after lighten.</returns>
        public static Color Lighten(Color value, float ratio)
        {
            if (ratio == 0) return value;
            if (ratio > 1) return Color.FromArgb(value.A, 255, 255, 255);
            if (ratio < -1) return Color.FromArgb(value.A, 0, 0, 0);
            var bg = ratio > 0 ? 255 : 0;
            ratio = Math.Abs(ratio);
            return Color.FromArgb(
                value.A,
                ToChannel((bg - value.R) * ratio + value.R),
                ToChannel((bg - value.G) * ratio + value.G),
                ToChannel((bg - value.B) * ratio) + value.B);
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
        /// <param name="value">The source color value.</param>
        /// <param name="ratio">The brightness ratio to decrease. Value is from -1 to 1.</param>
        /// <returns>The color after darken.</returns>
        public static Color Darken(Color value, float ratio)
            => Lighten(value, -ratio);

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
        /// <param name="value">The source color value collection.</param>
        /// <returns>The color toggled.</returns>
        public static IEnumerable<Color> ToggleBrightness(IEnumerable<Color> value)
            => value?.Select(ele => ToggleBrightness(ele));

        /// <summary>
        /// Reverses RGB.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <returns>The color to reverse.</returns>
        public static Color Reverse(Color value)
            => Color.FromArgb(value.A, 255 - value.R, 255 - value.G, 255 - value.B);

        /// <summary>
        /// Reverses RGB.
        /// </summary>
        /// <param name="value">The source color value collection.</param>
        /// <returns>The color to reverse.</returns>
        public static IEnumerable<Color> Reverse(IEnumerable<Color> value)
            => value?.Select(ele => Reverse(ele));

        /// <summary>
        /// Adds saturate filter.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
        /// <returns>A new color with additional saturation.</returns>
        public static Color Saturate(Color value, double ratio)
        {
            if (double.IsNaN(ratio)) return value;
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
        public static Color Saturate(Color value, float ratio)
        {
            if (float.IsNaN(ratio)) return value;
            var hsl = ToSingleHSL(value);
            var saturate = hsl.Item2 * ratio;
            if (saturate < 0) saturate = 0;
            else if (saturate > 1) saturate = 1;
            return FromHSL(hsl.Item1, saturate, hsl.Item3);
        }

        /// <summary>
        /// Adds saturate filter.
        /// </summary>
        /// <param name="value">The source color value collection.</param>
        /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
        /// <returns>A new color with additional saturation.</returns>
        public static IEnumerable<Color> Saturate(IEnumerable<Color> value, double ratio)
            => value?.Select(ele => Saturate(ele, ratio));

        /// <summary>
        /// Adds saturate filter.
        /// </summary>
        /// <param name="value">The source color value collection.</param>
        /// <param name="ratio">The saturation ratio to change. Value should equal or be greater than 0.</param>
        /// <returns>A new color with additional saturation.</returns>
        public static IEnumerable<Color> Saturate(IEnumerable<Color> value, float ratio)
            => value?.Select(ele => Saturate(ele, ratio));

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
        /// <param name="level">The relative saturation level.</param>
        /// <returns>A new color with additional saturation.</returns>
        public static IEnumerable<Color> Saturate(IEnumerable<Color> value, RelativeSaturationLevels level)
            => value?.Select(ele => Saturate(ele, level));

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
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static Color RotateHue(Color value, byte amount)
            => RotateHue(value, (float)amount);

        /// <summary>
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static Color RotateHue(Color value, int amount)
            => RotateHue(value, (float)amount);

        /// <summary>
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static Color RotateHue(Color value, double amount)
        {
            if (double.IsNaN(amount)) return value;
            if (amount > 360 || amount < 0) amount %= 360;
            if (amount == 0) return value;
            var (h, s, l) = ToHSL(value);
            var hue = h + amount;
            if (hue < 0) hue += 360;
            else if (hue > 360) hue -= 360;
            return FromHSL(hue, s, l);
        }

        /// <summary>
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static Color RotateHue(Color value, float amount)
        {
            if (float.IsNaN(amount)) return value;
            var hsl = ToSingleHSL(value);
            if (amount > 360 || amount < 0) amount %= 360;
            var hue = hsl.Item1 + amount;
            if (hue < 0) hue += 360;
            else if (hue > 360) hue -= 360;
            return FromHSL(hue, hsl.Item2, hsl.Item3);
        }

        /// <summary>
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value collection.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, double amount)
            => value?.Select(ele => RotateHue(ele, amount));

        /// <summary>
        /// Rotates hue.
        /// </summary>
        /// <param name="value">The source color value collection.</param>
        /// <param name="amount">The hue to rotate. Value is from 0 to 360.</param>
        /// <returns>A new color with hue rotation.</returns>
        public static IEnumerable<Color> RotateHue(IEnumerable<Color> value, float amount)
            => value?.Select(ele => RotateHue(ele, amount));

        /// <summary>
        /// Gets a collection.
        /// </summary>
        /// <param name="from">The color from.</param>
        /// <param name="to">The color to.</param>
        /// <param name="count">The count of color to return.</param>
        /// <returns>A collection of console text.</returns>
        public static IEnumerable<Color> LinearGradient(Color from, Color to, int count)
        {
            if (count < 1) yield break;
            if (count == 1)
            {
                yield return Color.FromArgb((from.A + to.A) / 2, (from.R + to.R) / 2, (from.G + to.G) / 2, (from.B + to.B) / 2);
                yield break;
            }

            yield return from;
            var steps = count - 1;
            var deltaA = (to.A - from.A) * 1f / steps;
            var deltaR = (to.R - from.R) * 1f / steps;
            var deltaG = (to.G - from.B) * 1f / steps;
            var deltaB = (to.B - from.B) * 1f / steps;
            float a = from.A;
            float r = from.R;
            float g = from.G;
            float b = from.B;
            for (var i = 1; i < steps; i++)
            {
                var c = Color.FromArgb(
                    PlusChannel(ref a, deltaA),
                    PlusChannel(ref r, deltaR),
                    PlusChannel(ref g, deltaG),
                    PlusChannel(ref b, deltaB));
                yield return c;
            }

            yield return to;
        }

        private static int PlusChannel(ref float c, float delta)
        {
#if NETOLDVER
            var r = (int)Math.Round(c + delta);
#else
            var r = (int)MathF.Round(c + delta);
#endif
            if (r < 0) return 0;
            else if (r > 255) return 255;
            return r;
        }

        private static int ToChannel(float c)
        {
#if NETOLDVER
            var r = (int)Math.Round(c);
#else
            var r = (int)MathF.Round(c);
#endif
            if (r < 0) return 0;
            else if (r > 255) return 255;
            return r;
        }

        private static int ToChannel(double c)
        {
            var r = (int)Math.Round(c);
            if (r < 0) return 0;
            else if (r > 255) return 255;
            return r;
        }
    }
}
