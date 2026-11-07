#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Parses the text of a literal default value, as the dialect of the database that it comes from writes it, into a typed value.
    /// A literal that is not understood is not parsed, so it is never written wrongly in another database.
    /// </summary>
    internal static class SchemaValueParser
    {
        #region Private Variables

        private static readonly string[] DateTimeFormats =
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.FFFFFFF",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF"
        };
        private static readonly Regex Number = new Regex(@"^[+-]?(\d+(\.\d*)?|\.\d+)([eE][+-]?\d+)?$",
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        private static readonly string[] OffsetFormats =
        {
            "yyyy-MM-dd HH:mm:ssK",
            "yyyy-MM-dd HH:mm:ss.FFFFFFFK",
            "yyyy-MM-dd HH:mm:sszz",
            "yyyy-MM-dd HH:mm:ss.FFFFFFFzz",
            "yyyy-MM-dd HH:mm:ss zzz",
            "yyyy-MM-dd HH:mm:ss.FFFFFFF zzz",
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.FFFFFFF",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF"
        };
        private static readonly Regex Quoted = new Regex(@"^(?<prefix>[A-Za-z_][A-Za-z0-9_ ]*?)?\s*'(?<value>(?:[^']|'')*)'$",
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        private static readonly string[] TimeFormats =
        {
            "hh\\:mm",
            "hh\\:mm\\:ss",
            "hh\\:mm\\:ss\\.FFFFFFF"
        };

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the bytes of the hexadecimal digits.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>The bytes, or <c>null</c> if the digits are not valid.</returns>
        private static SchemaValue FromHex(string digits)
        {
            if (digits.Length % 2 != 0)
            {
                return null;
            }

            var bytes = new byte[digits.Length / 2];
            for (var index = 0; index < bytes.Length; index++)
            {
                if (!byte.TryParse(digits.Substring(index * 2,
                    2),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture,
                    out bytes[index]))
                {
                    return null;
                }
            }

            return SchemaValue.FromBinary(bytes);
        }

        /// <summary>
        /// Parses a binary as a hexadecimal literal, i.e.: <c>X'AB'</c>.
        /// </summary>
        /// <param name="isQuoted">Whether the literal is between quotes.</param>
        /// <param name="prefix">The prefix of the quoted literal, in upper case.</param>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not a binary.</returns>
        private static SchemaValue ParseBinary(bool isQuoted,
            string prefix,
            string text) =>
            isQuoted && prefix == "X" ? FromHex(text) : null;

        /// <summary>
        /// Parses a boolean: <c>1</c>, <c>0</c>, <c>true</c>, <c>false</c>, <c>t</c>, <c>f</c>, <c>yes</c>, <c>no</c>, <c>y</c>, <c>n</c>, <c>on</c> or <c>off</c>, in any case.
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not a boolean.</returns>
        private static SchemaValue ParseBoolean(string text)
        {
            switch (text.Trim().ToLowerInvariant())
            {
                case "1":
                case "true":
                case "t":
                case "yes":
                case "y":
                case "on":
                    return SchemaValue.FromBoolean(true);
                case "0":
                case "false":
                case "f":
                case "no":
                case "n":
                case "off":
                    return SchemaValue.FromBoolean(false);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parses a date and time in the ISO format.
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <param name="isDate">Whether the value is a date without a time.</param>
        /// <returns>The value, or <c>null</c> if it is not in the ISO format.</returns>
        private static SchemaValue ParseDateTime(string text,
            bool isDate)
        {
            if (!DateTime.TryParseExact(text,
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var value))
            {
                return null;
            }

            return isDate ? SchemaValue.FromDate(value) : SchemaValue.FromDateTime(value);
        }

        /// <summary>
        /// Parses a date and time with an offset in the ISO format. A value without an offset is taken as UTC.
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not in the ISO format.</returns>
        private static SchemaValue ParseDateTimeOffset(string text) =>
            DateTimeOffset.TryParseExact(text,
                OffsetFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var value) ? SchemaValue.FromDateTimeOffset(value) : null;

        /// <summary>
        /// Parses a GUID, with or without braces.
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not a GUID.</returns>
        private static SchemaValue ParseGuid(string text) =>
            Guid.TryParse(text,
                out var value) ? SchemaValue.FromGuid(value) : null;

        /// <summary>
        /// Parses a number, which can be between quotes (i.e.: <c>'0'::numeric</c>).
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not a number.</returns>
        private static SchemaValue ParseNumber(string text) =>
            Number.IsMatch(text.Trim()) ? SchemaValue.FromNumber(text.Trim().TrimStart('+')) : null;

        /// <summary>
        /// Parses a time of the day in the ISO format.
        /// </summary>
        /// <param name="text">The text of the literal, without quotes.</param>
        /// <returns>The value, or <c>null</c> if it is not in the ISO format.</returns>
        private static SchemaValue ParseTime(string text) =>
            TimeSpan.TryParseExact(text,
                TimeFormats,
                CultureInfo.InvariantCulture,
                out var value) ? SchemaValue.FromTime(value) : null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the text of a literal default value into a typed value of the kind.
        /// </summary>
        /// <param name="kind">The kind of the value (see <see cref="SchemaValueKindResolver"/>).</param>
        /// <param name="expression">The default expression, as the database that it comes from writes it (i.e.: <c>((0))</c> or <c>N'x'</c>).</param>
        /// <param name="dialect">The dialect of the database that it comes from, which rewrites its own literals (the standard one if it is <c>null</c>).</param>
        /// <returns>The value, or <c>null</c> if the expression is not a literal of the kind that is understood.</returns>
        public static SchemaValue Parse(SchemaValueKind kind,
            string expression,
            ISchemaDialect dialect)
        {
            var normalized = (dialect ?? SchemaDialect.Standard).NormalizeLiteral(SchemaDefaultValue.Unwrap(expression));
            if (normalized == null)
            {
                return null;
            }

            var literal = normalized.Trim();
            var match = Quoted.Match(literal);
            var text = match.Success ? match.Groups["value"].Value.Replace("''",
                "'") : literal;
            var prefix = match.Success ? match.Groups["prefix"].Value.Trim().ToUpperInvariant() : string.Empty;
            switch (kind)
            {
                case SchemaValueKind.Boolean:
                    return ParseBoolean(text);
                case SchemaValueKind.Number:
                    return ParseNumber(text);
                case SchemaValueKind.Text:
                    return match.Success ? SchemaValue.FromText(text) : null;
                case SchemaValueKind.Date:
                    return match.Success ? ParseDateTime(text,
                        true) : null;
                case SchemaValueKind.Time:
                    return match.Success ? ParseTime(text) : null;
                case SchemaValueKind.DateTime:
                    return match.Success ? ParseDateTime(text,
                        false) : null;
                case SchemaValueKind.DateTimeOffset:
                    return match.Success ? ParseDateTimeOffset(text) : null;
                case SchemaValueKind.Guid:
                    return ParseGuid(text);
                default:
                    return ParseBinary(match.Success,
                        prefix,
                        text);
            }
        }

        #endregion
    }
}
