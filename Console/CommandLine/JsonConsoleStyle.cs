﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The style for JSON output.
    /// </summary>
    public class JsonConsoleStyle
    {
        /// <summary>
        /// Gets or sets the foreground color of string.
        /// </summary>
        public Color? StringForegroundRgbColor { get; set; } = Color.FromArgb(0xCE, 0x91, 0x78);

        /// <summary>
        /// Gets or sets the foreground color of language keyword.
        /// </summary>
        public Color? KeywordForegroundRgbColor { get; set; } = Color.FromArgb(0x56, 0x9C, 0xD6);

        /// <summary>
        /// Gets or sets the foreground color of number.
        /// </summary>
        public Color? NumberForegroundRgbColor { get; set; } = Color.FromArgb(0xB5, 0xCE, 0xA8);

        /// <summary>
        /// Gets or sets the foreground color of punctuation.
        /// </summary>
        public Color? PunctuationForegroundRgbColor { get; set; } = Color.FromArgb(0xDC, 0xDC, 0xDC);

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color? BackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of language keyword.
        /// </summary>
        public ConsoleColor? StringForegroundConsoleColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the foreground color of number.
        /// </summary>
        public ConsoleColor? KeywordForegroundConsoleColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the foreground color of punctuation.
        /// </summary>
        public ConsoleColor? NumberForegroundConsoleColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the foreground color of string.
        /// </summary>
        public ConsoleColor? PunctuationForegroundConsoleColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public ConsoleColor? BackgroundConsoleColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        public ConsoleText CreateText(bool value)
            => CreateByKeyword(value ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString);

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        public ConsoleText CreateText(int value)
            => new(
                value.ToString("g"),
                NumberForegroundRgbColor,
                NumberForegroundConsoleColor,
                BackgroundRgbColor,
                BackgroundConsoleColor);

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        public ConsoleText CreateText(string value)
        {
            return value == null ? CreateByKeyword(JsonValues.Null.ToString()) : new(
                JsonStringNode.ToJson(value),
                StringForegroundRgbColor,
                StringForegroundConsoleColor,
                BackgroundRgbColor,
                BackgroundConsoleColor);
        }

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="json">The JSON instance.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A console text instance.</returns>
        internal List<ConsoleText> CreateTextCollection(IJsonDataNode json, int indentLevel = 0)
        {
            var cmd = new List<ConsoleText>();
            if (json == null)
            {
                cmd.Add(CreateByKeyword(JsonValues.Null.ToString()));
                return cmd;
            }

            switch (json.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    cmd.Add(CreateText(null));
                    break;
                case JsonValueKind.String:
                    cmd.Add(new(json.ToString(),
                        StringForegroundRgbColor,
                        StringForegroundConsoleColor,
                        BackgroundRgbColor,
                        BackgroundConsoleColor));
                    break;
                case JsonValueKind.Number:
                    cmd.Add(new(json.ToString(),
                        NumberForegroundRgbColor,
                        NumberForegroundConsoleColor,
                        BackgroundRgbColor,
                        BackgroundConsoleColor));
                    break;
                case JsonValueKind.True:
                    cmd.Add(CreateText(true));
                    break;
                case JsonValueKind.False:
                    cmd.Add(CreateText(false));
                    break;
                case JsonValueKind.Object:
                    cmd.AddRange(CreateTextCollection(json as JsonObjectNode, indentLevel));
                    break;
                case JsonValueKind.Array:
                    cmd.AddRange(CreateTextCollection(json as JsonArrayNode, indentLevel));
                    break;
                default:
                    break;
            }

            return cmd;
        }

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="json">The JSON instance.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A console text instance.</returns>
        private List<ConsoleText> CreateTextCollection(JsonObjectNode json, int indentLevel)
        {
            var cmd = new List<ConsoleText>();
            if (json == null)
            {
                cmd.Add(CreateByKeyword(JsonValues.Null.ToString()));
                return cmd;
            }

            var spaces = CreateByWhitespace(Environment.NewLine + new string(' ', (indentLevel + 1) * 2));
            cmd.Add(CreateByPunctuation("{"));
            foreach (var prop in json)
            {
                cmd.Add(spaces);
                cmd.Add(CreateText(prop.Key));
                cmd.Add(CreateByPunctuation(": "));
                cmd.AddRange(CreateTextCollection(prop.Value, indentLevel + 1));
                cmd.Add(CreateByPunctuation(","));
            }

            if (cmd.Count > 1) cmd.RemoveAt(cmd.Count - 1);
            cmd.Add(CreateByWhitespace(Environment.NewLine));
            cmd.Add(CreateByWhitespace(new string(' ', indentLevel * 2)));
            cmd.Add(CreateByPunctuation("}"));
            return cmd;
        }

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="json">The JSON instance.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A console text instance.</returns>
        private List<ConsoleText> CreateTextCollection(JsonArrayNode json, int indentLevel)
        {
            var cmd = new List<ConsoleText>();
            if (json == null)
            {
                cmd.Add(CreateByKeyword(JsonValues.Null.ToString()));
                return cmd;
            }

            var spaces = CreateByWhitespace(Environment.NewLine + new string(' ', (indentLevel + 1) * 2));
            cmd.Add(CreateByPunctuation("{"));
            foreach (var prop in json)
            {
                cmd.Add(spaces);
                cmd.AddRange(CreateTextCollection(prop, indentLevel + 1));
                cmd.Add(CreateByPunctuation(","));
            }

            if (cmd.Count > 1) cmd.RemoveAt(cmd.Count - 1);
            cmd.Add(CreateByWhitespace(Environment.NewLine));
            cmd.Add(CreateByWhitespace(new string(' ', indentLevel * 2)));
            cmd.Add(CreateByPunctuation("}"));
            return cmd;
        }

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        private ConsoleText CreateByKeyword(string value)
            => new(
                value,
                KeywordForegroundRgbColor,
                KeywordForegroundConsoleColor,
                BackgroundRgbColor,
                BackgroundConsoleColor);

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        private ConsoleText CreateByPunctuation(string value)
            => new(
                value,
                PunctuationForegroundRgbColor,
                PunctuationForegroundConsoleColor,
                BackgroundRgbColor,
                BackgroundConsoleColor);

        /// <summary>
        /// Creates a console text by this style.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A console text instance.</returns>
        private ConsoleText CreateByWhitespace(string value)
            => new(
                value,
                null,
                null,
                BackgroundRgbColor,
                BackgroundConsoleColor);
    }
}
