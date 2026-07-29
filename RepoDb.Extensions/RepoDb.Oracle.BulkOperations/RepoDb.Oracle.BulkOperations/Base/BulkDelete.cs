using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Oracle.BulkOperations;
using RepoDb.Oracle.BulkOperations.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class OracleConnectionExtension
    {
        #region Sync

        #region BulkDeleteBase(PrimaryKeys)

        /// <summary>
        /// Deletes existing rows from <paramref name="tableName"/> in bulk, matched by their primary (or
        /// identity) key value - or by <paramref name="qualifiers"/>, when explicitly provided - via a
        /// staging (pseudo) table. See <see cref="BulkDeleteBaseViaKeyValues"/> for the detailed steps
        /// (and for where the <see cref="Tracer"/> Before/After pair is actually invoked - this overload
        /// is a pure pass-through so the bulk operation is only traced once).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys">The list of primary/identity key values to be bulk-deleted.</param>
        /// <param name="qualifiers">
        /// The single field to match <paramref name="primaryKeys"/> against, when the table's primary/identity
        /// key is not the desired match column. Only the first field is used - each <paramref name="primaryKeys"/>
        /// entry is a single scalar value, so there is nothing for a second field to match against.
        /// </param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows deleted.</returns>
        private static int BulkDeleteBase(this OracleConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
        {
            var primaryKeyList = primaryKeys?.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, primaryKeyList?.Count);

            return BulkDeleteBaseViaKeyValues(connection,
                tableName,
                primaryKeyList,
                qualifiers,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                trace,
                traceKey,
                transaction);
        }

        #endregion

        #region BulkDeleteBase<TEntity>

        /// <summary>
        /// Deletes rows from <paramref name="tableName"/> in bulk that are matched by <paramref name="entities"/>,
        /// via a staging (pseudo) table: the entities are bulk-written into the pseudo table, and a single
        /// <c>DELETE ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c> statement removes every row on the
        /// real table matched (on <paramref name="qualifiers"/>, defaulting to the primary/identity key) by
        /// a staged row. This is the "actual base execution" for the non-redirected path - the single
        /// <see cref="Tracer.InvokeBeforeExecution"/>/<see cref="Tracer.InvokeAfterExecution"/> pair wraps
        /// the entire create/truncate/write/delete/drop sequence below (the key-value redirect below is
        /// already traced once, inside <see cref="BulkDeleteBaseViaKeyValues"/>).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities">
        /// The list of entities to be bulk-deleted. When <typeparamref name="TEntity"/> is <see cref="object"/>
        /// and every element is a raw scalar/struct value (e.g. boxed <see cref="int"/> or <see cref="Guid"/>
        /// primary key values, routed here via a named <c>entities:</c> argument rather than the dedicated
        /// <c>primaryKeys</c> overload) - as opposed to a real entity/anonymous-type instance with properties -
        /// this is routed through the same key-value staging path as the <c>primaryKeys</c> overload instead,
        /// since there are no properties to bulk-write as a full entity.
        /// </param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows deleted.</returns>
        private static int BulkDeleteBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            if (IsKeyValueCollection(entityList))
            {
                return BulkDeleteBaseViaKeyValues(connection,
                    tableName,
                    (IEnumerable<object>)entityList,
                    qualifiers,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, entityList, null, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = OracleExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkDeleteBase<DataTable>

        /// <summary>
        /// Deletes rows from <paramref name="tableName"/> in bulk that are matched by the rows of
        /// <paramref name="table"/>, following the same steps as the <c>TEntity</c> overload - see
        /// <see cref="BulkDeleteBase{TEntity}"/> for the detailed remarks. This is the "actual base
        /// execution" that the <see cref="Tracer"/> Before/After pair wraps.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows deleted.</returns>
        private static int BulkDeleteBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, null, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = OracleExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkDeleteBase<DbDataReader>

        /// <summary>
        /// Deletes rows from <paramref name="tableName"/> in bulk that are matched by streaming
        /// <paramref name="dataReader"/> straight into a staging (pseudo) table - see
        /// <see cref="BulkDeleteBase{TEntity}"/> for the detailed remarks. A reader is always columnar/tabular
        /// like a <see cref="DataTable"/> (never a bare list of scalar key values), so unlike the
        /// <c>TEntity</c> overload there is no raw-key-value redirect to consider here - this is always the
        /// "actual base execution" directly.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataReader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows deleted.</returns>
        private static int BulkDeleteBase(this OracleConnection connection,
            string tableName,
            DbDataReader dataReader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
        {
            // Row count is unknown for a streaming reader (see the remarks on the DbDataReader BulkMerge
            // overload); Auto-resolution is currently a no-op regardless.
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
                WriteToServerInternal(connection, pseudoTableName, dataReader, null, bulkCopyTimeout, batchSize);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = OracleExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, qualifierFields, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkDeleteBaseAsync(PrimaryKeys)

        /// <summary>
        /// Asynchronous counterpart of the <c>primaryKeys</c> <see cref="BulkDeleteBase(OracleConnection, string, IEnumerable{object}, IEnumerable{Field}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows deleted.</returns>
        private static async Task<int> BulkDeleteBaseAsync(this OracleConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var primaryKeyList = primaryKeys?.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, primaryKeyList?.Count);

            return await BulkDeleteBaseViaKeyValuesAsync(connection,
                tableName,
                primaryKeyList,
                qualifiers,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                trace,
                traceKey,
                transaction,
                cancellationToken);
        }

        #endregion

        #region BulkDeleteBaseAsync<TEntity>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkDeleteBase{TEntity}"/> - see its remarks for the
        /// detailed behavior and caveats (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows deleted.</returns>
        private static async Task<int> BulkDeleteBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);

            if (IsKeyValueCollection(entityList))
            {
                return await BulkDeleteBaseViaKeyValuesAsync(connection,
                    tableName,
                    (IEnumerable<object>)entityList,
                    qualifiers,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }

            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityList, null, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await OracleExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkDeleteBaseAsync<DataTable>

        /// <summary>
        /// Asynchronous counterpart of the <c>DataTable</c> <see cref="BulkDeleteBase(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows deleted.</returns>
        private static async Task<int> BulkDeleteBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, null, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await OracleExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkDeleteBaseAsync<DbDataReader>

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkDeleteBase(OracleConnection, string, DbDataReader, IEnumerable{Field}, int?, int?, OracleBulkImportPseudoTableType, ITrace, string, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataReader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of rows deleted.</returns>
        private static async Task<int> BulkDeleteBaseAsync(this OracleConnection connection,
            string tableName,
            DbDataReader dataReader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction: transaction, cancellationToken: cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataReader, null, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers).AsList();
                result = await OracleExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, qualifierFields, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Deletes rows from <paramref name="tableName"/> in bulk that are matched by a list of raw scalar
        /// key values (e.g. primary key values), via a staging (pseudo) table: the key values are bulk-written
        /// into a single-column pseudo table shaped after <paramref name="qualifiers"/> (defaulting to the
        /// primary/identity key when not provided), and a single <c>DELETE ... WHERE ROWID IN (SELECT ... INNER JOIN ...)</c>
        /// statement removes every matched row from the real table. Shared by the dedicated <c>primaryKeys</c> overload
        /// and by the <c>TEntity</c> overload's raw-key-value redirect (see <see cref="IsKeyValueCollection{TEntity}"/>).
        /// This is the "actual base execution" for both of those callers - the single
        /// <see cref="Tracer.InvokeBeforeExecution"/>/<see cref="Tracer.InvokeAfterExecution"/> pair wraps
        /// the entire create/truncate/write/delete/drop sequence below.
        /// </summary>
        /// <exception cref="PrimaryFieldNotFoundException">
        /// No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.
        /// </exception>
        private static int BulkDeleteBaseViaKeyValues(OracleConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            IEnumerable<Field> qualifiers,
            int? bulkCopyTimeout,
            int? batchSize,
            OracleBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            OracleTransaction transaction)
        {
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields, qualifiers).First();

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process - the pseudo table only ever needs the one qualifier column
                OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, transaction);
                OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new OracleBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                WriteToServerInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize);

                // Execute and return
                result = OracleExecution.DeleteFromPseudoTable(connection, tableName, pseudoTableName, new[] { qualifierField }, transaction);
            }
            finally
            {
                // Drop the pseudo table
                OracleExecution.DropPseudoTable(connection, pseudoTableName, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkDeleteBaseViaKeyValues"/> - see its remarks for the
        /// detailed behavior (identical here).
        /// </summary>
        private static async Task<int> BulkDeleteBaseViaKeyValuesAsync(OracleConnection connection,
            string tableName,
            IEnumerable<object> keyValues,
            IEnumerable<Field> qualifiers,
            int? bulkCopyTimeout,
            int? batchSize,
            OracleBulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            OracleTransaction transaction,
            CancellationToken cancellationToken)
        {
            var pseudoTableName = OracleText.GetPseudoTableNameForDelete(tableName, pseudoTableType);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierField = GetQualifierFields(tableName, dbFields, qualifiers).First();

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process - the pseudo table only ever needs the one qualifier column
                await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, qualifierField, transaction, cancellationToken);
                await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);

                using var dataTable = CreateKeyValuesDataTable(qualifierField, keyValues);
                var mappings = new[] { new OracleBulkInsertMapItem(qualifierField.Name, qualifierField.Name) };
                await WriteToServerAsyncInternal(connection, pseudoTableName, dataTable, null, mappings, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                result = await OracleExecution.DeleteFromPseudoTableAsync(connection, tableName, pseudoTableName, new[] { qualifierField }, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await OracleExecution.DropPseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        /// <summary>
        /// Builds a single-column <see cref="DataTable"/> (named and typed after <paramref name="qualifierField"/>)
        /// populated with <paramref name="keyValues"/> - one row per value. Used to bulk-write raw scalar key
        /// values into the staging/pseudo table, since they have no properties/columns of their own to reflect.
        /// </summary>
        private static DataTable CreateKeyValuesDataTable(Field qualifierField,
            IEnumerable<object> keyValues)
        {
            var table = new DataTable();
            table.Columns.Add(qualifierField.Name, qualifierField.Type ?? typeof(object));

            foreach (var keyValue in keyValues)
            {
                table.Rows.Add(keyValue ?? DBNull.Value);
            }

            return table;
        }

        /// <summary>
        /// Detects whether <paramref name="entityList"/> is actually a list of raw scalar/struct key values
        /// (e.g. boxed <see cref="int"/> or <see cref="Guid"/> primary key values) rather than a list of real
        /// entity/anonymous-type instances - true only when <typeparamref name="TEntity"/> is <see cref="object"/>
        /// (i.e. the static element type carries no information of its own) and the runtime type of its first
        /// element is not a class type (<see cref="TypeExtension.IsClassType(Type)"/> - excludes <see cref="object"/>,
        /// covers structs like <see cref="int"/>/<see cref="Guid"/>/<see cref="DateTime"/>, and also excludes
        /// <see cref="string"/>, which implements <see cref="IEnumerable{T}"/>). A real entity, anonymous object,
        /// or <see cref="System.Dynamic.ExpandoObject"/> passed in as <c>TEntity == object</c> is a class type and
        /// is therefore correctly left on the normal (non-redirected) entity path.
        /// </summary>
        private static bool IsKeyValueCollection<TEntity>(IList<TEntity> entityList)
            where TEntity : class
        {
            if (typeof(TEntity) != typeof(object))
            {
                return false;
            }

            var firstEntity = entityList.FirstOrDefault();

            return firstEntity != null && firstEntity.GetType().IsClassType() != true;
        }

        #endregion
    }
}
