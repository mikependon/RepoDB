#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vertica.Data.VerticaClient;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for Vertica. Targets Vertica 3.0 and later.
    /// </summary>
    public sealed class VerticaStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaStatementBuilder"/> object.
        /// </summary>
        public VerticaStatementBuilder()
            : this(DbSettingMapper.Get<VerticaConnection>(),
                  new VerticaConvertFieldResolver(),
                  new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="VerticaStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public VerticaStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  convertFieldResolver,
                  averageableClientTypeResolver)
        { }

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
            GuardHints(hints);

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query.
            builder.Clear()
                .Select()
                .WriteText(string.Concat("1 AS ", "ExistsValue".AsQuoted(DbSetting)))
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .Limit(1);

            // Return the query. Deliberately no ".End()" - see the remarks on TrimTrailingSemicolon:
            // Vertica's DSQL layer rejects a trailing ';' on a statement sent through VerticaCommand.
            return builder.GetString();
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
            GuardHints(hints);

            // There should be fields
            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of queryable fields must not be null for '{tableName}'.");
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query.
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

            // Return the query. Deliberately no ".End()" - see CreateExists.
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

            // Build the query.
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .LimitOffset(rowsPerBatch, skip);

            // Return the query. Deliberately no ".End()" - see CreateExists.
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

            // Build the query.
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .LimitOffset(take, skip);

            // Return the query. Deliberately no ".End()" - see CreateExists.
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
            return TrimTrailingSemicolon(base.CreateInsert(tableName,
                fields,
                primaryField,
                identityField,
                hints));
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

            // Verify the fields
            if (fields?.Any() != true)
            {
                throw new EmptyException($"The list of insertable fields must not be null or empty for '{tableName}'.");
            }

            if (batchSize <= 1)
            {
                return CreateInsert(tableName, fields, primaryField, identityField, hints);
            }

            // IDENTITY columns can never be written to in Vertica - exclude, same as CreateInsert.
            var insertableFields = (identityField == null
                ? fields
                : fields.Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)))
                .AsList();
            var builder = new QueryBuilder();
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values();

            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .OpenParen()
                    .ParametersFrom(insertableFields, index, DbSetting)
                    .CloseParen();

                if (index < batchSize - 1)
                {
                    builder.WriteText(",");
                }
            }

            // Return the query. Deliberately no ".End()" - see CreateExists.
            return builder.GetString();
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
                throw new EmptyException($"The list of fields cannot be null or empty for '{tableName}'.");
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
                    throw new PrimaryFieldNotFoundException($"There is no primary field from the table '{tableName}' that can be used as a qualifier.");
                }
                else
                {
                    throw new InvalidQualifiersException("There are no defined qualifier fields.");
                }
            }

            return BuildMergeStatement(tableName, fields, qualifiers, primaryField, identityField);
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
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            ValidateMultipleStatementExecution(batchSize);
            return CreateMerge(tableName, fields, qualifiers, primaryField, identityField, hints);
        }

        #endregion

        #region CreateUpdate

        /// <summary>
        /// Creates a SQL Statement for update operation.
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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateUpdate(tableName, fields, where, primaryField, identityField, hints));

        #endregion

        #region CreateUpdateAll

        /// <summary>
        /// Creates a SQL Statement for update-all operation.
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
            string hints = null) =>
            // The base implementation already calls ValidateMultipleStatementExecution(batchSize)
            // internally, which throws given VerticaDbSetting.IsMultiStatementExecutable == false
            // and batchSize > 1 - no need to duplicate that guard here.
            TrimTrailingSemicolon(base.CreateUpdateAll(tableName, fields, qualifiers, batchSize, primaryField, identityField, hints));

        #endregion

        #region CreateDelete

        /// <summary>
        /// Creates a SQL Statement for delete operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete operation.</returns>
        public override string CreateDelete(string tableName,
            QueryGroup where = null,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateDelete(tableName, where, hints));

        #endregion

        #region CreateDeleteAll

        /// <summary>
        /// Creates a SQL Statement for delete-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for delete-all operation.</returns>
        public override string CreateDeleteAll(string tableName,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateDeleteAll(tableName, hints));

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

            // Vertica has no TRUNCATE TABLE statement (as of 5.0). DELETE FROM without a WHERE
            // clause is the closest equivalent; unlike TRUNCATE elsewhere, it does not reset a
            // GENERATED AS IDENTITY column's next value.
            var builder = new QueryBuilder();

            builder.Clear()
                .WriteText("DELETE FROM")
                .TableNameFrom(tableName, DbSetting);

            // Return the query. Deliberately no ".End()" - see CreateExists.
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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateCount(tableName, where, hints));

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for count-all operation.</returns>
        public override string CreateCountAll(string tableName,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateCountAll(tableName, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMax(tableName, field, where, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMaxAll(tableName, field, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMin(tableName, field, where, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMinAll(tableName, field, hints));

        #endregion

        #region CreateAverage

        /// <summary>
        /// Creates a SQL Statement for average operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for average operation.</returns>
        public override string CreateAverage(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateAverage(tableName, field, where, hints));

        #endregion

        #region CreateAverageAll

        /// <summary>
        /// Creates a SQL Statement for average-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be averaged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for average-all operation.</returns>
        public override string CreateAverageAll(string tableName,
            Field field,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateAverageAll(tableName, field, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateSum(tableName, field, where, hints));

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
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateSumAll(tableName, field, hints));

        #endregion

        #region Helpers

        /// <summary>
        /// Every <c>Create*</c> method in <see cref="BaseStatementBuilder"/> ends its generated SQL with
        /// <c>QueryBuilder.End()</c>, which unconditionally appends <c>" ;"</c>. Vertica's DSQL layer
        /// treats the semicolon purely as an isql/script statement separator, not as part of the grammar
        /// for a single statement submitted through the API - sending one via <c>VerticaCommand.CommandText</c>
        /// fails the same way it does on Oracle/DB2. Strip it for every base-inherited method.
        /// </summary>
        private static string TrimTrailingSemicolon(string sql) =>
            sql?.TrimEnd().TrimEnd(';').TrimEnd();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="qualifiers"></param>
        /// <param name="primaryField"></param>
        /// <param name="identityField"></param>
        /// <returns></returns>
        private string BuildMergeStatement(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            DbField primaryField,
            DbField identityField)
        {
            var fieldList = fields.AsList();
            var qualifierList = qualifiers.AsList();
            var quotedTable = tableName.AsQuoted(true, DbSetting);

            // IDENTITY columns (and columns defaulting to a named sequence) can never be written to
            // in Vertica - exclude from both the UPDATE SET and INSERT column lists.
            var writableFields = (identityField == null
                ? fieldList
                : fieldList.Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)))
                .AsList();
            var updatableFields = writableFields
                .Where(f => !qualifierList.Any(qf => string.Equals(qf.Name, f.Name, StringComparison.OrdinalIgnoreCase)))
                .AsList();

            string WhereQualifiers() =>
                qualifierList.Select(f => string.Concat(f.Name.AsQuoted(DbSetting), " = ", f.Name.AsParameter(DbSetting))).Join(" AND ");

            var sb = new StringBuilder();

            if (updatableFields.Count > 0)
            {
                sb.Append("UPDATE ").Append(quotedTable).Append(" SET ")
                    .Append(updatableFields.Select(f => string.Concat(f.Name.AsQuoted(DbSetting), " = ", f.Name.AsParameter(DbSetting))).Join(", "))
                    .Append(" WHERE ").Append(WhereQualifiers()).Append("; ");
            }

            sb.Append("INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(writableFields.Select(f => f.Name.AsQuoted(DbSetting)).Join(", ")).Append(')')
                .Append(" SELECT ").Append(writableFields.Select(f => f.Name.AsParameter(DbSetting)).Join(", "))
                .Append(" WHERE NOT EXISTS (SELECT 1 FROM ").Append(quotedTable).Append(" WHERE ").Append(WhereQualifiers()).Append(')');

            var mergeStatement = sb.ToString();
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            if (keyColumn == null || identityField == null)
            {
                return mergeStatement;
            }
            var resultAlias = "Result".AsQuoted(DbSetting);

            if (qualifierList.Any(qf => string.Equals(qf.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var identityParam = identityField.Name.AsParameter(DbSetting);
                return string.Concat(mergeStatement, "; SELECT CASE WHEN ", identityParam,
                    " IS NULL THEN LAST_INSERT_ID() ELSE ", identityParam, " END AS ", resultAlias);
            }

            return string.Concat(mergeStatement, "; SELECT LAST_INSERT_ID() AS ", resultAlias);
        }

        #endregion
    }
}
