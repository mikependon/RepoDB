#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.SapHana;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Builds every SQL text used by the SapHana bulk-operations pseudo-table pipeline.
    /// </summary>
    /// <remarks>
    /// This was mechanically renamed from the SapHana provider (itself adapted from the Oracle
    /// provider) and then re-derived for SAP HANA, which differs from both in several ways:
    /// <list type="bullet">
    /// <item><description>No session user variables (MySQL's <c>SET :var := ...</c>) - every identity
    /// pre-assignment here is a single declarative <c>UPDATE</c> using the pseudo table's own
    /// <see cref="RowOrderColumnName"/> identity column plus arithmetic against a seed value fetched via
    /// <see cref="GetIdentitySequenceMetadataSql"/>, rather than a running counter.</description></item>
    /// <item><description>HANA has a real, ANSI-shaped <c>MERGE</c> statement, so <c>BulkMerge</c> is a single
    /// <c>MERGE INTO ... USING ... ON (...) WHEN MATCHED THEN UPDATE ... WHEN NOT MATCHED THEN INSERT ...</c>
    /// statement instead of MySQL's two-statement <c>UPDATE ... INNER JOIN</c> + anti-join
    /// <c>INSERT ... SELECT</c> workaround.</description></item>
    /// <item><description>HANA does not support a multi-table <c>UPDATE ... JOIN</c> or <c>DELETE ... JOIN</c>
    /// the way MySQL does. <c>BulkUpdate</c> uses the multi-column
    /// <c>SET (col1, col2) = (SELECT ...) WHERE EXISTS (...)</c> form instead, and <c>BulkDelete</c> uses a
    /// plain <c>DELETE ... WHERE EXISTS (...)</c>.</description></item>
    /// <item><description>This is the single highest-risk file in the whole provider: every statement below
    /// was written from documented SAP HANA SQL syntax, but has not been executed against a live HANA
    /// instance. Verify it there before relying on it.</description></item>
    /// </list>
    /// </remarks>
    internal static class SapHanaText
    {
        /// <summary>
        /// Name of the surrogate, always-present ordering/identity column added to every pseudo table (see
        /// <see cref="GetCreatePseudoTableSql"/>). Used both to read pseudo-table rows back in their original
        /// bulk-load order, and (via simple arithmetic) to pre-assign identity values before an insert/merge -
        /// see <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>.
        /// </summary>
        private const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        /// <summary>
        /// Name of the index created on every pseudo table's qualifier columns (see
        /// <see cref="GetCreatePseudoTableIndexSql"/>).
        /// </summary>
        private const string QualifierIndexName = "__RepoDbBulkQualifierIndex__";

        #region Shared

        /// <summary>
        /// Builds the DDL that (re-)creates a pseudo (staging) table shaped after <paramref name="tableName"/>.
        /// HANA's <c>CREATE TABLE ... AS (subquery)</c> cannot itself declare an extra column alongside the
        /// ones copied from the subquery, so the surrogate <see cref="RowOrderColumnName"/> identity column is
        /// added afterward via a separate <c>ALTER TABLE ... ADD</c>.
        /// </summary>
        /// <param name="tableName">The real table the pseudo table is staged for.</param>
        /// <param name="pseudoTableName">The name to create the pseudo table under.</param>
        /// <param name="pseudoTableType">Whether the pseudo table should be a persistent or (local temporary) table.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <param name="qualifierField">
        /// When provided, only this single column is staged (e.g. <c>BulkDeleteByKey</c>); otherwise every
        /// column of <paramref name="tableName"/> is staged.
        /// </param>
        /// <returns>The <c>DROP TABLE</c> + <c>CREATE TABLE</c> + <c>ALTER TABLE</c> SQL text.</returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var temporaryKeyword = pseudoTableType == SapHanaBulkImportPseudoTableType.Physical
                ? string.Empty
                : "LOCAL TEMPORARY ";

            return string.Concat(
                "DROP TABLE IF EXISTS ", quotedPseudoTableName, "; ",
                "CREATE ", temporaryKeyword, "TABLE ", quotedPseudoTableName, " AS ",
                "(SELECT ", columnList, " FROM ", quotedTableName, " WHERE (1 = 0)); ",
                "ALTER TABLE ", quotedPseudoTableName, " ADD (", quotedRowOrderColumn, " BIGINT GENERATED BY DEFAULT AS IDENTITY);");
        }

        /// <summary>
        /// Builds the DDL that creates an index on a pseudo table's qualifier columns. Must run right after
        /// <see cref="GetCreatePseudoTableSql"/> and before the client bulk-loads data into the pseudo table.
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to index.</param>
        /// <param name="qualifiers">The columns the final merge/update/delete will match against.</param>
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
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// Builds the DDL that drops a pseudo table once a bulk operation is done with it.
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
        /// Builds the deterministic pseudo table name used by the <c>BulkInsert</c> "return identity" path.
        /// </summary>
        public static string GetPseudoTableNameForInsert(string tableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Insert";

        /// <summary>
        /// No-op for SAP HANA. Unlike MySQL's <c>MODIFY COLUMN</c>, the identity pre-assignment below never
        /// needs the pseudo table's copy of the real identity column to be made nullable first - it is always
        /// overwritten unconditionally by <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/> and
        /// <see cref="GetMergeFromPseudoTableForReturnIdentitySql"/> regardless of whatever value it was loaded
        /// with. Kept only for call-site signature compatibility.
        /// </summary>
        public static string GetAllowNullForColumnSql(string pseudoTableName,
            string columnName,
            IDbSetting dbSetting) => "SELECT 1 FROM DUMMY";

        /// <summary>
        /// Builds a query that reports the next identity value for <paramref name="tableName"/> (one past the
        /// highest <paramref name="identityField"/> value currently stored), used to seed the identity
        /// pre-assignment arithmetic in <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/> and
        /// <see cref="GetMergeFromPseudoTableForReturnIdentitySql"/>. Reads the live maximum off the table's
        /// own row data (rather than any cached counter) so it can never be stale; this does leave a small race
        /// window against a concurrent writer to the same table, same as the equivalent lookup in the other
        /// pseudo-table-based providers in this codebase.
        /// </summary>
        public static string GetIdentitySequenceMetadataSql(string tableName,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);

            return string.Concat(
                "SELECT CAST(COALESCE(MAX(", quotedIdentityColumn, "), 0) + 1 AS VARCHAR(38)) AS ",
                "SequenceName".AsQuoted(dbSetting), ", ",
                "'NO' AS ", "GenerationType".AsQuoted(dbSetting), " FROM ", quotedTableName);
        }

        /// <summary>
        /// Builds the multi-statement SQL that moves every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/> and reports back the identity value assigned to each one, in the same
        /// order the rows were originally bulk-loaded in.
        /// </summary>
        /// <remarks>
        /// The pseudo table's <see cref="RowOrderColumnName"/> is itself a gap-free identity column starting
        /// at 1 (see <see cref="GetCreatePseudoTableSql"/>), so <c>RowOrder + (seed - 1)</c> assigns exactly
        /// <paramref name="sequenceName"/>, <paramref name="sequenceName"/> + 1, ... to the rows in load order -
        /// no session variable or per-row procedural loop is needed. <paramref name="isAlwaysGenerated"/> is
        /// unused here (both HANA identity generation modes accept an explicit value being inserted through
        /// this pseudo-table path); kept for signature compatibility with the other pseudo-table providers.
        /// </remarks>
        public static string GetInsertFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            string sequenceName,
            bool isAlwaysGenerated,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);
            var nextIdentityValue = string.IsNullOrWhiteSpace(sequenceName) ? "1" : sequenceName;

            var columnList = fields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return string.Concat(
                "UPDATE ", quotedPseudoTableName, " SET ", quotedIdentityColumn, " = ", quotedRowOrderColumn, " + (", nextIdentityValue, " - 1); ",
                "INSERT INTO ", quotedTableName, " (", columnList, ") ",
                "SELECT ", columnList, " FROM ", quotedPseudoTableName, "; ",
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, ";");
        }

        #endregion

        #region Merge

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkMerge</c> path.
        /// </summary>
        public static string GetPseudoTableNameForMerge(string tableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Merge";

        /// <summary>
        /// Builds the SQL that merges every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/>, without returning identity values, using a single, real HANA
        /// <c>MERGE</c> statement (no anti-join workaround is needed here, unlike MySQL).
        /// </summary>
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

            var onClause = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateableFields = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var insertableFields = fieldList
                .Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .AsList();

            var insertColumns = insertableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var insertValues = insertableFields
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var updateSetClause = updateableFields
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var whenMatched = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateSetClause} "
                : string.Empty;

            return string.Concat(
                "MERGE INTO ", quotedTableName, " T USING ", quotedPseudoTableName, " S ON (", onClause, ") ",
                whenMatched,
                "WHEN NOT MATCHED THEN INSERT (", insertColumns, ") VALUES (", insertValues, ");");
        }

        /// <summary>
        /// Builds the multi-statement SQL that merges every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/> and reports back each row's identity value - the existing value for a
        /// row that matched (and was updated), or a freshly assigned value for a row that didn't (and was
        /// inserted) - in the same order the rows were originally bulk-loaded in.
        /// </summary>
        /// <remarks>
        /// Step 1 copies the existing identity value onto every pseudo row that already has a match in
        /// <paramref name="tableName"/>. Step 2 assigns a fresh, gap-free identity value (continuing from
        /// <paramref name="sequenceName"/>) to every pseudo row that doesn't, using a correlated
        /// <c>COUNT(*)</c> as a portable stand-in for "this row's rank among the unmatched rows, ordered by
        /// load order" - deliberately plain, portable ANSI SQL (no window functions, no vendor-specific
        /// <c>UPDATE ... FROM</c>) since this is the least-verified statement in the whole provider. Step 3 is
        /// the same <c>MERGE</c> as <see cref="GetMergeFromPseudoTableSql"/>, now with every pseudo row's
        /// identity column already resolved either way.
        /// </remarks>
        public static string GetMergeFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            string sequenceName,
            bool isAlwaysGenerated,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);
            var nextIdentityValue = string.IsNullOrWhiteSpace(sequenceName) ? "1" : sequenceName;

            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var onClauseTS = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");
            var onClauseTP = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = P.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");
            var onClauseTP2 = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = P2.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateableFields = fieldList
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase) &&
                    qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var insertColumns = fieldList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var insertValues = fieldList
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var updateSetClause = updateableFields
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var whenMatched = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateSetClause} "
                : string.Empty;

            return string.Concat(
                // Step 1: matched rows keep their existing identity value.
                "UPDATE ", quotedPseudoTableName, " P SET ", quotedIdentityColumn, " = (",
                "SELECT T.", quotedIdentityColumn, " FROM ", quotedTableName, " T WHERE ", onClauseTP, ") ",
                "WHERE EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClauseTP, "); ",
                // Step 2: unmatched rows get a fresh, pre-assigned identity value, ranked by load order among
                // only the other unmatched rows.
                "UPDATE ", quotedPseudoTableName, " P SET ", quotedIdentityColumn, " = (", nextIdentityValue, " - 1) + (",
                "SELECT COUNT(*) FROM ", quotedPseudoTableName, " P2 ",
                "WHERE P2.", quotedRowOrderColumn, " <= P.", quotedRowOrderColumn, " ",
                "AND NOT EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClauseTP2, ")) ",
                "WHERE NOT EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClauseTP, "); ",
                // Step 3: the actual merge - matched rows update, unmatched rows insert with their assigned id.
                "MERGE INTO ", quotedTableName, " T USING ", quotedPseudoTableName, " S ON (", onClauseTS, ") ",
                whenMatched,
                "WHEN NOT MATCHED THEN INSERT (", insertColumns, ") VALUES (", insertValues, "); ",
                // Step 4: report every row's final identity value, in original bulk-load order.
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, ";");
        }

        #endregion

        #region Update

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkUpdate</c> path.
        /// </summary>
        public static string GetPseudoTableNameForUpdate(string tableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Update";

        /// <summary>
        /// Builds the SQL that updates every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>. HANA has no multi-table <c>UPDATE ... JOIN</c>, so this uses the
        /// multi-column <c>SET (col1, col2, ...) = (subquery)</c> form guarded by a correlated
        /// <c>WHERE EXISTS</c>, instead.
        /// </summary>
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
                .Select(f => $"{tableName.AsQuoted(true, dbSetting)}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateableFields = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var setColumns = updateableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var selectColumns = updateableFields
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return string.Concat(
                "UPDATE ", quotedTableName, " SET (", setColumns, ") = (",
                "SELECT ", selectColumns, " FROM ", quotedPseudoTableName, " S WHERE ", onClause, ") ",
                "WHERE EXISTS (SELECT 1 FROM ", quotedPseudoTableName, " S WHERE ", onClause, ")");
        }

        #endregion

        #region Delete

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkDelete</c> path.
        /// </summary>
        public static string GetPseudoTableNameForDelete(string tableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkDeleteByKey</c> path.
        /// </summary>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            SapHanaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

        /// <summary>
        /// Builds the SQL that deletes every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>, via a correlated <c>WHERE EXISTS</c> - HANA has no multi-table
        /// <c>DELETE ... JOIN</c> the way MySQL does.
        /// </summary>
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var onClause = qualifiers
                .Select(f => $"{tableName.AsQuoted(true, dbSetting)}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            return string.Concat(
                "DELETE FROM ", quotedTableName, " WHERE EXISTS (",
                "SELECT 1 FROM ", quotedPseudoTableName, " S WHERE ", onClause, ")");
        }

        #endregion
    }
}
