using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.MySqlConnector;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.MySqlConnector.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// A light-weight, allocation-cheap builder of the raw SQL text used by the MySqlConnector bulk operations
    /// (<c>BulkInsert</c>, <c>BulkMerge</c>, <c>BulkUpdate</c> and <c>BulkDelete</c>). Every method here is
    /// a pure string builder - no I/O, no caching - callers (<see cref="RepoDb.MySqlConnector.BulkOperations.Extensions.MySqlConnectorExecution"/>)
    /// own execution.
    /// </summary>
    internal static class MySqlConnectorText
    {
        #region Shared

        /// <summary>
        /// Strips MySqlConnector identifier quote characters from <paramref name="tableName"/> before it is embedded
        /// inside a newly-built identifier (a pseudo table name). Needed because a mapped table name can
        /// arrive pre-quoted (e.g. <c>[Map("\"MixedCaseTable\"")]</c>, used to force case-preservation) - if
        /// left as-is, concatenating it into <c>$"{pseudoTableType}{tableName}Suffix"</c> embeds the quote
        /// characters mid-string, and re-quoting that combined string later produces an invalid, only
        /// partially-quoted identifier (e.g. <c>"Physical"BulkOperationIdentityTable"Delete"</c>, which
        /// MySqlConnector rejects with <c>ORA-03049</c>).
        /// </summary>
        private static string UnquoteForPseudoTableName(string tableName) =>
            tableName?.Replace("\"", string.Empty);

        /// <summary>
        /// Builds a guarded (idempotent) <c>CREATE TABLE</c> statement for the staging/pseudo table used
        /// by a bulk operation. Guarded with a PL/SQL block that swallows ORA-00955 ("name is already
        /// used by an existing object") so repeated calls against the same table - common since the
        /// pseudo table name is deterministic per (tableName, pseudoTableType) pair, not a fresh GUID
        /// per call - do not fail. The staging table always starts out structurally identical to, and
        /// empty relative to, the target table (<c>WHERE (1 = 0)</c> copies columns/types, not rows).
        /// </summary>
        /// <param name="qualifierField">
        /// When provided, the pseudo table is projected down to just this one column (used by <c>BulkDelete</c>'s
        /// <c>primaryKeys</c> overload, which only ever has a single key value per row to stage). Defaults to
        /// every column (<c>SELECT *</c>) when <see langword="null"/>.
        /// </param>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";

            // Physical: an ordinary heap table, shared/visible across sessions.
            // Memory: a Global Temporary Table whose rows are private per session but whose definition
            // (created once) is shared - safe for concurrent connections to reuse. Auto resolves to either
            // of these (see MySqlConnectorConnectionExtension.ResolvePseudoTableType) before this method ever sees it.
            var createClause = pseudoTableType == MySqlConnectorBulkImportPseudoTableType.Physical
                ? $"CREATE TABLE {quotedPseudoTableName} AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)"
                : $"CREATE GLOBAL TEMPORARY TABLE {quotedPseudoTableName} ON COMMIT PRESERVE ROWS AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)";

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

        #region Insert

        /// <summary>
        /// Returns the deterministic name of the staging/pseudo table for a <c>BulkInsert</c>'s identity-return
        /// path (the plain fire-and-forget path bulk-writes straight into the real table - no pseudo table
        /// needed there at all).
        /// </summary>
        public static string GetPseudoTableNameForInsert(string tableName,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Insert";

        /// <summary>
        /// Builds an <c>ALTER TABLE ... MODIFY (column NULL)</c> statement that drops a <c>NOT NULL</c>
        /// constraint from a column of the staging/pseudo table.
        /// </summary>
        /// <remarks>
        /// MySqlConnector's <c>CREATE TABLE ... AS SELECT</c> (used by <see cref="GetCreatePseudoTableSql"/>) carries
        /// over a source column's <c>NOT NULL</c> constraint even though the <c>WHERE (1 = 0)</c> clause
        /// copies no rows - confirmed live via <c>ORA-26010: Column ... is NOT NULL and is not being loaded</c>.
        /// The identity column is the one column the identity-return path deliberately leaves unpopulated
        /// during the initial bulk-write into the staging table (its value is generated afterward, via
        /// <c>UPDATE ... SET identityColumn = sequence.NEXTVAL</c> - see
        /// <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>), so a <c>NOT NULL</c> inherited from
        /// the real table would otherwise make that initial bulk-write fail whenever the source entities/
        /// <c>DataTable</c>/rows don't already carry an explicit value for it (e.g. a dynamic
        /// <c>ExpandoObject</c> that simply omits the identity property). Safe to run unconditionally on the
        /// staging table - it is transient, internal scratch space with no integrity requirements of its own.
        /// </remarks>
        public static string GetAllowNullForColumnSql(string pseudoTableName,
            string columnName,
            IDbSetting dbSetting) =>
            $"ALTER TABLE {pseudoTableName.AsQuoted(true, dbSetting)} MODIFY ({columnName.AsQuoted(true, dbSetting)} NULL)";

        /// <summary>
        /// Builds the query that resolves the sequence (and its generation mode) backing an MySqlConnector 12c+
        /// <c>IDENTITY</c> column. Needed because MySqlConnector does not support <c>RETURNING ... BULK COLLECT INTO</c>
        /// on an <c>INSERT ... SELECT</c> statement at all (only on <c>INSERT ... VALUES</c>, and on
        /// <c>UPDATE</c>/<c>DELETE</c>) - so <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/> cannot
        /// rely on <c>RETURNING</c> to learn the generated values. Instead it calls <c>{sequence}.NEXTVAL</c>
        /// itself, once per staged row, which requires knowing the sequence's name up front.
        /// </summary>
        /// <remarks>
        /// Every identity column this provider recognizes already has a matching <c>ALL_TAB_IDENTITY_COLS</c>
        /// row - see <c>MySqlConnectorDbHelper.GetCommandText</c>'s <c>IsIdentity</c> detection, which is derived from
        /// this exact same join - so this is guaranteed to find exactly one row for a field that
        /// <see cref="DbFieldCollection.GetIdentity"/> already returned as non-<see langword="null"/>.
        /// </remarks>
        public static string GetIdentitySequenceMetadataSql() =>
            "SELECT SEQUENCE_NAME AS \"SequenceName\", GENERATION_TYPE AS \"GenerationType\" " +
            "FROM ALL_TAB_IDENTITY_COLS " +
            "WHERE OWNER = COALESCE(:Schema, SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA')) " +
            "AND TABLE_NAME = :TableName " +
            "AND COLUMN_NAME = :ColumnName";

        /// <summary>
        /// Builds the PL/SQL block that moves every row currently staged in <paramref name="pseudoTableName"/>
        /// into <paramref name="tableName"/>, along with the generated identity value for each row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// MySqlConnector does not support combining <c>RETURNING ... BULK COLLECT INTO</c> with an <c>INSERT ... SELECT</c>
        /// statement - only with <c>INSERT ... VALUES</c> (single-row, or array-bound via <c>FORALL</c>) and with
        /// <c>UPDATE</c>/<c>DELETE</c> (confirmed live via <c>ORA-03049</c>). So instead of asking MySqlConnector to hand
        /// back auto-generated values, this pre-generates every row's identity value itself - via
        /// <c>UPDATE ... SET identityColumn = sequence.NEXTVAL</c> against the staging table (an <c>UPDATE</c>,
        /// so no <c>RETURNING</c> restriction applies, and MySqlConnector evaluates <c>NEXTVAL</c> once per row updated) -
        /// then moves the now fully-populated staging rows into <paramref name="tableName"/> via a plain
        /// <c>INSERT ... SELECT</c> (no <c>RETURNING</c> needed there at all, since the identity values are
        /// already sitting in the staging table's columns), and finally reads them back from the staging table
        /// itself as an MySqlConnector 12c+ implicit result set (<c>DBMS_SQL.RETURN_RESULT</c>) - the same mechanism
        /// <c>RepoDb.MySqlConnector</c>'s single-row Insert/Merge statement builder uses (see
        /// <c>MySqlConnectorStatementBuilder.WrapWithReturningResult</c>).
        /// </para>
        /// <para>
        /// The staging rows are read back ordered by <c>ROWID</c>, so the returned identity values line up,
        /// position-for-position, with the order they were originally bulk-written into the staging table in -
        /// the practical (not contractually guaranteed by MySqlConnector, but true for an untouched, freshly-loaded
        /// table read back immediately after) order a full table scan returns them in. <c>UPDATE</c> does not
        /// change a row's <c>ROWID</c>, so this holds after the identity-generating <c>UPDATE</c> too.
        /// </para>
        /// </remarks>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be inserted (including <paramref name="identityField"/>).</param>
        /// <param name="identityField">The identity column to pre-generate a value for on every staged row.</param>
        /// <param name="sequenceName">The name of the sequence backing <paramref name="identityField"/> - see <see cref="GetIdentitySequenceMetadataSql"/>.</param>
        /// <param name="isAlwaysGenerated">
        /// Whether <paramref name="identityField"/> is <c>GENERATED ALWAYS AS IDENTITY</c> (rather than
        /// <c>GENERATED BY DEFAULT</c>) - if so, the <c>INSERT</c> needs <c>OVERRIDING SYSTEM VALUE</c> to be
        /// allowed to supply an explicit value for it at all.
        /// </param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
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
            var quotedSequenceName = sequenceName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);
            var overridingClause = isAlwaysGenerated ? "OVERRIDING SYSTEM VALUE " : string.Empty;

            var columnList = fields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return string.Concat(
                "DECLARE l_repodb_cursor SYS_REFCURSOR; ",
                "BEGIN ",
                "UPDATE ", quotedPseudoTableName, " SET ", quotedIdentityColumn, " = ", quotedSequenceName, ".NEXTVAL; ",
                "INSERT INTO ", quotedTableName, " (", columnList, ") ", overridingClause,
                "SELECT ", columnList, " FROM ", quotedPseudoTableName, "; ",
                "OPEN l_repodb_cursor FOR SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ROWID; ",
                "DBMS_SQL.RETURN_RESULT(l_repodb_cursor); ",
                "END;");
        }

        #endregion

        #region Merge

        /// <summary>
        /// Returns the deterministic name of the staging/pseudo table for a <c>BulkMerge</c> against
        /// <paramref name="tableName"/>. Deterministic (not a fresh GUID per call) so the same staging
        /// table definition can be created once and reused (after a <c>TRUNCATE</c>) by later calls.
        /// </summary>
        public static string GetPseudoTableNameForMerge(string tableName,
            MySqlConnectorBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Merge";

        /// <summary>
        /// Builds the <c>MERGE INTO ... USING ... ON (...) WHEN MATCHED ... WHEN NOT MATCHED ...</c>
        /// statement that upserts every row currently staged in <paramref name="pseudoTableName"/> into
        /// <paramref name="tableName"/>. Note: MySqlConnector's <c>MERGE</c> syntax does not accept the <c>AS</c>
        /// keyword before a table/subquery alias (unlike most other clauses) - the bare <c>T</c>/<c>S</c>
        /// aliases below are intentional, not an oversight.
        /// </summary>
        /// <remarks>
        /// <paramref name="identityField"/>, when provided, is always left out of the <c>WHEN NOT MATCHED THEN
        /// INSERT</c> column list - regardless of whether it also happens to be one of <paramref name="qualifiers"/>
        /// (the common default fallback - see <c>GetQualifierFields</c>). A brand new row's identity property is
        /// typically a non-nullable default (e.g. <c>0</c>), not a real value the caller intends to insert as-is;
        /// omitting the column from the <c>INSERT</c> lets MySqlConnector apply its own identity/sequence default instead.
        /// Left in, every unmatched row would explicitly insert that same default value, and every row past the
        /// first would fail with <c>ORA-00001</c> (unique constraint violated) the moment more than one row shares
        /// it - confirmed live via a <c>BulkMerge</c> of several new rows (all with a default, unset identity
        /// property) into an empty table.
        /// </remarks>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be merged (inserted and/or updated).</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="identityField">The identity column, if any, to leave out of the <c>INSERT</c> column list.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
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

            var insertableFields = fieldList
                .Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .AsList();

            var insertColumns = insertableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var insertValues = insertableFields
                .Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            // A MERGE with nothing but qualifier fields has no columns left to update on a match - MySqlConnector
            // rejects an empty "UPDATE SET" list, so the whole WHEN MATCHED branch is omitted for that
            // (unusual, qualifiers-cover-every-column) case rather than emitting invalid SQL.
            var whenMatchedClause = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateableFields.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ")} "
                : string.Empty;

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) {whenMatchedClause}WHEN NOT MATCHED THEN INSERT ({insertColumns}) VALUES ({insertValues})";
        }

        /// <summary>
        /// Builds the PL/SQL block that upserts every row currently staged in <paramref name="pseudoTableName"/>
        /// into <paramref name="tableName"/>, along with the identity value for each row - the existing value
        /// for a row that already exists in <paramref name="tableName"/> (matched by <paramref name="qualifiers"/>),
        /// or a freshly-generated one for a row that doesn't (about to be inserted).
        /// </summary>
        /// <remarks>
        /// <para>
        /// A single-row <c>MERGE ... RETURNING</c> is only supported starting with MySqlConnector Database 23ai (see the
        /// remarks on <c>MySqlConnectorStatementBuilder.CreateMerge</c>) - on every earlier version, <c>RETURNING</c>
        /// after <c>MERGE</c> doesn't parse at all (<c>ORA-00933</c>). So, exactly like
        /// <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>, this never uses <c>RETURNING</c>: it
        /// pre-generates or looks up every row's identity value itself, directly against the staging table,
        /// before the actual <c>MERGE</c> runs.
        /// </para>
        /// <para>
        /// Two <c>UPDATE</c> statements against the staging table do this, in order:
        /// </para>
        /// <para>
        /// 1. For every staged row that already exists in <paramref name="tableName"/> (matched by
        /// <paramref name="qualifiers"/>), copy that row's real, existing identity value onto the staged row -
        /// needed because the caller's own copy of an existing row may not carry it (e.g. <paramref name="qualifiers"/>
        /// is some other natural/business key, not the identity column itself).
        /// </para>
        /// <para>
        /// 2. For every staged row that does <i>not</i> match an existing row (i.e. about to be inserted),
        /// generate a brand new identity value via the backing sequence's <c>NEXTVAL</c> - matching
        /// <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>'s technique. Deliberately keyed off
        /// "does a matching row exist in <paramref name="tableName"/>" (a fresh <c>NOT EXISTS</c> check) rather
        /// than "is the staged identity column <c>NULL</c>" - the latter would misfire whenever <paramref name="qualifiers"/>
        /// happens to be the identity column itself (a common default - see <c>GetQualifierFields</c>'s
        /// primary-then-identity fallback), since a brand new row's identity property is typically a non-nullable
        /// default (e.g. <c>0</c>), not actually <see langword="null"/>, even though it still doesn't match any
        /// real row.
        /// </para>
        /// <para>
        /// The actual <c>MERGE</c> that follows never touches <paramref name="identityField"/> on a match (it is
        /// always excluded from the <c>WHEN MATCHED THEN UPDATE SET</c> list, regardless of whether it happens to
        /// also be one of <paramref name="qualifiers"/>) and supplies it explicitly - via <paramref name="isAlwaysGenerated"/>'s
        /// <c>OVERRIDING SYSTEM VALUE</c>, if needed - on a non-match, since by then every staged row's
        /// <paramref name="identityField"/> column already holds its final value from the two <c>UPDATE</c>s above.
        /// Finally, every staged row's identity value is read back ordered by <c>ROWID</c> - see the ordering
        /// caveat on <see cref="GetInsertFromPseudoTableForReturnIdentitySql"/>, identical here.
        /// </para>
        /// </remarks>
        /// <param name="tableName">The name of the real, target table.</param>
        /// <param name="pseudoTableName">The name of the staging table that was bulk-written to.</param>
        /// <param name="fields">Every field that was staged and should be merged (inserted and/or updated), including <paramref name="identityField"/>.</param>
        /// <param name="identityField">The identity column to resolve (matched rows) or pre-generate (new rows) a value for on every staged row.</param>
        /// <param name="qualifiers">The field(s) used to match an existing row (the <c>ON</c> clause).</param>
        /// <param name="sequenceName">The name of the sequence backing <paramref name="identityField"/> - see <see cref="GetIdentitySequenceMetadataSql"/>.</param>
        /// <param name="isAlwaysGenerated">
        /// Whether <paramref name="identityField"/> is <c>GENERATED ALWAYS AS IDENTITY</c> (rather than
        /// <c>GENERATED BY DEFAULT</c>) - if so, the <c>MERGE</c>'s insert branch needs <c>OVERRIDING SYSTEM VALUE</c>
        /// to be allowed to supply an explicit value for it at all.
        /// </param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
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
            var quotedSequenceName = sequenceName.AsQuoted(true, dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);
            var overridingClause = isAlwaysGenerated ? "OVERRIDING SYSTEM VALUE " : string.Empty;

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

            var whenMatchedClause = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateableFields.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ")} "
                : string.Empty;

            return string.Concat(
                "DECLARE l_repodb_cursor SYS_REFCURSOR; ",
                "BEGIN ",
                "UPDATE ", quotedPseudoTableName, " S SET ", quotedIdentityColumn, " = (SELECT T.", quotedIdentityColumn, " FROM ", quotedTableName, " T WHERE ", onClause, ") ",
                "WHERE EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClause, "); ",
                "UPDATE ", quotedPseudoTableName, " S SET ", quotedIdentityColumn, " = ", quotedSequenceName, ".NEXTVAL ",
                "WHERE NOT EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClause, "); ",
                "MERGE INTO ", quotedTableName, " T USING ", quotedPseudoTableName, " S ON (", onClause, ") ",
                whenMatchedClause,
                "WHEN NOT MATCHED THEN INSERT (", insertColumns, ") ", overridingClause, "VALUES (", insertValues, "); ",
                "OPEN l_repodb_cursor FOR SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM ", quotedPseudoTableName, " ORDER BY ROWID; ",
                "DBMS_SQL.RETURN_RESULT(l_repodb_cursor); ",
                "END;");
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
            MySqlConnectorBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Update";

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
            MySqlConnectorBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Delete";

        /// <summary>
        /// Builds the <c>DELETE FROM ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c> statement that
        /// removes every row on <paramref name="tableName"/> matched by a row currently staged in
        /// <paramref name="pseudoTableName"/>. MySqlConnector's <c>DELETE</c> statement cannot directly target a
        /// joined result the way e.g. SQL Server's <c>DELETE ... FROM ... INNER JOIN ...</c> can - the only
        /// join-based <c>DELETE</c> form MySqlConnector offers, <c>DELETE FROM (SELECT * FROM t1 JOIN t2 ...)</c>
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
