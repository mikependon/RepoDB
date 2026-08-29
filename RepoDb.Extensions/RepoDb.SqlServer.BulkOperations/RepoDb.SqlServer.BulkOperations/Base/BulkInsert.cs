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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static int BulkInsertInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
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

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null;
                var tempTableName = CreateBulkInsertTempTableIfNecessary(connection,
                    tableName,
                    pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    trace);

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
                    using (var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert))
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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <returns></returns>
        internal static int BulkInsertInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
            SqlTransaction? transaction = null,
            ITrace? trace = null,
            string? traceKey = null)
        {
            // Validate
            if (!reader.HasRows)
            {
                return default;
            }

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
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
        internal static int BulkInsertInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
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

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = Tracer
                .InvokeBeforeExecution(traceKey, trace, command);

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
                var tableFields = GetDataColumns(table)
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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null;
                var tempTableName = CreateBulkInsertTempTableIfNecessary(connection,
                    tableName,
                    pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    trace);

                // WriteToServer
                result = WriteToServerInternal(connection,
                    tempTableName ?? tableName,
                    table,
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
                    if (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity)
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
                        var column = table.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using var reader = (DbDataReader)connection.ExecuteReader(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert);

                            result = SetIdentityForEntities(table, reader, column);
                        }
                        else
                        {
                            result = connection.ExecuteNonQuery(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert);
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

            // After Execution
            Tracer
                .InvokeAfterExecution(traceResult, trace, result);

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
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkInsertAsyncInternalBase<TEntity>(SqlConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
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

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null;
                var tempTableName = await CreateBulkInsertTempTableIfNecessaryAsync(connection,
                    tableName,
                    pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    cancellationToken,
                    trace);

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
                    using (var reader = (DbDataReader)(await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert, cancellationToken: cancellationToken)))
                    {
                        var mapping = mappings?.FirstOrDefault(e => string.Equals(e.DestinationColumn, identityDbField.Name, StringComparison.OrdinalIgnoreCase));
                        var mappedIdentityDbField = mapping != null
                            ? new DbField(mapping.SourceColumn, identityDbField.IsPrimary, identityDbField.IsIdentity, identityDbField.IsNullable, identityDbField.Type, identityDbField.Size, identityDbField.Precision, identityDbField.Scale, identityDbField.DatabaseType, identityDbField.HasDefaultValue, identityDbField.Provider)
                            : identityDbField;
                        result = await SetIdentityForEntitiesAsync(entities, reader, mappedIdentityDbField, cancellationToken);
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
        /// <param name="mappings"></param>
        /// <param name="options"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="batchSize"></param>
        /// <param name="transaction"></param>
        /// <param name="trace"></param>
        /// <param name="traceKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase(SqlConnection connection,
            string tableName,
            DbDataReader reader,
            IEnumerable<SqlServerBulkInsertMapItem> mappings = null,
            SqlBulkCopyOptions options = default,
            int? bulkCopyTimeout = null,
            int? batchSize = null,
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

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<int> BulkInsertAsyncInternalBase(SqlConnection connection,
            string tableName,
            DataTable table,
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

            using var command = CreateTraceCommand(connection, $"BULK INSERT INTO {tableName}", bulkCopyTimeout, transaction);

            // Before Execution
            var traceResult = await Tracer
                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

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
                var tableFields = GetDataColumns(table)
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
                            new SqlServerBulkInsertMapItem(e.Name, e.Name));
                }

                // Throw an error if there are no fields
                if (fields?.Any() != true)
                {
                    throw new MissingFieldException("There are no field(s) found for this operation.");
                }

                // Pseudo temp table
                var withPseudoExecution = identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity && identityDbField != null;
                var tempTableName = await CreateBulkInsertTempTableIfNecessaryAsync(connection,
                    tableName,
                    pseudoTableType == SqlServerBulkImportPseudoTableType.Physical,
                    transaction,
                    withPseudoExecution,
                    dbSetting,
                    fields,
                    cancellationToken,
                    trace);

                // WriteToServer
                result = await WriteToServerAsyncInternal(connection,
                    tempTableName ?? tableName,
                    table,
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
                    if (identityBehavior == SqlServerBulkImportIdentityBehavior.ReturnIdentity)
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
                        var column = table.Columns[identityDbField.Name];
                        if (column?.ReadOnly == false)
                        {
                            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert, cancellationToken: cancellationToken);

                            result = await SetIdentityForEntitiesAsync(table, reader, column, cancellationToken);
                        }
                        else
                        {
                            result = await connection.ExecuteNonQueryAsync(sql, commandTimeout: bulkCopyTimeout, transaction: transaction, trace: trace, traceKey: traceKey ?? SqlServerTraceKeys.SqlServerBulkInsert, cancellationToken: cancellationToken);
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

            // After Execution
            await Tracer
                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

            // Return the result
            return result;
        }

        #endregion

        #region Helpers

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

        #endregion
    }
}
