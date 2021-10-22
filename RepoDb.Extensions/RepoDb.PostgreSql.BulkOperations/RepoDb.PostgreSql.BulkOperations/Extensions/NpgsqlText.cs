using RepoDb.Enumerations;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.PostgreSql.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        #region BinaryBulkInsert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetInsertCommandText(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            var key = HashCode.Combine(sourceTableName.GetHashCode(),
                destinationTableName.GetHashCode(),
                EnumerableGetHashCode(fields),
                identityField.GetHashCode(),
                identityBehavior.GetHashCode());

            // Get from cache
            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            // Eliminate the identity
            if (identityBehavior != BulkImportIdentityBehavior.KeepIdentity)
            {
                fields = fields?
                    .Where(field =>
                        !string.Equals(field.Name, identityField?.Name, System.StringComparison.OrdinalIgnoreCase));
            }

            // Build the query
            var builder = new QueryBuilder();

            // INSERT INTO
            builder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(fields, dbSetting)
                .CloseParen();

            if (identityBehavior == BulkImportIdentityBehavior.KeepIdentity)
            {
                builder
                    .WriteText("OVERRIDING SYSTEM VALUE");
            }

            builder
                .Select()
                .FieldsFrom(fields, dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .OrderByFrom(GetOderColumnOrderField().AsEnumerable(), dbSetting);

            // Return the Id
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null)
            {
                builder
                    .Returning()
                    .WriteText("-1")
                    .As("Index".AsQuoted(true, dbSetting))
                    .WriteText(", ")
                    .WriteText(identityField.Name.AsQuoted(true, dbSetting))
                    .As("Identity".AsQuoted(true, dbSetting));
            }

            // Extract the command text
            commandText = builder
                .End()
                .ToString();

            // Add to cache
            LocalCommandTextCache.Add(key, commandText, true);

            // Return
            return commandText;
        }

        #endregion

        #region BinaryBulkMerge

        /// <summary>
        /// Do the explicit UPDATE and INSERT command.
        /// Link: https://stackoverflow.com/questions/17267417/how-to-upsert-merge-insert-on-duplicate-update-in-postgresql
        /// </summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="mergeCommandType"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeCommandText(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            BulkImportMergeCommandType mergeCommandType,
            IDbSetting dbSetting)
        {
            var key = HashCode.Combine(sourceTableName.GetHashCode(),
                destinationTableName.GetHashCode(),
                EnumerableGetHashCode(fields),
                EnumerableGetHashCode(qualifiers),
                primaryField.GetHashCode(),
                identityField.GetHashCode(),
                identityBehavior.GetHashCode(),
                mergeCommandType.GetHashCode());

            // Get from cache
            var commandText = LocalCommandTextCache.Get(key);
            if (!string.IsNullOrEmpty(commandText))
            {
                return commandText;
            }

            // Compose
            commandText = mergeCommandType == BulkImportMergeCommandType.OnConflictDoUpdate ?
                GetMergeCommandTextViaOnConflictDoUpdate(sourceTableName,
                    destinationTableName,
                    fields,
                    qualifiers,
                    primaryField,
                    identityField,
                    identityBehavior,
                    dbSetting) :
                GetMergeCommandTextViaInsertAndUpdate(sourceTableName,
                    destinationTableName,
                    fields,
                    qualifiers,
                    primaryField,
                    identityField,
                    identityBehavior,
                    dbSetting);

            // Add to cache
            LocalCommandTextCache.Add(key, commandText, true);

            // Return
            return commandText;
        }

        /// <summary>
        /// Returns the following SQL Statement:
        ///     INSERT INTO "TargetTable"
        ///     OVERRIDING SYSTEM VALUE
        ///     SELECT "Columns"
        ///     FROM "SourceTable"
        ///     ON CONFLICT ("Qualifiers")
        ///     SET "Column" = EXCLUDED."Column";
        /// This failing with the following message because the qualifiers are not indexed.
        /// ERROR:  there is no unique or exclusion constraint matching the ON CONFLICT specification
        /// SQL state: 42P10
        /// Link: https://stackoverflow.com/questions/42022362/no-unique-or-exclusion-constraint-matching-the-on-conflict
        /// </summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeCommandTextViaOnConflictDoUpdate(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            // Qualifiers
            qualifiers = EnsurePrimaryAsQualifier(qualifiers, primaryField, destinationTableName);
            ThrowIfNoQualifiers(qualifiers, destinationTableName);
            ThrowOnMissingQualifiers(fields, qualifiers, dbSetting);

            // Build the query
            var builder = new QueryBuilder();

            // Insert
            builder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(fields, dbSetting)
                .CloseParen();

            if (identityBehavior == BulkImportIdentityBehavior.KeepIdentity ||
                primaryField == identityField)
            {
                builder
                    .WriteText("OVERRIDING SYSTEM VALUE");
            }

            // Select the fields
            builder
                .Select()
                .FieldsFrom(fields, dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting);

            // Return identity
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                builder
                    .OrderByFrom(GetOderColumnOrderField().AsEnumerable(), dbSetting);
            }

            // Set the qualifiers
            builder
                .OnConflict(qualifiers, dbSetting)
                .DoUpdate();

            // Set the columns
            var updatableFields = GetUpdatableFields(fields,
                qualifiers,
                primaryField,
                dbSetting);
            var setColumns = updatableFields
                .Select(field =>
                {
                    var name = field.Name.AsQuoted(true, dbSetting);
                    return $"{name} = EXCLUDED.{name}";
                })
                .Join(", ");
            builder
                .Set()
                .WriteText(setColumns);

            // Return the Id
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null)
            {
                WriteReturnIdentityResultsFromActualTable(builder,
                    sourceTableName,
                    destinationTableName,
                    qualifiers,
                    identityField,
                    dbSetting);
            }

            // Return the command text
            return builder
                .End()
                .ToString();
        }

        /// <summary>
        /// Do the explicit UPDATE and INSERT command.
        /// Link: https://stackoverflow.com/questions/17267417/how-to-upsert-merge-insert-on-duplicate-update-in-postgresql
        /// </summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeCommandTextViaInsertAndUpdate(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            // Qualifiers
            qualifiers = EnsurePrimaryAsQualifier(qualifiers, primaryField, destinationTableName);
            ThrowIfNoQualifiers(qualifiers, destinationTableName);
            ThrowOnMissingQualifiers(fields, qualifiers, dbSetting);

            // Build the query
            var builder = new QueryBuilder();

            // Create temp table
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                WriteCreateTemporaryReturnIdentityTable(builder,
                    sourceTableName,
                    destinationTableName,
                    qualifiers,
                    identityField,
                    dbSetting);

                // Create the index
                WriteCreateTemporaryReturnIdentityTableIndex(builder,
                    dbSetting);
            }

            // Update the target table (from the pseudo table)
            var updatableFields = GetUpdatableFields(
                fields,
                qualifiers,
                primaryField,
                dbSetting);
            WriteUpdateTargetTableFromPseudoTable(builder,
                sourceTableName,
                destinationTableName,
                qualifiers,
                updatableFields,
                dbSetting);

            // Insert into target table (from the pseudo table)
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                // Insert Into (via CTE)
                var insertableFields = GetInsertableFields(
                    fields,
                    identityField,
                    identityBehavior,
                    dbSetting);
                WriteInsertIntoTargetTableFromPseudoTableForMergeWithReturnIdentity(builder,
                    sourceTableName,
                    destinationTableName,
                    insertableFields,
                    qualifiers,
                    identityField,
                    dbSetting);
            }
            else
            {
                // Direct Insert Into
                WriteInsertIntoTargetTableFromPseudoTableForMerge(builder,
                    sourceTableName,
                    destinationTableName,
                    fields,
                    qualifiers,
                    identityField,
                    identityBehavior,
                    dbSetting);
            }

            // Return the identities (if needed)
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity && identityField != null)
            {
                WriteReturnIdentityResultsFromTemporaryReturnIdentityTable(builder,
                    dbSetting);
            }

            // Return the command text
            return builder
                .ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        private static void WriteCreateTemporaryReturnIdentityTable(QueryBuilder builder,
            string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var returnIdentityTableName = GetTemporaryReturnIdentityTableName().AsQuoted(true, dbSetting);
            var orderColumnOrderField = GetOderColumnOrderField();
            var orderColumnName = orderColumnOrderField.Name.AsQuoted(true, dbSetting);
            var idColumnName = identityField.Name.AsQuoted(true, dbSetting);

            // Drop if exists
            builder
                .WriteText($"DROP TABLE IF EXISTS")
                .TableNameFrom(returnIdentityTableName, dbSetting)
                .End();

            // Compose
            var commandText = @$"WITH CTE AS
(
	SELECT ROW_NUMBER() OVER(PARTITION BY S.{orderColumnName} ORDER BY T.{idColumnName} DESC) AS ""RowNumber"",
		S.{orderColumnName} AS ""Index"", T.{idColumnName} AS ""Identity""
	FROM {sourceTableName.AsQuoted(true, dbSetting)} AS S
	LEFT JOIN {destinationTableName.AsQuoted(true, dbSetting)} AS T
	ON ({qualifiers
            .Select(
                field =>
                    field.AsJoinQualifier("T", "S", true, dbSetting))
            .Join(" AND ")})
)
SELECT ""Index""
	, ""Identity""
INTO TEMPORARY {returnIdentityTableName}
FROM CTE
WHERE ""RowNumber"" = 1
ORDER BY ""Index"";";

            // Select into
            builder
                .NewLine()
                .WriteText(commandText)
                .End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dbSetting"></param>
        private static void WriteCreateTemporaryReturnIdentityTableIndex(QueryBuilder builder,
            IDbSetting dbSetting)
        {
            var returnIdentityTableName = GetTemporaryReturnIdentityTableName();
            var indexColumnName = "Index";
            var indexName = $"{returnIdentityTableName}_{indexColumnName}_IDX".AsField(dbSetting);

            // Create
            builder
                .NewLine()
                .WriteText("CREATE UNIQUE INDEX IF NOT EXISTS")
                .WriteText(indexName)
                .On()
                .TableNameFrom(returnIdentityTableName, dbSetting)
                .OpenParen()
                .WriteText(indexColumnName.AsQuoted(true, dbSetting))
                .CloseParen()
                .End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="updatableFields"></param>
        /// <param name="dbSetting"></param>
        private static void WriteUpdateTargetTableFromPseudoTable(QueryBuilder builder,
            string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> updatableFields,
            IDbSetting dbSetting)
        {
            var returnIdentityTableName = GetTemporaryReturnIdentityTableName();
            var orderColumnOrderField = GetOderColumnOrderField();
            var orderColumnName = orderColumnOrderField.Name.AsQuoted(true, dbSetting);
            var indexColumnName = "Index".AsQuoted(true, dbSetting);
            var identityColumnName = "Identity".AsQuoted(true, dbSetting);

            // Create
            builder
                .NewLine()
                .Update()
                .TableNameFrom(destinationTableName, dbSetting)
                .As("T")
                .Set()
                // ====================
                // > v1.12.9
                //.FieldsAndAliasFieldsFrom(updatableFields, string.Empty, "S", dbSetting)
                // TODO: Remove soon
                .WriteText(updatableFields
                    .Select(
                        field =>
                        {
                            var fieldName = field.Name.AsQuoted(true, true, dbSetting);
                            return string.Concat(fieldName, " = S.", fieldName);
                        })
                    .Join(", "))
                // ====================
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .As("S")
                .Where()
                .WriteText(qualifiers
                    .Select(
                        field =>
                            field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        private static void WriteInsertIntoTargetTableFromPseudoTableForMerge(QueryBuilder builder,
            string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            if (identityBehavior != BulkImportIdentityBehavior.KeepIdentity)
            {
                fields = fields?
                    .Where(field =>
                        !string.Equals(field.Name, identityField?.Name, System.StringComparison.OrdinalIgnoreCase));
            }

            // Insert
            builder
                .NewLine()
                .Insert()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(fields, dbSetting)
                .CloseParen();

            if (identityBehavior == BulkImportIdentityBehavior.KeepIdentity)
            {
                builder
                    .WriteText("OVERRIDING SYSTEM VALUE");
            }

            builder
                .Select()
                .AsAliasFieldsFrom(fields, "S", dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .As("S")
                .WriteText("LEFT JOIN")
                .TableNameFrom(destinationTableName, dbSetting)
                .As("T")
                .On()
                .WriteText(qualifiers
                    .Select(
                        field =>
                            field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .Where()
                .WriteText(
                    qualifiers
                        .Select(
                            field =>
                                $"T.{field.Name.AsQuoted(true, dbSetting)} IS NULL")
                        .Join(" AND "))
                .End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        private static void WriteInsertIntoTargetTableFromPseudoTableForMergeWithReturnIdentity(QueryBuilder builder,
            string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var returnIdentityTableName = GetTemporaryReturnIdentityTableName()
                .AsQuoted(true, dbSetting);
            var targetFields = fields
                .Select(field =>
                    field.Name.AsQuoted(true, dbSetting))
                .Join(", ");
            var sourceFields = fields
                .Select(field =>
                    $"S.{field.Name.AsQuoted(true, dbSetting)}")
                .Join(", ");
            var joinQualifiers = qualifiers
                .Select(
                    field =>
                        field.AsJoinQualifier("S", "T", true, dbSetting))
                .Join(" AND ");
            var whereQualifiers = qualifiers
                .Select(
                    field =>
                        $"T.{field.Name.AsQuoted(true, dbSetting)} IS NULL")
                .Join(" AND ");
            var identityName = identityField.Name.AsField(dbSetting);
            var orderColumnOrderField = GetOderColumnOrderField();
            var orderColumnName = orderColumnOrderField.Name.AsQuoted(true, dbSetting);

            // Quote the arguments
            destinationTableName = destinationTableName.AsQuoted(true, dbSetting);
            sourceTableName = sourceTableName.AsQuoted(true, dbSetting);

            // Compose the command text (CTEs)
            var commandText = @$"WITH CTE_Insert AS
(
	INSERT INTO {destinationTableName} ({targetFields})
	SELECT {sourceFields}
    FROM {sourceTableName} AS S
    LEFT JOIN {destinationTableName} AS T ON {joinQualifiers}
    WHERE {whereQualifiers}
    ORDER BY S.{orderColumnName} ASC
    RETURNING {identityName}
),
CTE_Destination AS
(
    SELECT ROW_NUMBER() OVER(ORDER BY {identityName}) AS ""RowNumber""
		, {identityName} AS ""Identity""
    FROM CTE_Insert
),
CTE_Source AS
(
    SELECT ROW_NUMBER() OVER(ORDER BY ""Index"") AS ""RowNumber""
		, ""Index""
    FROM {returnIdentityTableName}
    WHERE ""Identity"" IS NULL /* This is the trick */
)
INSERT INTO {returnIdentityTableName}
(
    ""Index""
    , ""Identity""
)
SELECT S.""Index""
	, T.""Identity""
FROM CTE_Destination T
INNER JOIN CTE_Source S ON S.""RowNumber"" = T.""RowNumber""
ON CONFLICT(""Index"") DO UPDATE
SET ""Identity"" = EXCLUDED.""Identity"";";

            // Write to builder
            builder
                .NewLine()
                .WriteText(commandText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dbSetting"></param>
        private static void WriteReturnIdentityResultsFromTemporaryReturnIdentityTable(QueryBuilder builder,
            IDbSetting dbSetting)
        {
            var returnIdentityTableName = GetTemporaryReturnIdentityTableName().AsQuoted(true, dbSetting);
            var rowNumberName = "RowNumber".AsQuoted(true, dbSetting);
            var indexName = "Index".AsQuoted(true, dbSetting);
            var identityName = "Identity".AsQuoted(true, dbSetting);

            // Build
            builder
                .NewLine()
                .WriteText("WITH CTE AS")
                .OpenParen()
                .Select()
                .WriteText($"ROW_NUMBER() OVER (PARTITION BY {indexName} ORDER BY {identityName} DESC) AS {rowNumberName},")
                .WriteText($"{indexName},")
                .WriteText(identityName)
                .From()
                .TableNameFrom(returnIdentityTableName, dbSetting)
                .CloseParen()
                .Select()
                .WriteText($"{indexName},")
                .WriteText(identityName)
                .From()
                .WriteText("CTE")
                .Where()
                .WriteText($"{rowNumberName} = 1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static void WriteReturnIdentityResultsFromActualTable(QueryBuilder builder,
            string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> qualifiers,
            Field identityField,
            IDbSetting dbSetting)
        {
            var orderColumnName = GetOderColumnOrderField().Name.AsQuoted(true, dbSetting);
            var indexName = "Index".AsQuoted(true, dbSetting);
            var identityName = "Identity".AsQuoted(true, dbSetting);
            var rowNumber = "RowNumber".AsQuoted(true, dbSetting);

            builder
                .End()
                .WriteText("WITH CTE AS (")
                .Select()
                .WriteText($"ROW_NUMBER() OVER (PARTITION BY S.{orderColumnName} ORDER BY T.{identityField.Name.AsQuoted(true, dbSetting)} DESC) AS {rowNumber},")
                .WriteText($"S.{orderColumnName} AS {indexName},")
                .WriteText($"T.{identityField.Name.AsQuoted(true, dbSetting)} AS {identityName}")
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .WriteText("AS S")
                .WriteText("LEFT JOIN")
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("AS T")
                .On()
                .WriteText(qualifiers
                    .Select(
                        field =>
                            field.AsJoinQualifier("S", "T", true, dbSetting))
                    .Join(" AND "))
                .WriteText(")")
                .Select()
                .WriteText($"{indexName},")
                .WriteText(identityName)
                .From()
                .WriteText("CTE")
                .Where()
                .WriteText(string.Concat(rowNumber, " = 1"));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBinaryInsertPseudoTableName(string tableName,
            IDbSetting dbSetting) =>
            $"_RepoDb_BinaryBulkInsert_{tableName.AsUnquoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBinaryMergePseudoTableName(string tableName,
            IDbSetting dbSetting) =>
            $"_RepoDb_BinaryBulkMerge_{tableName.AsUnquoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetTemporaryReturnIdentityTableName() =>
            "_RepoDb_ReturnIdentity";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static OrderField GetOderColumnOrderField() =>
            new OrderField("__RepoDb_OrderColumn", Order.Ascending);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="tableName"></param>
        private static void ThrowIfNoQualifiers(IEnumerable<Field> qualifiers,
            string tableName)
        {
            if (qualifiers?.Any() != true)
            {
                throw new InvalidOperationException("The qualifier fields are not defined. To rectify this problem, ensure to pass the list of qualifier fields, " +
                    $"or, ensure that the target table '{tableName}' has a primary/identity column.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        private static void ThrowOnMissingQualifiers(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting)
        {
            var missingQualifiers = GetMissingQualifiers(fields, qualifiers, dbSetting);

            if (missingQualifiers?.Any() == true)
            {
                throw new InvalidOperationException($"The qualifiers '{missingQualifiers.Select(field => field.Name).Join(", ")}' are not found from the list of fields.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetMissingQualifiers(IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            IDbSetting dbSetting) =>
            qualifiers?
                .Where(qualifier =>
                    fields?.FirstOrDefault(field => field == qualifier ||
                        string.Equals(field.Name.AsQuoted(true, dbSetting), qualifier.Name.AsQuoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) == null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="destinationTableName"></param>
        /// <returns></returns>
        private static IEnumerable<Field> EnsurePrimaryAsQualifier(IEnumerable<Field> qualifiers,
            Field primaryField,
            string destinationTableName)
        {
            if (qualifiers?.Any() != true && primaryField != null)
            {
                qualifiers = primaryField.AsEnumerable();
            }

            return qualifiers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetInsertableFields(IEnumerable<Field> fields,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting) =>
            fields?
                .Where(field =>
                {
                    var isIdentity = string.Equals(identityField?.Name.AsQuoted(true, dbSetting), field.Name.AsQuoted(true, dbSetting), StringComparison.OrdinalIgnoreCase);
                    return (isIdentity == false) ||
                        (isIdentity && identityBehavior == BulkImportIdentityBehavior.KeepIdentity);
                });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="qualfiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetUpdatableFields(IEnumerable<Field> fields,
            IEnumerable<Field> qualfiers,
            Field primaryField,
            IDbSetting dbSetting) =>
            fields?
                .Where(field =>
                    !string.Equals(primaryField?.Name.AsQuoted(true, dbSetting), field.Name.AsQuoted(true, dbSetting), StringComparison.OrdinalIgnoreCase))
                .Where(field =>
                    qualfiers?.FirstOrDefault(qualifier => qualifier == field ||
                        string.Equals(qualifier.Name.AsQuoted(true, dbSetting), field.Name.AsQuoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) == null);

        #endregion
    }
}
