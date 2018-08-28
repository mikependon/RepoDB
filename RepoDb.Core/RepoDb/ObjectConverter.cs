using System;

namespace RepoDb
{
    /// <summary>
    /// A class used for converting an object.
    /// </summary>
    internal static class ObjectConverter
    {
        /// <summary>
        /// Converts a value to NULL if the value is equals to <see cref="DBNull.Value"/>, otherwise, returns the object value.
        /// </summary>
        /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
        /// <returns>The converted value.</returns>
        public static object DbNullToNull(object value)
        {
            return ReferenceEquals(value, DBNull.Value) ? null : value;
        }
    }
}
