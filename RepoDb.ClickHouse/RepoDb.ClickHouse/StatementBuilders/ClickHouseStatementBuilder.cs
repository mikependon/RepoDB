using System;
using System.Collections.Generic;
using System.Linq;
using ClickHouse.Driver.ADO;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for ClickHouse.
    /// </summary>
    public sealed class ClickHouseStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseStatementBuilder"/> object.
        /// </summary>
        public ClickHouseStatementBuilder()
            : this(DbSettingMapper.Get<ClickHouseConnection>(),
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public ClickHouseStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  convertFieldResolver,
                  averageableClientTypeResolver)
        { }

        #region Helpers

        private static void GuardNoIdentity(DbField identityField)
        {
            if (identityField != null)
            {
                throw new NotSupportedException("ClickHouse does not support identity/auto-increment columns.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="batchSize"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        /// <exception cref="EmptyException"></exception>
        /// <exception cref="PrimaryFieldNotFoundException"></exception>
        private string BuildInsertValues(string tableName,
            IEnumerable<Field> fields,
            int batchSize,
            DbField primaryField,
            DbField identityField,
            string hints)
        {
            GuardTableName(tableName);
            GuardHints(hints);
            GuardNoIdentity(identityField);

            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of insertable fields must not be null or empty for '{tableName}'.");
            }

            if (primaryField != null &&
                primaryField.HasDefaultValue == false &&
                fields.FirstOrDefault(f => string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) == null)
            {
                throw new PrimaryFieldNotFoundException($"Primary field '{primaryField.Name}' must be present from the list.");
            }

            var builder = new QueryBuilder();

            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(fields, DbSetting)
                .CloseParen()
                .Values();

            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .OpenParen()
                    .ParametersFrom(fields, index, DbSetting)
                    .CloseParen();

                if (index < batchSize - 1)
                {
                    builder.WriteText(",");
                }
            }

            builder.End();

            return builder.GetString();
        }

        #endregion

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

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for delete operation. See <see cref="CreateDeleteAll"/> for the WHERE-clause
        /// rationale: when no filter is supplied (e.g. <c>Delete&lt;TEntity&gt;((object)null)</c>), this emits
        /// <c>WHERE 1 = 1</c> instead of an unconditional <c>DELETE FROM table;</c>, which ClickHouse's
        /// lightweight DELETE rejects as a syntax error.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public override string CreateDelete(string tableName,
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
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints);

            if (where != null)
            {
                builder.WhereFrom(where, DbSetting);
            }
            else
            {
                builder.Where().WriteText("1 = 1");
            }

            builder.End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation. ClickHouse's lightweight <c>DELETE FROM
        /// table WHERE ...</c> mandates a WHERE clause - unlike most RDBMS, an unconditional "delete
        /// every row" cannot be expressed without one. This emits <c>WHERE 1 = 1</c> so the statement
        /// stays valid while still deleting every row of the table.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public override string CreateDeleteAll(string tableName,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);

            // Validate the hints
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .Delete()
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .Where()
                .WriteText("1 = 1")
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
        /// Creates a SQL Statement for insert operation. ClickHouse has no identity/auto-increment
        /// concept, so a non-null <paramref name="identityField"/> (e.g. from a manually-forced
        /// <c>[Identity]</c> mapping) is rejected outright. There is no trailing 'Result' SELECT:
        /// ClickHouse cannot chain a SELECT after an INSERT in one request, and a caller-supplied
        /// primary key needs no round trip to be reported back.
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
            string hints = null) =>
            BuildInsertValues(tableName, fields, 1, primaryField, identityField, hints);

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for insert-all operation: one multi-row
        /// <c>INSERT INTO table (fields) VALUES (row0), (row1), ...;</c> statement.
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
            ValidateMultipleStatementExecution(batchSize);
            return BuildInsertValues(tableName, fields, batchSize, primaryField, identityField, hints);
        }

        #endregion

        #region CreateMax / CreateMaxAll / CreateMin / CreateMinAll / CreateSum / CreateSumAll / CreateCount / CreateCountAll

        // Intentionally not overridden: the base class's default aggregate formatting
        // ("SELECT MAX (`Field`) AS `MaxValue` FROM ...", etc.) is valid ClickHouse SQL as-is.

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation. ClickHouse has no <c>ON DUPLICATE KEY UPDATE</c> /
        /// <c>MERGE</c> statement and no reliable synchronous <c>UPDATE</c>, so this emits the same plain
        /// <c>INSERT</c> as <see cref="CreateInsert"/>. True de-duplication is deferred to the table engine
        /// (e.g. <c>ReplacingMergeTree</c>) and its background merges - the idiomatic ClickHouse upsert
        /// pattern - rather than hard-failing the Merge/MergeAll operations.
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
            GuardPrimary(primaryField);

            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}'.");
            }

            return BuildInsertValues(tableName, fields, 1, primaryField, identityField, hints);
        }

        #endregion

        #region CreateMergeAll

        /// <summary>
        /// Creates a SQL Statement for merge-all operation. See <see cref="CreateMerge"/> for the
        /// rationale: this emits the same multi-row <c>INSERT</c> as <see cref="CreateInsertAll"/>.
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
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            GuardPrimary(primaryField);
            ValidateMultipleStatementExecution(batchSize);

            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}'.");
            }

            return BuildInsertValues(tableName, fields, batchSize, primaryField, identityField, hints);
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

        #region CreateSkipQuery

        /// <summary>
        /// Creates a SQL Statement for 'BatchQuery' operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <param name="take">The number of rows to take.</param>
        /// <param name="orderBy">The list of fields for ordering.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for batch query operation.</returns>
        public override string CreateSkipQuery(string tableName,
            IEnumerable<Field> fields,
            int skip,
            int take,
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
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .LimitTake(take, skip)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for update operation. ClickHouse has no plain <c>UPDATE ... SET ...
        /// WHERE ...</c> statement - only <c>ALTER TABLE table UPDATE col = expr [, ...] WHERE filter</c>,
        /// an asynchronous mutation applied by background merges rather than immediately. The WHERE
        /// clause is mandatory in ClickHouse's ALTER TABLE UPDATE syntax.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for update operation.</returns>
        public override string CreateUpdate(string tableName,
            IEnumerable<Field> fields,
            QueryGroup where = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Check if there are updatable fields
            if (updatableFields.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear()
                .WriteText("ALTER TABLE")
                .TableNameFrom(tableName, DbSetting)
                .WriteText("UPDATE")
                .FieldsAndParametersFrom(updatableFields, 0, DbSetting)
                .WhereFrom(where, DbSetting)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateUpdateAll

        /// <summary>
        /// Creates a SQL Statement for update-all operation. See <see cref="CreateUpdate"/> for the
        /// <c>ALTER TABLE ... UPDATE ... WHERE</c> rationale.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be updated.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the operation.</param>
        /// <param name="primaryField">The primary field from the database.</param>
        /// <param name="identityField">The identity field from the database.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for update-all operation.</returns>
        public override string CreateUpdateAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
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
                var unmatchesQualifiers = qualifiers.Where(field =>
                    fields?.FirstOrDefault(f =>
                        string.Equals(field.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

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
                    var isPresent = fields.FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                    if (isPresent == false)
                    {
                        throw new InvalidQualifiersException($"There are no qualifier field objects found for '{tableName}'. Ensure that the " +
                            $"primary field is present at the given fields '{fields.Select(field => field.Name).Join(", ")}'.");
                    }

                    qualifiers = primaryField.AsField().AsEnumerable();
                }
                else
                {
                    throw new NullReferenceException($"There are no qualifier field objects found for '{tableName}'.");
                }
            }

            // Gets the updatable fields
            var updatableFields = fields
                .Where(f => !string.Equals(f.Name, primaryField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase) &&
                    qualifiers.FirstOrDefault(q => string.Equals(q.Name, f.Name, StringComparison.OrdinalIgnoreCase)) == null);

            if (updatableFields.Any() != true)
            {
                throw new EmptyException("The list of updatable fields cannot be null or empty.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .WriteText("ALTER TABLE")
                    .TableNameFrom(tableName, DbSetting)
                    .WriteText("UPDATE")
                    .FieldsAndParametersFrom(updatableFields, index, DbSetting)
                    .WhereFrom(qualifiers, index, DbSetting)
                    .End();
            }

            // Return the query
            return builder.GetString();
        }

        #endregion
    }
}
