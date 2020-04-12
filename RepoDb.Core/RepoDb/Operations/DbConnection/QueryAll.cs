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
        #region QueryAll<TEntity>

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> QueryAll<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAllInternal<TEntity>(connection: connection,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<TEntity> QueryAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAllInternalBase<TEntity>(connection: connection,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAllAsync<TEntity>

        /// <summary>
        /// Query all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAllAsyncInternal<TEntity>(connection: connection,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query all the data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static Task<IEnumerable<TEntity>> QueryAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAllAsyncInternalBase<TEntity>(connection: connection,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAll(TableName)

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<dynamic> QueryAll(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return QueryAllInternal(connection: connection,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<dynamic> QueryAllInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return QueryAllInternalBase(connection: connection,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAllAsync(TableName)

        /// <summary>
        /// Query all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<dynamic>> QueryAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return QueryAllAsyncInternal(connection: connection,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query all the data from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static Task<IEnumerable<dynamic>> QueryAllAsyncInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return QueryAllAsyncInternalBase(connection: connection,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAllInternalBase<TEntity>

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<TEntity> QueryAllInternalBase<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TEntity>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryAllRequest(typeof(TEntity),
                connection,
                transaction,
                FieldCache.Get<TEntity>(),
                orderBy,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryAllText(request);
            var param = (object)null;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryAll(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static async Task<IEnumerable<TEntity>> QueryAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TEntity>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryAllRequest(typeof(TEntity),
                connection,
                transaction,
                FieldCache.Get<TEntity>(),
                orderBy,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryAllText(request);
            var param = (object)null;

            // Database pre-touch for field definitions
            await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryAll(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAllInternalBase(TableName)

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<dynamic> QueryAllInternalBase(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<dynamic>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Check the fields
            if (fields?.Any() != true)
            {
                fields = DbFieldCache.Get(connection, tableName, transaction)?.AsFields();
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryAllRequest(tableName,
                connection,
                transaction,
                fields,
                orderBy,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryAllText(request);
            var param = (object)null;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryAll(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAllAsyncInternalBase(TableName)

        /// <summary>
        /// Query all the data from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The list of fields to be queried.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static async Task<IEnumerable<dynamic>> QueryAllAsyncInternalBase(this IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<dynamic>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Check the fields
            if (fields?.Any() != true)
            {
                fields = (await DbFieldCache.GetAsync(connection, tableName, transaction))?.AsFields();
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryAllRequest(tableName,
                connection,
                transaction,
                fields,
                orderBy,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryAllText(request);
            var param = (object)null;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryAll(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration, false);
            }

            // Result
            return result;
        }

        #endregion
    }
}
