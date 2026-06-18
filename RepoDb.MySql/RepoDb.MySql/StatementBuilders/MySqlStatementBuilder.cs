using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for MySql.
    /// </summary>
    public sealed class MySqlStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlStatementBuilder"/> object.
        /// </summary>
        public MySqlStatementBuilder()
            : this(DbSettingMapper.Get<MySqlConnection>(),
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="MySqlStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public MySqlStatementBuilder(IDbSetting dbSetting,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count operation.</returns>
        public override string CreateCount(string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateCount(tableName,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public override string CreateCountAll(string tableName,
            string hints = null)
        {
            var result = base.CreateCountAll(tableName,
                hints);

            // Return the query
            return result.Replace("COUNT (", "COUNT(");
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
            // Initialize the builder
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
            var returnValue = keyColumn != null ?
                keyColumn.IsIdentity ? "LAST_INSERT_ID()" :
                    keyColumn.Name.AsParameter(DbSetting) : "NULL";

            // Set the return value
            builder
                .Select()
                .WriteText(returnValue)
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
            // Ensure with guards
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            // Validate the multiple statement execution
            ValidateMultipleStatementExecution(batchSize);

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException("The list of fields cannot be null or empty.");
            }

            // Primary Key
            if (primaryField != null &&
                primaryField.HasDefaultValue == false &&
                !string.Equals(primaryField.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isPresent = fields
                    .FirstOrDefault(f =>
                        string.Equals(f.Name, primaryField.Name, StringComparison.OrdinalIgnoreCase)) != null;

                if (isPresent == false)
                {
                    throw new PrimaryFieldNotFoundException($"As the primary field '{primaryField.Name}' is not an identity nor has a default value, it must be present on the insert operation.");
                }
            }

            // Insertable fields
            var insertableFields = fields
                .Where(f =>
                    !string.Equals(f.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query
            builder.Clear();

            // Compose
            builder
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values();

            // Iterate the indexes
            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .WriteText("ROW")
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .CloseParen();

                if (index < batchSize - 1)
                {
                    builder
                        .WriteText(",");
                }
            }

            // Close
            builder
                .End();

            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            if (keyColumn?.IsIdentity == true)
            {
                builder
                .Values();

                for (var index = 0; index < batchSize; index++)
                {
                    if (index > 0)
                        builder.WriteText(",");

                    builder
                        .WriteText("ROW")
                        .OpenParen()
                        .WriteText("LAST_INSERT_ID() +")
                        .WriteText($"{index}")
                        .CloseParen();
                };

                builder.End();

                return builder.GetString();
            }
            else
            {
                var commandText = builder.GetString();

                // Variables needed
                var commandTexts = new List<string>();
                var splitted = commandText.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


                // Iterate the indexes
                for (var index = 0; index < splitted.Length; index++)
                {
                    var returnValue = keyColumn != null ?
                        keyColumn.IsIdentity ? "LAST_INSERT_ID()" :
                            keyColumn.Name.AsParameter(index, DbSetting) : "NULL";
                    var line = splitted[index].Trim();
                    commandTexts.Add(string.Concat(line, " ; SELECT ", returnValue, " AS ", "Result".AsQuoted(DbSetting), ";"));
                }

                // Set the command text
                commandText = commandTexts.Join(" ");

                // Return the query
                return commandText;
            }
        }

        #endregion

        #region CreateMax

        /// <summary>
        /// Creates a SQL Statement for maximum operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum operation.</returns>
        public override string CreateMax(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMax(tableName,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for maximum-all operation.</returns>
        public override string CreateMaxAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMaxAll(tableName,
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
            var builder = new QueryBuilder();

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

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var returnValue = keyColumn != null ? keyColumn.Name.AsParameter(DbSetting) : "NULL";

            // Set the return value
            builder
                .Select()
                .WriteText($"COALESCE({returnValue}, LAST_INSERT_ID())")
                .As("Result".AsQuoted(DbSetting))
                .End();

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

            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var builder = new QueryBuilder();
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

                // Set the return value
                var returnValue = keyColumn != null ? keyColumn.Name.AsParameter(index, DbSetting) : "NULL";
                builder
                    .Select()
                        .WriteText(
                            string.Concat($"COALESCE({returnValue}, LAST_INSERT_ID())", " AS ", "Result".AsQuoted(DbSetting), ","))
                        .WriteText(
                            string.Concat($"{DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index}", " AS ", "OrderColumn".AsQuoted(DbSetting)));

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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum operation.</returns>
        public override string CreateMin(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMin(tableName,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for minimum-all operation.</returns>
        public override string CreateMinAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMinAll(tableName,
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

        #region CreateSum

        /// <summary>
        /// Creates a SQL Statement for sum operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum operation.</returns>
        public override string CreateSum(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateSum(tableName,
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be sumd.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum-all operation.</returns>
        public override string CreateSumAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateSumAll(tableName,
                field,
                hints);

            // Return the query
            return result.Replace("SUM (", "SUM(");
        }

        #endregion
    }
}
