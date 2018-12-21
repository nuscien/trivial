using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Web
{
    /// <summary>
    /// Web utilities.
    /// </summary>
    public static class WebExtension
    {
        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime ParseDate(long tick)
        {
            var time = new DateTime(tick * 10000 + 621355968000000000, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(long? tick)
        {
            if (!tick.HasValue) return null;
            return ParseDate(tick.Value);
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long ParseDate(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long? ParseDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }
    }
}
