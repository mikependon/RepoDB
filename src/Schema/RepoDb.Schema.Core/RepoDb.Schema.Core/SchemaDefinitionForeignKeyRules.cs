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
    /// Checks the foreign keys of a table against the table that they reference: the referenced columns must be a key of it, and their types must match.
    /// A table that is not part of the definition cannot be checked, which is reported as a warning.
    /// </summary>
    internal static class SchemaDefinitionForeignKeyRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the foreign keys of the table.
        /// </summary>
        /// <param name="table">The table that has the foreign keys.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(TableSchema table,
            IDictionary<TableInfo, TableSchema> tables,
            DefinitionFindings findings)
        {
            foreach (var foreignKey in table.ForeignKeys.Where(key => key.ReferencedTable != null))
            {
                if (tables.TryGetValue(foreignKey.ReferencedTable, out var parent))
                {
                    CheckReferencedKey(table,
                        foreignKey,
                        parent,
                        findings);
                    CheckColumnTypes(table,
                        foreignKey,
                        parent,
                        findings);
                }
                else
                {
                    findings.AddWarning(table, $"The referenced columns of the foreign key '{foreignKey.Name}' cannot be verified, as the table '{DefinitionFindings.Describe(foreignKey.ReferencedTable)}' is not part of the definition.");
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the sets of columns of the parent table that a foreign key can reference: its primary key, its unique constraints and its unique indexes that have no filter.
        /// </summary>
        /// <param name="parent">The referenced table.</param>
        /// <returns>The column sets of the keys of the table.</returns>
        private static IEnumerable<IList<string>> GetKeys(TableSchema parent)
        {
            var keys = new List<IList<string>>();
            if (parent.PrimaryKey != null)
            {
                keys.Add(parent.PrimaryKey.Columns);
            }
            keys.AddRange(parent.UniqueConstraints.Select(constraint => constraint.Columns));
            keys.AddRange(parent.Indexes.Where(index => index.IsUnique && string.IsNullOrWhiteSpace(index.Filter)).Select(index => index.Columns));
            return keys;
        }

        /// <summary>
        /// Reports a foreign key whose referenced columns are not a key of the referenced table, in any order.
        /// </summary>
        /// <param name="table">The table that has the foreign key.</param>
        /// <param name="foreignKey">The foreign key to be checked.</param>
        /// <param name="parent">The referenced table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckReferencedKey(TableSchema table,
            ForeignKeyInfo foreignKey,
            TableSchema parent,
            DefinitionFindings findings)
        {
            var referenced = new HashSet<string>(foreignKey.ReferencedColumns, StringComparer.Ordinal);
            if (GetKeys(parent).Any(key => referenced.SetEquals(key)))
            {
                return;
            }
            findings.AddError(table, $"The foreign key '{foreignKey.Name}' references the columns ({string.Join(", ", foreignKey.ReferencedColumns)}) of the table '{DefinitionFindings.Describe(parent.Table)}', which are not its primary key or a unique key.");
        }

        /// <summary>
        /// Reports the columns of a foreign key whose type differs from the type of the column that they reference. Unknown types are not compared.
        /// </summary>
        /// <param name="table">The table that has the foreign key.</param>
        /// <param name="foreignKey">The foreign key to be checked.</param>
        /// <param name="parent">The referenced table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckColumnTypes(TableSchema table,
            ForeignKeyInfo foreignKey,
            TableSchema parent,
            DefinitionFindings findings)
        {
            var count = Math.Min(foreignKey.Columns.Count, foreignKey.ReferencedColumns.Count);
            for (var index = 0; index < count; index++)
            {
                var child = FindColumn(table, foreignKey.Columns[index]);
                var referenced = FindColumn(parent, foreignKey.ReferencedColumns[index]);
                if (HaveDifferentTypes(child, referenced))
                {
                    findings.AddWarning(table, $"The column '{child.Field.Name}' of the foreign key '{foreignKey.Name}' is a {GetTypeName(child)}, but the column '{referenced.Field.Name}' that it references is a {GetTypeName(referenced)}.");
                }
            }
        }

        /// <summary>
        /// Finds a column of a table by its name.
        /// </summary>
        /// <param name="table">The table that has the column.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns>The column, or <c>null</c> if the table has no such column.</returns>
        private static ColumnInfo FindColumn(TableSchema table,
            string name) =>
            table.Columns.FirstOrDefault(column => string.Equals(column.Field.Name, name, StringComparison.Ordinal));

        /// <summary>
        /// Checks whether two columns have different types. A missing column or an unknown type is not a difference.
        /// </summary>
        /// <param name="first">The first column.</param>
        /// <param name="second">The second column.</param>
        /// <returns><c>true</c> if both types are known and different; otherwise, <c>false</c>.</returns>
        private static bool HaveDifferentTypes(ColumnInfo first,
            ColumnInfo second)
        {
            var firstType = GetUnderlyingType(first);
            var secondType = GetUnderlyingType(second);
            return firstType != null && secondType != null && firstType != secondType;
        }

        /// <summary>
        /// Gets the type of a column without its nullability.
        /// </summary>
        /// <param name="column">The column, or <c>null</c>.</param>
        /// <returns>The type of the column, or <c>null</c> if the column or its type is not known.</returns>
        private static Type GetUnderlyingType(ColumnInfo column)
        {
            var type = column?.Field?.Type;
            return type == null ? null : Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Gets the readable name of the type of a column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The name of the type of the column.</returns>
        private static string GetTypeName(ColumnInfo column) =>
            GetUnderlyingType(column).Name;

        #endregion
    }
}
