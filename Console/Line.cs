using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private StringBuilder line = new StringBuilder();

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
        public bool AutoFlush {
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
        /// Moves to the latest cursor position.
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
            var str = string.Format(value, arg);
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
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public void Write(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            var str = string.Format(value, arg);
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
            var str = string.Format(value, arg);
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
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <param name="question">The optional question message to output.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine(string question = null)
        {
            if (question != null) Write(question);
            Flush();
            LineIndex++;
            initLeft = -1;
            return System.Console.ReadLine();
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
        public static void Backspace(int count = 1)
        {
            var str = new StringBuilder();
            str.Append('\b', count);
            str.Append(' ', count);
            str.Append('\b', count);
            System.Console.Write(str.ToString());
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
            System.Console.Write(value, arg);
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
            System.Console.Write(value, arg);
            if (foregroundColor.HasValue) System.Console.ForegroundColor = fore;
            if (backgroundColor.HasValue) System.Console.BackgroundColor = back;
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
        /// Write a line and an empty line to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteDoubleLines(string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            System.Console.WriteLine(value, arg);
            System.Console.WriteLine();
        }
    }
}
