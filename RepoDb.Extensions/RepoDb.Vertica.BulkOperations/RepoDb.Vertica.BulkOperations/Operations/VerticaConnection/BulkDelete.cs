using Vertica.Data.VerticaClient;
using RepoDb.Enumerations.Vertica;
using RepoDb.Interfaces;
using RepoDb.Vertica.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class VerticaConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk based on a list of entities, matched by
        /// <paramref name="qualifiers"/> (or the primary/identity key when not specified). Returns the
        /// number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The expression defining the properties used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this VerticaConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk based on a list of entities, matched by
        /// <paramref name="qualifiers"/> (or the primary/identity key when not specified). Returns the
        /// number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk based on the rows of a <see cref="DataTable"/>,
        /// matched by <paramref name="qualifiers"/> (or the primary/identity key when not specified). Uses
        /// the <see cref="DataTable.TableName"/> property as the target table. Returns the number of
        /// deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete(this VerticaConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null) =>
            BulkDelete(connection, table?.TableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk based on the rows of a <see cref="DataTable"/>,
        /// matched by <paramref name="qualifiers"/> (or the primary/identity key when not specified).
        /// Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/> whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null) =>
            BulkDeleteBase(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk by streaming rows from a
        /// <see cref="IDataReader"/>, matched by <paramref name="qualifiers"/> (or the primary/identity key
        /// when not specified). Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="reader">The source <see cref="IDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null) =>
            BulkDeleteBase(connection, tableName, reader, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, based on a list of
        /// entities, matched by <paramref name="qualifiers"/> (or the primary/identity key when not
        /// specified). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The expression defining the properties used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this VerticaConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, based on a list of
        /// entities, matched by <paramref name="qualifiers"/> (or the primary/identity key when not
        /// specified). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this VerticaConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, based on the rows of a
        /// <see cref="DataTable"/>, matched by <paramref name="qualifiers"/> (or the primary/identity key
        /// when not specified). Uses the <see cref="DataTable.TableName"/> property as the target table.
        /// Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="table">The source <see cref="DataTable"/> whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync(this VerticaConnection connection,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteAsync(connection, table?.TableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, based on the rows of a
        /// <see cref="DataTable"/>, matched by <paramref name="qualifiers"/> (or the primary/identity key
        /// when not specified). Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/> whose matching rows are to be deleted.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync(this VerticaConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            BulkDeleteBaseAsync(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, by streaming rows from a
        /// <see cref="IDataReader"/>, matched by <paramref name="qualifiers"/> (or the primary/identity key
        /// when not specified). Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="reader">The source <see cref="IDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match the rows to delete; defaults to the primary/identity key when null.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows submitted per round trip. When null, every row is submitted in one round trip.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create for the delete operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync(this VerticaConnection connection,
            string tableName,
            IDataReader reader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            VerticaBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = VerticaTraceKeys.VerticaBulkDelete,
            VerticaTransaction transaction = null,
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
