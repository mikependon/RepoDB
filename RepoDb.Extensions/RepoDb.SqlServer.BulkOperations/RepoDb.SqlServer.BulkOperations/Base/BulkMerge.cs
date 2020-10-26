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
        #region BulkMergeInternal

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
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data reader)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                BulkInsertInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tempTableName,
                    entities,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                if (isReturnIdentity != true || entityType.IsAnonymousType())
                {
                    result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);
                }
                else
                {

                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
                    {
                        result = SetIdentityForEntities<TEntity>(entities, reader, identityDbField);
                    }
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

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
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                BulkInsertInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tempTableName,
                    reader,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());
                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

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
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="hints"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                BulkInsertInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                var column = dataTable.Columns[identityDbField.Name];
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
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

            // Result
            return result;
        }

        #endregion

        #region BulkMergeAsyncInternal

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
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeAsyncInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var entityType = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = entityType.IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities?.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data reader)
                if (entityFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            entityFields.Any(f => string.Equals(f.Name, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                    identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                await BulkInsertAsyncInternalBase<TEntity, TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tempTableName,
                    entities,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction,
                    cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                if (isReturnIdentity != true || entityType.IsAnonymousType())
                {
                    result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout,
                        transaction: transaction, cancellationToken: cancellationToken);
                }
                else
                {

                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction))
                    {
                        result = await SetIdentityForEntitiesAsync<TEntity>(entities, reader, identityDbField, cancellationToken);
                    }
                }

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

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
        /// <param name="qualifiers"></param>
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
        private static async Task<int> BulkMergeAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<Field> qualifiers = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var readerFields = Enumerable.Range(0, reader.FieldCount)
                    .Select((index) => reader.GetName(index));
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data reader)
                if (readerFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            readerFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                await BulkInsertAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection,
                    tempTableName,
                    reader,
                    filteredDbFields,
                    mappings,
                    options,
                    null,
                    bulkCopyTimeout,
                    batchSize,
                    false,
                    false,
                    transaction,
                    cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                // Drop the table after used
                sql = GetDropTemporaryTableSqlText(tempTableName, dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

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
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
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
        private static async Task<int> BulkMergeAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
            TSqlBulkCopyColumnMapping, TSqlTransaction>(DbConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
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
            // Variables
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

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Must be fixed name so the RepoDb.Core caches will not be bloated
            var tempTableName = string.Concat("_RepoDb_BulkMerge_", GetTableName(tableName, dbSetting));

            // Add a # prefix if not physical
            if (usePhysicalPseudoTempTable != true)
            {
                tempTableName = string.Concat("#", tempTableName);
            }

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var tableFields = Enumerable.Range(0, dataTable.Columns.Count)
                    .Select((index) => dataTable.Columns[index].ColumnName);
                var fields = dbFields?.Select(dbField => dbField.AsField());
                var primaryDbField = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
                var identityDbField = dbFields?.FirstOrDefault(dbField => dbField.IsIdentity);
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

                // Filter the fields (based on the data table)
                if (tableFields?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            tableFields.Any(fieldName => string.Equals(fieldName, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
                }

                // Filter the fields (based on the mappings)
                if (mappings?.Any() == true)
                {
                    fields = fields
                        .Where(e =>
                            mappings.Any(m => string.Equals(m.SourceColumn, e.Name, StringComparison.OrdinalIgnoreCase)) == true);
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
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Filter the DB Fields
                var filteredDbFields = dbFields?
                    .Where(dbField =>
                        fields?.Any(field => string.Equals(field.Name, dbField.Name, StringComparison.OrdinalIgnoreCase)) == true);

                // Do the bulk insertion first
                await BulkInsertAsyncInternalBase<TSqlBulkCopy, TSqlBulkCopyOptions, TSqlBulkCopyColumnMappingCollection,
                    TSqlBulkCopyColumnMapping, TSqlTransaction>(connection: connection,
                    tableName: tempTableName,
                    dataTable: dataTable,
                    rowState: rowState,
                    dbFields: filteredDbFields,
                    mappings: mappings,
                    options: options,
                    hints: hints,
                    bulkCopyTimeout: bulkCopyTimeout,
                    batchSize: batchSize,
                    isReturnIdentity: false,
                    usePhysicalPseudoTempTable: false,
                    transaction,
                    cancellationToken: cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, cancellationToken: cancellationToken);

                // Set the options to KeepIdentity if needed
                if (object.Equals(options, default(TSqlBulkCopyOptions)) &&
                identityDbField?.IsIdentity == true &&
                    qualifiers?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                    fields?.Any(
                        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    options = Compiler.GetEnumFunc<TSqlBulkCopyOptions>("KeepIdentity")();
                }

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity.GetValueOrDefault());

                // Identity if the identity is to return
                var column = dataTable.Columns[identityDbField.Name];
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken)))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var value = Converter.DbNullToNull((await reader.GetFieldValueAsync<object>(0, cancellationToken)));
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

            // Result
            return result;
        }

        #endregion
    }
}
