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
            // Db2DbSetting.IsMultiStatementExecutable is false, so RepoDb.Core always forces
            // batchSize down to 1 before calling this method (true multi-row batching into a
            // single round-trip is not implemented yet - see the "Known limitations" section of
            // the package README). Guard defensively anyway, then reuse the single-row Insert
            // statement, which already produces parameters/RETURNING wiring for index 0.
            ValidateMultipleStatementExecution(batchSize);

            return CreateInsert(tableName, fields, primaryField, identityField, hints);
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

            // Build the query. Db2 requires "MERGE INTO" (not just "MERGE") and requires the
            // USING source subquery to have a FROM clause even when only selecting bind
            // variables/constants. NOTE: this used to write "FROM DUAL" here - Oracle's
            // universally-available single-row dummy table - on the (unverified, wrong)
            // assumption that Db2 has the same thing. It doesn't: confirmed live against Db2 LUW,
            // "FROM DUAL" fails with SQL0204N ("DB2INST1.DUAL is an undefined name"). Db2's
            // equivalent is the catalog table SYSIBM.SYSDUMMY1 (already relied on elsewhere in
            // this provider - see ExecuteQueryMultipleTest.cs and WrapMergeWithReturningResult
            // below), referenced unquoted/schema-qualified like any other system catalog object.
            // Also: unlike a SELECT's column aliases, Db2's MERGE syntax does NOT accept the "AS"
            // keyword before a table/subquery alias - "MERGE INTO t AS T" is illegal and fails to
            // parse (every alias in Db2's own MERGE examples is bare, e.g. "MERGE INTO bonuses D
            // USING (...) S ON (...)"). Using ".As(...)" here previously produced "AS T"/"AS S",
            // which Db2's parser rejected with a confusing "ORA-38107: Invalid syntax with MERGE
            // without USING clause" pointing at the start of the statement - it aborts as soon as
            // it hits the unexpected "AS" token, before it ever reaches the (perfectly valid)
            // USING clause.
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

            if (keyColumn == null)
            {
                // Deliberately no ".End()" - see the comment in CreateExists/TrimTrailingSemicolon
                // for why a trailing " ;" breaks Db2 regardless of statement type.
                return builder.GetString();
            }

            // Return the query, wrapped so the generated/matched key can flow back through the
            // same ExecuteScalar()-based pipeline RepoDb.Core uses for every provider.
            //
            // NOTE: this used to reuse the same "SELECT ... FROM FINAL TABLE (...)" wrapper as
            // CreateInsert, on the (unverified, wrong) assumption that Db2's data-change-table-
            // reference works identically for MERGE as it does for INSERT. Verified against a
            // live Db2 LUW instance: it does not - "FROM FINAL TABLE (MERGE ...)" fails with
            // SQL0104N ("... Expected tokens may include: <insert-statement> ..."), and Db2 LUW's
            // official MERGE statement reference has no FINAL TABLE mention at all (this appears
            // to be a Db2 for z/OS-only extension). See WrapMergeWithReturningResult for the
            // actual (multi-statement SELECT-based) mechanism used instead.
            return WrapMergeWithReturningResult(builder.GetString(), tableName, keyColumn, qualifiers, identityField);
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
            // See the comment on CreateInsertAll: batching multiple MERGE statements (and their
            // RETURNING values) into a single round-trip is not implemented yet, so RepoDb.Core
            // always calls this with batchSize == 1.
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
            // The base implementation already calls ValidateMultipleStatementExecution(batchSize)
            // internally, which throws given Db2DbSetting.IsMultiStatementExecutable == false
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
            fields
                .Select(field =>
                {
                    var parameter = field.Name.AsParameter(0, dbSetting);
                    var quotedField = field.Name.AsField(dbSetting);
                    var dbType = field.Type != null ? m_clientTypeToDbTypeResolver.Resolve(field.Type) : null;
                    var isLobFamily = dbType is DbType.Binary or DbType.Object or DbType.Xml;

                    var castedParameter = dbType != null && !isLobFamily
                        ? string.Concat("CAST(", parameter, " AS ", m_dbTypeToStringNameResolver.Resolve(dbType.Value).ToUpperInvariant(), ")")
                        : parameter;

                    return string.Concat(castedParameter, " AS ", quotedField);
                })
                .Join(", ");

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

        #endregion
    }
}
