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
        #region BulkInsertInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int BulkInsertInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
            where TEntity : class
        {
            // Validate
            // ThrowIfNullOrEmpty(entities);

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.GetIdentity();
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = TypeCache.Get(entityType).IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data entity)
                    if (entityFields?.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = isReturnIdentity == true && identityDbField != null;
                var tempTableName = CreateBulkInsertTempTableIfNecessary(connection,
                    tableName,
                    usePhysicalPseudoTempTable,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields);

                // WriteToServer
                result = WriteToServerInternal(connection,
                    tempTableName ?? tableName,
                    entities,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    withPseudoExecution,
                    transaction);

                // Check if this is with pseudo
                if (withPseudoExecution)
                {
                    // Merge the actual data
                    var sql = GetBulkInsertSqlText(tableName,
                        tempTableName,
                        fields,
                        identityDbField?.AsField(),
                        hints,
                        dbSetting,
                        withPseudoExecution,
                        options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                    // Execute the SQL
                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
                    {
                        var mapping = mappings?.FirstOrDefault(e => string.Equals(e.DestinationColumn, identityDbField.Name, StringComparison.OrdinalIgnoreCase));
                        var identityField = mapping != null ? new Field(mapping.SourceColumn) : identityDbField.AsField();
                        result = SetIdentityForEntities(entities, reader, identityField);
                    }

                    // Drop the table after used
                    sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                    connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);
                }

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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkInsertInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction? transaction = null)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables needed
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select(index => reader.GetName(index));
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data reader)
                    if (readerFields.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // WriteToServer
                result = WriteToServerInternal(connection,
                    tableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

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
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal static int BulkInsertInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null)
        {
            // Validate
            if (dataTable?.Rows.Count <= 0)
            {
                return default;
            }

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.GetIdentity();
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data table)
                    if (tableFields?.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                var tempTableName = CreateBulkInsertTempTableIfNecessary(connection,
                    tableName,
                    usePhysicalPseudoTempTable,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields);

                // WriteToServer
                result = WriteToServerInternal(connection,
                    tempTableName ?? tableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    withPseudoExecution,
                    transaction);

                // Check if this is with pseudo
                if (withPseudoExecution)
                {
                    if (isReturnIdentity == true)
                    {
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting,
                            withPseudoExecution,
                            options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                        // Identify the column
                        var column = dataTable.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

                            result = SetIdentityForEntities(dataTable, reader, column);
                        }
                        else
                        {
                            result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);
                    }
                }

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

        #region BulkInsertAsyncInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertAsyncInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
            bool usePhysicalPseudoTempTable = false,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Validate
            var firstEntity = entities?.FirstOrDefault();
            if (firstEntity is null)
            {
                return default;
            }

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken: cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.GetIdentity();
                var entityType = firstEntity.GetType();
                var entityFields = TypeCache.Get(entityType).IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(firstEntity as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data entity)
                    if (entityFields?.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                var withPseudoExecution = isReturnIdentity == true && identityDbField != null;
                var tempTableName = await CreateBulkInsertTempTableIfNecessaryAsync(connection,
                    tableName,
                    usePhysicalPseudoTempTable,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    cancellationToken);

                // WriteToServer
                result = await WriteToServerAsyncInternal(connection,
                    tempTableName ?? tableName,
                    entities,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    withPseudoExecution,
                    transaction,
                    cancellationToken);

                // Check if this is with pseudo
                if (withPseudoExecution)
                {
                    // Merge the actual data
                    var sql = GetBulkInsertSqlText(tableName,
                        tempTableName,
                        fields,
                        identityDbField?.AsField(),
                        hints,
                        dbSetting,
                        withPseudoExecution,
                        options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                    // Execute the SQL
                    using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                    {
                        result = await SetIdentityForEntitiesAsync(entities, reader, identityDbField, cancellationToken);
                    }

                    // Drop the table after used
                    sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                    await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);
                }

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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables needed
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select(index => reader.GetName(index));
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data reader)
                    if (readerFields.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // WriteToServer
                result = await WriteToServerAsyncInternal(connection,
                    tableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction,
                    cancellationToken);

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
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<BulkInsertMapItem>? mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool isReturnIdentity = false,
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

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.GetIdentity();
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.GetAsFields();

                // Filter the fields (based on mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields?
                        .Where(e =>
                            mappings.Any(mapping => string.Equals(mapping.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    // Filter the fields (based on the data table)
                    if (tableFields?.Any() == true)
                    {
                        fields = fields?
                            .Where(e =>
                                tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                    }

                    // Explicitly define the mappings
                    mappings = fields?
                        .Select(e =>
                            new BulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = isReturnIdentity == true && identityDbField != null;
                var tempTableName = await CreateBulkInsertTempTableIfNecessaryAsync(connection,
                    tableName,
                    usePhysicalPseudoTempTable,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    cancellationToken);

                // WriteToServer
                result = await WriteToServerAsyncInternal(connection,
                    tempTableName ?? tableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    withPseudoExecution,
                    transaction,
                    cancellationToken);

                // Check if this is with pseudo
                if (withPseudoExecution)
                {
                    if (isReturnIdentity == true)
                    {
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting,
                            withPseudoExecution,
                            options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                        // Identify the column
                        var column = dataTable.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                            result = await SetIdentityForEntitiesAsync(dataTable, reader, column, cancellationToken);
                        }
                        else
                        {
                            result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);
                    }
                }

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

        private static string CreateBulkInsertTempTableIfNecessary<TSqlTransaction>(
            IDbConnection connection,
            string tableName,
            bool? usePhysicalPseudoTempTable,
            TSqlTransaction transaction,
            bool withPseudoExecution,
            IDbSetting dbSetting,
            IEnumerable<Field> fields,
            ITrace? trace = null)
            where TSqlTransaction : DbTransaction
        {
            if (withPseudoExecution == false)
                return null;

            var tempTableName = CreateBulkInsertTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);
            var sql = GetCreateTemporaryTableSqlText(tableName, tempTableName, fields, dbSetting, true);

            connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

            return tempTableName;
        }

        private static async Task<string> CreateBulkInsertTempTableIfNecessaryAsync<TSqlTransaction>(IDbConnection connection,
            string tableName,
            bool? usePhysicalPseudoTempTable,
            TSqlTransaction transaction,
            bool withPseudoExecution,
            IDbSetting dbSetting,
            IEnumerable<Field> fields,
            CancellationToken cancellationToken,
            ITrace? trace = null)
            where TSqlTransaction : DbTransaction
        {
            if (withPseudoExecution == false)
                return null;

            var tempTableName = CreateBulkInsertTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);
            var sql = GetCreateTemporaryTableSqlText(tableName, tempTableName, fields, dbSetting, true);

            await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

            return tempTableName;
        }
    }
}
