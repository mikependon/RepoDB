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
    /// Checks the columns of a table: that it has some, that its primary key columns are not nullable, and that its identity columns make sense.
    /// </summary>
    internal static class SchemaDefinitionColumnRules
    {
        #region Private Variables

        /// <summary>
        /// The types that a column can have to be an identity column.
        /// </summary>
        private static readonly HashSet<Type> _identityTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal)
        };

        #endregion

        #region Internal Methods

        /// <summary>
        /// Checks the columns of the table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(TableSchema table,
            DefinitionFindings findings)
        {
            if (table.Columns.Count == 0)
            {
                findings.AddError(table, $"The table has no columns. Add at least one column. Location: {DefinitionFindings.Location(table, "Columns")}.");
                return;
            }
            ReportNullablePrimaryKeyColumns(table,
                findings);
            ReportIdentityColumns(table,
                findings);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reports the primary key columns that are nullable. Most databases refuse them, but some (i.e.: SQLite) allow them, so it is a warning.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportNullablePrimaryKeyColumns(TableSchema table,
            DefinitionFindings findings)
        {
            if (table.PrimaryKey == null)
            {
                return;
            }
            var nullable = table.Columns
                .Where(column => column.Field.IsNullable && table.PrimaryKey.Columns.Contains(column.Field.Name))
                .Select(column => column.Field.Name);
            foreach (var name in nullable)
            {
                findings.AddWarning(table, $"The primary key column '{name}' is nullable, which most databases do not allow. Make the column non-nullable or remove it from the primary key. Location: {DefinitionFindings.Location(table, "Columns", name, "IsNullable")}.");
            }
        }

        /// <summary>
        /// Reports the identity columns that have a type that cannot be an identity, and the tables that have more than one identity column.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportIdentityColumns(TableSchema table,
            DefinitionFindings findings)
        {
            var identities = table.Columns.Where(column => column.Field.IsIdentity).ToList();
            foreach (var column in identities.Where(column => !CanBeIdentity(column.Field.Type)))
            {
                findings.AddError(table, $"The identity column '{column.Field.Name}' is not of a numeric type. Choose a supported numeric type or disable identity generation. Location: {DefinitionFindings.Location(table, "Columns", column.Field.Name, "IsIdentity / Type")}.");
            }
            if (identities.Count > 1)
            {
                findings.AddWarning(table, $"The table has more than one identity column, which most databases do not allow: {string.Join(", ", identities.Select(column => column.Field.Name))}. Keep identity generation enabled on only one column if required by the destination. Location: {DefinitionFindings.Location(table, "Columns")}.");
            }
        }

        /// <summary>
        /// Checks whether a column of the type can be an identity column. A column of an unknown type is given the benefit of the doubt.
        /// </summary>
        /// <param name="type">The type of the column, or <c>null</c> if it is not known.</param>
        /// <returns><c>true</c> if the type is not known or is a numeric type; otherwise, <c>false</c>.</returns>
        private static bool CanBeIdentity(Type type) =>
            type == null || _identityTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);

        #endregion
    }
}
