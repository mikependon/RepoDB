#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    internal static class EDBText
    {
        /// <summary>
        ///
        /// </summary>
        private const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        /// <summary>
        ///
        /// </summary>
        private const string QualifierIndexName = "__RepoDbBulkQualifierIndex__";

        #region Shared

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table the pseudo table is staged for.</param>
        /// <param name="pseudoTableName">The name to create the pseudo table under.</param>
        /// <param name="pseudoTableType">Whether the pseudo table should be a persistent or temporary table.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <param name="qualifierField">
        /// When provided, only this single column is staged (e.g. <c>BulkDeleteByKey</c>); otherwise every
        /// column of <paramref name="tableName"/> is staged.
        /// </param>
        /// <returns>The <c>DROP TABLE</c> + <c>CREATE TABLE ... AS SELECT</c> + <c>ALTER TABLE ... ADD COLUMN</c> SQL text.</returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var tempKeyword = pseudoTableType == EDBBulkImportPseudoTableType.Physical
                ? string.Empty
                : "TEMP ";

            return $"DROP TABLE IF EXISTS {quotedPseudoTableName}; " +
                $"CREATE {tempKeyword}TABLE {quotedPseudoTableName} " +
                $"AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0); " +
                $"ALTER TABLE {quotedPseudoTableName} ADD COLUMN {quotedRowOrderColumn} BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to index.</param>
        /// <param name="qualifiers">The qualifier column(s) to index.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>CREATE INDEX</c> SQL text.</returns>
        public static string GetCreatePseudoTableIndexSql(string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIndexName = QualifierIndexName.AsQuoted(true, dbSetting);
            var columnList = qualifiers
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return $"CREATE INDEX {quotedIndexName} ON {quotedPseudoTableName} ({columnList})";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to truncate.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>TRUNCATE TABLE</c> SQL text.</returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)} RESTART IDENTITY";

        /// <summary>
        ///
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to drop.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>DROP TABLE</c> SQL text.</returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {pseudoTableName.AsQuoted(true, dbSetting)}";

        #endregion

        #region Insert

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table being inserted into.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForInsert(string tableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Insert";

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// A single <c>INSERT ... SELECT ... ORDER BY ... RETURNING</c> statement - Postgres/EDB generates the
        /// identity value for each inserted row itself (from the destination table's own identity/sequence
        /// column) and <c>RETURNING</c> hands every one of them straight back, in the same statement, in the
        /// order the rows were fed to it via <c>ORDER BY</c> <see cref="RowOrderColumnName"/>. This replaces
        /// both MariaDB's and Oracle's multi-step "pre-assign the identity into the pseudo table before the
        /// real <c>INSERT</c>" techniques entirely - there is no sequence/counter to look up in advance here,
        /// so unlike those two providers this needs no <c>sequenceName</c>/<c>isAlwaysGenerated</c> metadata
        /// step at all (see <c>EDBExecution.InsertFromPseudoTableForReturnIdentity</c>, which no longer looks
        /// any up before calling this).
        /// </remarks>
        /// <param name="tableName">The real table to insert into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">The columns to copy from the pseudo table into the real table.</param>
        /// <param name="identityField">The identity column being returned.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The single <c>INSERT ... RETURNING</c> SQL text; yields one row per inserted entity, in original bulk-load order.</returns>
        public static string GetInsertFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);

            var columnList = fields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return string.Concat(
                "INSERT INTO ", quotedTableName, " (", columnList, ") ",
                "SELECT ", columnList, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, " ",
                "RETURNING ", quotedIdentityColumn, " AS ", resultAlias, ";");
        }

        #endregion

        #region Merge

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table being merged into.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Merge";

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// A single <c>INSERT ... SELECT ... ON CONFLICT (qualifiers) DO UPDATE SET ...</c> statement -
        /// Postgres/EDB's native upsert. Unlike MariaDB's <c>ON DUPLICATE KEY UPDATE</c> (which this class's
        /// MariaDB-era version could not use here either, for the identical reason), <c>ON CONFLICT</c> only
        /// fires against an actual unique/primary key constraint on <paramref name="qualifiers"/> - the same
        /// requirement the return-identity variant below has. When there is nothing left to update once the
        /// qualifier columns are excluded (a merge keyed on every column), the statement degrades to
        /// <c>ON CONFLICT (qualifiers) DO NOTHING</c>, which still correctly inserts unmatched rows.
        /// </remarks>
        /// <param name="tableName">The real table to merge into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being merged.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="identityField">Excluded from the <c>INSERT</c> column list, if present, so the real table's own identity column generates it for new rows.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The single <c>INSERT ... ON CONFLICT</c> SQL text.</returns>
        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var insertableFields = fieldList
                .Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase))
                .AsList();

            var insertColumns = insertableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var conflictColumns = qualifierList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var updateableFields = insertableFields
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var conflictAction = updateableFields.Count > 0
                ? string.Concat("DO UPDATE SET ", updateableFields
                    .Select(f => string.Concat(f.Name.AsQuoted(true, dbSetting), " = EXCLUDED.", f.Name.AsQuoted(true, dbSetting)))
                    .Join(", "))
                : "DO NOTHING";

            return string.Concat(
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertColumns, " FROM ", quotedPseudoTableName, " ",
                "ON CONFLICT (", conflictColumns, ") ", conflictAction, ";");
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Same <c>INSERT ... ON CONFLICT DO UPDATE</c> statement as <see cref="GetMergeFromPseudoTableSql"/>,
        /// with a <c>RETURNING</c> clause added - Postgres/EDB reports the identity column's value for *every*
        /// affected row, whichever branch (insert or update) it took, in one round trip. This replaces both
        /// MariaDB's five-statement pre-assign-then-report technique and its own remark about needing to
        /// combine two different techniques - there is only the one statement here. Just like
        /// <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>, the source <c>SELECT</c> is ordered by
        /// <see cref="RowOrderColumnName"/> so <c>RETURNING</c> emits rows in the same order the pseudo table
        /// was bulk-loaded in - a single, non-parallel <c>INSERT ... SELECT ... RETURNING</c> processes and
        /// emits its source rows one at a time, in the scan order of that ordered <c>SELECT</c>, whether a
        /// given row lands in the insert or the update branch of <c>ON CONFLICT</c>.
        /// </remarks>
        /// <param name="tableName">The real table to merge into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being merged.</param>
        /// <param name="identityField">The identity column being returned.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The single <c>INSERT ... ON CONFLICT ... RETURNING</c> SQL text; yields one row per merged entity, in original bulk-load order.</returns>
        public static string GetMergeFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);

            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var insertColumns = fieldList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var conflictColumns = qualifierList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var updateableFields = fieldList
                .Where(f => !string.Equals(f.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase) &&
                    qualifierList.Any(q => string.Equals(q.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var firstConflictColumn = qualifierList.First().Name.AsQuoted(true, dbSetting);
            var conflictAction = updateableFields.Count > 0
                ? string.Concat("DO UPDATE SET ", updateableFields
                    .Select(f => string.Concat(f.Name.AsQuoted(true, dbSetting), " = EXCLUDED.", f.Name.AsQuoted(true, dbSetting)))
                    .Join(", "))
                : string.Concat("DO UPDATE SET ", firstConflictColumn, " = ", quotedTableName, ".", firstConflictColumn);

            return string.Concat(
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertColumns, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, " ",
                "ON CONFLICT (", conflictColumns, ") ", conflictAction, " ",
                "RETURNING ", quotedIdentityColumn, " AS ", resultAlias, ";");
        }

        #endregion

        #region Update

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table being updated.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForUpdate(string tableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Update";

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table to update.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being updated.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>UPDATE ... FROM</c> SQL text.</returns>
        public static string GetUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var onClause = qualifierList
                .Select(f => $"{quotedTableName}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateClause = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)) == false)
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return $"UPDATE {quotedTableName} SET {updateClause} FROM {quotedPseudoTableName} S WHERE ({onClause})";
        }

        #endregion

        #region Delete

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table being deleted from.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForDelete(string tableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table being deleted from.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            EDBBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName">The real table to delete from.</param>
        /// <param name="pseudoTableName">The pseudo table holding the key values (or full rows) to match on.</param>
        /// <param name="qualifiers">The columns used to match a row to delete.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>DELETE ... USING</c> SQL text.</returns>
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var onClause = qualifiers
                .Select(f => $"{quotedTableName}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            return $"DELETE FROM {quotedTableName} USING {quotedPseudoTableName} S WHERE ({onClause})";
        }

        #endregion
    }
}
