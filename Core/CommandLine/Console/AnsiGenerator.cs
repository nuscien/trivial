using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    /// <summary>
    /// Code generator of ANSI escape sequences.
    /// </summary>
    internal static class AnsiCodeGenerator
    {
        // https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences

        /// <summary>
        /// The sign of escape.
        /// </summary>
        public const string Esc = "\u001b";

        /// <summary>
        /// Gets the ANSI escape sequences to set the foreground color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Foreground(ConsoleColor? color)
            => color.HasValue ? color.Value switch
            {
                ConsoleColor.Black => $"{Esc}[30m",
                ConsoleColor.White => $"{Esc}[37m",
                ConsoleColor.DarkRed => $"{Esc}[31m",
                ConsoleColor.DarkGreen => $"{Esc}[32m",
                ConsoleColor.DarkYellow => $"{Esc}[33m",
                ConsoleColor.DarkBlue => $"{Esc}[34m",
                ConsoleColor.DarkMagenta => $"{Esc}[35m",
                ConsoleColor.DarkCyan => $"{Esc}[36m",
                ConsoleColor.DarkGray => $"{Esc}[90m",
                ConsoleColor.Red => $"{Esc}[91m",
                ConsoleColor.Green => $"{Esc}[92m",
                ConsoleColor.Yellow => $"{Esc}[93m",
                ConsoleColor.Blue => $"{Esc}[94m",
                ConsoleColor.Magenta => $"{Esc}[95m",
                ConsoleColor.Cyan => $"{Esc}[96m",
                ConsoleColor.Gray => $"{Esc}[97m",
                _ => string.Empty
            } : string.Empty;

        /// <summary>
        /// Gets the ANSI escape sequences to set the foreground color.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Foreground(byte r, byte g, byte b)
            => $"{Esc}[38;2;{r};{g};{b}m";

        /// <summary>
        /// Gets the ANSI escape sequences to set the foreground color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Foreground(System.Drawing.Color color)
            => $"{Esc}[38;2;{color.R};{color.G};{color.B}m";

        /// <summary>
        /// Gets the ANSI escape sequences to set the foreground color.
        /// </summary>
        /// <param name="reset">true if resets color; otherwise, false.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Foreground(bool reset)
            => reset ? $"{Esc}[39m" : string.Empty;

        /// <summary>
        /// Gets the ANSI escape sequences to set the background color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Background(ConsoleColor? color)
            => color.HasValue ? color.Value switch
            {
                ConsoleColor.Black => $"{Esc}[40m",
                ConsoleColor.White => $"{Esc}[47m",
                ConsoleColor.DarkRed => $"{Esc}[41m",
                ConsoleColor.DarkGreen => $"{Esc}[42m",
                ConsoleColor.DarkYellow => $"{Esc}[43m",
                ConsoleColor.DarkBlue => $"{Esc}[44m",
                ConsoleColor.DarkMagenta => $"{Esc}[45m",
                ConsoleColor.DarkCyan => $"{Esc}[46m",
                ConsoleColor.DarkGray => $"{Esc}[100m",
                ConsoleColor.Red => $"{Esc}[101m",
                ConsoleColor.Green => $"{Esc}[102m",
                ConsoleColor.Yellow => $"{Esc}[103m",
                ConsoleColor.Blue => $"{Esc}[104m",
                ConsoleColor.Magenta => $"{Esc}[105m",
                ConsoleColor.Cyan => $"{Esc}[106m",
                ConsoleColor.Gray => $"{Esc}[107m",
                _ => string.Empty
            } : string.Empty;

        /// <summary>
        /// Gets the ANSI escape sequences to set the background color.
        /// </summary>
        /// <param name="r">The red.</param>
        /// <param name="g">The green.</param>
        /// <param name="b">The blue.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Background(byte r, byte g, byte b)
            => $"{Esc}[48;2;{r};{g};{b}m";

        /// <summary>
        /// Gets the ANSI escape sequences to set the background color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Background(System.Drawing.Color color)
            => $"{Esc}[48;2;{color.R};{color.G};{color.B}m";

        /// <summary>
        /// Gets the ANSI escape sequences to set the foreground color.
        /// </summary>
        /// <param name="reset">true if resets color; otherwise, false.</param>
        /// <returns>The specific ANSI escape sequences.</returns>
        public static string Background(bool reset)
            => reset ? $"{Esc}[49m" : string.Empty;

        public static string AttributesOff() => $"{Esc}[0m";
        public static string Blink(bool enable) => enable ? $"{Esc}[5m" : $"{Esc}[25m";
        public static string Bold(bool enable) => enable ? $"{Esc}[1m" : $"{Esc}[22m";
        public static string TextHidden() => $"{Esc}[8m";
        public static string Reverse(bool enable) => enable ? $"{Esc}[7m" : $"{Esc}[27m";
        public static string Italic(bool enable) => enable ? $"{Esc}[3m" : $"{Esc}[23m";
        public static string Underline(bool enable) => enable ? $"{Esc}[4m" : $"{Esc}[24m";
        public static string Strikeout(bool enable) => enable ? $"{Esc}[9m" : $"{Esc}[29m";

        public static string MoveCursorX(int columns)
        {
            if (columns == 0) return string.Empty;
            return columns > 0 ? $"{Esc}[{columns}C" : $"{Esc}[{-columns}D";
        }

        public static string MoveCursorY(int lines)
        {
            if (lines == 0) return string.Empty;
            return lines > 0 ? $"{Esc}[{lines}B" : $"{Esc}[{-lines}A";
        }

        public static string MoveCursorBy(int x, int y)
            => $"{MoveCursorX(x)}{MoveCursorY(y)}";

        public static string MoveCursorNextLine(int line = 1) => $"{Esc}[{line}E";

        public static string MoveCursorTo(int? left = null, int? top = null) => $"{Esc}[{top ?? 1};{left ?? 1}H";

        public static string MoveCursorAt(int left = 0, int top = 0)
            => $"{Esc}[{top};{left}H";

        public static string CursorVisibility(bool show)
            => show ? $"{Esc}[?25h" : $"{Esc}[?25l";

        public static string ScrollBy(int step)
        {
            if (step == 0) return string.Empty;
            var r = step > 0 ? $"{Esc}[T" : $"{Esc}[S";
            var i = Math.Abs(step);
            if (i == 1) return r;
            var s = new StringBuilder(r);
            for (var j = 1; j < i; j++)
            {
                s.Append(r);
            }

            return s.ToString();
        }

        /// <summary>
        /// Code to clear lines.
        /// </summary>
        public static string Clear(ConsoleInterface.RelativeAreas area)
            => area switch
            {
                ConsoleInterface.RelativeAreas.Line => $"{Esc}[2K",
                ConsoleInterface.RelativeAreas.ToBeginningOfLine => $"{Esc}[1K",
                ConsoleInterface.RelativeAreas.ToEndOfLine => $"{Esc}[K",
                ConsoleInterface.RelativeAreas.EntireScreen => $"{Esc}[2J",
                ConsoleInterface.RelativeAreas.ToBeginningOfScreen => $"{Esc}[1J",
                ConsoleInterface.RelativeAreas.ToEndOfScreen => $"{Esc}[J",
                ConsoleInterface.RelativeAreas.EntireBuffer => $"{Esc}[3J",
                _ => string.Empty
            };

        public static string SaveCursorPosition() => $"{Esc}7";

        public static string RestoreCursorPosition() => $"{Esc}8";

        public static string Remove(int count = 1)
        {
            if (count == 0) return string.Empty;
            return count > 0 ? $"{Esc}[{count}X" : $"{Esc}[{-count}P";
        }

        //public static ConsoleColor ToConsoleColor(System.Drawing.Color color)
        //{
        //    return ToConsoleColor(color.R, color.G, color.B);
        //}

        //public static ConsoleColor ToConsoleColor(byte r, byte g, byte b)
        //{

        //    if (r >= 220 && g >= 220 && b >= 220)
        //        return ConsoleColor.White;
        //    if (r <= 48 && g <= 48 && b <= 48)
        //        return ConsoleColor.Black;

        //    if (r >= 192 || g >= 192 || b >= 192)
        //    {

        //    }

        //    var rank = new Maths.RankResult3<int>(r, g, b);

        //    if (r <= 220 && g <= 220 && b <= 220 && r >= 160 && g >= 160 && b >= 160)
        //        return ConsoleColor.Gray;
        //    if (r <= 160 && g <= 160 && b <= 160 && r >= 48 && g >= 48 && b >= 48)
        //        return ConsoleColor.DarkGray;
        //    if (r >= 192)
        //        return g >= 192 ? ConsoleColor.Yellow : ConsoleColor.Red;
        //    if (g >= 192)
        //        return b >= 192 ? ConsoleColor.Cyan : ConsoleColor.Green;
        //    if (b >= 192)
        //        return r >= 192 ? ConsoleColor.Magenta : ConsoleColor.Blue;
        //}
    }
}
