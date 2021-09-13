﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            => StyleConsole.Default.Write(content);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public static void Write(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
            => StyleConsole.Default.Write(content1, content2, additionalContext);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public static void Write(IEnumerable<ConsoleText> content)
            => StyleConsole.Default.Write(content);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(string s, params object[] args)
            => StyleConsole.Default.Write(s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(ConsoleTextStyle style, string s, params object[] args)
            => StyleConsole.Default.Write(style, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(ConsoleColor foreground, string s, params object[] args)
            => StyleConsole.Default.Write(foreground, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
            => StyleConsole.Default.Write(foreground, background, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(Color foreground, string s, params object[] args)
            => StyleConsole.Default.Write(foreground, s, args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Write(Color foreground, Color background, string s, params object[] args)
            => StyleConsole.Default.Write(foreground, background, s, args);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(int number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, int number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, int number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, int number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(long number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, long number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, long number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, long number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(ulong number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, ulong number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, ulong number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, ulong number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(float number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, float number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, float number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, float number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(decimal number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, decimal number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, decimal number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, decimal number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void Write(double number)
            => StyleConsole.Default.Write(number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleTextStyle style, double number)
            => StyleConsole.Default.Write(style, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor foreground, double number)
            => StyleConsole.Default.Write(foreground, number);

        /// <summary>
        /// Writes the specified number to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, double number)
            => StyleConsole.Default.Write(foreground, background, number);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.Write(value, start, count);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.Write(style, value, start, count);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void Write(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.Write(foreground, value, start, count);

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
            => StyleConsole.Default.Write(foreground, background, value, start, count);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void Write(char value, int repeatCount = 1)
            => StyleConsole.Default.Write(value, repeatCount);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void Write(ConsoleTextStyle style, char value, int repeatCount = 1)
            => StyleConsole.Default.Write(style, value, repeatCount);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void Write(ConsoleColor foreground, char value, int repeatCount = 1)
            => StyleConsole.Default.Write(foreground, value, repeatCount);

        /// <summary>
        /// Writes the specified characters to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void Write(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
            => StyleConsole.Default.Write(foreground, background, value, repeatCount);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content.</param>
        public static void WriteLine(ConsoleText content = null)
            => StyleConsole.Default.WriteLine(content);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content1">The text content 1.</param>
        /// <param name="content2">The text content 2.</param>
        /// <param name="additionalContext">The additional text content collection.</param>
        public static void WriteLine(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
            => StyleConsole.Default.WriteLine(content1, content2, additionalContext);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="content">The text content collection.</param>
        public static void WriteLine(IEnumerable<ConsoleText> content)
            => StyleConsole.Default.WriteLine(content);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(string s, params object[] args)
            => StyleConsole.Default.WriteLine(s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(ConsoleTextStyle style, string s, params object[] args)
            => StyleConsole.Default.WriteLine(style, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(ConsoleColor foreground, string s, params object[] args)
            => StyleConsole.Default.WriteLine(foreground, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
            => StyleConsole.Default.WriteLine(foreground, background, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(Color foreground, string s, params object[] args)
            => StyleConsole.Default.WriteLine(foreground, s, args);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="s">A composite format string to output.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void WriteLine(Color foreground, Color background, string s, params object[] args)
            => StyleConsole.Default.WriteLine(foreground, background, s, args);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(int number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, int number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, int number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(long number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, long number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, long number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, long number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ulong number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, ulong number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, ulong number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, ulong number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(float number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, float number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, float number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, float number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(decimal number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, decimal number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, decimal number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, decimal number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(double number)
            => StyleConsole.Default.WriteLine(number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleTextStyle style, double number)
            => StyleConsole.Default.WriteLine(style, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor foreground, double number)
            => StyleConsole.Default.WriteLine(foreground, number);

        /// <summary>
        /// Writes the specified number, followed by the current line terminator, to the standard output stream.
        /// Note it may not flush immediately.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="number">A number to output.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number)
            => StyleConsole.Default.WriteLine(foreground, background, number);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.WriteLine(value, start, count);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.WriteLine(style, value, start, count);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.WriteLine(foreground, value, start, count);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="start">The starting position in value.</param>
        /// <param name="count">The number of characters to write.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
            => StyleConsole.Default.WriteLine(foreground, background, value, start, count);


        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void WriteLine(char value, int repeatCount = 1)
            => StyleConsole.Default.WriteLine(value, repeatCount);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="style">The content style.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void WriteLine(ConsoleTextStyle style, char value, int repeatCount = 1)
            => StyleConsole.Default.WriteLine(style, value, repeatCount);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void WriteLine(ConsoleColor foreground, char value, int repeatCount = 1)
            => StyleConsole.Default.WriteLine(foreground, value, repeatCount);

        /// <summary>
        /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
            => StyleConsole.Default.WriteLine(foreground, background, value, repeatCount);

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(IEnumerable<string> col)
            => StyleConsole.Default.WriteLines(col);

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foreground">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(ConsoleColor foreground, IEnumerable<string> col)
            => StyleConsole.Default.WriteLines(foreground, col);

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public static void Flush()
            => StyleConsole.Default.Flush();

        /// <summary>
        /// Clears output cache.
        /// </summary>
        public static void ClearOutputCache()
            => StyleConsole.Default.ClearOutputCache();

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
        public static void Backspace(int count = 1, bool doNotRemoveOutput = false)
            => StyleConsole.Default.Backspace(count, doNotRemoveOutput);

        /// <summary>
        /// Enters backspaces to console to remove the charactors to the beginning of the line.
        /// </summary>
        public static void BackspaceToBeginning()
            => StyleConsole.Default.BackspaceToBeginning();

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line of characters is greater than max value of 32-bit integer.</exception>
        public static string ReadLine()
            => StyleConsole.Default.ReadLine();

        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
        /// </summary>
        /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="InvalidOperationException">The input stream is redirected from the one other than the console.</exception>
        public static ConsoleKeyInfo ReadKey(bool intercept = false)
            => StyleConsole.Default.ReadKey(intercept);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword()
            => StyleConsole.Default.ReadPassword(null, null);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(char replaceChar, bool inline = false)
            => StyleConsole.Default.ReadPassword(null, replaceChar, inline);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="foreground">The replace charactor color.</param>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public static SecureString ReadPassword(ConsoleColor? foreground, char? replaceChar, bool inline = false)
            => StyleConsole.Default.ReadPassword(foreground, replaceChar, inline);

        /// <summary>
        /// Moves cursor by a specific relative position.
        /// </summary>
        /// <param name="origin">The relative origin.</param>
        /// <param name="x">The horizontal translation size.</param>
        /// <param name="y">The vertical translation size.</param>
        public static void MoveCursor(StyleConsole.Origins origin, int x, int y)
            => StyleConsole.Default.MoveCursor(origin, x, y);

        /// <summary>
        /// Moves cursor by a specific relative position.
        /// </summary>
        /// <param name="x">The horizontal translation size.</param>
        /// <param name="y">The vertical translation size.</param>
        public static void MoveCursorBy(int x, int y = 0)
            => StyleConsole.Default.MoveCursorBy(x, y);

        /// <summary>
        /// Moves cursor at a specific position in viewport.
        /// </summary>
        /// <param name="x">Row, the top from the edge of viewport.</param>
        /// <param name="y">Column, the left from the edge of viewport.</param>
        public static void MoveCursorAt(int x, int y)
            => StyleConsole.Default.MoveCursorAt(x, y);

        /// <summary>
        /// Moves cursor at a specific position in buffer.
        /// </summary>
        /// <param name="x">Row, the top from the edge of buffer.</param>
        /// <param name="y">Column, the left from the edge of buffer.</param>
        public static void MoveCursorTo(int x, int y)
            => StyleConsole.Default.MoveCursorTo(x, y);

        /// <summary>
        /// Removes the specific area.
        /// </summary>
        /// <param name="area">The area to remove.</param>
        public static void Clear(StyleConsole.RelativeAreas area)
            => StyleConsole.Default.Clear(area); 
    }
}
