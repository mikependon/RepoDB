#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that writes typed values in the ANSI SQL format. A formatter of a database inherits it and changes only what the database writes differently.
    /// </summary>
    public class SchemaValueFormatter : ISchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too. It is <c>false</c> by default.
        /// </summary>
        public virtual bool HandlesKinds => false;

        /// <summary>
        /// Gets the maximum digits of the fractional seconds that the database holds.
        /// </summary>
        protected virtual int MaxFractionDigits => 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a typed value in the format of the database.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column in the database, as the composer writes it.</param>
        /// <param name="warning">The reason why the value was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the database can not hold the value.</returns>
        public string Format(SchemaValue value,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (value.Kind)
            {
                case SchemaValueKind.Boolean:
                    return FormatBoolean((bool)value.Value,
                        destinationType);
                case SchemaValueKind.Number:
                    return FormatNumber((string)value.Value,
                        destinationType);
                case SchemaValueKind.Text:
                    return FormatText((string)value.Value,
                        destinationType);
                case SchemaValueKind.Date:
                    return FormatDate((DateTime)value.Value,
                        destinationType);
                case SchemaValueKind.Time:
                    return FormatTime((TimeSpan)value.Value,
                        destinationType);
                case SchemaValueKind.DateTime:
                    return FormatDateTime((DateTime)value.Value,
                        destinationType);
                case SchemaValueKind.DateTimeOffset:
                    return FormatDateTimeOffset((DateTimeOffset)value.Value,
                        destinationType,
                        out warning);
                case SchemaValueKind.Guid:
                    return FormatGuid((Guid)value.Value,
                        destinationType);
                default:
                    return FormatBinary((byte[])value.Value,
                        destinationType);
            }
        }

        /// <summary>
        /// Writes a kind of default value in the format of the database. The ANSI kinds are the current date and time.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column in the database, as the composer writes it.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the database has no equivalent.</returns>
        public virtual string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "CURRENT_TIMESTAMP";
                case ColumnDefaultValue.CurrentDate:
                    return "CURRENT_DATE";
                case ColumnDefaultValue.CurrentTime:
                    return "CURRENT_TIME";
                default:
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Writes a binary. The ANSI format has none, so the database that holds binary defaults writes it.
        /// </summary>
        /// <param name="value">The bytes.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text, or <c>null</c> if the database can not hold it.</returns>
        protected virtual string FormatBinary(byte[] value,
            string destinationType) => null;

        /// <summary>
        /// Writes a boolean as <c>TRUE</c> or <c>FALSE</c>.
        /// </summary>
        /// <param name="value">The boolean.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatBoolean(bool value,
            string destinationType) => value ? "TRUE" : "FALSE";

        /// <summary>
        /// Writes a date as a typed literal, i.e.: <c>DATE '2020-01-31'</c>.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatDate(DateTime value,
            string destinationType) => "DATE " + Quote(ToIsoDate(value));

        /// <summary>
        /// Writes a date and time as a typed literal, i.e.: <c>TIMESTAMP '2020-01-31 10:20:30'</c>.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatDateTime(DateTime value,
            string destinationType) => "TIMESTAMP " + Quote(ToIsoDateTime(value,
            ' ',
            destinationType));

        /// <summary>
        /// Writes a date and time with an offset as a typed literal, i.e.: <c>TIMESTAMP WITH TIME ZONE '2020-01-31 10:20:30+01:00'</c>.
        /// </summary>
        /// <param name="value">The date and time with its offset.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the value was changed, or <c>null</c>.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatDateTimeOffset(DateTimeOffset value,
            string destinationType,
            out string warning)
        {
            warning = null;
            return "TIMESTAMP WITH TIME ZONE " + Quote(ToIsoDateTime(value.DateTime,
                ' ',
                destinationType) + ToOffset(value.Offset));
        }

        /// <summary>
        /// Writes a GUID as a quoted text.
        /// </summary>
        /// <param name="value">The GUID.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatGuid(Guid value,
            string destinationType) => Quote(value.ToString("D",
            CultureInfo.InvariantCulture));

        /// <summary>
        /// Writes a number as its invariant text.
        /// </summary>
        /// <param name="value">The invariant text of the number.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatNumber(string value,
            string destinationType) => value;

        /// <summary>
        /// Writes a text between single quotes.
        /// </summary>
        /// <param name="value">The text.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatText(string value,
            string destinationType) => Quote(value);

        /// <summary>
        /// Writes a time as a typed literal, i.e.: <c>TIME '10:20:30'</c>.
        /// </summary>
        /// <param name="value">The time of the day.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected virtual string FormatTime(TimeSpan value,
            string destinationType) => "TIME " + Quote(ToIsoTime(value,
            destinationType));

        /// <summary>
        /// Gets the digits of the fractional seconds of the column: the ones that its type says (i.e.: <c>timestamp(3)</c>), up to the digits that the database holds.
        /// </summary>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The digits.</returns>
        protected virtual int GetFractionDigits(string destinationType) =>
            Math.Min(ParseDigits(destinationType) ?? MaxFractionDigits,
                MaxFractionDigits);

        /// <summary>
        /// Reads the digits between the parentheses of a type, i.e.: <c>3</c> in <c>timestamp(3)</c>.
        /// </summary>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The digits, or <c>null</c> if the type has none.</returns>
        protected static int? ParseDigits(string destinationType)
        {
            var match = Regex.Match(destinationType ?? string.Empty,
                @"\((\d+)\)",
                RegexOptions.CultureInvariant,
                TimeSpan.FromSeconds(1));
            return match.Success && int.TryParse(match.Groups[1].Value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var digits) ? digits : (int?)null;
        }

        /// <summary>
        /// Quotes a text between single quotes, where a single quote inside of the text is doubled.
        /// </summary>
        /// <param name="value">The text.</param>
        /// <returns>The quoted text.</returns>
        protected static string Quote(string value) =>
            "'" + value.Replace("'",
                "''") + "'";

        /// <summary>
        /// Writes the bytes as hexadecimal digits.
        /// </summary>
        /// <param name="value">The bytes.</param>
        /// <returns>The digits, in upper case.</returns>
        protected static string ToHex(byte[] value)
        {
            var text = new StringBuilder(value.Length * 2);
            foreach (var item in value)
            {
                text.Append(item.ToString("X2",
                    CultureInfo.InvariantCulture));
            }

            return text.ToString();
        }

        /// <summary>
        /// Writes a date as <c>yyyy-MM-dd</c>.
        /// </summary>
        /// <param name="value">The date.</param>
        /// <returns>The text.</returns>
        protected static string ToIsoDate(DateTime value) =>
            value.ToString("yyyy-MM-dd",
                CultureInfo.InvariantCulture);

        /// <summary>
        /// Writes a date and time as <c>yyyy-MM-dd HH:mm:ss</c> (the separator can be changed), with the fractional seconds that the column holds.
        /// </summary>
        /// <param name="value">The date and time.</param>
        /// <param name="separator">The character between the date and the time.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected string ToIsoDateTime(DateTime value,
            char separator,
            string destinationType) =>
            ToIsoDate(value) + separator + value.ToString("HH:mm:ss",
                CultureInfo.InvariantCulture) + ToFraction(value.Ticks,
                GetFractionDigits(destinationType));

        /// <summary>
        /// Writes a time as <c>HH:mm:ss</c>, with the fractional seconds that the column holds.
        /// </summary>
        /// <param name="value">The time of the day.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected string ToIsoTime(TimeSpan value,
            string destinationType) =>
            value.ToString("hh\\:mm\\:ss",
                CultureInfo.InvariantCulture) + ToFraction(value.Ticks,
                GetFractionDigits(destinationType));

        /// <summary>
        /// Writes the fractional seconds of the ticks, without the zeros at the end.
        /// </summary>
        /// <param name="ticks">The ticks of the value.</param>
        /// <param name="digits">The digits that the column holds.</param>
        /// <returns>The text, i.e.: <c>.125</c>, or an empty text if there are no fractional seconds.</returns>
        protected static string ToFraction(long ticks,
            int digits)
        {
            var fraction = (ticks % TimeSpan.TicksPerSecond).ToString("0000000",
                CultureInfo.InvariantCulture).Substring(0,
                Math.Max(0,
                    Math.Min(digits,
                        7))).TrimEnd('0');
            return fraction.Length == 0 ? string.Empty : "." + fraction;
        }

        /// <summary>
        /// Writes an offset as <c>+hh:mm</c> or <c>-hh:mm</c>.
        /// </summary>
        /// <param name="offset">The offset from UTC.</param>
        /// <returns>The text.</returns>
        protected static string ToOffset(TimeSpan offset) =>
            (offset < TimeSpan.Zero ? "-" : "+") + offset.Duration().ToString("hh\\:mm",
                CultureInfo.InvariantCulture);

        #endregion
    }
}
