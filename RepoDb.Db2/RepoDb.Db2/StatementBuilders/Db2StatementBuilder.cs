#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for Db2. Targets Db2 Database 12c and later.
    /// </summary>
    public sealed class Db2StatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2StatementBuilder"/> object.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        public Db2StatementBuilder(IDbSetting dbSetting)
            : this(dbSetting,
                new Db2ConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Db2StatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public Db2StatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  (convertFieldResolver ?? new Db2ConvertFieldResolver()),
                  (averageableClientTypeResolver ?? new ClientTypeToAverageableClientTypeResolver()))
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

            // Build the query. Db2 has no "TOP" keyword; "FETCH FIRST n ROWS ONLY" is the
            // ANSI-standard equivalent and must be placed at the end of the statement.
            builder.Clear()
                .Select()
                .WriteText(string.Concat("1 AS ", "ExistsValue".AsQuoted(DbSetting)))
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .WriteText("FETCH FIRST 1 ROWS ONLY");

            // Return the query. NOTE: deliberately no ".End()" here - it appends a trailing " ;",
            // which the IBM Data Server .NET Provider rejects with a syntax error on any plain
            // (non-compound-statement) command, SELECT included, regardless of execute method.
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

            // Build the query. Db2's "TOP"-equivalent ("FETCH FIRST n ROWS ONLY") is a
            // trailing clause, unlike SQL Server's TOP which is placed right after SELECT.
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
                builder.WriteText(string.Concat("FETCH FIRST ", top, " ROWS ONLY"));
            }

            // Return the query. Deliberately no ".End()" - see the comment in CreateExists for why
            // a trailing " ;" breaks Db2 regardless of statement type.
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
                throw new MissingFieldsException(fields?.Select(f => f.Name));
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

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query. "OFFSET n ROWS FETCH NEXT m ROWS ONLY" is ANSI SQL:2008 and is
            // supported unchanged on Db2 Database 12c and later.
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .WriteText(string.Concat("OFFSET ", page * rowsPerBatch))
                .WriteText(string.Concat("ROWS FETCH NEXT ", rowsPerBatch, " ROWS ONLY"));

            // Return the query. Deliberately no ".End()" - see the comment in CreateExists.
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
        /// <param name="take">The number of rows per batch.</param>
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
                throw new MissingFieldsException(fields?.Select(f => f.Name));
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

            // Build the query. Unlike SQL Server (which historically needed a CTE + ROW_NUMBER()
            // for this), Db2's OFFSET/FETCH (12c+) directly supports an arbitrary skip/take.
            builder.Clear()
                .Select()
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting)
                .WriteText(string.Concat("OFFSET ", skip))
                .WriteText(string.Concat("ROWS FETCH NEXT ", take, " ROWS ONLY"));

            // Return the query. Deliberately no ".End()" - see the comment in CreateExists.
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
            // Let the base implementation handle the guards/validation and produce the plain
            // "INSERT INTO ... VALUES ( ... ) ;" statement.
            var insertStatement = base.CreateInsert(tableName,
                fields,
                primaryField,
                identityField,
                hints);

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            if (keyColumn == null)
            {
                // No key column requested. A plain INSERT executed via ExecuteScalar() simply
                // yields no rows in Db2 (no error), so no further wrapping is necessary - but
                // the base statement still ends in " ;", which the driver rejects on any plain
                // (non-compound-statement) command, so it still needs trimming.
                return TrimTrailingSemicolon(insertStatement);
            }

            // Return the query, wrapped so the generated key can flow back through the same
            // ExecuteScalar()-based pipeline RepoDb.Core uses for every provider.
            return WrapWithReturningResult(TrimTrailingSemicolon(insertStatement), keyColumn);
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
            ValidateMultipleStatementExecution(batchSize);

            if (batchSize <= 1)
            {
                // Single row: reuse the already-correct, already-tested single-row Insert
                // statement (FINAL TABLE wrapping etc.) rather than duplicating it below for the
                // batchSize-1 case.
                return CreateInsert(tableName, fields, primaryField, identityField, hints);
            }

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

            // Insertable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Build the query. Db2 LUW supports a multi-row "VALUES (...), (...), ..." table
            // constructor directly as an INSERT's source (no derived-table/SYSIBM.SYSDUMMY1
            // indirection needed here, unlike CreateMerge's USING clause - a VALUES list feeding
            // an INSERT's own column list doesn't need a FROM at all).
            var builder = new QueryBuilder();
            builder.Clear()
                .Insert()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values();

            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .OpenParen()
                    .WriteText(BuildCastedParameters(insertableFields, index, DbSetting))
                    .CloseParen();

                if (index < batchSize - 1)
                {
                    builder.WriteText(",");
                }
            }

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            if (keyColumn == null)
            {
                // Deliberately no ".End()" - see the comment in CreateExists for why a trailing
                // " ;" breaks Db2 regardless of statement type.
                return builder.GetString();
            }

            // Return the query, wrapped the same way as the single-row Insert: "SELECT <key> FROM
            // FINAL TABLE (<insert>)" returns every row the wrapped INSERT touched - for a
            // multi-row VALUES list, that's all <batchSize> rows in one result set.
            //
            // NOTE ON ROW ORDER: unlike CreateMergeAll's follow-up SELECT (which can freely
            // project an explicit per-row order/index value, since it's a plain SELECT the caller
            // fully controls), FINAL TABLE's result columns are restricted to the target table's
            // own columns - there's no way to also project a synthetic "this was VALUES row #3"
            // marker through it. This mirrors PostgreSqlStatementBuilder.CreateInsertAll in this
            // same solution, which likewise trusts that a multi-row VALUES/RETURNING round trip
            // preserves input row order without an explicit order column, and
            // RepoDb.Core's own InsertAll/InsertAllAsync execution loop already falls back to
            // positional correlation (`reader.Read()` order == `batchItems` order) whenever a
            // result set carries only a single column, as this one does. Not verified against a
            // live Db2 instance - if Db2 ever reorders FINAL TABLE's result rows relative to the
            // source VALUES list for a case like this, generated identity values would be
            // assigned to the wrong entities. Verify thoroughly before relying on this in
            // production; see the "Known limitations" section of the package README.
            return WrapWithReturningResult(builder.GetString(), keyColumn);
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

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => qualifiers.Any(qf => string.Equals(qf.Name, field.Name, StringComparison.OrdinalIgnoreCase)) != true &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();
            builder.Clear()
                .Merge()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .WriteText("T")
                .Using()
                .OpenParen()
                .Select()
                .WriteText(BuildCastedParametersAsFields(fields, DbSetting))
                .From()
                .WriteText("SYSIBM.SYSDUMMY1")
                .CloseParen()
                .WriteText("S")
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(field => field.AsJoinQualifier("S", "T", true, DbSetting))
                    .Join(" AND "))
                .CloseParen()
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", DbSetting)
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                .CloseParen();

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            return (keyColumn == null) ?
                builder.GetString() :
                WrapMergeWithReturningResult(builder.GetString(), tableName, keyColumn, qualifiers, identityField);
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

            if (batchSize <= 1)
            {
                // Single row: reuse the already-correct, already-tested single-row Merge
                // statement rather than duplicating it below for the batchSize-1 case.
                return CreateMerge(tableName, fields, qualifiers, primaryField, identityField, hints);
            }

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

            // TODO: A DB2 limitation. The batched MergeAll can only safely correlate each returned key back to the entity.
            // It is important to test and verify the behavior, or else, uncomment the error handler below.
            if (identityField != null &&
                qualifiers.Any(qf => string.Equals(qf.Name, identityField.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotSupportedException(
                    $"MergeAll cannot batch multiple rows into a single round-trip for '{tableName}' " +
                    $"when the identity column ('{identityField.Name}') is used as a qualifier - a " +
                    "freshly-inserted row's generated identity value can't be safely correlated back " +
                    "to a specific entity within a batch that may mix matched and unmatched rows. " +
                    "Pass an explicit non-identity qualifier (e.g. a natural key) to MergeAll, or call " +
                    "it with a batch size of 1.");
            }

            // Get the insertable and updateable fields
            var insertableFields = fields
                .Where(field => !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));
            var updateableFields = fields
                .Where(field => qualifiers.Any(qf => string.Equals(qf.Name, field.Name, StringComparison.OrdinalIgnoreCase)) != true &&
                    !string.Equals(field.Name, identityField?.Name, StringComparison.OrdinalIgnoreCase));

            // Build the query. Extends the single-row CreateMerge's USING clause to a
            // <batchSize>-row source by UNION ALL-ing one "SELECT ... FROM SYSIBM.SYSDUMMY1" per
            // row - Db2's MERGE then matches/inserts each source row against the target
            // independently, same as it would across <batchSize> separate single-row MERGE calls.
            var builder = new QueryBuilder();
            builder.Clear()
                .Merge()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .WriteText("T")
                .Using()
                .OpenParen();

            for (var index = 0; index < batchSize; index++)
            {
                builder
                    .Select()
                    .WriteText(BuildCastedParametersAsFields(fields, index, DbSetting))
                    .From()
                    .WriteText("SYSIBM.SYSDUMMY1");

                if (index < batchSize - 1)
                {
                    builder.WriteText("UNION ALL");
                }
            }

            builder
                .CloseParen()
                .WriteText("S")
                .On()
                .OpenParen()
                .WriteText(qualifiers
                    .Select(field => field.AsJoinQualifier("S", "T", true, DbSetting))
                    .Join(" AND "))
                .CloseParen()
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "T", "S", DbSetting)
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S", DbSetting)
                .CloseParen();

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            if (keyColumn == null)
            {
                return builder.GetString();
            }

            // Return the query, wrapped so every row's matched/generated key can flow back
            // through the same ExecuteScalar()/ExecuteReader()-based pipeline RepoDb.Core uses
            // for batched operations - see WrapMergeAllWithReturningResult.
            return WrapMergeAllWithReturningResult(builder.GetString(), tableName, keyColumn, qualifiers, batchSize);
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
            string hints = null)
        {
            EnsureParameters(where);
            return TrimTrailingSemicolon(base.CreateUpdate(tableName, fields, where, primaryField, identityField, hints));
        }

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
            // The base implementation already builds exactly what's needed for a batched
            // UpdateAll: <batchSize> concatenated "UPDATE ... WHERE ... ;" statements (one per
            // row, each with its own index-suffixed parameters), which Db2 executes as one round
            // trip via ExecuteNonQuery() - no per-row result correlation is needed here (unlike
            // InsertAll/MergeAll) since UpdateAll never reads a generated value back, only an
            // aggregate affected-row count. The base implementation also already calls
            // ValidateMultipleStatementExecution(batchSize) internally, which is a no-op now that
            // Db2DbSetting.IsMultiStatementExecutable is true - no need to duplicate that guard
            // here. TrimTrailingSemicolon only strips the very last "; " the base class's loop
            // appends - the internal ones separating each row's UPDATE statement are untouched.
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
        public override string CreateTruncate(string tableName) =>
            // Db2 requires the trailing IMMEDIATE keyword on a bare "TRUNCATE TABLE t" statement -
            // unlike SQL Server/MySql/PostgreSql, omitting it is a syntax error (SQL0104N,
            // "... Expected tokens may include: IMMEDIATE"). IMMEDIATE only became optional in Db2
            // 11.5 Mod Pack 2+; this provider targets 10.5+, so it's always included for broad
            // version compatibility (it remains valid, non-deprecated syntax on newer versions too).
            string.Concat(TrimTrailingSemicolon(base.CreateTruncate(tableName)), " IMMEDIATE");

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
        /// Creates a SQL Statement for max operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for max operation.</returns>
        public override string CreateMax(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMax(tableName, field, where, hints));

        #endregion

        #region CreateMaxAll

        /// <summary>
        /// Creates a SQL Statement for max-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be maximized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for max-all operation.</returns>
        public override string CreateMaxAll(string tableName,
            Field field,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMaxAll(tableName, field, hints));

        #endregion

        #region CreateMin

        /// <summary>
        /// Creates a SQL Statement for min operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for min operation.</returns>
        public override string CreateMin(string tableName,
            Field field,
            QueryGroup where = null,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMin(tableName, field, where, hints));

        #endregion

        #region CreateMinAll

        /// <summary>
        /// Creates a SQL Statement for min-all operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be minimized.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for min-all operation.</returns>
        public override string CreateMinAll(string tableName,
            Field field,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateMinAll(tableName, field, hints));

        #endregion

        #region CreateSum

        /// <summary>
        /// Creates a SQL Statement for sum operation.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="field">The field to be summed.</param>
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
        /// <param name="field">The field to be summed.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <returns>A sql statement for sum-all operation.</returns>
        public override string CreateSumAll(string tableName,
            Field field,
            string hints = null) =>
            TrimTrailingSemicolon(base.CreateSumAll(tableName, field, hints));

        #endregion

        #region Helpers

        private static readonly IResolver<Type, DbType?> m_clientTypeToDbTypeResolver = new ClientTypeToDbTypeResolver();
        private static readonly IResolver<DbType, string> m_dbTypeToStringNameResolver = new DbTypeToDb2StringNameResolver();

        /// <summary>
        /// Builds the SELECT list for a MERGE statement's USING source subquery
        /// (<c>SELECT &lt;this&gt; FROM SYSIBM.SYSDUMMY1</c>), casting each bind variable to its
        /// column's Db2 type before aliasing it to the field name - e.g.
        /// <c>CAST(:Field1 AS INTEGER) AS "Field1"</c> rather than the bare <c>:Field1 AS "Field1"</c>
        /// that <see cref="QueryBuilder.ParametersAsFieldsFrom(IEnumerable{Field}, int, IDbSetting)"/>
        /// (used by every other provider's MERGE/upsert equivalent) produces.
        /// <para>
        /// Confirmed necessary against a live Db2 LUW instance: without the CAST, PREPARE fails
        /// with SQL0418N ("... an invalid use of ... an untyped parameter marker ..."). Db2 cannot
        /// infer a parameter marker's type just from it being selected-and-aliased in a derived
        /// table that a MERGE's ON/UPDATE SET clauses later reference by that alias - unlike, say,
        /// a marker compared directly against a typed column, which is why the qualifier lookup in
        /// <see cref="WrapMergeWithReturningResult"/> doesn't need this treatment.
        /// </para>
        /// <para>
        /// Fields with no known <see cref="Field.Type"/> (possible when a caller builds a
        /// <see cref="Field"/> list by hand rather than letting RepoDb.Core resolve it from the
        /// entity's mapped properties) fall back to the bare, uncast form - the common
        /// RepoDb.Core-driven path always supplies a type here.
        /// </para>
        /// <para>
        /// LOB-family targets (<see cref="DbType.Binary"/>, <see cref="DbType.Object"/>,
        /// <see cref="DbType.Xml"/> - i.e. whatever <see cref="DbTypeToDb2StringNameResolver"/> maps
        /// to <c>BLOB(1M)</c>/<c>XML</c>) are deliberately excluded from the CAST and left bare.
        /// Confirmed live: a CLR <c>byte[]</c> property can map to either a genuine LOB column
        /// (e.g. "ColumnBlob" BLOB(1M)) or a plain short binary column (e.g. "ColumnRaw"
        /// VARCHAR(500) FOR BIT DATA) - <see cref="Field.Type"/> can't tell these apart, it's just
        /// <c>typeof(byte[])</c> either way. CASTing the latter's parameter to <c>BLOB(1M)</c> broke
        /// the driver's own parameter marshaling with SQL0301N ("... cannot be used because of its
        /// data type") from <c>DB2Command.StreamInputParameters</c> - LOB and non-LOB parameters are
        /// streamed to the server differently, and the CAST's declared type has to match how the
        /// *value* was actually bound, not just be type-compatible with it in the abstract. Since
        /// there's no reliable way to tell "ColumnRaw" and "ColumnBlob" apart from here, both are
        /// left uncast rather than risk asserting the wrong binding style for either.
        /// </para>
        /// <para>
        /// XML columns are unsupported through this path entirely (there is no CAST/wrapping that
        /// makes them work here - see git history for a since-abandoned attempt via a dedicated
        /// wrapper type). A data entity property mapped to a Db2 XML column should not be included
        /// in a <c>Merge</c>/<c>MergeAll</c> call's fields; <c>Insert</c>/<c>Update</c>/<c>Query</c>
        /// are unaffected.
        /// </para>
        /// </summary>
        private static string BuildCastedParametersAsFields(IEnumerable<Field> fields,
            IDbSetting dbSetting) =>
            BuildCastedParametersAsFields(fields, 0, dbSetting);

        /// <summary>
        /// Same as <see cref="BuildCastedParametersAsFields(IEnumerable{Field}, IDbSetting)"/>, but
        /// for a specific row's <paramref name="index"/> within a batched (InsertAll/MergeAll)
        /// statement - e.g. <c>CAST(:Field1_1 AS INTEGER) AS "Field1"</c> for <paramref name="index"/> 1.
        /// See <see cref="CreateMergeAll"/>.
        /// </summary>
        private static string BuildCastedParametersAsFields(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting) =>
            fields
                .Select(field => string.Concat(BuildCastedParameter(field, index, dbSetting), " AS ", field.Name.AsField(dbSetting)))
                .Join(", ");

        /// <summary>
        /// Same CAST logic as <see cref="BuildCastedParametersAsFields(IEnumerable{Field}, int, IDbSetting)"/>,
        /// but without the trailing <c>AS "Field"</c> alias - for a plain multi-row <c>VALUES (...), (...)</c>
        /// tuple list (see <see cref="CreateInsertAll"/>), where the target column list already provides
        /// the positional field names and an inline alias would be a syntax error.
        /// </summary>
        private static string BuildCastedParameters(IEnumerable<Field> fields,
            int index,
            IDbSetting dbSetting) =>
            fields
                .Select(field => BuildCastedParameter(field, index, dbSetting))
                .Join(", ");

        /// <summary>
        /// The single-parameter CAST logic shared by <see cref="BuildCastedParametersAsFields(IEnumerable{Field}, int, IDbSetting)"/>
        /// and <see cref="BuildCastedParameters(IEnumerable{Field}, int, IDbSetting)"/> - see the
        /// remarks on the former for why the CAST exists and why LOB-family/untyped fields are left bare.
        /// </summary>
        private static string BuildCastedParameter(Field field,
            int index,
            IDbSetting dbSetting)
        {
            var parameter = field.Name.AsParameter(index, dbSetting);
            var dbType = field.Type != null ? m_clientTypeToDbTypeResolver.Resolve(field.Type) : null;
            var isLobFamily = dbType is DbType.Binary or DbType.Object or DbType.Xml;

            return dbType != null && !isLobFamily
                ? string.Concat("CAST(", parameter, " AS ", m_dbTypeToStringNameResolver.Resolve(dbType.Value).ToUpperInvariant(), ")")
                : parameter;
        }

        /// <summary>
        /// RepoDb.Core's <c>Update</c> operation prepends <c>StringConstant.UpdateParameterPrefix</c>
        /// ("m_") to every WHERE-clause parameter name before any <see cref="IStatementBuilder"/> runs
        /// (<c>QueryField.IsForUpdate()</c> -&gt; <c>Parameter.PrependText(...)</c> in RepoDb.Core), to
        /// guarantee a WHERE-clause bind variable can never collide with a same-named SET-clause one.
        /// This prefix starts with a letter, so it is legal for every provider's bind-variable syntax,
        /// including Db2's (<c>:m_Id</c> is fine; an earlier, no-longer-current version of this
        /// constant was a bare underscore, which Db2 rejected with a syntax error since Db2 host
        /// variables must start with a letter).
        /// <para>
        /// This call is a defensive no-op in the normal end-to-end path (<c>Parameter.PrependText</c> is
        /// idempotent - it only prepends if the name doesn't already start with the prefix, and Core has
        /// already applied it by the time this statement builder runs). It's kept here so
        /// <see cref="CreateUpdate"/> also behaves correctly if ever invoked directly against a
        /// <see cref="QueryGroup"/> that hasn't been through <c>IsForUpdate()</c> yet.
        /// </para>
        /// </summary>
        private static void EnsureParameters(QueryGroup where)
        {
            if (where == null)
            {
                return;
            }
            foreach (var queryField in where.GetFields(true))
            {
                queryField.Parameter.PrependText(StringConstant.UpdateParameterPrefix);
            }
        }

        /// <summary>
        /// Every <c>Create*</c> method in <see cref="BaseStatementBuilder"/> ends its generated SQL with
        /// <c>QueryBuilder.End()</c>, which unconditionally appends <c>" ;"</c>. SQL Server/PostgreSQL's
        /// drivers tolerate a trailing semicolon on an ordinary (non-compound-statement) command sent via
        /// <c>ExecuteNonQuery()</c>/<c>ExecuteScalar()</c>, but the IBM Data Server .NET Provider does
        /// not - it fails with a syntax error. Strip it for every base-inherited method
        /// (Update/UpdateAll/Delete/DeleteAll/Truncate) and before manually appending the
        /// "SELECT ... FROM FINAL TABLE (...)" wrapper on Insert/Merge.
        /// </summary>
        private static string TrimTrailingSemicolon(string sql) =>
            sql?.TrimEnd().TrimEnd(';').TrimEnd();

        /// <summary>
        /// Wraps a single INSERT statement (without its trailing semicolon) so that the value of
        /// <paramref name="keyColumn"/> flows back to the caller as an ordinary result set, via
        /// Db2's "SELECT ... FROM FINAL TABLE (...)" data-change-table-reference. The row produced
        /// by the wrapped INSERT - including any Db2-generated identity column value - becomes a
        /// queryable result table whose columns can be referenced in the outer SELECT's column
        /// list. This is exactly what RepoDb.Core's ExecuteScalar()-based Insert pipeline needs,
        /// with no PL/SQL block, OUT parameter, or cursor plumbing required (unlike Oracle's
        /// RETURNING ... INTO). Confirmed working against a live Db2 LUW instance.
        /// <para>
        /// INSERT-only: Db2 LUW's MERGE statement does not support FINAL TABLE (confirmed via
        /// SQL0104N on a live instance - see the remarks on <see cref="WrapMergeWithReturningResult"/>
        /// for the mechanism used for MERGE instead).
        /// </para>
        /// </summary>
        private string WrapWithReturningResult(string dmlStatementWithoutTrailingSemicolon,
            DbField keyColumn)
        {
            var quotedKeyColumn = keyColumn.Name.AsQuoted(DbSetting);

            return string.Concat("SELECT ", quotedKeyColumn,
                " FROM FINAL TABLE (", dmlStatementWithoutTrailingSemicolon, ")");
        }

        /// <summary>
        /// Appends a follow-up SELECT to a MERGE statement (as a second statement in the same
        /// command text, separated by ";") so that the merged/matched row's <paramref name="keyColumn"/>
        /// value flows back to the caller through the same ExecuteScalar()-based pipeline
        /// RepoDb.Core uses for every provider. Db2 LUW's MERGE does not support FINAL TABLE (see
        /// the remarks on <see cref="WrapWithReturningResult"/>), so the key has to be re-queried
        /// after the MERGE completes instead of being read directly off it.
        /// <para>
        /// This relies on the IBM Data Server .NET Provider accepting more than one statement in a
        /// single command text and executing all of them in one round trip (confirmed for a
        /// "SELECT ...; SELECT ...;" read-only batch - see ExecuteQueryMultipleTest.cs.
        /// ExecuteScalar() itself only ever reads the *first* result set of a multi-statement
        /// batch, which is why the SELECT is appended *after* the MERGE, not before). A single
        /// command text sends its parameters once; the same named parameter marker (e.g. ":Id")
        /// can be referenced again in the trailing SELECT and is bound to the same value the MERGE
        /// used, without declaring a duplicate parameter.
        /// </para>
        /// <para>
        /// The re-query predicate is the same qualifier predicate as the MERGE's own ON clause
        /// (e.g. "WHERE ("Id" = :Id)"), so it deterministically finds whichever row the MERGE just
        /// touched - <c>except</c> when the qualifier field is itself the auto-generated identity
        /// column and the row was newly inserted: the caller's bound value for that parameter is
        /// whatever placeholder/default they passed in (not the real generated value), so the
        /// lookup by that value would find nothing.
        /// </para>
        /// <para>
        /// <b>This used to fall back to <c>IDENTITY_VAL_LOCAL()</c> for that case. Confirmed live,
        /// that was wrong</b>: IBM's docs state <c>IDENTITY_VAL_LOCAL()</c> returns NULL "if ...a
        /// COMMIT or ROLLBACK of a unit of work occurred since the most recent qualifying data
        /// change statement" - and Db2 LUW's autocommit semantics commit after *each individual
        /// SQL statement*, not after the client's round trip/command text as a whole. So even
        /// though the MERGE and the follow-up SELECT are sent together in one command text/round
        /// trip, the MERGE's own autocommit fires between them, clearing the identity register
        /// before the SELECT ever reads it - <c>IDENTITY_VAL_LOCAL()</c> came back NULL 100% of the
        /// time for a fresh INSERT, which <c>COALESCE</c> then propagated as an overall NULL,
        /// silently read back as 0 for every merged entity's identity property
        /// (<c>MergeAllForEmptyTable</c> failing "table.Id &gt; 0" was the symptom).
        /// </para>
        /// <para>
        /// The fallback here instead re-reads <c>MAX(&lt;identity column&gt;)</c> off the table
        /// itself - ordinary committed data, unaffected by the autocommit-clears-the-register
        /// problem above, since the row is already durably committed by the time this SELECT runs.
        /// This is a best-effort approximation, not a guarantee: it assumes no other connection
        /// concurrently inserts into the same table between the MERGE and this SELECT. That's an
        /// inherent limitation of not having a true FINAL TABLE/OUTPUT-clause equivalent for MERGE
        /// on Db2 LUW - see the "Known limitations" section of the package README.
        /// </para>
        /// </summary>
        private string WrapMergeWithReturningResult(string mergeStatementWithoutTrailingSemicolon,
            string tableName,
            DbField keyColumn,
            IEnumerable<Field> qualifiers,
            DbField identityField)
        {
            var quotedKeyColumn = keyColumn.Name.AsQuoted(DbSetting);

            var selectBuilder = new QueryBuilder();
            selectBuilder.Clear()
                .Select()
                .WriteText("COALESCE((")
                .Select()
                .WriteText(quotedKeyColumn)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(qualifiers, 0, DbSetting)
                .WriteText("),");

            if (identityField != null)
            {
                selectBuilder
                    .WriteText("(")
                    .Select()
                    .WriteText("MAX(").WriteText(quotedKeyColumn).WriteText(")")
                    .From()
                    .TableNameFrom(tableName, DbSetting)
                    .WriteText("))");
            }
            else
            {
                selectBuilder.WriteText("NULL)");
            }

            selectBuilder
                .From()
                .WriteText("SYSIBM.SYSDUMMY1");

            return string.Concat(mergeStatementWithoutTrailingSemicolon, "; ",
                TrimTrailingSemicolon(selectBuilder.GetString()));
        }

        /// <summary>
        /// Appends a follow-up SELECT to a batched (<paramref name="batchSize"/> &gt; 1) MERGE
        /// statement so that every row's matched/generated <paramref name="keyColumn"/> value flows
        /// back to the caller through RepoDb.Core's ExecuteReader()-based batched-operation
        /// pipeline. <see cref="CreateMergeAll"/> already guarantees, before calling this method,
        /// that the identity column (if any) is never part of <paramref name="qualifiers"/> - so
        /// unlike <see cref="WrapMergeWithReturningResult"/>, every row here is guaranteed
        /// findable by its own qualifier value alone (whether it was just matched-and-updated, or
        /// just inserted using a caller-supplied, non-identity qualifier value) - no
        /// MAX(<c>&lt;key&gt;</c>)/IDENTITY_VAL_LOCAL()-style fallback is needed at all.
        /// <para>
        /// Each row's lookup is independent - a plain UNION ALL of <paramref name="batchSize"/>
        /// single-row SELECTs, one per original entity index - so nothing here depends on Db2
        /// preserving any particular row order. RepoDb.Core's batched-operation execution loop
        /// (<c>InsertAllExecutionContextProvider</c>/<c>MergeAllExecutionContextProvider</c> et al.
        /// via <c>DbConnectionExtension.AddOrderColumnParameters</c>) already binds an
        /// <c>:__RepoDb_OrderColumn_&lt;index&gt;</c> parameter per row for exactly this purpose;
        /// each branch below echoes its own back as an explicit second result column, so the
        /// caller's `reader.GetInt32(1)` correlation always matches the right row to the right
        /// entity regardless of the order Db2 actually returns them in.
        /// </para>
        /// <para>
        /// The echoed order-column marker is wrapped in <c>CAST(... AS INTEGER)</c> for the same
        /// reason <see cref="BuildCastedParameter"/> exists: confirmed live, a bare
        /// <c>:__RepoDb_OrderColumn_&lt;index&gt; AS "..."</c> - selected-and-aliased with no
        /// comparison against a typed column - fails PREPARE with SQL0418N ("... an invalid use of
        /// ... an untyped parameter marker ..."), the exact same class of error the MERGE USING
        /// clause's CAST already works around. The order column is always a plain 0-based row
        /// index, so it's always safe to CAST as INTEGER (no LOB-family concern here).
        /// </para>
        /// </summary>
        private string WrapMergeAllWithReturningResult(string mergeStatementWithoutTrailingSemicolon,
            string tableName,
            DbField keyColumn,
            IEnumerable<Field> qualifiers,
            int batchSize)
        {
            var quotedKeyColumn = keyColumn.Name.AsQuoted(DbSetting);
            var quotedOrderColumn = "__RepoDb_OrderColumn".AsQuoted(DbSetting);

            var selectBuilder = new QueryBuilder();
            selectBuilder.Clear();

            for (var index = 0; index < batchSize; index++)
            {
                selectBuilder
                    .Select()
                    .WriteText(quotedKeyColumn)
                    .WriteText(",")
                    .WriteText($"CAST({DbSetting.ParameterPrefix}__RepoDb_OrderColumn_{index} AS INTEGER)")
                    .As(quotedOrderColumn)
                    .From()
                    .TableNameFrom(tableName, DbSetting)
                    .WhereFrom(qualifiers, index, DbSetting);

                if (index < batchSize - 1)
                {
                    selectBuilder.WriteText("UNION ALL");
                }
            }

            selectBuilder
                .OrderBy()
                .WriteText(quotedOrderColumn);

            return string.Concat(mergeStatementWithoutTrailingSemicolon, "; ",
                TrimTrailingSemicolon(selectBuilder.GetString()));
        }

        #endregion
    }
}
