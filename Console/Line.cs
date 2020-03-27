using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// A client to control a line to write and read.
    /// </summary>
    public class Line
    {
        /// <summary>
        /// The element to output.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Initializes a new instance of the LineWriter.Item class.
            /// </summary>
            public Item()
            {
            }

            /// <summary>
            /// Initializes a new instance of the LineWriter.Item class.
            /// </summary>
            /// <param name="value">The string to write.</param>
            /// <param name="arg">An array of objects to write using format.</param>
            public Item(string value, params object[] arg)
            {
                Value.AppendFormat(value, arg);
            }

            /// <summary>
            /// Initializes a new instance of the LineWriter.Item class.
            /// </summary>
            /// <param name="foregroundColor">The foreground color of the console.</param>
            /// <param name="value">The string to write.</param>
            /// <param name="arg">An array of objects to write using format.</param>
            public Item(ConsoleColor foregroundColor, string value, params object[] arg)
            {
                Value.AppendFormat(value, arg);
                ForegroundColor = foregroundColor;
            }

            /// <summary>
            /// Gets or sets the foreground color.
            /// </summary>
            public ConsoleColor? ForegroundColor { get; set; }

            /// <summary>
            /// Gets or sets the background color.
            /// </summary>
            public ConsoleColor? BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the string.
            /// </summary>
            public StringBuilder Value { get; } = new StringBuilder();
        }

        private int initLeft = -1;
        private int initTop = -1;
        private int curLeft = -1;
        private int curTop = -1;

        /// <summary>
        /// The value writen.
        /// </summary>
        private readonly StringBuilder line = new StringBuilder();

        /// <summary>
        /// A value indicating whether write immediately.
        /// </summary>
        private bool autoFlush = true;

        /// <summary>
        /// Gets the queue of the value pending to write.
        /// </summary>
        public List<Item> Pending { get; } = new List<Item>();

        /// <summary>
        /// Gets the line index writen by this instance.
        /// </summary>
        public int LineIndex { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether write to standard output stream immediately.
        /// </summary>
        public bool AutoFlush
        {
            get
            {
                return autoFlush;
            }

            set
            {
                autoFlush = value;
                if (autoFlush) Flush();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether need remeber the cursor position and set back when it has moved.
        /// </summary>
        public bool RememberCursorPosition { get; set; }

        /// <summary>
        /// Gets the length of the current line output.
        /// This does not contain the pending value.
        /// </summary>
        public int FlushedLength => line.Length;

        /// <summary>
        /// Gets the length of the pending value.
        /// </summary>
        public int PendingLength
        {
            get
            {
                var count = 0;
                foreach (var item in Pending)
                {
                    count += item.Value.Length;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the length of the current line output and the pending value.
        /// </summary>
        public int Length => line.Length + PendingLength;

        /// <summary>
        /// Moves cursor to its latest position.
        /// </summary>
        public void MoveToLatestCursorPosition()
        {
            if (curTop != System.Console.CursorTop) System.Console.CursorTop = curTop;
            if (curLeft != System.Console.CursorLeft) System.Console.CursorLeft = curLeft;
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void Write(string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var str = arg.Length > 0 ? string.Format(value, arg) : value;
            if (AutoFlush)
            {
                line.Append(str);
                InitPosition();
                System.Console.Write(str);
                SaveCursorPosition();
            }
            else
            {
                var last = GetLastPendingItem();
                if (last == null) Pending.Add(new Item(value));
            }
        }

        /// <summary>
        /// Writes the specified character to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(char value, int repeatCount = 1)
        {
            Write(null, null, value, repeatCount);
        }

        /// <summary>
        /// Writes the specified character to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(ConsoleColor? foregroundColor, char value, int repeatCount = 1)
        {
            Write(foregroundColor, null, value, repeatCount);
        }

        /// <summary>
        /// Writes the specified character to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, char value, int repeatCount = 1)
        {
            if (repeatCount < 1) return;
            if (repeatCount == 1)
            {
                Write(foregroundColor, backgroundColor, value.ToString());
                return;
            }

            var sb = new StringBuilder();
            sb.Append(value, repeatCount);
            Write(foregroundColor, backgroundColor, sb.ToString());
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                Write(null, null, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            Write(null, null, str);
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(ConsoleColor? foregroundColor, char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                Write(foregroundColor, null, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            Write(foregroundColor, null, str);
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                Write(foregroundColor, backgroundColor, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            Write(foregroundColor, backgroundColor, str);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(int value)
        {
            Write(null, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, int value)
        {
            Write(foregroundColor, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, int value)
        {
            Write(foregroundColor, backgroundColor, value.ToString());
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(long value)
        {
            Write(null, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, long value)
        {
            Write(foregroundColor, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, long value)
        {
            Write(foregroundColor, backgroundColor, value.ToString());
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(double value)
        {
            Write(null, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, double value)
        {
            Write(foregroundColor, null, value);
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, double value)
        {
            Write(foregroundColor, backgroundColor, value.ToString());
        }

        /// <summary>
        /// Writes the specified string to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(StringBuilder value)
        {
            Write(null, null, value);
        }

        /// <summary>
        /// Writes the specified string to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, StringBuilder value)
        {
            Write(foregroundColor, null, value);
        }

        /// <summary>
        /// Writes the specified string to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, StringBuilder value)
        {
            Write(foregroundColor, backgroundColor, value.ToString());
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void Write(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var str = arg.Length > 0 ? string.Format(value, arg) : value;
            if (AutoFlush)
            {
                line.Append(str);
                InitPosition();
                LineUtilities.Write(foregroundColor, str);
                SaveCursorPosition();
            }
            else
            {
                var last = GetLastPendingItem();
                if (last == null) Pending.Add(new Item(foregroundColor, value));
            }
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var str = arg.Length > 0 ? string.Format(value, arg) : value;
            if (AutoFlush)
            {
                line.Append(str);
                InitPosition();
                LineUtilities.Write(foregroundColor, backgroundColor, str);
                SaveCursorPosition();
            }
            else
            {
                var last = GetLastPendingItem();
                if (last == null) Pending.Add(new Item(value)
                {
                    ForegroundColor = foregroundColor,
                    BackgroundColor = backgroundColor
                });
            }
        }

        /// <summary>
        /// Writes a secure string to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor? foregroundColor, ConsoleColor backgroundColor, SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(foregroundColor, backgroundColor, LineUtilities.ToUnsecureString(value));
        }

        /// <summary>
        /// Writes a secure string to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void Write(ConsoleColor foregroundColor, SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(foregroundColor, LineUtilities.ToUnsecureString(value));
        }

        /// <summary>
        /// Writes a secure string to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(LineUtilities.ToUnsecureString(value));
        }

        /// <summary>
        /// Writes the current line terminator in given count, to the standard output stream.
        /// </summary>
        /// <param name="count">The line count.</param>
        public void WriteEmptyLines(int count)
        {
            ClearPending();
            for (var i = 0; i < count; i++)
            {
                End(true);
            }
        }

        /// <summary>
        /// Writes the current line terminator, to the standard output stream.
        /// </summary>
        public void WriteLine()
        {
            End(true);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void WriteLine(string value, params object[] arg)
        {
            Write(value, arg);
            End();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void WriteLine(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, value, arg);
            End();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void WriteLine(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, backgroundColor, value, arg);
            End();
        }

        /// <summary>
        /// Writes a progress component after ending the current pending output, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="options">The options.</param>
        public ProgressLineResult WriteLine(string caption, ProgressLineOptions options)
        {
            End();
            var result = LineUtilities.WriteLine(caption, options);
            initTop = System.Console.CursorTop;
            curTop = System.Console.CursorTop;
            curLeft = System.Console.CursorLeft;
            return result;
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                WriteLine(null, null, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            WriteLine(null, null, str);
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(ConsoleColor? foregroundColor, char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                WriteLine(foregroundColor, null, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            WriteLine(foregroundColor, null, str);
        }

        /// <summary>
        /// Writes the specified character, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteLine(char value, int repeatCount = 1)
        {
            Write(null, null, value, repeatCount);
            End();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteLine(int value)
        {
            Write(value);
            End();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteLine(long value)
        {
            Write(value);
            End();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteLine(double value)
        {
            Write(value);
            End();
        }

        /// <summary>
        /// Writes a secure string, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteLine(SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(LineUtilities.ToUnsecureString(value));
            End();
        }

        /// <summary>
        /// Writes a secure string, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void WriteLine(ConsoleColor foregroundColor, SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(foregroundColor, LineUtilities.ToUnsecureString(value));
            End();
        }

        /// <summary>
        /// Writes a secure string, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        public void WriteLine(ConsoleColor? foregroundColor, ConsoleColor backgroundColor, SecureString value)
        {
            if (value == null || value.Length == 0) return;
            Write(foregroundColor, backgroundColor, LineUtilities.ToUnsecureString(value));
            End();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, char[] value, int start = 0, int? count = null)
        {
            if (start == 0 && count == null)
            {
                WriteLine(foregroundColor, backgroundColor, new string(value));
            }

            var list = value.Skip(start);
            if (count.HasValue) list = list.Take(count.Value);
            var str = new string(list.ToArray());
            WriteLine(foregroundColor, backgroundColor, str);
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public void WriteLines(IEnumerable<string> col)
        {
            if (col == null)
            {
                End();
                return;
            }

            foreach (var item in col)
            {
                Write(item);
                End(true);
            }
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public void WriteLines(ConsoleColor foregroundColor, IEnumerable<string> col)
        {
            if (col == null)
            {
                End();
                return;
            }

            var c = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            foreach (var item in col)
            {
                Write(item);
                End(true);
            }

            System.Console.ForegroundColor = c;
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        /// <param name="converter">A string converter.</param>
        public void WriteLines<T>(IEnumerable<T> col, Func<T, string> converter)
        {
            if (col == null)
            {
                End();
                return;
            }

            if (converter == null) converter = ele => ele?.ToString();
            foreach (var item in col.Select(converter))
            {
                Write(item);
                End(true);
            }
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <param name="question">The optional question message to output.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine(string question = null)
        {
            Write(question);
            Flush();
            LineIndex++;
            initLeft = -1;
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <param name="question">The question message to output.</param>
        /// <param name="questionColor">The question message color.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine(string question, ConsoleColor questionColor)
        {
            Write(questionColor, question);
            Flush();
            LineIndex++;
            initLeft = -1;
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <param name="retry">The maximum retry count for empty.</param>
        /// <param name="prefix">The prefix text to output.</param>
        /// <param name="prefixColor">The prefix text color.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine(int retry, string prefix = null, ConsoleColor? prefixColor = null)
        {
            for (var i = 0; i < retry; i++)
            {
                if (prefixColor.HasValue) Write(prefixColor.Value, prefix);
                else Write(prefix);
                Flush();
                LineIndex++;
                initLeft = -1;
                var str = System.Console.ReadLine();
                if (!string.IsNullOrEmpty(str)) return str;
            }

            return string.Empty;
        }

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// </summary>
        /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
        /// <returns>
        /// An object that describes the System.ConsoleKey constant and Unicode character, if any, that correspond to the pressed console key.
        /// The object also describes, in a bitwise combination of System.ConsoleModifiers values, whether one or more Shift, Alt, or Ctrl modifier keys was pressed simultaneously with the console key.
        /// </returns>
        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            Flush();
            var key = System.Console.ReadKey(intercept);
            if (key.Key == ConsoleKey.Enter)
            {
                LineIndex++;
                initLeft = -1;
            }
            else
            {
                SaveCursorPosition();
            }

            return key;
        }

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <returns>
        /// The password.
        /// </returns>
        public SecureString ReadPassword()
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
        public SecureString ReadPassword(char replaceChar)
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
        public SecureString ReadPassword(char? replaceChar, ConsoleColor? foregroundColor)
        {
            Flush();
            var str = new SecureString();
            while (true)
            {
                var key = System.Console.ReadKey(true);
                var len = str.Length;
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        LineIndex++;
                        initLeft = -1;
                        return str;
                    case ConsoleKey.Escape:
                        str.Dispose();
                        LineIndex++;
                        initLeft = -1;
                        return null;
                    case ConsoleKey.Backspace:
                        if (key.Modifiers == ConsoleModifiers.Shift || key.Modifiers == ConsoleModifiers.Control)
                        {
                            str.Clear();
                            if (replaceChar.HasValue) Backspace(len + 1);
                            break;
                        }

                        SaveCursorPosition();
                        Write('\0');
                        if (str.Length == 0) break;
                        str.RemoveAt(str.Length - 1);
                        Backspace();
                        if (replaceChar.HasValue) Backspace();
                        break;
                    case ConsoleKey.Delete:
                    case ConsoleKey.Clear:
                    case ConsoleKey.F5:
                        str.Clear();
                        SaveCursorPosition();
                        if (replaceChar.HasValue) Backspace(len + 1);
                        break;
                    default:
                        var hasKey = key.KeyChar != '\0';
                        if (hasKey) str.AppendChar(key.KeyChar);
                        SaveCursorPosition();
                        if (hasKey && replaceChar.HasValue) Write(foregroundColor, replaceChar.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the foreground and background console colors to their defaults.
        /// </summary>
        public void ResetColor()
        {
            System.Console.ResetColor();
        }

        /// <summary>
        /// Writes the pending string and the current line terminator to the standard output stream.
        /// And then reset the pending information in this instance for the new line.
        /// </summary>
        /// <param name="evenEmpty">true if still write a new line when there is nothing output; otherwise, false.</param>
        public void End(bool evenEmpty = false)
        {
            Flush();
            initLeft = -1;
            if (line.Length < 1 && !evenEmpty) return;
            System.Console.ResetColor();
            System.Console.WriteLine();
            LineIndex++;
            line.Clear();
            initTop = System.Console.CursorTop;
            curTop = System.Console.CursorTop;
            curLeft = System.Console.CursorLeft;
        }

        /// <summary>
        /// Clears the pending string.
        /// </summary>
        public void ClearPending()
        {
            Pending.Clear();
        }

        /// <summary>
        /// Clears the pending string and flushed string.
        /// </summary>
        public void Clear()
        {
            Pending.Clear();
            if (initLeft >= 0) InitPosition();
            LineUtilities.Backspace(line.Length);
            if (initLeft >= 0)
            {
                if (System.Console.CursorTop == initTop && System.Console.CursorLeft > initLeft)
                {
                    LineUtilities.Backspace(System.Console.CursorLeft - initLeft);
                }

                SaveCursorPosition();
            }

            line.Clear();
        }

        /// <summary>
        /// Flushes the pending string.
        /// </summary>
        public void Flush()
        {
            var str = new StringBuilder();
            InitPosition();
            foreach (var item in Pending)
            {
                var itemStr = item.Value.ToString();
                str.Append(itemStr);
                if (item != null) LineUtilities.Write(item.ForegroundColor, item.BackgroundColor, itemStr);
            }

            SaveCursorPosition();
            Pending.Clear();
            line.Append(str);
        }

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        public void Backspace(int count = 1)
        {
            if (count <= 0) return;
            Pending.RemoveAll(item => item == null);
            while (count > 0 && Pending.Count > 0)
            {
                var last = Pending[Pending.Count - 1]?.Value;
                if (last != null && last.Length >= count)
                {
                    last.Remove(last.Length - count, count);
                    return;
                }

                count -= last.Length;
                Pending.RemoveAt(Pending.Count - 1);
            }

            if (count <= 0) return;
            if (line.Length >= count)
            {
                line.Remove(line.Length - count, count);
            }
            else
            {
                line.Clear();
            }

            InitPosition();
            LineUtilities.Backspace(count);
            SaveCursorPosition();
        }

        /// <summary>
        /// Converts the value in pending queue to the string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public string ToPendingString()
        {
            var str = new StringBuilder();
            foreach (var item in Pending)
            {
                if (item != null) str.Append(item.ToString());
            }

            return str.ToString();
        }

        /// <summary>
        /// Converts the value of this instance to the string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Converts the value of this instance to the string.
        /// </summary>
        /// <param name="onlyFlushed">true if only for the value flushed; otherwise, false.</param>
        /// <returns>A string whose value is the same as this instance.</returns>
        public string ToString(bool onlyFlushed)
        {
            var str = line.ToString();
            if (onlyFlushed) return str;
            str += ToPendingString();
            return str;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Gets the last item in pending queue.
        /// </summary>
        /// <returns>The last item in pending queue.</returns>
        private Item GetLastPendingItem()
        {
            Pending.RemoveAll(item => item == null);
            return Pending.Count > 0 ? Pending[Pending.Count - 1] : null;
        }

        /// <summary>
        /// Pre-handles before to write anything.
        /// </summary>
        private void InitPosition()
        {
            try
            {
                if (initLeft >= 0)
                {
                    if (RememberCursorPosition)
                    {
                        if (curTop != System.Console.CursorTop) System.Console.CursorTop = curTop;
                        if (curLeft != System.Console.CursorLeft) System.Console.CursorLeft = curLeft;
                    }

                    return;
                }

                initLeft = System.Console.CursorLeft;
                initTop = System.Console.CursorTop;
                if (System.Console.CursorTop != curTop) return;
                if (System.Console.CursorLeft != curLeft) System.Console.CursorLeft = curLeft;
                return;
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

            initLeft = -1;
        }

        /// <summary>
        /// Post-handles before to write anything.
        /// </summary>
        private void SaveCursorPosition()
        {
            try
            {
                curLeft = System.Console.CursorLeft;
                curTop = System.Console.CursorTop;
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
    }

    /// <summary>
    /// The utilities.
    /// </summary>
    public static class LineUtilities
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
                str.Append('\0', count);
                str.Append('\b', count);
            }

            System.Console.Write(str.ToString());
        }

        /// <summary>
        /// Clear current line.
        /// </summary>
        /// <param name="row">The additional row index to clear; or null, if clear the line where the cursor is.</param>
        public static void ClearLine(int? row = null)
        {
            try
            {
                System.Console.SetCursorPosition(System.Console.BufferWidth - 1, row ?? System.Console.CursorTop);
                Backspace(System.Console.CursorLeft);
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
            var c = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            if (arg.Length > 0) System.Console.Write(value, arg);
            else System.Console.Write(value);
            System.Console.ForegroundColor = c;
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
            var fore = System.Console.ForegroundColor;
            var back = System.Console.BackgroundColor;
            if (foregroundColor.HasValue) System.Console.ForegroundColor = foregroundColor.Value;
            if (backgroundColor.HasValue) System.Console.BackgroundColor = backgroundColor.Value;
            if (arg.Length > 0) System.Console.Write(value, arg);
            else System.Console.Write(value);
            if (foregroundColor.HasValue) System.Console.ForegroundColor = fore;
            if (backgroundColor.HasValue) System.Console.BackgroundColor = back;
        }

        /// <summary>
        /// Writes the current line terminator, to the standard output stream.
        /// </summary>
        public static void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes a progress component, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="options">The options.</param>
        public static ProgressLineResult WriteLine(string caption, ProgressLineOptions options)
        {
            if (options == null) options = ProgressLineOptions.Empty;
            var winWidth = 70;
            try
            {
                winWidth = System.Console.WindowWidth;
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }

            var width = options.Size switch
            {
                ProgressLineOptions.Sizes.None => 0,
                ProgressLineOptions.Sizes.Short => winWidth > 70 ? 20 : 10,
                ProgressLineOptions.Sizes.Wide => winWidth > 88 ? 60 : 40,
                _ => winWidth > 70 ? (winWidth > 88 ? 40 : 30) : 20
            };

            var sb = new StringBuilder();
            var barChar = options.Style switch
            {
                ProgressLineOptions.Styles.AngleBracket => '>',
                ProgressLineOptions.Styles.Plus => '+',
                ProgressLineOptions.Styles.Sharp => '#',
                ProgressLineOptions.Styles.X => options.UsePendingBackgroundForAll ? 'X' : 'x',
                ProgressLineOptions.Styles.O => options.UsePendingBackgroundForAll ? 'O' : 'o',
                _ => ' ',
            };
            var pendingChar = options.UsePendingBackgroundForAll ? ' ' : (options.Style switch
            {
                ProgressLineOptions.Styles.AngleBracket => '=',
                ProgressLineOptions.Styles.Plus => '-',
                ProgressLineOptions.Styles.Sharp => '-',
                ProgressLineOptions.Styles.X => '.',
                ProgressLineOptions.Styles.O => '.',
                _ => ' ',
            });
            sb.Append(pendingChar, width);
            var fore = System.Console.ForegroundColor;
            var back = System.Console.BackgroundColor;
            var background = options.BackgroundColor ?? back;
            var barBackground = background;
            var progressBackground = background;
            if (barChar == ' ')
            {
                barBackground = options.BarColor;
                progressBackground = options.PendingColor;
            }
            else if (pendingChar == ' ')
            {
                progressBackground = barBackground = options.PendingColor;
            }

            if (!string.IsNullOrEmpty(caption))
            {
                if (options.CaptionColor.HasValue) System.Console.ForegroundColor = options.CaptionColor.Value;
                System.Console.BackgroundColor = background;
                System.Console.Write(caption);
                if (!options.IgnoreCaptionSeparator && caption[caption.Length - 1] != ' ') System.Console.Write(' ');
            }

            var left = System.Console.CursorLeft;
            var top = System.Console.CursorTop;
            if (options.Size == ProgressLineOptions.Sizes.Full)
            {
                width = System.Console.WindowWidth - left - 6;
                sb.Clear();
                sb.Append(pendingChar, width);
            }

            var progress = new ProgressLineResult();
            System.Console.ForegroundColor = options.PendingColor;
            System.Console.BackgroundColor = progressBackground;
            System.Console.Write(sb.ToString());
            System.Console.ForegroundColor = options.ValueColor ?? fore;
            System.Console.BackgroundColor = background;
            System.Console.WriteLine(" ... ");
            System.Console.ForegroundColor = fore;
            System.Console.BackgroundColor = back;
            var locker = new object();
            progress.ProgressChanged += (sender, ev) =>
            {
                var left2 = System.Console.CursorLeft;
                var top2 = System.Console.CursorTop;
                var fore2 = System.Console.ForegroundColor;
                var back2 = System.Console.BackgroundColor;
                if (width > 0)
                {
                    var w = (int)Math.Round(width * ev);
                    var sb2 = new StringBuilder();
                    sb2.Append(barChar, w);
                    var sb3 = new StringBuilder();
                    sb3.Append(pendingChar, width - w);
                    System.Console.CursorLeft = left;
                    System.Console.CursorTop = top;
                    System.Console.ForegroundColor = progress.IsFailed ? options.ErrorColor : options.BarColor;
                    System.Console.BackgroundColor = progress.IsFailed ? options.ErrorColor : barBackground;
                    System.Console.Write(sb2.ToString());
                    System.Console.ForegroundColor = options.PendingColor;
                    System.Console.BackgroundColor = progressBackground;
                    System.Console.Write(sb3.ToString());
                    System.Console.ForegroundColor = options.ValueColor ?? fore2;
                    System.Console.BackgroundColor = options.BackgroundColor ?? back2;
                    System.Console.Write(' ');
                }
                else
                {
                    System.Console.CursorLeft = left;
                    System.Console.CursorTop = top;
                    System.Console.ForegroundColor = options.ValueColor ?? fore2;
                    System.Console.BackgroundColor = options.BackgroundColor ?? back2;
                }

                System.Console.Write(ev.ToString("#0%"));
                System.Console.CursorLeft = left2;
                System.Console.CursorTop = top2;
                System.Console.ForegroundColor = fore2;
                System.Console.BackgroundColor = back2;
            };
            return progress;
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
            System.Console.WriteLine();
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
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes a line and an empty line to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteDoubleLines(string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            System.Console.WriteLine(value, arg);
            System.Console.WriteLine();
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
                System.Console.WriteLine(item);
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
            var c = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            foreach (var item in col)
            {
                System.Console.WriteLine(item);
            }

            System.Console.ForegroundColor = c;
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<object> Select(SelectionData collection, SelectionOptions options = null)
        {
            return Select<object>(collection, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="convert">The converter.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(IEnumerable<T> collection, Func<T, SelectionItem<T>> convert, SelectionOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection.Select(convert));
            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(IEnumerable<SelectionItem<T>> collection, SelectionOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection);
            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="path">The parent foler path.</param>
        /// <param name="options">The selection display options.</param>
        /// <param name="searchPattern">The search string to match against the names of directories and files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="ArgumentException">searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars method.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, SelectionOptions options = null, string searchPattern = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            var col = string.IsNullOrEmpty(searchPattern) ? path.GetFileSystemInfos() : path.GetFileSystemInfos(searchPattern);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="path">The parent foler path.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, Func<FileSystemInfo, bool> predicate, SelectionOptions options = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            IEnumerable<FileSystemInfo> col = path.GetFileSystemInfos();
            if (predicate != null) col = col.Where(predicate);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(SelectionData<T> collection, SelectionOptions options = null)
        {
            if (collection == null) return null;
            if (options == null) options = new SelectionOptions();
            var maxWidth = System.Console.BufferWidth;
            var itemLen = options.Column.HasValue ? (int)Math.Floor(maxWidth * 1.0 / options.Column.Value) : maxWidth;
            if (options.MaxLength.HasValue) itemLen = Math.Min(options.MaxLength.Value, itemLen);
            if (options.MinLength.HasValue) itemLen = Math.Max(options.MinLength.Value, itemLen);
            var columns = (int)Math.Floor(maxWidth * 1.0 / itemLen);
            if (options.Column.HasValue && columns > options.Column.Value) columns = options.Column.Value;
            var itemLen2 = Math.Max(1, itemLen - 1);
            if (System.Console.CursorLeft > 0) System.Console.WriteLine();
            var top = System.Console.CursorTop;
            var offset = 0;
            var selected = 0;
            var oldSelected = -1;
            var list = collection.CopyList();
            var keys = new Dictionary<char, SelectionItem<T>>();
            foreach (var item in list)
            {
                if (!item.Hotkey.HasValue) continue;
                keys[item.Hotkey.Value] = item;
            }

            list = list.Where(item =>
            {
                return !string.IsNullOrEmpty(item.Title ?? item.Value);
            }).ToList();
            if (list.Count == 0 || itemLen < 1) return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
            var pageSize = options.MaxRow.HasValue ? options.MaxRow.Value * columns : list.Count;
            var prefix = options.Prefix;
            var selectedPrefix = options.SelectedPrefix;
            var tips = options.Tips;
            var tips2 = options.TipsLine2;
            var tipsP = options.PagingTips;
            var question = options.Question;
            var questionM = options.ManualQuestion;
            var fore = options.ForegroundColor;
            var back = options.BackgroundColor;
            var foreSel = options.SelectedForegroundColor;
            var backSel = options.SelectedBackgroundColor;
            var foreQ = options.QuestionForegroundColor;
            var backQ = options.QuestionBackgroundColor;
            var foreTip = options.TipsForegroundColor;
            var backTip = options.TipsBackgroundColor;
            var forePag = options.PagingForegroundColor;
            var backPag = options.PagingBackgroundColor;
            var foreDef = options.DefaultValueForegroundColor;
            var backDef = options.DefaultValueBackgroundColor;
            var inputTop = -1;
            var inputLeft = -1;
            if (!fore.HasValue && !back.HasValue && !foreSel.HasValue && !backSel.HasValue && prefix == null && selectedPrefix == null)
            {
                foreSel = System.Console.BackgroundColor;
                backSel = System.Console.ForegroundColor;
            }

            void change(int selIndex)
            {
                var isSel = selIndex == selected;
                if (selIndex < 0) selIndex = selected;
                var selItem = list[selIndex];
                var k = selIndex - offset;
                var rowI = (int)Math.Floor(k * 1.0 / columns);
                var curLeft = (k % columns) * itemLen;
                var str = ((isSel ? selectedPrefix : prefix) ?? string.Empty) + (selItem.Title ?? selItem.Value);
                if (str.Length > itemLen2) str = str.Substring(0, itemLen2);
                var curLeft2 = curLeft + itemLen2;
                var curLeftDiff = itemLen2;
                System.Console.SetCursorPosition(curLeft, top + rowI);
                var strOffset = (int)Math.Floor(str.Length * 1.0 / 2);
                if (isSel) Write(foreSel, backSel, str.Substring(0, strOffset));
                else Write(fore, back, str.Substring(0, strOffset));
                for (var i = strOffset; i < str.Length; i++)
                {
                    curLeftDiff = System.Console.CursorLeft - curLeft2;
                    if (curLeftDiff > 0)
                    {
                        Backspace(curLeftDiff);
                        break;
                    }
                    else if (curLeftDiff == 0)
                    {
                        break;
                    }

                    var charactor = str[i];
                    if (isSel) Write(foreSel, backSel, charactor.ToString());
                    else Write(fore, back, charactor.ToString());
                }

                curLeftDiff = System.Console.CursorLeft - curLeft2;
                if (curLeftDiff < 0)
                {
                    var spaces = new StringBuilder();
                    spaces.Append('\0', -curLeftDiff);
                    if (isSel) Write(foreSel, backSel, spaces.ToString());
                    else Write(fore, back, spaces.ToString());
                }

                if (inputTop >= 0) System.Console.SetCursorPosition(inputLeft, inputTop);
            }

            void select()
            {
                if (oldSelected != selected)
                {
                    if (selected < 0) selected = 0;
                    else if (selected >= list.Count) selected = list.Count - 1;
                    if (selected < offset || selected >= offset + pageSize || inputTop < 0)
                    {
                        render();
                        return;
                    }

                    if (oldSelected >= 0 && oldSelected >= offset && oldSelected < offset + pageSize) change(oldSelected);
                    change(selected);
                }

                System.Console.SetCursorPosition(inputLeft, inputTop);
                var sel = list[selected];
                if (question != null) Write(foreDef, backDef, sel.Value);
                oldSelected = selected;
            }

            void render()
            {
                if (selected < 0) selected = 0;
                else if (selected >= list.Count) selected = list.Count - 1;
                if (selected < offset || selected >= offset + pageSize) offset = (int)Math.Floor(selected * 1.0 / pageSize) * pageSize;
                var k = 0;
                System.Console.SetCursorPosition(0, top);
                ClearLine();
                SelectionData.Some(list, (item, i, j) =>
                {
                    if (j >= pageSize) return true;
                    k = j;
                    var str = ((selected == i ? selectedPrefix : prefix) ?? string.Empty) + (item.Title ?? item.Value);
                    var index = j % columns;
                    var row = (int)Math.Floor(j * 1.0 / columns);
                    if (str.Length > itemLen2) str = str.Substring(0, itemLen2);
                    var curLeft = index * itemLen;
                    var curLeft2 = curLeft + itemLen2;
                    var curTop = top + row;
                    System.Console.SetCursorPosition(curLeft, curTop);
                    if (selected == i) Write(foreSel, backSel, str);
                    else Write(fore, back, str);
                    if (System.Console.CursorTop != curTop)
                    {
                        Backspace(System.Console.CursorLeft);
                        System.Console.SetCursorPosition(maxWidth - 1, curTop);
                    }

                    if (System.Console.CursorLeft > curLeft2)
                    {
                        Backspace(System.Console.CursorLeft - curLeft2);
                    }
                    else if (System.Console.CursorLeft < curLeft2)
                    {
                        var spaces = new StringBuilder();
                        spaces.Append('\0', curLeft2 - System.Console.CursorLeft);
                        if (selected == i) Write(foreSel, backSel, spaces.ToString());
                        else Write(fore, back, spaces.ToString());
                    }

                    if (index == columns - 1)
                    {
                        System.Console.WriteLine();
                        ClearLine();
                    }

                    return false;
                }, offset);
                if ((k + 1) % columns > 0)
                {
                    System.Console.WriteLine();
                    ClearLine();
                }

                if (tipsP != null && pageSize < list.Count)
                {
                    WriteLine(fore, back, tipsP
                        .Replace("{from}", (offset + 1).ToString())
                        .Replace("{end}", (offset + k + 1).ToString())
                        .Replace("{count}", k.ToString())
                        .Replace("{size}", pageSize.ToString())
                        .Replace("{total}", list.Count.ToString()));
                    ClearLine();
                }

                if (inputTop > 0)
                {
                    var curTop = System.Console.CursorTop;
                    for (var i = inputTop; i >= curTop; i--)
                    {
                        ClearLine(i);
                    }
                }

                if (question != null) Write(foreQ, backQ, question);
                inputTop = System.Console.CursorTop;
                inputLeft = System.Console.CursorLeft;
                var sel = list[selected];
                if (question != null) Write(foreDef, backDef, sel.Value);
                oldSelected = selected;
            }

            var tipsTop = -1;
            var tipsTop2 = -1;
            void showTips()
            {
                if (string.IsNullOrWhiteSpace(tips)) return;
                var curTop = System.Console.CursorTop;
                var curLeft = System.Console.CursorLeft;
                System.Console.WriteLine();
                System.Console.WriteLine();
                tipsTop = System.Console.CursorTop;
                if (string.IsNullOrWhiteSpace(tips2))
                {
                    Write(foreTip, backTip, tips);
                }
                else
                {
                    WriteLine(foreTip, backTip, tips);
                    Write(foreTip, backTip, tips2);
                }

                tipsTop2 = Math.Max(tipsTop, System.Console.CursorTop);
                System.Console.SetCursorPosition(curLeft, curTop);
            }

            render();
            showTips();

            while (true)
            {
                var key = System.Console.ReadKey();
                if (inputTop > 0 && System.Console.CursorTop >= inputTop)
                {
                    for (var i = System.Console.CursorTop; i > inputTop; i--)
                    {
                        ClearLine(i);
                    }

                    if (inputLeft >= 0 && System.Console.CursorLeft != inputLeft)
                    {
                        System.Console.SetCursorPosition(maxWidth - 1, inputTop);
                        Backspace(System.Console.CursorLeft - inputLeft);
                    }
                }

                if (tipsTop > 0)
                {
                    for (var i = tipsTop; i <= tipsTop2; i++)
                    {
                        ClearLine(i);
                    }

                    tipsTop = -1;
                }

                if (inputTop > 0) System.Console.SetCursorPosition(inputLeft, inputTop);
                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Select)
                {
                    var sel = list[selected];
                    if (question != null) System.Console.WriteLine(sel.Value);
                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete || key.Key == ConsoleKey.Clear)
                {
                    if (questionM == null)
                    {
                        change(-1);
                        if (question != null) System.Console.WriteLine();
                        return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
                    }

                    change(-1);
                    Backspace(System.Console.CursorLeft);
                    Write(foreQ, backQ, questionM);
                    var inputStr = System.Console.ReadLine();
                    return new SelectionResult<T>(inputStr, SelectionResultTypes.Typed);
                }
                else if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Pause)
                {
                    change(-1);
                    if (question != null) System.Console.WriteLine();
                    return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
                }
                else if (key.Key == ConsoleKey.F1)
                {
                    if (keys.ContainsKey('?'))
                    {
                        change(-1);
                        var sel = keys['?'];
                        if (question != null) System.Console.WriteLine(sel.Value);
                        return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                    }

                    showTips();
                    select();
                }
                else if (key.Key == ConsoleKey.F12)
                {
                    showTips();
                    select();
                }
                else if (key.Key == ConsoleKey.F5)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = 0;
                    }

                    render();
                }
                else if (key.Key == ConsoleKey.PageUp)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        var selLeft = selected % columns;
                        selected = offset + selLeft;
                        select();
                        continue;
                    }

                    if (offset < 1)
                    {
                        select();
                        continue;
                    }

                    offset -= pageSize;
                    if (offset < 0) offset = 0;
                    selected = offset;
                    render();
                }
                else if (key.Key == ConsoleKey.PageDown)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        var selLeft = selected % columns;
                        var sel2 = offset + pageSize - columns + selLeft;
                        while (sel2 > list.Count)
                        {
                            sel2 -= columns;
                        }

                        if (sel2 < offset) sel2 = offset;
                        selected = sel2;
                        select();
                        continue;
                    }

                    if (offset + pageSize >= list.Count)
                    {
                        select();
                        continue;
                    }

                    offset += pageSize;
                    selected = offset;
                    render();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (selected - columns >= 0) selected -= columns;
                    select();
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (selected + columns < list.Count) selected += columns;
                    select();
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (selected - 1 >= 0) selected--;
                    select();
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (selected + 1 < list.Count) selected++;
                    select();
                }
                else if (key.Key == ConsoleKey.Home)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = 0;
                    }
                    else
                    {
                        var rowI = (int)Math.Floor(selected * 1.0 / columns);
                        selected = rowI * columns;
                    }

                    select();
                }
                else if (key.Key == ConsoleKey.End)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = list.Count - 1;
                    }
                    else
                    {
                        var rowI = (int)Math.Floor(selected * 1.0 / columns);
                        var toSel = (rowI + 1) * columns - 1;
                        if (toSel < list.Count) selected = toSel;
                        else selected = list.Count - 1;
                    }

                    select();
                }
                else if (keys.ContainsKey(key.KeyChar))
                {
                    var sel = keys[key.KeyChar];
                    var isUnselected = true;
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i] != sel) continue;
                        isUnselected = false;
                        selected = i;
                        select();
                        if (question != null) System.Console.WriteLine();
                        break;
                    }

                    if (isUnselected)
                    {
                        change(-1);
                        if (question != null) System.Console.WriteLine(sel.Value);
                    }

                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    var sel = list[selected];
                    if (question != null) System.Console.WriteLine(sel.Value);
                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else
                {
                    select();
                }
            }
        }

        /// <summary>
        /// Converts a secure string to unsecure string.
        /// </summary>
        /// <param name="value">The secure string to convert.</param>
        /// <returns>The unsecure string.</returns>
        internal static string ToUnsecureString(SecureString value)
        {
            if (value == null) return null;
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
