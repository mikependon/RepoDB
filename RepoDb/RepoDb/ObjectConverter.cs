using System;

namespace RepoDb
{
    /// <summary>
    /// A class used for converting an object.
    /// </summary>
    internal static class ObjectConverter
    {
        /// <summary>
        /// Converts a value to null if the value is equals to <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
        /// <returns>The converted value.</returns>
        public static object DbNullToNull(object value)
        {
            return ReferenceEquals(DBNull.Value, value) ? null : value;
        }

        /// <summary>
        /// Converts a value to a target type if the value is equals to null or <see cref="DBNull.Value"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
        /// <returns>The converted value.</returns>
        public static T ToType<T>(object value)
        {
            if (value is T)
            {
                return (T)value;
            }
            return DbNullToNull(value) == null ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
