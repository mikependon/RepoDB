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
        /// Upserts a list of entities into the database in bulk - inserts new rows and updates existing ones
        /// based on <paramref name="qualifiers"/> (or the primary/identity key when omitted). Returns the
        /// number of affected rows.
        /// </summary>
        public static int BulkMerge<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkMerge{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkMerge<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkMergeBase(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts the rows of a <see cref="DataTable"/> into the database in bulk. Uses the
        /// <see cref="DataTable.TableName"/> property as the target table. Returns the number of affected rows.
        /// </summary>
        public static int BulkMerge(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null) =>
            BulkMerge(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkMerge(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkMerge(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null) =>
            BulkMergeBase(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Upserts the rows of a <see cref="IDataReader"/> into the database in bulk. There is no
        /// <c>identityBehavior</c> - see the remarks on <c>BulkInsert</c>'s reader overload.
        /// </summary>
        public static int BulkMerge(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null) =>
            BulkMergeBase(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <inheritdoc cref="BulkMerge{TEntity}(FbConnection, IEnumerable{TEntity}, Expression{Func{TEntity, object}}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkMergeAsync<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkMerge{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkMergeAsync<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeBaseAsync(connection, tableName, entities, qualifiers, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkMerge(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkMergeAsync(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeAsync(connection, table?.TableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkMerge(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportIdentityBehavior, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkMergeAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportIdentityBehavior identityBehavior = default,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeBaseAsync(connection, tableName, table, qualifiers, rowState, mappings, bulkCopyTimeout, batchSize, identityBehavior, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkMerge(FbConnection, string, IDataReader, IEnumerable{Field}, IEnumerable{FirebirdCommandBatcherMapItem}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkMergeAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<FirebirdCommandBatcherMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkMerge,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeBaseAsync(connection, tableName, reader, qualifiers, mappings, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
