using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ClickHouseText
    {
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
            IDbSetting dbSetting) =>
            $"DROP TABLE IF EXISTS {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="quotedPseudoTableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForMerge(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Merge";

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
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Update";

        /// <summary>
        /// 
        /// </summary>
        private const string MergeKeyColumnName = "__RepoDb_MergeKey__";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <returns></returns>
        public static string GetPseudoJoinTableName(string pseudoTableName) => $"{pseudoTableName}Join";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeKeyExpression(IList<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var columnList = qualifiers
                .Select(f => f.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            return $"sipHash64({columnList})";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoJoinTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetCreatePseudoJoinTableSql(string pseudoTableName,
            string pseudoJoinTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedPseudoJoinTableName = pseudoJoinTableName.AsQuoted(true, dbSetting);
            var quotedMergeKeyColumn = MergeKeyColumnName.AsQuoted(true, dbSetting);
            var keyExpression = GetMergeKeyExpression(qualifiers.AsList(), dbSetting);

            return $"CREATE TABLE {quotedPseudoJoinTableName} ENGINE = Join(ANY, LEFT, {quotedMergeKeyColumn}) AS SELECT {keyExpression} AS {quotedMergeKeyColumn}, * FROM {quotedPseudoTableName} WHERE (1 = 0)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="pseudoJoinTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPopulatePseudoJoinTableSql(string pseudoTableName,
            string pseudoJoinTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedPseudoJoinTableName = pseudoJoinTableName.AsQuoted(true, dbSetting);
            var keyExpression = GetMergeKeyExpression(qualifiers.AsList(), dbSetting);

            return $"INSERT INTO {quotedPseudoJoinTableName} SELECT {keyExpression}, * FROM {quotedPseudoTableName}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="keyFields"></param>
        /// <returns></returns>
        public static IList<Field> GetUpdatableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> keyFields)
        {
            var nonUpdatable = qualifiers.AsList().Concat(keyFields ?? Enumerable.Empty<Field>()).AsList();

            return fields
                .Where(f => nonUpdatable.Any(nu => string.Equals(nu.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == false)
                .AsList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="keyFields"></param>
        /// <param name="database"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> keyFields,
            string database,
            IDbSetting dbSetting)
        {
            var quotedTableName = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var pseudoJoinTableName = GetPseudoJoinTableName(pseudoTableName);

            var qualifierList = qualifiers.AsList();
            var updatableFields = GetUpdatableFields(fields, qualifierList, keyFields);

            var keyExpression = GetMergeKeyExpression(qualifierList, dbSetting);

            var setClause = updatableFields
                .Select(f =>
                {
                    var quotedField = f.Name.AsQuoted(true, dbSetting);
                    return $"{quotedField} = joinGet('{database}.{pseudoJoinTableName}', '{f.Name}', {keyExpression})";
                })
                .Join(", ");

            var whereClause = GetTupleInPseudoTableClause(qualifierList, quotedPseudoTableName, dbSetting);

            return $"ALTER TABLE {quotedTableName} UPDATE {setClause} WHERE {whereClause}";
        }

        #endregion

        #region Insert (unmatched rows of a merge)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
                "WHERE T.", firstQualifierColumn, " IS NULL ",
                "SETTINGS join_use_nulls = 1");
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
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}Delete";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableNameForDeleteByKey(string tableName,
            ClickHouseBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting) => $"{pseudoTableType.ToString()}{tableName.AsUnquoted(dbSetting)}DeleteByKey";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
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
            var whereClause = GetTupleInPseudoTableClause(qualifiers.AsList(), quotedPseudoTableName, dbSetting);

            return $"ALTER TABLE {quotedTableName} DELETE WHERE {whereClause}";
        }

        #endregion
    }
}
