using RepoDb.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Exceptions;
using RepoDb.Resolvers;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    public sealed class SqlServerStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerStatementBuilder"/> object.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        public SqlServerStatementBuilder(IDbSetting dbSetting)
            : this(dbSetting,
                new SqlServerConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public SqlServerStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  (convertFieldResolver ?? new SqlServerConvertFieldResolver()),
                  (averageableClientTypeResolver ?? new ClientTypeToAverageableClientTypeResolver()))
        { }

        #region CreateBatchQuery

        /// <summary>
        /// Creates a SQL Statement for batch query operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public override string CreateBatchQuery(string tableName,
            IEnumerable<Field> fields,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new MissingFieldsException(fields?.Select(f => f.Name));
            }

            // Validate order by
            if (orderBy == null || orderBy.Any() != true)
            {
                throw new EmptyException("The argument 'orderBy' is required.");
            }

            // Validate the page
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "The page must be equals or greater than 0.");
            }

            // Validate the page
            if (rowsPerBatch < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowsPerBatch), "The rows per batch must be equals or greater than 1.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .WriteText(string.Concat("OFFSET ", page * rowsPerBatch))
                .WriteText(string.Concat("ROWS FETCH NEXT " + rowsPerBatch + " ROWS ONLY"))
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for count operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count operation.</returns>
        public override string CreateCount(string tableName,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .CountBig(null, DbSetting)
                .WriteText("AS [CountValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public override string CreateCountAll(string tableName,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .CountBig(null, DbSetting)
                .WriteText("AS [CountValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsert(string tableName,
            IEnumerable<Field> fields = null,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Initialize the builder
            var builder = new QueryBuilder();

            // Call the base
            builder
                .WriteText(base.CreateInsert(tableName,
                    fields,
                    primaryField,
                    identityField,
                    hints));

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var returnValue = "NULL";

            // Set the return value
            if (keyColumn != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(keyColumn.Type);
                string databaseType = null;

                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }

                var keyColumnText = (keyColumn == identityField || string.Equals(keyColumn.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase)) ? "SCOPE_IDENTITY()" :
                    keyColumn.Name.AsParameter(DbSetting);
                returnValue = keyColumn != null ?
                    string.IsNullOrWhiteSpace(databaseType) ?
                    keyColumnText : string.Concat("CONVERT(", databaseType, ", ", keyColumnText, ")") : "NULL";
            }
            builder
                .Select()
                .WriteText(returnValue)
                .As("[Result]")
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for insert-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsertAll(string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Call the base
            var commandText = base.CreateInsertAll(tableName,
                fields,
                batchSize,
                primaryField,
                identityField,
                hints);

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            // Set the return value
            if (keyColumn != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(keyColumn.Type);
                var databaseType = (dbType != null) ? new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value) : null;
                var returnValue = keyColumn == null ? "NULL" :
                    string.IsNullOrWhiteSpace(databaseType) ?
                        string.Concat("[INSERTED].", keyColumn.Name.AsQuoted(DbSetting)) :
                            string.Concat("CONVERT(", databaseType, ", [INSERTED].", keyColumn.Name.AsQuoted(DbSetting), ")");
                var result = string.Concat("OUTPUT ", returnValue, $" AS [Result] ");
                commandText = commandText.Insert(commandText.IndexOf("SELECT"), result);
            }

            // Return the query
            return commandText;
        }

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public override string CreateMerge(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field>? qualifiers = null,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new MissingFieldsException();
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers.Any() == true)
                {
                    throw new InvalidQualifiersException($"The qualifiers '{unmatchesQualifiers.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }
            else
            {
                if (primaryField != null)
                {
                    // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                    var isPresent = fields.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                    // Throw if not present
                    if (isPresent == false)
                    {
                        throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                            $"primary field is present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                    }

                    // The primary is present, use it as a default if there are no qualifiers given
                    qualifiers = primaryField.AsField().AsEnumerable();
                }
                else
                {
                    // Throw exception, qualifiers are not defined
                    throw new MissingQualifierFieldsException($"There are no qualifier fields found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => !string.Equals(field.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(fields, 0, DbSetting)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T", true, DbSetting))
                            .Join(" AND "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", DbSetting);

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            // Set the output
            if (keyColumn != null)
            {
                builder
                    .WriteText(string.Concat("OUTPUT INSERTED.", keyColumn.Name.AsField(DbSetting)))
                    .As("[Result]");
            }

            // End the builder
            builder.End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateMergeAll

        /// <summary>
        /// Creates a SQL Statement for merge-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public override string CreateMergeAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field>? qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField? primaryField = null,
            DbField? identityField = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new MissingFieldsException();
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers.Any() == true)
                {
                    throw new InvalidQualifiersException($"The qualifiers '{unmatchesQualifiers.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }
            else
            {
                if (primaryField != null)
                {
                    // Make sure that primary is present in the list of fields before qualifying to become a qualifier
                    var isPresent = fields.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                    // Throw if not present
                    if (isPresent == false)
                    {
                        throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                            $"primary field is present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                    }

                    // The primary is present, use it as a default if there are no qualifiers given
                    qualifiers = primaryField.AsField().AsEnumerable();
                }
                else
                {
                    // Throw exception, qualifiers are not defined
                    throw new MissingQualifierFieldsException($"There are no qualifier fields found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => !string.Equals(field.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                // MERGE T USING S
                builder.Merge()
                    .TableNameFrom(tableName, DbSetting)
                    .HintsFrom(hints)
                    .As("T")
                    .Using()
                    .OpenParen()
                    .Select()
                    .ParametersAsFieldsFrom(fields, index, DbSetting)
                    .CloseParen()
                    .As("S")
                    // QUALIFIERS
                    .On()
                    .OpenParen()
                    .WriteText(qualifiers?
                        .Select(
                            field => field.AsJoinQualifier("S", "T", true, DbSetting))
                                .Join(" AND "))
                    .CloseParen()
                    // WHEN NOT MATCHED THEN INSERT VALUES
                    .When()
                    .Not()
                    .Matched()
                    .Then()
                    .Insert()
                    .OpenParen()
                    .FieldsFrom(insertableFields, DbSetting)
                    .CloseParen()
                    .Values()
                    .OpenParen()
                    .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                    .CloseParen()
                    // WHEN MATCHED THEN UPDATE SET
                    .When()
                    .Matched()
                    .Then()
                    .Update()
                    .Set()
                    .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", DbSetting);

                // Set the output
                if (keyColumn != null)
                {
                    builder
                        .WriteText(string.Concat("OUTPUT INSERTED.", keyColumn.Name.AsField(DbSetting)))
                            .As("[Result],")
                        .WriteText($"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index}")
                            .As("[__RepoDb_OrderColumn]");
                }

                // End the builder
                builder.End();
            }

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateSkipQuery

        /// <summary>
        /// Creates a SQL Statement for 'BatchQuery' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public override string CreateSkipQuery(string tableName,
            IEnumerable<Field> fields,
            int skip,
            int take,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup? where = null,
            string? hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new MissingFieldsException(fields?.Select(f => f.Name));
            }

            // Validate order by
            if (orderBy == null || orderBy.Any() != true)
            {
                throw new EmptyException("The argument 'orderBy' is required.");
            }

            // Validate the skip
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip), "The rows skipped must be equals or greater than 0.");
            }

            // Validate the take
            if (take < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(take), "The rows per batch must be equals or greater than 1.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .With()
                .WriteText("CTE")
                .As()
                .OpenParen()
                .Select()
                .TopFrom(take + skip)
                .RowNumber()
                .Over()
                .OpenParen()
                .OrderByFrom(orderBy, DbSetting)
                .CloseParen()
                .As("[RowNumber],")
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .CloseParen()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .WriteText("CTE")
                .WriteText(string.Concat("WHERE ([RowNumber] BETWEEN ", skip + 1, " AND ", take + skip, ")"))
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion
    }
}
