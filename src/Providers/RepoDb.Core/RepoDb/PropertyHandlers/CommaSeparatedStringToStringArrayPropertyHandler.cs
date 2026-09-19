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
    /// A property handler that maps a comma-separated string column (for example, the MySQL <c>SET</c> type) into a <see cref="string"/> array property.
    /// </summary>
    public class CommaSeparatedStringToStringArrayPropertyHandler : IPropertyHandler<string, string[]>
    {
        private const char Separator = ',';

        /// <summary>
        /// Splits the comma-separated value, as returned by the database, into a <see cref="string"/> array.
        /// </summary>
        /// <param name="input">The comma-separated value of the column (for example <c>a,b,c</c>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="string"/> array with the members of the value, an empty array when the value is empty, or <c>null</c> when the value is <c>null</c>.</returns>
        public string[] Get(string input,
            PropertyHandlerGetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            return input.Length == 0 ? Array.Empty<string>() : input.Split(Separator);
        }

        /// <summary>
        /// Joins the <see cref="string"/> array into a comma-separated value, to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="string"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The comma-separated value (for example <c>a,b,c</c>), or <c>null</c> when the array is <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when a member of the array contains a comma, as it cannot be represented in the comma-separated value.</exception>
        public string Set(string[] input,
            PropertyHandlerSetOptions options)
        {
            if (input == null)
            {
                return null;
            }
            foreach (var member in input)
            {
                if (member != null && member.IndexOf(Separator) >= 0)
                {
                    throw new ArgumentException($"The member '{member}' contains a comma and cannot be written into a comma-separated value.", nameof(input));
                }
            }
            return string.Join(Separator.ToString(), input);
        }
    }
}
