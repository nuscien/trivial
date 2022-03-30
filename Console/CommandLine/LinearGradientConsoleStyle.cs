using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.CommandLine;

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

        var steps = s.Length - 1;
        var hasFore = fromFore.HasValue || toFore.HasValue;
        var hasBack = fromBack.HasValue || toBack.HasValue;
        var foreDelta = fromFore.HasValue && toFore.HasValue
            ? ((toFore.Value.R - fromFore.Value.R) * 1.0 / steps, (toFore.Value.G - fromFore.Value.B) * 1.0 / steps, (toFore.Value.B - fromFore.Value.B) * 1.0 / steps)
            : (0.0, 0.0, 0.0);
        var backDelta = fromBack.HasValue && toBack.HasValue
            ? ((toBack.Value.R - fromBack.Value.R) * 1.0 / steps, (toBack.Value.G - fromBack.Value.B) * 1.0 / steps, (toBack.Value.B - fromBack.Value.B) * 1.0 / steps)
            : (0.0, 0.0, 0.0);
        double foreR = fromFore?.R ?? toFore?.R ?? 0;
        double foreG = fromFore?.G ?? toFore?.G ?? 0;
        double foreB = fromFore?.B ?? toFore?.B ?? 0;
        double backR = fromBack?.R ?? toBack?.R ?? 0;
        double backG = fromBack?.G ?? toBack?.G ?? 0;
        double backB = fromBack?.B ?? toBack?.B ?? 0;
        for (var i = 1; i < steps; i++)
        {
            Color? fore = hasFore ? Color.FromArgb(
                PlusChannel(ref foreR, foreDelta.Item1),
                PlusChannel(ref foreG, foreDelta.Item2),
                PlusChannel(ref foreB, foreDelta.Item3)) : null;
            Color? back = hasBack ? Color.FromArgb(
                PlusChannel(ref backR, backDelta.Item1),
                PlusChannel(ref backG, backDelta.Item2),
                PlusChannel(ref backB, backDelta.Item3)) : null;
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

    private static int PlusChannel(ref double c, double delta)
    {
        c = Math.Round(c + delta);
        var r = (int)c;
        if (r < 0) return 0;
        else if (r > 255) return 255;
        return r;
    }
}
