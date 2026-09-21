#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve a <see cref="DateTimeOffset"/> into the <see cref="FbZonedTime"/> that is written into a Firebird <c>TIME WITH TIME ZONE</c> column.
    /// </summary>
    public class DateTimeOffsetToFirebirdZonedTimeResolver : IResolver<DateTimeOffset, object>
    {
        private readonly IResolver<TimeSpan, string> timeZoneNameResolver;

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeOffsetToFirebirdZonedTimeResolver"/> class.
        /// </summary>
        public DateTimeOffsetToFirebirdZonedTimeResolver()
            : this(new TimeSpanToFirebirdTimeZoneNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeOffsetToFirebirdZonedTimeResolver"/> class.
        /// </summary>
        /// <param name="timeZoneNameResolver">The resolver that is used to convert the UTC offset into the name of a Firebird time zone.</param>
        public DateTimeOffsetToFirebirdZonedTimeResolver(IResolver<TimeSpan, string> timeZoneNameResolver)
        {
            this.timeZoneNameResolver = timeZoneNameResolver;
        }

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> into an <see cref="FbZonedTime"/>, holding the UTC time of day and the offset as a fixed-offset <c>Etc/GMT</c> time zone name.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> to convert.</param>
        /// <returns>The <see cref="FbZonedTime"/> (boxed).</returns>
        public virtual object Resolve(DateTimeOffset value)
        {
            var utcTime = value.UtcDateTime.TimeOfDay;
            return new FbZonedTime(utcTime, timeZoneNameResolver.Resolve(value.Offset));
        }
    }
}
