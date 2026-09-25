#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.DuckDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Composes the SQL statements of the DuckDB bulk operations.
    /// </summary>
    internal static class DuckDbText
    {
        #region Shared

        /// <summary>
        /// Gets the statement that creates an empty pseudo table with the columns of the table.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            IEnumerable<Field> fields = null)
        {
            var columnList = fields?.Any() == true ?
                fields.Select(f => f.Name.AsQuoted(true, dbSetting)).Join(", ") :
                "*";
            var temporary = pseudoTableType == DuckDbBulkImportPseudoTableType.Physical ? string.Empty : "TEMP ";

            return $"CREATE OR REPLACE {temporary}TABLE {pseudoTableName.AsQuoted(true, dbSetting)} AS SELECT {columnList} FROM {tableName.AsQuoted(true, dbSetting)} LIMIT 0;";
        }

        /// <summary>
        /// Gets the statement that drops the pseudo table.
        /// </summary>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {pseudoTableName.AsQuoted(true, dbSetting)};";

        /// <summary>
        /// Gets the name of the pseudo table for the operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        private static string GetPseudoTableName(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            string operation,
            IDbSetting dbSetting) =>
            $"{pseudoTableType}{DataEntityExtension.GetTableName(tableName, dbSetting).AsUnquoted(true, dbSetting)}{operation}";

        /// <summary>
        /// Gets the ON clause that matches the table rows with the pseudo table rows.
        /// </summary>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The ON clause.</returns>
        private static string GetOnClause(IEnumerable<Field> qualifiers,
            IDbSetting dbSetting) =>
            qualifiers
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

        /// <summary>
        /// Gets the SET clause that assigns the pseudo table values.
        /// </summary>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SET clause.</returns>
        private static string GetSetClause(IEnumerable<Field> fields,
            IDbSetting dbSetting) =>
            fields
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

        /// <summary>
        /// Checks whether the field exists in the fields.
        /// </summary>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="field">The field to find.</param>
        /// <returns><c>true</c> if the field exists; otherwise, <c>false</c>.</returns>
        private static bool Contains(IEnumerable<Field> fields,
            Field field) =>
            fields?.Any(f => string.Equals(f.Name, field?.Name, StringComparison.OrdinalIgnoreCase)) == true;

        #endregion

        #region Insert

        /// <summary>
        /// Gets the name of the pseudo table for the insert operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        public static string GetPseudoTableNameForInsert(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) =>
            GetPseudoTableName(tableName, pseudoTableType, "Insert", dbSetting);

        /// <summary>
        /// Gets the statement that inserts the pseudo table rows and returns the generated identities in row order.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetInsertFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IDbSetting dbSetting)
        {
            var columnList = fields.Select(f => f.Name.AsQuoted(true, dbSetting)).Join(", ");

            return $"INSERT INTO {tableName.AsQuoted(true, dbSetting)} ({columnList}) " +
                $"SELECT {columnList} FROM {pseudoTableName.AsQuoted(true, dbSetting)} ORDER BY rowid " +
                $"RETURNING {identityField.Name.AsQuoted(true, dbSetting)};";
        }

        #endregion

        #region Merge

        /// <summary>
        /// Gets the name of the pseudo table for the merge operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) =>
            GetPseudoTableName(tableName, pseudoTableType, "Merge", dbSetting);

        /// <summary>
        /// Gets the statement that merges the pseudo table rows into the table.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var updateableFields = fields.Where(f => !Contains(qualifiers, f)).AsList();
            var insertableFields = fields.Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)).AsList();
            var matchedClause = updateableFields.Count > 0 ?
                $"WHEN MATCHED THEN UPDATE SET {GetSetClause(updateableFields, dbSetting)} " :
                string.Empty;

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({GetOnClause(qualifiers, dbSetting)}) " +
                matchedClause +
                $"WHEN NOT MATCHED THEN INSERT ({insertableFields.Select(f => f.Name.AsQuoted(true, dbSetting)).Join(", ")}) " +
                $"VALUES ({insertableFields.Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ")});";
        }

        /// <summary>
        /// Gets the statement that returns the 1-based row order and the matched identity of every pseudo table row.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetMergeMatchSnapshotSql(string tableName,
            string pseudoTableName,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting) =>
            $"SELECT S.rowid + 1 AS {"RowOrder".AsQuoted(dbSetting)}, T.{identityField.Name.AsQuoted(true, dbSetting)} AS {"Result".AsQuoted(dbSetting)} " +
            $"FROM {pseudoTableName.AsQuoted(true, dbSetting)} S LEFT JOIN {tableName.AsQuoted(true, dbSetting)} T ON ({GetOnClause(qualifiers, dbSetting)}) " +
            "ORDER BY S.rowid;";

        /// <summary>
        /// Gets the statement that updates the matched rows, or <c>null</c> if there is nothing to update.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement, or <c>null</c> if there is nothing to update.</returns>
        public static string GetMergeUpdateOnlySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var updateableFields = fields
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase) && !Contains(qualifiers, f))
                .AsList();

            return updateableFields.Count == 0 ? null :
                GetUpdateFromPseudoTableSql(tableName, pseudoTableName, updateableFields, qualifiers, dbSetting);
        }

        /// <summary>
        /// Gets the statement that inserts the unmatched rows and returns the generated identities in row order.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="identityField">The identity field.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetMergeInsertOnlyForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var columnList = fields
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return $"INSERT INTO {quotedTableName} ({columnList}) " +
                $"SELECT {columnList} FROM {pseudoTableName.AsQuoted(true, dbSetting)} S " +
                $"WHERE NOT EXISTS (SELECT 1 FROM {quotedTableName} T WHERE {GetOnClause(qualifiers, dbSetting)}) " +
                $"ORDER BY S.rowid RETURNING {identityField.Name.AsQuoted(true, dbSetting)};";
        }

        #endregion

        #region Update

        /// <summary>
        /// Gets the name of the pseudo table for the update operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        public static string GetPseudoTableNameForUpdate(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) =>
            GetPseudoTableName(tableName, pseudoTableType, "Update", dbSetting);

        /// <summary>
        /// Gets the statement that updates the table from the pseudo table rows.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="fields">The fields to be used.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting) =>
            $"UPDATE {tableName.AsQuoted(true, dbSetting)} AS T SET {GetSetClause(fields.Where(f => !Contains(qualifiers, f)), dbSetting)} " +
            $"FROM {pseudoTableName.AsQuoted(true, dbSetting)} AS S WHERE {GetOnClause(qualifiers, dbSetting)};";

        #endregion

        #region Delete

        /// <summary>
        /// Gets the name of the pseudo table for the delete operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        public static string GetPseudoTableNameForDelete(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) =>
            GetPseudoTableName(tableName, pseudoTableType, "Delete", dbSetting);

        /// <summary>
        /// Gets the name of the pseudo table for the delete-by-key operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableType">The type of the pseudo table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The name of the pseudo table.</returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            DuckDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) =>
            GetPseudoTableName(tableName, pseudoTableType, "DeleteByKey", dbSetting);

        /// <summary>
        /// Gets the statement that deletes the table rows that match the pseudo table rows.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="pseudoTableName">The name of the pseudo table.</param>
        /// <param name="qualifiers">The qualifier fields to be used.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The SQL statement.</returns>
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting) =>
            $"DELETE FROM {tableName.AsQuoted(true, dbSetting)} AS T " +
            $"WHERE EXISTS (SELECT 1 FROM {pseudoTableName.AsQuoted(true, dbSetting)} AS S WHERE {GetOnClause(qualifiers, dbSetting)});";

        #endregion
    }
}
