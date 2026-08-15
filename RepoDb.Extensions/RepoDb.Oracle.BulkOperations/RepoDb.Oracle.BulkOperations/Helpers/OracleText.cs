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
    /// 
    /// </summary>
    internal static class OracleText
    {
        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static string UnquoteForPseudoTableName(string tableName) =>
            tableName?.Replace("\"", string.Empty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="qualifierField"></param>
        /// <returns></returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            OracleBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var createClause = pseudoTableType == OracleBulkImportPseudoTableType.Physical
                ? $"CREATE TABLE {quotedPseudoTableName} AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)"
                : $"CREATE GLOBAL TEMPORARY TABLE {quotedPseudoTableName} ON COMMIT PRESERVE ROWS AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0)";

            // ORA-00955: name is already used by an existing object
            return $"BEGIN EXECUTE IMMEDIATE '{createClause}'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -955 THEN RAISE; END IF; END;";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            // ORA-00942: table or view does not exist
            return $"BEGIN EXECUTE IMMEDIATE 'DROP TABLE {quotedPseudoTableName}'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -942 THEN RAISE; END IF; END;";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetCreatePseudoTableIndexSql(string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIndexName = $"IX_{pseudoTableName}".AsQuoted(true, dbSetting);
            var columnList = qualifiers
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var createClause = $"CREATE INDEX {quotedIndexName} ON {quotedPseudoTableName} ({columnList})";

            // ORA-00955: name is already used by an existing object
            return $"BEGIN EXECUTE IMMEDIATE '{createClause}'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -955 THEN RAISE; END IF; END;";
        }

        #endregion

        #region Insert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForInsert(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Insert";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="columnName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetAllowNullForColumnSql(string pseudoTableName,
            string columnName,
            IDbSetting dbSetting) =>
            $"ALTER TABLE {pseudoTableName.AsQuoted(true, dbSetting)} MODIFY ({columnName.AsQuoted(true, dbSetting)} NULL)";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetIdentitySequenceMetadataSql() =>
            "SELECT SEQUENCE_NAME AS \"SequenceName\", GENERATION_TYPE AS \"GenerationType\" " +
            "FROM ALL_TAB_IDENTITY_COLS " +
            "WHERE OWNER = COALESCE(:Schema, SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA')) " +
            "AND TABLE_NAME = :TableName " +
            "AND COLUMN_NAME = :ColumnName";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="sequenceName"></param>
        /// <param name="isAlwaysGenerated"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Merge";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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

            // A MERGE with nothing but qualifier fields has no columns left to update on a match - Oracle
            // rejects an empty "UPDATE SET" list, so the whole WHEN MATCHED branch is omitted for that
            // (unusual, qualifiers-cover-every-column) case rather than emitting invalid SQL.
            var whenMatchedClause = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateableFields.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ")} "
                : string.Empty;

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) {whenMatchedClause}WHEN NOT MATCHED THEN INSERT ({insertColumns}) VALUES ({insertValues})";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="sequenceName"></param>
        /// <param name="isAlwaysGenerated"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForUpdate(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Update";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForDelete(string tableName,
            OracleBulkImportPseudoTableType pseudoTableType) => $"{pseudoTableType.ToString()}{UnquoteForPseudoTableName(tableName)}Delete";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
