#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System.Collections.Generic;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a database array (for example a PostgreSQL <c>T[]</c> column), which the driver returns as an array, into a <see cref="List{T}"/> property.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    public class ArrayToListPropertyHandler<T> : IPropertyHandler<T[], List<T>>
    {
        /// <summary>
        /// Converts the array, as returned by the database, into a <see cref="List{T}"/>.
        /// </summary>
        /// <param name="input">The array value of the column, as returned by the driver.</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>A <see cref="List{T}"/> with the elements of the array, or <c>null</c> when the value is <c>null</c>.</returns>
        public List<T> Get(T[] input,
            PropertyHandlerGetOptions options) =>
            input == null ? null : new List<T>(input);

        /// <summary>
        /// Converts the <see cref="List{T}"/> into an array, to be written into a database array column.
        /// </summary>
        /// <param name="input">The <see cref="List{T}"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>An array with the elements of the list, or <c>null</c> when the list is <c>null</c>.</returns>
        public T[] Set(List<T> input,
            PropertyHandlerSetOptions options) =>
            input?.ToArray();
    }
}
