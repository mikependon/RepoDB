using RepoDb.Enumerations;
using System;
using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A generalized converter class.
    /// </summary>
    public static class Converter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conversion type when converting the instance of <see cref="DbDataReader"/> object into its destination .NET CLR Types.
        /// The default value is <see cref="ConversionType.Default"/>.
        /// </summary>
        public static ConversionType ConversionType { get; set; } = ConversionType.Default;

        /// <summary>
        /// Gets or sets the default equivalent database type (of type <see cref="DbType"/>) of an enumeration if it is being used as a parameter to the 
        /// execution of any non-entity-based operations.
        /// </summary>
        public static DbType EnumDefaultDatabaseType { get; set; } = DbType.String;

        #endregion

        #region Methods

        /// <summary>
        /// Converts a value to null if the value is equals to <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
        /// <returns>The converted value.</returns>
        public static object DbNullToNull(object value) =>
            ReferenceEquals(DBNull.Value, value) ? null : value;

        /// <summary>
        /// Converts a value to a target type if the value is equals to null or <see cref="DBNull.Value"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        public static T ToType<T>(object value)
        {
            if (value is T t)
            {
                return t;
            }
            return value == null || DbNullToNull(value) == null ? default(T) :
                (T)Convert.ChangeType(value, typeof(T));
        }

        #endregion
    }
}
