﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The way to pick in the interceptor, e.g. invoke the first one, the last one or the whole.
    /// </summary>
    public enum InterceptorModes : byte
    {
        /// <summary>
        /// Execute all actions without any intercept.
        /// </summary>
        Pass = 0,

        /// <summary>
        /// Only execute the first one which meets the policy condition.
        /// </summary>
        Mono = 1,

        /// <summary>
        /// For concurrent pending or invoking, only execute the last one which meets the condition.
        /// And execute the ones which are none-concurrent.
        /// </summary>
        Debounce = 2,

        /// <summary>
        /// Pend the first one to execute until no more invoking concurrently.
        /// </summary>
        Lock = 3
    }

    /// <summary>
    /// The policy of interceptor.
    /// </summary>
    public class InterceptorPolicy : ICloneable, IEquatable<InterceptorPolicy>
    {
        /// <summary>
        /// Gets or sets the minimum count of invoking times in the interceptor couting cycle.
        /// </summary>
        [JsonPropertyName("min")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int MinCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum count of invoking times in the interceptor couting cycle.
        /// null for unlimited.
        /// </summary>
        [JsonPropertyName("max")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxCount { get; set; }

        /// <summary>
        /// Gets or sets the delay time span to execute actually.
        /// </summary>
        [JsonPropertyName("delay")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Gets or sets the timeout time span used to reset counting after the latest invoking.
        /// </summary>
        [JsonPropertyName("timeout")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the duration time span for interceptor couting cycle.
        /// After this from latest invoking, it will reset the count of invoking times to zero.
        /// </summary>
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the mode to control the invoking passing way.
        /// It determines which one invokes during the counting limitation, e.g. the first one, the last one or all.
        /// </summary>
        [JsonPropertyName("mode")]
        public InterceptorModes Mode { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public InterceptorPolicy Clone() => new()
        {
            MinCount = MinCount,
            MaxCount = MaxCount,
            Delay = Delay,
            Timeout = Timeout,
            Duration = Duration,
            Mode = Mode
        };

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(InterceptorPolicy other)
        {
            if (other is null) return false;
            return MinCount == other.MinCount
                && MaxCount == other.MaxCount
                && Delay == other.Delay
                && Timeout == other.Timeout
                && Duration == other.Duration
                && Mode == other.Mode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as InterceptorPolicy);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return new Tuple<int, int?, TimeSpan?, TimeSpan?, TimeSpan?, InterceptorModes>(
                MinCount,
                MaxCount,
                Delay,
                Timeout,
                Duration,
                Mode).GetHashCode();
        }

        /// <summary>
        /// Creates a debounce interceptor policy.
        /// </summary>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The interceptor policy.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static InterceptorPolicy Debounce(TimeSpan delay) => new()
        {
            Mode = InterceptorModes.Debounce,
            Delay = delay
        };

        /// <summary>
        /// Creates a throttle interceptor policy.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>A interceptor policy.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static InterceptorPolicy Throttle(TimeSpan duration) => new()
        {
            Mode = InterceptorModes.Mono,
            Timeout = duration,
            Duration = duration,
            MaxCount = 1
        };

        /// <summary>
        /// Creates a multi-hit interceptor policy.
        /// </summary>
        /// <param name="min">The minmum invoking count.</param>
        /// <param name="max">The maxmum invoking count.</param>
        /// <param name="timeout">The time span between each invoking.</param>
        /// <returns>A interceptor policy.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static InterceptorPolicy Mutliple(int min, int? max, TimeSpan timeout) => new()
        {
            Timeout = timeout,
            MinCount = min,
            MaxCount = max
        };

        /// <summary>
        /// Creates an interceptor policy responded at a specific times.
        /// </summary>
        /// <param name="min">The minmum invoking count.</param>
        /// <param name="max">The maxmum invoking count.</param>
        /// <param name="timeout">The time span between each invoking.</param>
        /// <returns>A interceptor policy.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static InterceptorPolicy Times(int min, int? max, TimeSpan timeout) => new()
        {
            Delay = timeout,
            Timeout = timeout,
            MinCount = min,
            MaxCount = max,
            Mode = InterceptorModes.Debounce
        };

        /// <summary>
        /// Creates an interceptor policy responded at a specific times.
        /// </summary>
        /// <param name="count">The invoking count.</param>
        /// <param name="timeout">The time span between each invoking.</param>
        /// <returns>A interceptor policy.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static InterceptorPolicy Times(int count, TimeSpan timeout) => new()
        {
            Delay = timeout,
            Timeout = timeout,
            MinCount = count,
            Mode = InterceptorModes.Debounce
        };
    }
}
