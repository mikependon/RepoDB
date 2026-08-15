using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.MySql;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Builds every SQL text used by the MySql bulk-operations pseudo-table pipeline. This class was
    /// originally copied from the Oracle provider (PL/SQL anonymous blocks, sequences, <c>MERGE</c>, <c>ROWID</c>)
    /// and has been rewritten here for plain MySQL, which has none of those:
    /// <list type="bullet">
    /// <item><description>No PL/SQL - "guard block" DDL (<c>CREATE TABLE IF NOT EXISTS</c>, <c>DROP TABLE IF EXISTS</c>)
    /// is native MySQL syntax, so no wrapper is needed at all.</description></item>
    /// <item><description>No <c>MERGE</c> statement - a merge is split into an <c>UPDATE ... INNER JOIN</c> for
    /// matched rows plus an <c>INSERT ... SELECT</c> guarded by a <c>LEFT JOIN ... WHERE ... IS NULL</c> anti-join
    /// for unmatched rows.</description></item>
    /// <item><description>No user-defined sequences - identity values that must be known before <c>INSERT</c>
    /// (the "return identity" paths) are pre-assigned into the pseudo table via a session user variable seeded
    /// from the target table's current <c>AUTO_INCREMENT</c> counter, then copied over as literal values -
    /// MySQL always accepts an explicit value for an <c>AUTO_INCREMENT</c> column, unlike Oracle's
    /// <c>GENERATED ALWAYS</c> identity columns.</description></item>
    /// <item><description>No <c>ROWID</c> - every pseudo table gets an extra, always-present
    /// <c>__RepoDbBulkRowOrder__ BIGINT AUTO_INCREMENT PRIMARY KEY</c> column (see
    /// <see cref="GetCreatePseudoTableSql"/>) purely to give a deterministic row order to bulk-load into and
    /// read back from, standing in for the physical-order guarantee Oracle's <c>ROWID</c> gave for free.</description></item>
    /// <item><description><c>ALTER TABLE ... MODIFY COLUMN ... NULL</c> requires re-stating the column's full
    /// type in MySQL (Oracle lets you toggle nullability alone) - <see cref="GetAllowNullForColumnSql"/> looks
    /// the type up from <c>information_schema.COLUMNS</c> and rebuilds the <c>ALTER</c> dynamically via
    /// <c>PREPARE</c>/<c>EXECUTE</c>/<c>DEALLOCATE PREPARE</c>.</description></item>
    /// </list>
    /// Every method that uses a session user variable (<c>@repodb_...</c>) or dynamic SQL (<c>PREPARE</c>)
    /// requires <c>AllowUserVariables=True</c> on the connection string - MySql defaults this to
    /// <see langword="false"/>.
    /// </summary>
    internal static class MySqlText
    {
        /// <summary>
        /// Name of the surrogate, always-present ordering column added to every pseudo table (see
        /// <see cref="GetCreatePseudoTableSql"/>). MySQL has no equivalent to Oracle's implicit <c>ROWID</c>
        /// pseudo-column, so this stands in for it wherever a pseudo table's rows need to be read back in the
        /// same order they were bulk-loaded in (matching the original entity/row list order).
        /// </summary>
        private const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        /// <summary>
        /// Name of the index created on every pseudo table's qualifier columns (see
        /// <see cref="GetCreatePseudoTableIndexSql"/>). The pseudo table is always freshly dropped and
        /// re-created (see <see cref="GetCreatePseudoTableSql"/>) before this index is built, and a MySQL
        /// index name only needs to be unique within its own table, so a fixed literal name is safe here -
        /// no risk of colliding with a real index a caller might have on <c>tableName</c> itself.
        /// </summary>
        private const string QualifierIndexName = "__RepoDbBulkQualifierIndex__";

        #region Shared

        /// <summary>
        /// Builds the DDL that (re-)creates a pseudo (staging) table shaped after <paramref name="tableName"/> -
        /// either every column (<paramref name="qualifierField"/> is <see langword="null"/>) or just the one
        /// qualifier column (used by <c>BulkDeleteByKey</c>, which only ever needs the key column staged).
        /// <c>CREATE TABLE ... AS SELECT ... WHERE (1 = 0)</c> copies column names/types (and nullability -
        /// see <see cref="GetAllowNullForColumnSql"/>) but never a source column's own <c>PRIMARY KEY</c> or
        /// <c>AUTO_INCREMENT</c> attribute, so the pseudo table's own identity column (when present) always
        /// comes out as a plain, non-generating column - exactly what the "return identity" paths need, since
        /// they assign it explicitly themselves. <c>MySqlBulkImportPseudoTableType.Physical</c>
        /// creates an ordinary persistent table; anything else (<c>Memory</c>, or an unresolved <c>Auto</c>)
        /// creates a <c>TEMPORARY</c> table instead - MySQL's closest equivalent to Oracle's session-private
        /// Global Temporary Table. In practice every call currently resolves to <c>Physical</c> before it
        /// reaches here (see the remarks on <c>ResolvePseudoTableType</c> in <c>Base/WriteToServer.cs</c>), so
        /// the <c>TEMPORARY</c> branch is written correctly but not yet exercised.
        /// <para>
        /// Leads with an unconditional <c>DROP TABLE IF EXISTS</c> for <paramref name="pseudoTableName"/>
        /// rather than guarding the <c>CREATE</c> with <c>IF NOT EXISTS</c>. The pseudo table name is
        /// deterministic - derived purely from <paramref name="tableName"/> and <paramref name="pseudoTableType"/>
        /// (see <c>GetPseudoTableNameForDelete</c>) - and the very same name is shared by callers that stage
        /// different column shapes against the same real table (e.g. <c>BulkDeleteByKey</c>'s qualifier-only
        /// pseudo table vs. the <c>TEntity</c>/<c>DataTable</c>/<c>DbDataReader</c> <c>BulkDelete</c> overloads'
        /// full-column one). Because <c>Physical</c> pseudo tables persist beyond a single call, an
        /// <c>IF NOT EXISTS</c> guard would silently reuse a leftover table left in the wrong shape by a
        /// different caller (or by a prior run whose own <c>DROP TABLE</c> cleanup never got to execute - e.g.
        /// because the connection had already broken) instead of recreating it - the bulk-copy or JOIN that
        /// follows then fails against mismatched columns, and that failure can itself leave the connection
        /// broken. Dropping first guarantees the table this call creates always matches what this call is
        /// about to stage into it, regardless of what any previous caller left behind.
        /// </para>
        /// </summary>
        /// <param name="tableName">The real table the pseudo table is staged for.</param>
        /// <param name="pseudoTableName">The name to create the pseudo table under.</param>
        /// <param name="pseudoTableType">Whether the pseudo table should be a persistent or temporary table.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <param name="qualifierField">
        /// When provided, only this single column is staged (e.g. <c>BulkDeleteByKey</c>); otherwise every
        /// column of <paramref name="tableName"/> is staged.
        /// </param>
        /// <returns>The <c>DROP TABLE IF EXISTS</c> + <c>CREATE TABLE</c> SQL text.</returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var temporaryKeyword = pseudoTableType == MySqlBulkImportPseudoTableType.Physical
                ? string.Empty
                : "TEMPORARY ";

            return $"DROP TABLE IF EXISTS {quotedPseudoTableName}; " +
                $"CREATE {temporaryKeyword}TABLE {quotedPseudoTableName} " +
                $"({quotedRowOrderColumn} BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY) " +
                $"AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)";
        }

        /// <summary>
        /// Builds the DDL that indexes a pseudo table's qualifier columns - the columns
        /// <c>UpdateFromPseudoTable</c>/<c>MergeFromPseudoTable</c>/<c>DeleteFromPseudoTable</c> join or filter
        /// on against the real table. <c>CREATE TABLE ... AS SELECT</c> (see <see cref="GetCreatePseudoTableSql"/>)
        /// never carries over a source column's own key/index definitions, so without this the pseudo table's
        /// qualifier columns would otherwise be entirely unindexed - forcing a full table scan on the pseudo
        /// side of every merge/update/delete join. Must run before the pseudo table is bulk-loaded, so the
        /// index is built once up front rather than incrementally maintained row-by-row during the load.
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
        /// Builds the DDL that empties a pseudo table before it's bulk-loaded - also resets
        /// <see cref="RowOrderColumnName"/>'s <c>AUTO_INCREMENT</c> counter back to 1. <see cref="GetCreatePseudoTableSql"/>
        /// already hands back a freshly dropped-and-recreated (therefore already-empty) table, so this is
        /// purely defensive belt-and-suspenders at the call sites that run both back to back.
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table to truncate.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>TRUNCATE TABLE</c> SQL text.</returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// Builds the DDL that drops a pseudo table once a bulk operation is done with it. MySQL's
        /// <c>DROP TABLE IF EXISTS</c> is a no-op (not an error) when the table is already gone, and works for
        /// both persistent and <c>TEMPORARY</c> tables alike - no PL/SQL-style exception guard needed.
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
        /// <param name="tableName">The real table being inserted into.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForInsert(string tableName,
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Insert";

        /// <summary>
        /// Builds DDL that dynamically re-creates <paramref name="columnName"/> on <paramref name="pseudoTableName"/>
        /// with the same type it already has, but nullable. Needed because <c>CREATE TABLE ... AS SELECT</c>
        /// (see <see cref="GetCreatePseudoTableSql"/>) carries a source column's <c>NOT NULL</c> attribute over
        /// to the pseudo table, but the identity column is deliberately left unpopulated during the bulk-load
        /// (it's assigned afterward - see <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>), so it
        /// must be made nullable first. Unlike Oracle's <c>ALTER TABLE t MODIFY (col NULL)</c>, MySQL's
        /// <c>MODIFY COLUMN</c> requires the column's full type to be re-stated - there's no "just the
        /// nullability" form - so the current type is looked up from
        /// <c>information_schema.COLUMNS.COLUMN_TYPE</c> (which already includes length/precision/unsigned-ness)
        /// and used to build the <c>ALTER TABLE</c> text dynamically via <c>PREPARE</c>/<c>EXECUTE</c>/
        /// <c>DEALLOCATE PREPARE</c> - all of which work as plain, non-stored-procedure client SQL. Requires
        /// <c>AllowUserVariables=True</c> on the connection string.
        /// </summary>
        /// <param name="pseudoTableName">The pseudo table whose column should be made nullable.</param>
        /// <param name="columnName">The column to make nullable.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The dynamic <c>ALTER TABLE</c> SQL text.</returns>
        public static string GetAllowNullForColumnSql(string pseudoTableName,
            string columnName,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedColumnName = columnName.AsQuoted(true, dbSetting);
            var unquotedPseudoTableName = pseudoTableName.AsUnquoted(true, dbSetting);
            var unquotedColumnName = columnName.AsUnquoted(true, dbSetting);

            return string.Concat(
                "SET @repodb_coltype := (SELECT `COLUMN_TYPE` FROM `information_schema`.`COLUMNS` ",
                "WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = '", unquotedPseudoTableName, "' ",
                "AND `COLUMN_NAME` = '", unquotedColumnName, "'); ",
                "SET @repodb_sql := CONCAT('ALTER TABLE ", quotedPseudoTableName, " MODIFY COLUMN ", quotedColumnName, " ', @repodb_coltype, ' NULL'); ",
                "PREPARE repodb_stmt FROM @repodb_sql; ",
                "EXECUTE repodb_stmt; ",
                "DEALLOCATE PREPARE repodb_stmt;");
        }

        /// <summary>
        /// Builds a query that reports the next identity value for <paramref name="tableName"/> (one past the
        /// highest <paramref name="identityField"/> value currently stored), used to seed the identity
        /// pre-assignment sequence in <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>/
        /// <see cref="GetMergeFromPseudoTableForReturnIdentitySql"/>.
        /// This replaces Oracle's <c>ALL_TAB_IDENTITY_COLS</c> lookup - MySQL has no named sequence object to
        /// look up at all, so the two-column, one-row shape the caller expects
        /// (<c>MySqlExecution.GetIdentitySequenceMetadata</c>) is kept for signature compatibility,
        /// but column 0 now holds the numeric seed (as a string) rather than a sequence name, and column 1 is
        /// always <c>'NO'</c> - MySQL's <c>AUTO_INCREMENT</c> has no Oracle-style <c>GENERATED ALWAYS</c>
        /// distinction; an explicit value is always accepted.
        /// </summary>
        /// <remarks>
        /// Deliberately reads <c>MAX(identityColumn)</c> off <paramref name="tableName"/> itself rather than
        /// <c>information_schema.TABLES.AUTO_INCREMENT</c> (the original implementation). MySQL 8 caches
        /// <c>information_schema.TABLES</c>' dynamic columns - including <c>AUTO_INCREMENT</c> - in
        /// <c>mysql.innodb_table_stats</c> for <c>information_schema_stats_expiry</c> seconds (24 hours by
        /// default), refreshed only via <c>ANALYZE TABLE</c> or expiry - not on every insert. That let a stale,
        /// pre-insert counter get reused as the seed here, so a return-identity bulk insert straight after a
        /// row was already inserted elsewhere in the same table (see the integration test that inserts 10 rows
        /// via <c>InsertAll</c> and then bulk-inserts 10 more with <c>ReturnIdentity</c>) collided on the same
        /// primary key ("Duplicate entry '1' for key ...PRIMARY") instead of continuing after the existing
        /// rows. <c>MAX(identityColumn) + 1</c> is always read live off the table's actual row data, so it can
        /// never be stale - at the cost of reusing an id freed by a deleted row instead of skipping past it the
        /// way the real <c>AUTO_INCREMENT</c> counter would (harmless: MySQL never reserves a deleted row's id).
        /// Reading this value and later using it to seed the pre-assignment statement are still two separate
        /// round trips, which leaves a small race window against a concurrent writer to the same table - see
        /// the remarks on <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>.
        /// </remarks>
        /// <param name="tableName">The real table whose next identity value is being seeded.</param>
        /// <param name="identityField">The identity column to read the current maximum of.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The live <c>MAX(identityColumn) + 1</c> lookup SQL text.</returns>
        public static string GetIdentitySequenceMetadataSql(string tableName,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);

            return string.Concat(
                "SELECT CAST(COALESCE(MAX(", quotedIdentityColumn, "), 0) + 1 AS CHAR) AS `SequenceName`, ",
                "'NO' AS `GenerationType` FROM ", quotedTableName);
        }

        /// <summary>
        /// Builds the multi-statement SQL that moves every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/> and reports back the identity value assigned to each one, in the same
        /// order the rows were originally bulk-loaded in.
        /// </summary>
        /// <remarks>
        /// MySQL's <c>AUTO_INCREMENT</c> has no equivalent to Oracle's per-row <c>SEQUENCE.NEXTVAL</c>, and
        /// relying on <c>LAST_INSERT_ID()</c> plus positional arithmetic after a multi-row
        /// <c>INSERT ... SELECT</c> is not safe under MySQL 8's default (interleaved) <c>innodb_autoinc_lock_mode</c>,
        /// which does not guarantee gap-free identity allocation for that statement shape under concurrent
        /// writers. Instead, this mirrors Oracle's own technique: identity values are pre-assigned into
        /// <paramref name="pseudoTableName"/> <i>before</i> the real <c>INSERT</c>, seeded from
        /// <paramref name="sequenceName"/> (the numeric string fetched moments earlier via
        /// <see cref="GetIdentitySequenceMetadataSql"/>) and incremented once per row via a session user
        /// variable, then copied over as literal values - so the real table's own <c>AUTO_INCREMENT</c> never
        /// fires for these rows at all (MySQL still auto-advances its internal counter past whatever explicit
        /// value it sees, so this doesn't create future collisions). The pre-assignment step and the seed
        /// lookup are two separate round trips, so there is a small window where a concurrent insert into
        /// <paramref name="tableName"/> could race with this operation; this is a known limitation of doing this
        /// without table-level locking (which isn't used here, since <c>LOCK TABLES</c> would silently commit
        /// any transaction the caller already has open on <paramref name="dbSetting"/>'s connection).
        /// <paramref name="identityField"/>'s pre-assigned value and <see cref="RowOrderColumnName"/> (added by
        /// <see cref="GetCreatePseudoTableSql"/>) are what let the final <c>SELECT ... ORDER BY</c> report each
        /// row's identity value back in the original entity/row list order, standing in for Oracle's implicit
        /// <c>ROWID</c> ordering. <paramref name="isAlwaysGenerated"/> needs no MySQL equivalent - there is no
        /// "OVERRIDING SYSTEM VALUE"-style clause required to insert an explicit value into an
        /// <c>AUTO_INCREMENT</c> column. Requires <c>AllowUserVariables=True</c> on the connection string.
        /// </remarks>
        /// <param name="tableName">The real table to insert into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">The columns to copy from the pseudo table into the real table.</param>
        /// <param name="identityField">The identity column being pre-assigned and returned.</param>
        /// <param name="sequenceName">
        /// The numeric seed (as a string) fetched via <see cref="GetIdentitySequenceMetadataSql"/> - the first
        /// identity value that will be assigned.
        /// </param>
        /// <param name="isAlwaysGenerated">Unused for MySQL - kept for signature compatibility with the Oracle-derived caller.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The multi-statement SQL text; the final statement is a <c>SELECT</c> yielding one row per inserted entity.</returns>
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
                "SET @repodb_seq := (", nextIdentityValue, " - 1); ",
                "UPDATE ", quotedPseudoTableName, " SET ", quotedIdentityColumn, " = (@repodb_seq := @repodb_seq + 1); ",
                "INSERT INTO ", quotedTableName, " (", columnList, ") ",
                "SELECT ", columnList, " FROM ", quotedPseudoTableName, "; ",
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, ";");
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
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Merge";

        /// <summary>
        /// Builds the SQL that merges every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/>, without returning identity values.
        /// </summary>
        /// <remarks>
        /// MySQL has no <c>MERGE</c> statement, and <c>INSERT ... ON DUPLICATE KEY UPDATE</c> - MySQL's native
        /// upsert - only detects a "duplicate" via an actual unique/primary key constraint, which
        /// <paramref name="qualifiers"/> is not guaranteed to correspond to (a caller can merge on any field
        /// combination). So this is split into two statements instead: an <c>UPDATE ... INNER JOIN</c> for rows
        /// that match on <paramref name="qualifiers"/>, followed by an <c>INSERT ... SELECT</c> guarded by a
        /// <c>LEFT JOIN ... WHERE ... IS NULL</c> anti-join for rows that don't. The anti-join assumes the
        /// first qualifier column is never legitimately <see langword="null"/> on a real, matched row (true for
        /// the typical case of qualifying on a primary/unique key). <c>ExecuteNonQuery</c>'s return value for
        /// this multi-statement text is expected to be the sum of both statements' affected-row counts, per
        /// MySql's documented behavior for a batched command.
        /// </remarks>
        /// <param name="tableName">The real table to merge into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being merged.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="identityField">Excluded from the <c>INSERT</c> column list, if present, so the real table's own <c>AUTO_INCREMENT</c> generates it for new rows.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The two-statement SQL text.</returns>
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
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var firstQualifierColumn = qualifierList.First().Name.AsQuoted(true, dbSetting);

            var updateStatement = updateableFields.Count > 0
                ? $"UPDATE {quotedTableName} T INNER JOIN {quotedPseudoTableName} S ON ({onClause}) SET {updateSetClause}; "
                : string.Empty;

            return string.Concat(
                updateStatement,
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertValues, " FROM ", quotedPseudoTableName, " S ",
                "LEFT JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "WHERE T.", firstQualifierColumn, " IS NULL;");
        }

        /// <summary>
        /// Builds the multi-statement SQL that merges every row of <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/> and reports back each row's identity value - the existing value for a
        /// row that matched (and was updated), or a freshly assigned value for a row that didn't (and was
        /// inserted) - in the same order the rows were originally bulk-loaded in.
        /// </summary>
        /// <remarks>
        /// Combines the two-statement anti-join technique from <see cref="GetMergeFromPseudoTableSql"/> with
        /// the identity pre-assignment technique from <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>
        /// (see its remarks for the sequence-seeding caveat) - see the inline steps below. A MySQL multi-table
        /// <c>UPDATE</c> (one with a <c>JOIN</c>) cannot use <c>ORDER BY</c>, unlike a single-table <c>UPDATE</c>;
        /// this doesn't affect correctness here since each row's session-variable-assigned value only needs to
        /// be distinct from every other row's, not tied to a particular processing order - only the final
        /// <c>SELECT ... ORDER BY</c> (a plain single-table query) needs to, and does, preserve row order.
        /// Requires <c>AllowUserVariables=True</c> on the connection string.
        /// </remarks>
        /// <param name="tableName">The real table to merge into.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being merged.</param>
        /// <param name="identityField">The identity column being pre-assigned and returned.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="sequenceName">
        /// The numeric seed (as a string) fetched via <see cref="GetIdentitySequenceMetadataSql"/> - the first
        /// identity value that will be assigned to an unmatched (to-be-inserted) row.
        /// </param>
        /// <param name="isAlwaysGenerated">Unused for MySQL - kept for signature compatibility with the Oracle-derived caller.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The multi-statement SQL text; the final statement is a <c>SELECT</c> yielding one row per merged entity.</returns>
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

            var onClause = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
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
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            var firstQualifierColumn = qualifierList.First().Name.AsQuoted(true, dbSetting);

            var updateMatchedRealTableStatement = updateableFields.Count > 0
                ? $"UPDATE {quotedTableName} T INNER JOIN {quotedPseudoTableName} S ON ({onClause}) SET {updateSetClause}; "
                : string.Empty;

            return string.Concat(
                // Step 1: matched rows keep their existing identity value - copy it onto the pseudo row.
                "UPDATE ", quotedPseudoTableName, " S INNER JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "SET S.", quotedIdentityColumn, " = T.", quotedIdentityColumn, "; ",
                // Step 2: unmatched rows get a fresh, pre-assigned identity value.
                "SET @repodb_seq := (", nextIdentityValue, " - 1); ",
                "UPDATE ", quotedPseudoTableName, " S LEFT JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "SET S.", quotedIdentityColumn, " = (@repodb_seq := @repodb_seq + 1) ",
                "WHERE T.", firstQualifierColumn, " IS NULL; ",
                // Step 3: update matched rows in the real table.
                updateMatchedRealTableStatement,
                // Step 4: insert unmatched rows into the real table, using their pre-assigned identity value.
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertValues, " FROM ", quotedPseudoTableName, " S ",
                "LEFT JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "WHERE T.", firstQualifierColumn, " IS NULL; ",
                // Step 5: report every row's final identity value, in original bulk-load order.
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, ";");
        }

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
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Update";

        /// <summary>
        /// Builds the SQL that updates every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>, via MySQL's multi-table <c>UPDATE ... INNER JOIN</c> syntax -
        /// the direct MySQL equivalent of Oracle's <c>MERGE ... WHEN MATCHED THEN UPDATE</c> (there is no
        /// "WHEN NOT MATCHED" branch to translate here; a plain <c>BulkUpdate</c>, unlike <c>BulkMerge</c>,
        /// never inserts).
        /// </summary>
        /// <param name="tableName">The real table to update.</param>
        /// <param name="pseudoTableName">The pseudo table the rows were bulk-loaded into.</param>
        /// <param name="fields">Every column being updated.</param>
        /// <param name="qualifiers">The columns used to match an existing row.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>UPDATE ... INNER JOIN</c> SQL text.</returns>
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

            return $"UPDATE {tableName.AsQuoted(true, dbSetting)} T INNER JOIN {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) SET {updateClause}";
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
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        /// Builds the deterministic pseudo table name used by the <c>BulkDeleteByKey</c> path. Suffixed
        /// <c>DeleteByKey</c> rather than plain <c>Delete</c> so it never collides with
        /// <see cref="GetPseudoTableNameForDelete"/> for the same <paramref name="tableName"/>/<paramref name="pseudoTableType"/> -
        /// <c>BulkDeleteByKey</c> stages only the qualifier column, while <c>BulkDelete</c> stages every
        /// column, and two callers sharing one deterministic name while staging different shapes against the
        /// same real table caused a stale, wrongly-shaped pseudo table (and its equally stale
        /// <c>DbFieldCache</c> entry) to be silently reused.
        /// </summary>
        /// <param name="tableName">The real table being deleted from.</param>
        /// <param name="pseudoTableType">Included in the name so different pseudo table types never collide.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>, used to strip any quoting from <paramref name="tableName"/> before it's folded into the new identifier.</param>
        /// <returns>The pseudo table name.</returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            MySqlBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

        /// <summary>
        /// Builds the SQL that deletes every row of <paramref name="tableName"/> matched by
        /// <paramref name="pseudoTableName"/>, via MySQL's multi-table <c>DELETE ... INNER JOIN</c> syntax -
        /// much simpler than Oracle's <c>ROWID</c>-based subquery form, since MySQL supports deleting directly
        /// through a join.
        /// </summary>
        /// <param name="tableName">The real table to delete from.</param>
        /// <param name="pseudoTableName">The pseudo table holding the key values (or full rows) to match on.</param>
        /// <param name="qualifiers">The columns used to match a row to delete.</param>
        /// <param name="dbSetting">The current <see cref="IDbSetting"/>.</param>
        /// <returns>The <c>DELETE ... INNER JOIN</c> SQL text.</returns>
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

            return $"DELETE T FROM {quotedTableName} T INNER JOIN {quotedPseudoTableName} S ON ({onClause})";
        }

        #endregion
    }
}
