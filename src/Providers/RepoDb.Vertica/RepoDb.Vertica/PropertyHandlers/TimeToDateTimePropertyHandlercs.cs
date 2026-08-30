#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers.Vertica
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that re-bases the date component of a value read
    /// back from a Vertica <c>TIME</c> column onto <see cref="DateTime"/>'s default (0001-01-01) date.
    /// </summary>
    public class TimeToDateTimePropertyHandler : IPropertyHandler<DateTime, DateTime>
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
