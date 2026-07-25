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
    /// A class used to build a SQL Statement for Oracle. Targets Oracle Database 12c and later.
    /// </summary>
    public sealed class OracleStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleStatementBuilder"/> object.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        public OracleStatementBuilder(IDbSetting dbSetting)
            : this(dbSetting,
                new OracleConvertFieldResolver(),
                new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="OracleStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public OracleStatementBuilder(IDbSetting dbSetting,
            IResolver<Field, IDbSetting, string> convertFieldResolver = null,
            IResolver<Type, Type> averageableClientTypeResolver = null)
            : base(dbSetting,
                  (convertFieldResolver ?? new OracleConvertFieldResolver()),
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

            // Build the query. Oracle has no "TOP" keyword; "FETCH FIRST n ROWS ONLY" (12c+) is
            // the ANSI-standard equivalent and must be placed at the end of the statement.
            builder.Clear()
                .Select()
                .WriteText(string.Concat("1 AS ", "ExistsValue".AsQuoted(DbSetting)))
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .WriteText("FETCH FIRST 1 ROWS ONLY");

            // Return the query. NOTE: deliberately no ".End()" here - it appends a trailing " ;",
            // which Oracle rejects with "ORA-00911: invalid character after" on any plain
            // (non-PL/SQL-block) statement, SELECT included, regardless of execute method.
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

            // Build the query. Oracle's "TOP"-equivalent ("FETCH FIRST n ROWS ONLY") is a
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
            // a trailing " ;" breaks Oracle regardless of statement type.
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
            // supported unchanged on Oracle Database 12c and later.
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
            // for this), Oracle's OFFSET/FETCH (12c+) directly supports an arbitrary skip/take.
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
                // yields no rows in Oracle (no error), so no further wrapping is necessary - but
                // the base statement still ends in " ;", which Oracle rejects on any plain
                // (non-PL/SQL-block) statement (ORA-00911), so it still needs trimming.
                return TrimTrailingSemicolon(insertStatement);
            }

            // Return the query, wrapped so the generated key can flow back through the same
            // ExecuteScalar()-based pipeline RepoDb.Core uses for every provider.
            return WrapWithReturningResult(TrimTrailingSemicolon(insertStatement), tableName, keyColumn);
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
            // OracleDbSetting.IsMultiStatementExecutable is false, so RepoDb.Core always forces
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

            // Build the query. Oracle requires "MERGE INTO" (not just "MERGE") and requires the
            // USING source subquery to have a FROM clause even when only selecting bind
            // variables/constants (hence "FROM DUAL"). NOTE: unlike a SELECT's column aliases,
            // Oracle's MERGE syntax does NOT accept the "AS" keyword before a table/subquery
            // alias - "MERGE INTO t AS T" is illegal and fails to parse (every alias in Oracle's
            // own MERGE examples is bare, e.g. "MERGE INTO bonuses D USING (...) S ON (...)").
            // Using ".As(...)" here previously produced "AS T"/"AS S", which Oracle's parser
            // rejected with a confusing "ORA-38107: Invalid syntax with MERGE without USING
            // clause" pointing at the start of the statement - it aborts as soon as it hits the
            // unexpected "AS" token, before it ever reaches the (perfectly valid) USING clause.
            builder.Clear()
                .Merge()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .WriteText("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(fields, 0, DbSetting)
                .From()
                .WriteText("DUAL")
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
                // for why a trailing " ;" breaks Oracle regardless of statement type.
                return builder.GetString();
            }

            // Return the query, wrapped so the generated/matched key can flow back through the
            // same ExecuteScalar()-based pipeline RepoDb.Core uses for every provider.
            // IMPORTANT: a RETURNING clause on MERGE is only supported starting with Oracle
            // Database 23ai - NOT 12.2 as originally assumed here. On 12c/18c/19c/21c this will
            // fail (with a different error, ORA-00933, since those versions don't parse RETURNING
            // after MERGE at all). This provider targets 12c+ for every other operation, but
            // Merge-with-identity-retrieval specifically requires 23ai+. If you're on an older
            // version, either omit the primary/identity field from the qualifiers so no RETURNING
            // is requested (see the keyColumn == null branch above), or avoid Merge for
            // identity-generating tables and use Insert instead.
            return WrapWithReturningResult(builder.GetString(), tableName, keyColumn);
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
            // internally, which throws given OracleDbSetting.IsMultiStatementExecutable == false
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
            TrimTrailingSemicolon(base.CreateTruncate(tableName));

        #endregion

        #region Helpers

        /// <summary>
        /// RepoDb.Core's <c>Update</c> operation unconditionally prepends an underscore to every
        /// WHERE-clause parameter name before any <see cref="IStatementBuilder"/> runs (see
        /// <c>Parameter.PrependAnUnderscore()</c> in RepoDb.Core), to guarantee it can never collide
        /// with a same-named SET-clause bind parameter. SQL Server/PostgreSQL tolerate the resulting
        /// name (e.g. <c>@_Id</c> is a legal parameter name in both dialects), but Oracle bind
        /// variables must start with a letter - <c>:_Id</c> is illegal and fails with
        /// <c>ORA-00911: invalid character after</c>.
        /// <para>
        /// Renaming here (before calling <c>base.CreateUpdate</c>) mutates the same <see cref="Parameter"/>
        /// instance that RepoDb.Core reads again afterward when it creates the actual bound
        /// <c>OracleParameter</c>, so the SQL text and the bound parameter name stay in sync. This
        /// requires <c>Parameter.SetName</c> (internal), hence the <c>InternalsVisibleTo</c> entry for
        /// "RepoDb.Oracle" added to RepoDb.Core's csproj.
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
        /// drivers tolerate a trailing semicolon on an ordinary (non-PL/SQL-block) statement sent via
        /// <c>ExecuteNonQuery()</c>/<c>ExecuteScalar()</c>, but Oracle's OCI/ODP.NET layer does not -
        /// it fails with <c>ORA-00911: invalid character after</c>. Strip it for every base-inherited
        /// method (Update/UpdateAll/Delete/DeleteAll/Truncate) and before manually appending the
        /// RETURNING/DBMS_SQL.RETURN_RESULT wrapper on Insert/Merge.
        /// </summary>
        private static string TrimTrailingSemicolon(string sql) =>
            sql?.TrimEnd().TrimEnd(';').TrimEnd();

        /// <summary>
        /// Wraps a single DML statement (INSERT or MERGE, without its trailing semicolon) so that
        /// the value of <paramref name="keyColumn"/> - captured via Oracle's native
        /// "RETURNING ... INTO ..." clause - flows back to the caller as an Oracle 12c+ implicit
        /// result set (<c>DBMS_SQL.RETURN_RESULT</c>). Oracle's RETURNING clause only binds to a
        /// PL/SQL variable/OUT parameter; it cannot, by itself, produce a result set that
        /// <c>ExecuteScalar()</c> can read the way SQL Server's trailing SELECT or PostgreSql's
        /// RETURNING-as-resultset can. Implicit result sets are Oracle's mechanism for exposing
        /// PL/SQL results to ordinary result-set-reading client code without any special output
        /// parameter handling, which is exactly what RepoDb.Core's ExecuteScalar()-based Insert/
        /// Merge pipeline needs. This is the least-proven part of this provider - verify it
        /// against a real Oracle instance before relying on it in production.
        /// </summary>
        private string WrapWithReturningResult(string dmlStatementWithoutTrailingSemicolon,
            string tableName,
            DbField keyColumn)
        {
            var quotedTable = tableName.AsQuoted(true, DbSetting);
            var quotedKeyColumn = keyColumn.Name.AsQuoted(DbSetting);
            var resultAlias = "Result".AsQuoted(DbSetting);

            // NOTE: DBMS_SQL.RETURN_RESULT takes a SYS_REFCURSOR argument. A `CURSOR(SELECT ...)`
            // expression is a SQL-only construct (e.g. valid inside a SELECT's column list or as a
            // table function argument) and is NOT allowed as a PL/SQL procedure-call argument -
            // passing it directly here fails with "PLS-00405: subquery not allowed in this context".
            // The cursor must instead be OPENed into a local SYS_REFCURSOR variable first, then that
            // variable is passed to DBMS_SQL.RETURN_RESULT.
            return string.Concat(
                "DECLARE l_repodb_result ", quotedTable, ".", quotedKeyColumn, "%TYPE; ",
                "l_repodb_cursor SYS_REFCURSOR; ",
                "BEGIN ",
                dmlStatementWithoutTrailingSemicolon, " RETURNING ", quotedKeyColumn, " INTO l_repodb_result; ",
                "OPEN l_repodb_cursor FOR SELECT l_repodb_result AS ", resultAlias, " FROM DUAL; ",
                "DBMS_SQL.RETURN_RESULT(l_repodb_cursor); ",
                "END;");
        }

        #endregion
    }
}
