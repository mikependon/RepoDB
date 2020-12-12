using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.SqlServer.BulkOperations;
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
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
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

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data entity)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var field = entityFields?.FirstOrDefault(ef =>
                            string.Equals(ef.Name, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(field.Name ?? f.Name, f.Name);
                    });
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;
                    var recordsAffected = 0;

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
                            dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction))
                    {
                        var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                        writeToServerMethod(sqlBulkCopy, new[] { reader });
                        recordsAffected = reader.RecordsAffected;
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
                        {
                            result = SetIdentityForEntities<TEntity>(entities, reader, identityDbField);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }
                    else
                    {
                        // Set the return value
                        var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                            Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                        result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : recordsAffected;
                    }
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
            DbDataReader reader,
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
            if (!reader.HasRows)
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
                transaction = (TSqlTransaction)connection.EnsureOpen().BeginTransaction();
            }
            else
            {
                // Validate the objects
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = readerFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
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
                            dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DbDataReader) });
                    writeToServerMethod(sqlBulkCopy, new[] { reader });

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }
                    else
                    {
                        // Set the return value
                        var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                            Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                        result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
                    }
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
            if (dataTable?.Rows?.Count <= 0)
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
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
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

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = tableFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
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
                           dbSetting);
                        connection.ExecuteNonQuery(sql, transaction: transaction);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    connection.EnsureOpen();
                    if (rowState.HasValue == true)
                    {
                        var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DataTable), typeof(DataRowState) });
                        writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value });
                    }
                    else
                    {
                        var writeToServerMethod = Compiler.GetParameterizedVoidMethodFunc<TSqlBulkCopy>("WriteToServer", new[] { typeof(DataTable) });
                        writeToServerMethod(sqlBulkCopy, new[] { dataTable });
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        var column = dataTable.Columns[identityDbField.Name];

                        if (isReturnIdentity == true)
                        {
                            sql = GetBulkInsertSqlText(tableName,
                                tempTableName,
                                fields,
                                identityDbField?.AsField(),
                                hints,
                                dbSetting);

                            // Identify the column
                            if (column?.ReadOnly == false)
                            {
                                using (var reader = (DbDataReader)(connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction)))
                                {
                                    while (reader.Read())
                                    {
                                        var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                                        dataTable.Rows[result][column] = value;
                                        result++;
                                    }
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
                    else
                    {
                        // Get the result from the table rows count
                        result = GetDataRows(dataTable, rowState).Count();
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
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken: cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data entity)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var field = entityFields?.FirstOrDefault(ef =>
                            string.Equals(ef.Name, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(field.Name ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
                    var withPseudoExecution = (isReturnIdentity == true && identityDbField != null);
                    var tempTableName = (string)null;
                    var recordsAffected = 0;

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
                            dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    await connection.EnsureOpenAsync(cancellationToken);
                    using (var reader = new DataEntityDataReader<TEntity>(tableName, entities, connection, transaction))
                    {
                        var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                        await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });
                        recordsAffected = reader.RecordsAffected;
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction)))
                        {
                            result = await SetIdentityForEntitiesAsync<TEntity>(entities, reader, identityDbField, cancellationToken);
                        }

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        // Set the return value
                        var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                            Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                        result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : recordsAffected;
                    }
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
            DbDataReader reader,
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
            if (!reader.HasRows)
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
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
            }

            try
            {
                // Get the DB Fields
                dbFields = dbFields ?? await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
                var readerFields = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = readerFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
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
                            dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    await connection.EnsureOpenAsync(cancellationToken);
                    var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DbDataReader), typeof(CancellationToken) });
                    await writeToServerMethod(sqlBulkCopy, new object[] { reader, cancellationToken });

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        // Merge the actual merge
                        var sql = GetBulkInsertSqlText(tableName,
                            tempTableName,
                            fields,
                            identityDbField?.AsField(),
                            hints,
                            dbSetting);

                        // Execute the SQL
                        result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                        // Drop the table after used
                        sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        // Set the return value
                        var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<TSqlBulkCopy, int>("_rowsCopied") ??
                            Compiler.GetPropertyGetterFunc<TSqlBulkCopy, int>("RowsCopied");
                        result = rowsCopiedFieldOrProperty != null ? rowsCopiedFieldOrProperty(sqlBulkCopy) : reader.RecordsAffected;
                    }
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
            if (dataTable?.Rows?.Count <= 0)
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
                SqlConnectionExtension.ValidateTransactionConnectionObject(connection, transaction);
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

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Ensure to have the mappings
                if (mappings == null)
                {
                    mappings = fields?.Select(f =>
                    {
                        var readerField = tableFields.FirstOrDefault(rf =>
                            string.Equals(rf, f.Name, StringComparison.OrdinalIgnoreCase));
                        return new BulkInsertMapItem(readerField ?? f.Name, f.Name);
                    });
                }
                else
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.DestinationColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Throw an error if there are no mappings
                if (mappings?.Any() != true)
                {
                    throw new MissingMappingException("There are no mapping(s) found for this operation.");
                }

                // Actual Execution
                using (var sqlBulkCopy = (TSqlBulkCopy)Activator.CreateInstance(typeof(TSqlBulkCopy), connection, options, transaction))
                {
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
                           dbSetting);
                        await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);
                    }

                    // Set the destinationtable
                    Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "DestinationTableName", (tempTableName ?? tableName));

                    // Set the timeout
                    if (bulkCopyTimeout.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BulkCopyTimeout", bulkCopyTimeout.Value);
                    }

                    // Set the batch szie
                    if (batchSize.HasValue)
                    {
                        Compiler.SetProperty<TSqlBulkCopy>(sqlBulkCopy, "BatchSize", batchSize.Value);
                    }

                    // Add the mappings
                    AddMappings<TSqlBulkCopy, TSqlBulkCopyColumnMappingCollection, TSqlBulkCopyColumnMapping>(sqlBulkCopy, mappings);

                    // Open the connection and do the operation
                    await connection.EnsureOpenAsync(cancellationToken);
                    if (rowState.HasValue == true)
                    {
                        var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(DataRowState), typeof(CancellationToken) });
                        await writeToServerMethod(sqlBulkCopy, new object[] { dataTable, rowState.Value, cancellationToken });
                    }
                    else
                    {
                        var writeToServerMethod = Compiler.GetParameterizedMethodFunc<TSqlBulkCopy, Task>("WriteToServerAsync", new[] { typeof(DataTable), typeof(CancellationToken) });
                        await writeToServerMethod(sqlBulkCopy, new object[] { dataTable, cancellationToken });
                    }

                    // Check if this is with pseudo
                    if (withPseudoExecution)
                    {
                        var column = dataTable.Columns[identityDbField.Name];

                        if (isReturnIdentity == true)
                        {
                            sql = GetBulkInsertSqlText(tableName,
                                tempTableName,
                                fields,
                                identityDbField?.AsField(),
                                hints,
                                dbSetting);

                            // Identify the column
                            if (column?.ReadOnly == false)
                            {
                                using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                                {
                                    while (await reader.ReadAsync(cancellationToken))
                                    {
                                        var value = Converter.DbNullToNull(await reader.GetFieldValueAsync<object>(0, cancellationToken));
                                        dataTable.Rows[result][column] = value;
                                        result++;
                                    }
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
                    else
                    {
                        // Get the result from the table rows count
                        result = GetDataRows(dataTable, rowState).Count();
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
