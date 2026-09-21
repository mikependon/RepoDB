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
    /// A class that is being used to resolve a <see cref="DateTimeOffset"/> into the <see cref="FbZonedDateTime"/> that is written into a Firebird <c>TIMESTAMP WITH TIME ZONE</c> column.
    /// </summary>
    public class DateTimeOffsetToFirebirdZonedDateTimeResolver : IResolver<DateTimeOffset, object>
    {
        private readonly IResolver<TimeSpan, string> timeZoneNameResolver;

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeOffsetToFirebirdZonedDateTimeResolver"/> class.
        /// </summary>
        public DateTimeOffsetToFirebirdZonedDateTimeResolver()
            : this(new TimeSpanToFirebirdTimeZoneNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DateTimeOffsetToFirebirdZonedDateTimeResolver"/> class.
        /// </summary>
        /// <param name="timeZoneNameResolver">The resolver that is used to convert the UTC offset into the name of a Firebird time zone.</param>
        public DateTimeOffsetToFirebirdZonedDateTimeResolver(IResolver<TimeSpan, string> timeZoneNameResolver)
        {
            this.timeZoneNameResolver = timeZoneNameResolver;
        }

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> into an <see cref="FbZonedDateTime"/>, holding the UTC date and time and the offset as a fixed-offset <c>Etc/GMT</c> time zone name.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> to convert.</param>
        /// <returns>The <see cref="FbZonedDateTime"/> (boxed).</returns>
        public virtual object Resolve(DateTimeOffset value) =>
            new FbZonedDateTime(value.UtcDateTime, timeZoneNameResolver.Resolve(value.Offset));
    }
}
