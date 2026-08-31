#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Sap.Data.Hana;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for SAP HANA.
    /// </summary>
    public sealed class SapHanaStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SapHanaStatementBuilder"/> object.
        /// </summary>
        public SapHanaStatementBuilder()
            : this(DbSettingMapper.Get<HanaConnection>(),
                  null,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaStatementBuilder"/> class.
        /// </summary>
        public SapHanaStatementBuilder(IDbSetting dbSetting,
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

            // Build the query - HANA only supports the ANSI "LIMIT take OFFSET skip" form, not MySQL's
            // "LIMIT skip, take" comma form, hence LimitOffset() rather than LimitTake().
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

        #region CreateQuery

        /// <summary>
        /// Creates a SQL Statement for query operation.
        /// </summary>
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

            // Build the query - HANA has no TOP (n) syntax, use a trailing LIMIT n clause instead.
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

            // Build the query - see the remark on CreateBatchQuery re: LimitOffset() vs LimitTake().
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .LimitOffset(take, skip)
                .End();

            // Return the query
            return builder.GetString();
        }

        #endregion

        #region CreateCount

        /// <summary>
        /// Creates a SQL Statement for count operation.
        /// </summary>
        public override string CreateCount(string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateCount(tableName,
                where,
                hints);

            return result.Replace("COUNT (", "COUNT(");
        }

        #endregion

        #region CreateCountAll

        /// <summary>
        /// Creates a SQL Statement for count-all operation.
        /// </summary>
        public override string CreateCountAll(string tableName,
            string hints = null)
        {
            var result = base.CreateCountAll(tableName,
                hints);

            return result.Replace("COUNT (", "COUNT(");
        }

        #endregion

        #region CreateExists

        /// <summary>
        /// Creates a SQL Statement for exists operation.
        /// </summary>
        public override string CreateExists(string tableName,
            QueryGroup where = null,
            string hints = null)
        {
            GuardTableName(tableName);
            GuardHints(hints);

            var builder = new QueryBuilder();

            builder.Clear()
                .Select()
                .WriteText($"1 AS {("ExistsValue").AsQuoted(DbSetting)}")
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .Limit(1)
                .End();

            return builder.GetString();
        }

        #endregion

        #region CreateInsert

        /// <summary>
        /// Creates a SQL Statement for insert operation.
        /// </summary>
        public override string CreateInsert(string tableName,
            IEnumerable<Field> fields = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            return base.CreateInsert(tableName,
                fields,
                primaryField,
                identityField,
                hints);
        }

        #endregion

        #region CreateInsertAll

        /// <summary>
        /// Creates a SQL Statement for insert-all operation.
        /// </summary>
        public override string CreateInsertAll(string tableName,
            IEnumerable<Field> fields = null,
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            ValidateMultipleStatementExecution(batchSize);

            return CreateInsert(tableName, fields, primaryField, identityField, hints);
        }

        #endregion

        #region CreateMax

        /// <summary>
        /// Creates a SQL Statement for maximum operation.
        /// </summary>
        public override string CreateMax(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMax(tableName,
                field,
                where,
                hints);

            return result.Replace("MAX (", "MAX(");
        }

        #endregion

        #region CreateMaxAll

        /// <summary>
        /// Creates a SQL Statement for maximum-all operation.
        /// </summary>
        public override string CreateMaxAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMaxAll(tableName,
                field,
                hints);

            return result.Replace("MAX (", "MAX(");
        }

        #endregion

        #region CreateMerge

        /// <summary>
        /// Creates a SQL Statement for merge operation, using HANA's native <c>UPSERT ... WITH PRIMARY KEY</c>.
        /// </summary>
        public override string CreateMerge(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers = null,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            GuardTableName(tableName);
            GuardHints(hints);
            GuardPrimary(primaryField);
            GuardIdentity(identityField);

            if (fields?.Any() != true)
            {
                throw new NullReferenceException($"The list of fields cannot be null or empty.");
            }

            if (primaryField == null)
            {
                throw new PrimaryFieldNotFoundException($"The is no primary field from the table '{tableName}'.");
            }

            var builder = new QueryBuilder();

            builder.Clear()
                .WriteText("UPSERT")
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(fields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields, 0, DbSetting)
                .CloseParen()
                .WriteText("WITH PRIMARY KEY")
                .End();

            return builder.GetString();
        }

        #endregion

        #region CreateMergeAll

        /// <summary>
        /// Creates a SQL Statement for merge-all operation.
        /// </summary>
        public override string CreateMergeAll(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            int batchSize = 10,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            ValidateMultipleStatementExecution(batchSize);

            return CreateMerge(tableName, fields, qualifiers, primaryField, identityField, hints);
        }

        #endregion

        #region CreateMin

        /// <summary>
        /// Creates a SQL Statement for minimum operation.
        /// </summary>
        public override string CreateMin(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateMin(tableName,
                field,
                where,
                hints);

            return result.Replace("MIN (", "MIN(");
        }

        #endregion

        #region CreateMinAll

        /// <summary>
        /// Creates a SQL Statement for minimum-all operation.
        /// </summary>
        public override string CreateMinAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateMinAll(tableName,
                field,
                hints);

            return result.Replace("MIN (", "MIN(");
        }

        #endregion

        #region CreateSum

        /// <summary>
        /// Creates a SQL Statement for sum operation.
        /// </summary>
        public override string CreateSum(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null)
        {
            var result = base.CreateSum(tableName,
                field,
                where,
                hints);

            return result.Replace("SUM (", "SUM(");
        }

        #endregion

        #region CreateSumAll

        /// <summary>
        /// Creates a SQL Statement for sum-all operation.
        /// </summary>
        public override string CreateSumAll(string tableName,
            Field field,
            string hints = null)
        {
            var result = base.CreateSumAll(tableName,
                field,
                hints);

            return result.Replace("SUM (", "SUM(");
        }

        #endregion
    }
}
