using RepoDb.Enumerations;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
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
            if (identityField != null)
            {
                fields = fields
                    .Where(field =>
                        !string.Equals(field.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase));
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
                .CloseParen()
                .WriteText("OVERRIDING SYSTEM VALUE")
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
                    .WriteText(identityField.Name.AsQuoted(true, dbSetting));
            }

            // Return the command text
            return builder
                .End()
                .ToString();
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
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetMergeCommandText(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            // Ensure the qualifiers
            qualifiers = EnsureQualifiers(qualifiers, primaryField, destinationTableName);

            // Validate the qualifiers
            ThrowOnMissingQualifiers(fields, qualifiers, dbSetting);

            // Build the query
            var builder = new QueryBuilder();

            // Update
            var updatableFields = GetUpdatableFields(
                fields,
                qualifiers,
                primaryField,
                dbSetting);
            builder
                .Clear()
                .Update()
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("AS T")
                .Set()
                .FieldsAndAliasFieldsFrom(updatableFields, string.Empty, "S", dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .WriteText("AS S")
                .Where()
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .End();

            // Insert
            var insertableFields = GetInsertableFields(fields,
                identityField,
                identityBehavior,
                dbSetting);
            builder
                .Insert()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(insertableFields, dbSetting)
                .CloseParen();

            if (identityBehavior == BulkImportIdentityBehavior.KeepIdentity)
            {
                builder
                    .WriteText("OVERRIDING SYSTEM VALUE");
            }

            builder
                .Select()
                .AsAliasFieldsFrom(insertableFields, "S", dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .WriteText("AS S")
                .WriteText("LEFT JOIN")
                .TableNameFrom(destinationTableName, dbSetting)
                .WriteText("AS T")
                .On()
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .Where()
                .WriteText(qualifiers
                    .Select(
                        field => string.Concat("T.", field.Name.AsQuoted(true, true, dbSetting), " IS NULL"))
                            .Join(" AND "));

            // Return the Id
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                builder
                    .OrderByFrom(GetOderColumnOrderField().AsEnumerable(), dbSetting);

                if (identityField != null)
                {
                    builder
                        .Returning()
                        .WriteText(identityField.Name.AsQuoted(true, dbSetting));
                }
            }

            // Return the command text
            return builder
                .End()
                .ToString();
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
        [Obsolete]
        private static string GetMergeCommandTextViaOnConflictDoUpdate(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            BulkImportIdentityBehavior identityBehavior,
            IDbSetting dbSetting)
        {
            // Ensure the qualifiers
            qualifiers = EnsureQualifiers(qualifiers, primaryField, destinationTableName);

            // Validate the qualifiers
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
                .CloseParen()
                .WriteText("OVERRIDING SYSTEM VALUE");

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
            if (identityBehavior == BulkImportIdentityBehavior.ReturnIdentity)
            {
                builder
                    .OrderByFrom(GetOderColumnOrderField().AsEnumerable(), dbSetting);

                if (identityField != null)
                {
                    builder
                        .Returning()
                        .WriteText(identityField.Name.AsQuoted(true, dbSetting));
                }
            }

            // Return the command text
            return builder
                .End()
                .ToString();
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
        private static IEnumerable<Field> EnsureQualifiers(IEnumerable<Field> qualifiers,
            Field primaryField,
            string destinationTableName)
        {
            if (qualifiers?.Any() != true && primaryField != null)
            {
                qualifiers = primaryField.AsEnumerable();
            }

            ThrowIfNoQualifiers(qualifiers, destinationTableName);

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
