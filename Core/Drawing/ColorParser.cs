using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Maths;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Drawing
{
    /// <summary>
    /// Color calculator.
    /// </summary>
    public static partial class ColorCalculator
    {
#pragma warning disable IDE0057
        /// <summary>
        /// Tries to parse a color.
        /// </summary>
        /// <param name="s">The input string to parse.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public static bool TryParse(string s, out Color result)
        {
            s = s.Trim();
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
                var arr = value.Split(new[] { ',' }).Select(ele =>
                {
                    var item = ele?.Trim();
                    if (string.IsNullOrEmpty(item)) return 0;
                    if (float.TryParse(item, out var r)) return r;
                    if (item.Length > 1 && float.TryParse(item.Substring(0, item.Length - 1), out r))
                    {
                        if (item.EndsWith('%')) return r * 0.01;
                        if (item.EndsWith('‰')) return r * 0.001;
                        if (item.EndsWith('‱')) return r * 0.0001;
                    }

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

#if NETSTANDARD2_0
            try
            {
                result = Color.FromName(s);
                return true;
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (FormatException)
            {
            }
#else
            if (Enum.TryParse<KnownColor>(s, true, out var kc))
            {
                result = Color.FromKnownColor(kc);
                return true;
            }
#endif

            return TryParseHexColor(s, out result);
        }

        /// <summary>
        /// Parses a color.
        /// </summary>
        /// <param name="s">The input string to parse.</param>
        /// <returns>The result color parsed.</returns>
        public static Color Parse(string s)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s)) return Color.Transparent;
            if (TryParse(s, out var r)) return r;
            throw new FormatException("s is not a color.", new ArgumentException("s is not supported.", nameof(s)));
        }

        #region HSL

        /// <summary>
        /// Creates color from HSL.
        /// </summary>
        /// <param name="hue">The hue. Value is from 0 to 360.</param>
        /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
        /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
        /// <returns>A color.</returns>
        public static Color FromHSL(double hue, double saturation, double lightness)
            => FromHSL(hue, saturation, lightness, 255);

        /// <summary>
        /// Creates color from HSL.
        /// </summary>
        /// <param name="hue">The hue. Value is from 0 to 360.</param>
        /// <param name="saturation">The saturation. Value is from 0 to 1.</param>
        /// <param name="lightness">The lightness. Value is from 0 to 1.</param>
        /// <param name="alpha">The alpha. Value is from 0 to 1.</param>
        /// <returns>A color.</returns>
        public static Color FromHSL(double hue, double saturation, double lightness, double alpha)
            => FromHSL(hue, saturation, lightness, (byte)ToChannel(alpha * 255));

        /// <summary>
        /// Creates color from HSL.
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
            return Color.FromArgb(
                alpha,
                ToChannel(255d * FromHue(i, j, h + (1d / 3d))),
                ToChannel(255d * FromHue(i, j, h)),
                ToChannel(255d * FromHue(i, j, h - (1d / 3d))));
        }

        private static double FromHue(double v1, double v2, double vH)
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if (6.0 * vH < 1) return v1 + (v2 - v1) * 6.0 * vH;
            if (2.0 * vH < 1) return v2;
            if (3.0 * vH < 2) return v1 + (v2 - v1) * ((2.0 / 3.0) - vH) * 6.0;
            return v1;
        }

        /// <summary>
        /// Converts a color to HSL.
        /// </summary>
        /// <param name="value">The color to get HSL values.</param>
        /// <returns>The HSL tuple.</returns>
        public static (double, double, double) ToHSL(Color value)
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

        #endregion
        #region HSV

        /// <summary>
        /// Converts a color to HSV.
        /// </summary>
        /// <param name="value">The color to get HSV values.</param>
        /// <returns>The HSV tuple.</returns>
        public static (double, double, double) ToHSV(Color value)
        {
            var min = Arithmetic.Min(value.R, value.G, value.B) / 255d;
            var max = Arithmetic.Max(value.R, value.G, value.B) / 255d;
            return (value.GetHue(), max == 0d ? 0d : (max - min) / max, max);
        }

        #endregion
        #region CMYK

        /// <summary>
        /// Creates color from HSL.
        /// </summary>
        /// <param name="cyan">The cyan. Value is from 0 to 1.</param>
        /// <param name="magenta">The magenta. Value is from 0 to 1.</param>
        /// <param name="yellow">The yellow. Value is from 0 to 1.</param>
        /// <param name="black">The black. Value is from 0 to 1.</param>
        /// <returns>A color.</returns>
        public static Color FromCMYK(double cyan, double magenta, double yellow, double black)
            => FromCMYK(cyan, magenta, yellow, black, 255);

        /// <summary>
        /// Creates color from HSL.
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
        /// Creates color from HSL.
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
        /// Converts a color to CMYK.
        /// </summary>
        /// <param name="value">The color to get CMYK values.</param>
        /// <returns>The CMYK tuple.</returns>
        public static (double, double, double, double) ToCMYK(Color value)
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

            if (s.Length == 4)
            {
                var a = GetDigit(s, 0);
                var r = GetDigit(s, 1);
                var g = GetDigit(s, 2);
                var b = GetDigit(s, 3);
                if (a < 0 || r < 0 || g < 0 || b < 0)
                {
                    result = default;
                    return false;
                }

                result = Color.FromArgb(a * 16 + a, r * 16 + r, g * 16 + g, b * 16 + b);
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
#pragma warning restore IDE0057
    }
}
