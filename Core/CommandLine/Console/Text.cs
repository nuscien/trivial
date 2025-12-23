using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The options for segment.
/// </summary>
public class ConsoleTextStyle : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the ConsoleTextStyle class.
    /// </summary>
    public ConsoleTextStyle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleTextStyle class.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    public ConsoleTextStyle(ConsoleColor? foreground, ConsoleColor? background = null)
    {
        ForegroundConsoleColor = foreground;
        BackgroundConsoleColor = background;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleTextStyle class.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="nullConsoleColors">true if set console colors as null; otherwise, false.</param>
    public ConsoleTextStyle(Color? foreground, Color? background = null, bool nullConsoleColors = false)
    {
        ForegroundRgbColor = foreground;
        BackgroundRgbColor = background;
        if (nullConsoleColors) return;
        if (foreground.HasValue) ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(foreground.Value);
        if (background.HasValue) BackgroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(background.Value);
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleTextStyle class.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="foreground2">The fallback foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="background2">The fallback background color.</param>
    public ConsoleTextStyle(Color? foreground, ConsoleColor? foreground2, Color? background, ConsoleColor? background2)
    {
        ForegroundRgbColor = foreground;
        BackgroundRgbColor = background;
        ForegroundConsoleColor = foreground2;
        BackgroundConsoleColor = background2;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the text is blink.
    /// </summary>
    [JsonPropertyName("blink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Blink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is bold.
    /// </summary>
    [JsonPropertyName("b")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Bold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is italic.
    /// </summary>
    [JsonPropertyName("i")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Italic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is underlined.
    /// </summary>
    [JsonPropertyName("u")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Underline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text is strikeout.
    /// </summary>
    [JsonPropertyName("strikeout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Strikeout { get; set; }

    /// <summary>
    /// Gets or sets the RGB color for foreground.
    /// </summary>
    [JsonPropertyName("fore")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? ForegroundRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color for background.
    /// </summary>
    [JsonPropertyName("back")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? BackgroundRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the console color for foreground.
    /// </summary>
    [JsonPropertyName("fore2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? ForegroundConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the console color for background.
    /// </summary>
    [JsonPropertyName("back2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? BackgroundConsoleColor { get; set; }

#if NETFRAMEWORK
    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    [JsonIgnore]
    public FontStyle FontStyle
    {
        get
        {
            return (Bold ? FontStyle.Bold : FontStyle.Regular)
                | (Italic ? FontStyle.Italic : FontStyle.Regular)
                | (Underline ? FontStyle.Underline : FontStyle.Regular)
                | (Strikeout ? FontStyle.Strikeout : FontStyle.Regular);
        }

        set
        {
            Bold = value.HasFlag(FontStyle.Bold);
            Italic = value.HasFlag(FontStyle.Italic);
            Underline = value.HasFlag(FontStyle.Underline);
            Strikeout = value.HasFlag(FontStyle.Strikeout);
        }
    }
#endif

    /// <summary>
    /// Sets the foreground.
    /// </summary>
    /// <param name="r">The red component value. Valid values are 0 through 255.</param>
    /// <param name="g">The green component value. Valid values are 0 through 255.</param>
    /// <param name="b">The blue component value. Valid values are 0 through 255.</param>
    /// <param name="fallback">The fallback color.</param>
    public void SetForeground(byte r, byte g, byte b, ConsoleColor fallback)
    {
        ForegroundRgbColor = Color.FromArgb(r, g, b);
        ForegroundConsoleColor = fallback;
    }

    /// <summary>
    /// Sets the foreground.
    /// </summary>
    /// <param name="r">The red component value. Valid values are 0 through 255.</param>
    /// <param name="g">The green component value. Valid values are 0 through 255.</param>
    /// <param name="b">The blue component value. Valid values are 0 through 255.</param>
    /// <param name="convertToFallback">true if also convert to fallback console color.</param>
    public void SetForeground(byte r, byte g, byte b, bool convertToFallback = false)
    {
        var color = Color.FromArgb(r, g, b);
        ForegroundRgbColor = color;
        if (convertToFallback) ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(color);
    }

    /// <summary>
    /// Sets the foreground.
    /// </summary>
    /// <param name="rgbColor">The RGB color.</param>
    /// <param name="consoleColor">The fallback console color.</param>
    public void SetForeground(Color? rgbColor, ConsoleColor? consoleColor)
    {
        ForegroundRgbColor = rgbColor;
        ForegroundConsoleColor = consoleColor;
    }

    /// <summary>
    /// Sets the background.
    /// </summary>
    /// <param name="r">The red component value. Valid values are 0 through 255.</param>
    /// <param name="g">The green component value. Valid values are 0 through 255.</param>
    /// <param name="b">The blue component value. Valid values are 0 through 255.</param>
    /// <param name="convertToFallback">true if also convert to fallback console color.</param>
    public void SetBackground(byte r, byte g, byte b, bool convertToFallback = false)
    {
        var color = Color.FromArgb(r, g, b);
        BackgroundRgbColor = color;
        if (convertToFallback) BackgroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(color);
    }

    /// <summary>
    /// Sets the background.
    /// </summary>
    /// <param name="r">The red component value. Valid values are 0 through 255.</param>
    /// <param name="g">The green component value. Valid values are 0 through 255.</param>
    /// <param name="b">The blue component value. Valid values are 0 through 255.</param>
    /// <param name="fallback">The fallback color.</param>
    public void SetBackground(byte r, byte g, byte b, ConsoleColor fallback)
    {
        BackgroundRgbColor = Color.FromArgb(r, g, b);
        BackgroundConsoleColor = fallback;
    }

    /// <summary>
    /// Sets the background.
    /// </summary>
    /// <param name="rgbColor">The RGB color.</param>
    /// <param name="consoleColor">The fallback console color.</param>
    public void SetBackground(Color? rgbColor, ConsoleColor? consoleColor)
    {
        BackgroundRgbColor = rgbColor;
        BackgroundConsoleColor = consoleColor;
    }

    /// <summary>
    /// Clears all foreground colors.
    /// </summary>
    public void ClearForeground()
    {
        ForegroundConsoleColor = null;
        ForegroundRgbColor = null;
    }

    /// <summary>
    /// Clears all background colors.
    /// </summary>
    public void ClearBackground()
    {
        BackgroundConsoleColor = null;
        BackgroundRgbColor = null;
    }

    /// <summary>
    /// Clears all RGB colors.
    /// </summary>
    public void ClearRgbColors()
    {
        ForegroundRgbColor = null;
        BackgroundRgbColor = null;
    }

    /// <summary>
    /// Clears all console colors.
    /// </summary>
    public void ClearConsoleColors()
    {
        ForegroundConsoleColor = null;
        BackgroundConsoleColor = null;
    }

    /// <summary>
    /// Clones an object.
    /// </summary>
    /// <returns>The object copied from this instance.</returns>
    public virtual ConsoleTextStyle Clone()
        => new()
        {
            Blink = Blink,
            Bold = Bold,
            Italic = Italic,
            Underline = Underline,
            Strikeout = Strikeout,
            ForegroundRgbColor = ForegroundRgbColor,
            ForegroundConsoleColor = ForegroundConsoleColor,
            BackgroundRgbColor = BackgroundRgbColor,
            BackgroundConsoleColor = BackgroundConsoleColor
        };

    /// <summary>
    /// Clones an object.
    /// </summary>
    /// <returns>The object copied from this instance.</returns>
    object ICloneable.Clone()
        => Clone();

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(ConsoleTextStyle leftValue, string rightValue)
    {
        if (rightValue is null) return null;
        return new ConsoleText(rightValue, leftValue);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(string leftValue, ConsoleTextStyle rightValue)
    {
        if (leftValue is null) return null;
        return new ConsoleText(leftValue, rightValue);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(ConsoleTextStyle leftValue, StringBuilder rightValue)
    {
        if (rightValue is null) return null;
        return new ConsoleText(rightValue, leftValue);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(StringBuilder leftValue, ConsoleTextStyle rightValue)
    {
        if (leftValue is null) return null;
        return new ConsoleText(leftValue, rightValue);
    }
}

/// <summary>
/// The segement.
/// </summary>
public class ConsoleText
{
    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    public ConsoleText()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    /// <param name="style">The style.</param>
    public ConsoleText(char c, int repeatCount = 1, ConsoleTextStyle style = null)
    {
        if (repeatCount >= 0) Content.Append(c, repeatCount);
        if (style != null) Style = style;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="style">The style.</param>
    public ConsoleText(string s, ConsoleTextStyle style = null)
    {
        Content.Append(s);
        if (style != null) Style = style;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="style">The style.</param>
    public ConsoleText(StringBuilder s, ConsoleTextStyle style = null)
    {
        if (s != null) Content = s;
        if (style != null) Style = style;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    public ConsoleText(char c, int repeatCount, ConsoleColor? foreground, ConsoleColor? background = null)
    {
        if (repeatCount >= 0) Content.Append(c, repeatCount);
        Style.ForegroundConsoleColor = foreground;
        Style.BackgroundConsoleColor = background;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    public ConsoleText(string s, ConsoleColor? foreground, ConsoleColor? background = null)
    {
        Content = new StringBuilder(s);
        Style.ForegroundConsoleColor = foreground;
        Style.BackgroundConsoleColor = background;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    public ConsoleText(StringBuilder s, ConsoleColor? foreground, ConsoleColor? background = null)
    {
        if (s != null) Content = s;
        Style.ForegroundConsoleColor = foreground;
        Style.BackgroundConsoleColor = background;
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    public ConsoleText(string s, Color foreground)
    {
        Content = new StringBuilder(s);
        Style.ForegroundRgbColor = foreground;
        Style.ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(foreground);
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="nullConsoleColors">true if set console colors as null; otherwise, false.</param>
    public ConsoleText(string s, Color foreground, Color? background = null, bool nullConsoleColors = false)
    {
        Content = new StringBuilder(s);
        Style.ForegroundRgbColor = foreground;
        Style.BackgroundRgbColor = background;
        if (nullConsoleColors) return;
        Style.ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(foreground);
        if (background.HasValue) Style.BackgroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(background.Value);
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="nullConsoleColors">true if set console colors as null; otherwise, false.</param>
    public ConsoleText(StringBuilder s, Color foreground, Color? background = null, bool nullConsoleColors = false)
    {
        if (s != null) Content = s;
        Style.ForegroundRgbColor = foreground;
        Style.BackgroundRgbColor = background;
        if (nullConsoleColors) return;
        Style.ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(foreground);
        if (background.HasValue) Style.BackgroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(background.Value);
    }

    /// <summary>
    /// Initializes a new instance of the ConsoleText class.
    /// </summary>
    /// <param name="s">The text content.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="foreground2">The backup foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="background2">The backup background color.</param>
    public ConsoleText(string s, Color? foreground, ConsoleColor? foreground2, Color? background, ConsoleColor? background2)
    {
        Content = new StringBuilder(s);
        Style.ForegroundRgbColor = foreground;
        Style.BackgroundRgbColor = background;
        Style.ForegroundConsoleColor = foreground2;
        Style.BackgroundConsoleColor = background2;
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public ConsoleTextStyle Style { get; } = new();

    /// <summary>
    /// Gets the text content.
    /// </summary>
    public StringBuilder Content { get; } = new();

    /// <summary>
    /// Gets the length of the text content.
    /// </summary>
    public int Length => Content.Length;

    /// <summary>
    /// Clears the content.
    /// </summary>
    public void ClearContent()
        => Content.Clear();

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="s">The string to append.</param>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public void Append(string s)
        => Content.Append(s);

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="s">The string to append.</param>
    /// <param name="start">The starting position of the substring within value.</param>
    /// <param name="count">The number of characters in value to append.</param>
    /// <exception cref="ArgumentNullException">value is null, and startIndex and count are not zero.</exception>
    /// <exception cref="ArgumentOutOfRangeException">count less than zero. -or- startIndex less than zero. -or- startIndex + count is greater than the length of value. -or- Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public void Append(string s, int start, int count)
        => Content.Append(s, start, count);

    /// <summary>
    /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to format.</param>
    /// <exception cref="ArgumentNullException">format or args is null.</exception>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than 0 (zero), or greater than or equal to the length of the args array.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public void AppendFormat(string format, params object[] args)
        => Content.AppendFormat(format, args);

#if NETCOREAPP
    /// <summary>
    /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to format.</param>
    /// <exception cref="ArgumentNullException">format or args is null.</exception>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than 0 (zero), or greater than or equal to the length of the args array.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public void AppendFormat(string format, params ReadOnlySpan<object> args)
        => Content.AppendFormat(format, args);
#endif

    /// <summary>
    /// Returns a string with ANSI escape sequences that represents the content.
    /// </summary>
    /// <returns>A string that represents the content.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        AppendTo(sb);
        return sb.ToString();
    }

    internal void AppendTo(StringBuilder sb)
    {
        var foreground = Style.ForegroundRgbColor.HasValue
            ? AnsiCodeGenerator.Foreground(Style.ForegroundRgbColor.Value)
            : AnsiCodeGenerator.Foreground(Style.ForegroundConsoleColor);
        var background = Style.BackgroundRgbColor.HasValue
            ? AnsiCodeGenerator.Background(Style.BackgroundRgbColor.Value)
            : AnsiCodeGenerator.Background(Style.BackgroundConsoleColor);
        var b = Style.Bold;
        var i = Style.Italic;
        var u = Style.Underline;
        var s = Style.Strikeout;
        var blink = Style.Blink;
        sb.Append(foreground);
        sb.Append(background);
        if (b) sb.Append(AnsiCodeGenerator.Bold(true));
        if (i) sb.Append(AnsiCodeGenerator.Italic(true));
        if (u) sb.Append(AnsiCodeGenerator.Underline(true));
        if (s) sb.Append(AnsiCodeGenerator.Strikeout(true));
        if (blink) sb.Append(AnsiCodeGenerator.Blink(true));
        StringExtensions.Append(sb, Content);
        if (blink) sb.Append(AnsiCodeGenerator.Blink(false));
        if (s) sb.Append(AnsiCodeGenerator.Strikeout(false));
        if (u) sb.Append(AnsiCodeGenerator.Underline(false));
        if (i) sb.Append(AnsiCodeGenerator.Italic(false));
        if (b) sb.Append(AnsiCodeGenerator.Bold(false));
        if (background.Length > 1) sb.Append(AnsiCodeGenerator.Background(true));
        if (foreground.Length > 1) sb.Append(AnsiCodeGenerator.Foreground(true));
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(ConsoleText leftValue, string rightValue)
    {
        if (rightValue is null) return leftValue;
        var sb = new StringBuilder();
        StringExtensions.Append(sb, leftValue?.Content);
        sb.Append(rightValue);
        return new ConsoleText(sb, leftValue?.Style);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(string leftValue, ConsoleText rightValue)
    {
        if (leftValue is null) return rightValue;
        var sb = new StringBuilder();
        sb.Append(leftValue);
        StringExtensions.Append(sb, rightValue?.Content);
        return new ConsoleText(sb, rightValue?.Style);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(ConsoleText leftValue, StringBuilder rightValue)
    {
        if (rightValue is null) return leftValue;
        var sb = new StringBuilder();
        StringExtensions.Append(sb, leftValue?.Content);
        StringExtensions.Append(sb, rightValue);
        return new ConsoleText(sb, leftValue?.Style);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after AND operation.</returns>
    public static ConsoleText operator +(StringBuilder leftValue, ConsoleText rightValue)
    {
        if (leftValue is null) return rightValue;
        var sb = new StringBuilder();
        StringExtensions.Append(sb, leftValue);
        StringExtensions.Append(sb, rightValue?.Content);
        return new ConsoleText(sb, rightValue?.Style);
    }
}
