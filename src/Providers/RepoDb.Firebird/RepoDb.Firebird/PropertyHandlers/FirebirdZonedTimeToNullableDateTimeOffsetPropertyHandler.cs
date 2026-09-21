#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A property handler that maps the Firebird <c>TIME WITH TIME ZONE</c> type into a <see cref="Nullable{T}"/> of <see cref="DateTimeOffset"/> property, as no single BCL type represents a time of day with an offset. Only the time of day and the UTC offset are meaningful: the date part is always 1970-01-01, and the name of the time zone is not kept.
    /// </summary>
    public class FirebirdZonedTimeToNullableDateTimeOffsetPropertyHandler : IPropertyHandler<object, DateTimeOffset?>
    {
        /// <summary>
        /// Converts the <c>TIME WITH TIME ZONE</c> value, as returned by the driver, into a <see cref="Nullable{T}"/> of <see cref="DateTimeOffset"/> on 1970-01-01.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="DateTimeOffset"/> or an <c>FbZonedTime</c>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="DateTimeOffset"/>, or <c>null</c> when the value is <c>null</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the UTC offset of the time zone cannot be determined.</exception>
        public DateTimeOffset? Get(object input,
            PropertyHandlerGetOptions options) =>
            FirebirdZonedValueConverter.ToDateTimeOffset(input);

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> into an <c>FbZonedTime</c> (holding the UTC time of day, with the offset as a fixed-offset <c>Etc/GMT</c> time zone name, for example <c>Etc/GMT-3</c>), to be written into a <c>TIME WITH TIME ZONE</c> column. The date part is ignored.
        /// </summary>
        /// <param name="input">The <see cref="DateTimeOffset"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <c>FbZonedTime</c> (boxed), or <c>null</c> when the value is <c>null</c>.</returns>
        public object Set(DateTimeOffset? input,
            PropertyHandlerSetOptions options) =>
            input.HasValue ? FirebirdZonedValueConverter.ToZonedTime(input.Value) : null;
    }
}
