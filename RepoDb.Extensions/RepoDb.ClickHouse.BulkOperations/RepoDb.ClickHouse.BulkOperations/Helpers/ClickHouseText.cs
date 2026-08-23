using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Builds every SQL text used by the ClickHouse bulk-operations pseudo-table pipeline. This class was
    /// originally copied from the MySQL provider (a MySQL-specific pseudo-table pipeline built on
    /// <c>AUTO_INCREMENT</c>, session user variables, and multi-table <c>UPDATE ... INNER JOIN</c> /
    /// <c>DELETE ... INNER JOIN</c> syntax) and has been rewritten here for ClickHouse, which has none of
    /// those:
    /// <list type="bullet">
    /// <item><description>No identity/auto-increment/sequence of any kind (see
    /// <c>RepoDb.DbHelpers.ClickHouseDbHelper.GetScopeIdentity</c> in <c>RepoDb.ClickHouse</c>) - so unlike
    /// the MySQL and Oracle providers this pipeline never pre-assigns or returns identity values, and the
    /// pseudo table needs no surrogate row-order column to support that (it was only ever read back in
    /// original bulk-load order for the "return identity" paths).</description></item>
    /// <item><description>No multi-table <c>UPDATE ... JOIN ... SET</c> or <c>DELETE ... JOIN</c> - only
    /// <c>ALTER TABLE table UPDATE col = expr [, ...] WHERE filter</c> and
    /// <c>ALTER TABLE table DELETE WHERE filter</c>, asynchronous "mutations" applied by background merges
    /// rather than immediately, and with no synchronously-reliable affected-row count. Cross-referencing the
    /// pseudo table from a mutation's <c>SET</c> expression therefore uses a scalar subquery correlated back
    /// to the row being mutated (via its own, unqualified column names) rather than a join alias - see
    /// <see cref="GetUpdateFromPseudoTableSql"/>.</description></item>
    /// <item><description>No <c>MERGE</c> statement, and (per the above) no multi-table <c>UPDATE</c> either -
    /// a merge is still split, as with the MySQL provider, into a matched-rows <c>ALTER TABLE ... UPDATE</c>
    /// plus an unmatched-rows <c>INSERT ... SELECT</c> guarded by a <c>LEFT JOIN ... WHERE ... IS NULL</c>
    /// anti-join (a plain <c>SELECT</c> with a <c>JOIN</c>, which ClickHouse supports natively and
    /// synchronously) - see <see cref="GetUpdateFromPseudoTableSql"/> and
    /// <see cref="GetInsertUnmatchedFromPseudoTableSql"/>.</description></item>
    /// <item><description>No <c>CREATE TABLE ... AUTO_INCREMENT PRIMARY KEY</c> staging trick - the pseudo
    /// table is instead created with an explicit <c>ENGINE</c> clause on the same <c>CREATE TABLE ... AS
    /// SELECT ... WHERE (1 = 0)</c> shape (still copying column names/types without needing to enumerate
    /// them), backed by ClickHouse's own <c>Memory</c> engine (the direct equivalent of a MySQL
    /// <c>TEMPORARY</c> table - in-memory, unindexed, non-replicated) for
    /// <see cref="ClickHouseBulkImportPseudoTableType.Memory"/>, or a plain, unsorted <c>MergeTree</c>
    /// (<c>ORDER BY tuple()</c> - no sort key needed since this pipeline only ever scans the pseudo table in
    /// full, never point-looks-up into it) for <see cref="ClickHouseBulkImportPseudoTableType.Physical"/>. See
    /// <see cref="GetCreatePseudoTableSql"/>.</description></item>
    /// <item><description>No secondary/qualifier index equivalent to a plain <c>CREATE INDEX</c> - ClickHouse's
    /// MergeTree "data-skipping indices" (<c>minmax</c>/<c>set</c>/<c>bloom_filter</c>, requiring
    /// <c>ALTER TABLE ... ADD INDEX</c> + <c>MATERIALIZE INDEX</c>) are a different mechanism aimed at pruning
    /// whole granules/parts during a scan, not a B-tree-style row lookup - and are not useful here since every
    /// query against the pseudo table is already a full scan (a correlated subquery or a join), never a point
    /// lookup. The MySQL provider's <c>MySqlExecution.CreatePseudoTableIndex</c> step therefore has no
    /// ClickHouse counterpart and was dropped rather than ported.</description></item>
    /// <item><description>No multi-statement execution - <c>RepoDb.DbSettings.ClickHouseDbSetting.IsMultiStatementExecutable</c>
    /// is <see langword="false"/> (ClickHouse.Driver, unlike <c>MySql.Data</c> with
    /// <c>AllowUserVariables=True</c>, does not support batching several statements into one
    /// <c>ExecuteNonQuery</c> call). Every method below therefore returns exactly one statement - the MySQL
    /// provider's semicolon-joined multi-statement strings (e.g. its combined
    /// <c>UPDATE ... JOIN; INSERT ... SELECT;</c> merge text) are split into separately-issued statements at
    /// the <c>ClickHouseExecution</c> call sites instead.</description></item>
    /// </list>
    /// <para>
    /// Because <c>ALTER TABLE ... UPDATE</c>/<c>DELETE</c> mutations are asynchronous (registered
    /// immediately, applied by a background merge afterward) and ClickHouse.Driver's <c>ExecuteNonQuery</c>
    /// has no reliable "rows affected" figure for them, none of the update/delete/merge SQL built here is
    /// expected to report a meaningful affected-row count - see the remarks on
    /// <c>ClickHouseExecution.UpdateFromPseudoTable</c> for how the callers in <c>Base/*.cs</c> compensate
    /// (by reporting the staged/submitted row count instead).
    /// </para>
    /// <para>
    /// The correlated scalar subquery pattern used by <see cref="GetUpdateFromPseudoTableSql"/> (a subquery in
    /// a mutation's <c>SET</c> expression that references the outer, mutated row's own columns in its
    /// <c>WHERE</c> clause) is a commonly-documented idiom for "update a ClickHouse table from another table",
    /// but ClickHouse's support for per-row-correlated mutation subqueries has evolved across versions -
    /// verify against the target ClickHouse server version before relying on it in production.
    /// </para>
    /// </summary>
    internal static class ClickHouseText
    {
        #region Shared

        /// <summary>
        /// Builds the DDL that creates a pseudo (staging) table shaped after <paramref name="tableName"/> -
        /// either every column (<paramref name="qualifierField"/> is <see langword="null"/>) or just the one
        /// qualifier column (used by <c>BulkDeleteByKey</c>, which only ever needs the key column staged).
        /// <c>CREATE TABLE ... ENGINE = ... AS SELECT ... WHERE (1 = 0)</c> copies column names/types from
        /// <paramref name="tableName"/> without needing to enumerate them - callers must issue a separate
        /// <see cref="GetDropPseudoTableSql"/> first (this pipeline always starts from a guaranteed-fresh
        /// pseudo table; see <c>ClickHouseExecution.CreatePseudoTable</c>), since ClickHouse's DDL has no
        /// <c>CREATE TABLE IF NOT EXISTS ... AS SELECT</c> "replace" form and this class emits exactly one
        /// statement per call (see the type-level remarks on why).
        /// <see cref="ClickHouseBulkImportPseudoTableType.Memory"/> backs the pseudo table with ClickHouse's
        /// <c>Memory</c> engine (in-memory, unindexed - the direct equivalent of a MySQL <c>TEMPORARY</c>
        /// table); anything else (<see cref="ClickHouseBulkImportPseudoTableType.Physical"/>, or an unresolved
        /// <see cref="ClickHouseBulkImportPseudoTableType.Auto"/>) backs it with a plain, unsorted
        /// <c>MergeTree</c> table instead. In practice every call currently resolves to <c>Physical</c> before
        /// it reaches here (see the remarks on <c>ResolvePseudoTableType</c> in <c>Base/WriteToServer.cs</c>),
        /// so the <c>Memory</c> branch is written correctly but not yet exercised.
        /// </summary>
        /// <param name="tableName">The real table the pseudo table is staged for.</param>
        /// <param name="pseudoTableName">The name to create the pseudo table under.</param>
        /// <param name="pseudoTableType">Whether the pseudo table should be an in-memory or on-disk staging table.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <param name="qualifierField">
        /// When provided, only this single column is staged (e.g. <c>BulkDeleteByKey</c>); otherwise every
        /// column of <paramref name="tableName"/> is staged.
        /// </param>
        /// <returns>The <c>CREATE TABLE ... AS SELECT</c> SQL text.</returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var engine = pseudoTableType == ClickHouseBulkImportPseudoTableType.Memory
                ? "Memory"
                : "MergeTree ORDER BY tuple()";

            return $"CREATE TABLE {quotedPseudoTableName} ENGINE = {engine} AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)";
        }

        /// <summary>
        /// Builds the DDL that empties a pseudo table before it's bulk-loaded. <see cref="GetCreatePseudoTableSql"/>
        /// already hands back a freshly created (therefore already-empty) table, so this is purely defensive
        /// belt-and-suspenders at the call sites that run both back to back - mirroring the MySQL provider.
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to truncate.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>TRUNCATE TABLE</c> SQL text.</returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// Builds the DDL that drops a pseudo table, either to guarantee a fresh start before it's (re)created
        /// or to clean up once a bulk operation is done with it. ClickHouse's <c>DROP TABLE IF EXISTS</c> is a
        /// no-op (not an error) when the table is already gone, and works for both <c>Memory</c> and
        /// <c>MergeTree</c> tables alike.
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to drop.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>DROP TABLE</c> SQL text.</returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// Builds the <c>(qualifier1, qualifier2, ...) IN (SELECT qualifier1, qualifier2, ... FROM pseudo)</c>
        /// (or the unparenthesized single-column form) clause shared by <see cref="GetUpdateFromPseudoTableSql"/>
        /// and <see cref="GetDeleteFromPseudoTableSql"/> to select every row of the real table that has a
        /// matching row in the pseudo table.
        /// </summary>
        private static string GetTupleInPseudoTableClause(IList<Field> qualifiers,
            string quotedPseudoTableName,
            IDbSetting dbSetting)
        {
            var columnList = qualifiers
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");
            var tuple = qualifiers.Count > 1 ? $"({columnList})" : columnList;

            return $"{tuple} IN (SELECT {columnList} FROM {quotedPseudoTableName})";
        }

        #endregion

        #region Merge

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkMerge</c> path.
        /// </summary>
        /// <param name="tableName">The real table being merged into.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Merge";

        #endregion

        #region Update

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkUpdate</c> path.
        /// </summary>
        /// <param name="tableName">The real table being updated.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForUpdate(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Update";

        /// <summary>
        /// Builds the SQL that updates every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>, via ClickHouse's <c>ALTER TABLE ... UPDATE ... WHERE</c>
        /// mutation syntax - the closest ClickHouse equivalent to the MySQL provider's multi-table
        /// <c>UPDATE ... INNER JOIN</c>. Because a mutation has no join-alias for a second table, each updated
        /// column is instead assigned from a scalar subquery correlated back to the row being mutated (via its
        /// own, unqualified column names - see the type-level remarks on this class for the version-dependent
        /// caveat). This same statement shape backs both a plain <c>BulkUpdate</c> and the matched-rows half of
        /// a <c>BulkMerge</c> (there is no "WHEN NOT MATCHED" branch to build here; that half is
        /// <see cref="GetInsertUnmatchedFromPseudoTableSql"/> instead).
        /// </summary>
        /// <param name="tableName">The real table to update.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being updated/merged.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>ALTER TABLE ... UPDATE ... WHERE</c> SQL text.</returns>
        public static string GetUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var qualifierList = qualifiers.AsList();
            var updatableFields = fields
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var correlation = qualifierList
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)} = {f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var setClause = updatableFields
                .Select(f =>
                {
                    var quotedField = f.Name.AsQuoted(true, dbSetting);
                    return $"{quotedField} = (SELECT S.{quotedField} FROM {quotedPseudoTableName} S WHERE {correlation} LIMIT 1)";
                })
                .Join(", ");

            var whereClause = GetTupleInPseudoTableClause(qualifierList, quotedPseudoTableName, dbSetting);

            return $"ALTER TABLE {quotedTableName} UPDATE {setClause} WHERE {whereClause}";
        }

        #endregion

        #region Insert (unmatched rows of a merge)

        /// <summary>
        /// Builds the SQL that inserts every row of <paramref name="pseudoTableName"/> that has <i>no</i>
        /// matching row in <paramref name="tableName"/> (matched on <paramref name="qualifiers"/>) - the
        /// unmatched/"WHEN NOT MATCHED THEN INSERT" half of a <c>BulkMerge</c>, via a plain
        /// <c>LEFT JOIN ... WHERE ... IS NULL</c> anti-join. Unlike <see cref="GetUpdateFromPseudoTableSql"/>,
        /// this is an ordinary <c>SELECT</c> with a <c>JOIN</c> feeding an <c>INSERT</c> - fully synchronous,
        /// native ClickHouse SQL with no mutation involved. The anti-join assumes the first qualifier column is
        /// never legitimately <see langword="null"/> on a real, matched row (true for the typical case of
        /// qualifying on a primary/unique key).
        /// </summary>
        /// <param name="tableName">The real table to insert into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being merged.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>INSERT ... SELECT ... LEFT JOIN ... WHERE ... IS NULL</c> SQL text.</returns>
        public static string GetInsertUnmatchedFromPseudoTableSql(string tableName,
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
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var insertColumns = fieldList
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var insertValues = fieldList
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var firstQualifierColumn = qualifierList.First().Name.AsQuoted(true, dbSetting);

            return string.Concat(
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertValues, " FROM ", quotedPseudoTableName, " S ",
                "LEFT JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "WHERE T.", firstQualifierColumn, " IS NULL");
        }

        #endregion

        #region Delete

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkDelete</c> path.
        /// </summary>
        /// <param name="tableName">The real table being deleted from.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForDelete(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkDeleteByKey</c> path. Suffixed
        /// <c>DeleteByKey</c> rather than plain <c>Delete</c> so it never collides with
        /// <see cref="GetPseudoTableNameForDelete"/> for the same <paramref name="tableName"/>/<paramref name="pseudoTableType"/> -
        /// <c>BulkDeleteByKey</c> stages only the qualifier column, while <c>BulkDelete</c> stages every
        /// column.
        /// </summary>
        /// <param name="tableName">The real table being deleted from.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

        /// <summary>
        /// Builds the SQL that counts every row of <paramref name="tableName"/> that has a matching row in
        /// <paramref name="pseudoTableName"/> (matched on <paramref name="qualifiers"/>) - used by
        /// <c>ClickHouseExecution.DeleteFromPseudoTable</c>/<c>ClickHouseExecution.DeleteFromPseudoTableAsync</c>
        /// to capture how many rows will be deleted <i>before</i> issuing the asynchronous
        /// <c>ALTER TABLE ... DELETE WHERE</c> mutation built by <see cref="GetDeleteFromPseudoTableSql"/>
        /// (counting afterward would always see 0, since the matching rows would already be gone by the time
        /// the mutation - which applies asynchronously - actually finishes). Shares the same
        /// <see cref="GetTupleInPseudoTableClause"/> matching predicate as <see cref="GetDeleteFromPseudoTableSql"/>
        /// so the count and the delete are guaranteed to agree on which rows match (modulo a concurrent write
        /// between the two - see the caller's remarks).
        /// </summary>
        /// <param name="tableName">The real table to count matching rows in.</param>
        /// <param name="pseudoTableName">The pseudo table holding the key values (or full rows) to match on.</param>
        /// <param name="qualifiers">The columns used to match a row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>SELECT count(*) ... WHERE ... IN (...)</c> SQL text.</returns>
        public static string GetCountMatchedByPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var whereClause = GetTupleInPseudoTableClause(qualifiers.AsList(), quotedPseudoTableName, dbSetting);

            return $"SELECT count(*) FROM {quotedTableName} WHERE {whereClause}";
        }

        /// <summary>
        /// Builds the SQL that deletes every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>, via ClickHouse's <c>ALTER TABLE ... DELETE WHERE</c> mutation
        /// syntax - much simpler than <see cref="GetUpdateFromPseudoTableSql"/>'s correlated-subquery dance,
        /// since a delete needs no per-row value lookup, only a membership test.
        /// </summary>
        /// <param name="tableName">The real table to delete from.</param>
        /// <param name="pseudoTableName">The pseudo table holding the key values (or full rows) to match on.</param>
        /// <param name="qualifiers">The columns used to match a row to delete.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>ALTER TABLE ... DELETE WHERE</c> SQL text.</returns>
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var whereClause = GetTupleInPseudoTableClause(qualifiers.AsList(), quotedPseudoTableName, dbSetting);

            return $"ALTER TABLE {quotedTableName} DELETE WHERE {whereClause}";
        }

        #endregion
    }
}
