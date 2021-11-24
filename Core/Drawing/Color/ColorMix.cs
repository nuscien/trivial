using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Drawing
{
    /// <summary>
    /// The types to mix colors.
    /// </summary>
    public enum ColorMixTypes
    {
        /// <summary>
        /// Average (mean).
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Merge each channel by the maximum.
        /// </summary>
        Lighten = 1,

        /// <summary>
        /// Merge each channel by the minimum.
        /// </summary>
        Darken = 2,

        /// <summary>
        /// Color linear dodge.
        /// </summary>
        Weaken = 3,

        /// <summary>
        /// Color linear burn.
        /// </summary>
        Deepen = 4,

        /// <summary>
        /// Add each channel.
        /// </summary>
        Accent = 5,

        /// <summary>
        /// Diff each channel.
        /// </summary>
        Diff = 6,
    }

    /// <summary>
    /// Color calculator.
    /// </summary>
    public static partial class ColorCalculator
    {
        /// <summary>
        /// Adds a color on another one.
        /// </summary>
        /// <param name="top">The color on top.</param>
        /// <param name="bottom">The color on bottom.</param>
        /// <returns>A new color.</returns>
        public static Color Overlay(Color top, Color bottom)
        {
            if (top.A == 0) return bottom;
            if (bottom.A == 0 || top.A == 255) return top;
            var ratio = top.A * 1f / 255;
            if (bottom.A < 255) ratio += (255 - bottom.A) / 255f * (1 - ratio);
            if (ratio >= 1) return top;
            var negRatio = 1 - ratio;
#if NETOLDVER
            var a = 255 - (int)Math.Round((255d - top.A) * (255d - bottom.A) * 255);
#else
            var a = 255 - (int)MathF.Round((255f - top.A) * (255f - bottom.A) * 255);
#endif
            return Color.FromArgb(
                a,
                ToChannel(top.R * ratio + bottom.R * negRatio),
                ToChannel(top.G * ratio + bottom.G * negRatio),
                ToChannel(top.B * ratio + bottom.B * negRatio));
        }

        /// <summary>
        /// Adds a color on another one.
        /// </summary>
        /// <param name="top">The color on top.</param>
        /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
        /// <param name="bottom">The color on bottom.</param>
        /// <returns>A new color.</returns>
        public static Color Overlay(Color top, double alpha, Color bottom)
            => Overlay(Alpha(top, alpha), bottom);

        /// <summary>
        /// Adds a color on another one.
        /// </summary>
        /// <param name="top">The color on top.</param>
        /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
        /// <param name="bottom">The color on bottom.</param>
        /// <returns>A new color.</returns>
        public static Color Overlay(Color top, float alpha, Color bottom)
            => Overlay(Alpha(top, alpha), bottom);

        /// <summary>
        /// Adds a color on another one.
        /// </summary>
        /// <param name="top">The color on top.</param>
        /// <param name="alpha">Alpha for top color. Value is from 0 to 1.</param>
        /// <param name="bottom">The color on bottom.</param>
        /// <returns>A new color.</returns>
        public static Color Overlay(Color top, byte alpha, Color bottom)
            => Overlay(Alpha(top, alpha), bottom);

        /// <summary>
        /// Mixes colors.
        /// </summary>
        /// <param name="type">The type to mix colors.</param>
        /// <param name="a">The color 1.</param>
        /// <param name="b">The color 2.</param>
        /// <returns>A new color.</returns>
        public static Color Mix(ColorMixTypes type, Color a, Color b)
        {
            return type switch
            {
                ColorMixTypes.Lighten => MixByLighten(a, b),
                ColorMixTypes.Darken => MixByDarken(a, b),
                ColorMixTypes.Weaken => MixByWeaken(a, b),
                ColorMixTypes.Deepen => MixByDeepen(a, b),
                ColorMixTypes.Accent => MixByAccent(a, b),
                ColorMixTypes.Diff => MixByDiff(a, b),
                _ => MixByMean(a, b),
            };
        }

        private static Color MixByMean(Color a, Color b)
        {
            if (a.A == b.A)
                return Color.FromArgb(
                    Math.Max(a.A, b.A),
                    ToChannel((a.R + b.R) / 2),
                    ToChannel((a.G + b.G) / 2),
                    ToChannel((a.B + b.B) / 2));
            var topAlpha = a.A / 255f;
            var bottomAlpha = b.A / 255f;
            var total = topAlpha + bottomAlpha;
            return Color.FromArgb(
                Math.Max(a.A, b.A),
                ToChannel((a.R * topAlpha + b.R * bottomAlpha) / total),
                ToChannel((a.G * topAlpha + b.G * bottomAlpha) / total),
                ToChannel((a.B * topAlpha + b.B * bottomAlpha) / total));
        }

        private static Color MixByLighten(Color a, Color b)
        {
            var red = Math.Max(a.R, b.R);
            var green = Math.Max(a.G, b.G);
            var blue = Math.Max(a.B, b.B);
            return MixWithAlpha(red, green, blue, a, b);
        }

        private static Color MixByDarken(Color a, Color b)
        {
            var red = Math.Min(a.R, b.R);
            var green = Math.Min(a.G, b.G);
            var blue = Math.Min(a.B, b.B);
            return MixWithAlpha(red, green, blue, a, b);
        }

        private static Color MixByWeaken(Color a, Color b)
        {
            var red = 255 - (255 - a.R) * (255 - b.R) / 255f;
            var green = 255 - (255 - a.G) * (255 - b.G) / 255f;
            var blue = 255 - (255 - a.B) * (255 - b.B) / 255f;
            return MixWithAlpha(red, green, blue, a, b);
        }

        private static Color MixByDeepen(Color a, Color b)
        {
            var red = a.R * b.R / 255f;
            var green = a.G * b.G / 255f;
            var blue = a.B * b.B / 255f;
            return MixWithAlpha(red, green, blue, a, b);
        }

        private static Color MixByAccent(Color a, Color b)
        {
            if (a.A == b.A)
                return Color.FromArgb(
                    Math.Max(a.A, b.A),
                    ToChannel(a.R + b.R),
                    ToChannel(a.G + b.G),
                    ToChannel(a.B + b.B));
            float alpha = a.A + b.A;
            var topAlpha = a.A / alpha;
            var bottomAlpha = b.A / alpha;
            return Color.FromArgb(
                Math.Max(a.A, b.A),
                    ToChannel(a.R * topAlpha + b.R * bottomAlpha),
                    ToChannel(a.G * topAlpha + b.G * bottomAlpha),
                    ToChannel(a.B * topAlpha + b.B * bottomAlpha));
        }

        private static Color MixByDiff(Color a, Color b)
        {
            var red = a.R - b.R;
            if (red < 0) red += 256;
            var green = a.G - b.G;
            if (green < 0) green += 256;
            var blue = a.B - b.B;
            if (blue < 0) blue += 256;
            return MixWithAlpha(red, green, blue, a, b);
        }

        private static Color MixWithAlpha(int red, int green, int blue, Color a, Color b)
        {
            if (a.A == b.A)
                return Color.FromArgb(Math.Max(a.A, b.A), red, green, blue);
            var ratio = Math.Abs(a.A - b.A) * 1f / Math.Max(a.A, b.A);
            var negRation = 1 - ratio;
            var c = a.A > b.A ? a : b;
            return Color.FromArgb(
                Math.Max(a.A, b.A),
                ToChannel(red * negRation + c.R * ratio),
                ToChannel(green * negRation + c.G * ratio),
                ToChannel(blue * negRation + c.B * ratio));
        }

        private static Color MixWithAlpha(float red, float green, float blue, Color a, Color b)
        {
            if (a.A == b.A)
                return Color.FromArgb(Math.Max(a.A, b.A), ToChannel(red), ToChannel(green), ToChannel(blue));
            var ratio = Math.Abs(a.A - b.A) * 1f / Math.Max(a.A, b.A);
            var negRation = 1 - ratio;
            var c = a.A > b.A ? a : b;
            return Color.FromArgb(
                Math.Max(a.A, b.A),
                ToChannel(red * negRation + c.R * ratio),
                ToChannel(green * negRation + c.G * ratio),
                ToChannel(blue * negRation + c.B * ratio));
        }
    }
}
