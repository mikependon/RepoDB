using RepoDb.Enumerations;
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
        /*
         * The supposed maximum parameters of 2100 is not workin with Microsoft.Data.SqlClient.
         * I reported this issue to SqlClient repository at Github.
         * Link: https://github.com/dotnet/SqlClient/issues/531
         */

        private const int ParameterBatchCount = Constant.MaxParametersCount - 2;

        #region DeleteAll<TEntity> (Delete<TEntity>)

        /// <summary>
        /// Deletes all the target existing data from the database. It uses the <see cref="Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int DeleteAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
            var primaryKeys = ExtractPropertyValues<TEntity>(entities, primary).AsList();

            return DeleteAll<TEntity>(connection: connection,
                primaryKeys: primaryKeys,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the target existing data from the database. It uses the <see cref="Delete{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int DeleteAll<TEntity>(this IDbConnection connection,
            IEnumerable<object> primaryKeys,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
            var hasImplicitTransaction = false;
            var count = primaryKeys?.AsList()?.Count;
            var deletedRows = 0;

            try
            {
                // Creates a transaction (if needed)
                if (transaction == null && count > ParameterBatchCount)
                {
                    transaction = connection.EnsureOpen().BeginTransaction();
                    hasImplicitTransaction = true;
                }

                // Call the underlying method
                var splitted = primaryKeys.Split(ParameterBatchCount).AsList();
                foreach (var keys in splitted)
                {
                    if (keys.Any() != true)
                    {
                        break;
                    }
                    var field = new QueryField(primary.GetMappedName(), Operation.In, keys.AsList());
                    deletedRows += DeleteInternal<TEntity>(connection: connection,
                        where: new QueryGroup(field),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Commit the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Commit();
                }

            }
            finally
            {
                // Dispose the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Dispose();
                }
            }

            // Return the value
            return deletedRows;
        }

        #endregion

        #region DeleteAllAsync<TEntity> (DeleteAsync<TEntity>)

        /// <summary>
        /// Deletes all the target existing data from the database in an asynchronous way. It uses the <see cref="DeleteAsync{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
            var primaryKeys = ExtractPropertyValues<TEntity>(entities, primary).AsList();

            return DeleteAllAsync<TEntity>(connection: connection,
                primaryKeys: primaryKeys,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the target existing data from the database in an asynchronous way. It uses the <see cref="DeleteAsync{TEntity}(IDbConnection, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<object> primaryKeys,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
            var hasImplicitTransaction = false;
            var count = primaryKeys?.AsList()?.Count;
            var deletedRows = 0;

            try
            {
                // Creates a transaction (if needed)
                if (transaction == null && count > ParameterBatchCount)
                {
                    transaction = connection.EnsureOpen().BeginTransaction();
                    hasImplicitTransaction = true;
                }

                // Call the underlying method
                var splitted = primaryKeys.Split(ParameterBatchCount).AsList();
                foreach (var keys in splitted)
                {
                    if (keys.Any() != true)
                    {
                        break;
                    }
                    var field = new QueryField(primary.GetMappedName(), Operation.In, keys?.AsList());
                    deletedRows += await DeleteAsync<TEntity>(connection: connection,
                        where: new QueryGroup(field),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Commit the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Commit();
                }

            }
            finally
            {
                // Dispose the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Dispose();
                }
            }

            // Return the value
            return deletedRows;
        }

        #endregion

        #region DeleteAll<TEntity>

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int DeleteAll<TEntity>(this IDbConnection connection,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAllInternal<TEntity>(connection: connection,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int DeleteAllInternal<TEntity>(this IDbConnection connection,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                transaction,
                hints,
                statementBuilder);

            // Return the result
            return DeleteAllInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region DeleteAllAsync<TEntity>

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAllAsyncInternal<TEntity>(connection: connection,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> DeleteAllAsyncInternal<TEntity>(this IDbConnection connection,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                transaction,
                hints,
                statementBuilder);

            // Return the result
            return DeleteAllAsyncInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region DeleteAll(TableName) (Delete(TableName))

        /// <summary>
        /// Deletes all the target existing data from the database. It uses the <see cref="Delete(IDbConnection, string, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int DeleteAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            var primary = GetAndGuardPrimaryKey(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();
            var hasImplicitTransaction = false;
            var count = primaryKeys?.AsList()?.Count;
            var deletedRows = 0;

            try
            {
                // Creates a transaction (if needed)
                if (transaction == null && count > ParameterBatchCount)
                {
                    transaction = connection.EnsureOpen().BeginTransaction();
                    hasImplicitTransaction = true;
                }

                // Call the underlying method
                var splitted = primaryKeys.Split(ParameterBatchCount).AsList();
                foreach (var keys in splitted)
                {
                    if (keys.Any() != true)
                    {
                        break;
                    }
                    var field = new QueryField(primary.Name.AsQuoted(dbSetting), Operation.In, keys?.AsList());
                    deletedRows += DeleteInternal(connection: connection,
                        tableName: tableName,
                        where: new QueryGroup(field),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Commit the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Commit();
                }

            }
            finally
            {
                // Dispose the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Dispose();
                }
            }

            // Return the value
            return deletedRows;
        }

        #endregion

        #region DeleteAllAsync(TableName) (DeleteAsync(TableName))

        /// <summary>
        /// Deletes all the target existing data from the database in an asynchronous way. It uses the <see cref="DeleteAsync(IDbConnection, string, QueryGroup, string, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation as the underlying operation.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="primaryKeys">The list of the primary keys to be deleted.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static async Task<int> DeleteAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> primaryKeys,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            var primary = GetAndGuardPrimaryKey(connection, tableName, transaction);
            var dbSetting = connection.GetDbSetting();
            var hasImplicitTransaction = false;
            var count = primaryKeys?.AsList()?.Count;
            var deletedRows = 0;

            try
            {
                // Creates a transaction (if needed)
                if (transaction == null && count > ParameterBatchCount)
                {
                    transaction = connection.EnsureOpen().BeginTransaction();
                    hasImplicitTransaction = true;
                }

                // Call the underlying method
                var splitted = primaryKeys.Split(ParameterBatchCount).AsList();
                foreach (var keys in splitted)
                {
                    if (keys.Any() != true)
                    {
                        break;
                    }
                    var field = new QueryField(primary.Name.AsQuoted(dbSetting), Operation.In, keys?.AsList());
                    deletedRows += await DeleteAsyncInternal(connection: connection,
                        tableName: tableName,
                        where: new QueryGroup(field),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Commit the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Commit();
                }

            }
            finally
            {
                // Dispose the transaction
                if (hasImplicitTransaction)
                {
                    transaction?.Dispose();
                }
            }

            // Return the value
            return deletedRows;
        }

        #endregion

        #region DeleteAll(TableName)

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int DeleteAll(this IDbConnection connection,
            string tableName,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return DeleteAllInternal(connection: connection,
                tableName: tableName,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int DeleteAllInternal(this IDbConnection connection,
            string tableName,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new DeleteAllRequest(tableName,
                connection,
                transaction,
                hints,
                statementBuilder);

            // Return the result
            return DeleteAllInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region DeleteAllAsync(TableName)

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> DeleteAllAsync(this IDbConnection connection,
            string tableName,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return DeleteAllAsyncInternal(connection: connection,
                tableName: tableName,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> DeleteAllAsyncInternal(this IDbConnection connection,
            string tableName,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new DeleteAllRequest(tableName,
                connection,
                transaction,
                hints,
                statementBuilder);

            // Return the result
            return DeleteAllAsyncInternalBase(connection: connection,
                request: request,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region DeleteAllInternalBase

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="DeleteAllRequest"/> object.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int DeleteAllInternalBase(this IDbConnection connection,
            DeleteAllRequest request,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetDeleteAllText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, null, null);
                trace.BeforeDeleteAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterDeleteAll(new TraceLog(commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region DeleteAllAsyncInternalBase

        /// <summary>
        /// Deletes all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="DeleteAllRequest"/> object.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> DeleteAllAsyncInternalBase(this IDbConnection connection,
            DeleteAllRequest request,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetDeleteAllText(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, null, null);
                trace.BeforeDeleteAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteNonQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterDeleteAll(new TraceLog(commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion
    }
}
