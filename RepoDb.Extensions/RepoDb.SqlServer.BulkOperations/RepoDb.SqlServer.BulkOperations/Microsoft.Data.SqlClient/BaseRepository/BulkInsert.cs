using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;

using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="BaseRepository{TEntity, TDbConnection}"/> object.
    /// </summary>
    public static partial class BaseRepositoryExtension
    {
        #region BulkInsert<TEntity>

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            IEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            repository.BulkInsert(entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsert(entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-insert operation.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            repository.BulkInsert(tableName: tableName,
//                entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsert(tableName: tableName,
                entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkInsertAsync<TEntity>

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            IEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            repository.BulkInsertAsync(entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync(entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-insert operation.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            repository.BulkInsertAsync(tableName: tableName,
//                entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync(tableName: tableName,
                entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            IAsyncEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            repository.BulkInsertAsync(entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            IAsyncEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync(entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-insert operation.</param>
//        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table. This argument will only be used if the 'isReturnIdentity' argument is 'true'.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
//            string tableName,
//            IAsyncEnumerable<TEntity> entities,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            repository.BulkInsertAsync(tableName: tableName,
//                entities: entities,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk insert a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="BaseRepository{TEntity, TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-insert operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of how the identity column would work during the operation.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use. This argument will only be used if the 'identityBehavior' argument is 'ReturnIdentity'.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this BaseRepository<TEntity, SqlConnection> repository,
            string tableName,
            IAsyncEnumerable<TEntity> entities,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkInsert,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return repository.DbRepository.BulkInsertAsync(tableName: tableName,
                entities: entities,
                mappings: mappings,
                options: options,
                hints: hints,
                batchSize: batchSize,
                identityBehavior: identityBehavior,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
