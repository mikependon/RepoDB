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
    /// the other tables that reference the renamed columns) follow the new names. The columns can be selected for every requested table the same way, or table by table.
    /// </summary>
    internal static class ColumnSelector
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
        /// Validates the mappings of the columns, and gets the columns to be created and their target names.
        /// </summary>
        /// <param name="mappings">The mappings of the columns of the source table to the columns of the destination table.</param>
        /// <param name="columns">The distinct names of the source columns to be created.</param>
        /// <param name="renamed">The target names by the source column name (not case sensitive), for the columns whose target name is not the source name (<c>null</c> if no column is renamed).</param>
        /// <exception cref="ArgumentNullException">The mappings are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The list is empty, an item is <c>null</c>, a source column is <c>null</c> or white space, or a source column is mapped more than once.</exception>
        public static void Normalize(IList<ColumnMapping> mappings,
            out IList<string> columns,
            out IDictionary<string, string> renamed)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }
            if (mappings.Count == 0)
            {
                throw new ArgumentException("At least one column mapping must be given: a table can not be created without columns.", nameof(mappings));
            }
            var sources = new List<string>();
            var targets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mapping in mappings)
            {
                if (mapping == null)
                {
                    throw new ArgumentException("The column mappings can not contain a null item.", nameof(mappings));
                }
                if (string.IsNullOrWhiteSpace(mapping.Source))
                {
                    throw new ArgumentException("The source column of a column mapping can not be null or white space.", nameof(mappings));
                }
                var source = mapping.Source.Trim();
                if (sources.Contains(source, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"The source column '{source}' is mapped more than once.", nameof(mappings));
                }
                sources.Add(source);
                if (!string.IsNullOrWhiteSpace(mapping.Target) && !string.Equals(mapping.Target.Trim(), source, StringComparison.Ordinal))
                {
                    targets[source] = mapping.Target.Trim();
                }
            }
            columns = sources;
            renamed = targets.Count == 0 ? null : targets;
        }

        /// <summary>
        /// Validates the mappings of the tables.
        /// </summary>
        /// <param name="tables">The tables and the mappings of their columns.</param>
        /// <returns>The tables.</returns>
        /// <exception cref="ArgumentNullException">The list is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The list is empty, an item is <c>null</c>, a table name is <c>null</c> or white space, a table is given more than once, or the mappings of a table are not valid (see <see cref="Normalize(IList{ColumnMapping}, out IList{string}, out IDictionary{string, string})"/>).</exception>
        public static IList<TableMapping> Normalize(IList<TableMapping> tables)
        {
            if (tables == null)
            {
                throw new ArgumentNullException("mappings");
            }
            if (tables.Count == 0)
            {
                throw new ArgumentException("At least one table must be given.", "mappings");
            }
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var table in tables)
            {
                if (table == null)
                {
                    throw new ArgumentException("The tables can not contain a null item.", "mappings");
                }
                if (string.IsNullOrWhiteSpace(table.Source))
                {
                    throw new ArgumentException("The name of a table can not be null or white space.", "mappings");
                }
                if (!names.Add(Unquote(table.Source)))
                {
                    throw new ArgumentException($"The table '{table.Source}' is given more than once.", "mappings");
                }
                if (table.Columns == null || table.Columns.Count == 0)
                {
                    throw new ArgumentException($"At least one column mapping must be given for the table '{table.Source}': a table can not be created without columns.", "mappings");
                }
                if (table.Destination != null && string.IsNullOrWhiteSpace(Unquote(table.Destination)))
                {
                    throw new ArgumentException($"The new name of the table '{table.Source}' can not be white space.", "mappings");
                }
                Normalize(table.Columns, out _, out _);
            }
            var targets = tables.Where(table => table.Destination != null).Select(table => Unquote(table.Destination)).ToList();
            var repeated = targets.GroupBy(name => name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
            if (repeated != null)
            {
                throw new ArgumentException($"More than one table is renamed to '{repeated.Key}'.", "mappings");
            }
            return tables;
        }

        /// <summary>
        /// Finds the table that each mapped table is and gets the columns of each one.
        /// </summary>
        /// <param name="tables">The tables and the mappings of their columns (see <see cref="Normalize(IList{TableMapping})"/>).</param>
        /// <param name="schemas">The schemas of all the tables that are copied (the requested tables and their related tables).</param>
        /// <returns>The columns to be created, by the table that they are created in.</returns>
        /// <exception cref="ArgumentException">A mapped table is not one of the tables that are copied, or it can be more than one of them.</exception>
        public static IDictionary<TableInfo, Selection> Resolve(IList<TableMapping> tables,
            IList<TableSchema> schemas)
        {
            var selections = new Dictionary<TableInfo, Selection>();
            foreach (var table in tables)
            {
                var name = Unquote(table.Source);
                var matches = schemas.Select(schema => schema.Table).Where(info => Matches(name, info)).Distinct().ToList();
                if (matches.Count == 0)
                {
                    var found = string.Join(", ", schemas.Select(schema => $"'{GetName(schema.Table)}'").Distinct());
                    throw new ArgumentException($"The table '{table.Source}' is not one of the tables that are copied. The tables found are: {(found.Length == 0 ? "(none)" : found)}. The name must be the one of the table in the source database, not the new name of the table.", "mappings");
                }
                if (matches.Count > 1)
                {
                    throw new ArgumentException($"The table '{table.Source}' is more than one of the tables that are copied: give the name of its schema, i.e.: 'dbo.{name}'.", "mappings");
                }
                if (selections.ContainsKey(matches[0]))
                {
                    throw new ArgumentException($"The table '{GetName(matches[0])}' is given more than once.", "mappings");
                }
                Normalize(table.Columns, out var columns, out var renamed);
                selections[matches[0]] = new Selection(columns, renamed, table.Destination == null ? null : Unquote(table.Destination));
            }
            return selections;
        }

        /// <summary>
        /// Leaves out of the requested tables the columns that are not given, and the objects that use them, and renames the columns that are asked to be renamed.
        /// </summary>
        /// <param name="schemas">The schemas of all the tables that are copied (the requested tables and their related tables).</param>
        /// <param name="requested">The schemas of the tables that are requested to be copied: the columns are only filtered and renamed in these tables.</param>
        /// <param name="columns">The distinct names of the columns to be copied (see <see cref="Normalize(string[])"/>), or <c>null</c> if all the columns are copied.</param>
        /// <returns>The schemas of the tables, where the requested ones have only the given columns. The schemas that are read from the source database are not changed.</returns>
        /// <exception cref="ArgumentException">A column is not a column of a requested table.</exception>
        public static IList<TableSchema> Apply(IList<TableSchema> schemas,
            IEnumerable<TableSchema> requested,
            IList<string> columns)
        {
            var selections = new Dictionary<TableInfo, Selection>();
            foreach (var schema in requested)
            {
                selections[schema.Table] = new Selection(columns, null, null);
            }
            return Apply(schemas, selections);
        }

        /// <summary>
        /// Leaves out of the tables the columns that are not selected, and the objects that use them, and renames the columns that are asked to be renamed, table by table.
        /// </summary>
        /// <param name="schemas">The schemas of all the tables that are copied (the requested tables and their related tables).</param>
        /// <param name="selections">The columns to be created, by the table that they are created in. The tables that are not in the dictionary are copied as they are.</param>
        /// <returns>The schemas of the tables. The schemas that are read from the source database are not changed.</returns>
        /// <exception cref="ArgumentException">A column is not a column of its table, or the new names make a table have the same column twice.</exception>
        public static IList<TableSchema> Apply(IList<TableSchema> schemas,
            IDictionary<TableInfo, Selection> selections)
        {
            // The columns that are kept in each table (the names are the ones of the source table)
            var kept = new Dictionary<TableInfo, ISet<string>>();
            var renamedByTable = new Dictionary<TableInfo, IDictionary<string, string>>();
            foreach (var pair in selections)
            {
                var schema = schemas.First(item => item.Table.Equals(pair.Key));
                var names = new HashSet<string>(schema.Columns.Select(column => column.Field.Name), StringComparer.OrdinalIgnoreCase);
                var missing = pair.Value.Columns?.FirstOrDefault(column => !names.Contains(column));
                if (missing != null)
                {
                    throw new ArgumentException($"The column '{missing}' is not a column of the table '{GetName(schema.Table)}'.", "columns");
                }
                missing = pair.Value.Renamed?.Keys.FirstOrDefault(column => !names.Contains(column));
                if (missing != null)
                {
                    throw new ArgumentException($"The column '{missing}' is not a column of the table '{GetName(schema.Table)}'.", "mappings");
                }
                kept[pair.Key] = pair.Value.Columns == null ? names : new HashSet<string>(pair.Value.Columns, StringComparer.OrdinalIgnoreCase);
                if (pair.Value.Renamed != null)
                {
                    renamedByTable[pair.Key] = pair.Value.Renamed;
                }
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
            return renamedByTable.Count == 0 ? result : result.Select(schema => Rename(schema, renamedByTable)).ToList();
        }

        /// <summary>
        /// Gives the tables their names in the destination database, and the foreign keys that reference them follow the new names.
        /// </summary>
        /// <param name="schemas">The schemas of all the tables that are copied.</param>
        /// <param name="renamed">The new names of the tables by the table.</param>
        /// <returns>The schemas of the tables (the same list if no table is renamed). The schemas that are read from the source database are not changed.</returns>
        /// <exception cref="ArgumentException">A table is renamed to the name of another table of the same schema.</exception>
        public static IList<TableSchema> RenameTables(IList<TableSchema> schemas,
            IDictionary<TableInfo, string> renamed)
        {
            if (renamed == null || renamed.Count == 0)
            {
                return schemas;
            }

            TableInfo Map(TableInfo table) =>
                renamed.TryGetValue(table, out var name) && !string.Equals(name, table.Name, StringComparison.Ordinal) ?
                    new TableInfo(name, table.Schema, table.Database, table.Provider) : table;

            var result = schemas.Select(schema => new TableSchema(Map(schema.Table))
            {
                Columns = schema.Columns,
                PrimaryKey = schema.PrimaryKey,
                Indexes = schema.Indexes,
                UniqueConstraints = schema.UniqueConstraints,
                CheckConstraints = schema.CheckConstraints,
                ForeignKeys = schema.ForeignKeys.Select(foreignKey => new ForeignKeyInfo(foreignKey.Name)
                {
                    Columns = foreignKey.Columns,
                    ReferencedTable = Map(foreignKey.ReferencedTable),
                    ReferencedColumns = foreignKey.ReferencedColumns,
                    UpdateRule = foreignKey.UpdateRule,
                    DeleteRule = foreignKey.DeleteRule
                }).ToList()
            }).ToList();
            var duplicate = result.GroupBy(schema => schema.Table).FirstOrDefault(group => group.Count() > 1);
            if (duplicate != null)
            {
                throw new ArgumentException($"More than one table is created as '{GetName(duplicate.Key)}'.", "mappings");
            }
            return result;
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

            return new TableSchema(source.Table)
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

            return new TableSchema(schema.Table)
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
        /// <param name="renamedByTable">The new names of the columns by the source column name, by the table.</param>
        /// <returns>The schema of the table (the same schema if nothing is renamed).</returns>
        /// <exception cref="ArgumentException">The new names make the table have the same column twice.</exception>
        private static TableSchema Rename(TableSchema source,
            IDictionary<TableInfo, IDictionary<string, string>> renamedByTable)
        {
            renamedByTable.TryGetValue(source.Table, out var own);
            var hasOwn = own != null && own.Count > 0;
            var referencesRenamed = source.ForeignKeys.Any(foreignKey =>
                renamedByTable.TryGetValue(foreignKey.ReferencedTable, out var referenced) && foreignKey.ReferencedColumns.Any(referenced.ContainsKey));
            if (!hasOwn && !referencesRenamed)
            {
                return source;
            }

            IList<string> Names(IList<string> names) =>
                hasOwn ? names.Select(name => Name(name, own)).ToList() : names;

            var columns = source.Columns;
            if (hasOwn)
            {
                columns = source.Columns.Select(column => RenameColumn(column, Name(column.Field.Name, own))).ToList();
                var duplicate = columns.GroupBy(column => column.Field.Name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
                if (duplicate != null)
                {
                    throw new ArgumentException($"The column '{duplicate.Key}' exists more than once in the table '{GetName(source.Table)}' after the columns are renamed.", "mappings");
                }
            }

            return new TableSchema(source.Table)
            {
                Columns = columns,
                PrimaryKey = source.PrimaryKey == null || !hasOwn ? source.PrimaryKey : new PrimaryKeyInfo(source.PrimaryKey.Name) { Columns = Names(source.PrimaryKey.Columns), IsClustered = source.PrimaryKey.IsClustered },
                Indexes = !hasOwn ? source.Indexes : source.Indexes.Select(index => new IndexInfo(index.Name)
                {
                    IsUnique = index.IsUnique,
                    Columns = Names(index.Columns),
                    IncludedColumns = Names(index.IncludedColumns),
                    DescendingColumns = Names(index.DescendingColumns),
                    IsClustered = index.IsClustered,
                    Filter = RenameInExpression(index.Filter, own)
                }).ToList(),
                UniqueConstraints = !hasOwn ? source.UniqueConstraints : source.UniqueConstraints.Select(constraint => new UniqueConstraintInfo(constraint.Name) { Columns = Names(constraint.Columns) }).ToList(),
                CheckConstraints = !hasOwn ? source.CheckConstraints : source.CheckConstraints.Select(constraint => new CheckConstraintInfo(constraint.Name) { Expression = RenameInExpression(constraint.Expression, own) }).ToList(),
                ForeignKeys = source.ForeignKeys.Select(foreignKey =>
                {
                    renamedByTable.TryGetValue(foreignKey.ReferencedTable, out var referenced);
                    return RenameForeignKey(foreignKey, hasOwn ? own : null, referenced);
                }).ToList()
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
                DefaultValue = column.DefaultValue,
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
        /// <param name="own">The new names of the columns of the table of the foreign key by the source column name (<c>null</c> if no column of the table is renamed).</param>
        /// <param name="referenced">The new names of the columns of the referenced table by the source column name (<c>null</c> if no column of the table is renamed).</param>
        /// <returns>The copy of the foreign key.</returns>
        private static ForeignKeyInfo RenameForeignKey(ForeignKeyInfo foreignKey,
            IDictionary<string, string> own,
            IDictionary<string, string> referenced) =>
            new ForeignKeyInfo(foreignKey.Name)
            {
                Columns = own == null ? foreignKey.Columns : foreignKey.Columns.Select(name => Name(name, own)).ToList(),
                ReferencedTable = foreignKey.ReferencedTable,
                ReferencedColumns = referenced == null ? foreignKey.ReferencedColumns : foreignKey.ReferencedColumns.Select(name => Name(name, referenced)).ToList(),
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
        /// Checks whether the name that is given in the mappings is the name of the table, with or without its schema.
        /// </summary>
        /// <param name="name">The name given in the mappings, without quotes (i.e.: <c>Person</c> or <c>dbo.Person</c>).</param>
        /// <param name="table">The identity of the table.</param>
        /// <returns><c>true</c> if the name is the one of the table.</returns>
        private static bool Matches(string name,
            TableInfo table) =>
            name.IndexOf('.') >= 0
                ? string.Equals(name, GetName(table), StringComparison.OrdinalIgnoreCase)
                : string.Equals(name, table.Name, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Removes the quotes of the name of a table (i.e.: <c>[dbo].[Person]</c> is <c>dbo.Person</c>).
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <returns>The name without the quotes.</returns>
        internal static string Unquote(string name) =>
            name.Trim().Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Replace("`", string.Empty);

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

        #region Nested Types

        /// <summary>
        /// The columns that are created in a table: the source columns, and the new names of the ones that are renamed.
        /// </summary>
        internal sealed class Selection
        {
            #region Constructors

            /// <summary>
            /// Creates a new instance of <see cref="Selection"/> class.
            /// </summary>
            /// <param name="columns">The distinct names of the source columns to be created (<c>null</c> if all the columns are created).</param>
            /// <param name="renamed">The new names of the columns by the source column name (<c>null</c> if no column is renamed).</param>
            /// <param name="targetName">The name of the table in the destination database (<c>null</c> if the table keeps its name).</param>
            public Selection(IList<string> columns,
                IDictionary<string, string> renamed,
                string targetName)
            {
                Columns = columns;
                Renamed = renamed;
                TargetName = targetName;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the distinct names of the source columns to be created (<c>null</c> if all the columns are created).
            /// </summary>
            public IList<string> Columns { get; }

            /// <summary>
            /// Gets the new names of the columns by the source column name (<c>null</c> if no column is renamed).
            /// </summary>
            public IDictionary<string, string> Renamed { get; }

            /// <summary>
            /// Gets the name of the table in the destination database (<c>null</c> if the table keeps its name).
            /// </summary>
            public string TargetName { get; }

            #endregion
        }

        #endregion
    }
}
