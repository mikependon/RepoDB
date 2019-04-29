using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
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

        #region InsertAllInternalBase

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
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Variables needed
            var identity = IdentityCache.Get<TEntity>();

            // Set the identify value
            if (identity == null)
            {
                var dbField = DbFieldCache.Get(connection, request.Name)?.FirstOrDefault(f => f.IsIdentity);
                if (dbField != null)
                {
                    var properties = PropertyCache.Get<TEntity>();
                    identity = properties.FirstOrDefault(p => p.GetUnquotedMappedName().ToLower() == dbField.UnquotedName.ToLower());
                }
            }

            // Result set
            var result = 0;

            // Iterate and insert every entity
            foreach (var entity in entities)
            {
                // Actual Execution
                var executeResult = ExecuteScalarInternal(connection: connection,
                    commandText: commandText,
                    param: entity,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    skipCommandArrayParametersCheck: true);

                // Set the primary value
                if (identity != null)
                {
                    identity.PropertyInfo.SetValue(entity, executeResult);
                }

                // Add to the list
                result++;
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

        #region InsertAllInternalBase(Optimal)

        ///// <summary>
        ///// Inserts multiple data in the database.
        ///// </summary>
        ///// <typeparam name="TEntity">The type of the objects to be enumerated.</typeparam>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="request">The actual <see cref="InsertAllRequest"/> object.</param>
        ///// <param name="entities">The data entity object to be inserted.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <returns>The number of inserted rows.</returns>
        //internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
        //    InsertAllRequest request,
        //    IEnumerable<TEntity> entities,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null)
        //    where TEntity : class
        //{
        //    // Variables
        //    var commandType = CommandType.Text;
        //    var commandText = CommandTextCache.GetInsertAllText(request);

        //    // Before Execution
        //    if (trace != null)
        //    {
        //        var cancellableTraceLog = new CancellableTraceLog(commandText, entities, null);
        //        trace.BeforeInsertAll(cancellableTraceLog);
        //        if (cancellableTraceLog.IsCancelled)
        //        {
        //            if (cancellableTraceLog.IsThrowException)
        //            {
        //                throw new CancelledExecutionException(commandText);
        //            }
        //            return 0;
        //        }
        //        commandText = (cancellableTraceLog.Statement ?? commandText);
        //        entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
        //    }

        //    // Before Execution Time
        //    var beforeExecutionTime = DateTime.UtcNow;

        //    // Variables needed
        //    var primary = PrimaryCache.Get<TEntity>();
        //    var dbFields = DbFieldCache.Get(connection, request.Name);
        //    var primaryDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);
        //    var isIdentity = false;

        //    // Set the identify value
        //    if (primary != null && primaryDbField != null)
        //    {
        //        isIdentity = primary.GetUnquotedMappedName().ToLower() == primaryDbField.UnquotedName.ToLower();
        //    }

        //    // The actua result
        //    var result = 0;

        //    // Get the properties
        //    var properties = PropertyCache.Get(request.Type);

        //    // Get the first item properties if needed
        //    if (properties == null)
        //    {
        //        properties = PropertyCache.Get(entities.First().GetType());
        //    }

        //    // Set the proper fields
        //    var fields = request.Fields?.Select(f => f.UnquotedName);
        //    var propertiesToSkip = fields?
        //        .Where(field => dbFields?.FirstOrDefault(dbField => dbField.UnquotedName.ToLower() == field.ToLower()) == null);

        //    // Create a command object
        //    using (var command = connection.EnsureOpen().CreateCommand(commandText: commandText,
        //        commandType: commandType,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction))
        //    {
        //        // Create the parameters
        //        command.CreateParametersFromClassProperties(properties, propertiesToSkip);

        //        // Iterate the params
        //        foreach (var obj in entities)
        //        {
        //            // Set the parameters
        //            command.SetParameters(obj, propertiesToSkip, true);

        //            // Execute the command
        //            var value = ObjectConverter.DbNullToNull(command.ExecuteScalar());

        //            // Set the property value
        //            primary?.PropertyInfo.SetValue(obj, value);

        //            // Iterate the counters
        //            result++;
        //        }
        //    }

        //    // After Execution
        //    if (trace != null)
        //    {
        //        trace.AfterInsertAll(new TraceLog(commandText, entities, result,
        //            DateTime.UtcNow.Subtract(beforeExecutionTime)));
        //    }

        //    // Return the result
        //    return result;
        //}

        #endregion

        #region InsertAllAsyncInternalBase

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
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Variables needed
            var identity = IdentityCache.Get<TEntity>();

            // Set the identify value
            if (identity == null)
            {
                var dbField = DbFieldCache.Get(connection, request.Name)?.FirstOrDefault(f => f.IsIdentity);
                if (dbField != null)
                {
                    var properties = PropertyCache.Get<TEntity>();
                    identity = properties.FirstOrDefault(p => p.GetUnquotedMappedName().ToLower() == dbField.UnquotedName.ToLower());
                }
            }

            #region 

            // Note: This code is commented as it is vigorously intensive in a CPU by creating a task
            // based on the number of data entity to be inserted

            // Result set
            //var result = 0;
            //var tasks = new List<Task<Tuple<TEntity, object>>>();

            //// Iterate and insert every entity
            //foreach (var entity in entities)
            //{
            //    /* TODO : Can we do an 'await' here at 'ExecuteScalarAsyncInternal', but it would sure affect the CPU */
            //    var executeResult = await ExecuteScalarAsyncInternal(connection: connection,
            //        commandText: commandText,
            //        param: entity,
            //        commandType: commandType,
            //        commandTimeout: commandTimeout,
            //        transaction: transaction);

            //    // Create a new tuple
            //    var tuple = Tuple.Create(entity, executeResult);

            //    // Add to the tasks list
            //    tasks.Add(Task.FromResult(tuple));

            //    // Increase the result
            //    result++;
            //}

            //// Await all here
            //await Task.WhenAll(tasks);

            //// Iterate after all has completed
            //foreach (var task in tasks)
            //{
            //    // Get the result
            //    var result = task.Result;

            //    // Set the primary value
            //    if (primary != null && isIdentity == true)
            //    {
            //        primary.PropertyInfo.SetValue(result.Item1, result.Item2);
            //    }
            //}

            #endregion

            // Result set
            var result = 0;

            // Iterate and insert every entity
            foreach (var entity in entities)
            {
                // Actual Execution
                var executeResult = await ExecuteScalarAsyncInternal(connection: connection,
                    commandText: commandText,
                    param: entity,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    skipCommandArrayParametersCheck: true);

                // Set the primary value
                if (identity != null)
                {
                    identity.PropertyInfo.SetValue(entity, executeResult);
                }

                // Add to the list
                result++;
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
