using System;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A class used for converting an object.
    /// </summary>
    public static class ObjectConverter
    {
        /// <summary>
        /// Converts a value to NULL if the value is equals to System.DBNull.Value, otherwise, returns the object value.
        /// </summary>
        /// <param name="value">The value to be checked for System.DbNull.Value value.</param>
        /// <returns>The converted value.</returns>
        public static object DbNullToNull(object value)
        {
            return ReferenceEquals(value, DBNull.Value) ? null : value;
        }

        /// <summary>
        /// Gets a property value from an object.
        /// </summary>
        /// <param name="obj">An object where to the retrieve the property value.</param>
        /// <param name="property">The property of the object.</param>
        /// <returns>A value of the property being held by the object.</returns>
        public static object GetValue(object obj, PropertyInfo property)
        {
            return property.GetMethod.Invoke(obj, null);
        }
    }
}
