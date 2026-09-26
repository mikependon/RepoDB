#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    internal static class CockroachDbText
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
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <param name="qualifierField"></param>
        /// <returns></returns>
        public static string GetCreatePseudoTableSql(string tableName,
            string pseudoTableName,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting,
            Field qualifierField = null)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var columnList = qualifierField != null ? qualifierField.Name.AsQuoted(true, dbSetting) : "*";
            var isPhysical = pseudoTableType == CockroachDbBulkImportPseudoTableType.Physical;
            var tempKeyword = isPhysical ? string.Empty : "TEMP ";

            // CockroachDB only supports temporary tables experimentally and rejects 'CREATE TEMP TABLE'
            // (SQLSTATE XCEXF) unless the session variable below is switched on first.
            var enableTempTables = isPhysical ? string.Empty : "SET experimental_enable_temp_tables = 'on'; ";

            // A CREATE TABLE ... AS in CockroachDB always gets an implicit hidden 'rowid' primary key, so the
            // row-order column cannot be declared as a second PRIMARY KEY (SQLSTATE 42611). It is only used for
            // ORDER BY, so an identity column without a key constraint is sufficient.
            return enableTempTables +
                $"DROP TABLE IF EXISTS {quotedPseudoTableName}; " +
                $"CREATE {tempKeyword}TABLE {quotedPseudoTableName} " +
                $"AS SELECT {columnList} FROM {quotedTableName} WHERE (1 = 0); " +
                $"ALTER TABLE {quotedPseudoTableName} ADD COLUMN {quotedRowOrderColumn} BIGINT GENERATED ALWAYS AS IDENTITY";
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
            var quotedIndexName = QualifierIndexName.AsQuoted(true, dbSetting);
            var columnList = qualifiers
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return $"CREATE INDEX {quotedIndexName} ON {quotedPseudoTableName} ({columnList})";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetTruncatePseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DELETE FROM {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetDropPseudoTableSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {pseudoTableName.AsQuoted(true, dbSetting)}";

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
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Insert";

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
                .Where(f => !string.Equals(f.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase))
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
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Merge";

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
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);

            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();

            var matchClause = qualifierList
                .Select(f => string.Concat(quotedTableName, ".", f.Name.AsQuoted(true, dbSetting), " = S.", f.Name.AsQuoted(true, dbSetting)))
                .Join(" AND ");

            var insertableFields = fieldList
                .Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase))
                .AsList();

            var updateableFields = insertableFields
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();

            var insertColumns = insertableFields
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var updateStatement = updateableFields.Count > 0
                ? string.Concat("UPDATE ", quotedTableName, " SET ", updateableFields
                    .Select(f => string.Concat(f.Name.AsQuoted(true, dbSetting), " = S.", f.Name.AsQuoted(true, dbSetting)))
                    .Join(", "), " FROM ", quotedPseudoTableName, " S WHERE (", matchClause, "); ")
                : string.Empty;

            return string.Concat(
                updateStatement,
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") ",
                "SELECT ", insertColumns, " FROM ", quotedPseudoTableName, " S ",
                "WHERE NOT EXISTS (SELECT 1 FROM ", quotedTableName, " WHERE ", matchClause, ") ",
                "ORDER BY S.", quotedRowOrderColumn, ";");
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

            var matchClause = qualifierList
                .Select(f => string.Concat(quotedTableName, ".", f.Name.AsQuoted(true, dbSetting), " = ", quotedPseudoTableName, ".", f.Name.AsQuoted(true, dbSetting)))
                .Join(" AND ");

            var sequenceExpression = string.Concat(
                "nextval(pg_get_serial_sequence('", quotedTableName.Replace("'", "''", System.StringComparison.Ordinal), "', '", identityField.Name.Replace("'", "''", System.StringComparison.Ordinal), "'))");

            var preAssignStatement = string.Concat(
                "UPDATE ", quotedPseudoTableName, " SET ", quotedIdentityColumn, " = ", sequenceExpression, " ",
                "WHERE NOT EXISTS (SELECT 1 FROM ", quotedTableName, " WHERE ", matchClause, "); ");

            return string.Concat(
                preAssignStatement,
                "INSERT INTO ", quotedTableName, " (", insertColumns, ") OVERRIDING SYSTEM VALUE ",
                "SELECT ", insertColumns, " FROM ", quotedPseudoTableName, " ORDER BY ", quotedRowOrderColumn, " ",
                "ON CONFLICT (", conflictColumns, ") ", conflictAction, " ",
                "RETURNING ", quotedIdentityColumn, " AS ", resultAlias, ";");
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
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Update";

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
        public static string GetUpdateFromPseudoTableSql(string tableName,
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
                .Select(f => $"{quotedTableName}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            var updateClause = fieldList
                .Where(f => qualifierList.Any(q => string.Equals(q.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)) == false &&
                    (identityField == null || !string.Equals(identityField.Name, f.Name, System.StringComparison.OrdinalIgnoreCase)))
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return $"UPDATE {quotedTableName} SET {updateClause} FROM {quotedPseudoTableName} S WHERE ({onClause})";
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
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            CockroachDbBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

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
                .Select(f => $"{quotedTableName}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(" AND ");

            return $"DELETE FROM {quotedTableName} USING {quotedPseudoTableName} S WHERE ({onClause})";
        }

        #endregion
    }
}
