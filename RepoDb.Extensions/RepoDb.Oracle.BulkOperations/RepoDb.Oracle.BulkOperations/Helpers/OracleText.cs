using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// A light-weight, allocation-cheap builder of the raw SQL text used by the Oracle bulk operations
    /// (currently <c>BulkMerge</c> and <c>BulkUpdate</c>). Every method here is a pure string builder -
    /// no I/O, no caching - callers (<see cref="RepoDb.Oracle.BulkOperations.Extensions.OracleExecution"/>)
    /// own execution.
    /// </summary>
    internal static class OracleText
    {
        #region Shared

        /// <summary>
        /// Builds a guarded (idempotent) <c>CREATE TABLE</c> statement for the staging/pseudo table used
        /// by a bulk operation. Guarded with a PL/SQL block that swallows ORA-00955 ("name is already
        /// used by an existing object") so repeated calls against the same table - common since the
        /// pseudo table name is deterministic per (tableName, pseudoTableType) pair, not a fresh GUID
        /// per call - do not fail. The staging table always starts out structurally identical to, and
        /// empty relative to, the target table (<c>WHERE (1 = 0)</c> copies columns/types, not rows).
        /// </summary>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            // Physical: an ordinary heap table, shared/visible across sessions.
            // Memory: a Global Temporary Table whose rows are private per session but whose definition
            // (created once) is shared - safe for concurrent connections to reuse. Auto resolves to either
            // of these (see OracleConnectionExtension.ResolvePseudoTableType) before this method ever sees it.
            var createClause = pseudoTableType == OracleBulkImportPseudoTableType.Physical
                ? $"CREATE TABLE {quotedPseudoTableName} AS SELECT * FROM {quotedTableName} WHERE (1 = 0)"
                : $"CREATE GLOBAL TEMPORARY TABLE {quotedPseudoTableName} ON COMMIT PRESERVE ROWS AS SELECT * FROM {quotedTableName} WHERE (1 = 0)";

            // ORA-00955: name is already used by an existing object
            return $"BEGIN EXECUTE IMMEDIATE '{createClause}'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -955 THEN RAISE; END IF; END;";
        }

        /// <summary>
        /// Builds a <c>TRUNCATE TABLE</c> statement for the staging/pseudo table. Always run right before
        /// writing to the staging table (whether it was just created or is being reused from a prior call
        /// on the same session/connection) so leftover rows from an earlier bulk operation - possible for
        /// the <c>Memory</c> pseudo table type, whose rows are preserved across commits within the same
        /// session - never leak into the current merge.
        /// </summary>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// Builds a guarded (idempotent) <c>DROP TABLE</c> statement for the staging/pseudo table. Run
        /// once a bulk operation is done with it, for maximum cleanup - unlike <see cref="GetTruncatePseudoTableSql"/>,
        /// this removes the table definition itself (not just its rows), so the next call against the
        /// same table starts from a clean <see cref="GetCreatePseudoTableSql"/> again. Guarded with a
        /// PL/SQL block that swallows ORA-00942 ("table or view does not exist") so this is safe to call
        /// even if the table was already dropped (e.g. by a concurrent session sharing the same
        /// deterministic pseudo table name).
        /// </summary>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            // ORA-00942: table or view does not exist
            return $"BEGIN EXECUTE IMMEDIATE 'DROP TABLE {quotedPseudoTableName}'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -942 THEN RAISE; END IF; END;";
        }

        #endregion

        #region Merge

        /// <summary>
        /// Returns the deterministic name of the staging/pseudo table for a <c>BulkMerge</c> against
        /// <paramref name="tableName"/>. Deterministic (not a fresh GUID per call) so the same staging
        /// table definition can be created once and reused (after a <c>TRUNCATE</c>) by later calls.
        /// </summary>
        public static string GetPseudoTableNameForMerge(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{tableName}Merge";

        /// <summary>
        /// Builds the <c>MERGE INTO ... USING ... ON (...) WHEN MATCHED ... WHEN NOT MATCHED ...</c>
        /// statement that upserts every row currently staged in <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/>. Note: Oracle's <c>MERGE</c> syntax does not accept the <c>AS</c>
        /// keyword before a table/subquery alias (unlike most other clauses) - the bare <c>T</c>/<c>S</c>
        /// aliases below are intentional, not an oversight.
        /// </summary>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be merged (inserted and/or updated).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var onClause = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateableFields = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var insertColumns = fieldList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var insertValues = fieldList
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            // A MERGE with nothing but qualifier fields has no columns left to update on a match - Oracle
            // rejects an empty "UPDATE SET" list, so the whole WHEN MATCHED branch is omitted for that
            // (unusual, qualifiers-cover-every-column) case rather than emitting invalid SQL.
            var whenMatchedClause = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateableFields.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ")} "
                : string.Empty;

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) {whenMatchedClause}WHEN NOT MATCHED THEN INSERT ({insertColumns}) VALUES ({insertValues})";
        }

        #endregion

        #region Update

        /// <summary>
        /// Returns the deterministic name of the staging/pseudo table for a <c>BulkUpdate</c> against
        /// <paramref name="tableName"/>. Suffixed differently than <see cref="GetPseudoTableNameForMerge"/>
        /// so a <c>BulkUpdate</c> and a <c>BulkMerge</c> against the same table never share (and clobber)
        /// one staging table.
        /// </summary>
        public static string GetPseudoTableNameForUpdate(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{tableName}Update";

        /// <summary>
        /// Builds the <c>MERGE INTO ... USING ... ON (...) WHEN MATCHED THEN UPDATE ...</c> statement that
        /// updates every row on <paramref name="tableName"/> matched by a row currently staged in
        /// <paramref name="pseudoTableName"/>. Unlike <see cref="GetMergeFromPseudoTableSql"/>, there is no
        /// <c>WHEN NOT MATCHED</c> branch - a <c>BulkUpdate</c> only ever touches rows that already exist;
        /// staged rows with no matching target row are silently left as-is (not inserted).
        /// </summary>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged (the qualifier(s) plus every field to update).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        public static string GetUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var onClause = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateClause = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) WHEN MATCHED THEN UPDATE SET {updateClause}";
        }

        #endregion

        #region Delete

        /// <summary>
        /// Returns the deterministic name of the staging/pseudo table for a <c>BulkDelete</c> against
        /// <paramref name="tableName"/>. Suffixed differently than <see cref="GetPseudoTableNameForMerge"/>
        /// and <see cref="GetPseudoTableNameForUpdate"/> so a <c>BulkDelete</c> never shares (and clobbers)
        /// the staging table of a concurrent <c>BulkMerge</c>/<c>BulkUpdate</c> against the same table.
        /// </summary>
        public static string GetPseudoTableNameForDelete(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{tableName}Delete";

        /// <summary>
        /// Builds the <c>DELETE FROM ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c> statement that
        /// removes every row on <paramref name="tableName"/> matched by a row currently staged in
        /// <paramref name="pseudoTableName"/>. Oracle's <c>DELETE</c> statement cannot directly target a
        /// joined result the way e.g. SQL Server's <c>DELETE ... FROM ... INNER JOIN ...</c> can - the only
        /// join-based <c>DELETE</c> form Oracle offers, <c>DELETE FROM (SELECT * FROM t1 JOIN t2 ...)</c>
        /// (an "updatable/deletable join view"), only works when the joined-against table is <em>key-preserved</em>
        /// (i.e. backed by a real primary/unique key or index) - which the staging/pseudo table never is,
        /// since it is created without constraints (see <see cref="GetCreatePseudoTableSql"/>) - and would fail
        /// with <c>ORA-01779</c> at runtime. A <c>ROWID IN (SELECT T.ROWID FROM ... T INNER JOIN ... S ON (...))</c>
        /// subquery sidesteps that restriction entirely (plain <c>SELECT</c>s have no key-preservation
        /// requirement) while still literally performing the match as an <c>INNER JOIN</c>, and - since
        /// <c>ROWID</c> uniquely identifies a physical row - is safe even if a staged row matches more than
        /// one target row on the qualifier field(s).
        /// </summary>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="qualifiers">The field(s) used to match an existing row for deletion.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var onClause = qualifiers
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            return $"DELETE FROM {quotedTableName} WHERE ROWID IN (SELECT T.ROWID FROM {quotedTableName} T INNER JOIN {quotedPseudoTableName} S ON ({onClause}))";
        }

        #endregion
    }
}
