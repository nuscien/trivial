using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /// Gets the length of the current line output.
        /// This does not contain the pending value.
        /// </summary>
        public int FlushedLength
        {
            get
            {
                return line.Length;
            }
        }

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
        public int Length
        {
            get
            {
                return line.Length + PendingLength;
            }
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
                System.Console.Write(str);
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
                LineUtilities.Write(foregroundColor, str);
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
                LineUtilities.Write(foregroundColor, backgroundColor, str);
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
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine()
        {
            Flush();
            LineIndex++;
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public int Read()
        {
            Flush();
            var key = System.Console.Read();
            if (key == 13) LineIndex++;
            return key;
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
            if (key.Key == ConsoleKey.Enter) LineIndex++;
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
        public void End()
        {
            Flush();
            if (line.Length < 1) return;
            System.Console.ResetColor();
            System.Console.WriteLine();
            LineIndex++;
            line.Clear();
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
            LineUtilities.Backspace(line.Length);
            line.Clear();
        }

        /// <summary>
        /// Flushes the pending string.
        /// </summary>
        public void Flush()
        {
            var str = new StringBuilder();
            foreach (var item in Pending)
            {
                var itemStr = item.Value.ToString();
                str.Append(itemStr);
                if (item != null) LineUtilities.Write(item.ForegroundColor, item.BackgroundColor, itemStr);
            }

            Pending.Clear();
            line.Append(str);
        }

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        public void Backspace(int count = 1)
        {
            if (count < 0) return;
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

            if (line.Length >= count)
            {
                line.Remove(line.Length - count, count);
            }
            else
            {
                line.Clear();
            }

            LineUtilities.Backspace(count);
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
            for (var i = 0; i < count; i++)
            {
                System.Console.Write('\u0008');
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

    /// <summary>
    /// The utilities for execution.
    /// </summary>
    public static class CommandUtilities
    {
        /// <summary>
        /// Open the file explorer.
        /// </summary>
        /// <param name="dir">The directory</param>
        /// <returns>true if a process resource is started; false if no new process resource is started.</returns>
        public static bool Directory(string dir)
        {
            var p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = dir;
            return p.Start();
        }

        /// <summary>
        /// Processes a command.
        /// </summary>
        /// <param name="cmd">The command string.</param>
        /// <returns>The output string.</returns>
        public static string Execute(string cmd)
        {
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return output;
        }
    }
}
