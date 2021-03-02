using Npgsql;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for PostgreSql.
    /// </summary>
    public sealed class PostgreSqlStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlStatementBuilder"/> object.
        /// </summary>
        public PostgreSqlStatementBuilder()
            : this(DbSettingMapper.Get(typeof(NpgsqlConnection)),
                  new PostgreSqlConvertFieldResolver(),
                  new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public PostgreSqlStatementBuilder(IDbSetting dbSetting,
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
                .LimitOffset(rowsPerBatch, skip)
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
                .WriteText("1 AS \"ExistsValue\"")
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

            // Variables needed
            var databaseType = (string)null;

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToPostgreSqlStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Set the return value
            var result = identityField != null ?
                string.IsNullOrEmpty(databaseType) ?
                    identityField.Name.AsQuoted(DbSetting) :
                        $"CAST({identityField.Name.AsQuoted(DbSetting)} AS {databaseType})" :
                            primaryField != null ? primaryField.Name.AsQuoted(DbSetting) : "NULL";

            // Get the string
            var sql = builder.GetString().Trim();

            // Append the result
            sql = string.Concat(sql.Substring(0, sql.Length - 1),
                "RETURNING ", result, " AS ", "Result".AsQuoted(DbSetting), " ;");

            // Return the query
            return sql;
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

            // Variables needed
            var databaseType = "BIGINT";

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToPostgreSqlStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Variables needed
            var commandTexts = new List<string>();
            var splitted = commandText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Iterate the indexes
            for (var index = 0; index < splitted.Length; index++)
            {
                var line = splitted[index].Trim();

                // Set the return value
                var returnValue = identityField != null ?
                    string.IsNullOrEmpty(databaseType) ?
                        identityField.Name.AsQuoted(DbSetting) :
                        $"CAST({identityField.Name.AsQuoted(DbSetting)} AS {databaseType})" :
                    primaryField != null ? primaryField.Name.AsQuoted(DbSetting) : "NULL";
                commandTexts.Add(string.Concat(line, " RETURNING ", returnValue, " AS ", "Id".AsQuoted(DbSetting), ", ",
                    $"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index} AS ", "OrderColumn".AsQuoted(DbSetting), " ;"));
            }

            // Set the command text
            commandText = commandTexts.Join(" ");

            // Return the query
            return commandText;
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

            // Set the qualifiers
            if (qualifiers?.Any() != true && primaryField != null)
            {
                qualifiers = primaryField.AsField().AsEnumerable();
            }

            // Validate the qualifiers
            if (qualifiers?.Any() != true)
            {
                if (primaryField == null)
                {
                    throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}' that can be used as qualifier.");
                }
                else
                {
                    throw new InvalidQualifiersException($"There are no defined qualifier fields.");
                }
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Remove the qualifiers from the fields
            var updatableFields = fields
                .Where(f =>
                    qualifiers?.Any(qf => string.Equals(qf.Name, f.Name, StringComparison.OrdinalIgnoreCase)) != true)
                .AsList();

            // Build the query
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(fields, DbSetting)
                .CloseParen();

            // Override the system value
            if (identityField != null)
            {
                builder.WriteText("OVERRIDING SYSTEM VALUE");
            }

            // Continue
            builder
                .Values()
                .OpenParen()
                .ParametersFrom(fields, 0, DbSetting)
                .CloseParen()
                .OnConflict(qualifiers, DbSetting)
                .DoUpdate()
                .Set()
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting);

            // Variables needed
            var databaseType = (string)null;

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToPostgreSqlStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Set the return value
            var result = identityField == null ? primaryField.Name.AsParameter(DbSetting) :
                string.IsNullOrEmpty(databaseType) ? identityField.Name.AsQuoted(DbSetting) :
                $"CAST({identityField.Name.AsQuoted(DbSetting)} AS {databaseType})";

            if (!string.IsNullOrEmpty(result))
            {
                // Get the string
                var sql = string.Concat("RETURNING ", result, " AS ", "Result".AsQuoted(DbSetting));

                // Set the result
                builder
                    .WriteText(sql);
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

            // Set the qualifiers
            if (qualifiers?.Any() != true && primaryField != null)
            {
                qualifiers = primaryField.AsField().AsEnumerable();
            }

            // Validate the qualifiers
            if (qualifiers?.Any() != true)
            {
                if (primaryField == null)
                {
                    throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}' that can be used as qualifier.");
                }
                else
                {
                    throw new InvalidQualifiersException($"There are no defined qualifier fields.");
                }
            }

            // Initialize the builder
            var builder = queryBuilder ?? new QueryBuilder();

            // Remove the qualifiers from the fields
            var updatableFields = fields
                .Where(f =>
                    qualifiers?.Any(qf => string.Equals(qf.Name, f.Name, StringComparison.OrdinalIgnoreCase)) != true)
                .AsList();

            // Variables needed
            var databaseType = (string)null;

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToPostgreSqlStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Set the return value
            var result = identityField == null ? primaryField.Name.AsParameter(DbSetting) :
                string.IsNullOrEmpty(databaseType) ? identityField.Name.AsQuoted(DbSetting) :
                $"CAST({identityField.Name.AsQuoted(DbSetting)} AS {databaseType})";

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
                    .CloseParen();

                // Override the system value
                if (identityField != null)
                {
                    builder.WriteText("OVERRIDING SYSTEM VALUE");
                }

                // Continue
                builder
                    .Values()
                    .OpenParen()
                    .ParametersFrom(fields, index, DbSetting)
                    .CloseParen()
                    .OnConflict(qualifiers, DbSetting)
                    .DoUpdate()
                    .Set()
                    .FieldsAndParametersFrom(updatableFields, index, DbSetting);

                if (!string.IsNullOrEmpty(result))
                {
                    // Get the string
                    var sql = string.Concat("RETURNING ", result, " AS ", "Id".AsQuoted(DbSetting), ", ",
                        $"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index}", " AS ", "OrderColumn".AsQuoted(DbSetting));

                    // Set the result
                    builder
                        .WriteText(sql);
                }

                // End the builder
                builder.End();
            }

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

        #region CreateTruncate

        /// <summary>
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
                .Truncate()
                .Table()
                .TableNameFrom(tableName, DbSetting)
                .WriteText("RESTART IDENTITY")
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion
    }
}
