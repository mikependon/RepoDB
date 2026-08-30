#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Builds the UPDATE half of a merge. Vertica flatly refuses to run a MERGE statement at all against a
        /// table that has an IDENTITY/AUTO_INCREMENT column - "Sequence or IDENTITY/AUTO_INCREMENT column in
        /// merge query is not supported" - regardless of whether that column appears in the SET/INSERT lists,
        /// and it has no equivalent of Firebird's EXECUTE BLOCK/PSQL for a procedural row-by-row alternative
        /// either. A merge is instead always expressed as this UPDATE ... FROM, followed by
        /// <see cref="GetMergeInsertFromPseudoTableSql"/> and, when identities need to be returned,
        /// <see cref="GetSelectIdentityAfterMergeSql"/>.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns>The UPDATE statement, or <c>null</c> when there are no non-qualifier, non-identity fields to update.</returns>
        public static string GetMergeUpdateFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var qualifierList = qualifiers.AsList();
            var updateableFields = fields.AsList()
                .Where(f => !qualifierList.Any(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)))
                .Where(f => identityField == null || !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .AsList();

            if (updateableFields.Count == 0)
            {
                return null;
            }

            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var setClause = updateableFields.Select(f => $"{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ");
            var onClause = qualifierList.Select(f => $"{quotedTable}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");

            return $"UPDATE {quotedTable} SET {setClause} FROM {quotedPseudoTable} S WHERE {onClause}";
        }

        /// <summary>
        /// Builds the INSERT half of a merge - see <see cref="GetMergeUpdateFromPseudoTableSql"/>. Only
        /// pseudo-table rows with no matching row in the target (by <paramref name="qualifiers"/>) are inserted,
        /// and the identity column (if any) is always excluded, since Vertica rejects an explicit value for it.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetMergeInsertFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var qualifierList = qualifiers.AsList();
            var insertableFields = identityField == null
                ? fields.AsList()
                : fields.AsList().Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)).AsList();
            var insertColumns = ColumnList(insertableFields, dbSetting);
            var insertSelect = insertableFields.Select(f => $"S.{f.Name.AsQuoted(true, dbSetting)}").Join(", ");
            var existsClause = qualifierList.Select(f => $"{quotedTable}.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");

            // Without an explicit ORDER BY, Vertica is free to insert the unmatched rows in whatever order its
            // projections happen to yield them, which can silently scramble row order relative to the source -
            // ordering by the pseudo table's row-order column keeps newly-inserted rows in source order.
            return $"INSERT INTO {quotedTable} ({insertColumns}) SELECT {insertSelect} FROM {quotedPseudoTable} S WHERE NOT EXISTS (SELECT 1 FROM {quotedTable} WHERE {existsClause}) ORDER BY S.{quotedRowOrderColumn}";
        }

        /// <summary>
        /// Reads back, per pseudo-table row and in source row order, the identity value of whichever target row
        /// the merge's UPDATE/INSERT left in place for it - the pre-existing value for an updated row, or the
        /// newly-generated one for an inserted row. Run this only after both <see cref="GetMergeUpdateFromPseudoTableSql"/>
        /// and <see cref="GetMergeInsertFromPseudoTableSql"/> have executed, so every pseudo-table row has a match.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pseudoTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetSelectIdentityAfterMergeSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var onClause = qualifiers.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");

            return $"SELECT T.{quotedIdentityColumn} FROM {quotedPseudoTable} S JOIN {quotedTable} T ON ({onClause}) ORDER BY S.{quotedRowOrderColumn}";
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
