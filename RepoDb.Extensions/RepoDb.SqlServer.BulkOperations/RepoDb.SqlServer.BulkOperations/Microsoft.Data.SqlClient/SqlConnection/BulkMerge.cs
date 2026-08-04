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
    /// Contains the extension methods for <see cref="SqlConnection"/> object.
    /// </summary>
    public static partial class SqlConnectionExtension
    {
        #region BulkMerge<TEntity>

//        /// <summary>
//        /// Bulk merge a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkMerge<TEntity>(this SqlConnection connection,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null)
//            where TEntity : class
//        {
//            return BulkMerge<TEntity>(connection: connection,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return BulkMergeInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

//        /// <summary>
//        /// Bulk merge a list of data entity objects into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkMerge<TEntity>(this SqlConnection connection,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace trace = null)
//            where TEntity : class
//        {
//            return BulkMerge<TEntity>(connection: connection,
//                tableName: tableName,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return BulkMergeInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportPseudoTableType' instead of the 'usePhysicalPseudoTempTable' boolean flag.")]
//        public static int BulkMerge<TEntity>(this SqlConnection connection,
//            DbDataReader reader,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace trace = null)
//            where TEntity : class
//        {
//            return BulkMerge<TEntity>(connection: connection,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return BulkMergeInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

        #endregion

        #region BulkMerge(TableName)

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
//        /// </summary>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportPseudoTableType' instead of the 'usePhysicalPseudoTempTable' boolean flag.")]
//        public static int BulkMerge(this SqlConnection connection,
//            string tableName,
//            DbDataReader reader,
//            IEnumerable<Field>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null)
//        {
//            return BulkMerge(connection: connection,
//                tableName: tableName,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
        {
            return BulkMergeInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkMerge<TEntity>(this SqlConnection connection,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null)
//            where TEntity : class
//        {
//            return BulkMerge<TEntity>(connection: connection,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
            where TEntity : class
        {
            return BulkMergeInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
//        /// </summary>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static int BulkMerge(this SqlConnection connection,
//            string tableName,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null)
//        {
//            return BulkMerge(connection: connection,
//                tableName: tableName,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                transaction: transaction);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int BulkMerge(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null)
        {
            return BulkMergeInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey);
        }

        #endregion

        #region BulkMergeAsync<TEntity>

//        /// <summary>
//        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class
//        {
//            return BulkMergeAsync<TEntity>(connection: connection,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace">The tracer to be used.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
//            string tableName,
//            IEnumerable<TEntity> entities,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class
//        {
//            return BulkMergeAsync<TEntity>(connection: connection,
//                tableName: tableName,
//                entities: entities,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge a list of data entity objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of the data entities to be bulk-merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportPseudoTableType' instead of the 'usePhysicalPseudoTempTable' boolean flag.")]
//        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
//            DbDataReader reader,
//            Expression<Func<TEntity, object>>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class
//        {
//            return BulkMergeAsync<TEntity>(connection: connection,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The expression for the qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            DbDataReader reader,
            Expression<Func<TEntity, object>>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                reader: reader,
                qualifiers: ParseExpression(qualifiers),
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkMergeAsync(TableName)

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportPseudoTableType' instead of the 'usePhysicalPseudoTempTable' boolean flag.")]
//        public static Task<int> BulkMergeAsync(this SqlConnection connection,
//            string tableName,
//            DbDataReader reader,
//            IEnumerable<Field>? qualifiers = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//        {
//            return BulkMergeAsync(connection: connection,
//                tableName: tableName,
//                reader: reader,
//                qualifiers: qualifiers,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync(this SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: tableName,
                reader: reader,
                qualifiers: qualifiers,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//            where TEntity : class
//        {
//            return BulkMergeAsync<TEntity>(connection: connection,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync<TEntity>(this SqlConnection connection,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

//        /// <summary>
//        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
//        /// </summary>
//        /// <param name="connection">The connection object to be used.</param>
//        /// <param name="tableName">The target table for bulk-merge operation.</param>
//        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
//        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
//        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
//        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
//        /// <param name="options">The bulk-copy options to be used.</param>
//        /// <param name="hints">The table hints to be used.</param>
//        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
//        /// <param name="batchSize">The size per batch to be used.</param>
//        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
//        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
//        /// <param name="transaction">The transaction to be used.</param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
//        /// <returns>The number of rows affected by the execution.</returns>
//        [Obsolete("This method is obsolete and will be removed in a future version. Use the overload that accepts 'SqlServerBulkImportIdentityBehavior' and 'SqlServerBulkImportPseudoTableType' instead of the 'isReturnIdentity' and 'usePhysicalPseudoTempTable' boolean flags.")]
//        public static Task<int> BulkMergeAsync(this SqlConnection connection,
//            string tableName,
//            DataTable dataTable,
//            IEnumerable<Field>? qualifiers = null,
//            DataRowState? rowState = null,
//            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
//            SqlBulkCopyOptions options = default,
//            string? hints = null,
//            int? bulkCopyTimeout = null,
//            int? batchSize = null,
//            bool isReturnIdentity = false,
//            bool usePhysicalPseudoTempTable = false,
//            SqlTransaction? transaction = null,
//            ITrace? trace = null,
//            CancellationToken cancellationToken = default)
//        {
//            return BulkMergeAsync(connection: connection,
//                tableName: tableName,
//                dataTable: dataTable,
//                qualifiers: qualifiers,
//                rowState: rowState,
//                mappings: mappings,
//                options: options,
//                hints: hints,
//                bulkCopyTimeout: bulkCopyTimeout,
//                batchSize: batchSize,
//                identityBehavior: isReturnIdentity ? SqlServerBulkImportIdentityBehavior.ReturnIdentity : SqlServerBulkImportIdentityBehavior.KeepIdentity,
//                pseudoTableType: usePhysicalPseudoTempTable ? SqlServerBulkImportPseudoTableType.Physical : SqlServerBulkImportPseudoTableType.Auto,
//                trace: trace,
//                transaction: transaction,
//                cancellationToken: cancellationToken);
//        }

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="identityBehavior">The behavior of the identity property/column to be used.</param>
        /// <param name="pseudoTableType">The type of the pseudo (staging) table to use.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> BulkMergeAsync(this SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            ITrace trace = null,
            string traceKey = SqlServerTraceKeys.SqlServerBulkMerge,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return BulkMergeAsyncInternal(connection: connection,
                tableName: tableName,
                dataTable: dataTable,
                qualifiers: qualifiers,
                rowState: rowState,
                mappings: mappings,
                options: options,
                hints: hints,
                bulkCopyTimeout: bulkCopyTimeout,
                batchSize: batchSize,
                isReturnIdentity: identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                usePhysicalPseudoTempTable: pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                transaction: transaction,
                trace: trace,
                traceKey: traceKey,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region BulkMergeInternal

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns></returns>
        internal static int BulkMergeInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
            where TEntity : class =>
            BulkMergeInternalBase<TEntity>(connection,
                tableName,
                entities,
                qualifiers,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey);

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkMergeInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null) =>
            BulkMergeInternalBase(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey);

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkMergeInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null) =>
            BulkMergeInternalBase(connection,
                tableName,
                dataTable,
                qualifiers,
                rowState,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey);

        #endregion

        #region BulkMergeAsyncInternal

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<int> BulkMergeAsyncInternal<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            BulkMergeAsyncInternalBase(connection,
                tableName,
                entities,
                qualifiers,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey,
                cancellationToken);

        /// <summary>
        /// Bulk merge an instance of <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> BulkMergeAsyncInternal(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeAsyncInternalBase(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey,
                cancellationToken);

        /// <summary>
        /// Bulk merge an instance of <see cref="DataTable"/> object into the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-merge operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-merge operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-merge operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="isReturnIdentity">The flags that signify whether the identity values will be returned.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> BulkMergeAsyncInternal(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default) =>
            BulkMergeAsyncInternalBase(connection,
                tableName,
                dataTable,
                qualifiers,
                rowState,
                mappings,
                options,
                hints,
                bulkCopyTimeout,
                batchSize,
                isReturnIdentity,
                usePhysicalPseudoTempTable,
                transaction,
                trace,
                traceKey,
                cancellationToken);

        #endregion
    }
}
