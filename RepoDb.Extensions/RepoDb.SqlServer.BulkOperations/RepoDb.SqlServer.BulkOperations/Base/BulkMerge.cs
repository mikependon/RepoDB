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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
            where TEntity : class
        {
            // Validate
            if (entities?.Any() != true)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                var hasOrderingColumn = (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null);
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, fields, qualifiers, dbSetting, hasOrderingColumn, transaction, trace);

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

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                if (hasOrderingColumn != true || TypeCache.Get(entityType).IsAnonymousType())
                {
                    result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge);
                }
                else
                {
                    using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge);

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
        private static int BulkMergeInternalBase(SqlConnection connection,
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

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace);

                // WriteToServer
                WriteToServerInternal(connection,
                    tempTableName,
                    reader,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    transaction);

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    false,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));
                result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge);

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int BulkMergeInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
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

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            var result = default(int);

            transaction = CreateOrValidateCurrentTransaction(connection, transaction);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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

                    // Explicitly define the mappings
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
                var hasOrderingColumn = (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null);
                CreateTemporaryTableWithIndex(connection, tableName, tempTableName, fields, qualifiers, dbSetting, hasOrderingColumn, transaction, trace);

                // WriteToServer
                WriteToServerInternal(connection,
                    tempTableName,
                    table,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction);

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                var column = identityDbField is not null ? table.Columns[identityDbField.Name] : null;
                if (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && column?.ReadOnly == false)
                {
                    using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge);

                    while (reader.Read())
                    {
                        var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
                        table.Rows[result][column] = value;
                        result++;
                    }
                }
                else
                {
                    result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge);
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

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeAsyncInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
            SqlServerBulkImportPseudoTableType pseudoTableType = default,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Validate
            if (entities?.Any() != true)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                var hasOrderingColumn = (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null);
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, fields, qualifiers, dbSetting, hasOrderingColumn, transaction, trace, cancellationToken);

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

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                if (hasOrderingColumn != true || TypeCache.Get(entityType).IsAnonymousType())
                {
                    result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout,
                        transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge, cancellationToken: cancellationToken);
                }
                else
                {
                    using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge, cancellationToken: cancellationToken);

                    var mapping = mappings?.FirstOrDefault(e => string.Equals(e.DestinationColumn, identityDbField.Name, StringComparison.OrdinalIgnoreCase));
                    var mappedIdentityDbField = mapping != null
                        ? new DbField(mapping.SourceColumn, identityDbField.IsPrimary, identityDbField.IsIdentity, identityDbField.IsNullable, identityDbField.Type, identityDbField.Size, identityDbField.Precision, identityDbField.Scale, identityDbField.DatabaseType, identityDbField.HasDefaultValue, identityDbField.Provider)
                        : identityDbField;
                    result = await SetIdentityForEntitiesAsync(entities, reader, mappedIdentityDbField, cancellationToken);
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
        private static async Task<int> BulkMergeAsyncInternalBase(SqlConnection connection,
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

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = transaction != null;
            int result;

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Create the temporary table and its qualifier index (index must exist before the data load)
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, fields, qualifiers, dbSetting, false, transaction, trace, cancellationToken);

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

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    false,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));
                result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge, cancellationToken: cancellationToken);

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field>? qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            string? hints = null,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlServerBulkImportIdentityBehavior identityBehavior = default,
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

            using var command = CreateTraceCommand(connection, $"BULK MERGE INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

            // Variables
            var dbSetting = connection.GetDbSetting();
            var hasTransaction = (transaction != null);
            var result = default(int);

            transaction = await CreateOrValidateCurrentTransactionAsync(connection, transaction, cancellationToken);
            var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);

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

                    // Explicitly define the mappings
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
                var hasOrderingColumn = (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null);
                await CreateTemporaryTableWithIndexAsync(connection, tableName, tempTableName, fields, qualifiers, dbSetting, hasOrderingColumn, transaction, trace, cancellationToken);

                // WriteToServer
                await WriteToServerAsyncInternal(connection,
                    tempTableName,
                    table,
                    rowState,
                    mappings,
                    options,
                    bulkCopyTimeout,
                    batchSize,
                    hasOrderingColumn,
                    transaction,
                    cancellationToken);

                // Merge the actual merge
                var sql = GetBulkMergeSqlText(tableName,
                    tempTableName,
                    fields,
                    qualifiers,
                    primaryDbField?.AsField(),
                    identityDbField?.AsField(),
                    hints,
                    dbSetting,
                    identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity,
                    options.HasFlag(SqlBulkCopyOptions.KeepIdentity));

                // Identity if the identity is to return
                var column = identityDbField is not null ? table.Columns[identityDbField.Name] : null;
                if (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && column?.ReadOnly == false)
                {
                    using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge, cancellationToken: cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var value = Converter.DbNullToNull((await reader.GetFieldValueAsync<object>(0, cancellationToken)));
                        table.Rows[result][column] = value;
                        result++;
                    }
                }
                else
                {
                    result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkMerge, cancellationToken: cancellationToken);
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

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            // Return the result
            return result;
        }

        #endregion
    }
}
