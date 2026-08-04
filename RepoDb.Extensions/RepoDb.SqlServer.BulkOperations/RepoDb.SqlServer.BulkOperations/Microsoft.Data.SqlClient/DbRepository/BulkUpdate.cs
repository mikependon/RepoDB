using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Interfaces;

using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    /// <summary>
    /// An extension class for <see cref="DbRepository{TDbConnection}"/> object.
    /// </summary>
    public static partial class DbRepositoryExtension
    {
        #region BulkUpdate<TEntity>

//        /// <summary>
//        /// Bulk update a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            BulkUpdate(repository: repository,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

//        /// <summary>
//        /// Bulk update a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            BulkUpdate(repository: repository,
//                tableName: tableName,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
//            IDataReader reader,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            BulkUpdate(repository: repository,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
            IDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate(reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdate(TableName)

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate(this DbRepository<SqlConnection> repository,
//            string tableName,
//            IDataReader reader,
//            IEnumerable<Field>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null) =>
//            BulkUpdate(repository: repository,
//                tableName: tableName,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate(this DbRepository<SqlConnection> repository,
            string tableName,
            IDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate(tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//            where TEntity : class =>
//            BulkUpdate<TEntity>(repository: repository,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate<TEntity>(this DbRepository<SqlConnection> repository,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate<TEntity>(dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static int BulkUpdate(this DbRepository<SqlConnection> repository,
//            string tableName,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null) =>
//            BulkUpdate(repository: repository,
//                tableName: tableName,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkUpdate(this DbRepository<SqlConnection> repository,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null)
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return bulkDbConnector.Connection.BulkUpdate(tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction);
        }

        #endregion

        #region BulkUpdateAsync<TEntity>

//        /// <summary>
//        /// Bulk update a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            BulkUpdateAsync(repository: repository,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync(entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk update a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            BulkUpdateAsync(repository: repository,
//                tableName: tableName,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-updated.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync(tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
//            IDataReader reader,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            BulkUpdateAsync(repository: repository,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
            IDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync(reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkUpdateAsync(TableName)

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync(this DbRepository<SqlConnection> repository,
//            string tableName,
//            IDataReader reader,
//            IEnumerable<Field>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default) =>
//            BulkUpdateAsync(repository: repository,
//                tableName: tableName,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync(this DbRepository<SqlConnection> repository,
            string tableName,
            IDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync(tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class =>
//            BulkUpdateAsync<TEntity>(repository: repository,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync<TEntity>(this DbRepository<SqlConnection> repository,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync<TEntity>(dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
//        /// <param name="tableName">The target table for bulk-update operation.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("Use the overload that accepts a 'RepoDb.Enumerations.SqlServer.SqlServerBulkImportPseudoTableType pseudoTableType' parameter instead of 'usePhysicalPseudoTempTable'.")]
//        public static Task<int> BulkUpdateAsync(this DbRepository<SqlConnection> repository,
//            string tableName,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            CancellationToken cancellationToken = default) =>
//            BulkUpdateAsync(repository: repository,
//                tableName: tableName,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction,
//                cancellationToken: cancellationToken);

        /// <summary>
        /// Bulk update an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="repository">The instance of <see cref="DbRepository{TDbConnection}"/> object.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> BulkUpdateAsync(this DbRepository<SqlConnection> repository,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkUpdate,
            SqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
// Create a connection
            using var bulkDbConnector = new BulkDbConnector(transaction, repository);

// Call the method
            return await bulkDbConnector.Connection.BulkUpdateAsync(tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: repository.CommandTimeout,
                batchSize: batchSize,
                pseudoTableType: pseudoTableType,
                trace: trace,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
