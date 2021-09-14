using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int? MinLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length for each item.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum column count to display.
        /// </summary>
        public int? Column { get; set; }

        /// <summary>
        /// Gets or sets maximum row count per page.
        /// null for disable paging.
        /// </summary>
        public int? MaxRow { get; set; }

        /// <summary>
        /// Gets or sets the tips.
        /// null for disable tips.
        /// </summary>
        public string Tips { get; set; } = Resource.SelectionTips;

        /// <summary>
        /// Gets or sets the paging tips.
        /// Or null to disable tips.
        /// </summary>
        public string PagingTips { get; set; } = "← [PgUp] | {from} - {end} / {total} | [PgDn] →";

        /// <summary>
        /// Gets or sets the question message for keyboard selecting.
        /// Or null to disable additional question line.
        /// </summary>
        public string Question { get; set; } = Resource.ToSelect;

        /// <summary>
        /// Gets or sets the question message for manual typing.
        /// Or null to disable manual mode.
        /// </summary>
        public string ManualQuestion { get; set; }

        /// <summary>
        /// Gets or sets the question message displayed when it is not supported.
        /// Or null to disable manual mode.
        /// </summary>
        public string QuestionWhenNotSupported { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item.
        /// </summary>
        public ConsoleColor? ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for item.
        /// </summary>
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        public ConsoleColor? SelectedForegroundConsoleColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        public Color? SelectedForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        public ConsoleColor? SelectedBackgroundConsoleColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Gets or sets the foreground color for item selected.
        /// </summary>
        public Color? SelectedBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for question.
        /// </summary>
        public ConsoleColor? QuestionForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for question.
        /// </summary>
        public Color? QuestionForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for question.
        /// </summary>
        public ConsoleColor? QuestionBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for question.
        /// </summary>
        public Color? QuestionBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for tips.
        /// </summary>
        public ConsoleColor? TipsForegroundConsoleColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the foreground color for tips.
        /// </summary>
        public Color? TipsForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for tips.
        /// </summary>
        public ConsoleColor? TipsBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for tips.
        /// </summary>
        public Color? TipsBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for paing tips.
        /// </summary>
        public ConsoleColor? PagingForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for paing tips.
        /// </summary>
        public Color? PagingForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for paging tips.
        /// </summary>
        public ConsoleColor? PagingBackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for paging tips.
        /// </summary>
        public Color? PagingBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for default value.
        /// </summary>
        public ConsoleColor? ItemForegroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for default value.
        /// </summary>
        public Color? ItemForegroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for default value.
        /// </summary>
        public ConsoleColor? ItemBackgroundConsoleColor { get; set; }
        /// <summary>
        /// Gets or sets the background color for default value.
        /// </summary>
        public Color? ItemBackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the prefix for the item.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the prefix for the item selected.
        /// </summary>
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
