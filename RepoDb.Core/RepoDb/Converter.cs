using RepoDb.Enumerations;
using RepoDb.Extensions;
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
        [Obsolete("Use the definition of the ApplicationConfigurationOptions class instead.")]
        public static ConversionType ConversionType { get; set; } = ConversionType.Default;

        /// <summary>
        /// Gets or sets the default equivalent database type (of type <see cref="DbType"/>) of an enumeration if it is being used as a parameter to the 
        /// execution of any non-entity-based operations.
        /// </summary>
        [Obsolete("Use the definition of the ApplicationConfigurationOptions class instead.")]
        public static DbType EnumDefaultDatabaseType { get; set; } = DbType.String;

        #endregion

        #region Methods

        /// <summary>
        /// Converts the value into <see cref="DBNull.Value"/> if it is null.
        /// </summary>
        /// <param name="value">The value to be checked for <see cref="DBNull.Value"/>.</param>
        /// <returns>The converted value.</returns>
        public static object NullToDbNull(object value) =>
            value is null ? DBNull.Value : value;

        /// <summary>
        /// Converts the value into null if the value is equals to <see cref="DBNull.Value"/>.
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
        public static T ToType<T>(object value) =>
            ToType<T>(value, false);

        /// <summary>
        /// Converts a value to a target type if the value is equals to null or <see cref="DBNull.Value"/>.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <param name="forceAutomatic">Force the automatic conversion.</param>
        /// <returns>The converted value.</returns>
        internal static T ToType<T>(object value,
            bool forceAutomatic = false)
        {
            if (value != null && value != DBNull.Value && value is T t)
            {
                return t;
            }
            if (value == null || value == DBNull.Value)
            {
                if (forceAutomatic || GlobalConfiguration.Options.ConversionType == ConversionType.Automatic ||
                    Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return default;
                }
                else if (typeof(T).IsValueType)
                {
                    throw new InvalidCastException($"Failed to convert '{(value == DBNull.Value ? "DBNull" : "Null")}' to '{typeof(T).GetUnderlyingType().FullName}'. " +
                        $"Consider enabling 'GlobalConfiguration.Options.ConversionType' to '{ConversionType.Automatic.ToString()}' or make the type '{typeof(T).FullName}' nullable.");
                }
            }
            try
            {
                value = (typeof(T).Equals(StaticType.Guid) && value is string) ?
                    (T)StringToGuidAsObject(value) : (T)Convert.ChangeType(value, typeof(T));
                if (value == DBNull.Value)
                {
                    throw new Exception("Failed to convert the 'DBNull' value.");
                }
                return (T)value;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"{ex.Message} " +
                    $"Consider enabling 'GlobalConfiguration.Options.ConversionType' to '{ConversionType.Automatic.ToString()}'.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object StringToGuidAsObject(object value)
        {
            if (Guid.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            return null;
        }

        #endregion
    }
}
