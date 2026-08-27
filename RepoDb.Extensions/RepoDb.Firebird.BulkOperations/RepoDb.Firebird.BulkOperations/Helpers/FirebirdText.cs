using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal static class FirebirdText
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetColumnTypeSql(DbField field)
        {
            var precision = field.Precision ?? 18;
            var scale = field.Scale ?? 0;
            var size = field.Size ?? 1;

            return field.DatabaseType?.ToLowerInvariant() switch
            {
                "smallint" => "SMALLINT",
                "integer" => "INTEGER",
                "bigint" => "BIGINT",
                "boolean" => "BOOLEAN",
                "float" => "FLOAT",
                "double precision" => "DOUBLE PRECISION",
                "date" => "DATE",
                "time" => "TIME",
                "time_tz" => "TIME WITH TIME ZONE",
                "timestamp" => "TIMESTAMP",
                "timestamp_tz" => "TIMESTAMP WITH TIME ZONE",
                "numeric" => $"NUMERIC({precision},{scale})",
                "decimal" => $"DECIMAL({precision},{scale})",
                "dec16" => "DECFLOAT(16)",
                "dec34" => "DECFLOAT(34)",
                "int128" => "INT128",
                "char" => $"CHAR({size})",
                "varchar" => $"VARCHAR({size})",
                "binary" => $"CHAR({size}) CHARACTER SET OCTETS",
                "varbinary" => $"VARCHAR({size}) CHARACTER SET OCTETS",
                "blob_binary" => "BLOB SUB_TYPE 0",
                _ => "BLOB SUB_TYPE TEXT", // blob_text, and a safe catch-all for anything unrecognized.
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
            FirebirdBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting)
        {
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);

            var columnDefinitions = fields
                .Select(f => $"{f.Name.AsQuoted(true, dbSetting)} {GetColumnTypeSql(dbFields.GetByUnquotedName(f.Name.AsUnquoted(true, dbSetting)))}")
                .Join(", ");

            var isMemory = pseudoTableType == FirebirdBulkImportPseudoTableType.Memory;
            var tableKind = isMemory ? "GLOBAL TEMPORARY TABLE" : "TABLE";
            var onCommitClause = isMemory ? " ON COMMIT PRESERVE ROWS" : string.Empty;
            return $"CREATE {tableKind} {quotedPseudoTableName} ({columnDefinitions}, {quotedRowOrderColumn} BIGINT DEFAULT 0 NOT NULL){onCommitClause}";
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
            var quotedIndexName = ("IX" + pseudoTableName).AsQuoted(true, dbSetting);
            var quotedPseudoTableName = pseudoTableName.AsQuoted(true, dbSetting);
            var columnList = qualifiers.Select(f => f.Name.AsQuoted(true, dbSetting)).Join(", ");

            return $"CREATE INDEX {quotedIndexName} ON {quotedPseudoTableName} ({columnList})";
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
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static string GetInsertFromPseudoTableForReturnIdentitySql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IDbSetting dbSetting)
        {
            var quotedTable = tableName.AsQuoted(true, dbSetting);
            var quotedPseudoTable = pseudoTableName.AsQuoted(true, dbSetting);
            var quotedRowOrderColumn = RowOrderColumnName.AsQuoted(true, dbSetting);
            var quotedIdentityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var fieldList = fields.AsList();

            var sb = new StringBuilder("EXECUTE BLOCK RETURNS (R0 TYPE OF COLUMN ")
                .Append(quotedTable).Append('.').Append(quotedIdentityColumn).Append(") AS ");

            for (var i = 0; i < fieldList.Count; i++)
            {
                sb.Append("DECLARE VARIABLE V").Append(i).Append(" TYPE OF COLUMN ")
                    .Append(quotedPseudoTable).Append('.').Append(fieldList[i].Name.AsQuoted(true, dbSetting)).Append("; ");
            }

            sb.Append("BEGIN FOR SELECT ").Append(ColumnList(fieldList, dbSetting))
                .Append(" FROM ").Append(quotedPseudoTable)
                .Append(" ORDER BY ").Append(quotedRowOrderColumn)
                .Append(" INTO ").Append(VariableList(fieldList.Count))
                .Append(" DO BEGIN INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(fieldList, dbSetting)).Append(") VALUES (")
                .Append(VariableList(fieldList.Count)).Append(") RETURNING ")
                .Append(quotedIdentityColumn).Append(" INTO :R0; SUSPEND; END END");

            return sb.ToString();
        }

        #endregion

        #region Merge

        /// <summary>
        /// Builds the statement(s) used to apply a pseudo table's staged rows onto <paramref name="tableName"/>
        /// as an upsert. Three shapes, depending on whether the identity column is itself a merge qualifier
        /// (see the remarks on <c>FirebirdStatementBuilder.CreateMerge</c> for why that case needs special
        /// handling - a plain <c>MATCHING</c>/<c>ON</c> clause can't tell "match this literal 0/null" apart
        /// from "auto-generate me"):
        /// <list type="bullet">
        /// <item><description>Identity is not a qualifier, no return-identity: a single ANSI <c>MERGE INTO ...
        /// USING ... WHEN MATCHED ... WHEN NOT MATCHED ...</c> statement - one round trip for every row.</description></item>
        /// <item><description>Identity is not a qualifier, with return-identity: an <c>EXECUTE BLOCK</c> loop
        /// of plain <c>UPDATE OR INSERT ... MATCHING ... RETURNING</c> calls, one per row.</description></item>
        /// <item><description>Identity is a qualifier (with or without return-identity): an <c>EXECUTE BLOCK</c>
        /// loop that branches per row between a plain <c>INSERT</c> (identity null/0 - auto-generate) and
        /// <c>UPDATE OR INSERT ... MATCHING</c> (a real identity value - match-or-insert-with-that-id).</description></item>
        /// </list>
        /// </summary>
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
        /// The simple (identity-not-a-qualifier) return-identity case: one <c>UPDATE OR INSERT ...
        /// MATCHING ... RETURNING</c> per pseudo-table row, looped via <c>EXECUTE BLOCK</c>.
        /// </summary>
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
        /// The identity-as-qualifier case: per pseudo-table row, branch between a plain <c>INSERT</c>
        /// (identity null/0) and <c>UPDATE OR INSERT ... MATCHING</c> (a real identity value), mirroring
        /// <c>FirebirdStatementBuilder.BuildMergeExecuteBlock</c>'s single-row version of the same logic.
        /// <c>RETURNS</c>/<c>SUSPEND</c> are only emitted when <paramref name="returnIdentity"/> is set - a
        /// bulk merge that doesn't need identities back skips them, since a plain procedural <c>EXECUTE
        /// BLOCK</c> (no result set) is a valid, slightly cheaper Firebird construct.
        /// </summary>
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
        /// A plain ANSI <c>MERGE ... WHEN MATCHED THEN UPDATE</c> against the pseudo table - there is no
        /// <c>WHEN NOT MATCHED</c> branch, so the identity-as-qualifier ambiguity that <see cref="GetMergeFromPseudoTableSql"/>
        /// has to work around never applies here (nothing is ever inserted).
        /// </summary>
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
        public static string GetDeleteFromPseudoTableSql(string tableName,
            string pseudoTableName,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var onClause = qualifiers.Select(f => $"T.{f.Name.AsQuoted(true, dbSetting)} = S.{f.Name.AsQuoted(true, dbSetting)}").Join(" AND ");

            return $"DELETE FROM {tableName.AsQuoted(true, dbSetting)} T WHERE EXISTS (SELECT 1 FROM {pseudoTableName.AsQuoted(true, dbSetting)} S WHERE {onClause})";
        }

        #endregion
    }
}
