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
            Content = s;
            if (style != null) Style = style;
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
            Content = s;
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
        }

        /// <summary>
        /// Initializes a new instance of the ConsoleText class.
        /// </summary>
        /// <param name="s">The text content.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        public ConsoleText(string s, Color foreground, Color background)
        {
            Content = new StringBuilder(s);
            Style.ForegroundRgbColor = foreground;
            Style.BackgroundRgbColor = background;
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
