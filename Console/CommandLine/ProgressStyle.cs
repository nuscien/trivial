using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The style of progress line console component.
/// </summary>
public class ConsoleProgressStyle
{
    /// <summary>
    /// Progress sizes (width).
    /// </summary>
    public enum Sizes : byte
    {
        /// <summary>
        /// Normal size.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Short size.
        /// </summary>
        Short = 1,

        /// <summary>
        /// Wide size.
        /// </summary>
        Wide = 2,

        /// <summary>
        /// The progress and its related text will stretch horizontal in the console.
        /// </summary>
        Full = 3,

        /// <summary>
        /// No progress bar but only a value.
        /// </summary>
        None = 4
    }

    /// <summary>
    /// The output text kinds filling in progress.
    /// </summary>
    public enum Kinds : byte
    {
        /// <summary>
        /// Whitespace (rectangle).
        /// </summary>
        Full = 0,

        /// <summary>
        /// Left angle bracket (less sign).
        /// </summary>
        AngleBracket = 1,

        /// <summary>
        /// Plus sign.
        /// </summary>
        Plus = 2,

        /// <summary>
        /// Sharp.
        /// </summary>
        Sharp = 3,

        /// <summary>
        /// Character x.
        /// </summary>
        X = 4,

        /// <summary>
        /// Character o.
        /// </summary>
        O = 5
    }

    /// <summary>
    /// Gets or sets the background color of the component.
    /// </summary>
    [JsonPropertyName("back2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? BackgroundConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the background color of the component.
    /// </summary>
    [JsonPropertyName("back")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? BackgroundRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the progress background color.
    /// </summary>
    [JsonPropertyName("pending2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public ConsoleColor PendingConsoleColor { get; set; } = ConsoleColor.DarkGray;

    /// <summary>
    /// Gets or sets the progress background color.
    /// </summary>
    [JsonPropertyName("pending")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? PendingRgbColor { get; set; } = Color.FromArgb(68, 68, 68);

    /// <summary>
    /// Gets or sets the progress bar color.
    /// </summary>
    [JsonPropertyName("bar2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public ConsoleColor BarConsoleColor { get; set; } = ConsoleColor.Green;

    /// <summary>
    /// Gets or sets the progress bar color.
    /// </summary>
    [JsonPropertyName("bar")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? BarRgbColor { get; set; } = Color.FromArgb(48, 192, 128);

    /// <summary>
    /// Gets or sets the error color.
    /// </summary>
    [JsonPropertyName("error2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public ConsoleColor ErrorConsoleColor { get; set; } = ConsoleColor.Red;

    /// <summary>
    /// Gets or sets the error color.
    /// </summary>
    [JsonPropertyName("error")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? ErrorRgbColor { get; set; } = Color.FromArgb(212, 48, 48);

    /// <summary>
    /// Gets or sets the foreground color of caption.
    /// </summary>
    [JsonPropertyName("caption2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? CaptionConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the foreground color of caption.
    /// </summary>
    [JsonPropertyName("caption")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? CaptionRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the foreground color of value.
    /// </summary>
    [JsonPropertyName("value2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? ValueConsoleColor { get; set; } = ConsoleColor.Gray;

    /// <summary>
    /// Gets or sets the foreground color of value.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? ValueRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the progress size (width).
    /// </summary>
    [JsonPropertyName("size")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Sizes Size { get; set; }

    /// <summary>
    /// Gets or sets the progress style.
    /// </summary>
    [JsonPropertyName("kind")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    public Kinds Kind { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether remove the white space between caption and progress.
    /// </summary>
    [JsonPropertyName("slim")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IgnoreCaptionSeparator { get; set; }
}
