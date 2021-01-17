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
    /// A class used to build a SQL Statement for SqLite.
    /// </summary>
    public sealed class SqLiteStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public SqLiteStatementBuilder(IDbSetting dbSetting,
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
                .WriteText("1 AS [ExistsValue]")
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
            var databaseType = "BIGINT";

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqLiteStringNameResolver().Resolve(dbType.Value);
                }
            }

            // Set the return value
            var result = identityField != null ?
                $"CAST(last_insert_rowid() AS {databaseType})" :
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

            // Variables needed
            var databaseType = (string)null;

            // Check for the identity
            if (identityField != null)
            {
                var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
                if (dbType != null)
                {
                    databaseType = new DbTypeToSqLiteStringNameResolver().Resolve(dbType.Value);
                }
            }

            if (identityField != null)
            {
                // Variables needed
                var commandTexts = new List<string>();
                var splitted = commandText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                // Iterate the indexes
                for (var index = 0; index < splitted.Length; index++)
                {
                    var line = splitted[index].Trim();
                    var returnValue = string.IsNullOrEmpty(databaseType) ?
                        "SELECT last_insert_rowid()" :
                        $"SELECT CAST(last_insert_rowid() AS {databaseType}) AS [Id]";
                    commandTexts.Add(string.Concat(line, " ; ", returnValue, $", {DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index} AS [OrderColumn] ;"));
                }

                // Set the command text
                commandText = commandTexts.Join(" ");
            }

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
            throw new NotImplementedException("The merge statement is not supported in SQLite. SQLite is using the 'Upsert (Insert/Update)' operation.");
            //// Ensure with guards
            //GuardTableName(tableName);
            //GuardHints(hints);
            //GuardPrimary(primaryField);
            //GuardIdentity(identityField);

            //// Verify the fields
            //if (fields?.Any() != true)
            //{
            //    throw new NullReferenceException($"The list of fields cannot be null or empty.");
            //}

            //// Check the primary field
            //if (primaryField == null)
            //{
            //    throw new PrimaryFieldNotFoundException($"SqLite is using the primary key as qualifier for (INSERT or REPLACE) operation.");
            //}

            //// Check the qualifiers
            //if (qualifiers?.Any() == true)
            //{
            //    var others = qualifiers.Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase));
            //    if (others?.Any() == true)
            //    {
            //        throw new InvalidQualifiersException($"SqLite is using the primary key as qualifier for (INSERT or REPLACE) operation. " +
            //            $"Consider creating 'PrimaryKey' in the {tableName} and set the 'qualifiers' to NULL.");
            //    }
            //}

            //// Initialize the builder
            //var builder = queryBuilder ?? new QueryBuilder();

            //// Variables needed
            //var databaseType = "BIGINT";

            //// Set the return value
            //var result = (string)null;

            //// Check both primary and identity
            //if (identityField != null)
            //{
            //    result = string.Concat($"CAST(COALESCE(last_insert_rowid(), {primaryField.Name.AsParameter(DbSetting)}) AS {databaseType})");

            //    // Set the type
            //    var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
            //    if (dbType != null)
            //    {
            //        databaseType = new DbTypeToSqLiteStringNameResolver().Resolve(dbType.Value);
            //    }
            //}
            //else
            //{
            //    result = string.Concat($"CAST({primaryField.Name.AsParameter(DbSetting)} AS {databaseType})");
            //}

            //// Build the query
            //builder.Clear()
            //    .Insert()
            //    .Or()
            //    .Replace()
            //    .Into()
            //    .TableNameFrom(tableName, DbSetting)
            //    .OpenParen()
            //    .FieldsFrom(fields, DbSetting)
            //    .CloseParen()
            //    .Values()
            //    .OpenParen()
            //    .ParametersFrom(fields, 0, DbSetting)
            //    .CloseParen()
            //    .End();

            //if (!string.IsNullOrEmpty(result))
            //{
            //    // Set the result
            //    builder
            //        .Select()
            //        .WriteText(result)
            //        .As("Result".AsQuoted(DbSetting))
            //        .End();
            //}

            //// Return the query
            //return builder.GetString();
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
            throw new NotImplementedException("The merge statement is not supported in SQLite. SQLite is using the 'Upsert (Insert/Update)' operation.");

            //// Ensure with guards
            //GuardTableName(tableName);
            //GuardHints(hints);
            //GuardPrimary(primaryField);
            //GuardIdentity(identityField);

            //// Verify the fields
            //if (fields?.Any() != true)
            //{
            //    throw new NullReferenceException($"The list of fields cannot be null or empty.");
            //}

            //// Check the primary field
            //if (primaryField == null)
            //{
            //    throw new PrimaryFieldNotFoundException($"SqLite is using the primary key as qualifier for (INSERT or REPLACE) operation.");
            //}

            //// Check the qualifiers
            //if (qualifiers?.Any() == true)
            //{
            //    var others = qualifiers.Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase));
            //    if (others?.Any() == true)
            //    {
            //        throw new InvalidQualifiersException($"SqLite is using the primary key as qualifier for (INSERT or REPLACE) operation. " +
            //            $"Consider creating 'PrimaryKey' in the {tableName} and set the 'qualifiers' to NULL.");
            //    }
            //}

            //// Initialize the builder
            //var builder = queryBuilder ?? new QueryBuilder();

            //// Variables needed
            //var databaseType = "BIGINT";

            //// Set the return value
            //var result = (string)null;

            //// Set the type
            //if (identityField != null)
            //{
            //    var dbType = new ClientTypeToDbTypeResolver().Resolve(identityField.Type);
            //    if (dbType != null)
            //    {
            //        databaseType = new DbTypeToSqLiteStringNameResolver().Resolve(dbType.Value);
            //    }
            //}

            //// Clear the builder
            //builder.Clear();

            //// Iterate the indexes
            //for (var index = 0; index < batchSize; index++)
            //{
            //    // Build the query
            //    builder
            //        .Insert()
            //        .Or()
            //        .Replace()
            //        .Into()
            //        .TableNameFrom(tableName, DbSetting)
            //        .OpenParen()
            //        .FieldsFrom(fields, DbSetting)
            //        .CloseParen()
            //        .Values()
            //        .OpenParen()
            //        .ParametersFrom(fields, index, DbSetting)
            //        .CloseParen()
            //        .End();

            //    // Check both primary and identity
            //    if (identityField != null)
            //    {
            //        result = string.Concat($"CAST(COALESCE(last_insert_rowid(), {primaryField.Name.AsParameter(index, DbSetting)}) AS {databaseType})");
            //    }
            //    else
            //    {
            //        result = string.Concat($"CAST({primaryField.Name.AsParameter(index, DbSetting)} AS {databaseType})");
            //    }

            //    if (!string.IsNullOrEmpty(result))
            //    {
            //        // Set the result
            //        builder
            //            .Select()
            //            .WriteText(result)
            //            .As("Result".AsQuoted(DbSetting))
            //            .End();
            //    }
            //}

            //// Return the query
            //return builder.GetString();
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
                .Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .End()
                .WriteText("VACUUM")
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion
    }
}
