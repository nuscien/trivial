using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The style of progress line console component.
    /// </summary>
    public class ConsoleProgressStyle
    {
        /// <summary>
        /// Progress sizes (width).
        /// </summary>
        public enum Sizes
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
        public enum Kinds
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
        public ConsoleColor? BackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of the component.
        /// </summary>
        public Color? BackgroundRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the progress background color.
        /// </summary>
        public ConsoleColor PendingConsoleColor { get; set; } = ConsoleColor.DarkGray;

        /// <summary>
        /// Gets or sets the progress background color.
        /// </summary>
        public Color? PendingRgbColor { get; set; } = Color.FromArgb(68, 68, 68);

        /// <summary>
        /// Gets or sets the progress bar color.
        /// </summary>
        public ConsoleColor BarConsoleColor { get; set; } = ConsoleColor.Green;
        /// <summary>
        /// Gets or sets the progress bar color.
        /// </summary>
        public Color? BarRgbColor { get; set; } = Color.FromArgb(48, 192, 128);

        /// <summary>
        /// Gets or sets the error color.
        /// </summary>
        public ConsoleColor ErrorConsoleColor { get; set; } = ConsoleColor.Red;

        /// <summary>
        /// Gets or sets the error color.
        /// </summary>
        public Color? ErrorRgbColor { get; set; } = Color.FromArgb(212, 48, 48);

        /// <summary>
        /// Gets or sets the foreground color of caption.
        /// </summary>
        public ConsoleColor? CaptionConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of caption.
        /// </summary>
        public Color? CaptionRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of value.
        /// </summary>
        public ConsoleColor? ValueConsoleColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the foreground color of value.
        /// </summary>
        public Color? ValueRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the progress size (width).
        /// </summary>
        public Sizes Size { get; set; }

        /// <summary>
        /// Gets or sets the progress style.
        /// </summary>
        public Kinds Kind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove the white space between caption and progress.
        /// </summary>
        public bool IgnoreCaptionSeparator { get; set; }
    }
}
