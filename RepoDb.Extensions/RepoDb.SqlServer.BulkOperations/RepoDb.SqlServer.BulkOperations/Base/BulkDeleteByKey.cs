using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region BulkDeleteByKeyInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns></returns>
        internal static int BulkDeleteByKeyInternalBase<TPrimaryKey>(SqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            string? hints = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            return BulkDeleteByKeyInternalBase(connection,
                tableName,
                primaryKeys?.Cast<object>(),
                hints,
                options,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                transaction,
                trace,
                traceKey);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkDeleteByKeyInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            // Validate
            if (primaryKeys?.Any() != true)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkDeleteTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Throw an error if there are is no primary key
                if (primaryOrIdentityDbField == null)
                {
                    throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                var primaryOrIdentityField = primaryOrIdentityDbField.AsField();
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, primaryOrIdentityField.AsEnumerable(), primaryOrIdentityField.AsEnumerable(), dbSetting, false, transaction, trace);

                // Do the bulk insertion first
                using (var table = CreateDataTableWithSingleColumn(primaryOrIdentityField, primaryKeys))
                {
                    options |= primaryOrIdentityDbField.IsIdentity == true ?
                        Compiler.GetEnumFunc<SqlBulkCopyOptions>("KeepIdentity")() : default;
                    var mappings = new[] { new SqlServerBulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

                    // WriteToServer
                    WriteToServerInternal(connection,
                       tempTableName,
                       table,
                       null,
                       mappings,
                       options,
                       bulkCopyTimeout,
                       batchSize,
                       false,
                       transaction);
                }

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    hints,
                    dbSetting);
                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkDelete);

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                CommitTransaction(transaction, hasTransaction);
            }
            catch
            {
                RollbackTransaction(transaction, hasTransaction);
                throw;
            }
            finally
            {
                DisposeTransaction(transaction, hasTransaction);
            }

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

            // Return the result
            return result;
        }

        #endregion

        #region BulkDeleteByKeyAsyncInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteByKeyAsyncInternalBase<TPrimaryKey>(SqlConnection connection,
            string tableName,
            IEnumerable<TPrimaryKey> primaryKeys,
            string? hints = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
        {
            return await BulkDeleteByKeyAsyncInternalBase(connection,
                tableName,
                primaryKeys?.Cast<object>(),
                hints,
                options,
                bulkCopyTimeout,
                batchSize,
                pseudoTableType,
                transaction,
                trace,
                traceKey,
                cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteByKeyAsyncInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (primaryKeys?.Any() != true)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE BY KEY FROM {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkDeleteTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Throw an error if there are is no primary key
                if (primaryOrIdentityDbField == null)
                {
                    throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                var primaryOrIdentityField = primaryOrIdentityDbField.AsField();
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, primaryOrIdentityField.AsEnumerable(), primaryOrIdentityField.AsEnumerable(), dbSetting, false, transaction, trace, cancellationToken);

                // Do the bulk insertion first
                using (var table = CreateDataTableWithSingleColumn(primaryOrIdentityField, primaryKeys))
                {
                    options |= primaryOrIdentityDbField.IsIdentity == true ?
                        Compiler.GetEnumFunc<SqlBulkCopyOptions>("KeepIdentity")() : default;
                    var mappings = new[] { new SqlServerBulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

                    // WriteToServer
                    await WriteToServerAsyncInternal(connection,
                       tempTableName,
                       table,
                       null,
                       mappings,
                       options,
                       bulkCopyTimeout,
                       batchSize,
                       false,
                       transaction,
                       cancellationToken);
                }

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    hints,
                    dbSetting);
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkDelete, cancellationToken: cancellationToken);

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                CommitTransaction(transaction, hasTransaction);
            }
            catch
            {
                RollbackTransaction(transaction, hasTransaction);
                throw;
            }
            finally
            {
                DisposeTransaction(transaction, hasTransaction);
            }

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            // Return the result
            return result;
        }

        #endregion
    }
}
