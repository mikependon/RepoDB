#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Checks the structure of a table: its columns, and that its keys, indexes and foreign keys refer to columns that exist, and to tables that the definition can resolve.
    /// </summary>
    internal static class SchemaDefinitionTableRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the structure of the table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="external">The identities of the external dependencies of the definition.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(TableSchema table,
            IDictionary<TableInfo, TableSchema> tables,
            ISet<TableInfo> external,
            DefinitionFindings findings)
        {
            var columns = new HashSet<string>(table.Columns.Select(column => column.Field.Name),
                StringComparer.Ordinal);
            CheckColumns(table,
                columns,
                findings);
            CheckKeysAndIndexes(table,
                columns,
                findings);
            foreach (var foreignKey in table.ForeignKeys)
            {
                CheckForeignKey(table,
                    foreignKey,
                    columns,
                    tables,
                    external,
                    findings);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reports the columns whose names are repeated, and the column ordinals that are invalid or repeated.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="columns">The distinct names of the columns of the table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckColumns(TableSchema table,
            ISet<string> columns,
            DefinitionFindings findings)
        {
            if (columns.Count != table.Columns.Count)
            {
                findings.AddError(table, "Duplicate column name.");
            }
            if (table.Columns.Any(column => column.Ordinal < 1) || table.Columns.Select(column => column.Ordinal).Distinct().Count() != table.Columns.Count)
            {
                findings.AddError(table, "Invalid or duplicate column ordinal.");
            }
        }

        /// <summary>
        /// Checks the columns of the primary key, of the unique constraints and of the indexes.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckKeysAndIndexes(TableSchema table,
            ISet<string> columns,
            DefinitionFindings findings)
        {
            if (table.PrimaryKey != null)
            {
                ReportInvalidColumnReferences(table,
                    $"primary key '{table.PrimaryKey.Name}'",
                    table.PrimaryKey.Columns,
                    columns,
                    "the table",
                    false,
                    findings);
            }
            foreach (var constraint in table.UniqueConstraints)
            {
                ReportInvalidColumnReferences(table,
                    $"unique constraint '{constraint.Name}'",
                    constraint.Columns,
                    columns,
                    "the table",
                    false,
                    findings);
            }
            foreach (var index in table.Indexes)
            {
                ReportInvalidColumnReferences(table,
                    $"index '{index.Name}'",
                    index.Columns,
                    columns,
                    "the table",
                    false,
                    findings);
                ReportInvalidColumnReferences(table,
                    $"included columns of the index '{index.Name}'",
                    index.IncludedColumns,
                    columns,
                    "the table",
                    true,
                    findings);
            }
        }

        /// <summary>
        /// Checks a foreign key: its columns exist, they match the referenced columns in number, and the referenced table can be resolved.
        /// </summary>
        /// <param name="table">The table that has the foreign key.</param>
        /// <param name="foreignKey">The foreign key to be checked.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="external">The identities of the external dependencies of the definition.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckForeignKey(TableSchema table,
            ForeignKeyInfo foreignKey,
            ISet<string> columns,
            IDictionary<TableInfo, TableSchema> tables,
            ISet<TableInfo> external,
            DefinitionFindings findings)
        {
            ReportInvalidColumnReferences(table,
                $"foreign key '{foreignKey.Name}'",
                foreignKey.Columns,
                columns,
                "the table",
                false,
                findings);
            if (foreignKey.Columns.Count != foreignKey.ReferencedColumns.Count)
            {
                findings.AddError(table, $"Foreign-key column counts differ: the foreign key '{foreignKey.Name}' has {foreignKey.Columns.Count} column(s) but references {foreignKey.ReferencedColumns.Count}.");
            }
            if (foreignKey.ReferencedTable != null)
            {
                CheckReferencedTable(table,
                    foreignKey,
                    tables,
                    external,
                    findings);
            }
        }

        /// <summary>
        /// Checks the table that a foreign key references: if the definition has it, the referenced columns must exist; otherwise, it must be an external dependency.
        /// </summary>
        /// <param name="table">The table that has the foreign key.</param>
        /// <param name="foreignKey">The foreign key to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="external">The identities of the external dependencies of the definition.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckReferencedTable(TableSchema table,
            ForeignKeyInfo foreignKey,
            IDictionary<TableInfo, TableSchema> tables,
            ISet<TableInfo> external,
            DefinitionFindings findings)
        {
            if (tables.TryGetValue(foreignKey.ReferencedTable, out var target))
            {
                var targetColumns = new HashSet<string>(target.Columns.Select(column => column.Field.Name),
                    StringComparer.Ordinal);
                ReportInvalidColumnReferences(table,
                    $"referenced columns of the foreign key '{foreignKey.Name}'",
                    foreignKey.ReferencedColumns,
                    targetColumns,
                    $"the table '{DefinitionFindings.Describe(target.Table)}'",
                    false,
                    findings);
            }
            else if (!external.Contains(foreignKey.ReferencedTable))
            {
                findings.AddError(table, $"A foreign-key target is missing from the definition and external dependencies: the foreign key '{foreignKey.Name}' references the table '{DefinitionFindings.Describe(foreignKey.ReferencedTable)}'.");
            }
        }

        /// <summary>
        /// Reports the columns of a key or an index that are missing, repeated, or not in the table.
        /// </summary>
        /// <param name="table">The table that has the key or the index.</param>
        /// <param name="owner">The description of the key or the index (i.e.: primary key 'PK_Orders').</param>
        /// <param name="names">The names of the columns of the key or the index.</param>
        /// <param name="columns">The names of the columns of the table that they refer to.</param>
        /// <param name="where">The description of the table that they refer to (i.e.: the table).</param>
        /// <param name="allowEmpty">Whether the key or the index can have no columns.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportInvalidColumnReferences(TableSchema table,
            string owner,
            IList<string> names,
            ISet<string> columns,
            string where,
            bool allowEmpty,
            DefinitionFindings findings)
        {
            if (!allowEmpty && names.Count == 0)
            {
                findings.AddError(table, $"A key or index contains invalid column references: the {owner} has no columns.");
                return;
            }
            var repeated = names.GroupBy(name => name, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
            if (repeated.Count > 0)
            {
                findings.AddError(table, $"A key or index contains invalid column references: the {owner} repeats the columns ({string.Join(", ", repeated)}).");
            }
            var unknown = names.Where(name => !columns.Contains(name)).Distinct(StringComparer.Ordinal).ToList();
            if (unknown.Count > 0)
            {
                findings.AddError(table, $"A key or index contains invalid column references: the {owner} refers to the columns ({string.Join(", ", unknown)}) that {where} does not have.");
            }
        }

        #endregion
    }
}
