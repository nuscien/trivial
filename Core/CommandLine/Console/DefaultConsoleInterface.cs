using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The command line interface.
    /// </summary>
    public static class DefaultConsoleInterface
    {
        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content">The text content.</param>
        public static void Write(ConsoleText content)
            => ConsoleInterface.Default.Write(content);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public static void Write(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
            => ConsoleInterface.Default.Write(content1, content2, additionalContext);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public static void Write(IEnumerable<ConsoleText> content)
            => ConsoleInterface.Default.Write(content);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(string s, params object[] args)
            => ConsoleInterface.Default.Write(s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(ConsoleColor? foreground, string s, params object[] args)
            => ConsoleInterface.Default.Write(foreground, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
            => ConsoleInterface.Default.Write(foreground, background, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(Color foreground, string s, params object[] args)
            => ConsoleInterface.Default.Write(foreground, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(Color foreground, Color background, string s, params object[] args)
            => ConsoleInterface.Default.Write(foreground, background, s, args);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(int number)
            => ConsoleInterface.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, int number)
            => ConsoleInterface.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, int number)
            => ConsoleInterface.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(long number)
            => ConsoleInterface.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, long number)
            => ConsoleInterface.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, long number)
            => ConsoleInterface.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(double number)
            => ConsoleInterface.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, double number)
            => ConsoleInterface.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, double number)
            => ConsoleInterface.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.Write(value, start, count);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(ConsoleColor? foreground, char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.Write(foreground, value, start, count);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.Write(foreground, background, value, start, count);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content.</param>
        public static void WriteLine(ConsoleText content = null)
            => ConsoleInterface.Default.WriteLine(content);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public static void WriteLine(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
            => ConsoleInterface.Default.WriteLine(content1, content2, additionalContext);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public static void WriteLine(IEnumerable<ConsoleText> content)
            => ConsoleInterface.Default.WriteLine(content);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(string s, params object[] args)
            => ConsoleInterface.Default.WriteLine(s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(ConsoleColor? foreground, string s, params object[] args)
            => ConsoleInterface.Default.WriteLine(foreground, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
            => ConsoleInterface.Default.WriteLine(foreground, background, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(Color foreground, string s, params object[] args)
            => ConsoleInterface.Default.WriteLine(foreground, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(Color foreground, Color background, string s, params object[] args)
            => ConsoleInterface.Default.WriteLine(foreground, background, s, args);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(int number)
            => ConsoleInterface.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, int number)
            => ConsoleInterface.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number)
            => ConsoleInterface.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(long number)
            => ConsoleInterface.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, long number)
            => ConsoleInterface.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, long number)
            => ConsoleInterface.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(double number)
            => ConsoleInterface.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, double number)
            => ConsoleInterface.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number)
            => ConsoleInterface.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.WriteLine(value, start, count);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(ConsoleColor? foreground, char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.WriteLine(foreground, value, start, count);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
            => ConsoleInterface.Default.WriteLine(foreground, background, value, start, count);

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(IEnumerable<string> col)
            => ConsoleInterface.Default.WriteLines(col);

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(ConsoleColor foreground, IEnumerable<string> col)
            => ConsoleInterface.Default.WriteLines(foreground, col);

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public static void Flush()
            => ConsoleInterface.Default.Flush();

        /// <summary>
        /// Clears output cache.
        /// </summary>
        public static void ClearOutputCache()
            => ConsoleInterface.Default.ClearOutputCache();

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
        public static void Backspace(int count = 1, bool doNotRemoveOutput = false)
            => ConsoleInterface.Default.Backspace(count, doNotRemoveOutput);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword()
            => ConsoleInterface.Default.ReadPassword(null, null);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="writeNewLine">true if writes a followed line terminator; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(char replaceChar, bool writeNewLine = false)
            => ConsoleInterface.Default.ReadPassword(null, replaceChar, writeNewLine);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="foregroundColor">The replace charactor color.</param>
        /// <param name="writeNewLine">true if writes a followed line terminator; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(char? replaceChar, ConsoleColor? foregroundColor, bool writeNewLine = false)
            => ConsoleInterface.Default.ReadPassword(foregroundColor, replaceChar, writeNewLine);
    }
}
