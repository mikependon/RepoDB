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
    /// A class that is being used to build a SQL Statement for SqLite.
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
            var builder = new QueryBuilder();

            // Build the query
            builder
                .Clear()
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for exists operation.</returns>
        public override string CreateExists(string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder
                .Clear()
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsert(string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            var builder = new QueryBuilder();

            // Call the base
            builder.WriteText(
                base.CreateInsert(tableName,
                    fields,
                    primaryField,
                    identityField,
                    hints));

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var databaseType = GetDatabaseType(keyColumn);

            // Set the return value
            var columnName = keyColumn != null ?
                keyColumn.IsIdentity ?
                    "last_insert_rowid()" : keyColumn.Name.AsParameter(DbSetting) : "NULL";
            var result = string.IsNullOrWhiteSpace(databaseType) ?
                columnName : $"CAST({columnName} AS {databaseType})";
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be inserted.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for insert operation.</returns>
        public override string CreateInsertAll(string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
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
            var databaseType = GetDatabaseType(keyColumn);

            // Set the return value
            var commandTexts = new List<string>();
            var splitted = commandText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Iterate the indexes
            for (var index = 0; index < splitted.Length; index++)
            {
                var line = splitted[index].Trim();
                var columnName = keyColumn != null ?
                    keyColumn.IsIdentity ?
                        "last_insert_rowid()" : keyColumn.Name.AsParameter(index, DbSetting) : "NULL";
                var result = string.IsNullOrWhiteSpace(databaseType) ?
                    columnName : $"CAST({columnName} AS {databaseType})";
                commandTexts.Add(string.Concat(line, " ; SELECT ", result, " AS ", "Result".AsQuoted(DbSetting), $", {DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index} AS [OrderColumn] ;"));
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for merge operation.</returns>
        public override string CreateMerge(string tableName,
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

            //// Initialize the builder
            //var builder = new QueryBuilder();

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
            //var builder = new QueryBuilder();

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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for query operation.</returns>
        public override string CreateQuery(string tableName,
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

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder
                .Clear()
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
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A sql statement for truncate operation.</returns>
        public override string CreateTruncate(string tableName)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder
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

        #region Helpers

        private string GetDatabaseType(DbField dbField)
        {
            if (dbField == null)
            {
                return default;
            }

            var dbType = new ClientTypeToDbTypeResolver().Resolve(dbField.Type);
            return dbType.HasValue ?
                new DbTypeToSqLiteStringNameResolver().Resolve(dbType.Value) : null;
        }

        #endregion
    }
}
