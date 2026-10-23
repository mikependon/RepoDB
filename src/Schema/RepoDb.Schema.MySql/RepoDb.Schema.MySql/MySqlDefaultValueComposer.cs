#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that composes the default value of a column in the language of MySQL, from the kind of the default value
    /// that the schema reader sets (see <see cref="ColumnDefaultValue"/>). The default value that is an expression (everything but <c>CURRENT_TIMESTAMP</c> in a date and time column)
    /// is written between parentheses, which MySQL 8.0.13 and above accept.
    /// </summary>
    internal static class MySqlDefaultValueComposer
    {
        #region Private Variables

        /// <summary>
        /// The name of the destination database, as it is said in the reason why a default value is not created.
        /// </summary>
        private const string Dialect = "MySQL";

        /// <summary>
        /// The providers (see <see cref="RepoDb.DbField.Provider"/>) whose default expressions the destination understands, including its own.
        /// </summary>
        private static readonly string[] Providers = { "MYSQL", "MYSQLC", "AURORAMYSQL" };

        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

        /// <summary>
        /// The type of a date and time column, with the digits of its fractional seconds (<c>datetime(6)</c>).
        /// </summary>
        private static readonly Regex DateTimeType = new Regex(@"^(datetime|timestamp)(\((?<digits>\d)\))?$", Options);

        /// <summary>
        /// The type of a time column, with the digits of its fractional seconds (<c>time(3)</c>).
        /// </summary>
        private static readonly Regex TimeType = new Regex(@"^time(\((?<digits>\d)\))?$", Options);

        #endregion

        #region Public Methods

        /// <summary>
        /// Composes the default value of the column.
        /// </summary>
        /// <param name="column">The column, as it is read from the source database.</param>
        /// <param name="columnType">The type of the column in MySQL (i.e.: <c>datetime(6)</c>), as the composer writes it.</param>
        /// <param name="reason">The reason why the default value is not created, when this method returns <c>null</c> for a column that has one.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the column has no default value or it can not be created (see <paramref name="reason"/>).</returns>
        public static string Compose(ColumnInfo column,
            string columnType,
            out string reason)
        {
            reason = null;
            var expression = column?.DefaultExpression;
            var kind = column?.DefaultValue ?? ColumnDefaultValue.None;
            if (kind == ColumnDefaultValue.None || (kind == ColumnDefaultValue.Unclassified && string.IsNullOrWhiteSpace(expression)))
            {
                return null;
            }

            // The default that is not classified is used as it is, and so is the default of a database of the same provider (it is already written in the language of the destination)
            var provider = column.Field?.Provider;
            var isCompatible = !string.IsNullOrWhiteSpace(provider) && Providers.Contains(provider, StringComparer.OrdinalIgnoreCase);
            if (kind == ColumnDefaultValue.Unclassified || isCompatible)
            {
                return expression;
            }

            var type = (columnType ?? string.Empty).Trim();
            switch (kind)
            {
                case ColumnDefaultValue.Literal:
                    return SchemaDefaultValue.GetLiteral(expression);
                case ColumnDefaultValue.Null:
                    return "NULL";
                case ColumnDefaultValue.BooleanTrue:
                    return "1";
                case ColumnDefaultValue.BooleanFalse:
                    return "0";
                case ColumnDefaultValue.CurrentTimestamp:
                    return ComposeCurrent(type, local: true);
                case ColumnDefaultValue.CurrentTimestampUtc:
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    return ComposeCurrent(type, local: false);
                case ColumnDefaultValue.CurrentDate:
                    return "(CURRENT_DATE)";
                case ColumnDefaultValue.CurrentTime:
                    return $"(CURRENT_TIME{Digits(TimeType, type)})";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return ComposeGuid(type, out reason);
                case ColumnDefaultValue.RandomNumber:
                    return "(RAND())";
                case ColumnDefaultValue.CurrentUser:
                    return IsText(type) ? "(CURRENT_USER())" : Refuse("its column is not a text", out reason);
                case ColumnDefaultValue.CurrentDatabase:
                    return IsText(type) ? "(DATABASE())" : Refuse("its column is not a text", out reason);
                case ColumnDefaultValue.SequenceNextValue:
                    return Refuse($"its value comes from a sequence, which {Dialect} can not use as a default value", out reason);
                default:
                    return Refuse($"its expression '{expression}' is written in the language of {(string.IsNullOrWhiteSpace(provider) ? "another database" : provider)}, which {Dialect} does not understand", out reason);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Composes the current date and time: <c>CURRENT_TIMESTAMP</c> in a date and time column (it must have the digits of the fractional seconds of the column),
        /// an expression in the other columns.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <param name="local"><c>true</c> for the date and time of the server; <c>false</c> for the date and time in UTC.</param>
        /// <returns>The text that follows <c>DEFAULT</c>.</returns>
        private static string ComposeCurrent(string type,
            bool local)
        {
            if (DateTimeType.IsMatch(type))
            {
                var digits = Digits(DateTimeType, type);
                return local ? $"CURRENT_TIMESTAMP{digits}" : $"(UTC_TIMESTAMP{digits})";
            }
            if (TimeType.IsMatch(type))
            {
                return local ? $"(CURRENT_TIME{Digits(TimeType, type)})" : $"(UTC_TIME{Digits(TimeType, type)})";
            }
            if (type.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                return local ? "(CURRENT_DATE)" : "(UTC_DATE())";
            }
            return local ? "(NOW())" : "(UTC_TIMESTAMP())";
        }

        /// <summary>
        /// Composes a new GUID: <c>UUID()</c> in a text column, and its bytes in a binary one.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <param name="reason">The reason why it is not created.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the type of the column can not hold a GUID.</returns>
        private static string ComposeGuid(string type,
            out string reason)
        {
            if (IsText(type))
            {
                reason = null;
                return "(UUID())";
            }
            if (type.StartsWith("binary(16)", StringComparison.OrdinalIgnoreCase) || type.StartsWith("varbinary(16)", StringComparison.OrdinalIgnoreCase))
            {
                reason = null;
                return "(UNHEX(REPLACE(UUID(), '-', '')))";
            }
            return Refuse("its column is not a text or a binary of 16 bytes, which are the types that hold a GUID", out reason);
        }

        /// <summary>
        /// Gets the digits of the fractional seconds of the type, between parentheses (empty if the type has none), because <c>CURRENT_TIMESTAMP</c> must have the digits of its column.
        /// </summary>
        /// <param name="pattern">The pattern of the type.</param>
        /// <param name="type">The type of the column.</param>
        /// <returns>The digits between parentheses, i.e.: <c>(6)</c>, or an empty text.</returns>
        private static string Digits(Regex pattern,
            string type)
        {
            var match = pattern.Match(type);
            var digits = match.Success ? match.Groups["digits"].Value : string.Empty;
            return digits.Length == 0 || digits == "0" ? string.Empty : $"({digits})";
        }

        /// <summary>
        /// Checks whether the type is a text.
        /// </summary>
        /// <param name="type">The type of the column.</param>
        /// <returns><c>true</c> if the type is a text.</returns>
        private static bool IsText(string type) =>
            type.StartsWith("char", StringComparison.OrdinalIgnoreCase) ||
            type.StartsWith("varchar", StringComparison.OrdinalIgnoreCase) ||
            type.EndsWith("text", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Sets the reason why the default value is not created.
        /// </summary>
        /// <param name="text">The reason.</param>
        /// <param name="reason">The reason.</param>
        /// <returns><c>null</c>.</returns>
        private static string Refuse(string text,
            out string reason)
        {
            reason = text;
            return null;
        }

        #endregion
    }
}
