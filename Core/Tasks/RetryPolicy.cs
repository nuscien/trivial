using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Tasks
{
    /// <summary>
    /// Retry policy.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Creates an instance of action retry.
        /// </summary>
        /// <returns>A processing retry instance.</returns>
        RetryInstance CreateInstance();
    }

    /// <summary>
    /// The instance for action retry.
    /// </summary>
    internal class InternalRetryInstance : RetryInstance
    {
        internal Func<TimeSpan?> next;

        /// <summary>
        /// Gets the waiting time span for next retry; or null, if no more retry.
        /// </summary>
        /// <returns>A time span for next retry; or null, if no more retry.</returns>
        protected override TimeSpan? GetNextSpan()
        {
            return next?.Invoke();
        }
    }

    /// <summary>
    /// The instance for action retry.
    /// </summary>
    public abstract class RetryInstance
    {
        private readonly List<DateTime> list = new List<DateTime>();

        /// <summary>
        /// The processing date time list.
        /// </summary>
        public IReadOnlyList<DateTime> ProcessTime => list.AsReadOnly();

        /// <summary>
        /// Gets the waiting time span for next retry; or null, if no more retry.
        /// </summary>
        /// <returns>A time span for next retry; or null, if no more retry.</returns>
        public TimeSpan? Next()
        {
            try
            {
                var span = GetNextSpan();
                if (span == null) return null;
                list.Add(DateTime.Now);
                return span;
            }
            catch (InvalidOperationException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (IndexOutOfRangeException)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the time span for next retry; or null, for no more retry.
        /// </summary>
        /// <returns>The time span for next retry; or null, for no more retry.</returns>
        protected abstract TimeSpan? GetNextSpan();
    }

    /// <summary>
    /// The linear retry policy.
    /// </summary>
    public class LinearRetryPolicy : IRetryPolicy
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
        /// <param name="increase">The increase per action.</param>
        public LinearRetryPolicy(int count, TimeSpan interval, TimeSpan increase) : this(count, interval) => Increase = increase;

        /// <summary>
        /// The retry count.
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// The interval between two actions.
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The increase per action.
        /// </summary>
        public TimeSpan Increase { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Creates an instance of action retry.
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
    }

    /// <summary>
    /// The function customized retry policy.
    /// </summary>
    public class CustomizedRetryPolicy : IRetryPolicy
    {
        private readonly Func<IReadOnlyList<DateTime>, TimeSpan?> next;

        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        /// <param name="nextHandler">The handler to gets the waiting time span for next retry or null for no retry.</param>
        public CustomizedRetryPolicy(Func<IReadOnlyList<DateTime>, TimeSpan?> nextHandler) => next = nextHandler;

        /// <summary>
        /// Creates an instance of action retry.
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
}
