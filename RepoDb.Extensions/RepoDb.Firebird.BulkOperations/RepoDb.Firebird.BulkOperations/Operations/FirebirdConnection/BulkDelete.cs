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
        /// Deletes existing rows from the database in bulk, matched by <paramref name="qualifiers"/> (or the
        /// primary/identity key when omitted). Returns the number of deleted rows.
        /// </summary>
        public static int BulkDelete<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkDelete{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkDelete<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk from a <see cref="DataTable"/>. Uses the
        /// <see cref="DataTable.TableName"/> property as the target table. Returns the number of deleted rows.
        /// </summary>
        public static int BulkDelete(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null) =>
            BulkDelete(connection, table?.TableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <inheritdoc cref="BulkDelete(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static int BulkDelete(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null) =>
            BulkDeleteBase(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk from a <see cref="IDataReader"/>. Returns the
        /// number of deleted rows.
        /// </summary>
        public static int BulkDelete(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null) =>
            BulkDeleteBase(connection, tableName, reader, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <inheritdoc cref="BulkDelete{TEntity}(FbConnection, IEnumerable{TEntity}, Expression{Func{TEntity, object}}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteAsync<TEntity>(this FbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDelete{TEntity}(FbConnection, IEnumerable{TEntity}, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteAsync<TEntity>(this FbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDelete(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteAsync(this FbConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteAsync(connection, table?.TableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDelete(FbConnection, DataTable, IEnumerable{Field}, DataRowState?, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteAsync(this FbConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteBaseAsync(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <inheritdoc cref="BulkDelete(FbConnection, string, IDataReader, IEnumerable{Field}, int?, int?, FirebirdBulkImportPseudoTableType, ITrace, string, FbTransaction)"/>
        public static Task<int> BulkDeleteAsync(this FbConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            FirebirdBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = FirebirdTraceKeys.FirebirdBulkDelete,
            FbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteBaseAsync(connection, tableName, reader, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion

        #region Helpers

        private static IEnumerable<Field> ParseQualifiers<TEntity>(Expression<Func<TEntity, object>> qualifiers)
            where TEntity : class =>
            qualifiers != null ? Field.Parse(qualifiers) : null;

        #endregion
    }
}
