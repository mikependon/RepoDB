using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Interfaces;
using RepoDb.Firebird.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class FirebirdConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Updates existing rows in the database in bulk, matched by <paramref name="qualifiers"/> (or the
        /// primary/identity key when omitted). Returns the number of updated rows.
        /// </summary>
        public static int BulkUpdate<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkUpdateBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkUpdate{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkUpdate<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkUpdateBase(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Updates existing rows in the database in bulk from a <see cref="DataTable"/>. Uses the
        /// <see cref="DataTable.TableName"/> property as the target table. Returns the number of updated rows.
        /// </summary>
        public static int BulkUpdate(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null) =>
            BulkUpdate(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkUpdate(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkUpdate(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null) =>
            BulkUpdateBase(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Updates existing rows in the database in bulk from a <see cref="IDataReader"/>. Returns the
        /// number of updated rows.
        /// </summary>
        public static int BulkUpdate(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null) =>
            BulkUpdateBase(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <inheritdoc cref="BulkUpdate{TEntity}(FbConnection, IEnumerable{TEntity}, Expression{Func{TEntity, object}}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkUpdateAsync<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkUpdateBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkUpdate{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkUpdateAsync<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkUpdateBaseAsync(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkUpdate(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkUpdateAsync(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkUpdateAsync(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkUpdate(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkUpdateAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkUpdateBaseAsync(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkUpdate(FbConnection, string, IDataReader, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkUpdateAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkUpdate,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkUpdateBaseAsync(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
