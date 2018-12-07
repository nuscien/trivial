using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Tasks
{
    /// <summary>
    /// The retry policy.
    /// </summary>
    public class RetryPolicy
    {
        /// <summary>
        /// The interval between two actions.
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The increase per action.
        /// </summary>
        public TimeSpan Increase { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The retry count.
        /// </summary>
        public int Count { get; set; } = 0;
    }
}
