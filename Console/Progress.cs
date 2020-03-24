using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Console
{
    /// <summary>
    /// The options of progress line console component.
    /// </summary>
    public class ProgressLineOptions
    {
        /// <summary>
        /// The empty options.
        /// </summary>
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
    /// The progress result.
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
            if (IsCompleted || double.IsNaN(value) || value == Value) return;
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
        /// <param name="delta">The delta value to update.</param>
        public void Increase(double delta = 0.01)
        {
            Report(Value + delta);
        }

        /// <summary>
        /// Increases
        /// </summary>
        /// <param name="task">The task to wait.</param>
        /// <param name="delta">The delta value to update.</param>
        /// <param name="max">The maximum value to update.</param>
        /// <param name="millisecondsDelay">The milliseconds to delay per checking.</param>
        /// <param name="millisecondsIncrease">The increase milliseconds for each delay.</param>
        /// <returns>true if the task is completed; otherwise, false.</returns>
        public async Task<bool> IncreaseAsync(Task task, double delta, double max, int millisecondsDelay, int millisecondsIncrease = 0)
        {
            if (max > 1) max = 1;
            var delay = millisecondsDelay;
            if (task == null)
            {
                if (delta == 0) return false;
                while (Value < max)
                {
                    await Task.Delay(delay);
                    Increase(delta);
                    if (Value <= 0) return false;
                    delay += millisecondsIncrease;
                }

                return true;
            }

            if (delta == 0)
            {
                try
                {
                    await task;
                }
                catch (Exception)
                {
                    Fail();
                    return false;
                }

                Report(max);
                return true;
            }
            
            while (Value < max)
            {
                if (task.IsCompleted)
                {
                    Report(max);
                    return true;
                }

                try
                {
                    await Task.WhenAny(task, Task.Delay(delay));
                }
                catch (Exception)
                {
                    Fail();
                    return false;
                }

                Increase(delta);
                if (Value <= 0) break;
                delay += millisecondsIncrease;
            }

            var result = task.IsCompleted;
            if (result) Report(max);
            return result;
        }

        /// <summary>
        /// Increases
        /// </summary>
        /// <param name="task">The task to wait.</param>
        /// <param name="delta">The delta value to update.</param>
        /// <param name="max">The maximum value to update.</param>
        /// <param name="delay">The time span to delay per checking.</param>
        /// <returns>true if the task is completed; otherwise, false.</returns>
        public async Task<bool> IncreaseAsync(Task task, double delta, double max, TimeSpan delay)
        {
            if (max > 1) max = 1;
            while (Value < max)
            {
                if (task.IsCompleted)
                {
                    Report(max);
                    return true;
                }

                await Task.WhenAny(task, Task.Delay(delay));
                Increase(delta);
            }

            return task.IsCompleted;
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
