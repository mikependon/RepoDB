using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.Db2;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    internal static class Db2Text
    {
        private const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <param name="qualifierFields"></param>
        /// <param name="nullableFields"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            IDbSetting dbSetting,
            IEnumerable<Field> qualifierFields = null,
            IEnumerable<Field> nullableFields = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var qualifierFieldList = qualifierFields?.AsList();
            var nullableFieldNames = nullableFields?
                .Select(f => f.Name)
                .AsList();

            if (nullableFieldNames?.Count > 0 && (qualifierFieldList == null || qualifierFieldList.Count == 0))
            {
                throw new ArgumentException(
                    $"'{nameof(nullableFields)}' requires '{nameof(qualifierFields)}' to also be supplied (the full column list, not just the fields that need forcing).",
                    nameof(qualifierFields));
            }

            string BuildSelectColumn(Field field)
            {
                var quotedName = field.Name.AsQuoted(true, dbSetting);
                var forceNullable = nullableFieldNames?.Any(name => string.Equals(name, field.Name, StringComparison.OrdinalIgnoreCase)) == true;
                return forceNullable ?
                    $"CASE WHEN 1 = 0 THEN NULL ELSE {quotedName} END AS {quotedName}" :
                    quotedName;
            }

            var columnList = qualifierFieldList?.Count > 0
                ? qualifierFieldList.Select(BuildSelectColumn).Join(", ")
                : "*";

            return string.Concat(
                "CREATE TABLE ", quotedPseudoTableName, " AS (SELECT ", columnList, " FROM ", quotedTableName, ") DEFINITION ONLY; ",
                "ALTER TABLE ", quotedPseudoTableName, " ADD COLUMN ", quotedRowOrderColumn, " BIGINT NOT NULL WITH DEFAULT 0; ",
                "ALTER TABLE ", quotedPseudoTableName, " ALTER COLUMN ", quotedRowOrderColumn, " DROP DEFAULT SET GENERATED ALWAYS AS IDENTITY");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"TRUNCATE TABLE {pseudoTableName.AsQuoted(true, dbSetting)} IMMEDIATE";

        /// <summary>
        ///
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE {pseudoTableName.AsQuoted(true, dbSetting)}";

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

            return $"CREATE INDEX {quotedIndexName} ON {quotedPseudoTableName} ({columnList}) CLUSTER";
        }

        #endregion

        #region Insert

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForInsert(string tableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Insert";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM FINAL TABLE (",
                "INSERT INTO ", quotedTableName, " (", columnList, ") ",
                "SELECT ", columnList, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn,
                ") ORDER BY ", quotedIdentityColumn);
        }

        #endregion

        #region Merge

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Merge";

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

            var matchedClause = updateableFields.Count > 0
                ? $"WHEN MATCHED THEN UPDATE SET {updateSetClause} "
                : string.Empty;

            return string.Concat(
                "MERGE INTO ", quotedTableName, " T USING ", quotedPseudoTableName, " S ON (", onClause, ") ",
                matchedClause,
                "WHEN NOT MATCHED THEN INSERT (", insertColumns, ") VALUES (", insertValues, ")");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetMergeMatchSnapshotSql(string tableName,
            string pseudoTableName,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var rowOrderAlias = "RowOrder".AsQuoted(dbSetting);
            var resultAlias = "Result".AsQuoted(dbSetting);

            var onClause = qualifiers.AsList()
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            return string.Concat(
                "SELECT S.", quotedRowOrderColumn, " AS ", rowOrderAlias, ", T.", quotedIdentityColumn, " AS ", resultAlias, " ",
                "FROM ", quotedPseudoTableName, " S LEFT JOIN ", quotedTableName, " T ON (", onClause, ") ",
                "ORDER BY S.", quotedRowOrderColumn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetMergeUpdateOnlySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);

            var qualifierList = qualifiers.AsList();

            var onClause = qualifierList
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateableFields = fields.AsList()
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase) &&
                    qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            if (updateableFields.Count == 0)
            {
                return null;
            }

            var updateSetClause = updateableFields
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return string.Concat(
                "MERGE INTO ", quotedTableName, " T USING ", quotedPseudoTableName, " S ON (", onClause, ") ",
                "WHEN MATCHED THEN UPDATE SET ", updateSetClause);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetMergeInsertOnlyForReturnIdentitySql(string tableName,
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

            var insertableFields = fields.AsList()
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .AsList();

            var onClause = qualifiers.AsList()
                .Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var insertColumns = insertableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return string.Concat(
                "SELECT ", quotedIdentityColumn, " AS ", resultAlias, " FROM FINAL TABLE (",
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertColumns, " FROM ", quotedPseudoTableName, " S ",
                "WHERE NOT EXISTS (SELECT 1 FROM ", quotedTableName, " T WHERE ", onClause, ") ",
                "ORDER BY S.", quotedRowOrderColumn,
                ") ORDER BY ", quotedIdentityColumn);
        }

        #endregion

        #region Update

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForUpdate(string tableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Update";

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
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForDelete(string tableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            Db2BulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

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

            return $"DELETE FROM {quotedTableName} T WHERE EXISTS (SELECT 1 FROM {quotedPseudoTableName} S WHERE {onClause})";
        }

        #endregion
    }
}
