using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the progress background color.
        /// </summary>
        public ConsoleColor PendingColor { get; set; } = ConsoleColor.DarkGray;

        /// <summary>
        /// Gets or sets the progress bar color.
        /// </summary>
        public ConsoleColor BarColor { get; set; } = ConsoleColor.Green;

        /// <summary>
        /// Gets or sets the error color.
        /// </summary>
        public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;

        /// <summary>
        /// Gets or sets the foreground color of caption.
        /// </summary>
        public ConsoleColor? CaptionColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of value.
        /// </summary>
        public ConsoleColor? ValueColor { get; set; } = ConsoleColor.Gray;

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
