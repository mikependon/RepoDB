#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Globalization;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve a UTC offset into the name of a Firebird time zone that has the same offset.
    /// </summary>
    public class TimeSpanToFirebirdTimeZoneNameResolver : IResolver<TimeSpan, string>
    {
        /// <summary>
        /// Gets the name of a Firebird time zone that has the UTC offset. The driver only accepts the names of the time zones known to Firebird when writing (not offsets like <c>+03:00</c>), so the fixed-offset <c>Etc/GMT</c> zones are used for whole-hour offsets (for example <c>Etc/GMT-3</c> for <c>+03:00</c>). The instant is always kept, but an offset that is not a whole hour between -12:00 and +14:00 is written as <c>UTC</c>.
        /// </summary>
        /// <param name="offset">The UTC offset.</param>
        /// <returns>The time zone name.</returns>
        public virtual string Resolve(TimeSpan offset)
        {
            if (offset == TimeSpan.Zero || offset.Ticks % TimeSpan.TicksPerHour != 0 || offset.TotalHours < -12 || offset.TotalHours > 14)
            {
                return "UTC";
            }
            var hours = (int)offset.TotalHours;
            return "Etc/GMT" + (hours > 0 ? "-" : "+") + Math.Abs(hours).ToString(CultureInfo.InvariantCulture);
        }
    }
}
