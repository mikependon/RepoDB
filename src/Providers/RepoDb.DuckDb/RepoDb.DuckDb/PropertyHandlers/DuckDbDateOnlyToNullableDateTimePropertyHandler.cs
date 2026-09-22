#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.DuckDb
{
    /// <summary>
    /// A property handler that maps a DuckDB <c>DATE</c> column onto a <see cref="Nullable{T}"/> of <see cref="DateTime"/> property.
    /// </summary>
    /// <remarks>
    /// Use <see cref="DuckDbDateOnlyToDateTimePropertyHandler"/> for a non-nullable <see cref="DateTime"/> property.
    /// </remarks>
    public class DuckDbDateOnlyToNullableDateTimePropertyHandler : IPropertyHandler<object, DateTime?>
    {
        /// <summary>
        /// Converts the <see cref="System.DateOnly"/> value, as returned by the driver, into a <see cref="Nullable{T}"/> of <see cref="DateTime"/> at midnight.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="System.DateOnly"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The equivalent <see cref="DateTime"/>, or <c>null</c> when the value is not a <see cref="System.DateOnly"/>.</returns>
        public DateTime? Get(object input,
            PropertyHandlerGetOptions options) =>
            input is DateOnly dateOnly ? dateOnly.ToDateTime(TimeOnly.MinValue) : null;

        /// <summary>
        /// Converts the <see cref="DateTime"/> into a <see cref="System.DateOnly"/>, to be written into a
        /// <c>DATE</c> column. The time-of-day component is ignored.
        /// </summary>
        /// <param name="input">The <see cref="DateTime"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="System.DateOnly"/> (boxed), or <c>null</c> when the value is <c>null</c>.</returns>
        public object Set(DateTime? input,
            PropertyHandlerSetOptions options) =>
            input.HasValue ? DateOnly.FromDateTime(input.Value) : null;
    }
}
