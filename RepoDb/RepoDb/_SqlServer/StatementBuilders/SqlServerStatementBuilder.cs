using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Exceptions;
using RepoDb.Resolvers;
using RepoDb.DbSettings;
using System.Data.SqlClient;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    internal sealed class SqlServerStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerStatementBuilder"/> object.
        /// </summary>
        public SqlServerStatementBuilder() { }

        #region Properties

        /// <summary>
        /// Gets the database setting object that is currently in used.
        /// </summary>
        private IDbSetting DbSetting => DbSettingMapper.Get<SqlConnection>();

        #endregion

        #region CreateAverage

        /// <summary>
        /// Creates a SQL Statement for average operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for average operation.</returns>
        public string CreateAverage(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Average(field, DbSetting)
                .WriteText("AS [AverageValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateAverageAll

        /// <summary>
        /// Creates a SQL Statement for average-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for average-all operation.</returns>
        public string CreateAverageAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Average(field, DbSetting)
                .WriteText("AS [AverageValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateBatchQuery

        /// <summary>
        /// Creates a SQL Statement for batch query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public string CreateBatchQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            int? page,
            int? rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Validate order by
            if (orderBy == null || orderBy?.Any() != true)
            {
                throw new EmptyException("The argument 'orderBy' is required.");
            }

            // Validate the page
            if (page == null || page < 0)
            {
                throw new ArgumentOutOfRangeException("The page must be equals or greater than 0.");
            }

            // Validate the page
            if (rowsPerBatch == null || rowsPerBatch < 1)
            {
                throw new ArgumentOutOfRangeException($"The rows per batch must be equals or greater than 1.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .With()
                .WriteText("CTE")
                .As()
                .OpenParen()
                .Select()
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
                .CloseParen()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .WriteText("CTE")
                .WriteText(string.Concat("WHERE ([RowNumber] BETWEEN ", (page * rowsPerBatch) + 1, " AND ", (page + 1) * rowsPerBatch, ")"))
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for count operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for count operation.</returns>
        public string CreateCount(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .CountBig(null, DbSetting)
                .WriteText("AS [CountValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public string CreateCountAll(QueryBuilder queryBuilder,
            string tableName,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .CountBig(null, DbSetting)
                .WriteText("AS [CountValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for delete operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public string CreateDelete(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public string CreateDeleteAll(QueryBuilder queryBuilder,
            string tableName)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public string CreateInsert(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of insertable fields must not be null or empty for '{tableName}'.");
            }

            // Ensure the primary is on the list if it is not an identity
            if (primaryField != null)
            {
                if (primaryField != identityField)
                {
                    var isPresent = fields.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;
                    if (isPresent == false)
                    {
                        throw new PrimaryFieldNotFoundException("The non-identity primary field must be present during insert operation.");
                    }
                }
            }

            // Variables needed
            var databaseType = "BIGINT";
            var insertableFields = fields
                .Where(f => !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(insertableFields, 0, DbSetting)
                .CloseParen()
                .End();

            // Set the return value
            var result = identityField != null ?
                string.Concat("CONVERT(", databaseType, ", SCOPE_IDENTITY())") :
                    primaryField != null ? primaryField.Name.AsParameter(DbSetting) : "NULL";
            queryBuilder
                .Select()
                .WriteText(result)
                .As("[Result]")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for insert-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public string CreateInsertAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of fields cannot be null or empty.");
            }

            // Ensure the primary is on the list if it is not an identity
            if (primaryField != null)
            {
                if (primaryField != identityField)
                {
                    var isPresent = fields.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;
                    if (isPresent == false)
                    {
                        throw new PrimaryFieldNotFoundException("The non-identity primary field must be present during insert operation.");
                    }
                }
            }

            // Variables needed
            var databaseType = (string)null;
            var insertableFields = fields
                .Where(f => !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                queryBuilder.Insert()
                    .Into()
                    .TableNameFrom(tableName, DbSetting)
                    .OpenParen()
                    .FieldsFrom(insertableFields, DbSetting)
                    .CloseParen()
                    .Values()
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .CloseParen()
                    .End();

                // Set the return field
                if (identityField != null)
                {
                    var returnValue = string.Concat(identityField.Name.AsUnquoted(true, DbSetting).AsParameter(index, DbSetting), " = ",
                        string.IsNullOrEmpty(databaseType) ?
                            "SCOPE_IDENTITY()" :
                            "CONVERT(", databaseType, ", SCOPE_IDENTITY())");
                    queryBuilder
                        .Set()
                        .WriteText(returnValue)
                        .End();
                }
            }

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMax

        /// <summary>
        /// Creates a SQL Statement for maximum operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for maximum operation.</returns>
        public string CreateMax(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText("AS [MaxValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMaxAll

        /// <summary>
        /// Creates a SQL Statement for maximum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for maximum-all operation.</returns>
        public string CreateMaxAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText("AS [MaxValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public string CreateMerge(QueryBuilder queryBuilder,
             string tableName,
             IEnumerable<Field> fields,
             IEnumerable<Field> qualifiers = null,
             DbField primaryField = null,
             DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of fields cannot be null or empty.");
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers?.Any() == true)
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
                    var isPresent = fields?.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

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
                    throw new NullReferenceException($"There are no qualifier field objects found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => !string.Equals(field.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Variables needed
            var databaseType = "BIGINT";

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                // MERGE T USING S
                .Merge()
                .TableNameFrom(tableName, DbSetting)
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
                        field => field.AsJoinQualifier("S", "T", DbSetting))
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
                .FieldsAndAliasFieldsFrom(updateableFields, "S", DbSetting);

            // Set the output
            var outputField = identityField ?? primaryField;
            if (outputField != null)
            {
                queryBuilder
                    .WriteText(string.Concat("OUTPUT INSERTED.", outputField.Name.AsField(DbSetting)))
                    .As("[Result]");
            }

            // End the builder
            queryBuilder.End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMergeAll

        /// <summary>
        /// Creates a SQL Statement for merge-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public string CreateMergeAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of fields cannot be null or empty.");
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers?.Any() == true)
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
                    var isPresent = fields?.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

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
                    throw new NullReferenceException($"There are no qualifier field objects found for '{tableName}'.");
                }
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => !string.Equals(field.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Variables needed
            var databaseType = (string)null;

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }
            }
            else if (primaryField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(primaryField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqlServerStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                // MERGE T USING S
                queryBuilder.Merge()
                    .TableNameFrom(tableName, DbSetting)
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
                            field => field.AsJoinQualifier("S", "T", DbSetting))
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
                    .FieldsAndAliasFieldsFrom(updateableFields, "S", DbSetting);

                // Set the output
                var outputField = identityField ?? primaryField;
                if (outputField != null)
                {
                    queryBuilder
                        .WriteText(string.Concat("OUTPUT INSERTED.", outputField.Name.AsField(DbSetting)))
                        .As("[Result]");
                }

                // End the builder
                queryBuilder.End();
            }

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMin

        /// <summary>
        /// Creates a SQL Statement for minimum operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for minimum operation.</returns>
        public string CreateMin(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText("AS [MinValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateMinAll

        /// <summary>
        /// Creates a SQL Statement for minimum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for minimum-all operation.</returns>
        public string CreateMinAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText("AS [MinValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateQuery

        /// <summary>
        /// Creates a SQL Statement for query operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for query operation.</returns>
        public string CreateQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            if (orderBy != null)
            {
                // Check if the order fields are present in the given fields
                var unmatchesOrderFields = orderBy?.Where(orderField =>
                    fields?.FirstOrDefault(f =>
                        string.Equals(orderField.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesOrderFields?.Any() == true)
                {
                    throw new MissingFieldsException($"The order fields '{unmatchesOrderFields.Select(field => field.Name).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .TopFrom(top)
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateQueryAll

        /// <summary>
        /// Creates a SQL Statement for query-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for query operation.</returns>
        public string CreateQueryAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<OrderField> orderBy = null,
            string hints = null)
        {
            // Guard the target table
            GuardTableName(tableName);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            if (orderBy != null)
            {
                // Check if the order fields are present in the given fields
                var unmatchesOrderFields = orderBy?.Where(orderField =>
                    fields?.FirstOrDefault(f =>
                        string.Equals(orderField.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesOrderFields?.Any() == true)
                {
                    throw new MissingFieldsException($"The order fields '{unmatchesOrderFields.Select(field => field.AsField(DbSetting)).Join(", ")}' are not " +
                        $"present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                }
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateSum

        /// <summary>
        /// Creates a SQL Statement for sum operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for sum operation.</returns>
        public string CreateSum(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText("AS [SumValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateSumAll

        /// <summary>
        /// Creates a SQL Statement for sum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for sum-all operation.</returns>
        public string CreateSumAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText("AS [SumValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateTruncate

        /// <summary>
        /// Creates a SQL Statement for truncate operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public string CreateTruncate(QueryBuilder queryBuilder,
            string tableName)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Truncate()
                .Table()
                .TableNameFrom(tableName, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for update operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for update operation.</returns>
        public string CreateUpdate(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Append the proper prefix
            where?.PrependAnUnderscoreAtTheParameters();

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check if there are updatable fields
            if (updatableFields?.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear()
                .Update()
                .TableNameFrom(tableName, DbSetting)
                .Set()
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region CreateUpdateAll

        /// <summary>
        /// Creates a SQL Statement for update-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <returns>A sql statement for update-all operation.</returns>
        public string CreateUpdateAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField primaryField = null,
            DbField identityField = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Ensure the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of fields cannot be null or empty.");
            }

            // Check the qualifiers
            if (qualifiers?.Any() == true)
            {
                // Check if the qualifiers are present in the given fields
                var unmatchesQualifiers = qualifiers?.Where(field =>
                    fields?.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesQualifiers?.Any() == true)
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
                    var isPresent = fields?.FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

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
                    throw new NullReferenceException($"There are no qualifier field objects found for '{tableName}'.");
                }
            }

            // Gets the updatable fields
            fields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    qualifiers.FirstOrDefault(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

            // Check if there are updatable fields
            if (fields?.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Build the query
            (queryBuilder ?? new QueryBuilder())
                .Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                queryBuilder
                    .Update()
                    .TableNameFrom(tableName, DbSetting)
                    .Set()
                    .FieldsAndParametersFrom(fields, index, DbSetting)
                    .WhereFrom(qualifiers, index, DbSetting)
                    .End();
            }

            // Return the query
            return queryBuilder.GetString();
        }

        #endregion

        #region Helper

        /// <summary>
        /// Throws an exception if the table name is null or empty.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        private void GuardTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName?.Trim()))
            {
                throw new NullReferenceException("The name of the table could be null.");
            }
        }

        /// <summary>
        /// Throws an exception if the primary field is not really a primary field.
        /// </summary>
        /// <param name="field">The instance of the primary field.</param>
        private void GuardPrimary(DbField field)
        {
            if (field?.IsPrimary == false)
            {
                throw new InvalidOperationException("The field is not defined as primary.");
            }
        }

        /// <summary>
        /// Throws an exception if the identity field is not really an identity field.
        /// </summary>
        /// <param name="field">The instance of the identity field.</param>
        private void GuardIdentity(DbField field)
        {
            if (field?.IsIdentity == false)
            {
                throw new InvalidOperationException("The field is not defined as identity.");
            }
        }

        #endregion
    }
}
