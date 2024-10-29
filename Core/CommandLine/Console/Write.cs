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

namespace Trivial.CommandLine;

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
    public void Append(ConsoleText content)
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
    public void Append(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
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
    public void Append(IEnumerable<ConsoleText> content)
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args)));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(ConsoleTextStyle style, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(ConsoleColor foreground, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground));
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(Color foreground, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground));
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(Color foreground, Color background, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Append(IConsoleTextPrettier style, string s, params object[] args)
    {
        if (s == null) return;
        if (style == null)
        {
            Append(s, args);
            return;
        }

        var list = style.CreateTextCollection(args == null || args.Length == 0 ? s : string.Format(s, args));
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public void Append(StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Append(ConsoleTextStyle style, StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s, style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Append(ConsoleColor foreground, StringBuilder s)
    {
        if (s == null) return;
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s, foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Append(Color foreground, StringBuilder s)
    {
        if (s == null) return;
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
    public void Append(Color foreground, Color background, StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s, foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Append(IConsoleTextPrettier style, StringBuilder s)
    {
        if (s == null) return;
        if (style == null)
        {
            Append(s);
            return;
        }

        var list = style.CreateTextCollection(s.ToString());
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(int number)
    {
        col.Add(new ConsoleText(number.ToString("g")));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format)));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleTextStyle style, int number)
    {
        col.Add(new ConsoleText(number.ToString("g"), style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleTextStyle style, int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleColor foreground, int number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleColor foreground, int number, string format)
    {
        if (string.IsNullOrEmpty(format)) format = "g";
        col.Add(new ConsoleText(number.ToString(format), foreground));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleColor? foreground, ConsoleColor? background, int number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleColor? foreground, ConsoleColor? background, int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(long number)
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
    public void Append(ConsoleTextStyle style, long number)
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
    public void Append(ConsoleColor foreground, long number)
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, long number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(ulong number)
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
    public void Append(ConsoleTextStyle style, ulong number)
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
    public void Append(ConsoleColor foreground, ulong number)
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, ulong number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(float number)
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
    public void Append(ConsoleTextStyle style, float number)
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
    public void Append(ConsoleColor foreground, float number)
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, float number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(decimal number)
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
    public void Append(ConsoleTextStyle style, decimal number)
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
    public void Append(ConsoleColor foreground, decimal number)
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, decimal number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Append(double number)
    {
        col.Add(new ConsoleText(number.ToString("g")));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format)));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleTextStyle style, double number)
    {
        col.Add(new ConsoleText(number.ToString("g"), style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleTextStyle style, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), style));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleColor foreground, double number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleColor foreground, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    public void Append(ConsoleColor? foreground, ConsoleColor? background, double number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void Append(ConsoleColor? foreground, ConsoleColor? background, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified characters to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public void Append(char[] value, int start = 0, int? count = null)
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
    public void Append(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
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
    public void Append(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
    {
        col.Add(new ConsoleText(StringExtensions.ToString(value, start, count), foreground, background));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public void Append(IConsoleTextPrettier style, char[] value, int start = 0, int? count = null)
    {
        if (style == null)
        {
            Append(value, start, count);
            return;
        }

        var list = style.CreateTextCollection(StringExtensions.ToString(value, start, count));
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified characters to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public void Append(char value, int repeatCount = 1)
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
    public void Append(ConsoleTextStyle style, char value, int repeatCount = 1)
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
    public void Append(ConsoleColor foreground, char value, int repeatCount = 1)
    {
        col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground)));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public void Append(IConsoleTextPrettier style, char value, int repeatCount = 1)
    {
        if (style == null || repeatCount < 1)
        {
            Append(value, repeatCount);
            return;
        }

        var list = style.CreateTextCollection(new string(value, repeatCount));
        if (list == null) return;
        col.AddRange(list);
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
    public void Append(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
    {
        col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
        OnAppend();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <param name="model">A representation model.</param>
    public void Append(IConsoleTextCreator model)
    {
        if (model == null) return;
        var list = model.CreateTextCollection();
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    public void Append<T>(IConsoleTextCreator<T> style, T data)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data);
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// Note it may not flush immediately.
    /// </summary>
    /// <typeparam name="TData">The type of data model.</typeparam>
    /// <typeparam name="TOptions">The additional options.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    /// <param name="options">The additional options.</param>
    public void Append<TData, TOptions>(IConsoleTextCreator<TData, TOptions> style, TData data, TOptions options)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data, options);
        if (list == null) return;
        col.AddRange(list);
        OnAppend();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="content">The text content.</param>
    public void Write(ConsoleText content)
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
    public void Write(ConsoleText content1, ConsoleText content2, params ConsoleText[] additionalContext)
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
    public void Write(IEnumerable<ConsoleText> content)
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Write(string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args)));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Write(ConsoleTextStyle style, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), style));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Write(ConsoleColor foreground, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground));
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Write(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
    {
        if (s == null) return;
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void Write(IConsoleTextPrettier style, string s, params object[] args)
    {
        if (s == null) return;
        if (style == null)
        {
            Write(s, args);
            return;
        }

        var list = style.CreateTextCollection(args == null || args.Length == 0 ? s : string.Format(s, args));
        if (list == null) return;
        col.AddRange(list);
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public void Write(StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Write(ConsoleTextStyle style, StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s, style));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Write(ConsoleColor foreground, StringBuilder s)
    {
        if (s == null) return;
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
    {
        if (s == null) return;
        col.Add(new ConsoleText(s, foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void Write(IConsoleTextPrettier style, StringBuilder s)
    {
        if (s == null) return;
        if (style == null)
        {
            Write(s);
            return;
        }

        var list = style.CreateTextCollection(s.ToString());
        if (list == null) return;
        col.AddRange(list);
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(int number)
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
    public void Write(ConsoleTextStyle style, int number)
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
    public void Write(ConsoleColor foreground, int number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, int number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(long number)
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
    public void Write(ConsoleTextStyle style, long number)
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
    public void Write(ConsoleColor foreground, long number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, long number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(ulong number)
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
    public void Write(ConsoleTextStyle style, ulong number)
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
    public void Write(ConsoleColor foreground, ulong number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, ulong number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(float number)
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
    public void Write(ConsoleTextStyle style, float number)
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
    public void Write(ConsoleColor foreground, float number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, float number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(decimal number)
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
    public void Write(ConsoleTextStyle style, decimal number)
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
    public void Write(ConsoleColor foreground, decimal number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, decimal number)
    {
        col.Add(new ConsoleText(number.ToString("g"), foreground, background));
        Flush();
    }

    /// <summary>
    /// Writes the specified number to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    public void Write(double number)
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
    public void Write(ConsoleTextStyle style, double number)
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
    public void Write(ConsoleColor foreground, double number)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, double number)
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
    public void Write(char[] value, int start = 0, int? count = null)
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
    public void Write(ConsoleTextStyle style, char[] value, int start = 0, int? count = null)
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
    public void Write(ConsoleColor foreground, char[] value, int start = 0, int? count = null)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, char[] value, int start = 0, int? count = null)
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
    public void Write(char value, int repeatCount = 1)
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
    public void Write(ConsoleTextStyle style, char value, int repeatCount = 1)
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
    public void Write(ConsoleColor foreground, char value, int repeatCount = 1)
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
    public void Write(ConsoleColor? foreground, ConsoleColor? background, char value, int repeatCount = 1)
    {
        col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
        Flush();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="model">A representation model.</param>
    public void Write(IConsoleTextCreator model)
    {
        if (model == null) return;
        var list = model.CreateTextCollection();
        if (list == null) return;
        col.AddRange(list);
        Flush();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    public void Write<T>(IConsoleTextCreator<T> style, T data)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data);
        if (list == null) return;
        col.AddRange(list);
        Flush();
    }

    /// <summary>
    /// Writes the specified data to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="TData">The type of data model.</typeparam>
    /// <typeparam name="TOptions">The additional options.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    /// <param name="options">The additional options.</param>
    public void Write<TData, TOptions>(IConsoleTextCreator<TData, TOptions> style, TData data, TOptions options)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data, options);
        if (list == null) return;
        col.AddRange(list);
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args)));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(ConsoleTextStyle style, string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), style));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(ConsoleColor foreground, string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(Color foreground, string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(Color foreground, Color background, string s, params object[] args)
    {
        col.Add(new ConsoleText(args == null || args.Length == 0 ? s : string.Format(s, args), foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="FormatException">format is invalid. -or- The index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
    public void WriteLine(IConsoleTextPrettier style, string s, params object[] args)
    {
        if (s == null) return;
        if (style == null)
        {
            WriteLine(s, args);
            return;
        }

        var list = style.CreateTextCollection(args == null || args.Length == 0 ? s : string.Format(s, args));
        if (list == null) return;
        col.AddRange(list);
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(ConsoleTextStyle style, StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s, style));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(ConsoleColor foreground, StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s, foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s, foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(Color foreground, StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s, foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(Color foreground, Color background, StringBuilder s)
    {
        if (s != null) col.Add(new ConsoleText(s, foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="s">A composite format string to output.</param>
    public void WriteLine(IConsoleTextPrettier style, StringBuilder s)
    {
        if (s == null) return;
        if (style == null)
        {
            WriteLine(s);
            return;
        }

        var list = style.CreateTextCollection(s.ToString());
        if (list == null) return;
        col.AddRange(list);
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(int number, string format)
    {
        if (string.IsNullOrEmpty(format)) format = "g";
        col.Add(new ConsoleText(number.ToString(format)));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleTextStyle style, int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), style));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleColor foreground, int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, int number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format)));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleTextStyle style, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), style));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleColor foreground, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// Writes the specified number, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <param name="number">A number to output.</param>
    /// <param name="format">A standard or custom numeric format string.</param>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public void WriteLine(ConsoleColor? foreground, ConsoleColor? background, double number, string format)
    {
        col.Add(new ConsoleText(number.ToString(format), foreground, background));
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified characters, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="start">The starting position in value.</param>
    /// <param name="count">The number of characters to write.</param>
    public void WriteLine(IConsoleTextPrettier style, char[] value, int start = 0, int? count = null)
    {
        if (style == null)
        {
            WriteLine(value, start, count);
            return;
        }

        var list = style.CreateTextCollection(StringExtensions.ToString(value, start, count));
        if (list == null) return;
        col.AddRange(list);
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// It will flush immediately.
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
    /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    public void WriteLine(IConsoleTextPrettier style, char value, int repeatCount = 1)
    {
        if (style == null || repeatCount < 1)
        {
            WriteLine(value, repeatCount);
            return;
        }

        var list = style.CreateTextCollection(new string(value, repeatCount));
        if (list == null) return;
        col.AddRange(list);
        col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="model">A representation model.</param>
    public void WriteLine(IConsoleTextCreator model)
    {
        if (model == null) return;
        var list = model.CreateTextCollection();
        if (list == null) return;
        col.AddRange(list);
        if (!model.ContainsTerminator) col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    public void WriteLine<T>(IConsoleTextCreator<T> style, T data)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data);
        if (list == null) return;
        col.AddRange(list);
        if (!style.ContainsTerminator) col.Add(new ConsoleText(Environment.NewLine));
        Flush();
    }

    /// <summary>
    /// Writes the specified data, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <typeparam name="TData">The type of data model.</typeparam>
    /// <typeparam name="TOptions">The additional options.</typeparam>
    /// <param name="style">The style.</param>
    /// <param name="data">A data model.</param>
    /// <param name="options">The additional options.</param>
    public void WriteLine<TData, TOptions>(IConsoleTextCreator<TData, TOptions> style, TData data, TOptions options)
    {
        if (style == null) return;
        var list = style.CreateTextCollection(data, options);
        if (list == null) return;
        col.AddRange(list);
        if (!style.ContainsTerminator) col.Add(new ConsoleText(Environment.NewLine));
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

        Write(sb.ToString());
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="content">The text content collection.</param>
    public void WriteLines(IEnumerable<ConsoleText> content)
    {
        if (content == null) return;
        foreach (var item in content)
        {
            if (item != null) col.Add(item);
            col.Add(new ConsoleText(Environment.NewLine));
        }

        Flush();
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
    /// <param name="value">The string collection to write. Each one in a line.</param>
    public void WriteLines(IEnumerable<string> value)
    {
        if (value == null) return;
        col.Add(new ConsoleText(string.Join(Environment.NewLine, value)));
        Flush();
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="style">The content style.</param>
    /// <param name="value">The string collection to write. Each one in a line.</param>
    public void WriteLines(ConsoleTextStyle style, IEnumerable<string> value)
    {
        if (value == null) return;
        col.Add(new ConsoleText(string.Join(Environment.NewLine, value), style));
        Flush();
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="value">The string collection to write. Each one in a line.</param>
    public void WriteLines(ConsoleColor foreground, IEnumerable<string> value)
    {
        if (value == null) return;
        col.Add(new ConsoleText(string.Join(Environment.NewLine, value), foreground));
        Flush();
    }

    /// <summary>
    /// Writes the current line terminator for each item, to the standard output stream.
    /// </summary>
    /// <param name="foreground">The foreground color of the console.</param>
    /// <param name="background">The background color.</param>
    /// <param name="value">The string collection to write. Each one in a line.</param>
    public void WriteLines(ConsoleColor foreground, ConsoleColor background, IEnumerable<string> value)
    {
        if (value == null) return;
        col.Add(new ConsoleText(string.Join(Environment.NewLine, value), foreground, background));
        Flush();
    }
}
