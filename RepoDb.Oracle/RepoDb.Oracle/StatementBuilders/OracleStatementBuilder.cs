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
                .WriteText("FETCH FIRST 1 ROWS ONLY")
                .End();

            // Return the query
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
            builder.End();

            // Return the query
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
                .WriteText(string.Concat("ROWS FETCH NEXT ", rowsPerBatch, " ROWS ONLY"))
                .End();

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
                .WriteText(string.Concat("ROWS FETCH NEXT ", take, " ROWS ONLY"))
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
                // yields no rows in Oracle (no error), so no further wrapping is necessary.
                return insertStatement;
            }

            // The base statement always ends in " ;" (see QueryBuilder.End()); drop the
            // semicolon so the RETURNING clause can be appended before it.
            var withoutTrailingSemicolon = insertStatement.Substring(0, insertStatement.Length - 1).TrimEnd();

            // Return the query, wrapped so the generated key can flow back through the same
            // ExecuteScalar()-based pipeline RepoDb.Core uses for every provider.
            return WrapWithReturningResult(withoutTrailingSemicolon, tableName, keyColumn);
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
            // variables/constants (hence "FROM DUAL").
            builder.Clear()
                .Merge()
                .Into()
                .TableNameFrom(tableName, DbSetting)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(fields, 0, DbSetting)
                .From()
                .WriteText("DUAL")
                .CloseParen()
                .As("S")
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
                return builder.End().GetString();
            }

            // Return the query, wrapped so the generated/matched key can flow back through the
            // same ExecuteScalar()-based pipeline RepoDb.Core uses for every provider. Note: Oracle
            // only supports a RETURNING clause on MERGE starting with 12.2, and only when the
            // statement affects exactly one row (RepoDb's Merge() operation always targets one
            // entity, so this holds) - verify against your own Oracle version.
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

        #region Helpers

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

            return string.Concat(
                "DECLARE l_repodb_result ", quotedTable, ".", quotedKeyColumn, "%TYPE; ",
                "BEGIN ",
                dmlStatementWithoutTrailingSemicolon, " RETURNING ", quotedKeyColumn, " INTO l_repodb_result; ",
                "DBMS_SQL.RETURN_RESULT(CURSOR(SELECT l_repodb_result AS ", resultAlias, " FROM DUAL)); ",
                "END;");
        }

        #endregion
    }
}
