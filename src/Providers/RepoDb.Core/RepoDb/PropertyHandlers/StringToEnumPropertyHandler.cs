#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a string-based database column (for example, the MySQL <c>ENUM</c> type, or the MySQL <c>SET</c> type when used with a <see cref="FlagsAttribute"/> enum) into a <typeparamref name="TEnum"/> property.
    /// </summary>
    /// <typeparam name="TEnum">The CLR enum type of the property.</typeparam>
    public class StringToEnumPropertyHandler<TEnum> : IPropertyHandler<string, TEnum>
        where TEnum : struct, Enum
    {
        private static readonly bool IsFlags = typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);

        /// <summary>
        /// Converts the string value, as returned by the database, into a <typeparamref name="TEnum"/> by member name (case-insensitive). A comma-separated list of names is combined when <typeparamref name="TEnum"/> is a <see cref="FlagsAttribute"/> enum.
        /// </summary>
        /// <param name="input">The string value of the column.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The parsed <typeparamref name="TEnum"/>, or its default value when the value is <c>null</c> or empty.</returns>
        public TEnum Get(string input,
            PropertyHandlerGetOptions options) =>
            string.IsNullOrWhiteSpace(input) ? default : (TEnum)Enum.Parse(typeof(TEnum), input, true);

        /// <summary>
        /// Converts the <typeparamref name="TEnum"/> into the member name written into the column. The names of a <see cref="FlagsAttribute"/> enum are joined by a comma without spaces (for example <c>Read,Write</c>), which is the format of the MySQL <c>SET</c> type.
        /// </summary>
        /// <param name="input">The <typeparamref name="TEnum"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The member name (or comma-separated member names) of the value.</returns>
        public string Set(TEnum input,
            PropertyHandlerSetOptions options)
        {
            var text = input.ToString();
            return IsFlags ? text.Replace(", ", ",") : text;
        }
    }
}
