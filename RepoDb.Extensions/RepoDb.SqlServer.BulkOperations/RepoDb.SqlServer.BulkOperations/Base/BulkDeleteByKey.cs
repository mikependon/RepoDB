using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkDeleteByKeyInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            // Validate
            if (primaryKeys?.Any() != true)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var primaryOrIdentityDbField =
                    (
                        dbFields.GetPrimary() ??
                        dbFields.GetIdentity()
                    );

                // Throw an error if there are is no primary key
                if (primaryOrIdentityDbField == null)
                {
                    throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                }

                // Create a temporary table
                var primaryOrIdentityField = primaryOrIdentityDbField.AsField();
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    dbSetting,
                    false);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Do the bulk insertion first
                using (var dataTable = CreateDataTableWithSingleColumn(primaryOrIdentityField, primaryKeys))
                {
                    var options = primaryOrIdentityDbField.IsIdentity == true ?
                        Compiler.GetEnumFunc<SqlBulkCopyOptions>("KeepIdentity")() : default;
                    var mappings = new[] { new SqlServerBulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

                    // WriteToServer
                    WriteToServerInternal(connection,
                       tempTableName,
                       dataTable,
                       null,
                       mappings,
                       options,
                       bulkCopyTimeout,
                       batchSize,
                       false,
                       transaction);
                }

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
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

            // Return the result
            return result;
        }

        #endregion

        #region BulkDeleteByKeyAsyncInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteByKeyAsyncInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
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

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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

                // Create a temporary table
                var primaryOrIdentityField = primaryOrIdentityDbField.AsField();
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    dbSetting,
                    false);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Do the bulk insertion first
                using (var dataTable = CreateDataTableWithSingleColumn(primaryOrIdentityField, primaryKeys))
                {
                    var options = primaryOrIdentityDbField.IsIdentity == true ?
                        Compiler.GetEnumFunc<SqlBulkCopyOptions>("KeepIdentity")() : default;
                    var mappings = new[] { new SqlServerBulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

                    // WriteToServer
                    await WriteToServerAsyncInternal(connection,
                       tempTableName,
                       dataTable,
                       null,
                       mappings,
                       options,
                       bulkCopyTimeout,
                       batchSize,
                       false,
                       transaction,
                       cancellationToken);
                }

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    primaryOrIdentityField.AsEnumerable(),
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
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

            // Return the result
            return result;
        }

        #endregion
    }
}
