#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of SQL Server.
    /// </summary>
    public sealed class SqlServerSchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        /// <summary>
        /// Gets the maximum digits of the fractional seconds that SQL Server holds.
        /// </summary>
        protected override int MaxFractionDigits => 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of SQL Server.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if SQL Server has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "(GETDATE())";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "(GETUTCDATE())";
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    return "(SYSDATETIMEOFFSET())";
                case ColumnDefaultValue.CurrentDate:
                    return "(CONVERT(date, GETDATE()))";
                case ColumnDefaultValue.CurrentTime:
                    return "(CONVERT(time, GETDATE()))";
                case ColumnDefaultValue.NewGuid:
                    return "(NEWID())";
                case ColumnDefaultValue.NewSequentialGuid:
                    return "(NEWSEQUENTIALID())";
                case ColumnDefaultValue.RandomNumber:
                    return "(RAND())";
                case ColumnDefaultValue.CurrentUser:
                    return "(SUSER_SNAME())";
                case ColumnDefaultValue.CurrentDatabase:
                    return "(DB_NAME())";
                default:
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Writes a binary as a hexadecimal number, i.e.: <c>0xDEAD</c>.
        /// </summary>
        /// <param name="value">The bytes.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBinary(byte[] value,
            string destinationType) => "0x" + ToHex(value);

        /// <summary>
        /// Writes a boolean as <c>1</c> or <c>0</c>, the values of a <c>bit</c>.
        /// </summary>
        /// <param name="value">The boolean.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBoolean(bool value,
            string destinationType) => value ? "1" : "0";

        /// <summary>
        /// Writes a date as a quoted text in the ISO format, which does not depend on the language of the session.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDate(DateTime value,
            string destinationType) => Quote(ToIsoDate(value));

        /// <summary>
        /// Writes a date and time as a quoted text in the ISO format, with <c>T</c> between the date and the time.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTime(DateTime value,
            string destinationType) => Quote(ToIsoDateTime(value,
            'T',
            destinationType));

        /// <summary>
        /// Writes a date and time with an offset as a quoted text in the ISO format.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the value was changed, or <c>null</c>.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTimeOffset(DateTimeOffset value,
            string destinationType,
            out string warning)
        {
            warning = null;
            return Quote(ToIsoDateTime(value.DateTime,
                'T',
                destinationType) + ToOffset(value.Offset));
        }

        /// <summary>
        /// Writes a text between single quotes, with the prefix <c>N</c> if it has characters that are not ASCII.
        /// </summary>
        /// <param name="value">The text.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatText(string value,
            string destinationType) => (value.Any(character => character > 127) ? "N" : string.Empty) + Quote(value);

        /// <summary>
        /// Writes a time as a quoted text.
        /// </summary>
        /// <param name="value">The time of the day.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatTime(TimeSpan value,
            string destinationType) => Quote(ToIsoTime(value,
            destinationType));

        /// <summary>
        /// Gets the digits of the fractional seconds of the column: 3 in a <c>datetime</c>, none in a <c>smalldatetime</c>, and the ones of the type in the others.
        /// </summary>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The digits.</returns>
        protected override int GetFractionDigits(string destinationType)
        {
            var type = (destinationType ?? string.Empty).Trim().ToLowerInvariant();
            if (type.StartsWith("smalldatetime", StringComparison.Ordinal))
            {
                return 0;
            }

            return type.StartsWith("datetime", StringComparison.Ordinal) && !type.StartsWith("datetime2", StringComparison.Ordinal) && !type.StartsWith("datetimeoffset", StringComparison.Ordinal) ? 3 : base.GetFractionDigits(destinationType);
        }

        #endregion
    }
}
