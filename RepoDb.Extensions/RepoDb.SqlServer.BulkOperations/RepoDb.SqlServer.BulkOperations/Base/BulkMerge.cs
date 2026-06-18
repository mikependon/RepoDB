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
        /// <returns></returns>
        private static int BulkMergeInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
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
            if (entities?.Any() != true)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = DbFieldCache.Get(connection, tableName, transaction, true);

                // Variables needed
                var entityType = entities.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = TypeCache.Get(entityType).IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
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
                        fields = fields
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

                // Create a temporary table
                var hasOrderingColumn = (isReturnIdentity == true && identityDbField != null);
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    hasOrderingColumn);
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
                    tempTableName ?? tableName,
                    entities,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction,
                    trace);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                if (hasOrderingColumn != true || TypeCache.Get(entityType).IsAnonymousType())
                {
                    result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace);
                }
                else
                {
                    using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace);

                    var mapping = mappings?.FirstOrDefault(e => string.Equals(e.DestinationColumn, identityDbField.Name, StringComparison.OrdinalIgnoreCase));
                    var identityField = mapping != null ? new Field(mapping.SourceColumn) : identityDbField.AsField();
                    result = SetIdentityForEntities<TEntity>(entities, reader, identityField);
                }

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
        private static int BulkMergeInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    false,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));
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
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
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

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            var result = default(int);

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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

                // Create a temporary table
                var hasOrderingColumn = (isReturnIdentity == true && identityDbField != null);
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    hasOrderingColumn);
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
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                connection.ExecuteNonQuery(sql, transaction: transaction, trace: trace);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                var column = identityDbField is not null ? dataTable.Columns[identityDbField.Name] : null;
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);

                    while (reader.Read())
                    {
                        var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                        dataTable.Rows[result][column] = value;
                        result++;
                    }
                }
                else
                {
                    result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction);
                }

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeAsyncInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
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
            if (entities?.Any() != true)
            {
                return default;
            }

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

            try
            {
                // Get the DB Fields
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, true, cancellationToken);

                // Variables needed
                var entityType = entities.FirstOrDefault()?.GetType() ?? typeof(TEntity);
                var entityFields = TypeCache.Get(entityType).IsDictionaryStringObject() ?
                    GetDictionaryStringObjectFields(entities.FirstOrDefault() as IDictionary<string, object>) :
                    FieldCache.Get(entityType);
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
                        fields = fields
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

                // Create a temporary table
                var hasOrderingColumn = (isReturnIdentity == true && identityDbField != null);
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    hasOrderingColumn);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

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
                await WriteToServerAsyncInternal(connection,
                    tempTableName ?? tableName,
                    entities,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction,
                    cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                if (hasOrderingColumn != true || TypeCache.Get(entityType).IsAnonymousType())
                {
                    result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout,
                        transaction: transaction, trace: trace, cancellationToken: cancellationToken);
                }
                else
                {
                    using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                    result = await SetIdentityForEntitiesAsync(entities, reader, identityDbField, cancellationToken);
                }

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
        private static async Task<int> BulkMergeAsyncInternalBase(SqlConnection connection,
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
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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
                //    qualifiers?.Any(
                //        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true &&
                //    fields?.Any(
                //        field => string.Equals(field.Name, identityDbField?.Name, StringComparison.OrdinalIgnoreCase)) == true)
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

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    false,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

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
        /// <param name="isReturnIdentity"></param>
        /// <param name="usePhysicalPseudoTempTable"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable dataTable,
            IEnumerable<Field>? qualifiers = null,
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

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkMergeTempTableName(tableName, usePhysicalPseudoTempTable, dbSetting);

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

                // Create a temporary table
                var hasOrderingColumn = (isReturnIdentity == true && identityDbField != null);
                var sql = GetCreateTemporaryTableSqlText(tableName,
                    tempTableName,
                    fields,
                    dbSetting,
                    hasOrderingColumn);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

                // WriteToServer
                await WriteToServerAsyncInternal(connection,
                    tempTableName,
                    dataTable,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction,
                    cancellationToken);

                // Create the clustered index
                sql = GetCreateTemporaryTableClusteredIndexSqlText(tempTableName,
                    qualifiers,
                    dbSetting);
                await connection.ExecuteNonQueryAsync(sql, transaction: transaction, trace: trace, cancellationToken: cancellationToken);

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

                // Merge the actual merge
                sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    isReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                var column = identityDbField is not null ? dataTable.Columns[identityDbField.Name] : null;
                if (isReturnIdentity == true && column?.ReadOnly == false)
                {
                    using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var value = Converter.DbNullToNull((await reader.GetFieldValueAsync<object>(0, cancellationToken)));
                        dataTable.Rows[result][column] = value;
                        result++;
                    }
                }
                else
                {
                    result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, cancellationToken: cancellationToken);
                }

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
