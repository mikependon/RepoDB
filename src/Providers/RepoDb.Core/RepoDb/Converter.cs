#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
        public static object NullToDbNull(object value)
        {
            return value is null ? DBNull.Value : value;
        }

        /// <summary>
        /// Converts the value into null if the value is equals to <see cref="DBNull.Value"/>.
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
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        public static T ToType<T>(object value)
        {
            return ToType<T>(value, forceAutomatic: false);
        }

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
                    (T)StringToGuidAsObject(value) : (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
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
        /// <summary>
        /// The non-generic equivalent of the <see cref="ToType{T}(object, bool)"/> method, which does not require the generic
        /// method to be instantiated at runtime (not possible when publishing with NativeAOT). The default value of the target
        /// type is returned as null.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The target type.</param>
        /// <returns>The converted value, or null for the default value of the target type.</returns>
        internal static object ToType(object value,
            Type type)
        {
            if (value != null && value != DBNull.Value && type.IsInstanceOfType(value))
            {
                return value;
            }
            if (value == null || value == DBNull.Value)
            {
                if (GlobalConfiguration.Options.ConversionType == ConversionType.Automatic ||
                    Nullable.GetUnderlyingType(type) != null)
                {
                    return null;
                }
                else if (type.IsValueType)
                {
                    throw new InvalidCastException($"Failed to convert '{(value == DBNull.Value ? "DBNull" : "Null")}' to '{type.GetUnderlyingType().FullName}'. " +
                        $"Consider enabling 'GlobalConfiguration.Options.ConversionType' to '{ConversionType.Automatic.ToString()}' or make the type '{type.FullName}' nullable.");
                }
            }
            try
            {
                value = (type.Equals(StaticType.Guid) && value is string) ?
                    StringToGuidAsObject(value) : Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
                if (value == DBNull.Value || (value == null && type.IsValueType))
                {
                    throw new Exception("Failed to convert the 'DBNull' value.");
                }
                return value;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"{ex.Message} " +
                    $"Consider enabling 'GlobalConfiguration.Options.ConversionType' to '{ConversionType.Automatic.ToString()}'.");
            }
        }

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
