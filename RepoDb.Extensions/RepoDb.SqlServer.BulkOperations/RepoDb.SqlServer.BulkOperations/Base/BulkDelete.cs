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
using RepoDb.SqlServer.BulkOperations;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region BulkDeleteInternalBase

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
        internal static int BulkDeleteInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
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
                    var mappings = new[] { new BulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

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
        internal static int BulkDeleteInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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
                //    fields?.FirstOrDefault(
                //        field => string.Equals(field.Name, identityDbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

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

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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
        /// <returns></returns>
        internal static int BulkDeleteInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Get the DB Fields
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

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

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

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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

        #region BulkDeleteAsyncInternalBase

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
        internal static async Task<int> BulkDeleteAsyncInternalBase(SqlConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
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
                    var mappings = new[] { new BulkInsertMapItem(primaryOrIdentityField.Name, primaryOrIdentityField.Name) };

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
        internal static async Task<int> BulkDeleteAsyncInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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
        internal static async Task<int> BulkDeleteAsyncInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkDeleteTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                //// Set the options to KeepIdentity if needed
                //if (options == SqlBulkCopyOptions.Default &&
                //    identityDbField?.IsIdentity == true &&
                //    fields?.FirstOrDefault(
                //        field => string.Equals(field.Name, identityDbField.Name, StringComparison.OrdinalIgnoreCase)) != null)
                //{
                //    options = SqlBulkCopyOptions.KeepIdentity;
                //}

                // If there is no mapping
                if (mappings?.Any() != true)
                {
                    mappings = GetBulkInsertMapItemsFromFields(fields);
                }

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

                // Delete the actual delete
                sql = GetBulkDeleteSqlText(tableName,
                    tempTableName,
                    qualifiers,
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
