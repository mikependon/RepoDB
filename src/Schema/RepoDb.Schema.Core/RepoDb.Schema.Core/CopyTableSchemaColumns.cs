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
    /// The columns of the requested tables can also be renamed: the primary key, the indexes, the unique constraints, the check constraints and the foreign keys (including the ones of
    /// the other tables that reference the renamed columns) follow the new names.
    /// </summary>
    internal static class CopyTableSchemaColumns
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
        /// Validates the new names of the columns.
        /// </summary>
        /// <param name="renamedColumns">The source column names and the names that the columns have in the destination database (<c>null</c> if no column is renamed).</param>
        /// <returns>The new names by the source column name (not case sensitive), or <c>null</c> if no column is renamed.</returns>
        /// <exception cref="ArgumentException">A name is <c>null</c> or white space, or a column is given more than once.</exception>
        public static IDictionary<string, string> NormalizeRenamed(IDictionary<string, string> renamedColumns)
        {
            if (renamedColumns == null || renamedColumns.Count == 0)
            {
                return null;
            }
            var renamed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in renamedColumns)
            {
                if (string.IsNullOrWhiteSpace(pair.Key) || string.IsNullOrWhiteSpace(pair.Value))
                {
                    throw new ArgumentException("The names of the renamed columns can not be null or white space.", nameof(renamedColumns));
                }
                if (renamed.ContainsKey(pair.Key.Trim()))
                {
                    throw new ArgumentException($"The column '{pair.Key.Trim()}' is renamed more than once.", nameof(renamedColumns));
                }
                renamed[pair.Key.Trim()] = pair.Value.Trim();
            }
            return renamed;
        }

        /// <summary>
        /// Leaves out of the requested tables the columns that are not given, and the objects that use them, and renames the columns that are asked to be renamed.
        /// </summary>
        /// <param name="schemas">The schemas of all the tables that are copied (the requested tables and their related tables).</param>
        /// <param name="requested">The schemas of the tables that are requested to be copied: the columns are only filtered and renamed in these tables.</param>
        /// <param name="columns">The distinct names of the columns to be copied (see <see cref="Normalize"/>), or <c>null</c> if all the columns are copied.</param>
        /// <param name="renamed">The new names of the columns by the source column name (see <see cref="NormalizeRenamed"/>), or <c>null</c> if no column is renamed.</param>
        /// <returns>The schemas of the tables, where the requested ones have only the given columns, by their new names. The schemas that are read from the source database are not changed.</returns>
        /// <exception cref="ArgumentException">A column is not a column of a requested table, or the new names make a table have the same column twice.</exception>
        public static IList<TableSchema> Apply(IList<TableSchema> schemas,
            IEnumerable<TableSchema> requested,
            IList<string> columns,
            IDictionary<string, string> renamed = null)
        {
            // The columns that are kept in each requested table (the names are the ones of the source table)
            var kept = new Dictionary<TableInfo, ISet<string>>();
            foreach (var schema in requested)
            {
                var names = new HashSet<string>(schema.Columns.Select(column => column.Field.Name), StringComparer.OrdinalIgnoreCase);
                var missing = columns?.FirstOrDefault(column => !names.Contains(column));
                if (missing != null)
                {
                    throw new ArgumentException($"The column '{missing}' is not a column of the table '{GetName(schema.Table)}'.", nameof(columns));
                }
                missing = renamed?.Keys.FirstOrDefault(column => !names.Contains(column));
                if (missing != null)
                {
                    throw new ArgumentException($"The column '{missing}' is not a column of the table '{GetName(schema.Table)}'.", "renamedColumns");
                }
                kept[schema.Table] = columns == null ? names : new HashSet<string>(columns, StringComparer.OrdinalIgnoreCase);
            }

            // The tables are filtered, then the foreign keys that reference the columns that are left out are left out
            var filtered = schemas
                .Select(schema => kept.TryGetValue(schema.Table, out var names) ? Filter(schema, names) : schema)
                .ToList();
            var removed = schemas
                .Where(schema => kept.ContainsKey(schema.Table))
                .ToDictionary(schema => schema.Table, schema => (ISet<string>)new HashSet<string>(
                    schema.Columns.Select(column => column.Field.Name).Where(name => !kept[schema.Table].Contains(name)), StringComparer.OrdinalIgnoreCase));
            var result = filtered.Select(schema => RemoveForeignKeys(schema, removed)).ToList();

            // The columns are renamed, and what uses them follows the new names
            return renamed == null ? result : result.Select(schema => Rename(schema, kept.ContainsKey(schema.Table), kept.Keys.ToList(), renamed)).ToList();
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
        /// Gets a copy of the schema of the table where the columns have their new names, and the objects that use them follow the new names.
        /// </summary>
        /// <param name="source">The schema of the table.</param>
        /// <param name="isRequested"><c>true</c> if the table is a requested table, whose columns are renamed.</param>
        /// <param name="requested">The identities of the requested tables: the foreign keys that reference their columns follow the new names.</param>
        /// <param name="renamed">The new names of the columns by the source column name.</param>
        /// <returns>The schema of the table (the same schema if nothing is renamed).</returns>
        /// <exception cref="ArgumentException">The new names make the table have the same column twice.</exception>
        private static TableSchema Rename(TableSchema source,
            bool isRequested,
            IList<TableInfo> requested,
            IDictionary<string, string> renamed)
        {
            IList<string> Names(IList<string> names) =>
                names.Select(name => Name(name, renamed)).ToList();

            if (!isRequested)
            {
                if (!source.ForeignKeys.Any(foreignKey => requested.Contains(foreignKey.ReferencedTable) && foreignKey.ReferencedColumns.Any(renamed.ContainsKey)))
                {
                    return source;
                }
                return new TableSchema(source.Table.Name, source.Table.Schema)
                {
                    CheckConstraints = source.CheckConstraints,
                    Columns = source.Columns,
                    Indexes = source.Indexes,
                    PrimaryKey = source.PrimaryKey,
                    UniqueConstraints = source.UniqueConstraints,
                    ForeignKeys = source.ForeignKeys.Select(foreignKey => RenameForeignKey(foreignKey, requested, renamed, false)).ToList()
                };
            }

            var columns = source.Columns.Select(column => RenameColumn(column, Name(column.Field.Name, renamed))).ToList();
            var duplicate = columns.GroupBy(column => column.Field.Name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
            if (duplicate != null)
            {
                throw new ArgumentException($"The column '{duplicate.Key}' exists more than once in the table '{GetName(source.Table)}' after the columns are renamed.", "renamedColumns");
            }

            return new TableSchema(source.Table.Name, source.Table.Schema)
            {
                Columns = columns,
                PrimaryKey = source.PrimaryKey == null ? null : new PrimaryKeyInfo(source.PrimaryKey.Name) { Columns = Names(source.PrimaryKey.Columns), IsClustered = source.PrimaryKey.IsClustered },
                Indexes = source.Indexes.Select(index => new IndexInfo(index.Name)
                {
                    IsUnique = index.IsUnique,
                    Columns = Names(index.Columns),
                    IncludedColumns = Names(index.IncludedColumns),
                    DescendingColumns = Names(index.DescendingColumns),
                    IsClustered = index.IsClustered,
                    Filter = RenameInExpression(index.Filter, renamed)
                }).ToList(),
                UniqueConstraints = source.UniqueConstraints.Select(constraint => new UniqueConstraintInfo(constraint.Name) { Columns = Names(constraint.Columns) }).ToList(),
                CheckConstraints = source.CheckConstraints.Select(constraint => new CheckConstraintInfo(constraint.Name) { Expression = RenameInExpression(constraint.Expression, renamed) }).ToList(),
                ForeignKeys = source.ForeignKeys.Select(foreignKey => RenameForeignKey(foreignKey, requested, renamed, true)).ToList()
            };
        }

        /// <summary>
        /// Gets the new name of a column.
        /// </summary>
        /// <param name="name">The name of the column in the source table.</param>
        /// <param name="renamed">The new names of the columns by the source column name.</param>
        /// <returns>The new name, or the same name if the column is not renamed.</returns>
        private static string Name(string name,
            IDictionary<string, string> renamed) =>
            renamed.TryGetValue(name, out var newName) ? newName : name;

        /// <summary>
        /// Gets a copy of the column with another name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The new name of the column.</param>
        /// <returns>The copy of the column.</returns>
        private static ColumnInfo RenameColumn(ColumnInfo column,
            string name)
        {
            var field = column.Field;
            return new ColumnInfo
            {
                Field = new DbField(name, field.IsPrimary, field.IsIdentity, field.IsNullable, field.Type, field.Size, field.Precision, field.Scale, field.DatabaseType, field.HasDefaultValue, field.Provider),
                Ordinal = column.Ordinal,
                DefaultExpression = column.DefaultExpression,
                IdentitySeed = column.IdentitySeed,
                IdentityIncrement = column.IdentityIncrement,
                ComputedExpression = column.ComputedExpression,
                Collation = column.Collation,
                Comment = column.Comment
            };
        }

        /// <summary>
        /// Gets a copy of the foreign key whose columns follow the new names.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <param name="requested">The identities of the requested tables.</param>
        /// <param name="renamed">The new names of the columns by the source column name.</param>
        /// <param name="ownColumns"><c>true</c> if the columns of the foreign key are columns of a requested table (and are renamed).</param>
        /// <returns>The copy of the foreign key.</returns>
        private static ForeignKeyInfo RenameForeignKey(ForeignKeyInfo foreignKey,
            IList<TableInfo> requested,
            IDictionary<string, string> renamed,
            bool ownColumns) =>
            new ForeignKeyInfo(foreignKey.Name)
            {
                Columns = ownColumns ? foreignKey.Columns.Select(name => Name(name, renamed)).ToList() : foreignKey.Columns,
                ReferencedTable = foreignKey.ReferencedTable,
                ReferencedColumns = requested.Contains(foreignKey.ReferencedTable) ? foreignKey.ReferencedColumns.Select(name => Name(name, renamed)).ToList() : foreignKey.ReferencedColumns,
                UpdateRule = foreignKey.UpdateRule,
                DeleteRule = foreignKey.DeleteRule
            };

        /// <summary>
        /// Replaces the names of the renamed columns that the expression mentions (as whole words) with their new names, all at once (so two columns can swap their names).
        /// </summary>
        /// <param name="expression">The expression of a check constraint or of the filter of an index.</param>
        /// <param name="renamed">The new names of the columns by the source column name.</param>
        /// <returns>The expression.</returns>
        private static string RenameInExpression(string expression,
            IDictionary<string, string> renamed)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return expression;
            }
            var pattern = $@"(?<![\w])({string.Join("|", renamed.Keys.Select(Regex.Escape))})(?![\w])";
            return Regex.Replace(expression, pattern, match => Name(match.Value, renamed), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
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
