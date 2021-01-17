using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    public static partial class SqlConnectionExtension
    {
        #region BulkInsertInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkInsertInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            TSqlTransaction transaction = null)
            where TEntity : class
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            // ThrowIfNullOrEmpty(entities);

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)connection.EnsureOpen().BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());

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
                var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                var tempTableName = (string)null;

                // Create the temp table if necessary
                if (withPseudoExecution)
                {
                    // Must be fixed name so the RepoDb.Core caches will not be bloated
                    tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                    // Add a # prefix if not physical
                    if (usePhysicalPseudoTempTable != true)
                    {
                        tempTableName = string.Concat("#", tempTableName);
                    }

                    // Create a temporary table
                    var sql = GetCreateTemporaryTableSqlText(tableName,
                        tempTableName,
                        fields,
                        dbSetting,
                        true);
                    connection.ExecuteNonQuery(sql, transaction: transaction);
                }

                // WriteToServer
                result = WriteToServerInternal<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    (tempTableName ?? tableName),
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
                        withPseudoExecution);

                    // Execute the SQL
                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
                    {
                        var mapping = mappings?.FirstOrDefault(e => string.Equals(e.DestinationColumn, identityDbField.Name, StringComparison.OrdinalIgnoreCase));
                        var identityField = mapping != null ? new Field(mapping.SourceColumn) : identityDbField.AsField();
                        result = SetIdentityForEntities<TEntity>(entities, reader, identityField);
                    }

                    // Drop the table after used
                    sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                    connection.ExecuteNonQuery(sql, transaction: transaction);
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkInsertInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            TSqlTransaction transaction = null)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables needed
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)connection.EnsureOpen().BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

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
                result = WriteToServerInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static int BulkInsertInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            TSqlTransaction transaction = null)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            if (dataTable?.Rows.Count <= 0)
            {
                return default;
            }

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)(connection.EnsureOpen()).BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());

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
                var tempTableName = (string)null;
                var sql = (string)null;

                // Create the temp table if necessary
                if (withPseudoExecution)
                {
                    // Must be fixed name so the RepoDb.Core caches will not be bloated
                    tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                    // Add a # prefix if not physical
                    if (usePhysicalPseudoTempTable != true)
                    {
                        tempTableName = string.Concat("#", tempTableName);
                    }

                    // Create a temporary table
                    sql = GetCreateTemporaryTableSqlText(tableName,
                       tempTableName,
                       fields,
                       dbSetting,
                       true);
                    connection.ExecuteNonQuery(sql, transaction: transaction);
                }

                // WriteToServer
                result = WriteToServerInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    (tempTableName ?? tableName),
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
                        sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting,
                            withPseudoExecution);

                        // Identify the column
                        var column = dataTable.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using (var reader = (DbDataReader)(connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction)))
                            {
                                result = SetIdentityForEntities(dataTable, reader, column);
                            }
                        }
                        else
                        {
                            result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Return the result
                return result;
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }
        }

        #endregion

        #region BulkInsertAsyncInternalBase

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertAsyncInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            if (entities?.Any() != true)
            {
                return default;
            }

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken: cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var entityType = entities.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());

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

                var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                var tempTableName = (string)null;

                // Create the temp table if necessary
                if (withPseudoExecution)
                {
                    // Must be fixed name so the RepoDb.Core caches will not be bloated
                    tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                    // Add a # prefix if not physical
                    if (usePhysicalPseudoTempTable != true)
                    {
                        tempTableName = string.Concat("#", tempTableName);
                    }

                    // Create a temporary table
                    var sql = GetCreateTemporaryTableSqlText(tableName,
                        tempTableName,
                        fields,
                        dbSetting,
                        true);
                    await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                }

                // WriteToServer
                result = await WriteToServerAsyncInternal<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    (tempTableName ?? tableName),
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
                        withPseudoExecution);

                    // Execute the SQL
                    using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                    {
                        result = await SetIdentityForEntitiesAsync<TEntity>(entities, reader, identityDbField, cancellationToken);
                    }

                    // Drop the table after used
                    sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                    await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="reader"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            // Variables needed
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

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
                result = await WriteToServerAsyncInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction,
                    cancellationToken);

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSqlBulkCopy"></typeparam>
        /// <typeparam name="TSqlBulkCopyOptions"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMappingCollection"></typeparam>
        /// <typeparam name="TSqlBulkCopyColumnMapping"></typeparam>
        /// <typeparam name="TSqlTransaction"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="rowState"></param>
        /// <param name="dbFields"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            DataRowState? rowState = null,
            IEnumerable<DbField> dbFields = null,
            IEnumerable<BulkInsertMapItem> mappings = null,
            TSqlBulkCopyOptions options = default,
            string hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            bool? isReturnIdentity = null,
            bool? usePhysicalPseudoTempTable = null,
            TSqlTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TSqlBulkCopy : class, IDisposable
            where TSqlBulkCopyOptions : Enum
            where TSqlBulkCopyColumnMappingCollection : class
            where TSqlBulkCopyColumnMapping : class
            where TSqlTransaction : DbTransaction
        {
            // Validate
            if (dataTable?.Rows.Count <= 0)
            {
                return default;
            }

            // Variables needed
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            // Check the transaction
            if (transaction == null)
            {
                // Add the transaction if not present
                transaction = (TSqlTransaction)(await connection.EnsureOpenAsync(cancellationToken)).BeginTransaction();
            }
            else
            {
                // Validate the objects
                ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var tableFields = GetDataColumns(dataTable)
                    .Select(column => column.ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());

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
                var tempTableName = (string)null;
                var sql = (string)null;

                // Create the temp table if necessary
                if (withPseudoExecution)
                {
                    // Must be fixed name so the RepoDb.Core caches will not be bloated
                    tempTableName = string.Concat("_RepoDb_BulkInsert_", GetTableName(tableName, dbSetting));

                    // Add a # prefix if not physical
                    if (usePhysicalPseudoTempTable != true)
                    {
                        tempTableName = string.Concat("#", tempTableName);
                    }

                    // Create a temporary table
                    sql = GetCreateTemporaryTableSqlText(tableName,
                       tempTableName,
                       fields,
                       dbSetting,
                       true);
                    await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                }

                // WriteToServer
                result = await WriteToServerAsyncInternal<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    (tempTableName ?? tableName),
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
                        sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting,
                            withPseudoExecution);

                        // Identify the column
                        var column = dataTable.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                            {
                                result = await SetIdentityForEntitiesAsync(dataTable, reader, column, cancellationToken);
                            }
                        }
                        else
                        {
                            result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }
                }

                // Commit the transaction
                if (hasTransaction == false)
                {
                    transaction?.Commit();
                }

                // Return the result
                return result;
            }
            catch
            {
                // Rollback the transaction
                if (hasTransaction == false)
                {
                    transaction?.Rollback();
                }

                // Throw
                throw;
            }
            finally
            {
                // Dispose the transaction
                if (hasTransaction == false)
                {
                    transaction?.Dispose();
                }
            }
        }

        #endregion
    }
}
