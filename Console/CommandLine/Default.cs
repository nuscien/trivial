using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Security;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The command line interface.
/// </summary>
public static class DefaultConsole
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void Write(string s, params object[] args)
        => StyleConsole.Default.Write(s, args);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void Write(ConsoleTextStyle style, string s, params object[] args)
        => StyleConsole.Default.Write(style, s, args);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void Write(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
        => StyleConsole.Default.Write(foreground, background, s, args);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void Write(Color foreground, Color background, string s, params object[] args)
        => StyleConsole.Default.Write(foreground, background, s, args);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void Write(IConsoleTextPrettier style, string s, params object[] args)
        => StyleConsole.Default.Write(style, s, args);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(StringBuilder s)
        => StyleConsole.Default.Write(s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleTextStyle style, StringBuilder s)
        => StyleConsole.Default.Write(style, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleColor foreground, StringBuilder s)
        => StyleConsole.Default.Write(foreground, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
        => StyleConsole.Default.Write(foreground, background, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(Color foreground, StringBuilder s)
        => StyleConsole.Default.Write(foreground, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(Color foreground, Color background, StringBuilder s)
        => StyleConsole.Default.Write(foreground, background, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(IConsoleTextPrettier style, StringBuilder s)
        => StyleConsole.Default.Write(style, s);

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(SecureString s)
        => StyleConsole.Default.Write(s.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleTextStyle style, SecureString s)
        => StyleConsole.Default.Write(style, s.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleColor foreground, SecureString s)
        => StyleConsole.Default.Write(foreground, s.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void Write(ConsoleColor? foreground, ConsoleColor? background, SecureString s)
        => StyleConsole.Default.Write(foreground, background, s.ToUnsecureString());

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
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(int number, string format)
        => StyleConsole.Default.Write(number, format);

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
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleTextStyle style, int number, string format)
        => StyleConsole.Default.Write(style, number, format);

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
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleColor foreground, int number, string format)
        => StyleConsole.Default.Write(foreground, number, format);

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
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleColor? foreground, ConsoleColor? background, int number, string format)
        => StyleConsole.Default.Write(foreground, background, number, format);

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
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(double number, string format)
        => StyleConsole.Default.Write(number, format);

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
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleTextStyle style, double number, string format)
        => StyleConsole.Default.Write(style, number, format);

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
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleColor foreground, double number, string format)
        => StyleConsole.Default.Write(foreground, number, format);

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
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void Write(ConsoleColor? foreground, ConsoleColor? background, double number, string format)
        => StyleConsole.Default.Write(foreground, background, number, format);

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
    /// <param name="style">The content style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public static void Write(IConsoleTextPrettier style, char[] value, int start = 0, int? count = null)
        => StyleConsole.Default.Write(style, value, start, count);

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
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void Write(IConsoleTextPrettier style, char value, int repeatCount = 1)
        => StyleConsole.Default.Write(style, value, repeatCount);

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="model">A representation model.</param>
    public static void Write(IConsoleTextCreator model)
        => StyleConsole.Default.Write(model);

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    public static void Write<T>(IConsoleTextCreator<T> style, T data)
        => StyleConsole.Default.Write(style, data);

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <typeparam name="TData">The type of data model.</typeparam>
    /// <typeparam name="TOptions">The additional options.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    /// <param name="options">The additional options.</param>
    public static void Write<TData, TOptions>(IConsoleTextCreator<TData, TOptions> style, TData data, TOptions options)
        => StyleConsole.Default.Write(style, data, options);

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
    /// It will flush immediately.
    /// </summary>
    /// <param name="content">The text content.</param>
    public static void WriteLine(ConsoleText content = null)
        => StyleConsole.Default.WriteLine(content);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="content1">The text content 1.</param>
    /// <param name="content2">The text content 2.</param>
    /// <param name="additionalContext">The additional text content collection.</param>
    public static void WriteLine(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
        => StyleConsole.Default.WriteLine(content1, content2, additionalContext);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="content">The text content collection.</param>
    public static void WriteLine(IEnumerable<ConsoleText> content)
        => StyleConsole.Default.WriteLine(content);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(string s, params object[] args)
        => StyleConsole.Default.WriteLine(s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(ConsoleTextStyle style, string s, params object[] args)
        => StyleConsole.Default.WriteLine(style, s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(ConsoleColor foreground, string s, params object[] args)
        => StyleConsole.Default.WriteLine(foreground, s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
        => StyleConsole.Default.WriteLine(foreground, background, s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(Color foreground, string s, params object[] args)
        => StyleConsole.Default.WriteLine(foreground, s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(Color foreground, Color background, string s, params object[] args)
        => StyleConsole.Default.WriteLine(foreground, background, s, args);

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public static void WriteLine(IConsoleTextPrettier style, string s, params object[] args)
        => StyleConsole.Default.WriteLine(style, s, args);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(StringBuilder s)
        => StyleConsole.Default.WriteLine(s);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleTextStyle style, StringBuilder s)
        => StyleConsole.Default.WriteLine(style, s);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleColor foreground, StringBuilder s)
        => StyleConsole.Default.WriteLine(foreground, s);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
        => StyleConsole.Default.WriteLine(foreground, background, s);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(Color foreground, StringBuilder s)
        => StyleConsole.Default.WriteLine(foreground, s);

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(Color foreground, Color background, StringBuilder s)
        => StyleConsole.Default.WriteLine(foreground, background, s);

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(IConsoleTextPrettier style, StringBuilder s)
        => StyleConsole.Default.WriteLine(style, s?.ToString());

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(SecureString s)
        => StyleConsole.Default.WriteLine(s?.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleTextStyle style, SecureString s)
        => StyleConsole.Default.WriteLine(style, s?.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleColor foreground, SecureString s)
        => StyleConsole.Default.WriteLine(foreground, s?.ToUnsecureString());

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, SecureString s)
        => StyleConsole.Default.WriteLine(foreground, background, s.ToUnsecureString());

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(int number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(int number, string format)
        => StyleConsole.Default.WriteLine(number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, int number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleTextStyle style, int number, string format)
        => StyleConsole.Default.WriteLine(style, number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, int number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleColor foreground, int number, string format)
        => StyleConsole.Default.WriteLine(foreground, number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number, string format)
        => StyleConsole.Default.WriteLine(foreground, background, number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(long number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, long number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, long number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, long number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ulong number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, ulong number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, ulong number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, ulong number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(float number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, float number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, float number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, float number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(decimal number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, decimal number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, decimal number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, decimal number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(double number)
        => StyleConsole.Default.WriteLine(number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(double number, string format)
        => StyleConsole.Default.WriteLine(number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleTextStyle style, double number)
        => StyleConsole.Default.WriteLine(style, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleTextStyle style, double number, string format)
        => StyleConsole.Default.WriteLine(style, number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor foreground, double number)
        => StyleConsole.Default.WriteLine(foreground, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleColor foreground, double number, string format)
        => StyleConsole.Default.WriteLine(foreground, number, format);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number)
        => StyleConsole.Default.WriteLine(foreground, background, number);

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number, string format)
        => StyleConsole.Default.WriteLine(foreground, background, number, format);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public static void WriteLine(char[] value, int start = 0, int? count = null)
        => StyleConsole.Default.WriteLine(value, start, count);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public static void WriteLine(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
        => StyleConsole.Default.WriteLine(style, value, start, count);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public static void WriteLine(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
        => StyleConsole.Default.WriteLine(foreground, value, start, count);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public static void WriteLine(IConsoleTextPrettier style, char[] value, int start = 0, int? count = null)
        => StyleConsole.Default.WriteLine(style, value, start, count);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void WriteLine(char value, int repeatCount = 1)
        => StyleConsole.Default.WriteLine(value, repeatCount);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void WriteLine(ConsoleTextStyle style, char value, int repeatCount = 1)
        => StyleConsole.Default.WriteLine(style, value, repeatCount);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void WriteLine(ConsoleColor foreground, char value, int repeatCount = 1)
        => StyleConsole.Default.WriteLine(foreground, value, repeatCount);

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void WriteLine(IConsoleTextPrettier style, char value, int repeatCount = 1)
        => StyleConsole.Default.WriteLine(style, value, repeatCount);

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public static void WriteLine(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
        => StyleConsole.Default.WriteLine(foreground, background, value, repeatCount);

    /// <summary>
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="stackTrace">true if output stack trace; otherwise, false.</param>
    public static void WriteLine(Exception ex, bool stackTrace = false)
        => StyleConsole.Default.WriteLine(new ConsoleTextStyle(ConsoleColor.Red), null as ConsoleTextStyle, ex, stackTrace);

    /// <summary>
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="captionStyle">The style of header.</param>
    /// <param name="messageStyle">The style of details.</param>
    /// <param name="stackTrace">true if output stack trace; otherwise, false.</param>
    public static void WriteLine(ConsoleTextStyle captionStyle, ConsoleTextStyle messageStyle, Exception ex, bool stackTrace = false)
        => StyleConsole.Default.WriteLine(captionStyle, messageStyle, ex, stackTrace);

    /// <summary>
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="ex">The error information.</param>
    public static void WriteLine(Data.ErrorMessageResult ex)
        => StyleConsole.Default.WriteLine(new ConsoleTextStyle(ConsoleColor.Red), null as ConsoleTextStyle, ex);

    /// <summary>
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="ex">The error information.</param>
    /// <param name="captionStyle">The style of header.</param>
    /// <param name="messageStyle">The style of details.</param>
    public static void WriteLine(ConsoleTextStyle captionStyle, ConsoleTextStyle messageStyle, Data.ErrorMessageResult ex)
        => StyleConsole.Default.WriteLine(captionStyle, messageStyle, ex);

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(IJsonDataNode json)
        => StyleConsole.Default.WriteLine(new JsonConsoleStyle().CreateTextCollection(json, 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(JsonConsoleStyle style, IJsonDataNode json)
        => StyleConsole.Default.WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json, 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(System.Text.Json.Nodes.JsonObject json)
        => StyleConsole.Default.WriteLine(new JsonConsoleStyle().CreateTextCollection(json == null ? null : (JsonObjectNode)json, 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(JsonConsoleStyle style, System.Text.Json.Nodes.JsonObject json)
        => StyleConsole.Default.WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json == null ? null : (JsonObjectNode)json, 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(IJsonObjectHost json)
        => StyleConsole.Default.WriteLine(new JsonConsoleStyle().CreateTextCollection(json?.ToJson(), 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(JsonConsoleStyle style, IJsonObjectHost json)
        => StyleConsole.Default.WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json?.ToJson(), 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(System.Text.Json.Nodes.JsonArray json)
        => StyleConsole.Default.WriteLine(new JsonConsoleStyle().CreateTextCollection(json == null ? null : (JsonArrayNode)json, 0));

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="json">The JSON instance.</param>
    public static void WriteLine(JsonConsoleStyle style, System.Text.Json.Nodes.JsonArray json)
        => StyleConsole.Default.WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json == null ? null : (JsonArrayNode)json, 0));

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="model">A representation model.</param>
    public static void WriteLine<T>(IConsoleTextCreator model)
        => StyleConsole.Default.WriteLine(model);

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    public static void WriteLine<T>(IConsoleTextCreator<T> style, T data)
        => StyleConsole.Default.WriteLine(style, data);

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="TData">The type of data model.</typeparam>
    /// <typeparam name="TOptions">The additional options.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    /// <param name="options">The additional options.</param>
    public static void WriteLine<TData, TOptions>(IConsoleTextCreator<TData, TOptions> style, TData data, TOptions options)
        => StyleConsole.Default.WriteLine(style, data, options);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="style">The options.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(ConsoleProgressStyle style)
        => StyleConsole.Default.WriteLine(style, null);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
    /// <param name="progressSize">The progress size.</param>
    /// <param name="kind">The progress kind.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(ConsoleProgressStyle.Sizes progressSize, string caption, ConsoleProgressStyle.Kinds kind = ConsoleProgressStyle.Kinds.Full)
        => StyleConsole.Default.WriteLine(progressSize, caption, kind);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="progressSize">The progress size.</param>
    /// <param name="kind">The progress kind.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(ConsoleProgressStyle.Sizes progressSize, ConsoleProgressStyle.Kinds kind = ConsoleProgressStyle.Kinds.Full)
        => StyleConsole.Default.WriteLine(progressSize, kind);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
    /// <param name="style">The style.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(ConsoleProgressStyle style, string caption)
        => StyleConsole.Default.WriteLine(style, caption);

    /// <summary>
    /// Writes the specific lines to the standard output stream.
    /// </summary>
    /// <param name="count">The count of line.</param>
    public static void WriteLines(int count)
        => StyleConsole.Default.WriteLines(count);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="content">The text content collection.</param>
    public static void WriteLines(IEnumerable<ConsoleText> content)
        => StyleConsole.Default.WriteLines(content);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="content">The text content.</param>
    /// <param name="additionalContext">The additional text content collection.</param>
    public static void WriteLines(ConsoleText content, params ConsoleText[] additionalContext)
        => StyleConsole.Default.WriteLines(content, additionalContext);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    public static void WriteLines(IEnumerable<string> col)
        => StyleConsole.Default.WriteLines(col);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    public static void WriteLines(ConsoleTextStyle style, IEnumerable<string> col)
        => StyleConsole.Default.WriteLines(style, col);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    public static void WriteLines(ConsoleColor foreground, IEnumerable<string> col)
        => StyleConsole.Default.WriteLines(foreground, col);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="background">The background color.</param>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    public static void WriteLines(ConsoleColor foreground, ConsoleColor background, IEnumerable<string> col)
        => StyleConsole.Default.WriteLines(foreground, background, col);

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    public static void WriteLines<T>(IEnumerable<T> col, Func<T, int, string> selector)
    {
        if (col == null) return;
        if (selector == null) selector = (ele, i) => ele?.ToString();
        StyleConsole.Default.WriteLines(col.Select(selector));
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    public static void WriteLines<T>(ConsoleColor foreground, IEnumerable<T> col, Func<T, int, string> selector)
    {
        if (col == null) return;
        if (selector == null) selector = (ele, i) => ele?.ToString();
        StyleConsole.Default.WriteLines(foreground, col.Select(selector));
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    public static void WriteLines<T>(IEnumerable<T> col, Func<T, string> selector)
    {
        if (col == null) return;
        if (selector == null) selector = ele => ele?.ToString();
        StyleConsole.Default.WriteLines(col.Select(selector));
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="col">The string collection to write. Each one in a line.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    public static void WriteLines<T>(ConsoleColor foreground, IEnumerable<T> col, Func<T, string> selector)
    {
        if (col == null) return;
        if (selector == null) selector = ele => ele?.ToString();
        StyleConsole.Default.WriteLines(foreground, col.Select(selector));
    }

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <param name="collection">The collection data.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    public static SelectionResult<object> Select(SelectionData collection, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(collection, options);

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <param name="collection">The collection data.</param>
    /// <param name="convert">The converter.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    public static SelectionResult<T> Select<T>(IEnumerable<T> collection, Func<T, SelectionItem<T>> convert, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(collection, convert, options);

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <param name="collection">The collection data.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    public static SelectionResult<T> Select<T>(IEnumerable<SelectionItem<T>> collection, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(collection, options);

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
    public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, SelectionConsoleOptions options = null, string searchPattern = null)
        => StyleConsole.Default.Select(path, options, searchPattern);

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <param name="path">The parent foler path.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, Func<FileSystemInfo, bool> predicate, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(path, predicate, options);

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="collection">The collection data.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    public static SelectionResult<T> Select<T>(SelectionData<T> collection, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(collection, options);

    /// <summary>
    /// Writes a collection of item for selecting.
    /// </summary>
    /// <param name="collection">The collection data.</param>
    /// <param name="options">The selection display options.</param>
    /// <returns>The result of selection.</returns>
    public static SelectionResult<string> Select(IEnumerable<string> collection, SelectionConsoleOptions options = null)
        => StyleConsole.Default.Select(collection, options);

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
    /// <param name="x">The horizontal translation size.</param>
    /// <param name="y">The vertical translation size.</param>
    public static void MoveCursorBy(int x, int y = 0)
        => StyleConsole.Default.MoveCursorBy(x, y);

    /// <summary>
    /// Moves cursor at a specific position in buffer.
    /// </summary>
    /// <param name="x">Column, the left from the edge of buffer.</param>
    /// <param name="y">Row, the top from the edge of buffer.</param>
    public static void MoveCursorTo(int x, int y)
        => StyleConsole.Default.MoveCursorTo(x, y);

    /// <summary>
    /// Removes the specific area.
    /// </summary>
    /// <param name="area">The area to remove.</param>
    public static void Clear(StyleConsole.RelativeAreas area)
        => StyleConsole.Default.Clear(area); 
}
