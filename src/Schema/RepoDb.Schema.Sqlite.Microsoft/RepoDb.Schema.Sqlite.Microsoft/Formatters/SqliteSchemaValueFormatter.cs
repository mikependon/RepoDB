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
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of SQLite.
    /// </summary>
    public sealed class SqliteSchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of SQLite.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if SQLite has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "(datetime('now','localtime'))";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "CURRENT_TIMESTAMP";
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    warning = "SQLite has no type with an offset, so the date and time is in UTC";
                    return "CURRENT_TIMESTAMP";
                case ColumnDefaultValue.CurrentDate:
                    return "(date('now','localtime'))";
                case ColumnDefaultValue.CurrentTime:
                    return "(time('now','localtime'))";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return "(lower(hex(randomblob(16))))";
                case ColumnDefaultValue.RandomNumber:
                    return "(random())";
                default:
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Writes a binary as a hexadecimal literal, i.e.: <c>X'DEAD'</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBinary(byte[] value,
            string destinationType) => "X'" + ToHex(value) + "'";

        /// <summary>
        /// Writes a boolean as <c>1</c> or <c>0</c>, the values of an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBoolean(bool value,
            string destinationType) => value ? "1" : "0";

        /// <summary>
        /// Writes a date as a quoted text in the ISO format, as the database has no typed literals.
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

        /// <summary>
        /// Writes a date and time with an offset in UTC, as the database has no type with an offset.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the value was changed.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTimeOffset(DateTimeOffset value,
            string destinationType,
            out string warning)
        {
            warning = $"its offset ({ToOffset(value.Offset)}) was applied and the value is written in UTC, as the database has no type with an offset";
            return Quote(ToIsoDateTime(value.UtcDateTime,
                ' ',
                destinationType));
        }

        /// <summary>
        /// Writes a time as a quoted text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatTime(TimeSpan value,
            string destinationType) => Quote(ToIsoTime(value,
            destinationType));

        #endregion
    }
}
