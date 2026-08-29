using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.StatementBuilders
{
    /// <summary>
    /// A class used to build a SQL Statement for Firebird. Targets Firebird 3.0 and later.
    /// </summary>
    public sealed class FirebirdStatementBuilder : BaseStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="FirebirdStatementBuilder"/> object.
        /// </summary>
        public FirebirdStatementBuilder()
            : this(DbSettingMapper.Get<FbConnection>(),
                  new FirebirdConvertFieldResolver(),
                  new ClientTypeToAverageableClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdStatementBuilder"/> class.
        /// </summary>
        /// <param name="dbSetting">The database settings object currently in used.</param>
        /// <param name="convertFieldResolver">The resolver used when converting a field in the database layer.</param>
        /// <param name="averageableClientTypeResolver">The resolver used to identity the type for average.</param>
        public FirebirdStatementBuilder(IDbSetting dbSetting,
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

            // Build the query. Firebird has no "TOP"; "FIRST n" is Firebird's equivalent and is
            // written directly after SELECT (unlike MySQL/PostgreSql's trailing "LIMIT").
            builder.Clear()
                .Select()
                .WriteText("FIRST 1")
                .WriteText(string.Concat("1 AS ", "ExistsValue".AsQuoted(DbSetting)))
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting);

            // Return the query. Deliberately no ".End()" - see the remarks on TrimTrailingSemicolon:
            // Firebird's DSQL layer rejects a trailing ';' on a statement sent through FbCommand.
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

            // Build the query
            builder.Clear()
                .Select();
            if (top > 0)
            {
                builder.WriteText(string.Concat("FIRST ", top));
            }
            builder
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .HintsFrom(hints)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting);

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

            // Build the query. "FIRST m SKIP n" is Firebird's LIMIT/OFFSET equivalent, written
            // directly after SELECT.
            builder.Clear()
                .Select()
                .WriteText(string.Concat("FIRST ", rowsPerBatch, " SKIP ", skip))
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting);

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

            // Build the query
            builder.Clear()
                .Select()
                .WriteText(string.Concat("FIRST ", take, " SKIP ", skip))
                .FieldsFrom(fields, DbSetting)
                .From()
                .TableNameFrom(tableName, DbSetting)
                .WhereFrom(where, DbSetting)
                .OrderByFrom(orderBy, DbSetting);

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
            // Let the base implementation handle the guards/validation and produce the plain
            // "INSERT INTO ... VALUES ( ... ) ;" statement. It already excludes the identity field
            // from the column list, which is exactly what Firebird needs: a GENERATED ALWAYS/BY
            // DEFAULT AS IDENTITY column auto-populates when omitted from the statement.
            var insertStatement = TrimTrailingSemicolon(base.CreateInsert(tableName,
                fields,
                primaryField,
                identityField,
                hints));

            // Variables needed
            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);

            if (keyColumn == null)
            {
                return insertStatement;
            }

            // Firebird's RETURNING clause on INSERT natively produces a single-row result set that
            // FbCommand.ExecuteScalar()/ExecuteReader() can read directly - unlike Oracle, no PL/SQL
            // block or OUT-parameter/implicit-result-set wrapping is required here.
            return string.Concat(insertStatement,
                " RETURNING ", keyColumn.Name.AsQuoted(DbSetting), " AS ", "Result".AsQuoted(DbSetting));
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
            // FirebirdDbSetting.IsMultiStatementExecutable is false - Firebird's ADO.NET provider
            // (FbCommand) does not support executing multiple statements in one round-trip, so
            // RepoDb.Core always calls this with batchSize == 1 (see ValidateMultipleStatementExecution).
            // True multi-row batching is not implemented; reuse the single-row Insert statement.
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

            var keyColumn = GetReturnKeyColumnAsDbField(primaryField, identityField);
            var identityIsQualifier = identityField != null &&
                qualifiers.Any(qf => string.Equals(qf.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));
            if (identityIsQualifier)
            {
                return BuildMergeExecuteBlock(tableName, fields, qualifiers, identityField, keyColumn ?? identityField);
            }
            var insertableFields = identityField == null
                ? fields
                : fields.Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase));

            // Initialize the builder
            var builder = new QueryBuilder();

            // Build the query. UPDATE OR INSERT is Firebird's native single-statement upsert: it
            // matches on the MATCHING(...) column list (falling back to the table's primary key when
            // omitted) and either updates the matched row or inserts a new one - closest in shape to
            // MySQL's "ON DUPLICATE KEY UPDATE"/PostgreSql's "ON CONFLICT DO UPDATE" used by the
            // providers this was ported from, and simpler than an ANSI MERGE for a single-row upsert.
            builder.Clear()
                .WriteText("UPDATE OR INSERT INTO")
                .TableNameFrom(tableName, DbSetting)
                .OpenParen()
                .FieldsFrom(insertableFields, DbSetting)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(insertableFields, 0, DbSetting)
                .CloseParen()
                .WriteText("MATCHING")
                .OpenParen()
                .FieldsFrom(qualifiers, DbSetting)
                .CloseParen();

            if (keyColumn != null)
            {
                // Firebird 3.0+ supports RETURNING on UPDATE OR INSERT natively, returning the value
                // (existing or newly generated) as an ordinary single-row result set.
                builder.WriteText(string.Concat("RETURNING ", keyColumn.Name.AsQuoted(DbSetting), " AS ", "Result".AsQuoted(DbSetting)));
            }

            // Return the query. Deliberately no ".End()" - see CreateExists.
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
            int batchSize = 1,
            DbField primaryField = null,
            DbField identityField = null,
            string hints = null)
        {
            // See the comment on CreateInsertAll - batching multiple UPDATE OR INSERT statements
            // (and their RETURNING values) into a single round-trip is not implemented, since
            // FirebirdDbSetting.IsMultiStatementExecutable is false.
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
            // internally, which throws given FirebirdDbSetting.IsMultiStatementExecutable == false
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

            // Firebird has no TRUNCATE TABLE statement (as of 5.0). DELETE FROM without a WHERE
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
        /// <c>QueryBuilder.End()</c>, which unconditionally appends <c>" ;"</c>. Firebird's DSQL layer
        /// treats the semicolon purely as an isql/script statement separator, not as part of the grammar
        /// for a single statement submitted through the API - sending one via <c>FbCommand.CommandText</c>
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
        /// <param name="identityField"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        private string BuildMergeExecuteBlock(string tableName,
            IEnumerable<Field> fields,
            IEnumerable<Field> qualifiers,
            DbField identityField,
            DbField keyColumn)
        {
            var fieldList = fields.AsList();
            var quotedTable = tableName.AsQuoted(true, DbSetting);
            var quotedKeyColumn = keyColumn.Name.AsQuoted(DbSetting);
            var paramNamesByField = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var sb = new StringBuilder("EXECUTE BLOCK (");

            for (var i = 0; i < fieldList.Count; i++)
            {
                var field = fieldList[i];
                var paramName = string.Concat("P", i);
                paramNamesByField[field.Name] = paramName;
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(paramName)
                    .Append(" TYPE OF COLUMN ").Append(quotedTable).Append('.').Append(field.Name.AsQuoted(DbSetting))
                    .Append(" = ").Append(field.Name.AsParameter(DbSetting));
            }

            sb.Append(") RETURNS (R0 TYPE OF COLUMN ").Append(quotedTable).Append('.').Append(quotedKeyColumn)
                .Append(") AS BEGIN ");

            var insertableFields = fieldList
                .Where(f => !string.Equals(f.Name, identityField.Name, StringComparison.OrdinalIgnoreCase))
                .AsList();
            string ColumnList(IEnumerable<Field> flds) =>
                flds.Select(f => f.Name.AsQuoted(DbSetting)).Join(", ");
            string ParamRefList(IEnumerable<Field> flds) =>
                flds.Select(f => string.Concat(":", paramNamesByField[f.Name])).Join(", ");
            var identityParam = paramNamesByField[identityField.Name];

            sb.Append("IF (:").Append(identityParam).Append(" IS NULL OR :").Append(identityParam).Append(" = 0) THEN BEGIN ")
                .Append("INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(insertableFields)).Append(") VALUES (")
                .Append(ParamRefList(insertableFields)).Append(") RETURNING ")
                .Append(quotedKeyColumn).Append(" INTO :R0; END ")
                .Append("ELSE BEGIN ")
                .Append("UPDATE OR INSERT INTO ").Append(quotedTable)
                .Append(" (").Append(ColumnList(fieldList)).Append(") VALUES (")
                .Append(ParamRefList(fieldList)).Append(") MATCHING (")
                .Append(ColumnList(qualifiers)).Append(") RETURNING ")
                .Append(quotedKeyColumn).Append(" INTO :R0; END ")
                .Append("SUSPEND; END");

            return sb.ToString();
        }

        #endregion
    }
}
