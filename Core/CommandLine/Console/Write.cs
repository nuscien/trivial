using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Trivial.Data;
using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The command line interface.
    /// </summary>
    public sealed partial class StyleConsole
    {
        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content">The text content.</param>
        public void Write(ConsoleText content)
        {
            if (content == null) return;
            col.Add(content);
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public void Write(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
        {
            if (content1 != null) col.Add(content1);
            if (content2 != null) col.Add(content2);
            foreach (var item in additionalContext)
            {
                if (item != null) col.Add(item);
            }

            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public void Write(IEnumerable<ConsoleText> content)
        {
            if (content == null) return;
            foreach (var item in content)
            {
                if (item != null) col.Add(item);
            }

            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args)));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(ConsoleTextStyle style, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(ConsoleColor foreground, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(Color foreground, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Write(Color foreground, Color background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        public void Write(StringBuilder s)
        {
            col.Add(new ConsoleText(s));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        public void Write(ConsoleTextStyle style, StringBuilder s)
        {
            col.Add(new ConsoleText(s, style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void Write(ConsoleColor foreground, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void Write(Color foreground, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void Write(Color foreground, Color background, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(int number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(long number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(float number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void Write(double number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleTextStyle style, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor foreground, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count)));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground, background));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(ConsoleTextStyle style, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, style));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(ConsoleColor foreground, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground)));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void Write(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
            OnAppend();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="content">The text content.</param>
        public void WriteImmediately(ConsoleText content)
        {
            if (content != null) col.Add(content);
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public void WriteImmediately(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
        {
            if (content1 != null) col.Add(content1);
            if (content2 != null) col.Add(content2);
            foreach (var item in additionalContext)
            {
                if (item != null) col.Add(item);
            }

            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public void WriteImmediately(IEnumerable<ConsoleText> content)
        {
            if (content == null) return;
            foreach (var item in content)
            {
                if (item != null) col.Add(item);
            }

            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteImmediately(string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args)));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteImmediately(ConsoleTextStyle style, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteImmediately(ConsoleColor foreground, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        public void WriteImmediately(StringBuilder s)
        {
            col.Add(new ConsoleText(s));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, StringBuilder s)
        {
            col.Add(new ConsoleText(s, style));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void WriteImmediately(ConsoleColor foreground, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
        {
            col.Add(new ConsoleText(s, foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(int number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(long number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(float number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(double number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleTextStyle style, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor foreground, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteImmediately(char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count)));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteImmediately(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), style));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteImmediately(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground, background));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteImmediately(char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteImmediately(ConsoleTextStyle style, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, style));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteImmediately(ConsoleColor foreground, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground)));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content.</param>
        public void WriteLine(ConsoleText content = null)
        {
            if (content != null) col.Add(content);
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public void WriteLine(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
        {
            if (content1 != null) col.Add(content1);
            if (content2 != null) col.Add(content2);
            foreach (var item in additionalContext)
            {
                if (item != null) col.Add(item);
            }

            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public void WriteLine(IEnumerable<ConsoleText> content)
        {
            if (content == null) return;
            foreach (var item in content)
            {
                if (item != null) col.Add(item);
            }

            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args)));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(ConsoleTextStyle style, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(ConsoleColor foreground, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(Color foreground, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void WriteLine(Color foreground, Color background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(int number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(long number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, long number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, ulong number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(float number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, float number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, decimal number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public void WriteLine(double number)
        {
            col.Add(new ConsoleText(number.ToString("g")));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleTextStyle style, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor foreground, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number)
        {
            col.Add(new ConsoleText(number.ToString("g"), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count)));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
        {
            col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground, background));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteLine(char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteLine(ConsoleTextStyle style, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, style));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteLine(ConsoleColor foreground, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground)));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
            col.Add(new ConsoleText(Environment.NewLine));
            Flush();
        }

        /// <summary>
        /// Writes the specific lines to the standard output stream.
        /// </summary>
        /// <param name="count">The count of line.</param>
        public void WriteLines(int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                sb.AppendLine();
            }

            WriteImmediately(sb.ToString());
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public void WriteLines(ConsoleText content, params ConsoleText[] additionalContext)
        {
            if (content != null) col.Add(content);
            col.Add(new ConsoleText(Environment.NewLine));
            foreach (var item in additionalContext)
            {
                if (item != null) col.Add(item);
                col.Add(new ConsoleText(Environment.NewLine));
            }

            Flush();
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public void WriteLines(IEnumerable<string> col)
        {
            if (col == null) return;
            foreach (var item in col)
            {
                WriteLine(item);
            }
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public void WriteLines(ConsoleColor foreground, IEnumerable<string> col)
        {
            if (col == null) return;
            foreach (var item in col)
            {
                WriteLine(foreground, item);
            }
        }
    }
}
