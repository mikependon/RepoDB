using MySqlConnector;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for MySql.
    /// </summary>
    public sealed class MySqlConnectorStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorStatementBuilder"/> object.
        /// </summary>
        public MySqlConnectorStatementBuilder()
            : this(DbSettingMapper.Get(typeof(MySqlConnection)),
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public MySqlConnectorStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  convertFieldResolver,
                  averageableClientTypeResolver)
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public override string CreateBatchQuery(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy = null,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
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
                .LimitTake(rowsPerBatch, skip)
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count operation.</returns>
        public override string CreateCount(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateCount(queryBuilder,
                tableName,
                where,
                hints);

            // Return the query
            return result.Replace("COUNT (", "COUNT(");
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public override string CreateCountAll(QueryBuilder queryBuilder,
            string tableName,
            string hints = null)
        {
            var result = base.CreateCountAll(queryBuilder,
                tableName,
                hints);

            // Return the query
            return result.Replace("COUNT (", "COUNT(");
        }

        #endregion

        #region CreateExists

        /// <summary>
        /// Creates a SQL Statement for exists operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for exists operation.</returns>
        public override string CreateExists(QueryBuilder queryBuilder,
            string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Build the query
            builder.Clear()
                .Select()
                .WriteText($"1 AS {("ExistsValue").AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .Limit(1)
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsert(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Call the base
            base.CreateInsert(builder,
                tableName,
                fields,
                primaryField,
                identityField,
                hints);

            // Set the return value
            var result = identityField != null ?
                "LAST_INSERT_ID()" :
                    primaryField != null ? primaryField.Name.AsParameter(DbSetting) : "NULL";

            builder
                .Select()
                .WriteText(result)
                .As("Result".AsQuoted(DbSetting))
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsertAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Call the base
            var commandText = base.CreateInsertAll(builder,
                tableName,
                fields,
                batchSize,
                primaryField,
                identityField,
                hints);

            if (identityField != null)
            {
                // Variables needed
                var commandTexts = new List<string>();
                var splitted = commandText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                // Iterate the indexes
                for (var index = 0; index < splitted.Length; index++)
                {
                    var line = splitted[index].Trim();
                    commandTexts.Add(string.Concat(line, " ; SELECT LAST_INSERT_ID() AS ", "Id".AsQuoted(DbSetting), ", ",
                        $"@__RepoDb_OrderColumn_{index} AS ", "OrderColumn".AsQuoted(DbSetting), " ;"));
                }

                // Set the command text
                commandText = commandTexts.Join(" ");
            }

            // Return the query
            return commandText;
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum operation.</returns>
        public override string CreateMax(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMax(queryBuilder,
                tableName,
                field,
                where,
                hints);

            // Return the query
            return result.Replace("MAX (", "MAX(");
        }

        #endregion

        #region CreateMaxAll

        /// <summary>
        /// Creates a SQL Statement for maximum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum-all operation.</returns>
        public override string CreateMaxAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMaxAll(queryBuilder,
                tableName,
                field,
                hints);

            // Return the query
            return result.Replace("MAX (", "MAX(");
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public override string CreateMerge(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of fields cannot be null or empty.");
            }

            // Validate the Primary Key
            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}'.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Set the return value
            var result = (string)null;

            // Check both primary and identity
            if (identityField != null && !string.Equals(identityField.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase))
            {
                result = $"(CASE WHEN {identityField.Name.AsParameter(DbSetting)} > 0 THEN " +
                    $"{identityField.Name.AsParameter(DbSetting)} ELSE " +
                    $"{primaryField.Name.AsParameter(DbSetting)} END)";
            }
            else
            {
                result = $"COALESCE({primaryField.Name.AsParameter(DbSetting)}, LAST_INSERT_ID())";
            }

            // Build the query
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(fields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields, 0, DbSetting)
                .CloseParen()
                .WriteText("ON DUPLICATE KEY")
                .Update()
                .FieldsAndParametersFrom(fields, 0, DbSetting)
                .End();

            if (!string.IsNullOrEmpty(result))
            {
                // Set the result
                builder
                    .Select()
                    .WriteText(result)
                    .As("Result".AsQuoted(DbSetting))
                    .End();
            }

            // Return the query
            return builder.GetString();
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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public override string CreateMergeAll(QueryBuilder queryBuilder,
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = 10,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of fields cannot be null or empty.");
            }

            // Validate the Primary Key
            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}'.");
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Set the return value
            var result = (string)null;

            // Clear the builder
            builder.Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                // Build the query
                builder
                    .Insert()
                    .Into()
                    .TableNameFrom(tableName, DbSetting)
                    .OpenParen()
                    .FieldsFrom(fields, DbSetting)
                    .CloseParen()
                    .Values()
                    .OpenParen()
                    .ParametersFrom(fields, index, DbSetting)
                    .CloseParen()
                    .WriteText("ON DUPLICATE KEY")
                    .Update()
                    .FieldsAndParametersFrom(fields, index, DbSetting)
                    .End();

                // Check both primary and identity
                if (identityField != null && !string.Equals(identityField.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase))
                {
                    result = $"(CASE WHEN {identityField.Name.AsParameter(index, DbSetting)} > 0 THEN " +
                        $"{identityField.Name.AsParameter(index, DbSetting)} ELSE " +
                        $"{primaryField.Name.AsParameter(index, DbSetting)} END)";
                }
                else
                {
                    result = $"COALESCE({primaryField.Name.AsParameter(index, DbSetting)}, LAST_INSERT_ID())";
                }

                if (!string.IsNullOrEmpty(result))
                {
                    // Set the result
                    builder
                        .Select()
                        .WriteText(string.Concat(result, " AS ", "Id".AsQuoted(DbSetting), ","))
                        .WriteText(string.Concat($"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index}", " AS ", "OrderColumn".AsQuoted(DbSetting)));
                }

                // End the builder
                builder
                    .End();
            }

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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum operation.</returns>
        public override string CreateMin(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMin(queryBuilder,
                tableName,
                field,
                where,
                hints);

            // Return the query
            return result.Replace("MIN (", "MIN(");
        }

        #endregion

        #region CreateMinAll

        /// <summary>
        /// Creates a SQL Statement for minimum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum-all operation.</returns>
        public override string CreateMinAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMinAll(queryBuilder,
                tableName,
                field,
                hints);

            // Return the query
            return result.Replace("MIN (", "MIN(");
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
        /// <param name="hints">The table hints to be used.</param>
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
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Validate the ordering
            if (orderBy != null)
            {
                // Check if the order fields are present in the given fields
                var unmatchesOrderFields = orderBy.Where(orderField =>
                    fields.FirstOrDefault(f =>
                        string.Equals(orderField.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

                // Throw an error we found any unmatches
                if (unmatchesOrderFields.Any() == true)
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
                builder.Limit(top);
            }
            builder.End();

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
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum operation.</returns>
        public override string CreateSum(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateSum(queryBuilder,
                tableName,
                field,
                where,
                hints);

            // Return the query
            return result.Replace("SUM (", "SUM(");
        }

        #endregion

        #region CreateSumAll

        /// <summary>
        /// Creates a SQL Statement for sum-all operation.
        /// </summary>
        /// <param name="queryBuilder">The query builder to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum-all operation.</returns>
        public override string CreateSumAll(QueryBuilder queryBuilder,
            string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateSumAll(queryBuilder,
                tableName,
                field,
                hints);

            // Return the query
            return result.Replace("SUM (", "SUM(");
        }

        #endregion
    }
}
