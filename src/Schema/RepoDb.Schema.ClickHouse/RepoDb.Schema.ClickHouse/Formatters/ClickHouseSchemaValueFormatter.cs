#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of ClickHouse.
    /// </summary>
    public sealed class ClickHouseSchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of ClickHouse.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if ClickHouse has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "now()";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "toTimeZone(now(), 'UTC')";
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    warning = "ClickHouse has no type with an offset, so the date and time is in the time zone of the server";
                    return "now()";
                case ColumnDefaultValue.CurrentDate:
                    return "today()";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return "generateUUIDv4()";
                case ColumnDefaultValue.RandomNumber:
                    return "rand()";
                case ColumnDefaultValue.CurrentUser:
                    return "currentUser()";
                case ColumnDefaultValue.CurrentDatabase:
                    return "currentDatabase()";
                default:
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Writes a date as a quoted text in the ISO format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDate(DateTime value,
            string destinationType) => Quote(ToIsoDate(value));

        /// <summary>
        /// Writes a date and time as a quoted text in the ISO format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTime(DateTime value,
            string destinationType) => Quote(ToIsoDateTime(value,
            ' ',
            destinationType));

        #endregion
    }
}
