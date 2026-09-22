#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Native;
using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.DuckDb
{
    /// <summary>
    /// A property handler that maps a DuckDB <c>TIME</c> column onto a <see cref="Nullable{T}"/> of <see cref="TimeSpan"/> property.
    /// </summary>
    /// <remarks>
    /// Use <see cref="DuckDbTimeOnlyToTimeSpanPropertyHandler"/> for a non-nullable <see cref="TimeSpan"/> property.
    /// </remarks>
    public class DuckDbTimeOnlyToNullableTimeSpanPropertyHandler : IPropertyHandler<object, TimeSpan?>
    {
        /// <summary>
        /// Converts the <see cref="System.TimeOnly"/> value, as returned by the driver, into a <see cref="Nullable{T}"/> of <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="System.TimeOnly"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The equivalent <see cref="TimeSpan"/>, or <c>null</c> when the value is not a <see cref="System.TimeOnly"/>.</returns>
        public TimeSpan? Get(object input,
            PropertyHandlerGetOptions options) =>
            input is TimeOnly timeOnly ? timeOnly.ToTimeSpan() : null;

        /// <summary>
        /// Converts the <see cref="TimeSpan"/> into a <see cref="DuckDBTimeOnly"/>, to be written into a
        /// <c>TIME</c> column. Any component beyond microsecond precision (DuckDB's native <c>TIME</c>
        /// resolution) is truncated.
        /// </summary>
        /// <param name="input">The <see cref="TimeSpan"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="DuckDBTimeOnly"/> (boxed), or <c>null</c> when the value is <c>null</c>.</returns>
        public object Set(TimeSpan? input,
            PropertyHandlerSetOptions options) =>
            input.HasValue
                ? new DuckDBTimeOnly(
                    (byte)input.Value.Hours,
                    (byte)input.Value.Minutes,
                    (byte)input.Value.Seconds,
                    (int)(input.Value.Ticks % TimeSpan.TicksPerSecond / (TimeSpan.TicksPerMillisecond / 1000)))
                : null;
    }
}
