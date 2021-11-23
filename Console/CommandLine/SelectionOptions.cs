using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The selection options.
    /// </summary>
    public class SelectionConsoleOptions : ICloneable
    {
        /// <summary>
        /// Gets or sets the minimum length for each item.
        /// </summary>
        [JsonPropertyName("minlen")]
        public int? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length for each item.
        /// </summary>
        [JsonPropertyName("maxlen")]
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum column count to display.
        /// </summary>
        [JsonPropertyName("columns")]
        public int? Column { get; set; }

        /// <summary>
        /// Gets or sets maximum row count per page.
        /// null for disable paging.
        /// </summary>
        [JsonPropertyName("rows")]
        public int? MaxRow { get; set; }

        /// <summary>
        /// Gets or sets the tips.
        /// null for disable tips.
        /// </summary>
        [JsonPropertyName("tip")]
        public string Tips { get; set; } = Resource.SelectionTips;

        /// <summary>
        /// Gets or sets the paging tips.
        /// Or null to disable tips.
        /// </summary>
        [JsonPropertyName("pagetip")]
        public string PagingTips { get; set; } = "← [PgUp] | {from} - {end} / {total} | [PgDn] →";

        /// <summary>
        /// Gets or sets the question message for keyboard selecting.
        /// Or null to disable additional question line.
        /// </summary>
        [JsonPropertyName("q")]
        public string Question { get; set; } = Resource.ToSelect;

        /// <summary>
        /// Gets or sets the question message for manual typing.
        /// Or null to disable manual mode.
        /// </summary>
        [JsonPropertyName("manualq")]
        public string ManualQuestion { get; set; }

        /// <summary>
        /// Gets or sets the question message displayed when it is not supported.
        /// Or null to disable manual mode.
        /// </summary>
        [JsonPropertyName("notsupportq")]
        public string QuestionWhenNotSupported { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item.
        /// </summary>
        [JsonPropertyName("fore")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for item.
        /// </summary>
        [JsonPropertyName("back")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        [JsonPropertyName("selfore2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? SelectedForegroundConsoleColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        [JsonPropertyName("selfore")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? SelectedForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        [JsonPropertyName("selback2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? SelectedBackgroundConsoleColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        [JsonPropertyName("selback")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? SelectedBackgroundRgbColor { get; set; } = Color.FromArgb(0x55, 0xCC, 0xEE);

        /// <summary>
        /// Gets or sets the foreground color for question.
        /// </summary>
        [JsonPropertyName("qfore2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? QuestionForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for question.
        /// </summary>
        [JsonPropertyName("qfore")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? QuestionForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for question.
        /// </summary>
        [JsonPropertyName("qback2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? QuestionBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for question.
        /// </summary>
        [JsonPropertyName("qback")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? QuestionBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for tips.
        /// </summary>
        [JsonPropertyName("tipfore2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? TipsForegroundConsoleColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the foreground color for tips.
        /// </summary>
        [JsonPropertyName("tipfore")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? TipsForegroundRgbColor { get; set; } = Color.FromArgb(0xF9, 0xEE, 0x88);

        /// <summary>
        /// Gets or sets the background color for tips.
        /// </summary>
        [JsonPropertyName("tipback2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? TipsBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for tips.
        /// </summary>
        [JsonPropertyName("tipback")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? TipsBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for paing tips.
        /// </summary>
        [JsonPropertyName("pagefore2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? PagingForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for paing tips.
        /// </summary>
        [JsonPropertyName("pagefore")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? PagingForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for paging tips.
        /// </summary>
        [JsonPropertyName("pageback2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? PagingBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for paging tips.
        /// </summary>
        [JsonPropertyName("pageback")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? PagingBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for default value.
        /// </summary>
        [JsonPropertyName("itemfore2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? ItemForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for default value.
        /// </summary>
        [JsonPropertyName("itemfore")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? ItemForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for default value.
        /// </summary>
        [JsonPropertyName("itemback2")]
        [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
        public ConsoleColor? ItemBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for default value.
        /// </summary>
        [JsonPropertyName("itemback")]
        [JsonConverter(typeof(JsonNumberConverter))]
        public Color? ItemBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the prefix for the item.
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the prefix for the item selected.
        /// </summary>
        [JsonPropertyName("selprefix")]
        public string SelectedPrefix { get; set; }

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        public virtual SelectionConsoleOptions Clone()
            => MemberwiseClone() as SelectionConsoleOptions;

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        object ICloneable.Clone()
            => MemberwiseClone();
    }
}
