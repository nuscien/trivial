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
public class RepeatedColorConsoleStyle : IConsoleTextPrettier
{
    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    public RepeatedColorConsoleStyle()
    {
    }

    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    /// <param name="fallbackForegroundColor">The fallback foreground color.</param>
    /// <param name="foregroundColors">The foreground colors.</param>
    public RepeatedColorConsoleStyle(ConsoleColor fallbackForegroundColor, IEnumerable<Color> foregroundColors)
    {
        ForegroundConsoleColors.Add(fallbackForegroundColor);
        if (foregroundColors != null) ForegroundRgbColors.AddRange(foregroundColors);
    }

    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    /// <param name="foregroundConsoleColors">The foreground console colors.</param>
    /// <param name="foregroundRgbColors">The foreground RGB colors.</param>
    public RepeatedColorConsoleStyle(IEnumerable<ConsoleColor> foregroundConsoleColors, IEnumerable<Color> foregroundRgbColors)
    {
        if (foregroundConsoleColors != null) ForegroundConsoleColors.AddRange(foregroundConsoleColors);
        if (foregroundRgbColors != null) ForegroundRgbColors.AddRange(foregroundRgbColors);
    }

    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    /// <param name="foregroundConsoleColors">The foreground console colors.</param>
    /// <param name="foregroundRgbColors">The foreground RGB colors.</param>
    /// <param name="backgroundConsoleColor">The background console colors.</param>
    /// <param name="backgroundRgbColor">The background RGB colors.</param>
    public RepeatedColorConsoleStyle(IEnumerable<ConsoleColor> foregroundConsoleColors, IEnumerable<Color> foregroundRgbColors, ConsoleColor? backgroundConsoleColor, Color? backgroundRgbColor)
    {
        if (foregroundConsoleColors != null) ForegroundConsoleColors.AddRange(foregroundConsoleColors);
        if (foregroundRgbColors != null) ForegroundRgbColors.AddRange(foregroundRgbColors);
        if (backgroundConsoleColor.HasValue) BackgroundConsoleColors.Add(backgroundConsoleColor.Value);
        if (backgroundRgbColor.HasValue) BackgroundRgbColors.Add(backgroundRgbColor.Value);
    }

    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    /// <param name="foregroundConsoleColors">The foreground console colors.</param>
    /// <param name="foregroundRgbColors">The foreground RGB colors.</param>
    /// <param name="backgroundConsoleColors">The background console colors.</param>
    /// <param name="backgroundRgbColors">The background RGB colors.</param>
    public RepeatedColorConsoleStyle(IEnumerable<ConsoleColor> foregroundConsoleColors, IEnumerable<Color> foregroundRgbColors, IEnumerable<ConsoleColor> backgroundConsoleColors, IEnumerable<Color> backgroundRgbColors)
    {
        if (foregroundConsoleColors != null) ForegroundConsoleColors.AddRange(foregroundConsoleColors);
        if (foregroundRgbColors != null) ForegroundRgbColors.AddRange(foregroundRgbColors);
        if (backgroundConsoleColors != null) BackgroundConsoleColors.AddRange(backgroundConsoleColors);
        if (backgroundRgbColors != null) BackgroundRgbColors.AddRange(backgroundRgbColors);
    }

    /// <summary>
    /// Initialzies a new instance of the RepeatedColorConsoleStyle class.
    /// </summary>
    /// <param name="foregroundConsoleColor">The foreground console color.</param>
    /// <param name="foregroundRgbColor">The foreground RGB color.</param>
    /// <param name="backgroundConsoleColors">The background console colors.</param>
    /// <param name="backgroundRgbColors">The background RGB colors.</param>
    public RepeatedColorConsoleStyle(ConsoleColor foregroundConsoleColor, Color foregroundRgbColor, IEnumerable<ConsoleColor> backgroundConsoleColors, IEnumerable<Color> backgroundRgbColors)
    {
        ForegroundConsoleColors.Add(foregroundConsoleColor);
        ForegroundRgbColors.Add(foregroundRgbColor);
        if (backgroundConsoleColors != null) BackgroundConsoleColors.AddRange(backgroundConsoleColors);
        if (backgroundRgbColors != null) BackgroundRgbColors.AddRange(backgroundRgbColors);
    }

    /// <summary>
    /// Gets or sets the foreground console colors.
    /// </summary>
    public List<ConsoleColor> ForegroundConsoleColors { get; } = new();

    /// <summary>
    /// Gets or sets the foreground RGB colors.
    /// </summary>
    public List<Color> ForegroundRgbColors { get; } = new();

    /// <summary>
    /// Gets or sets the background console colors.
    /// </summary>
    public List<ConsoleColor> BackgroundConsoleColors { get; } = new();

    /// <summary>
    /// Gets or sets the background RGB colors.
    /// </summary>
    public List<Color> BackgroundRgbColors { get; } = new();

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
        if (ForegroundConsoleColors.Count < 2 && ForegroundRgbColors.Count < 2 && BackgroundConsoleColors.Count < 2 && BackgroundRgbColors.Count < 2)
        {
            col.Add(s, new ConsoleTextStyle(
                ForegroundRgbColors.FirstOrDefault(),
                ForegroundConsoleColors.FirstOrDefault(),
                BackgroundRgbColors.FirstOrDefault(),
                BackgroundConsoleColors.FirstOrDefault())
            {
                Blink = Blink,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strikeout = Strikeout
            });
            return col;
        }

        var i = 0;
        foreach (var c in s)
        {
            Color? foreRgb = ForegroundRgbColors.Count > 0
                ? ForegroundRgbColors[i % ForegroundRgbColors.Count]
                : null;
            ConsoleColor? foreConsole = ForegroundConsoleColors.Count > 0
                ? ForegroundConsoleColors[i % ForegroundConsoleColors.Count]
                : null;
            Color? backRgb = BackgroundRgbColors.Count > 0
                ? BackgroundRgbColors[i % BackgroundRgbColors.Count]
                : null;
            ConsoleColor? backConsole = BackgroundConsoleColors.Count > 0
                ? BackgroundConsoleColors[i % BackgroundConsoleColors.Count]
                : null;
            i++;
            col.Add(c, 1, new ConsoleTextStyle(foreRgb, foreConsole, backRgb, backConsole)
            {
                Blink = Blink,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strikeout = Strikeout
            });
        }

        return col;
    }
}
