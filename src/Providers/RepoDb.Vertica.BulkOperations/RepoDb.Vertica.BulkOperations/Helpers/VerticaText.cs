#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoDb.Enumerations.Vertica;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal static class VerticaText
    {
        private const string RowOrderColumnName = "__RepoDbBulkRowOrder__";

        #region Shared

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationTag"></param>
        /// <returns></returns>
        public static string CreatePseudoTableName(string operationTag) =>
            "RDBLK" + operationTag + Guid.NewGuid().ToString("N")[..20];

        /// <summary>
        /// 
        /// </summary>
        public static Field RowOrderField { get; } = new(RowOrderColumnName, typeof(long));

        private static readonly DbTypeNameToColumnNameResolver ColumnTypeResolver = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetColumnTypeSql(DbField field)
        {
            var baseType = ColumnTypeResolver.Resolve(field.DatabaseType);
            var precision = field.Precision ?? 18;
            var scale = field.Scale ?? 0;
            var size = field.Size ?? 1;

            return field.DatabaseType?.ToLowerInvariant() switch
            {
                "numeric" or "decimal" => $"{baseType}({precision},{scale})",
                "char" or "varchar" or "binary" or "varbinary" => $"{baseType}({size})",
                _ => baseType,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetCreatePseudoTableSql(string pseudoTableName,
            IEnumerable<Field> fields,
            DbFieldCollection dbFields,
            VerticaBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);

            var columnDefinitions = fields
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} {GetColumnTypeSql(dbFields.GetByUnquotedName(f.Name.AsUnquoted(true, dbSetting)))}")
                .Join(", ");

            var isMemory = pseudoTableType == VerticaBulkImportPseudoTableType.Memory;
            var tableKind = isMemory ? "GLOBAL TEMPORARY TABLE" : "TABLE";
            var onCommitClause = isMemory ? " ON COMMIT PRESERVE ROWS" : string.Empty;
            return $"CREATE {tableKind} {quotedPseudoTableName} ({columnDefinitions}, {quotedRowOrderColumn} BIGINT DEFAULT 0 NOT NULL){onCommitClause}";
        }

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
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetPseudoTableRowCountSql(string pseudoTableName,
            IDbSetting dbSetting) =>
            $"SELECT COUNT(*) FROM {pseudoTableName.AsQuoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string ColumnList(IEnumerable<Field> fields, IDbSetting dbSetting) =>
            fields.Select(f => f.Name.AsQuoted(true, dbSetting)).Join(", ");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static string VariableList(int count) =>
            string.Join(", ", Enumerable.Range(0, count).Select(i => ":V" + i));

        #endregion

        #region Insert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetInsertFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var columnList = ColumnList(fields, dbSetting);

            return $"INSERT INTO {quotedTable} ({columnList}) SELECT {columnList} FROM {quotedPseudoTable} ORDER BY {quotedRowOrderColumn}";
        }

        #endregion

        #region Merge

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="returnIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetMergeFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            bool returnIdentity,
            IDbSetting dbSetting)
        {
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();
            var identityIsQualifier = identityField != null &&
                qualifierList.Any(q => string.Equals(q.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));

            if (identityIsQualifier)
            {
                return GetMergeExecuteBlockSql(tableName, pseudoTableName, fieldList, qualifierList, identityField, returnIdentity, dbSetting);
            }

            var insertableFields = identityField == null
                ? fieldList
                : fieldList.Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)).AsList();

            if (returnIdentity)
            {
                return GetUpsertLoopExecuteBlockSql(tableName, pseudoTableName, insertableFields, fieldList, qualifierList, identityField, dbSetting);
            }

            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var onClause = qualifierList.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");
            var updateableFields = fieldList.Where(f => !qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase))).AsList();
            var updateSetClause = updateableFields.Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ");
            var matchedClause = updateableFields.Count > 0 ? $"WHEN MATCHED THEN UPDATE SET {updateSetClause} " : string.Empty;
            var insertColumns = ColumnList(insertableFields, dbSetting);
            var insertValues = insertableFields.Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ");

            return string.Concat(
                "MERGE INTO ", quotedTable, " T USING ", quotedPseudoTable, " S ON (", onClause, ") ",
                matchedClause,
                "WHEN NOT MATCHED THEN INSERT (", insertColumns, ") VALUES (", insertValues, ")");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="insertableFields"></param>
        /// <param name="allFields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetUpsertLoopExecuteBlockSql(string tableName,
            string pseudoTableName,
            IList<Field> insertableFields,
            IList<Field> allFields,
            IList<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);

            var sb = new StringBuilder("EXECUTE BLOCK RETURNS (R0 TYPE OF COLUMN ")
                .Append(quotedTable).Append('.').Append(quotedIdentityColumn).Append(") AS ");

            for (var i = 0; i < insertableFields.Count; i++)
            {
                sb.Append("DECLARE VARIABLE V").Append(i).Append(" TYPE OF COLUMN ")
                    .Append(quotedPseudoTable).Append('.').Append(insertableFields[i].Name.AsQuoted(true, dbSetting)).Append("; ");
            }

            sb.Append("BEGIN FOR SELECT ").Append(ColumnList(insertableFields, dbSetting))
                .Append(" FROM ").Append(quotedPseudoTable)
                .Append(" ORDER BY ").Append(quotedRowOrderColumn)
                .Append(" INTO ").Append(VariableList(insertableFields.Count))
                .Append(" DO BEGIN UPDATE OR INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(insertableFields, dbSetting)).Append(") VALUES (")
                .Append(VariableList(insertableFields.Count)).Append(") MATCHING (")
                .Append(ColumnList(qualifiers, dbSetting)).Append(") RETURNING ")
                .Append(quotedIdentityColumn).Append(" INTO :R0; SUSPEND; END END");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="returnIdentity"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeExecuteBlockSql(string tableName,
            string pseudoTableName,
            IList<Field> fields,
            IList<Field> qualifiers,
            Field identityField,
            bool returnIdentity,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var identityIndex = fields.ToList().FindIndex(f => string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));

            var sb = new StringBuilder("EXECUTE BLOCK ");
            if (returnIdentity)
            {
                sb.Append("RETURNS (R0 TYPE OF COLUMN ").Append(quotedTable).Append('.').Append(quotedIdentityColumn).Append(") ");
            }
            sb.Append("AS ");

            for (var i = 0; i < fields.Count; i++)
            {
                sb.Append("DECLARE VARIABLE V").Append(i).Append(" TYPE OF COLUMN ")
                    .Append(quotedPseudoTable).Append('.').Append(fields[i].Name.AsQuoted(true, dbSetting)).Append("; ");
            }

            var insertableFields = fields.Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)).AsList();
            var insertableVariables = string.Join(", ", fields
                .Select((f, i) => (f, i))
                .Where(x => !string.Equals(x.f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .Select(x => ":V" + x.i));
            var returningClause = returnIdentity ? $" RETURNING {quotedIdentityColumn} INTO :R0" : string.Empty;
            // Row order only matters when correlating yielded identities back to source rows below -
            // a non-return-identity merge doesn't care what order its per-row upserts run in.
            var orderByClause = returnIdentity ? $" ORDER BY {quotedRowOrderColumn}" : string.Empty;

            sb.Append("BEGIN FOR SELECT ").Append(ColumnList(fields, dbSetting))
                .Append(" FROM ").Append(quotedPseudoTable).Append(orderByClause)
                .Append(" INTO ").Append(VariableList(fields.Count))
                .Append(" DO BEGIN IF (:V").Append(identityIndex).Append(" IS NULL OR :V").Append(identityIndex).Append(" = 0) THEN BEGIN ")
                .Append("INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(insertableFields, dbSetting)).Append(") VALUES (")
                .Append(insertableVariables).Append(')').Append(returningClause).Append("; END ")
                .Append("ELSE BEGIN ")
                .Append("UPDATE OR INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(fields, dbSetting)).Append(") VALUES (")
                .Append(VariableList(fields.Count)).Append(") MATCHING (")
                .Append(ColumnList(qualifiers, dbSetting)).Append(')').Append(returningClause).Append("; END ");

            if (returnIdentity)
            {
                sb.Append("SUSPEND; ");
            }
            sb.Append("END END");

            return sb.ToString();
        }

        #endregion

        #region Update

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
            var qualifierList = qualifiers.AsList();
            var onClause = qualifierList.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");
            var updateClause = fields.AsList()
                .Where(f => !qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");

            return $"MERGE INTO {tableName.AsQuoted(true, dbSetting)} T USING {pseudoTableName.AsQuoted(true, dbSetting)} S ON ({onClause}) WHEN MATCHED THEN UPDATE SET {updateClause}";
        }

        #endregion

        #region Delete

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
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var onClause = qualifiers.Select(f => $"{quotedTable}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");

            return $"DELETE FROM {quotedTable} WHERE EXISTS (SELECT 1 FROM {quotedPseudoTable} S WHERE {onClause})";
        }

        #endregion
    }
}
