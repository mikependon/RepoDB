#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of MariaDB.
    /// </summary>
    public sealed class MariaDbSchemaValueFormatter : SchemaValueFormatter
    {
        #region Private Variables

        private static readonly Regex DateTimeType = new Regex(@"^(datetime|timestamp)(\((?<digits>\d)\))?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        private static readonly Regex TimeType = new Regex(@"^time(\((?<digits>\d)\))?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of MariaDB.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if MariaDB has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            var type = (destinationType ?? string.Empty).Trim();
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return FormatCurrent(type,
                        true);
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return FormatCurrent(type,
                        false);
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    warning = "MariaDB has no type with an offset, so the date and time is in UTC";
                    return FormatCurrent(type,
                        false);
                case ColumnDefaultValue.CurrentDate:
                    return "(CURRENT_DATE)";
                case ColumnDefaultValue.CurrentTime:
                    return "(CURRENT_TIME" + GetDigits(TimeType,
                        type) + ")";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return FormatGuid(type);
                case ColumnDefaultValue.RandomNumber:
                    return "(RAND())";
                case ColumnDefaultValue.CurrentUser:
                    return IsText(type) ? "(CURRENT_USER())" : null;
                case ColumnDefaultValue.CurrentDatabase:
                    return IsText(type) ? "(DATABASE())" : null;
                default:
                    return null;
            }
        }

        #endregion

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
        /// Writes a date and time with an offset in UTC, as MariaDB has no type with an offset.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the value was changed.</param>
        /// <returns>The text.</returns>
        protected override string FormatDateTimeOffset(DateTimeOffset value,
            string destinationType,
            out string warning)
        {
            warning = $"its offset ({ToOffset(value.Offset)}) was applied and the value is written in UTC, as MariaDB has no type with an offset";
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
        /// Writes a text between single quotes, where a backslash is doubled too, as it escapes in MariaDB.
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
        /// Gets the digits of the fractional seconds of the column: the ones of its type, or none (the type of MariaDB has none unless it says them).
        /// </summary>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The digits.</returns>
        protected override int GetFractionDigits(string destinationType) =>
            Math.Min(ParseDigits(destinationType) ?? 0,
                MaxFractionDigits);

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes the current date and time of a column: <c>CURRENT_TIMESTAMP</c> in a date and time column (with its fractional seconds), an expression in the others.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <param name="local"><c>true</c> for the date and time of the server; <c>false</c> for the date and time in UTC.</param>
        /// <returns>The text that follows <c>DEFAULT</c>.</returns>
        private static string FormatCurrent(string type,
            bool local)
        {
            if (DateTimeType.IsMatch(type))
            {
                var digits = GetDigits(DateTimeType,
                    type);
                return local ? "CURRENT_TIMESTAMP" + digits : "(UTC_TIMESTAMP" + digits + ")";
            }

            if (TimeType.IsMatch(type))
            {
                var digits = GetDigits(TimeType,
                    type);
                return local ? "(CURRENT_TIME" + digits + ")" : "(UTC_TIME" + digits + ")";
            }

            if (type.Equals("date",
                StringComparison.OrdinalIgnoreCase))
            {
                return local ? "(CURRENT_DATE)" : "(UTC_DATE())";
            }

            return local ? "(NOW())" : "(UTC_TIMESTAMP())";
        }

        /// <summary>
        /// Writes a new GUID: <c>UUID()</c> in a text column, and its bytes in a binary of 16 bytes.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the column can not hold a GUID.</returns>
        private static string FormatGuid(string type)
        {
            if (IsText(type))
            {
                return "(UUID())";
            }

            return type.StartsWith("binary(16)",
                StringComparison.OrdinalIgnoreCase) || type.StartsWith("varbinary(16)",
                StringComparison.OrdinalIgnoreCase) ? "(UNHEX(REPLACE(UUID(), '-', '')))" : null;
        }

        /// <summary>
        /// Gets the digits of the fractional seconds of the type, between parentheses, as <c>CURRENT_TIMESTAMP</c> must have the digits of its column.
        /// </summary>
        /// <param name="pattern">The pattern of the type.</param>
        /// <param name="type">The type of the column.</param>
        /// <returns>The digits between parentheses, i.e.: <c>(6)</c>, or an empty text.</returns>
        private static string GetDigits(Regex pattern,
            string type)
        {
            var match = pattern.Match(type);
            var digits = match.Success ? match.Groups["digits"].Value : string.Empty;
            return digits.Length == 0 || digits == "0" ? string.Empty : "(" + digits + ")";
        }

        /// <summary>
        /// Checks whether the type is a text.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <returns><c>true</c> if the type is a text.</returns>
        private static bool IsText(string type) =>
            type.StartsWith("char",
                StringComparison.OrdinalIgnoreCase) || type.StartsWith("varchar",
                StringComparison.OrdinalIgnoreCase) || type.EndsWith("text",
                StringComparison.OrdinalIgnoreCase);

        #endregion
    }
}
