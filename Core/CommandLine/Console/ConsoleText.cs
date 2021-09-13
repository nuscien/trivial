using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The options for segment.
    /// </summary>
    public class ConsoleTextStyle
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
        public ConsoleTextStyle(Color? foreground, Color? background = null)
        {
            ForegroundRgbColor = foreground;
            BackgroundRgbColor = background;
        }

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
        /// Gets or sets the RGB color for foreground.
        /// </summary>
        public Color? ForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color for background.
        /// </summary>
        public Color? BackgroundRgbColor { get; set; }
        
        /// <summary>
        /// Gets or sets the console color for foreground.
        /// </summary>
        public ConsoleColor? ForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the console color for background.
        /// </summary>
        public ConsoleColor? BackgroundConsoleColor { get; set; }

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
            Content.Append(c, repeatCount);
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
            Content.Append(c, repeatCount);
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
        public ConsoleText(string s, Color foreground, Color? background = null)
        {
            Content = new StringBuilder(s);
            Style.ForegroundRgbColor = foreground;
            Style.BackgroundRgbColor = background;
            Style.ForegroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(foreground);
            if (background.HasValue) Style.BackgroundConsoleColor = AnsiCodeGenerator.ToConsoleColor(background.Value);
        }

        /// <summary>
        /// Initializes a new instance of the ConsoleText class.
        /// </summary>
        /// <param name="s">The text content.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        public ConsoleText(StringBuilder s, Color foreground, Color? background = null)
        {
            if (s != null) Content = s;
            Style.ForegroundRgbColor = foreground;
            Style.BackgroundRgbColor = background;
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
            Text.StringExtensions.Append(sb, Content);
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
            Text.StringExtensions.Append(sb, leftValue?.Content);
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
            Text.StringExtensions.Append(sb, rightValue?.Content);
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
            Text.StringExtensions.Append(sb, leftValue?.Content);
            Text.StringExtensions.Append(sb, rightValue);
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
            Text.StringExtensions.Append(sb, leftValue);
            Text.StringExtensions.Append(sb, rightValue?.Content);
            return new ConsoleText(sb, rightValue?.Style);
        }
    }
}
