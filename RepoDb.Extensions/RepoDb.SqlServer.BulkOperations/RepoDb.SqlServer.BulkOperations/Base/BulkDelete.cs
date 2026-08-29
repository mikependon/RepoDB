using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        #region BulkDeleteInternalBase

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
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns></returns>
        internal static int BulkDeleteInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
            where TEntity : class
        {
            using var reader = new DataEntityDataReader<TEntity>(entities);

            return BulkDeleteInternalBase(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options,
                hints,
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
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkDeleteInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

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
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the mappings and qualifiers)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true ||
                            qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data reader)
                    if (readerFields.Any() == true)
                    {
                        fields = fields
                            .Where(e =>
                                readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Filter the fields (based on the data table)
                    mappings = fields?
                        .Select(e =>
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace);

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

                // WriteToServer
                WriteToServerInternal(connection,
                   tempTableName,
                   reader,
                   mappings,
                   options,
                   bulkCopyTimeout,
                   batchSize,
                   transaction);

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkDeleteInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            // Validate
            if (table?.Rows.Count <= 0)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

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
                var tableFields = Enumerable.Range(0, table.Columns.Count)
                    .Select((index) => table.Columns[index].ColumnName);
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the mappings and qualifiers)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true ||
                            qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data table)
                    if (tableFields?.Any() == true)
                    {
                        fields = fields
                            .Where(e =>
                                tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Filter the fields (based on the data table)
                    mappings = fields?
                        .Select(e =>
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace);

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

                // WriteToServer
                WriteToServerInternal(connection,
                   tempTableName,
                   table,
                   rowState,
                   mappings,
                   options,
                   bulkCopyTimeout,
                   batchSize,
                   false,
                   transaction);

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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

        #region BulkDeleteAsyncInternalBase

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
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteAsyncInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using var reader = new DataEntityDataReader<TEntity>(entities);

            return await BulkDeleteAsyncInternalBase(connection,
                tableName,
                reader,
                qualifiers,
                mappings,
                options,
                hints,
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
        /// <param name="reader"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteAsyncInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

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
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the mappings and qualifiers)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true ||
                            qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data reader)
                    if (readerFields.Any() == true)
                    {
                        fields = fields
                            .Where(e =>
                                readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Filter the fields (based on the data table)
                    mappings = fields?
                        .Select(e =>
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace, cancellationToken);

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

                // WriteToServer
                await WriteToServerAsyncInternal(connection,
                   tempTableName,
                   reader,
                   mappings,
                   options,
                   bulkCopyTimeout,
                   batchSize,
                   transaction,
                   cancellationToken);

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkDeleteAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (table?.Rows.Count <= 0)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK DELETE FROM {tableName}", bulkCopyTimeout, transaction);

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
                var tableFields = Enumerable.Range(0, table.Columns.Count)
                    .Select((index) => table.Columns[index].ColumnName);
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = (primaryDbField ?? identityDbField);

                // Validate the primary keys
                if (qualifiers?.Any() != true)
                {
                    if (primaryOrIdentityDbField == null)
                    {
                        throw new MissingPrimaryKeyException($"No primary key or identity key found for table '{tableName}'.");
                    }
                    else
                    {
                        qualifiers = new[] { primaryOrIdentityDbField.AsField() };
                    }
                }

                // Filter the fields (based on the mappings and qualifiers)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true ||
                            qualifiers.Any(q => string.Equals(q.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data table)
                    if (tableFields?.Any() == true)
                    {
                        fields = fields
                            .Where(e =>
                                tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Filter the fields (based on the data table)
                    mappings = fields?
                        .Select(e =>
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace, cancellationToken);

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

                // WriteToServer
                await WriteToServerAsyncInternal(connection,
                   tempTableName,
                   table,
                   rowState,
                   mappings,
                   options,
                   bulkCopyTimeout,
                   batchSize,
                   false,
                   transaction,
                   cancellationToken);

                // Delete the actual delete
                var sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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
