using System;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Removes the milliseconds value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> object to be flattened.</param>
        /// <returns>The flatted <see cref="DateTime"/> value.</returns>
        public static DateTime Flatten(this DateTime datetime)
        {
            return new DateTime(datetime.Year,
                datetime.Month,
                datetime.Day,
                datetime.Hour,
                datetime.Minute,
                datetime.Second);
        }
    }
}
