#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds a typed value (i.e.: the default value of a column), independent of the database that it is read from,
    /// so it can be written in the format of any other database.
    /// </summary>
    public sealed class SchemaValue
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaValue"/> class.
        /// </summary>
        /// <param name="kind">The kind of the value.</param>
        /// <param name="value">The value, whose type is the one of the kind.</param>
        private SchemaValue(SchemaValueKind kind,
            object value)
        {
            Kind = kind;
            Value = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a binary value.
        /// </summary>
        /// <param name="value">The bytes.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromBinary(byte[] value) =>
            new SchemaValue(SchemaValueKind.Binary,
                value);

        /// <summary>
        /// Creates a boolean value.
        /// </summary>
        /// <param name="value">The boolean.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromBoolean(bool value) =>
            new SchemaValue(SchemaValueKind.Boolean,
                value);

        /// <summary>
        /// Creates a date value.
        /// </summary>
        /// <param name="value">The date (its time is ignored).</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromDate(DateTime value) =>
            new SchemaValue(SchemaValueKind.Date,
                value.Date);

        /// <summary>
        /// Creates a date and time value.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromDateTime(DateTime value) =>
            new SchemaValue(SchemaValueKind.DateTime,
                value);

        /// <summary>
        /// Creates a date and time value with an offset.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromDateTimeOffset(DateTimeOffset value) =>
            new SchemaValue(SchemaValueKind.DateTimeOffset,
                value);

        /// <summary>
        /// Creates a GUID value.
        /// </summary>
        /// <param name="value">The GUID.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromGuid(Guid value) =>
            new SchemaValue(SchemaValueKind.Guid,
                value);

        /// <summary>
        /// Creates a number value.
        /// </summary>
        /// <param name="value">The invariant text of the number, i.e.: <c>-12.50</c>.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromNumber(string value) =>
            new SchemaValue(SchemaValueKind.Number,
                value);

        /// <summary>
        /// Creates a text value.
        /// </summary>
        /// <param name="value">The text, without quotes.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromText(string value) =>
            new SchemaValue(SchemaValueKind.Text,
                value);

        /// <summary>
        /// Creates a time value.
        /// </summary>
        /// <param name="value">The time of the day.</param>
        /// <returns>The value.</returns>
        public static SchemaValue FromTime(TimeSpan value) =>
            new SchemaValue(SchemaValueKind.Time,
                value);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the kind of the value.
        /// </summary>
        public SchemaValueKind Kind { get; }

        /// <summary>
        /// Gets the value, whose type is the one of the kind.
        /// </summary>
        public object Value { get; }

        #endregion
    }
}
