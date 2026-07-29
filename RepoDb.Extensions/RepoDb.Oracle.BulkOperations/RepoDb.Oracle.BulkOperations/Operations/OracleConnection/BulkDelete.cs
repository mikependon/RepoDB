using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class OracleConnectionExtension
    {
        #region Sync

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by the defined qualifiers (defaults
        /// to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of primary keys to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this OracleConnection connection,
            IEnumerable<object> primaryKeys,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, ClassMappedNameCache.Get<TEntity>(), primaryKeys, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by the defined qualifiers (defaults
        /// to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this OracleConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes existing rows from the database in bulk, matched by the defined qualifiers (defaults
        /// to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes rows from the target table in bulk, matched against a <see cref="DataTable"/>. Returns
        /// the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null) =>
            BulkDeleteBase(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        /// <summary>
        /// Deletes rows from the target table in bulk by streaming a <see cref="DbDataReader"/> to a
        /// staging table as rows are read from the source rather than materializing them into memory first,
        /// matched by the defined qualifiers (defaults to the primary key). Returns the number of deleted
        /// rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="dataReader">The source <see cref="DbDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of deleted rows.</returns>
        public static int BulkDelete<TEntity>(this OracleConnection connection,
            string tableName,
            DbDataReader dataReader,
            IEnumerable<Field> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null)
            where TEntity : class =>
            BulkDeleteBase(connection, tableName, dataReader, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction);

        #endregion

        #region Async

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of primary keys to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this OracleConnection connection,
            IEnumerable<object> primaryKeys,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), primaryKeys, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this OracleConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = OracleTraceKeys.OracleBulkDelete,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, ClassMappedNameCache.Get<TEntity>(), entities, ParseQualifiers(qualifiers), bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes existing rows from the database in bulk in an asynchronous way, matched by the defined
        /// qualifiers (defaults to the primary key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of entities to be bulk-deleted.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this OracleConnection connection,
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
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, tableName, entities, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes rows from the target table in bulk in an asynchronous way, matched against a
        /// <see cref="DataTable"/>. Returns the number of deleted rows.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="table">The source <see cref="DataTable"/>.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="rowState">The state of the rows to be included; when null, every row is included.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync(this OracleConnection connection,
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
            CancellationToken cancellationToken = default) =>
            BulkDeleteBaseAsync(connection, tableName, table, qualifiers, rowState, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        /// <summary>
        /// Deletes rows from the target table in bulk in an asynchronous way by streaming a
        /// <see cref="DbDataReader"/> to a staging table as rows are read from the source rather than
        /// materializing them into memory first, matched by the defined qualifiers (defaults to the primary
        /// key). Returns the number of deleted rows.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="dataReader">The source <see cref="DbDataReader"/> to stream from.</param>
        /// <param name="qualifiers">The fields used to match existing rows.</param>
        /// <param name="bulkCopyTimeout">The command timeout, in seconds.</param>
        /// <param name="batchSize">The number of rows in each batch. When null, the provider's default batch size is used.</param>
        /// <param name="pseudoTableType">The type of staging (pseudo) table to create and reuse for this operation.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The token to cancel the asynchronous operation.</param>
        /// <returns>The number of deleted rows.</returns>
        public static Task<int> BulkDeleteAsync<TEntity>(this OracleConnection connection,
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
            where TEntity : class =>
            BulkDeleteBaseAsync(connection, tableName, dataReader, qualifiers, bulkCopyTimeout, batchSize, pseudoTableType, trace, traceKey, transaction, cancellationToken);

        #endregion
    }
}
