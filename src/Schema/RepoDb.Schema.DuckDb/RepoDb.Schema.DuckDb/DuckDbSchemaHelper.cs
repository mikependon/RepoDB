#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckDB.NET.Data;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that parses, quotes and formats the (multi-part) names of the DuckDb objects (i.e.: <c>sales.Invoice</c> or <c>sales.`Order Details`</c>).
    /// </summary>
    internal static class DuckDbSchemaHelper
    {
        #region Properties

        /// <summary>
        /// Gets the setting of the connection: its quotes and its separator are used to parse, quote and format the names.
        /// The default setting of the provider is used if the provider is not initialized.
        /// </summary>
        private static IDbSetting Setting =>
            DbSettingMapper.Get<DuckDBConnection>() ?? new DuckDbDbSetting();

        /// <summary>
        /// Gets the default schema of the setting, which owns the tables whose names have no schema.
        /// </summary>
        public static string DefaultSchema =>
            Setting.DefaultSchema;

        /// <summary>
        /// Gets the character that opens a quoted part of a name.
        /// </summary>
        private static char OpeningQuote =>
            Setting.OpeningQuote[0];

        /// <summary>
        /// Gets the character that closes a quoted part of a name.
        /// </summary>
        private static char ClosingQuote =>
            Setting.ClosingQuote[0];

        /// <summary>
        /// Gets the character that separates the parts of a name.
        /// </summary>
        private static char Separator =>
            string.IsNullOrEmpty(Setting.SchemaSeparator) ? '.' : Setting.SchemaSeparator[0];

        #endregion

        #region Public Methods

        /// <summary>
        /// Splits a name into its parts. The parts can be quoted with the quotes of the setting or with alternative quotes (a closing quote is escaped by doubling it), so a quoted part can contain dots and spaces.
        /// </summary>
        /// <param name="name">The name to be split.</param>
        /// <returns>The unquoted parts of the name.</returns>
        public static IList<string> SplitNameIntoParts(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parts = new List<string>();
            var current = new StringBuilder();
            var quoted = false;
            var i = 0;
            while (i < name.Length)
            {
                if (IsOpeningQuote(name[i]))
                {
                    i = ReadQuoted(name, i, current);
                    quoted = true;
                }
                else if (name[i] == Separator)
                {
                    AddCurrentPart(parts, current, quoted);
                    quoted = false;
                    i++;
                }
                else
                {
                    current.Append(name[i]);
                    i++;
                }
            }
            AddCurrentPart(parts, current, quoted);

            if (parts.Count == 0)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return parts;
        }

        /// <summary>
        /// Parses a table name. Only the last 2 parts are used: the schema (<c>null</c> if the name has no schema) and the table.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <returns>The schema and the table.</returns>
        public static (string Schema, string Table) ParseSchemaAndTable(string name)
        {
            var parts = SplitNameIntoParts(name);
            return parts.Count >= 2
                ? (parts[parts.Count - 2], parts[parts.Count - 1])
                : (null, parts[0]);
        }

        /// <summary>
        /// Quotes a single part of a name with the quotes of the setting (a part can contain dots and spaces). A closing quote inside the part is escaped by doubling it.
        /// </summary>
        /// <param name="part">The part of the name.</param>
        /// <returns>The quoted part.</returns>
        public static string Quote(string part)
        {
            var setting = Setting;
            return part.Length == 0 || part.IndexOf(OpeningQuote) >= 0 || part.IndexOf(ClosingQuote) >= 0
                ? string.Concat(setting.OpeningQuote, part.Replace(setting.ClosingQuote, setting.ClosingQuote + setting.ClosingQuote), setting.ClosingQuote)
                : part.AsQuoted(false, true, setting);
        }

        /// <summary>
        /// Quotes all the parts of a name with the quotes of the setting.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The quoted name.</returns>
        public static string QuoteName(string name) =>
            string.Join(Separator.ToString(), SplitNameIntoParts(name).Select(Quote));

        /// <summary>
        /// Quotes the schema and the table, and joins them with the separator of the setting.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The quoted name of the table.</returns>
        public static string QuoteSchemaAndTable(string schema, string table) =>
            string.Concat(Quote(schema), Separator, Quote(table));

        /// <summary>
        /// Formats the name of a table. The parts are kept as they are if they are plain identifiers (i.e.: <c>dbo.Person</c>),
        /// and are quoted with double quotes if not (i.e.: <c>sales."Order Details"</c>), so the result can always be parsed back.
        /// </summary>
        /// <param name="schema">The schema of the table (can be <c>null</c>).</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The formatted name of the table.</returns>
        public static string FormatTableName(string schema, string table)
        {
            var formattedTable = IsPlain(table) ? table : Quote(table);
            if (string.IsNullOrWhiteSpace(schema))
            {
                return formattedTable;
            }
            return $"{(IsPlain(schema) ? schema : Quote(schema))}{Separator}{formattedTable}";
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks whether the character opens a quoted part of a name: the opening quote of the setting, or an alternative quote.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsOpeningQuote(char c) =>
            c == OpeningQuote || c == '"';

        /// <summary>
        /// Gets the character that closes the quoted part that is opened by the character.
        /// </summary>
        /// <param name="opening"></param>
        /// <returns></returns>
        private static char GetClosingQuote(char opening) =>
            opening == OpeningQuote ? ClosingQuote : opening;

        /// <summary>
        /// Reads a quoted part of a name (a closing quote that is doubled is a closing quote of the part) and appends the unquoted part to the buffer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="start">The position of the opening quote.</param>
        /// <param name="current"></param>
        /// <returns>The position after the closing quote.</returns>
        private static int ReadQuoted(string name,
            int start,
            StringBuilder current)
        {
            var close = GetClosingQuote(name[start]);
            var i = start + 1;
            while (i < name.Length)
            {
                if (name[i] == close && !IsEscapedQuote(name, i, close))
                {
                    break;
                }
                if (name[i] == close)
                {
                    i++;
                }
                current.Append(name[i]);
                i++;
            }
            return i + 1;
        }

        /// <summary>
        /// Checks whether the closing quote at the position is doubled, which escapes it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        private static bool IsEscapedQuote(string name,
            int position,
            char close) =>
            position + 1 < name.Length && name[position + 1] == close;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="current"></param>
        /// <param name="quoted"></param>
        private static void AddCurrentPart(List<string> parts, StringBuilder current, bool quoted)
        {
            var part = quoted ? current.ToString() : current.ToString().Trim();
            current.Clear();
            if (part.Length > 0 || quoted)
            {
                parts.Add(part);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private static bool IsPlain(string part) =>
            part.Length > 0 &&
            !char.IsDigit(part[0]) &&
            part.All(c => char.IsLetterOrDigit(c) || c == '_');

        #endregion
    }
}
