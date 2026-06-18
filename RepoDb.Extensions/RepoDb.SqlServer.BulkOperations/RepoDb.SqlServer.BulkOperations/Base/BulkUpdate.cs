using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region BulkUpdateInternalBase

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
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkUpdateInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkUpdateTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    false);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                //// Set the options to KeepIdentity if needed
                //if (options == SqlBulkCopyOptions.Default &&
                //    identityDbField?.IsIdentity == true &&
                //    qualifiers?.Any(
                //        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                //    fields?.Any(
                //        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

                // WriteToServer
                WriteToServerInternal(connection,
                    tempTableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting);
                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

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

        /// <summary>
        /// Bulk update an instance of <see cref="DataTable"/> object into the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The target table for bulk-update operation.</param>
        /// <param name="dataTable">The <see cref="DataTable"/> object to be used in the bulk-update operation.</param>
        /// <param name="qualifiers">The qualifier fields to be used for this bulk-update operation. This is defaulted to the primary key; if not present, then it will use the identity key.</param>
        /// <param name="rowState">The state of the rows to be copied to the destination.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="options">The bulk-copy options to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="bulkCopyTimeout">The timeout in seconds to be used.</param>
        /// <param name="batchSize">The size per batch to be used.</param>
        /// <param name="usePhysicalPseudoTempTable">The flags that signify whether to create a physical pseudo table.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace"></param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int BulkUpdateInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
        {
            // Validate
            if (dataTable?.Rows.Count <= 0)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkUpdateTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
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
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    false);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                //// Set the options to KeepIdentity if needed
                //if (options == SqlBulkCopyOptions.Default &&
                //    identityDbField?.IsIdentity == true &&
                //    fields?.FirstOrDefault(
                //        field => string.Equals(field.Name, identityDbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

                // WriteToServer
                WriteToServerInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting);
                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

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

        #region BulkUpdateAsyncInternalBase

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
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkUpdateAsyncInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkUpdateTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select(index => reader.GetName(index));
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = primaryDbField ?? identityDbField;

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
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    false);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                //// Set the options to KeepIdentity if needed
                //if (options == SqlBulkCopyOptions.Default &&
                //    identityDbField?.IsIdentity == true &&
                //    fields?.FirstOrDefault(
                //        field => string.Equals(field.Name, identityDbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

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

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting);
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkUpdateAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (dataTable?.Rows.Count <= 0)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkUpdateTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
                var fields = dbFields?.GetAsFields();
                var primaryDbField = dbFields?.GetPrimary();
                var identityDbField = dbFields?.GetIdentity();
                var primaryOrIdentityDbField = primaryDbField ?? identityDbField;

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
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create a temporary table
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    false);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                //// Set the options to KeepIdentity if needed
                //if (options == SqlBulkCopyOptions.Default &&
                //    identityDbField?.IsIdentity == true &&
                //    fields?.FirstOrDefault(
                //        field => string.Equals(field.Name, identityDbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

                // WriteToServer
                await WriteToServerAsyncInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    transaction,
                    cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Update the actual update
                sql = GetBulkUpdateSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting);
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

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
