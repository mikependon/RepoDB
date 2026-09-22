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
    /// A property handler that maps a DuckDB <c>TIME</c> column onto a <see cref="TimeSpan"/> property.
    /// DuckDB.NET returns a <see cref="System.TimeOnly"/> when reading a <c>TIME</c> value, and requires a
    /// boxed <see cref="DuckDBTimeOnly"/> (not a plain <see cref="TimeSpan"/>) when binding one back as a
    /// parameter, so neither direction has a direct <see cref="TimeSpan"/> equivalent without this handler.
    /// </summary>
    public class DuckDbTimeOnlyToTimeSpanPropertyHandler : IPropertyHandler<object, TimeSpan>
    {
        /// <summary>
        /// Converts the <see cref="System.TimeOnly"/> value, as returned by the driver, into a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="System.TimeOnly"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The equivalent <see cref="TimeSpan"/>, or its default value when the value is not a <see cref="System.TimeOnly"/>.</returns>
        public TimeSpan Get(object input,
            PropertyHandlerGetOptions options) =>
            input is TimeOnly timeOnly ? timeOnly.ToTimeSpan() : default;

        /// <summary>
        /// Converts the <see cref="TimeSpan"/> into a <see cref="DuckDBTimeOnly"/>, to be written into a
        /// <c>TIME</c> column. Any component beyond microsecond precision (DuckDB's native <c>TIME</c>
        /// resolution) is truncated.
        /// </summary>
        /// <param name="input">The <see cref="TimeSpan"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="DuckDBTimeOnly"/> (boxed).</returns>
        public object Set(TimeSpan input,
            PropertyHandlerSetOptions options) =>
            new DuckDBTimeOnly(
                (byte)input.Hours,
                (byte)input.Minutes,
                (byte)input.Seconds,
                (int)(input.Ticks % TimeSpan.TicksPerSecond / (TimeSpan.TicksPerMillisecond / 1000)));
    }
}
