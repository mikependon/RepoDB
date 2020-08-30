using System;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Object"/>.
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Converts an object to a <see cref="long"/>.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="long"/> value of the object.</returns>
        internal static long ToNumber(this object value) =>
            Convert.ToInt64(value);
    }
}
