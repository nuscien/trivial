using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The options of progress line console component.
    /// </summary>
    public class ProgressLineOptions
    {
        internal static readonly ProgressLineOptions Empty = new ProgressLineOptions
        {
            Style = Styles.None
        };

        /// <summary>
        /// Progress styles.
        /// </summary>
        public enum Styles
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
        /// Gets or sets the progress style.
        /// </summary>
        public Styles Style { get; set; }
    }

    /// <summary>
    /// The result.
    /// </summary>
    public class ProgressLineResult : IProgress<double>
    {
        /// <summary>
        /// Gets the current value of the updated progress.
        /// </summary>
        public double Value { get; private set; } = 0;

        /// <summary>
        /// Gets a value indicating whether it is completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it is successful.
        /// </summary>
        public bool IsSuccessful => IsCompleted && Value == 1;

        /// <summary>
        /// Gets a value indicating whether it is failed.
        /// </summary>
        public bool IsFailed => IsCompleted && Value < 1;

        /// <summary>
        /// The event handler raised for each reported progress value.
        /// </summary>
        public event EventHandler<double> ProgressChanged;

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress. It should be 0 - 1.</param>
        public void Report(double value)
        {
            if (IsCompleted || double.IsNaN(value)) return;
            if (value >= 1)
            {
                Value = 1;
                IsCompleted = true;
            }
            else if (value < 0)
            {
                Value = 0;
            }
            else
            {
                Value = value;
            }

            ProgressChanged?.Invoke(this, Value);
        }

        /// <summary>
        /// Increases a percent.
        /// </summary>
        public void Increase()
        {
            Report(Value + 0.01);
        }

        /// <summary>
        /// Reports the progress update to 100%. It means that runs succeeded.
        /// </summary>
        public void Succeed()
        {
            Report(1);
        }

        /// <summary>
        /// Stops and marks as error.
        /// </summary>
        public void Fail()
        {
            IsCompleted = true;
            ProgressChanged?.Invoke(this, Value);
        }

        /// <summary>
        /// Resets the status.
        /// </summary>
        /// <param name="value">Optional value of the updated progress to set.</param>
        public void ResetStatus(double value = double.NaN)
        {
            IsCompleted = false;
            Report(value);
        }
    }
}
