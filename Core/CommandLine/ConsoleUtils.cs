using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The utilities.
    /// </summary>
    internal static class ConsoleUtilities
    {
        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
        public static void Backspace(int count = 1, bool doNotRemoveOutput = false)
        {
            var str = new StringBuilder();
            str.Append('\b', count);
            if (!doNotRemoveOutput)
            {
                str.Append(' ', count);
                str.Append('\b', count);
            }

            Console.Write(str.ToString());
        }

        /// <summary>
        /// Clear current line.
        /// </summary>
        /// <param name="row">The additional row index to clear; or null, if clear the line where the cursor is.</param>
        public static void ClearLine(int? row = null)
        {
            try
            {
                SetCursorPosition(Console.BufferWidth - 1, row ?? Console.CursorTop);
                Backspace(Console.CursorLeft);
            }
            catch (SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void Write(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var c = Console.ForegroundColor;
            SetColor(foregroundColor, false);
            if (arg.Length > 0) Console.Write(value, arg);
            else Console.Write(value);
            SetColor(c, false);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var fore = Console.ForegroundColor;
            var back = Console.BackgroundColor;
            SetColor(foregroundColor, false);
            SetColor(backgroundColor, true);
            if (arg.Length > 0) Console.Write(value, arg);
            else Console.Write(value);
            SetColor(fore, false);
            SetColor(back, true);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeat">The numbers of the charactor to repeat.</param>
        public static void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, char value, int repeat = 1)
        {
            if (repeat < 1) return;
            var fore = Console.ForegroundColor;
            var back = Console.BackgroundColor;
            SetColor(foregroundColor, false);
            SetColor(backgroundColor, true);
            for (var i = 0; i < repeat; i++)
            {
                Console.Write(value);
            }

            SetColor(fore, false);
            SetColor(back, true);
        }

        /// <summary>
        /// Writes the current line terminator, to the standard output stream.
        /// </summary>
        public static void WriteLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteLine(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, value, arg);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteLine(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, backgroundColor, value, arg);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a line and an empty line to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteDoubleLines(string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            Console.WriteLine(value, arg);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(IEnumerable<string> col)
        {
            if (col == null) return;
            foreach (var item in col)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(ConsoleColor foregroundColor, IEnumerable<string> col)
        {
            if (col == null) return;
            var c = Console.ForegroundColor;
            SetColor(foregroundColor, false);
            foreach (var item in col)
            {
                Console.WriteLine(item);
            }

            SetColor(c, false);
        }

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword()
        {
            return ReadPassword(null, null);
        }

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(char replaceChar)
        {
            return ReadPassword(replaceChar, null);
        }

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="foregroundColor">The replace charactor color.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(char? replaceChar, ConsoleColor? foregroundColor)
        {
            var str = new SecureString();
            while (true)
            {
                var key = Console.ReadKey(true);
                var len = str.Length;
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        return str;
                    case ConsoleKey.Escape:
                        str.Dispose();
                        return null;
                    case ConsoleKey.Backspace:
                        if (key.Modifiers == ConsoleModifiers.Shift || key.Modifiers == ConsoleModifiers.Control)
                        {
                            str.Clear();
                            if (replaceChar.HasValue) Backspace(len + 1);
                            break;
                        }

                        if (str.Length == 0) break;
                        Console.Write(' ');
                        str.RemoveAt(str.Length - 1);
                        Backspace();
                        if (replaceChar.HasValue) Backspace();
                        break;
                    case ConsoleKey.Delete:
                    case ConsoleKey.Clear:
                    case ConsoleKey.F5:
                        str.Clear();
                        if (replaceChar.HasValue) Backspace(len);
                        break;
                    default:
                        var hasKey = key.KeyChar != '\0';
                        if (hasKey) str.AppendChar(key.KeyChar);
                        if (hasKey && replaceChar.HasValue) Write(foregroundColor, null, replaceChar.Value);
                        break;
                }
            }
        }

        internal static void SetColor(ConsoleColor color, bool background)
        {
            try
            {
                if (background) Console.BackgroundColor = color;
                else Console.ForegroundColor = color;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
        }

        internal static void SetColor(ConsoleColor? color, bool background)
        {
            if (color.HasValue) SetColor(color.Value, background);
        }

        private static void SetCursorPosition(int left, int top)
        {
            try
            {
                Console.SetCursorPosition(left, top);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (Console.BufferHeight > top) throw;
#pragma warning disable CA1416
                try
                {
                    Console.BufferHeight = top + 64;
                }
                catch (OverflowException)
                {
                    Console.BufferHeight = int.MaxValue;
                }
                catch (PlatformNotSupportedException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
#pragma warning restore CA1416

                Console.SetCursorPosition(left, top);
            }
        }
    }
}
