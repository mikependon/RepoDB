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
using System.Globalization;

namespace RepoDb
{
    /// <summary>
    /// A generalized converter class.
    /// </summary>
    public static class Converter
    {
        #region Private Variables
        
        /// <summary>
        /// The maximum absolute value of a <see cref="decimal"/>, as a <see cref="double"/>. A larger value cannot be converted.
        /// </summary>
        private const double DecimalMaxValueAsDouble = 7.9228162514264337E+28;

        /// <summary>
        /// The maximum absolute value of a <see cref="decimal"/>, as a <see cref="float"/>. A larger value cannot be converted.
        /// </summary>
        private const float DecimalMaxValueAsSingle = 7.9228162514264337E+28f;

        // Not a constant on purpose: a compile-time evaluation would reflect the compiler instead of the runtime.
        private static readonly double FloatingPointToDecimalProbe = 100.05;

        /// <summary>
        /// Gets whether the runtime converts a <see cref="double"/> or <see cref="float"/> to a <see cref="decimal"/> using the exact
        /// binary value (.NET 11 and up, e.g.: 100.05 becomes 100.04999999999999715782905696) instead of rounding the value to its
        /// 15 (<see cref="double"/>) or 7 (<see cref="float"/>) significant digits (.NET 10 and below, e.g.: 100.05).
        /// </summary>
        internal static readonly bool IsFloatingPointToDecimalExact = (decimal)FloatingPointToDecimalProbe != 100.05m;

        #endregion

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

        #region Private Methods

        /// <summary>
        /// Checks whether a null or <see cref="DBNull.Value"/> is converted into the default value of the target type instead of an exception.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="forceAutomatic">Force the automatic conversion.</param>
        /// <returns><c>true</c> if the default value is returned.</returns>
        private static bool CanReturnDefault(Type type,
            bool forceAutomatic) =>
            forceAutomatic ||
            GlobalConfiguration.Options.ConversionType == ConversionType.Automatic ||
            IsNullableOrReferenceType(type);

        /// <summary>
        /// Checks whether the type accepts a null, which are the nullable value types and the reference types (including <see cref="object"/>).
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns><c>true</c> if the type accepts a null.</returns>
        private static bool IsNullableOrReferenceType(Type type) =>
            !type.IsValueType ||
            Nullable.GetUnderlyingType(type) != null;

        private static object ChangeType(object value,
            Type type)
        {
            if (type == typeof(decimal) && IsFloatingPointToDecimalExact)
            {
                if (value is double d) return DoubleToDecimal(d);
                if (value is float f) return SingleToDecimal(f);
            }
            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

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
                if (CanReturnDefault(typeof(T),
                    forceAutomatic))
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
                    (T)StringToGuidAsObject(value) : (T)ChangeType(value, typeof(T));
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
                if (CanReturnDefault(type,
                    forceAutomatic: false))
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
                    StringToGuidAsObject(value) : ChangeType(value, type);
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

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="decimal"/> rounded to 15 significant digits, regardless of the runtime.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        internal static decimal DoubleToDecimal(double value)
        {
            // NaN, infinity and out-of-range values keep throwing the 'OverflowException' of the runtime
            if (!IsFloatingPointToDecimalExact || double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value) >= DecimalMaxValueAsDouble)
            {
                return (decimal)value;
            }
            return decimal.Parse(value.ToString("G15", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a <see cref="float"/> to a <see cref="decimal"/> rounded to 7 significant digits, regardless of the runtime.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        internal static decimal SingleToDecimal(float value)
        {
            // NaN, infinity and out-of-range values keep throwing the 'OverflowException' of the runtime
            if (!IsFloatingPointToDecimalExact || float.IsNaN(value) || float.IsInfinity(value) || Math.Abs(value) >= DecimalMaxValueAsSingle)
            {
                return (decimal)value;
            }
            return decimal.Parse(value.ToString("G7", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object to a <see cref="decimal"/> like <see cref="Convert.ToDecimal(object)"/>, but keeps the rounding of a boxed
        /// <see cref="double"/> or <see cref="float"/> regardless of the runtime.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        internal static decimal ObjectToDecimal(object value)
        {
            return value switch
            {
                double d => DoubleToDecimal(d),
                float f => SingleToDecimal(f),
                _ => Convert.ToDecimal(value)
            };
        }

        /// <summary>
        /// Gets the method that converts the <paramref name="fromType"/> to a <see cref="decimal"/> keeping the pre-.NET 11 rounding,
        /// or null if the conversion does not need it.
        /// </summary>
        internal static System.Reflection.MethodInfo GetFloatingPointToDecimalMethod(Type fromType,
            Type toType)
        {
            if (!IsFloatingPointToDecimalExact || toType != typeof(decimal))
            {
                return null;
            }
            var name = fromType == typeof(double) ? nameof(DoubleToDecimal) :
                fromType == typeof(float) ? nameof(SingleToDecimal) :
                fromType == typeof(object) ? nameof(ObjectToDecimal) : null;
            return name == null ? null :
                typeof(Converter).GetMethod(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        #endregion
    }
}
