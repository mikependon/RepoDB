using RepoDb.Contexts.Execution;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternal<TEntity>(connection: connection,
                entities: entities,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Return the result
            return InsertAllInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                fields: FieldCache.Get<TEntity>(),
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: false);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            //// Return the result
            //return InsertAllAsyncInternalBase<TEntity>(connection: connection,
            //    tableName: ClassMappedNameCache.Get<TEntity>(),
            //    entities: entities,
            //    fields: FieldCache.Get<TEntity>(),
            //    batchSize: batchSize,
            //    commandTimeout: commandTimeout,
            //    transaction: transaction,
            //    trace: trace,
            //    statementBuilder: statementBuilder,
            //    skipIdentityCheck: false);
            // Return the result
            return InsertAllAsyncInternalBase(connection: connection,
                entities: entities,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region InsertAll(TableName)

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAllInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database (certain fields only).
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Check the fields
            if (fields == null)
            {
                fields = DbFieldCache.Get(connection, tableName)?.AsFields();
            }

            // Return the result
            return InsertAllInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                fields: fields,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: true);
        }

        #endregion

        #region InsertAllAsync(TableName)

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAllAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                fields: fields,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Guard the parameters
            GuardInsertAll(entities);

            // Check the fields
            if (fields == null)
            {
                fields = DbFieldCache.Get(connection, tableName)?.AsFields();
            }

            // Variables
            var request = new InsertAllRequest(tableName,
                connection,
                fields,
                batchSize,
                statementBuilder);

            // Return the result
            return InsertAllAsyncInternalBase(connection: connection,
                request: request,
                entities: entities,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region InsertAllInternalBase<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the objects to be enumerated.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> fields = null,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Guard the parameters
            var count = GuardInsertAll(entities);

            // Validate the batch size
            batchSize = Math.Min(batchSize, count);

            // Get the function
            var callback = new Func<int, InsertAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables
                var request = new InsertAllRequest(tableName,
                    connection,
                    fields,
                    batchSizeValue,
                    statementBuilder);

                // Variables needed
                var commandText = CommandTextCache.GetInsertAllText(request);
                var identity = (ClassProperty)null;
                var dbFields = DbFieldCache.Get(connection, request.Name);
                var inputFields = (IEnumerable<Field>)null;
                var outputFields = (IEnumerable<Field>)null;

                // Actual identity
                var identityDbField = dbFields.FirstOrDefault(f => f.IsIdentity);

                // Set the identity value
                if (skipIdentityCheck == false)
                {
                    identity = IdentityCache.Get<TEntity>();
                    if (identity == null)
                    {
                        if (identityDbField != null)
                        {
                            identity = PropertyCache.Get<TEntity>().FirstOrDefault(property =>
                                property.GetUnquotedMappedName().ToLower() == identityDbField?.UnquotedName.ToLower());
                        }
                    }
                }

                // Filter the actual properties for input fields
                inputFields = fields
                    .Where(field =>
                        field.UnquotedName.ToLower() != identityDbField?.UnquotedName.ToLower())
                    .Where(field =>
                        dbFields.FirstOrDefault(df => df.UnquotedName.ToLower() == field.UnquotedName.ToLower()) != null)
                    .ToList();

                // Set the output fields
                if (identity != null)
                {
                    outputFields = identity.AsField().AsEnumerable();
                }
                else
                {
                    outputFields = identityDbField?.AsField().AsEnumerable();
                }

                // Return the value
                return new InsertAllExecutionContext<TEntity>
                {
                    CommandText = commandText,
                    Identity = identity,
                    InputFields = inputFields,
                    OutputFields = outputFields,
                    BatchSize = batchSizeValue,
                    Execute = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", request.Name),
                        inputFields.AsList(),
                        outputFields,
                        batchSizeValue)
                };
            });

            // Create a command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand())
            {
                // Get the context
                var context = (InsertAllExecutionContext<TEntity>)null;

                // Identify the number of entities (performance)
                context = count == 1 ? InsertAllExecutionContextCache<TEntity>.Get(1, callback) :
                    InsertAllExecutionContextCache<TEntity>.Get(batchSize, callback);

                // Set the command properties
                command.CommandText = context.CommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = (DbTransaction)transaction;

                // Before Execution
                if (trace != null)
                {
                    var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
                    trace.BeforeInsertAll(cancellableTraceLog);
                    if (cancellableTraceLog.IsCancelled)
                    {
                        if (cancellableTraceLog.IsThrowException)
                        {
                            throw new CancelledExecutionException(context.CommandText);
                        }
                        return 0;
                    }
                    context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                    entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
                }

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Execution variables
                var result = 0;

                // Directly execute if the entities is only 1 (performance)
                if (context.BatchSize == 1)
                {
                    foreach (var entity in entities)
                    {
                        // Set the values
                        context.Execute(command, new[] { entity });

                        // Actual Execution
                        result += command.ExecuteNonQuery();

                        // Set the identities
                        if (context.Identity != null)
                        {
                            var parameterName = context.Identity.GetUnquotedMappedName();
                            if (command.Parameters.Contains(parameterName))
                            {
                                var parameter = command.Parameters[parameterName];
                                context.Identity.PropertyInfo.SetValue(entity, parameter.Value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var batchEntities in entities.Split(batchSize))
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
                            context = InsertAllExecutionContextCache<TEntity>.Get(batchItems.Count, callback);
                            command.CommandText = context.CommandText;
                        }

                        // Set the values
                        context.Execute(command, batchItems);

                        // Actual Execution
                        result += command.ExecuteNonQuery();

                        // Set the identities
                        if (context.Identity != null)
                        {
                            for (var i = 0; i < batchItems.Count; i++)
                            {
                                var parameterName = context.Identity.GetUnquotedMappedName().AsParameter(i, null);
                                if (command.Parameters.Contains(parameterName))
                                {
                                    var parameter = command.Parameters[parameterName];
                                    context.Identity.PropertyInfo.SetValue(batchItems.ElementAt(i), parameter.Value);
                                }
                            }
                        }
                    }
                }

                // After Execution
                if (trace != null)
                {
                    trace.AfterInsertAll(new TraceLog(context.CommandText, entities, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }

                // Return the result
                return result;
            }
        }

        #endregion

        #region InsertAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the objects to be enumerated.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static async Task<int> InsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchInsertSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Guard the parameters
            var count = GuardInsertAll(entities);

            // Validate the batch size
            batchSize = Math.Min(batchSize, count);

            // Get the function
            var callback = new Func<int, InsertAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables
                var request = new InsertAllRequest(typeof(TEntity),
                    connection,
                    FieldCache.Get<TEntity>(),
                    batchSizeValue,
                    statementBuilder);

                // Variables needed
                var commandText = CommandTextCache.GetInsertAllText(request);
                var inputFields = FieldCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                var dbFields = DbFieldCache.Get(connection, request.Name);

                // Filter the actual fields
                if (dbFields != null)
                {
                    // Set the identity value
                    if (identity == null)
                    {
                        var dbField = dbFields?.FirstOrDefault(f => f.IsIdentity);
                        if (dbField != null)
                        {
                            identity = PropertyCache.Get<TEntity>().FirstOrDefault(property =>
                                property.GetUnquotedMappedName().ToLower() == dbField.UnquotedName.ToLower());
                        }
                    }

                    // Filter the actual properties
                    inputFields = inputFields
                        .Where(field =>
                            identity?.GetUnquotedMappedName().ToLower() != field.UnquotedName.ToLower())
                        .Where(field =>
                            dbFields.FirstOrDefault(df => df.UnquotedName.ToLower() == field.UnquotedName.ToLower()) != null)
                        .ToList();
                }

                // Set the output fields
                var outputFields = identity?.AsField().AsEnumerable();

                // Return the value
                return new InsertAllExecutionContext<TEntity>
                {
                    CommandText = commandText,
                    Identity = identity,
                    InputFields = inputFields,
                    OutputFields = outputFields,
                    BatchSize = batchSizeValue,
                    Execute = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(
                        request.Name,
                        inputFields.AsList(),
                        outputFields,
                        batchSizeValue)
                };
            });

            // Create a command
            using (var command = (DbCommand)(await connection.EnsureOpenAsync()).CreateCommand())
            {
                // Get the context
                var context = (InsertAllExecutionContext<TEntity>)null;

                // Identify the number of entities (performance)
                context = count == 1 ? InsertAllExecutionContextCache<TEntity>.Get(1, callback) :
                    InsertAllExecutionContextCache<TEntity>.Get(batchSize, callback);

                // Set the command properties
                command.CommandText = context.CommandText;
                command.CommandType = CommandType.Text;
                command.Transaction = (DbTransaction)transaction;

                // Before Execution
                if (trace != null)
                {
                    var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
                    trace.BeforeInsertAll(cancellableTraceLog);
                    if (cancellableTraceLog.IsCancelled)
                    {
                        if (cancellableTraceLog.IsThrowException)
                        {
                            throw new CancelledExecutionException(context.CommandText);
                        }
                        return 0;
                    }
                    context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                    entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
                }

                // Before Execution Time
                var beforeExecutionTime = DateTime.UtcNow;

                // Execution variables
                var result = 0;

                // Directly execute if the entities is only 1 (performance)
                if (context.BatchSize == 1)
                {
                    foreach (var entity in entities)
                    {
                        // Set the values
                        context.Execute(command, new[] { entity });

                        // Actual Execution
                        result += await command.ExecuteNonQueryAsync();

                        // Set the identities
                        if (context.Identity != null)
                        {
                            var parameterName = context.Identity.GetUnquotedMappedName();
                            if (command.Parameters.Contains(parameterName))
                            {
                                var parameter = command.Parameters[parameterName];
                                context.Identity.PropertyInfo.SetValue(entity, parameter.Value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var batchEntities in entities.Split(batchSize))
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
                            context = InsertAllExecutionContextCache<TEntity>.Get(batchItems.Count, callback);
                            command.CommandText = context.CommandText;
                        }

                        // Set the values
                        context.Execute(command, batchItems);

                        // Actual Execution
                        result += await command.ExecuteNonQueryAsync();

                        // Set the identities
                        if (context.Identity != null)
                        {
                            for (var i = 0; i < batchItems.Count; i++)
                            {
                                var parameterName = context.Identity.GetUnquotedMappedName().AsParameter(i, null);
                                if (command.Parameters.Contains(parameterName))
                                {
                                    var parameter = command.Parameters[parameterName];
                                    context.Identity.PropertyInfo.SetValue(batchItems.ElementAt(i), parameter.Value);
                                }
                            }
                        }
                    }
                }

                // After Execution
                if (trace != null)
                {
                    trace.AfterInsertAll(new TraceLog(context.CommandText, entities, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }

                // Return the result
                return result;
            }
        }

        #endregion

        #region InsertAllInternalBase(TableName)

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="InsertAllRequest"/> object.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternalBase(this IDbConnection connection,
            InsertAllRequest request,
            IEnumerable<object> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetInsertAllText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entities, null);
                trace.BeforeInsertAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                entities = (IEnumerable<object>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Get the necessary values
            var dbFields = DbFieldCache.Get(connection, request.Name);
            var fields = request.Fields;

            // Filter the actual fields
            if (dbFields != null)
            {
                fields = dbFields
                    .Where(df =>
                        df.IsIdentity == false)
                    .Where(df =>
                        fields.FirstOrDefault(
                            f => f.UnquotedName.ToLower() == df.UnquotedName.ToLower()) != null)
                    .AsFields()
                    .ToList();
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Result set
            var result = 0;

            // Create a command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(commandText: commandText,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Create all the parameters
                DataCommand.CreateParameters(command, fields);

                // Get the function
                var func = FunctionCache.GetDataCommandParameterSetterFunction(request.Name,
                    fields);

                // Iterate each entity
                foreach (var entity in entities)
                {
                    // Set the values
                    func(command, entity);

                    // Actual Execution
                    command.ExecuteScalar();

                    // Add to the list
                    result++;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterInsertAll(new TraceLog(commandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region InsertAllAsyncInternalBase(TableName)

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="InsertAllRequest"/> object.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static async Task<int> InsertAllAsyncInternalBase(this IDbConnection connection,
            InsertAllRequest request,
            IEnumerable<object> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetInsertAllText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entities, null);
                trace.BeforeInsertAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                entities = (IEnumerable<object>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Get the necessary values
            var dbFields = DbFieldCache.Get(connection, request.Name);
            var fields = request.Fields;

            // Filter the actual fields
            if (dbFields != null)
            {
                fields = dbFields
                    .Where(df =>
                        df.IsIdentity == false)
                    .Where(df =>
                        fields.FirstOrDefault(
                            f => f.UnquotedName.ToLower() == df.UnquotedName.ToLower()) != null)
                    .AsFields()
                    .ToList();
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Result set
            var result = 0;

            // Create a command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(commandText: commandText,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Create all the parameters
                DataCommand.CreateParameters(command, fields);

                // Get the function
                var func = FunctionCache.GetDataCommandParameterSetterFunction(request.Name,
                    fields);

                // Iterate each entity
                foreach (var entity in entities)
                {
                    // Set the values
                    func(command, entity);

                    // Actual Execution
                    await command.ExecuteScalarAsync();

                    // Add to the list
                    result++;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterInsertAll(new TraceLog(commandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region Helpers

        private static int GuardInsertAll<TEntity>(IEnumerable<TEntity> entities)
        {
            var count = entities?.Count();
            if (count <= 0)
            {
                throw new InvalidOperationException("The entities must not be empty or null.");
            }
            return count.Value;
        }

        #endregion
    }
}
