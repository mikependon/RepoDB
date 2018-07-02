using System;

namespace RepoDb
{
    /// <summary>
    /// A class used for converting an object.
    /// </summary>
    internal static class ObjectConverter
    {
        /// <summary>
        /// Converts a value to NULL if the value is equals to <i>System.DBNull.Value</i>, otherwise, returns the object value.
        /// </summary>
        /// <param name="value">The value to be checked for <i>System.DbNull.Value</i> value.</param>
        /// <returns>The converted value.</returns>
        public static object DbNullToNull(object value)
        {
            return ReferenceEquals(value, DBNull.Value) ? null : value;
        }
    }
}
