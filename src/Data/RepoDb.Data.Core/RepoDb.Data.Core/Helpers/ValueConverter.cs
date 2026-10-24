#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Globalization;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Converts the values that were read from a file (JSON, XML, CSV) into the types of the columns of a table.
    /// </summary>
    internal static class ValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts a value that was read from a file into the type of a column of the table.
        /// </summary>
        /// <param name="value">The value that was read from the file.</param>
        /// <param name="type">The type of the column, or <c>null</c> if it is not known (the value is kept as it is).</param>
        /// <returns>The value as the type, or <c>null</c> if the value is <c>null</c>.</returns>
        /// <exception cref="FormatException">The value cannot be converted into the type.</exception>
        public static object Convert(object value,
            Type type)
        {
            if (value == null || type == null)
            {
                return value;
            }
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(object) || type.IsInstanceOfType(value))
            {
                return value;
            }
            if (IsIntegral(type) && HasFraction(value))
            {
                throw new FormatException($"The value '{value}' has a fraction, which '{type.Name}' cannot hold.");
            }
            try
            {
                return ConvertCore(value, type);
            }
            catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException || e is ArgumentException)
            {
                throw new FormatException($"The value '{value}' cannot be converted into '{type.Name}'.", e);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts a value that is not null, and is not of the type already.
        /// </summary>
        /// <param name="value">The value that was read from the file.</param>
        /// <param name="type">The type of the column.</param>
        /// <returns>The value as the type.</returns>
        private static object ConvertCore(object value,
            Type type)
        {
            if (type == typeof(string))
            {
                return value is bool flag ? (flag ? "true" : "false") : System.Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            if (type == typeof(Guid))
            {
                return Guid.Parse(System.Convert.ToString(value, CultureInfo.InvariantCulture));
            }
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(System.Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            if (type == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(System.Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(System.Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            }
            if (type == typeof(byte[]))
            {
                return System.Convert.FromBase64String(System.Convert.ToString(value, CultureInfo.InvariantCulture));
            }
            if (type == typeof(bool))
            {
                return value is string text ? bool.Parse(text) : System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }
            if (type == typeof(char))
            {
                return System.Convert.ToChar(value, CultureInfo.InvariantCulture);
            }
            if (type.IsEnum)
            {
                return value is string name ? Enum.Parse(type, name, true) : Enum.ToObject(type, ConvertCore(value, Enum.GetUnderlyingType(type)));
            }
            return System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks whether a number has a fraction.
        /// </summary>
        /// <param name="value">The value that was read from the file.</param>
        /// <returns><c>true</c> if the value is a number that has a fraction; otherwise, <c>false</c>.</returns>
        private static bool HasFraction(object value) =>
            (value is decimal number && decimal.Truncate(number) != number) ||
            (value is double real && Math.Truncate(real) != real);

        /// <summary>
        /// Checks whether the type is an integral number type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type is an integral number type; otherwise, <c>false</c>.</returns>
        private static bool IsIntegral(Type type) =>
            type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) ||
            type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);

        #endregion
    }
}
