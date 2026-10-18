#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that filters the columns of the tables that are requested to be copied: only the given columns are created in the destination database,
    /// together with the objects that only use them. The index, the primary key, the unique constraint and the foreign key that use a column that is left out are left out too,
    /// and so is the check constraint that mentions it. The foreign key of another table that references a column that is left out is left out as well.
    /// </summary>
    internal static class CopySchemaColumns
    {
        #region Public Methods

        /// <summary>
        /// Validates the names of the columns and removes the duplicates.
        /// </summary>
        /// <param name="columns">The names of the columns to be copied (<c>null</c> if all the columns are copied).</param>
        /// <returns>The distinct names of the columns, or <c>null</c> if all the columns are copied.</returns>
        /// <exception cref="ArgumentException">The array is empty, or a name is <c>null</c> or white space.</exception>
        public static IList<string> Normalize(string[] columns)
        {
            if (columns == null)
            {
                return null;
            }
            if (columns.Length == 0)
            {
                throw new ArgumentException("At least one column must be given: a table can not be created without columns.", nameof(columns));
            }
            if (columns.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("The names of the columns can not be null or white space.", nameof(columns));
            }
            return columns
                .Select(column => column.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// Leaves out of the requested tables the columns that are not given, and the objects that use them.
        /// </summary>
        /// <param name="schemas">The schemas of all the tables that are copied (the requested tables and their related tables).</param>
        /// <param name="requested">The schemas of the tables that are requested to be copied: the columns are only filtered in these tables.</param>
        /// <param name="columns">The distinct names of the columns to be copied (see <see cref="Normalize"/>).</param>
        /// <returns>The schemas of the tables, where the requested ones have only the given columns. The schemas that are read from the source database are not changed.</returns>
        /// <exception cref="ArgumentException">A column is not a column of a requested table.</exception>
        public static IList<TableSchema> Apply(IList<TableSchema> schemas,
            IEnumerable<TableSchema> requested,
            IList<string> columns)
        {
            // The columns that are kept in each requested table (the names are the ones of the source table)
            var kept = new Dictionary<TableInfo, ISet<string>>();
            foreach (var schema in requested)
            {
                var names = new HashSet<string>(schema.Columns.Select(column => column.Field.Name), StringComparer.OrdinalIgnoreCase);
                var missing = columns.FirstOrDefault(column => !names.Contains(column));
                if (missing != null)
                {
                    throw new ArgumentException($"The column '{missing}' is not a column of the table '{GetName(schema.Table)}'.", nameof(columns));
                }
                kept[schema.Table] = new HashSet<string>(columns, StringComparer.OrdinalIgnoreCase);
            }

            // The tables are filtered, then the foreign keys that reference the columns that are left out are left out
            var filtered = schemas
                .Select(schema => kept.TryGetValue(schema.Table, out var names) ? Filter(schema, names) : schema)
                .ToList();
            var removed = schemas
                .Where(schema => kept.ContainsKey(schema.Table))
                .ToDictionary(schema => schema.Table, schema => (ISet<string>)new HashSet<string>(
                    schema.Columns.Select(column => column.Field.Name).Where(name => !kept[schema.Table].Contains(name)), StringComparer.OrdinalIgnoreCase));
            return filtered.Select(schema => RemoveForeignKeys(schema, removed)).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the name of the table with its schema.
        /// </summary>
        /// <param name="table">The identity of the table.</param>
        /// <returns>The name of the table, i.e.: <c>dbo.Person</c>.</returns>
        private static string GetName(TableInfo table) =>
            string.IsNullOrWhiteSpace(table.Schema) ? table.Name : $"{table.Schema}.{table.Name}";

        /// <summary>
        /// Gets a copy of the schema of the table with only the given columns, and the objects that only use them.
        /// </summary>
        /// <param name="source">The schema of the table, as read from the source database.</param>
        /// <param name="kept">The names of the columns to be kept.</param>
        /// <returns>The schema of the table that has only the given columns.</returns>
        private static TableSchema Filter(TableSchema source,
            ISet<string> kept)
        {
            var removed = source.Columns
                .Select(column => column.Field.Name)
                .Where(name => !kept.Contains(name))
                .ToList();
            if (removed.Count == 0)
            {
                return source;
            }

            return new TableSchema(source.Table.Name, source.Table.Schema)
            {
                Columns = source.Columns.Where(column => kept.Contains(column.Field.Name)).ToList(),
                PrimaryKey = source.PrimaryKey != null && source.PrimaryKey.Columns.All(kept.Contains) ? source.PrimaryKey : null,
                Indexes = source.Indexes.Where(index => index.Columns.Concat(index.IncludedColumns).All(kept.Contains)).ToList(),
                UniqueConstraints = source.UniqueConstraints.Where(constraint => constraint.Columns.All(kept.Contains)).ToList(),
                CheckConstraints = source.CheckConstraints.Where(constraint => !Mentions(constraint.Expression, removed)).ToList(),
                ForeignKeys = source.ForeignKeys.Where(foreignKey => foreignKey.Columns.All(kept.Contains)).ToList()
            };
        }

        /// <summary>
        /// Gets the schema without the foreign keys that reference a column that is left out of a table.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="removed">The names of the columns that are left out of the tables that are filtered.</param>
        /// <returns>The schema of the table (a copy of it if a foreign key is left out).</returns>
        private static TableSchema RemoveForeignKeys(TableSchema schema,
            IDictionary<TableInfo, ISet<string>> removed)
        {
            bool References(ForeignKeyInfo foreignKey) =>
                removed.TryGetValue(foreignKey.ReferencedTable, out var names) && foreignKey.ReferencedColumns.Any(names.Contains);

            if (!schema.ForeignKeys.Any(References))
            {
                return schema;
            }

            return new TableSchema(schema.Table.Name, schema.Table.Schema)
            {
                CheckConstraints = schema.CheckConstraints,
                Columns = schema.Columns,
                Indexes = schema.Indexes,
                PrimaryKey = schema.PrimaryKey,
                UniqueConstraints = schema.UniqueConstraints,
                ForeignKeys = schema.ForeignKeys.Where(foreignKey => !References(foreignKey)).ToList()
            };
        }

        /// <summary>
        /// Checks whether the expression of a check constraint mentions one of the columns (as a whole word).
        /// </summary>
        /// <param name="expression">The expression of the check constraint.</param>
        /// <param name="columns">The names of the columns.</param>
        /// <returns><c>true</c> if the expression mentions a column; otherwise, <c>false</c>.</returns>
        private static bool Mentions(string expression,
            IEnumerable<string> columns) =>
            !string.IsNullOrWhiteSpace(expression) &&
            columns.Any(column => Regex.IsMatch(expression, $@"(?<![\w]){Regex.Escape(column)}(?![\w])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)));

        #endregion
    }
}
