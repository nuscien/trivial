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
        /// <param name="options">The options.</param>
        /// <returns>The progress result.</returns>
        public ProgressLineResult WriteLine(ProgressLineOptions options)
        {
            return WriteLine(null, options);
        }

        /// <summary>
        /// Writes a progress component after ending the current pending output, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="progressSize">The progress size.</param>
        /// <param name="style">The progress style.</param>
        /// <returns>The progress result.</returns>
        public ProgressLineResult WriteLine(string caption, ProgressLineOptions.Sizes progressSize, ProgressLineOptions.Styles style = ProgressLineOptions.Styles.Full)
        {
            return WriteLine(caption, progressSize != ProgressLineOptions.Sizes.None ? new ProgressLineOptions
            {
                Size = progressSize,
                Style = style
            } : null);
        }

        /// <summary>
        /// Writes a progress component after ending the current pending output, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="progressSize">The progress size.</param>
        /// <param name="style">The progress style.</param>
        /// <returns>The progress result.</returns>
        public ProgressLineResult WriteLine(ProgressLineOptions.Sizes progressSize, ProgressLineOptions.Styles style = ProgressLineOptions.Styles.Full)
        {
            return WriteLine(null, progressSize != ProgressLineOptions.Sizes.None ? new ProgressLineOptions
            {
                Size = progressSize,
                Style = style
            } : null);
        }

        /// <summary>
        /// Writes a progress component after ending the current pending output, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="options">The options.</param>
        /// <returns>The progress result.</returns>
        public ProgressLineResult WriteLine(string caption, ProgressLineOptions options)
        {
            End();
            var result = LineUtilities.WriteLine(caption, options);
            try
            {
                initTop = System.Console.CursorTop;
                curTop = System.Console.CursorTop;
                curLeft = System.Console.CursorLeft;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }

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
            LineUtilities.SetColor(foregroundColor, false);
            foreach (var item in col)
            {
                Write(item);
                End(true);
            }

            LineUtilities.SetColor(c, false);
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
                        if (str.Length == 0) break;
                        Write(' ');
                        str.RemoveAt(str.Length - 1);
                        Backspace();
                        if (replaceChar.HasValue) Backspace();
                        break;
                    case ConsoleKey.Delete:
                    case ConsoleKey.Clear:
                    case ConsoleKey.F5:
                        str.Clear();
                        SaveCursorPosition();
                        if (replaceChar.HasValue) Backspace(len);
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
            try
            {
                System.Console.ResetColor();
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }

            System.Console.WriteLine();
            LineIndex++;
            line.Clear();
            try
            {
                initTop = System.Console.CursorTop;
                curTop = System.Console.CursorTop;
                curLeft = System.Console.CursorLeft;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }

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
                try
                {
                    if (System.Console.CursorTop == initTop && System.Console.CursorLeft > initLeft)
                    {
                        LineUtilities.Backspace(System.Console.CursorLeft - initLeft);
                    }
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
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
                #pragma warning disable IDE0056 // 使用索引运算符
                var last = Pending[Pending.Count - 1]?.Value;
                #pragma warning restore IDE0056 // 使用索引运算符
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
            #pragma warning disable IDE0056
            return Pending.Count > 0 ? Pending[Pending.Count - 1] : null;
            #pragma warning restore IDE0056
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
}
