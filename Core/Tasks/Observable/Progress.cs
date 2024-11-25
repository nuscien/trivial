using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The progress from zero (0) to one (1) in double-floating number.
/// </summary>
public class OneProgress : IProgress<double>, INotifyPropertyChanged
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
    /// Gets a value indicating whether it is not supported.
    /// </summary>
    public bool IsNotSupported { get; internal set; }

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
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Reports a progress update.
    /// The status will change automatically when the value is 1.
    /// </summary>
    /// <param name="value">The value of the updated progress. It should be 0 - 1.</param>
    public void Report(double value)
    {
        if (IsCompleted || double.IsNaN(value) || value == Value) return;
        if (value >= 1)
        {
            Value = 1;
            IsCompleted = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }

    /// <summary>
    /// Reports a progress update and enforce the status is processing.
    /// </summary>
    /// <param name="value">The value of the updated progress. It should be 0 - 1.</param>
    public void ReportProcessing(double value)
    {
        if (IsCompleted || double.IsNaN(value) || value == Value) return;
        if (value >= 1)
        {
            Value = 1;
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
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
    /// Increases value in a given time inteval.
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

            Report(max);
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
            await Task.WhenAny(task, Task.Delay(delay));
            Increase(delta);
            if (Value <= 0) break;
            delay += millisecondsIncrease;
            if (!task.IsCompleted) continue;
            if (task.IsFaulted)
            {
                Fail();
                break;
            }

            Report(max);
            return true;
        }

        var result = task.IsCompleted;
        if (result) Report(max);
        return result;
    }

    /// <summary>
    /// Increases value in a given time inteval.
    /// </summary>
    /// <param name="task">The task to wait.</param>
    /// <param name="validation">The result validation handler for task.</param>
    /// <param name="delta">The delta value to update.</param>
    /// <param name="max">The maximum value to update.</param>
    /// <param name="millisecondsDelay">The milliseconds to delay per checking.</param>
    /// <param name="millisecondsIncrease">The increase milliseconds for each delay.</param>
    /// <returns>true if the task is completed; otherwise, false.</returns>
    public async Task<bool> IncreaseAsync<T>(Task<T> task, Func<T, bool> validation, double delta, double max, int millisecondsDelay, int millisecondsIncrease = 0)
    {
        if (max > 1) max = 1;
        if (validation == null) validation = t => true;
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

            Report(max);
            return true;
        }

        if (delta == 0)
        {
            try
            {
                var r = await task;
                if (validation(r))
                {
                    Report(max);
                    return true;
                }
            }
            catch (Exception)
            {
            }

            Fail();
            return false;
        }

        while (Value < max)
        {
            await Task.WhenAny(task, Task.Delay(delay));
            Increase(delta);
            if (Value <= 0) break;
            delay += millisecondsIncrease;
            if (!task.IsCompleted) continue;
            if (task.IsFaulted || !validation(task.Result))
            {
                Fail();
                break;
            }

            Report(max);
            return true;
        }

        if (Value > max) Report(max);
        return false;
    }

    /// <summary>
    /// Increases value in a given time inteval.
    /// </summary>
    /// <param name="task">The task to wait.</param>
    /// <param name="delta">The delta value to update.</param>
    /// <param name="max">The maximum value to update.</param>
    /// <param name="delay">The time span to delay per checking.</param>
    /// <returns>true if the task is completed; otherwise, false.</returns>
    public async Task<bool> IncreaseAsync(Task task, double delta, double max, TimeSpan delay)
    {
        if (max > 1) max = 1;
        if (task == null)
        {
            if (delta == 0) return false;
            while (Value < max)
            {
                await Task.Delay(delay);
                Increase(delta);
                if (Value <= 0) return false;
            }

            Report(max);
            return true;
        }

        while (Value < max)
        {
            await Task.WhenAny(task, Task.Delay(delay));
            Increase(delta);
            if (Value <= 0) break;
            if (!task.IsCompleted) continue;
            if (task.IsFaulted)
            {
                Fail();
                break;
            }

            Report(max);
            return true;
        }

        if (Value > max) Report(max);
        return false;
    }

    /// <summary>
    /// Increases value in a given time inteval.
    /// </summary>
    /// <param name="task">The task to wait.</param>
    /// <param name="validation">The result validation handler for task.</param>
    /// <param name="delta">The delta value to update.</param>
    /// <param name="max">The maximum value to update.</param>
    /// <param name="delay">The time span to delay per checking.</param>
    /// <returns>true if the task is completed; otherwise, false.</returns>
    public async Task<bool> IncreaseAsync<T>(Task<T> task, Func<T, bool> validation, double delta, double max, TimeSpan delay)
    {
        if (max > 1) max = 1;
        if (task == null)
        {
            if (delta == 0) return false;
            while (Value < max)
            {
                await Task.Delay(delay);
                Increase(delta);
                if (Value <= 0) return false;
            }

            Report(max);
            return true;
        }

        while (Value < max)
        {
            await Task.WhenAny(task, Task.Delay(delay));
            Increase(delta);
            if (Value <= 0) break;
            if (!task.IsCompleted) continue;
            if (task.IsFaulted || !validation(task.Result))
            {
                Fail();
                break;
            }

            Report(max);
            return true;
        }

        if (Value > max) Report(max);
        return false;
    }

    /// <summary>
    /// Reports the progress update to 100%. It means that runs succeeded.
    /// </summary>
    public void Succeed()
        => Report(1);

    /// <summary>
    /// Stops and marks as error.
    /// </summary>
    public void Fail()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        ProgressChanged?.Invoke(this, Value);
    }

    /// <summary>
    /// Resets the status to processing.
    /// </summary>
    /// <param name="value">Optional value of the updated progress to set.</param>
    public void ResetStatus(double value = double.NaN)
    {
        var wasCompleted = IsCompleted;
        IsCompleted = false;
        ReportProcessing(value);
        if (wasCompleted != IsCompleted) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        if (IsCompleted) return IsSuccessful ? "√" : $"× ({Value:#0.0%})";
        return Value.ToString("#0.0%");
    }

    /// <summary>
    /// Converts the progress result to a floating-point number.
    /// </summary>
    /// <param name="progress">The progress result value.</param>
    /// <returns>The current value of the updated progress.</returns>
    public static explicit operator double(OneProgress progress)
        => progress == null ? 0 : progress.Value;

    /// <summary>
    /// Converts the progress result to a floating-point number.
    /// </summary>
    /// <param name="progress">The progress result value.</param>
    /// <returns>The current value of the updated progress.</returns>
    public static explicit operator float(OneProgress progress)
        => progress == null ? 0 : (float)progress.Value;

    /// <summary>
    /// Converts the progress result to a boolean value.
    /// </summary>
    /// <param name="progress">The progress result value.</param>
    /// <returns>A value indicating whether it is completed.</returns>
    public static explicit operator bool(OneProgress progress)
        => progress != null && progress.IsCompleted;
}
