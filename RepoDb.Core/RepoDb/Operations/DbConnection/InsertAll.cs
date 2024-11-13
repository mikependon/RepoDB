using RepoDb.Contexts.Providers;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region InsertAll<TEntity>

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternal<TEntity>(connection: connection,
                tableName: GetMappedName<TEntity>(entities),
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        internal static int InsertAllInternal<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return InsertAllInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entities: entities?.WithType<IDictionary<string, object>>(),
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
            else
            {
                return InsertAllInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return InsertAllAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return InsertAllAsyncInternal<TEntity>(connection: connection,
                tableName: GetMappedName<TEntity>(entities),
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        internal static Task<int> InsertAllAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (TypeCache.Get(GetEntityType(entities)).IsDictionaryStringObject())
            {
                return InsertAllAsyncInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entities: entities?.WithType<IDictionary<string, object>>(),
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
            else
            {
                return InsertAllAsyncInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region InsertAll(TableName)

        /// <summary>
        /// Insert multiple rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static int InsertAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return InsertAllInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region InsertAllAsync(TableName)

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        public static Task<int> InsertAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return InsertAllAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region InsertAllInternalBase<TEntity>

        /// <summary>
        /// Insert multiple rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            if (entities?.Any() != true)
            {
                return default;
            }

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the context
            var entityType = GetEntityType<TEntity>(entities);
            var context = InsertAllExecutionContextProvider.Create(entityType,
                connection,
                tableName,
                batchSize,
                fields,
                hints,
                transaction,
                statementBuilder);
            var result = 0;
            var hasTransaction = (transaction != null || Transaction.Current != null);

            try
            {
                // Ensure the connection is open
                connection.EnsureOpen();

                if (hasTransaction == false)
                {
                    // Create a transaction
                    transaction = connection.BeginTransaction();
                }

                // Create the command
                using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
                    CommandType.Text, commandTimeout, transaction))
                {
                    // Directly execute if the entities is only 1 (performance)
                    if (context.BatchSize == 1)
                    {
                        foreach (var entity in entities.AsList())
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Before Execution
                            var traceResult = Tracer
                                .InvokeBeforeExecution(traceKey, trace, command);

                            // Silent cancellation
                            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                            {
                                return result;
                            }

                            // Actual Execution
                            var returnValue = Converter.DbNullToNull(command.ExecuteScalar());

                            // After Execution
                            Tracer
                                .InvokeAfterExecution(traceResult, trace, result);

                            // Set the return value
                            if (returnValue != null)
                            {
                                context.KeyPropertySetterFunc?.Invoke(entity, returnValue);
                            }

                            // Iterate the result
                            result++;
                        }
                    }
                    else
                    {
                        foreach (var batchEntities in entities.AsList().Split(batchSize))
                        {
                            var batchItems = batchEntities.AsList();

                            // Break if there is no more records
                            if (batchItems.Count <= 0)
                            {
                                break;
                            }

                            // Check if the batch size has changed (probably the last batch on the enumerables)
                            if (batchItems.Count != batchSize)
                            {
                                // Get a new execution context from cache
                                context = InsertAllExecutionContextProvider.Create(entityType,
                                    connection,
                                    tableName,
                                    batchItems.Count,
                                    fields,
                                    hints,
                                    transaction,
                                    statementBuilder);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems.OfType<object>().AsList());
                                AddOrderColumnParameters(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            if (context.KeyPropertySetterFunc == null)
                            {
                                // Before Execution
                                var traceResult = Tracer
                                    .InvokeBeforeExecution(traceKey, trace, command);

                                // Silent cancellation
                                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                                {
                                    return result;
                                }

                                // No identity setters
                                result += command.ExecuteNonQuery();

                                // After Execution
                                Tracer
                                    .InvokeAfterExecution(traceResult, trace, result);
                            }
                            else
                            {
                                // Before Execution
                                var traceResult = Tracer
                                    .InvokeBeforeExecution(traceKey, trace, command);

                                // Set the identity back
                                using var reader = command.ExecuteReader();

                                // Get the results
                                var position = 0;
                                while (reader.Read())
                                {
                                    var value = Converter.DbNullToNull(reader.GetValue(0));
                                    var index = batchItems.Count > 1 && reader.FieldCount > 1 ? reader.GetInt32(1) : position;
                                    context.KeyPropertySetterFunc.Invoke(batchItems[index], value);
                                    position++;
                                }

                                // Set the result
                                result += batchItems.Count;

                                // After Execution
                                Tracer
                                    .InvokeAfterExecution(traceResult, trace, result);
                            }
                        }
                    }
                }

                if (hasTransaction == false)
                {
                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    // Rollback for any exception
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    // Rollback and dispose the transaction
                    transaction.Dispose();
                }
            }

            // Return the result
            return result;
        }

        #endregion

        #region InsertAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Insert multiple rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of inserted rows in the table.</returns>
        internal static async Task<int> InsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.InsertAll,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            if (entities?.Any() != true)
            {
                return default;
            }

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the context
            var entityType = GetEntityType<TEntity>(entities);
            var context = await InsertAllExecutionContextProvider.CreateAsync(entityType,
                connection,
                tableName,
                batchSize,
                fields,
                hints,
                transaction,
                statementBuilder,
                cancellationToken);
            var result = 0;
            var hasTransaction = (transaction != null || Transaction.Current != null);

            try
            {
                // Ensure the connection is open
                await connection.EnsureOpenAsync(cancellationToken);

                if (hasTransaction == false)
                {
                    // Create a transaction
                    transaction = connection.BeginTransaction();
                }

                // Create the command
                using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
                    CommandType.Text, commandTimeout, transaction))
                {
                    // Directly execute if the entities is only 1 (performance)
                    if (context.BatchSize == 1)
                    {
                        foreach (var entity in entities.AsList())
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Before Execution
                            var traceResult = await Tracer
                                .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

                            // Silent cancellation
                            if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                            {
                                return result;
                            }

                            // Actual Execution
                            var returnValue = Converter.DbNullToNull(await command.ExecuteScalarAsync(cancellationToken));

                            // After Execution
                            await Tracer
                                .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

                            // Set the return value
                            if (returnValue != null)
                            {
                                context.KeyPropertySetterFunc?.Invoke(entity, returnValue);
                            }

                            // Iterate the result
                            result++;
                        }
                    }
                    else
                    {
                        foreach (var batchEntities in entities.AsList().Split(batchSize))
                        {
                            var batchItems = batchEntities.AsList();

                            // Break if there is no more records
                            if (batchItems.Count <= 0)
                            {
                                break;
                            }

                            // Check if the batch size has changed (probably the last batch on the enumerables)
                            if (batchItems.Count != batchSize)
                            {
                                // Get a new execution context from cache
                                context = await InsertAllExecutionContextProvider.CreateAsync(entityType,
                                    connection,
                                    tableName,
                                    batchItems.Count,
                                    fields,
                                    hints,
                                    transaction,
                                    statementBuilder,
                                    cancellationToken);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems.OfType<object>().AsList());
                                AddOrderColumnParameters<TEntity>(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            if (context.KeyPropertySetterFunc == null)
                            {
                                // Before Execution
                                var traceResult = await Tracer
                                    .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

                                // Silent cancellation
                                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                                {
                                    return result;
                                }

                                // No identity setters
                                result += await command.ExecuteNonQueryAsync(cancellationToken);

                                // After Execution
                                await Tracer
                                    .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
                            }
                            else
                            {
                                // Before Execution
                                var traceResult = await Tracer
                                    .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

                                // Set the identity back
                                using var reader = await command.ExecuteReaderAsync(cancellationToken);

                                // Get the results
                                var position = 0;
                                while (await reader.ReadAsync(cancellationToken))
                                {
                                    var value = Converter.DbNullToNull(reader.GetValue(0));
                                    var index = batchItems.Count > 1 && reader.FieldCount > 1 ? reader.GetInt32(1) : position;
                                    context.KeyPropertySetterFunc.Invoke(batchItems[index], value);
                                    position++;
                                }

                                // Set the result
                                result += batchItems.Count;

                                // After Execution
                                await Tracer
                                    .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);
                            }
                        }
                    }
                }

                if (hasTransaction == false)
                {
                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    // Rollback for any exception
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    // Rollback and dispose the transaction
                    transaction.Dispose();
                }
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
