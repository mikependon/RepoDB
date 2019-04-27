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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static void InsertAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            InsertAllInternal<TEntity>(connection: connection,
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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static void InsertAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new InsertAllRequest(typeof(TEntity),
                connection,
                FieldCache.Get<TEntity>(),
                statementBuilder);

            // Return the result
            InsertAllInternalBase(connection: connection,
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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static Task InsertAllAsync<TEntity>(this IDbConnection connection,
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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static Task InsertAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
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
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        public static void InsertAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            InsertAllInternal(connection: connection,
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
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        internal static void InsertAllInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
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

            // Variables
            var request = new InsertAllRequest(tableName,
                connection,
                fields,
                statementBuilder);

            // Return the result
            InsertAllInternalBase(connection: connection,
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
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        public static Task InsertAllAsync(this IDbConnection connection,
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
        /// <returns>The value of the primary key of the newly inserted data.</returns>
        internal static Task InsertAllAsyncInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> fields = null,
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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static void InsertAllInternalBase<TEntity>(this IDbConnection connection,
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
                    return;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Variables needed
            var primary = PrimaryCache.Get<TEntity>();
            var dbField = DbFieldCache.Get(connection, request.Name)?.FirstOrDefault(f => f.IsIdentity);
            var isIdentity = false;

            // Set the identify value
            if (primary != null && dbField != null)
            {
                isIdentity = primary.GetUnquotedMappedName().ToLower() == dbField.UnquotedName.ToLower();
            }

            // Result set
            var results = new List<object>();

            // Iterate and insert every entity
            foreach (var entity in entities)
            {
                // Actual Execution
                var result = ExecuteScalarInternal(connection: connection,
                    commandText: commandText,
                    param: entity,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction);

                // Set the primary value
                if (primary != null && isIdentity == true)
                {
                    primary.PropertyInfo.SetValue(entity, result);
                }

                // Add to the list
                results.Add(result);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterInsertAll(new TraceLog(commandText, entities, results,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }
        }

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
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static async Task InsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
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
                    return;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Variables needed
            var primary = PrimaryCache.Get<TEntity>();
            var dbField = DbFieldCache.Get(connection, request.Name)?.FirstOrDefault(f => f.IsIdentity);
            var isIdentity = false;

            // Set the identify value
            if (primary != null && dbField != null)
            {
                isIdentity = primary.GetUnquotedMappedName().ToLower() == dbField.UnquotedName.ToLower();
            }

            // Result set
            var results = new List<object>();

            // Iterate and insert every entity
            foreach (var entity in entities)
            {
                // Actual Execution
                var result = await ExecuteScalarAsyncInternal(connection: connection,
                    commandText: commandText,
                    param: entity,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction);

                // Set the primary value
                if (primary != null && isIdentity == true)
                {
                    primary.PropertyInfo.SetValue(entity, result);
                }

                // Add to the list
                results.Add(result);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterInsertAll(new TraceLog(commandText, entities, results,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }
        }

        #endregion
    }
}
