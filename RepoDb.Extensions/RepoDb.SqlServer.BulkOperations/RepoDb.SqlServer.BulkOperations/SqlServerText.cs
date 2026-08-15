using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// Contains the SQL text builders for the bulk DML statements (DELETE/INSERT/MERGE/UPDATE) executed
    /// against the temporary/pseudo table. Kept separate from <see cref="SqlConnectionExtension"/> so the
    /// pure text-building concern stays isolated, mirroring the PostgreSQL project's NpgsqlText.cs.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region GetBulkDeleteSqlText

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="qualifiers"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBulkDeleteSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> qualifiers,
            string hints,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Compose the statement
            builder
                .Clear()
                .Delete()
                .WriteText("T")
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .HintsFrom(hints)
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .End();

            // Return the sql
            return builder.ToString();
        }

        #endregion

        #region GetBulkInsertSqlText

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="forceIdentityColumn"></param>
        /// <returns></returns>
        private static string GetBulkInsertSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            Field identityField,
            string hints,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            bool forceIdentityColumn)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => forceIdentityColumn == true || string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Compose the statement
            builder.Clear();

            // SET IDENTITY_INSERT ON
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("ON;");
            }

            builder
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .Top()
                .WriteText("100 PERCENT")
                //.FieldsFrom(fields, dbSetting)
                .WriteText("*") // Including the [__RepoDb_OrderColumn]
                .From()
                .TableNameFrom(tempTableName, dbSetting);

            // Return Identity
            if (isReturnIdentity && identityField != null)
            {
                builder
                    .OrderBy()
                    .WriteText("[__RepoDb_OrderColumn]")
                    .Ascending();
            }

            // Continuation
            builder
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText("1 = 0")
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
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

            // Set the output
            if (isReturnIdentity == true && identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                        .As("[Result],")
                    .WriteText("S.[__RepoDb_OrderColumn]")
                        .As("[OrderColumn]");
            }

            // End
            builder.End();

            // SET IDENTITY_INSERT OFF (probably not necessary, but it won't hurt)
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("OFF;");
            }

            // Return the sql
            return builder.ToString();
        }

        #endregion

        #region GetBulkMergeSqlText

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="forceIdentityColumn"></param>
        /// <returns></returns>
        private static string GetBulkMergeSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            string hints,
            IDbSetting dbSetting,
            bool isReturnIdentity,
            bool forceIdentityColumn)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Insertable fields
            var insertableFields = fields
                .Where(field => forceIdentityColumn == true || string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) == false);

            // Updatable fields
            var updateableFields = fields
                .Where(field => field != identityField && field != primaryField)
                .Where(field =>
                    qualifiers.Any(
                        q => string.Equals(q.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

            // Compose the statement
            builder.Clear();

            // SET IDENTITY_INSERT ON
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("ON;");
            }

            builder
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .Top()
                .WriteText("100 PERCENT")
                //.FieldsFrom(fields, dbSetting)
                .WriteText("*") // Including the [__RepoDb_OrderColumn]
                .From()
                .TableNameFrom(tempTableName, dbSetting);

            // Return Identity
            if (isReturnIdentity && identityField != null)
            {
                builder
                    .OrderBy()
                    .WriteText("[__RepoDb_OrderColumn]")
                    .Ascending();
            }

            // Continuation
            builder
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .CloseParen();

            if (insertableFields?.Any() == true)
            {
                // WHEN NOT MATCHED THEN INSERT VALUES
                builder
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
            }

            if (updateableFields?.Any() == true)
            {
                // WHEN MATCHED THEN UPDATE SET
                builder
                    .When()
                    .Matched()
                    .Then()
                    .Update()
                    .Set()
                    .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", dbSetting);
            }

            // Set the output
            if (isReturnIdentity == true && identityField != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", identityField.Name.AsField(dbSetting)))
                        .As("[Result],")
                    .WriteText("S.[__RepoDb_OrderColumn]")
                        .As("[OrderColumn]");
            }

            // End the builder
            builder.End();

            // SET IDENTITY_INSERT OFF (probably not necessary, but it won't hurt)
            if (forceIdentityColumn)
            {
                builder
                    .WriteText("SET IDENTITY_INSERT")
                    .TableNameFrom(tableName, dbSetting)
                    .WriteText("OFF;");
            }

            // Return the sql
            return builder.ToString();
        }

        #endregion

        #region GetBulkUpdateSqlText

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBulkUpdateSqlText(string tableName,
            string tempTableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            Field primaryField,
            Field identityField,
            string hints,
            IDbSetting dbSetting)
        {
            // Validate the presence
            if (fields?.Any() != true)
            {
                throw new MissingFieldException("There are no field(s) defined.");
            }

            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldException("There is no qualifier field(s) defined.");
            }

            // Variables needed
            var builder = new QueryBuilder();

            // Updatable fields
            var updateableFields = fields
                .Where(field => field != identityField && field != primaryField)
                .Where(field =>
                    qualifiers.Any(
                        q => string.Equals(q.Name, field.Name, StringComparison.OrdinalIgnoreCase)) == false);

            // Compose the statement
            builder
                .Clear()
                .Update()
                .WriteText("T")
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", dbSetting)
                .From()
                .TableNameFrom(tableName, dbSetting)
                .WriteText("T")
                .HintsFrom(hints)
                .WriteText("INNER JOIN")
                .TableNameFrom(tempTableName, dbSetting)
                .WriteText("S")
                .WriteText("ON")
                .WriteText(qualifiers
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, dbSetting))
                            .Join(" AND "))
                .End();

            // Return the sql
            return builder.ToString();
        }

        #endregion
    }
}
