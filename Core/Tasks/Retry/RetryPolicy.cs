using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// Retry policy.
/// </summary>
public interface IRetryPolicy
{
    /// <summary>
    /// Creates an instance of Handler retry.
    /// </summary>
    /// <returns>A processing retry instance.</returns>
    RetryInstance CreateInstance();
}

/// <summary>
/// The linear retry policy.
/// </summary>
/// <example>
/// <para>
/// You can create this retry policy to process the specific handler within the specific times
/// with the specific time span between two processing.
/// </para>
/// <code>
/// // Initialize the policy instance with 3 times at most to retry and 10s interval for example.
/// var retryPolicy = new LinearRetryPolicy(3, TimeSpan.FromSeconds(10));
///
/// // Following task to throw InvalidOperationException to mock the scenario that it will retry as the condition.
/// var proccessResult = retryPolicy.ProcessAsync(() =>
/// {
///     throw new InvalidOperationException();
/// }, typeof(InvalidOperationException));
/// </code>
/// </example>
public sealed class LinearRetryPolicy : IRetryPolicy
{
    /// <summary>
    /// Initializes a new instance of the CustomizedRetryPolicy class.
    /// </summary>
    public LinearRetryPolicy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the CustomizedRetryPolicy class.
    /// </summary>
    /// <param name="count">The retry count.</param>
    public LinearRetryPolicy(int count) => Count = count;

    /// <summary>
    /// Initializes a new instance of the CustomizedRetryPolicy class.
    /// </summary>
    /// <param name="count">The retry count.</param>
    /// <param name="interval">The interval between two actions.</param>
    public LinearRetryPolicy(int count, TimeSpan interval) : this(count) => Interval = interval;

    /// <summary>
    /// Initializes a new instance of the CustomizedRetryPolicy class.
    /// </summary>
    /// <param name="count">The retry count.</param>
    /// <param name="interval">The interval between two actions.</param>
    /// <param name="increase">The increase per Handler.</param>
    public LinearRetryPolicy(int count, TimeSpan interval, TimeSpan increase) : this(count, interval) => Increase = increase;

    /// <summary>
    /// The maximum count of retry operation.
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// The interval between two actions.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// The increase per Handler.
    /// </summary>
    public TimeSpan Increase { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Creates an instance of Handler retry.
    /// </summary>
    /// <returns>A processing retry instance.</returns>
    public RetryInstance CreateInstance()
    {
        var retry = new InternalRetryInstance();
        retry.next = () =>
        {
            var len = retry.ProcessTime.Count;
            if (len >= Count) return null;
            return TimeSpan.FromTicks(Increase.Ticks * len + Interval.Ticks);
        };
        return retry;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder("RetryPolicy = Linear & Interval = ");
        if (Increase.Ticks > 0)
        {
            sb.Append(Increase.ToString());
            sb.Append(" × n + ");
        }

        sb.Append(Interval.ToString());
        sb.Append(" & MaxCount = ");
        sb.Append(Count);
        return sb.ToString();
    }
}

/// <summary>
/// The customized retry policy by given test function.
/// </summary>
/// <param name="nextHandler">The handler to gets the waiting time span for next retry or null for no retry.</param>
public class CustomizedRetryPolicy(Func<IReadOnlyList<DateTime>, TimeSpan?> nextHandler) : IRetryPolicy
{
    private readonly Func<IReadOnlyList<DateTime>, TimeSpan?> next = nextHandler;

    /// <summary>
    /// Creates an instance of Handler retry.
    /// </summary>
    /// <returns>A processing retry instance.</returns>
    public RetryInstance CreateInstance()
    {
        var retry = new InternalRetryInstance();
        retry.next = () =>
        {
            return next?.Invoke(retry.ProcessTime);
        };
        return retry;
    }
}
