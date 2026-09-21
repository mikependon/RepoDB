#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A property handler that maps the Firebird <c>TIMESTAMP WITH TIME ZONE</c> type into a <see cref="DateTimeOffset"/> property. The UTC offset is kept, but the name of the time zone (for example <c>America/New_York</c>) is not, as a <see cref="DateTimeOffset"/> cannot hold it; use an <c>FbZonedDateTime</c> property when the time zone name matters.
    /// </summary>
    /// <remarks>
    /// Use <see cref="FirebirdZonedDateTimeToNullableDateTimeOffsetPropertyHandler"/> for a <see cref="Nullable{T}"/> of <see cref="DateTimeOffset"/> property.
    /// </remarks>
    public class FirebirdZonedDateTimeToDateTimeOffsetPropertyHandler : IPropertyHandler<object, DateTimeOffset>
    {
        private static readonly FirebirdZonedValueToDateTimeOffsetResolver zonedValueToDateTimeOffsetResolver = new FirebirdZonedValueToDateTimeOffsetResolver();
        private static readonly DateTimeOffsetToFirebirdZonedDateTimeResolver dateTimeOffsetToZonedDateTimeResolver = new DateTimeOffsetToFirebirdZonedDateTimeResolver();

        /// <summary>
        /// Converts the <c>TIMESTAMP WITH TIME ZONE</c> value, as returned by the driver, into a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="DateTimeOffset"/> or an <c>FbZonedDateTime</c>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="DateTimeOffset"/>, or its default value when the value is <c>null</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the UTC offset of the time zone cannot be determined.</exception>
        public DateTimeOffset Get(object input,
            PropertyHandlerGetOptions options) =>
            zonedValueToDateTimeOffsetResolver.Resolve(input) ?? default;

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> into an <c>FbZonedDateTime</c> (holding the UTC date and time, with the offset as a fixed-offset <c>Etc/GMT</c> time zone name, for example <c>Etc/GMT-3</c>), to be written into a <c>TIMESTAMP WITH TIME ZONE</c> column.
        /// </summary>
        /// <param name="input">The <see cref="DateTimeOffset"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <c>FbZonedDateTime</c> (boxed).</returns>
        public object Set(DateTimeOffset input,
            PropertyHandlerSetOptions options) =>
            dateTimeOffsetToZonedDateTimeResolver.Resolve(input);
    }
}
