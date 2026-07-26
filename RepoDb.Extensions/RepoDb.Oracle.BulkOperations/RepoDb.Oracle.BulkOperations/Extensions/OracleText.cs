using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// All SQL command-text generation for the Oracle bulk operations - the equivalent of the PostgreSQL
    /// bulk package's <c>NpgsqlText.cs</c>, consolidated into one file per this package's leaner layout.
    /// Every method here builds text only; nothing here talks to a connection.
    /// </summary>
    internal static class OracleText
    {
        #region Staging Table DDL/DML

        /// <summary>
        /// Either <c>CREATE GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS AS SELECT ... WHERE 1 = 0</c>
        /// (<see cref="OracleBulkImportPseudoTableType.Temporary"/>) or a plain
        /// <c>CREATE TABLE ... AS SELECT ... WHERE 1 = 0</c> (<see cref="OracleBulkImportPseudoTableType.Physical"/>).
        /// Either way, CTAS mirrors the real table's column set exactly (no per-column DDL needs to be
        /// hand-built from <see cref="DbField"/> metadata) plus a leading <c>__RepoDb_OrderColumn</c> used
        /// to correlate staged rows back to their original position in the caller's input sequence.
        /// </summary>
        public static string GetCreateStagingTableCommandText(string tableName,
            string stagingTableName,
            DbFieldCollection dbFields,
            OracleBulkImportPseudoTableType pseudoTableType,
            IDbSetting dbSetting)
        {
            var columns = dbFields.GetItems()
                .Select(field => field.Name.AsQuoted(true, dbSetting))
                .Join(", ");

            var createClause = pseudoTableType == OracleBulkImportPseudoTableType.Physical ?
                "CREATE TABLE " :
                "CREATE GLOBAL TEMPORARY TABLE ";
            var onCommitClause = pseudoTableType == OracleBulkImportPseudoTableType.Physical ?
                string.Empty :
                " ON COMMIT PRESERVE ROWS";

            return string.Concat(createClause, stagingTableName.AsQuoted(true, dbSetting), onCommitClause,
                " AS SELECT 0 AS ", OracleStagingTable.OrderColumnName.AsQuoted(true, dbSetting),
                ", ", columns, " FROM ", tableName.AsQuoted(true, dbSetting), " WHERE 1 = 0");
        }

        /// <summary>
        /// <c>INSERT INTO staging (cols[, __RepoDb_OrderColumn]) VALUES (:cols[, :__RepoDb_OrderColumn])</c>,
        /// used to array-bind-load rows into the staging table ahead of BulkMerge/BulkUpdate/BulkDelete.
        /// </summary>
        public static string GetStagingInsertCommandText(string stagingTableName,
            IEnumerable<Field> fields,
            bool includeOrderColumn,
            IDbSetting dbSetting)
        {
            var allFields = includeOrderColumn ?
                fields.Concat(new Field(OracleStagingTable.OrderColumnName).AsEnumerable()) :
                fields;

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(stagingTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(allFields, dbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(allFields, 0, dbSetting)
                .CloseParen();

            // Deliberately no .End() - Oracle rejects a trailing " ;" on a plain (non-PL/SQL-block)
            // statement with ORA-00911.
            return builder.GetString();
        }

        #endregion

        #region BulkInsert (direct, no staging table)

        /// <summary>
        /// <c>INSERT INTO real (cols) VALUES (:cols) [RETURNING identityCol INTO :__out_identity]</c>.
        /// Executed with array binding (see <see cref="OracleStagingTable.ExecuteArrayBind"/>) directly
        /// against the real table - unlike every other bulk operation here, BulkInsert never needs a
        /// staging table: ODP.NET's array-bound RETURNING ... INTO clause already returns one identity
        /// value per bound row, in row order, in a single round trip.
        /// </summary>
        public static string GetInsertCommandText(string tableName,
            IEnumerable<Field> fields,
            Field identityField,
            OracleBulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            var key = HashCode.Combine("Insert".GetHashCode(),
                tableName.GetHashCode(),
                OracleHelpers.EnumerableGetHashCode(fields),
                identityField?.GetHashCode(),
                identityBehavior);

            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            if (identityBehavior != OracleBulkImportIdentityBehavior.KeepIdentity)
            {
                fields = fields?.Where(field =>
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            }

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, dbSetting)
                .OpenParen()
                .FieldsFrom(fields, dbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields, 0, dbSetting)
                .CloseParen();

            if (identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null)
            {
                builder
                    .WriteText("RETURNING")
                    .WriteText(identityField.Name.AsQuoted(true, dbSetting))
                    .WriteText("INTO")
                    .WriteText(OracleStagingTable.ReturningParameterName.AsParameter(dbSetting));
            }

            // Deliberately no .End() - see the comment in GetStagingInsertCommandText.
            commandText = builder.GetString();

            LocalCommandTextCache.Add(key, commandText, true);

            return commandText;
        }

        #endregion

        #region BulkMerge

        /// <summary>
        /// <c>MERGE INTO real T USING staging S ON (qualifiers) WHEN MATCHED THEN UPDATE ... WHEN NOT
        /// MATCHED THEN INSERT ...</c>. No RETURNING clause - Oracle only supports RETURNING on MERGE
        /// starting with 23ai (see the caveat already documented on <c>OracleStatementBuilder.CreateMerge</c>
        /// in the core Oracle provider) - so when identities are requested, a separate, version-independent
        /// correlated lookup (<see cref="GetMergeIdentityLookupCommandText"/>) runs immediately afterwards
        /// instead.
        /// </summary>
        public static string GetMergeCommandText(string destinationTableName,
            string stagingTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            OracleBulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            qualifiers = ResolveQualifiers(qualifiers, primaryField);
            ThrowIfNoQualifiers(qualifiers, destinationTableName);
            ThrowOnMissingQualifiers(fields, qualifiers, dbSetting);

            var key = HashCode.Combine("Merge".GetHashCode(),
                HashCode.Combine(destinationTableName.GetHashCode(),
                    OracleHelpers.EnumerableGetHashCode(fields),
                    OracleHelpers.EnumerableGetHashCode(qualifiers),
                    identityField?.GetHashCode(),
                    identityBehavior));

            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            var updatableFields = GetUpdatableFields(fields, qualifiers, primaryField);
            var insertableFields = GetInsertableFields(fields, identityField, identityBehavior);

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Merge()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("T")
                .Using()
                .TableNameFrom(stagingTableName, dbSetting)
                .WriteText("S")
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(field => field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .CloseParen()
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updatableFields, "T", "S", dbSetting)
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, dbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", dbSetting)
                .CloseParen();

            // Deliberately no .End() - see the comment in GetStagingInsertCommandText.
            commandText = builder.GetString();

            LocalCommandTextCache.Add(key, commandText, true);

            return commandText;
        }

        /// <summary>
        /// The post-MERGE identity correlation query - mirrors the PostgreSQL package's
        /// <c>WriteReturnIdentityResultsFromActualTable</c> approach exactly, just in Oracle syntax: for
        /// every staged row, find the (now inserted-or-updated) matching real-table row via the same
        /// qualifiers the MERGE itself used, and report back its identity value against the row's original
        /// input-order index. Works regardless of Oracle version since it never relies on RETURNING.
        /// </summary>
        public static string GetMergeIdentityLookupCommandText(string destinationTableName,
            string stagingTableName,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var orderColumn = OracleStagingTable.OrderColumnName.AsQuoted(true, dbSetting);
            var identityColumn = identityField.Name.AsQuoted(true, dbSetting);
            var indexAlias = "Index".AsQuoted(true, dbSetting);
            var identityAlias = "Identity".AsQuoted(true, dbSetting);
            var rowNumberAlias = "RowNumber".AsQuoted(true, dbSetting);

            var joinQualifiers = qualifiers
                .Select(field => field.AsJoinQualifier("S", "T", true, dbSetting))
                .Join(" AND ");

            return
                $"SELECT {indexAlias}, {identityAlias} FROM (" +
                $"SELECT ROW_NUMBER() OVER (PARTITION BY S.{orderColumn} ORDER BY T.{identityColumn} DESC) AS {rowNumberAlias}, " +
                $"S.{orderColumn} AS {indexAlias}, T.{identityColumn} AS {identityAlias} " +
                $"FROM {stagingTableName.AsQuoted(true, dbSetting)} S " +
                $"LEFT JOIN {destinationTableName.AsQuoted(true, dbSetting)} T ON ({joinQualifiers})" +
                $") WHERE {rowNumberAlias} = 1 ORDER BY {indexAlias}";
        }

        #endregion

        #region BulkUpdate

        /// <summary>
        /// <c>MERGE INTO real T USING staging S ON (qualifiers) WHEN MATCHED THEN UPDATE ...</c> - a
        /// MERGE with only the UPDATE branch. Oracle has no <c>UPDATE ... FROM</c>; a MATCHED-only MERGE is
        /// the idiomatic Oracle equivalent of a joined bulk update, and is valid syntax on its own (the
        /// NOT MATCHED / INSERT branch is optional). No identity handling - RepoDb's own BulkUpdate never
        /// generates or matches new identities, only PostgreSQL's own bulk package exposes the same
        /// restriction for this operation.
        /// </summary>
        public static string GetUpdateCommandText(string destinationTableName,
            string stagingTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            IDbSetting dbSetting)
        {
            qualifiers = ResolveQualifiers(qualifiers, primaryField);
            ThrowIfNoQualifiers(qualifiers, destinationTableName);
            ThrowOnMissingQualifiers(fields, qualifiers, dbSetting);

            var key = HashCode.Combine("Update".GetHashCode(),
                destinationTableName.GetHashCode(),
                OracleHelpers.EnumerableGetHashCode(fields),
                OracleHelpers.EnumerableGetHashCode(qualifiers));

            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            var updatableFields = GetUpdatableFields(fields, qualifiers, primaryField);

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Merge()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("T")
                .Using()
                .TableNameFrom(stagingTableName, dbSetting)
                .WriteText("S")
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(field => field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .CloseParen()
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updatableFields, "T", "S", dbSetting);

            // Deliberately no .End() - see the comment in GetStagingInsertCommandText.
            commandText = builder.GetString();

            LocalCommandTextCache.Add(key, commandText, true);

            return commandText;
        }

        #endregion

        #region BulkDelete

        /// <summary>
        /// <c>DELETE FROM real T WHERE EXISTS (SELECT 1 FROM staging S WHERE qualifiers)</c>.
        /// </summary>
        public static string GetDeleteCommandText(string destinationTableName,
            string stagingTableName,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            IDbSetting dbSetting)
        {
            qualifiers = ResolveQualifiers(qualifiers, primaryField);
            ThrowIfNoQualifiers(qualifiers, destinationTableName);

            var key = HashCode.Combine("Delete".GetHashCode(),
                destinationTableName.GetHashCode(),
                OracleHelpers.EnumerableGetHashCode(qualifiers));

            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("T")
                .Where()
                .WriteText("EXISTS")
                .OpenParen()
                .Select()
                .WriteText("1")
                .From()
                .TableNameFrom(stagingTableName, dbSetting)
                .WriteText("S")
                .Where()
                .WriteText(qualifiers
                    .Select(field => field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .CloseParen();

            // Deliberately no .End() - see the comment in GetStagingInsertCommandText.
            commandText = builder.GetString();

            LocalCommandTextCache.Add(key, commandText, true);

            return commandText;
        }

        #endregion

        #region Field Set Helpers

        /// <summary>
        /// The alphanumeric, unquoted bind-variable name ODP.NET expects for a given field - must match
        /// exactly what <see cref="QueryBuilder.ParametersFrom(IEnumerable{Field}, int, IDbSetting)"/>
        /// generates in the command text, since <see cref="OracleStagingTable"/> constructs its
        /// <see cref="OracleParameter"/> objects independently of the text builder.
        /// </summary>
        public static string GetParameterName(Field field,
            IDbSetting dbSetting) =>
            field.Name.AsUnquoted(true, dbSetting).AsAlphaNumeric();

        private static IEnumerable<Field> GetUpdatableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField) =>
            fields?
                .Where(field => !string.Equals(primaryField?.Name, field.Name, StringComparison.OrdinalIgnoreCase))
                .Where(field => qualifiers?.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)) != true);

        private static IEnumerable<Field> GetInsertableFields(IEnumerable<Field> fields,
            Field identityField,
            OracleBulkImportIdentityBehavior identityBehavior) =>
            fields?
                .Where(field =>
                {
                    var isIdentity = string.Equals(identityField?.Name, field.Name, StringComparison.OrdinalIgnoreCase);
                    return isIdentity == false || identityBehavior == OracleBulkImportIdentityBehavior.KeepIdentity;
                });

        /// <summary>
        /// Falls back to the primary key as the sole qualifier when the caller did not specify any -
        /// exposed publicly since callers in <c>Base/*.cs</c> need the exact same resolved qualifier list
        /// used to build the MERGE/DELETE text when they subsequently build the post-MERGE identity lookup.
        /// </summary>
        public static IEnumerable<Field> ResolveQualifiers(IEnumerable<Field> qualifiers,
            Field primaryField)
        {
            if (qualifiers?.Any() != true && primaryField != null)
            {
                qualifiers = primaryField.AsEnumerable();
            }

            return qualifiers;
        }

        private static void ThrowIfNoQualifiers(IEnumerable<Field> qualifiers,
            string tableName)
        {
            if (qualifiers?.Any() != true)
            {
                throw new InvalidOperationException(
                    "The qualifier fields are not defined. To rectify this problem, ensure to pass the list of qualifier fields, " +
                    $"or, ensure that the target table '{tableName}' has a primary/identity column.");
            }
        }

        private static void ThrowOnMissingQualifiers(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var missing = qualifiers?
                .Where(qualifier => fields?.Any(field => string.Equals(field.Name, qualifier.Name, StringComparison.OrdinalIgnoreCase)) != true);

            if (missing?.Any() == true)
            {
                throw new InvalidOperationException(
                    $"The qualifiers '{missing.Select(field => field.Name).Join(", ")}' are not found from the list of fields.");
            }
        }

        #endregion
    }
}
