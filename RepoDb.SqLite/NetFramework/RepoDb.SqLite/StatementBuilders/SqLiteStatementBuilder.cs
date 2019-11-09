using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for SqLite.
    /// </summary>
    internal sealed class SqLiteStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteStatementBuilder"/> object.
        /// </summary>
        public SqLiteStatementBuilder()
            : base(DbSettingMapper.Get(typeof(SQLiteConnection)),
                  new SqLiteConvertFieldResolver(),
                  new ClientTypeToAverageableClientTypeResolver())
        { }

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
        public override string CreateBatchQuery(QueryBuilder queryBuilder,
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

            // Validate the hints
            ValidateHints(hints);

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

            // Skipping variables
            var skip = (page * rowsPerBatch);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .LimitFrom(skip, rowsPerBatch)
                .End();

            // Return the query
            return queryBuilder.GetString();
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
        public override string CreateExists(QueryBuilder queryBuilder,
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
                .WriteText("1 AS [ExistsValue]")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .LimitFrom(0, 1)
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
        public override string CreateInsertAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null)
        {
            if (batchSize > 1)
            {
                throw new NotSupportedException("The multiple execution is not supported in SqLite. Consider setting the batchsize to 1.");
            }
            else
            {
                return base.CreateInsertAll(queryBuilder,
                    tableName,
                    fields,
                    batchSize,
                    primaryField,
                    identityField);
            }
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
        public override string CreateMerge(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers = null,
            DbField primaryField = null,
            DbField identityField = null)
        {
            throw new NotSupportedException("The merge statement is not supported in SqLite.");
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
        public override string CreateMergeAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = 10,
            DbField primaryField = null,
            DbField identityField = null)
        {
            throw new NotSupportedException("The merge statement is not supported in SqLite.");
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
        public override string CreateQuery(QueryBuilder queryBuilder,
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
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting);
            if (top > 0)
            {
                builder.LimitFrom(0, top);
            }
            builder.End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateTruncate

        //// <summary>
        /// Creates a SQL Statement for truncate operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public override string CreateTruncate(QueryBuilder queryBuilder,
            string tableName)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
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
        public override string CreateUpdateAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null)
        {
            if (batchSize > 1)
            {
                throw new NotSupportedException("The multiple execution is not supported in SqLite. Consider setting the batchsize to 1.");
            }
            else
            {
                return base.CreateUpdateAll(queryBuilder,
                    tableName,
                    fields,
                    qualifiers,
                    batchSize,
                    primaryField,
                    identityField);
            }
        }

        #endregion
    }
}
