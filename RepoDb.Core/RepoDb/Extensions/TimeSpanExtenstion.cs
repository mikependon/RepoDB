using System;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Removes the milliseconds value.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> object to be flattened.</param>
        /// <returns>The flatted <see cref="TimeSpan"/> value.</returns>
        public static TimeSpan Flatten(this TimeSpan timeSpan) =>
            new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
