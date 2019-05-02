using RepoDb.Contexts.Execution;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;
using System;
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
        #region SubClasses

        private static class InsertAllCommandExecutionContextCache<TEntity>
            where TEntity : class
        {
            private static InsertAllCommandExecutionContext<TEntity> m_context;

            public static InsertAllCommandExecutionContext<TEntity> Get(Func<InsertAllCommandExecutionContext<TEntity>> callback)
            {
                if (m_context != null)
                {
                    return m_context;
                }
                return m_context = callback();
            }
        }
        
        #endregion

        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternal<TEntity>(connection: connection,
                entities: entities,
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
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Guard the parameters
            GuardInsertAll(entities);

            // Variables
            var request = new InsertAllRequest(typeof(TEntity),
                connection,
                FieldCache.Get<TEntity>(),
                statementBuilder);

            // Return the result
            return InsertAllInternalBase(connection: connection,
                request: request,
                entities: entities,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The data entity objects to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
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
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Guard the parameters
            GuardInsertAll(entities);

            // Variables
            var request = new InsertAllRequest(typeof(TEntity),
                connection,
                FieldCache.Get<TEntity>(),
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

        #region InsertAll(TableName)

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used. Defaulted to database table fields.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAllInternal(connection: connection,
                tableName: tableName,
                entities: entities,
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
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
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
                statementBuilder);

            // Return the result
            return InsertAllInternalBase(connection: connection,
                request: request,
                entities: entities,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region InsertAllAsync(TableName)

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The dynamic objects to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used. Defaulted to database table fields.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAllAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                fields: fields,
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
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
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
        /// <param name="request">The actual <see cref="InsertAllRequest"/> object.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
            InsertAllRequest request,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
                // Get the function
                var callback = new Func<InsertAllCommandExecutionContext<TEntity>>(() =>
                {
                    // Variables needed
                    var commandText = CommandTextCache.GetInsertAllText(request);
                    var fields = FieldCache.Get<TEntity>();
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
                        fields = fields
                            .Where(field =>
                                identity?.GetUnquotedMappedName().ToLower() != field.UnquotedName.ToLower())
                            .Where(field =>
                                dbFields.FirstOrDefault(df => df.UnquotedName.ToLower() == field.UnquotedName.ToLower()) != null)
                            .ToList();
                    }

                    // Return the value
                    return new InsertAllCommandExecutionContext<TEntity>
                    {
                        CommandText = commandText,
                        Identity = identity,
                        Fields = fields,
                        Executor = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(fields)
                    };
                });

            // Create a command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand())
            {
                // Get the context
                var context = InsertAllCommandExecutionContextCache<TEntity>.Get(callback);

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

                // Result set
                var result = 0;

                // Iterate each entity
                foreach (var entity in entities)
                {
                    // Set the values
                    context.Executor(command, entity);

                    // Actual Execution
                    if (context.Identity != null)
                    {
                        context.Identity.PropertyInfo.SetValue(entity, command.ExecuteScalar());
                    }
                    else
                    {
                        command.ExecuteScalar();
                    }

                    // Add to the list
                    result++;
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
        /// <param name="request">The actual <see cref="InsertAllRequest"/> object.</param>
        /// <param name="entities">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static async Task<int> InsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            InsertAllRequest request,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Create a command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand())
            {
                // Get the function
                var callback = new Func<InsertAllCommandExecutionContext<TEntity>>(() =>
                {
                    // Variables needed
                    var commandText = CommandTextCache.GetInsertAllText(request);
                    var fields = FieldCache.Get<TEntity>();
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
                        fields = fields
                            .Where(field =>
                                identity?.GetUnquotedMappedName().ToLower() != field.UnquotedName.ToLower())
                            .Where(field =>
                                dbFields.FirstOrDefault(df => df.UnquotedName.ToLower() == field.UnquotedName.ToLower()) != null)
                            .ToList();
                    }

                    // Return the value
                    return new InsertAllCommandExecutionContext<TEntity>
                    {
                        CommandText = commandText,
                        Identity = identity,
                        Fields = fields,
                        Executor = FunctionCache.GetDataCommandParameterSetterFunction<TEntity>(fields)
                    };
                });

                // Get the context
                var context = InsertAllCommandExecutionContextCache<TEntity>.Get(callback);

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

                // Result set
                var result = 0;

                // Iterate each entity
                foreach (var entity in entities)
                {
                    // Set the values
                    context.Executor(command, entity);

                    // Actual Execution
                    if (context.Identity != null)
                    {
                        context.Identity.PropertyInfo.SetValue(entity, await command.ExecuteScalarAsync());
                    }
                    else
                    {
                        command.ExecuteScalar();
                    }

                    // Add to the list
                    result++;
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
        
        private static void GuardInsertAll<TEntity>(IEnumerable<TEntity> entities)
        {
            if (entities?.Any() != true)
            {
                throw new InvalidOperationException("The entities must not be empty or null.");
            }
        }

        #endregion
    }
}
