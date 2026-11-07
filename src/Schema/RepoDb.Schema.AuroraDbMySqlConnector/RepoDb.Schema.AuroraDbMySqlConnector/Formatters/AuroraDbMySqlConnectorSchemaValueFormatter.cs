#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values in the format of Aurora MySQL. The kinds of default values are written by its composer.
    /// </summary>
    public sealed class AuroraDbMySqlConnectorSchemaValueFormatter : SchemaValueFormatter
    {
        #region Protected Methods

        /// <summary>
        /// Writes a binary as a hexadecimal literal, i.e.: <c>X'DEAD'</c>.
        /// </summary>
        /// <param name="value">The bytes.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBinary(byte[] value,
            string destinationType) => "X'" + ToHex(value) + "'";

        /// <summary>
        /// Writes a boolean as <c>1</c> or <c>0</c>, the values of a <c>tinyint(1)</c>.
        /// </summary>
        /// <param name="value">The boolean.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBoolean(bool value,
            string destinationType) => value ? "1" : "0";

        /// <summary>
        /// Writes a date as a quoted text in the ISO format.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDate(DateTime value,
            string destinationType) => Quote(ToIsoDate(value));

        /// <summary>
        /// Writes a date and time as a quoted text in the ISO format.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTime(DateTime value,
            string destinationType) => Quote(ToIsoDateTime(value,
            ' ',
            destinationType));

        /// <summary>
        /// Writes a date and time with an offset in UTC, as Aurora MySQL has no type with an offset.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the value was changed.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTimeOffset(DateTimeOffset value,
            string destinationType,
            out string warning)
        {
            warning = $"its offset ({ToOffset(value.Offset)}) was applied and the value is written in UTC, as Aurora MySQL has no type with an offset";
            return Quote(ToIsoDateTime(value.UtcDateTime,
                ' ',
                destinationType));
        }

        /// <summary>
        /// Writes a GUID as a quoted text, or as its bytes if the column is a binary.
        /// </summary>
        /// <param name="value">The GUID.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatGuid(Guid value,
            string destinationType) => (destinationType ?? string.Empty).TrimStart().StartsWith("binary",
            StringComparison.OrdinalIgnoreCase) ? "(UNHEX('" + value.ToString("N") + "'))" : base.FormatGuid(value,
            destinationType);

        /// <summary>
        /// Writes a text between single quotes, where a backslash is doubled too, as it escapes in Aurora MySQL.
        /// </summary>
        /// <param name="value">The text.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatText(string value,
            string destinationType) => Quote(value.Replace("\\",
            "\\\\"));

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
        /// Gets the digits of the fractional seconds of the column: the ones of its type, or none (the type of Aurora MySQL has none unless it says them).
        /// </summary>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The digits.</returns>
        protected override int GetFractionDigits(string destinationType) =>
            Math.Min(ParseDigits(destinationType) ?? 0,
                MaxFractionDigits);

        #endregion
    }
}
