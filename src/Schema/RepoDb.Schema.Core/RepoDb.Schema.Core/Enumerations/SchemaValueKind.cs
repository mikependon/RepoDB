#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that defines the kind of a typed value, independent of the database that it is read from.
    /// </summary>
    public enum SchemaValueKind
    {
        /// <summary>
        /// A boolean.
        /// </summary>
        Boolean = 0,

        /// <summary>
        /// A number of any size, held as its invariant text, so no digit is lost.
        /// </summary>
        Number,

        /// <summary>
        /// A text, without its quotes.
        /// </summary>
        Text,

        /// <summary>
        /// A date without a time.
        /// </summary>
        Date,

        /// <summary>
        /// A time of the day.
        /// </summary>
        Time,

        /// <summary>
        /// A date and a time without an offset.
        /// </summary>
        DateTime,

        /// <summary>
        /// A date and a time with an offset from UTC.
        /// </summary>
        DateTimeOffset,

        /// <summary>
        /// A GUID.
        /// </summary>
        Guid,

        /// <summary>
        /// A binary.
        /// </summary>
        Binary
    }
}
