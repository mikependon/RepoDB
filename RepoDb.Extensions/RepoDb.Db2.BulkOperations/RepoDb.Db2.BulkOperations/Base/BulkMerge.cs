using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBM.Data.Db2;
using RepoDb.Enumerations.Db2;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Db2.BulkOperations;
using RepoDb.Db2.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class Db2ConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == Db2BulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseForReturnIdentity<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                // Create pseudo table
                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, dbFields.GetAsFields(), trace, traceKey, transaction, new[] { identityField });
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                
                // Write
                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : null;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                WriteToServerInternal(connection, pseudoTableName, entityTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = Db2Execution.MergeFromPseudoTableForReturnIdentity(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, entityList, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseNoReturnIdentity<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);

                // Write
                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : null;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                WriteToServerInternal(connection, pseudoTableName, entityTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = Db2Execution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == Db2BulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseForReturnIdentity(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, dbFields.GetAsFields(), trace, traceKey, transaction, new[] { identityField });
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = Db2Execution.MergeFromPseudoTableForReturnIdentityForDataTable(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, rows, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBaseNoReturnIdentity(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = Db2Execution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #region BulkMergeBase<DbDataReader>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this DB2Connection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                Db2Execution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction);
                Db2Execution.CreatePseudoTableIndex(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction);
                Db2Execution.TruncatePseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
                WriteToServerInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = Db2Execution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction);
            }
            finally
            {
                // Drop the pseudo table
                Db2Execution.DropPseudoTable(connection, pseudoTableName, trace, traceKey, transaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            return result;
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == Db2BulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                // Create pseudo table
                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, dbFields.GetAsFields(), trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken, nullableFields: new[] { identityField });
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);

                // Write
                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : null;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken: cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await Db2Execution.MergeFromPseudoTableForReturnIdentityAsync(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, entityList, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync<TEntity>(this DB2Connection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            IEnumerable<Db2BulkInsertMapItem> mappings,
            DB2BulkCopyOptions bulkCopyOptions,
            int? bulkCopyTimeout,
            int? batchSize,
            Db2BulkImportPseudoTableType pseudoTableType,
            ITrace trace,
            string traceKey,
            DB2Transaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            // Identify the columns
            var entityList = entities.AsList();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, entityList?.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);

                // Write
                var entityFields = mappings?.Any() == true ? mappings.Select(m => new Field(m.SourceColumn)).AsList() : null;
                using var entityTable = BuildEntityDataTable(entityList, entityFields);
                await WriteToServerAsyncInternal(connection, pseudoTableName, entityTable, null, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken: cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await Db2Execution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == Db2BulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    identityBehavior,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyOptions,
                    bulkCopyTimeout,
                    batchSize,
                    pseudoTableType,
                    trace,
                    traceKey,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportIdentityBehavior identityBehavior = default,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var rows = GetDataRows(table, rowState)?.ToArray();
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName} RETURNING PK", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var identityField = dbFields.GetIdentity().AsField();
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, dbFields.GetAsFields(), trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken, nullableFields: new[] { identityField });
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                result = await Db2Execution.MergeFromPseudoTableForReturnIdentityForDataTableAsync(connection, tableName, pseudoTableName, mergeFields, identityField, qualifierFields, rows, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync(this DB2Connection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, table?.Rows.Count);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await Db2Execution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            return result;
        }

        #endregion

        #region BulkMergeBaseAsync<DbDataReader>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyOptions"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync(this DB2Connection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Db2BulkInsertMapItem> mappings = null,
            DB2BulkCopyOptions bulkCopyOptions = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            Db2BulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = Db2TraceKeys.Db2BulkMerge,
            DB2Transaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Identify the columns
            pseudoTableType = ResolvePseudoTableType(pseudoTableType, null);
            var pseudoTableName = Db2Text.GetPseudoTableNameForMerge(tableName, pseudoTableType, connection.GetDbSetting());

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            int result;

            try
            {
                // Bulk and post process
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);

                await Db2Execution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, trace: trace, traceKey: traceKey, transaction: transaction, cancellationToken: cancellationToken);
                await Db2Execution.CreatePseudoTableIndexAsync(connection, pseudoTableName, qualifierFields, trace, traceKey, transaction, cancellationToken);
                await Db2Execution.TruncatePseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
                await WriteToServerAsyncInternal(connection, pseudoTableName, reader, mappings, bulkCopyOptions, bulkCopyTimeout, batchSize, cancellationToken);

                // Execute and return
                var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
                var identityField = dbFields.GetIdentity()?.AsField();
                result = await Db2Execution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, identityField, trace, traceKey, transaction, cancellationToken);
            }
            finally
            {
                // Drop the pseudo table
                await Db2Execution.DropPseudoTableAsync(connection, pseudoTableName, trace, traceKey, transaction, cancellationToken);
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
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="qualifierFields"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldsException"></exception>
        private static IEnumerable<Field> GetMergeFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Db2BulkInsertMapItem> mappings,
            IEnumerable<Field> qualifierFields)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)) ||
                    qualifierFields.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

        #endregion
    }
}
