using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Exceptions;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to be as base <see cref="IStatementBuilder"/> for the database.
    /// </summary>
    public abstract class BaseStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public BaseStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
        {
            ConvertFieldResolver = convertFieldResolver;
            AverageableClientTypeResolver = averageableClientTypeResolver;
            DbSetting = dbSetting;
        }

        #region Properties

        /// <summary>
        /// Gets the database setting object that is currently in used.
        /// </summary>
        protected IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the resolver used to convert the <see cref="Field"/> object.
        /// </summary>
        protected IResolver<Field, IDbSetting, string> ConvertFieldResolver { get; }

        /// <summary>
        /// Gets the resolver that is being used to resolve the type to be averageable type.
        /// </summary>
        protected IResolver<Type, Type> AverageableClientTypeResolver { get; }

        #endregion

        #region Virtual/Common

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
        public virtual string CreateAverage(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }
            else
            {
                field.Type = AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.DefaultAverageableType);
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Average(field, DbSetting, ConvertFieldResolver)
                .WriteText("AS [AverageValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateAverageAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }
            else
            {
                field.Type = AverageableClientTypeResolver?.Resolve(field.Type ?? DbSetting.DefaultAverageableType);
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Average(field, DbSetting, ConvertFieldResolver)
                .WriteText("AS [AverageValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateCount(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Count(null, DbSetting)
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
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public virtual string CreateCountAll(QueryBuilder queryBuilder,
            string tableName,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Count(null, DbSetting)
                .WriteText("AS [CountValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateDelete(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public virtual string CreateDeleteAll(QueryBuilder queryBuilder,
            string tableName)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateExists

        /// <summary>
        /// Creates a SQL Statement for exists operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used. See <see cref="SqlServerTableHints"/> class.</param>
        /// <returns>A sql statement for exists operation.</returns>
        public virtual string CreateExists(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .TopFrom(1)
                .WriteText("1 AS [ExistsValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateInsert(QueryBuilder queryBuilder,
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
            var insertableFields = fields
                .Where(f => !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
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

            // Return the query
            return builder.GetString();
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
        public virtual string CreateInsertAll(QueryBuilder queryBuilder,
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

            // Validate the multiple statement execution
            ValidateMultipleStatementExecution(batchSize);

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
            var insertableFields = fields
                .Where(f => !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear();

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
            }

            // Return the query
            return builder.GetString();
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
        public virtual string CreateMax(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText("AS [MaxValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateMaxAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Max(field, DbSetting)
                .WriteText("AS [MaxValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateMin(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText("AS [MinValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateMinAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Min(field, DbSetting)
                .WriteText("AS [MinValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Validate the ordering
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

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
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
            return builder.GetString();
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
        public virtual string CreateQueryAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<OrderField> orderBy = null,
            string hints = null)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Validate the ordering
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

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OrderByFrom(orderBy, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateSum(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText("AS [SumValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateSumAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            ValidateHints(hints);

            // Check the field
            if (field == null)
            {
                throw new NullReferenceException("The field cannot be null.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .Sum(field, DbSetting)
                .WriteText("AS [SumValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateTruncate

        /// <summary>
        /// Creates a SQL Statement for truncate operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public virtual string CreateTruncate(QueryBuilder queryBuilder,
            string tableName)
        {
            // Guard the target table
            GuardTableName(tableName);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Truncate()
                .Table()
                .TableNameFrom(tableName, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateUpdate(QueryBuilder queryBuilder,
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

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check if there are updatable fields
            if (updatableFields?.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Update()
                .TableNameFrom(tableName, DbSetting)
                .Set()
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
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
        public virtual string CreateUpdateAll(QueryBuilder queryBuilder,
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

            // Validate the multiple statement execution
            ValidateMultipleStatementExecution(batchSize);

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

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear();

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
            return builder.GetString();
        }

        #endregion

        #endregion

        #region Abstract

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
        public abstract string CreateBatchQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            int? page,
            int? rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup where = null,
            string hints = null);

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
        public abstract string CreateMerge(QueryBuilder queryBuilder,
             string tableName,
             IEnumerable<Field> fields,
             IEnumerable<Field> qualifiers = null,
             DbField primaryField = null,
             DbField identityField = null);

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
        public abstract string CreateMergeAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            DbField primaryField = null,
            DbField identityField = null);

        #endregion

        #endregion

        #region Helper

        /// <summary>
        /// Throws an exception if the table name is null or empty.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        protected void GuardTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new NullReferenceException("The name of the table could be null.");
            }
        }

        /// <summary>
        /// Throws an exception if the primary field is not really a primary field.
        /// </summary>
        /// <param name="field">The instance of the primary field.</param>
        protected void GuardPrimary(DbField field)
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
        protected void GuardIdentity(DbField field)
        {
            if (field?.IsIdentity == false)
            {
                throw new InvalidOperationException("The field is not defined as identity.");
            }
        }

        /// <summary>
        /// Throws an exception if the 'hints' is present and the <see cref="IDbSetting"/> object does not support it.
        /// </summary>
        /// <param name="hints">The value to be evaluated.</param>
        protected void ValidateHints(string hints = null)
        {
            if (!string.IsNullOrEmpty(hints) && !DbSetting.AreTableHintsSupported)
            {
                throw new NotSupportedException("The table hints are not supported on this database provider statement builder.");
            }
        }

        /// <summary>
        /// Throws an exception if the 'batchSize' is greater than 1 and the multiple statement execution is not being supported
        /// based on the settings of the <see cref="IDbSetting"/> object.
        /// </summary>
        /// <param name="batchSize">The batch size to be evaluated.</param>
        protected void ValidateMultipleStatementExecution(int batchSize = Constant.DefaultBatchOperationSize)
        {
            if (DbSetting.IsMultipleStatementExecutionSupported == false && batchSize > 1)
            {
                throw new NotSupportedException($"Multiple execution is not supported based on the current database setting '{DbSetting.GetType().FullName}'. Consider setting the batchsize to 1.");
            }
        }

        #endregion
    }
}
