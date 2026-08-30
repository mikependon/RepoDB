#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

fusing RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that re-bases the date component of a value read
    /// back from a Vertica <c>TIME</c> column onto <see cref="DateTime"/>'s default (0001-01-01) date.
    /// </summary>
    /// <remarks>
    /// Vertica's ADO.NET provider (<c>VerticaDataReader.GetDateTime()</c>) materializes a <c>TIME</c>
    /// column's value as a full <see cref="DateTime"/> combined with the current date at the time of the
    /// read, not a fixed placeholder date - so a value written and then queried back would otherwise never
    /// compare equal to what was written. Vertica has no accessor that returns a bare <see cref="TimeSpan"/>
    /// for a <c>TIME</c> column (<c>GetInterval()</c> exists, but is for its distinct <c>INTERVAL</c> type).
    /// <para>
    /// Use this handler if needed as it is intentionally NOT registered automatically for every
    /// <c>TIME</c>-mapped property, since <see cref="PropertyHandlerMapper"/> registrations keyed by CLR
    /// type are global across the whole process - auto-registering it for every <see cref="DateTime"/>
    /// property would also incorrectly re-base <c>DATE</c>/<c>TIMESTAMP</c> columns. Register it
    /// explicitly, scoped to the specific entity property that maps to a Vertica <c>TIME</c> column:
    /// <code>
    /// PropertyHandlerMapper.Add&lt;CompleteTable, VerticaTimeToDateTimePropertyHandler&gt;(
    ///     e => e.ColumnTime, new VerticaTimeToDateTimePropertyHandler(), true);
    /// </code>
    /// </para>
    /// </remarks>
    public class VerticaTimeToDateTimePropertyHandler : IPropertyHandler<DateTime, DateTime>
    {
        /// <summary>
        /// Strips the date component from the <see cref="DateTime"/> value read back from a <c>TIME</c>
        /// column, keeping only its time-of-day re-based onto 0001-01-01.
        /// </summary>
        public DateTime Get(DateTime input,
            PropertyHandlerGetOptions options) =>
            default(DateTime).Add(input.TimeOfDay);

        /// <summary>
        /// Passes the value through unchanged - Vertica only stores the time-of-day portion of a bound
        /// value in a <c>TIME</c> column regardless of its date component.
        /// </summary>
        public DateTime Set(DateTime input,
            PropertyHandlerSetOptions options) =>
            input;
    }
}
