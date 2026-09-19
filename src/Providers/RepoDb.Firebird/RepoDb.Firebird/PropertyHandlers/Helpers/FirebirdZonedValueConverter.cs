#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using System;
using System.Globalization;

namespace RepoDb.PropertyHandlers.Firebird
{
    /// <summary>
    /// A helper that converts the values of the Firebird <c>TIMESTAMP WITH TIME ZONE</c> and <c>TIME WITH TIME ZONE</c> columns, as returned by the driver, into a <see cref="DateTimeOffset"/>, and back.
    /// </summary>
    /// <remarks>
    /// Firebird transfers these values in UTC together with the time zone, which the driver exposes as an <see cref="FbZonedDateTime"/> (whose <see cref="FbZonedDateTime.DateTime"/> is in UTC) and an <see cref="FbZonedTime"/> (whose <see cref="FbZonedTime.Time"/> is treated as a UTC time of day). The local value is derived from the UTC value and the offset of the time zone.
    /// </remarks>
    internal static class FirebirdZonedValueConverter
    {
        /// <summary>
        /// The date used for the date part when a <c>TIME WITH TIME ZONE</c> value, which has none, is represented as a <see cref="DateTimeOffset"/>.
        /// </summary>
        internal static readonly DateTime TimeBaseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        /// <summary>
        /// Converts the value returned by the driver (a <see cref="DateTimeOffset"/>, an <see cref="FbZonedDateTime"/> or an <see cref="FbZonedTime"/>) into a <see cref="DateTimeOffset"/> holding the local value and the UTC offset of the time zone. The local time of an <see cref="FbZonedTime"/> is placed on 1970-01-01.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="DateTimeOffset"/>, or <c>null</c> when the value is <c>null</c> or <see cref="DBNull"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the UTC offset of the time zone cannot be determined.</exception>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        internal static DateTimeOffset? ToDateTimeOffset(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case DateTimeOffset offset:
                    return offset;
                case FbZonedDateTime zonedDateTime:
                    var utc = new DateTimeOffset(DateTime.SpecifyKind(zonedDateTime.DateTime, DateTimeKind.Utc));
                    return utc.ToOffset(ResolveOffset(zonedDateTime.Offset, zonedDateTime.TimeZone, utc));
                case FbZonedTime zonedTime:
                    var utcTime = new DateTimeOffset(DateTime.SpecifyKind(TimeBaseDate.Add(zonedTime.Time), DateTimeKind.Utc));
                    var timeOffset = ResolveOffset(zonedTime.Offset, zonedTime.TimeZone, utcTime);
                    return new DateTimeOffset(TimeBaseDate.Add(utcTime.ToOffset(timeOffset).TimeOfDay), timeOffset);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported time zone value.", nameof(value));
            }
        }

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> into an <see cref="FbZonedTime"/>, holding the UTC time of day and the offset as the time zone name.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> to convert.</param>
        /// <returns>The <see cref="FbZonedTime"/> (boxed).</returns>
        internal static object ToZonedTime(DateTimeOffset value)
        {
            var utcTime = value.UtcDateTime.TimeOfDay;
            return new FbZonedTime(utcTime, FormatOffset(value.Offset));
        }

        /// <summary>
        /// Formats a UTC offset as a Firebird time zone name (for example <c>+03:00</c> or <c>-04:30</c>).
        /// </summary>
        /// <param name="offset">The UTC offset.</param>
        /// <returns>The time zone name.</returns>
        internal static string FormatOffset(TimeSpan offset) =>
            (offset < TimeSpan.Zero ? "-" : "+") +
                Math.Abs(offset.Hours).ToString("00", CultureInfo.InvariantCulture) + ":" +
                Math.Abs(offset.Minutes).ToString("00", CultureInfo.InvariantCulture);

        private static TimeSpan ResolveOffset(TimeSpan? offset,
            string timeZone,
            DateTimeOffset utc)
        {
            if (offset.HasValue)
            {
                return offset.Value;
            }
            if (string.Equals(timeZone, "UTC", StringComparison.OrdinalIgnoreCase))
            {
                return TimeSpan.Zero;
            }
            if (timeZone != null &&
                timeZone.Length == 6 &&
                (timeZone[0] == '+' || timeZone[0] == '-') &&
                timeZone[3] == ':' &&
                int.TryParse(timeZone.Substring(1, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var hours) &&
                int.TryParse(timeZone.Substring(4, 2), NumberStyles.None, CultureInfo.InvariantCulture, out var minutes))
            {
                var span = new TimeSpan(hours, minutes, 0);
                return timeZone[0] == '-' ? -span : span;
            }
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZone).GetUtcOffset(utc);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException || ex is ArgumentException)
            {
                throw new InvalidOperationException($"The UTC offset of the time zone '{timeZone}' cannot be determined.", ex);
            }
        }
    }
}
