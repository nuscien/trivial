using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The linear gradient console text style.
    /// </summary>
    public class LinearGradientConsoleStyle : IConsoleTextPrettier
    {
        private readonly Color? fromFore;
        private readonly Color? toFore;
        private readonly Color? fromBack;
        private readonly Color? toBack;

        /// <summary>
        /// Initialzies a new instance of the LinearGradientConsoleStyle class.
        /// </summary>
        /// <param name="fallbackForegroundColor">The fallback foreground color.</param>
        /// <param name="fromForegroundColor">The from foreground color.</param>
        /// <param name="toForegroundColor">The to foreground color.</param>
        public LinearGradientConsoleStyle(ConsoleColor? fallbackForegroundColor, Color fromForegroundColor, Color toForegroundColor)
        {
            FallbackForegroundColor = fallbackForegroundColor;
            fromFore = fromForegroundColor;
            toFore = toForegroundColor;
        }

        /// <summary>
        /// Initialzies a new instance of the LinearGradientConsoleStyle class.
        /// </summary>
        /// <param name="fallbackForegroundColor">The fallback foreground color.</param>
        /// <param name="fromForegroundColor">The from foreground color.</param>
        /// <param name="toForegroundColor">The to foreground color.</param>
        /// <param name="fallbackBackgroundColor">The fallback background color.</param>
        /// <param name="fromBackgroundColor">The from background color.</param>
        /// <param name="toBackgroundColor">The to background color.</param>
        public LinearGradientConsoleStyle(ConsoleColor? fallbackForegroundColor, Color fromForegroundColor, Color toForegroundColor, ConsoleColor? fallbackBackgroundColor, Color fromBackgroundColor, Color toBackgroundColor)
            : this(fallbackForegroundColor, fromForegroundColor, toForegroundColor)
        {
            FallbackBackgroundColor = fallbackBackgroundColor;
            fromBack = fromBackgroundColor;
            toBack = toBackgroundColor;
        }

        /// <summary>
        /// Gets or sets the fallback foreground color.
        /// </summary>
        public ConsoleColor? FallbackForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the fallback background color.
        /// </summary>
        public ConsoleColor? FallbackBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is blink.
        /// </summary>
        public bool Blink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is bold.
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is italic.
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is underlined.
        /// </summary>
        public bool Underline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is strikeout.
        /// </summary>
        public bool Strikeout { get; set; }

        /// <summary>
        /// Creates the console text collection based on this style.
        /// </summary>
        /// <param name="s">The text.</param>
        /// <returns>A collection of console text.</returns>
        IEnumerable<ConsoleText> IConsoleTextPrettier.CreateTextCollection(string s)
        {
            var col = new List<ConsoleText>();
            if (string.IsNullOrEmpty(s)) return col;
            col.Add(s[0], 1, new ConsoleTextStyle(fromFore, FallbackForegroundColor, fromBack, FallbackBackgroundColor)
            {
                Blink = Blink,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strikeout = Strikeout
            });
            if (s.Length == 1)
            {
                if (fromFore.HasValue && toFore.HasValue)
                    col[0].Style.ForegroundRgbColor = Color.FromArgb((fromFore.Value.R + toFore.Value.R) / 2, (fromFore.Value.G + toFore.Value.G) / 2, (fromFore.Value.B + toFore.Value.B) / 2);
                else if (!fromFore.HasValue)
                    col[0].Style.ForegroundRgbColor = toFore;
                if (fromBack.HasValue && toBack.HasValue)
                    col[0].Style.BackgroundRgbColor = Color.FromArgb((fromBack.Value.R + toBack.Value.R) / 2, (fromBack.Value.G + toBack.Value.G) / 2, (fromBack.Value.B + toBack.Value.B) / 2);
                else if (!fromBack.HasValue)
                    col[0].Style.BackgroundRgbColor = toBack;
                return col;
            }

            var step = s.Length - 1;
            var fore = fromFore ?? toFore;
            var back = fromBack ?? toBack;
            var foreDelta = fromFore.HasValue && toFore.HasValue
                ? ((toFore.Value.R - fromFore.Value.R) * 1.0 / step, (toFore.Value.G - fromFore.Value.B) * 1.0 / step, (toFore.Value.B - fromFore.Value.B) * 1.0 / step)
                : (0.0, 0.0, 0.0);
            var backDelta = fromBack.HasValue && toBack.HasValue
                ? ((toBack.Value.R - fromBack.Value.R) * 1.0 / step, (toBack.Value.G - fromBack.Value.B) * 1.0 / step, (toBack.Value.B - fromBack.Value.B) * 1.0 / step)
                : (0.0, 0.0, 0.0);
            var last = s.Length - 1;
            for (var i = 1; i < last; i++)
            {
                if (fore.HasValue) fore = Color.FromArgb(
                    PlusChannel(fore.Value.R, foreDelta.Item1),
                    PlusChannel(fore.Value.G, foreDelta.Item2),
                    PlusChannel(fore.Value.B, foreDelta.Item3));
                if (back.HasValue) back = Color.FromArgb(
                    PlusChannel(back.Value.R, backDelta.Item1),
                    PlusChannel(back.Value.G, backDelta.Item2),
                    PlusChannel(back.Value.B, backDelta.Item3));
                col.Add(s[i], 1, new ConsoleTextStyle(fore, FallbackForegroundColor, back, FallbackBackgroundColor)
                {
                    Blink = Blink,
                    Bold = Bold,
                    Italic = Italic,
                    Underline = Underline,
                    Strikeout = Strikeout
                });
            }

            col.Add(s[s.Length - 1], 1, new ConsoleTextStyle(toFore, FallbackForegroundColor, toBack, FallbackBackgroundColor)
            {
                Blink = Blink,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strikeout = Strikeout
            });
            return col;
        }

        private int PlusChannel(int c, double delta)
        {
            c = (int)Math.Round(c + delta);
            if (c < 0) return 0;
            else if (c > 255) return 255;
            return c;
        }
    }
}
