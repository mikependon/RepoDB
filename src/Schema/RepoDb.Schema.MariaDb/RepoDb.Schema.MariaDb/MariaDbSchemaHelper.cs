#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that parses, quotes and formats the (multi-part) names of the MariaDB objects (i.e.: <c>sales.Invoice</c> or <c>sales.`Order Details`</c>).
    /// </summary>
    internal static class MariaDbSchemaHelper
    {
        #region Public Methods

        /// <summary>
        /// Splits a name into its parts. The parts can be quoted with backticks (a <c>`</c> is escaped as <c>``</c>) or with double quotes
        /// (a <c>"</c> is escaped as <c>""</c>), so a quoted part can contain dots and spaces.
        /// </summary>
        /// <param name="name">The name to be split.</param>
        /// <returns>The unquoted parts of the name.</returns>
        public static IList<string> Split(string name)
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
                var c = name[i];
                if (c == '`' || c == '"')
                {
                    var close = c;
                    quoted = true;
                    i++;
                    while (i < name.Length)
                    {
                        if (name[i] == close)
                        {
                            if (i + 1 < name.Length && name[i + 1] == close)
                            {
                                current.Append(close);
                                i += 2;
                                continue;
                            }
                            break;
                        }
                        current.Append(name[i]);
                        i++;
                    }
                    i++;
                }
                else if (c == '.')
                {
                    Add(parts, current, quoted);
                    quoted = false;
                    i++;
                }
                else
                {
                    current.Append(c);
                    i++;
                }
            }
            Add(parts, current, quoted);

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
        public static (string Schema, string Table) Parse(string name)
        {
            var parts = Split(name);
            return parts.Count >= 2
                ? (parts[parts.Count - 2], parts[parts.Count - 1])
                : (null, parts[0]);
        }

        /// <summary>
        /// Quotes a single part of a name with backticks.
        /// </summary>
        /// <param name="part">The part of the name.</param>
        /// <returns>The quoted part.</returns>
        public static string Quote(string part) =>
            $"`{part.Replace("`", "``")}`";

        /// <summary>
        /// Quotes all the parts of a name with backticks.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The quoted name.</returns>
        public static string QuoteName(string name) =>
            string.Join(".", Split(name).Select(Quote));

        /// <summary>
        /// Formats the name of a table. The parts are kept as they are if they are plain identifiers (i.e.: <c>dbo.Person</c>),
        /// and are quoted with backticks if not (i.e.: <c>sales.`Order Details`</c>), so the result can always be parsed back.
        /// </summary>
        /// <param name="schema">The schema of the table (can be <c>null</c>).</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The formatted name of the table.</returns>
        public static string Format(string schema, string table)
        {
            var formattedTable = IsPlain(table) ? table : Quote(table);
            if (string.IsNullOrWhiteSpace(schema))
            {
                return formattedTable;
            }
            return $"{(IsPlain(schema) ? schema : Quote(schema))}.{formattedTable}";
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="current"></param>
        /// <param name="quoted"></param>
        private static void Add(List<string> parts, StringBuilder current, bool quoted)
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
