﻿using System;
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
    public sealed partial class ConsoleInterface
    {
        /// <summary>
        /// The lock.
        /// </summary>
        private readonly object locker = new();

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly List<ConsoleText> col = new();

        /// <summary>
        /// The additional context.
        /// </summary>
        private object context;

        /// <summary>
        /// The command line handler.
        /// </summary>
        private IHandler handlerCache;

        /// <summary>
        /// Adds or removes the handler occurred after flushing.
        /// </summary>
        public event DataEventHandler<IReadOnlyList<ConsoleText>> Flushed;

        /// <summary>
        /// Adds or removes the handler occurred after flushing.
        /// </summary>
        public event EventHandler<RelativePositionEventArgs> CursorMoved;

        /// <summary>
        /// Adds or removes the handler occurred after output area clearing.
        /// </summary>
        public event DataEventHandler<RelativeAreas> Cleared;

        /// <summary>
        /// Gets or sets the terminal mode.
        /// </summary>
        public Modes Mode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need disable auto flush.
        /// </summary>
        public bool IsAutoFlushDisabled { get; set; }

        /// <summary>
        /// Gets or sets the handler to flush cache.
        /// </summary>
        public IHandler Handler
        {
            get
            {
                return handlerCache;
            }

            set
            {
                lock (locker)
                {
                    if (handlerCache == value) return;
                    handlerCache = value;
                    if (handlerCache == null) return;
                    context = handlerCache.CreateContext();
                }
            }
        }

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
        public void Write(ConsoleColor? foreground, ConsoleColor background, string s, params object[] args)
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
        public void Write(ConsoleColor? foreground, ConsoleColor background, int number)
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
        public void Write(ConsoleColor? foreground, ConsoleColor background, long number)
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
        public void Write(ConsoleColor? foreground, ConsoleColor background, double number)
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
        public void Write(ConsoleColor? foreground, ConsoleColor background, char[] value, int start = 0, int? count = null)
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
        public void Write(ConsoleColor? foreground, ConsoleColor background, char value, int repeatCount = 1)
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
            if (content == null) return;
            col.Add(content);
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
        public void WriteImmediately(ConsoleColor? foreground, ConsoleColor background, string s, params object[] args)
        {
            col.Add(new ConsoleText(args.Length == 0 ? s : string.Format(s, args), foreground, background));
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, string s, params object[] args)
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, int number)
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, long number)
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, double number)
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, char[] value, int start = 0, int? count = null)
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
        public void WriteLine(ConsoleColor? foreground, ConsoleColor background, char value, int repeatCount = 1)
        {
            col.Add(new ConsoleText(value, repeatCount, new ConsoleTextStyle(foreground, background)));
            col.Add(new ConsoleText(Environment.NewLine));
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

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public void Flush()
        {
            TestMode();
            if (col.Count == 0) return;
            IReadOnlyList<ConsoleText> list;
            lock (locker)
            {
                list = col.ToList().AsReadOnly();
                col.Clear();
            }

            var h = Handler;
            if (h != null)
            {
                h.Write(list, context);
                return;
            }

            if (Mode == Modes.Cmd)
            {
                foreach (var item in list)
                {
                    var s = item?.Content?.ToString();
                    if (string.IsNullOrEmpty(s)) continue;
                    var hasSetColor = false;
                    if (item.Style.ForegroundConsoleColor.HasValue)
                    {
                        Console.ForegroundColor = item.Style.ForegroundConsoleColor.Value;
                        hasSetColor = true;
                    }
                    else if (item.Style.ForegroundRgbColor.HasValue)
                    {
                    }

                    if (item.Style.BackgroundConsoleColor.HasValue)
                    {
                        Console.BackgroundColor = item.Style.BackgroundConsoleColor.Value;
                        hasSetColor = true;
                    }
                    else if (item.Style.BackgroundRgbColor.HasValue)
                    {
                    }

                    Console.Write(s);
                    if (hasSetColor) Console.ResetColor();
                }
            }
            else if (Mode == Modes.Text)
            {
                var sb = new StringBuilder();
                foreach (var item in list)
                {
                    if (item?.Content != null)
                        StringExtensions.Append(sb, item?.Content);
                }

                Console.Write(sb.ToString());
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var item in list)
                {
                    if (item?.Content == null) continue;
                    item.AppendTo(sb);
                }

                Console.Write(sb.ToString());
            }

            Flushed?.Invoke(this, new DataEventArgs<IReadOnlyList<ConsoleText>>(list));
        }

        /// <summary>
        /// Clears output cache.
        /// </summary>
        public void ClearOutputCache()
        {
            lock (locker)
            {
                col.Clear();
            }
        }

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
        public void Backspace(int count = 1, bool doNotRemoveOutput = false)
        {
            if (count < 1) return;
            lock (locker)
            {
                var item = col.LastOrDefault();
                while (item != null && count > 0)
                {
                    var len = item.Length;
                    if (len > count)
                    {
                        try
                        {
                            item.Content.Remove(item.Content.Length - count, count);
                            return;
                        }
                        catch (NullReferenceException)
                        {
                        }
                        catch (ArgumentException)
                        {
                        }
                    }

                    try
                    {
                        col.RemoveAt(col.Count - 1);
                        count -= len;
                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }

                    item = col.LastOrDefault();
                }
            }

            BackspaceInternal(count, doNotRemoveOutput ? 2 : 0);
        }

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line of characters is greater than max value of 32-bit integer.</exception>
        public string ReadLine()
        {
            Flush();
            var h = Handler;
            return h == null ? Console.ReadLine() : h.ReadLine(context);
        }

        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
        /// </summary>
        /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="InvalidOperationException">The input stream is redirected from the one other than the console.</exception>
        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            var h = Handler;
            return h == null ? Console.ReadKey(intercept) : h.ReadKey(intercept, context);
        }

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <returns>
        /// The password.
        /// </returns>
        public SecureString ReadPassword()
            => ReadPassword(null, null);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public SecureString ReadPassword(char replaceChar, bool inline = false)
            => ReadPassword(null, replaceChar, inline);

        /// <summary>
        /// Obtains the password pressed by the user.
        /// </summary>
        /// <param name="foreground">The replace charactor color.</param>
        /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
        /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
        /// <returns>
        /// The password.
        /// </returns>
        public SecureString ReadPassword(ConsoleColor? foreground, char? replaceChar, bool inline = false)
        {
            Flush();
            var str = new SecureString();
            var normalMode = Mode != Modes.Text;
            while (true)
            {
                ConsoleKeyInfo key;
                try
                {
                    key = ReadKey(true);
                }
                catch (InvalidOperationException)
                {
                    var s = ReadLine();
                    foreach (var c in s)
                    {
                        str.AppendChar(c);
                    }

                    MoveCursorBy(0, -1);
                    Clear(RelativeAreas.Line);
                    if (inline && replaceChar.HasValue)
                    {
                        col.Add(new ConsoleText(replaceChar.Value, 6, foreground));
                    }

                    return str;
                }

                var len = str.Length;
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        if (replaceChar.HasValue)
                        {
                            if (!normalMode)
                                col.Add(new ConsoleText(replaceChar.Value, 6, foreground));
                            else if (len < 6)
                                col.Add(new ConsoleText(replaceChar.Value, 6 - len, foreground));
                            else if (len > 6)
                                Backspace(len - 6);
                        }

                        if (!inline) col.Add(new ConsoleText(Environment.NewLine));
                        Flush();
                        return str;
                    case ConsoleKey.Escape:
                        str.Dispose();
                        return null;
                    case ConsoleKey.Backspace:
                        if (key.Modifiers == ConsoleModifiers.Shift || key.Modifiers == ConsoleModifiers.Control)
                        {
                            str.Clear();
                            if (replaceChar.HasValue && normalMode) Backspace(len + 1);
                            break;
                        }

                        if (str.Length == 0) break;
                        str.RemoveAt(str.Length - 1);
                        if (normalMode)
                        {
                            Write(' ');
                            Backspace(replaceChar.HasValue ? 2 : 1);
                        }

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
                        if (hasKey && replaceChar.HasValue && normalMode)
                        {
                            if (foreground.HasValue)
                                Write(foreground.Value, replaceChar.Value);
                            else
                                Write(replaceChar.Value);
                            Flush();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Moves cursor by a specific relative position.
        /// </summary>
        /// <param name="origin">The relative origin.</param>
        /// <param name="x">The horizontal translation size.</param>
        /// <param name="y">The vertical translation size.</param>
        public void MoveCursor(Origins origin, int x, int y)
        {
            switch (origin)
            {
                case Origins.Current:
                    MoveCursorBy(x, y);
                    break;
                case Origins.ViewPort:
                    MoveCursorAt(x, y);
                    break;
                case Origins.Buffer:
                    MoveCursorTo(x, y);
                    break;
            }
        }

        /// <summary>
        /// Moves cursor by a specific relative position.
        /// </summary>
        /// <param name="x">The horizontal translation size.</param>
        /// <param name="y">The vertical translation size.</param>
        public void MoveCursorBy(int x, int y = 0)
        {
            var h = Handler;
            if (h != null)
            {
                h.MoveCursor(Origins.Current, x, y, context);
                CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.Current, x, y));
                return;
            }

            TestMode();
            switch (Mode)
            {
                case Modes.Cmd:
                    if (y != 0)
                    {
                        try
                        {
                            Console.CursorTop += y;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (IOException)
                        {
                        }
                        catch (SecurityException)
                        {
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            if (y > 0)
                            {
                                Console.CursorTop = Math.Min(Console.CursorTop + y, Console.BufferHeight - 1);
                                WriteImmediately(Environment.NewLine);
                            }
                            else
                            {
                                Console.CursorTop = Math.Max(Console.CursorTop + y, 0);
                            }
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }

                    if (x != 0)
                    {
                        try
                        {
                            Console.CursorLeft += x;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (IOException)
                        {
                        }
                        catch (SecurityException)
                        {
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.CursorLeft = x > 0
                                ? Math.Min(Console.CursorLeft + x, Console.BufferWidth - 1)
                                : Math.Max(Console.CursorLeft + x, 0);
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }

                    break;
                case Modes.Text:
                    if (y > 0 && col.LastOrDefault()?.Content?.ToString() != Environment.NewLine)
                        col.Add(new ConsoleText(Environment.NewLine));
                    break;
                default:
                    WriteImmediately(AnsiCodeGenerator.MoveCursorBy(x, y));
                    break;
            }

            CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.Current, x, y));
        }

        /// <summary>
        /// Moves cursor at a specific position in viewport.
        /// </summary>
        /// <param name="x">Row, the top from the edge of viewport.</param>
        /// <param name="y">Column, the left from the edge of viewport.</param>
        public void MoveCursorAt(int x, int y)
        {
            var h = Handler;
            if (h != null)
            {
                h.MoveCursor(Origins.ViewPort, x, y, context);
                CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.ViewPort, x, y));
                return;
            }

            TestMode();
            switch (Mode)
            {
                case Modes.Cmd:
                    try
                    {
                        Console.CursorLeft = x;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.CursorLeft = Math.Min(x, Console.BufferWidth - 1);
                    }
                    catch (NotSupportedException)
                    {
                    }

                    break;
                case Modes.Text:
                    break;
                default:
                    WriteImmediately(AnsiCodeGenerator.MoveCursorAt(x, y));
                    break;
            }

            CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.ViewPort, x, y));
        }

        /// <summary>
        /// Moves cursor at a specific position in buffer.
        /// </summary>
        /// <param name="x">Row, the top from the edge of buffer.</param>
        /// <param name="y">Column, the left from the edge of buffer.</param>
        public void MoveCursorTo(int x, int y)
        {
            var h = Handler;
            if (h != null)
            {
                h.MoveCursor(Origins.Buffer, x, y, context);
                CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.Buffer, x, y));
                return;
            }

            TestMode();
            switch (Mode)
            {
                case Modes.Cmd:
                    try
                    {
                        Console.CursorTop = y;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.CursorTop = Math.Min(y, Console.BufferHeight - 1);
                        WriteImmediately(Environment.NewLine);
                    }
                    catch (NotSupportedException)
                    {
                    }

                    try
                    {
                        Console.CursorLeft = x;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.CursorLeft = Math.Min(x, Console.BufferWidth - 1);
                    }
                    catch (NotSupportedException)
                    {
                    }

                    break;
                case Modes.Text:
                    break;
                default:
                    WriteImmediately(AnsiCodeGenerator.MoveCursorTo(x, y));
                    break;
            }

            CursorMoved?.Invoke(this, new RelativePositionEventArgs(Origins.Buffer, x, y));
        }

        /// <summary>
        /// Removes the specific area.
        /// </summary>
        /// <param name="area">The area to remove.</param>
        public void Clear(RelativeAreas area)
        {
            Flush();
            var h = Handler;
            if (h != null)
            {
                h.Remove(area, context);
                Cleared?.Invoke(this, new DataEventArgs<RelativeAreas>(area));
                return;
            }

            switch (Mode)
            {
                case Modes.Cmd:
                    try
                    {
                        ClearInCmd(area);
                        break;
                    }
                    catch (IOException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }

                    WriteImmediately(AnsiCodeGenerator.Clear(area));
                    break;
                case Modes.Text:
                    switch (area)
                    {
                        case RelativeAreas.ToEndOfScreen:
                        case RelativeAreas.ToEndOfLine:
                            col.Add(new ConsoleText(" \b"));
                            break;
                        case RelativeAreas.None:
                            break;
                        default:
                            Write("\b \b");
                            break;
                    }

                    break;
                default:
                    WriteImmediately(AnsiCodeGenerator.Clear(area));
                    break;
            }

            Cleared?.Invoke(this, new DataEventArgs<RelativeAreas>(area));
        }

        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="keepLevel">0 to remove; 1 to remove but keep cursor position; 2 to only move cursor.</param>
        private void BackspaceInternal(int count = 1, int keepLevel = 0)
        {
            if (count < 1) return;
            if (Mode == Modes.Text && Handler != null)
            {
                col.Add(new ConsoleText(" \b "));
                return;
            }

            var str = new StringBuilder();
            str.Append('\b', count);
            switch (keepLevel)
            {
                case 0:
                    str.Append(' ', count);
                    str.Append('\b', count);
                    break;
                case 1:
                    str.Append(' ', count);
                    break;
            }

            col.Add(new ConsoleText(str));
            Flush();
        }

        private void ClearInCmd(RelativeAreas area)
        {
            switch (area)
            {
                case RelativeAreas.Line:
                    try
                    {
                        var l = Console.CursorLeft;
                        BackspaceInternal(l, 1);
                        Console.CursorLeft = Console.BufferWidth - 1;
                        BackspaceInternal(Console.BufferWidth, 1);
                        Console.SetCursorPosition(l, Console.CursorTop - 1);
                    }
                    catch (SecurityException)
                    {
                        BackspaceInternal(200, 1);
                    }

                    break;
                case RelativeAreas.ToBeginningOfLine:
                    try
                    {
                        BackspaceInternal(Console.CursorLeft, 1);
                    }
                    catch (SecurityException)
                    {
                        BackspaceInternal(200, 1);
                    }

                    break;
                case RelativeAreas.ToEndOfLine:
                    {
                        var w = Console.BufferWidth - Console.CursorLeft - 1;
                        Console.CursorLeft = Console.BufferWidth - 1;
                        BackspaceInternal(w);
                        break;
                    }
                case RelativeAreas.EntireScreen:
                    try
                    {
                        var l = Console.CursorLeft;
                        var t = Console.CursorTop;
                        Console.SetCursorPosition(Console.BufferWidth - 1, Math.Max(0, Console.CursorTop - 30));
                        for (; Console.CursorTop <= t;)
                        {
                            Console.CursorLeft = Console.BufferWidth - 1;
                            BackspaceInternal(Console.BufferWidth, 1);
                        }

                        var i = 0;
                        for (; i < 12; i++)
                        {
                            Console.CursorLeft = Console.BufferWidth - 1;
                            BackspaceInternal(Console.BufferWidth, 1);
                        }

                        Console.SetCursorPosition(l, Math.Max(0, Console.CursorTop - i - 1));
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (SecurityException)
                    {
                        BackspaceInternal(200, 1);
                    }

                    break;
                case RelativeAreas.ToBeginningOfScreen:
                    try
                    {
                        var l = Console.CursorLeft;
                        var t = Console.CursorTop;
                        Console.SetCursorPosition(Console.BufferWidth - 1, Math.Max(0, Console.CursorTop - 30));
                        for (; Console.CursorTop < t;)
                        {
                            Console.CursorLeft = Console.BufferWidth - 1;
                            BackspaceInternal(Console.BufferWidth, 1);
                        }

                        Console.CursorLeft = l;
                        BackspaceInternal(Console.CursorLeft, 1);
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (SecurityException)
                    {
                        BackspaceInternal(200, 1);
                    }

                    break;
                case RelativeAreas.ToEndOfScreen:
                    {
                        var l = Console.CursorLeft;
                        var w = Console.BufferWidth - Console.CursorLeft - 1;
                        Console.CursorLeft = Console.BufferWidth - 1;
                        BackspaceInternal(w, 1);
                        Console.CursorTop++;
                        var i = 0;
                        for (; i < 12; i++)
                        {
                            Console.CursorLeft = Console.BufferWidth - 1;
                            BackspaceInternal(Console.BufferWidth, 1);
                        }

                        Console.SetCursorPosition(l, Math.Max(0, Console.CursorTop - i - 1));
                        break;
                    }
                case RelativeAreas.EntireBuffer:
                    Console.Clear();
                    break;
            }
        }

        private void TestMode()
        {
            if (Mode != 0 || Handler is not null)
                return;
#if NET5_0_OR_GREATER
            if (!OperatingSystem.IsWindows())
            {
                Mode = OperatingSystem.IsBrowser() ? Modes.Text : Modes.Ansi;
                return;
            }
#elif NETCOREAPP || NETSTANDARD
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    Mode = Modes.Ansi;
                    return;
            }
#endif
            var left = -1;
            try
            {
                left = Console.CursorLeft;
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (IOException)
            {
            }
            catch (NotImplementedException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
            }

            if (left < 0)
            {
                Mode = Modes.Text;
                return;
            }

            var s = AnsiCodeGenerator.Background(true);
            Console.Write(s);
            if (left == Console.CursorLeft)
            {
                Mode = Modes.Ansi;
                return;
            }

            try
            {
                Console.CursorLeft = left;
                Mode = Modes.Cmd;
                var sb = new StringBuilder();
                sb.Append('\b', s.Length);
                sb.Append(' ', s.Length);
                sb.Append('\b', s.Length);
                Console.Write(sb.ToString());
                return;
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (IOException)
            {
            }
            catch (NotImplementedException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
            }

            Console.WriteLine();
            Mode = Modes.Text;
        }

        private void OnAppend()
        {
            if (!IsAutoFlushDisabled) Flush();
        }
    }
}
