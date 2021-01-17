using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region SubClasses

        /// <summary>
        ///
        /// </summary>
        internal class CommandArrayParametersText
        {
            public string CommandText { get; set; }
            public IList<CommandArrayParameter> CommandArrayParameters { get; } = new List<CommandArrayParameter>();
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Creates a command object.
        /// </summary>
        /// <param name="connection">The connection to be used when creating a command object.</param>
        /// <param name="commandText">The value of the <see cref="IDbCommand.CommandText"/> property.</param>
        /// <param name="commandType">The value of the <see cref="IDbCommand.CommandType"/> property.</param>
        /// <param name="commandTimeout">The value of the <see cref="IDbCommand.CommandTimeout"/> property.</param>
        /// <param name="transaction">The value of the <see cref="IDbCommand.Transaction"/> property.</param>
        /// <returns>A command object instance containing the defined property values passed.</returns>
        public static IDbCommand CreateCommand(this IDbConnection connection,
            string commandText,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Connection = connection;
            if (commandType != null)
            {
                command.CommandType = commandType.Value;
            }
            if (commandTimeout != null)
            {
                command.CommandTimeout = commandTimeout.Value;
            }
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            return command;
        }

        /// <summary>
        /// Ensures the connection object is open.
        /// </summary>
        /// <param name="connection">The connection to be opened.</param>
        /// <returns>The instance of the current connection object.</returns>
        public static IDbConnection EnsureOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// Ensures the connection object is open in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection to be opened.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The instance of the current connection object.</returns>
        public static async Task<IDbConnection> EnsureOpenAsync(this IDbConnection connection,
            CancellationToken cancellationToken = default)
        {
            if (connection.State != ConnectionState.Open)
            {
                await ((DbConnection)connection).OpenAsync(cancellationToken);
            }
            return connection;
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static IEnumerable<dynamic> ExecuteQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null)
        {
            return ExecuteQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                tableName: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static IEnumerable<dynamic> ExecuteQueryInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<dynamic>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // DB Fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                DbFieldCache.Get(connection, tableName, transaction, false) : null;

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = (IEnumerable<dynamic>)null;

                // Execute
                using (var reader = command.ExecuteReader())
                {
                    result = DataReader.ToEnumerable(reader, dbFields, connection.GetDbSetting()).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<dynamic>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteQueryAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static Task<IEnumerable<dynamic>> ExecuteQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                cancellationToken: cancellationToken,
                tableName: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<dynamic>> ExecuteQueryAsyncInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<dynamic>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // DB Fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                await DbFieldCache.GetAsync(connection, tableName, transaction, false, cancellationToken) : null;

            // Execute the actual method
            using (var command = await CreateDbCommandForExecutionAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = (IEnumerable<dynamic>)null;

                // Execute
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    result = (await DataReader.ToEnumerableAsync(reader, dbFields, connection.GetDbSetting(), cancellationToken)).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<dynamic>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteQuery<TResult>

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of the target result type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <returns>
        /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static IEnumerable<TResult> ExecuteQuery<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null)
        {
            return ExecuteQueryInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                tableName: ClassMappedNameCache.Get<TResult>(),
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static IEnumerable<TResult> ExecuteQueryInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Variables
            var typeOfResult = typeof(TResult);

            // Identify
            if (typeOfResult.IsDictionaryStringObject() || typeOfResult.IsObjectType())
            {
                return ExecuteQueryInternalForDictionaryStringObject<TResult>(connection: connection,
                   commandText: commandText,
                   param: param,
                   commandType: commandType,
                   cacheKey: cacheKey,
                   cacheItemExpiration: cacheItemExpiration,
                   commandTimeout: commandTimeout,
                   transaction: transaction,
                   cache: cache,
                   tableName: tableName,
                   skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            }
            else
            {
                return ExecuteQueryInternalForType<TResult>(connection: connection,
                   commandText: commandText,
                   param: param,
                   commandType: commandType,
                   cacheKey: cacheKey,
                   cacheItemExpiration: cacheItemExpiration,
                   commandTimeout: commandTimeout,
                   transaction: transaction,
                   cache: cache,
                   tableName: tableName,
                   skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        private static IEnumerable<TResult> ExecuteQueryInternalForDictionaryStringObject<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Call
            var result = ExecuteQueryInternal(connection: connection,
               commandText: commandText,
               param: param,
               commandType: commandType,
               cacheKey: null,
               cacheItemExpiration: null,
               commandTimeout: commandTimeout,
               transaction: transaction,
               cache: null,
               tableName: tableName,
               skipCommandArrayParametersCheck: skipCommandArrayParametersCheck).WithType<TResult>();

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
            }

            // Set the output parameters
            SetOutputParameters(param);

            // Return
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        private static IEnumerable<TResult> ExecuteQueryInternalForType<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // DB Fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                DbFieldCache.Get(connection, tableName, transaction, false) : null;

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: typeof(TResult),
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = (IEnumerable<TResult>)null;

                // Execute
                using (var reader = command.ExecuteReader())
                {
                    result = DataReader.ToEnumerable<TResult>(reader, dbFields, connection.GetDbSetting()).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<TResult>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteQueryAsync<TResult>

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of the target result type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>
        /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteQueryAsyncInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                cache: cache,
                tableName: ClassMappedNameCache.Get<TResult>(),
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static Task<IEnumerable<TResult>> ExecuteQueryAsyncInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return Task.FromResult(item.Value);
                }
            }

            // Variables
            var typeOfResult = typeof(TResult);

            // Identify
            if (typeOfResult.IsDictionaryStringObject() || typeOfResult.IsObjectType())
            {
                return ExecuteQueryAsyncInternalForDictionaryStringObject<TResult>(connection: connection,
                   commandText: commandText,
                   param: param,
                   commandType: commandType,
                   cacheKey: cacheKey,
                   cacheItemExpiration: cacheItemExpiration,
                   commandTimeout: commandTimeout,
                   transaction: transaction,
                   cache: cache,
                   cancellationToken: cancellationToken,
                   tableName: tableName,
                   skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            }
            else
            {
                return ExecuteQueryAsyncInternalForType<TResult>(connection: connection,
                   commandText: commandText,
                   param: param,
                   commandType: commandType,
                   cacheKey: cacheKey,
                   cacheItemExpiration: cacheItemExpiration,
                   commandTimeout: commandTimeout,
                   transaction: transaction,
                   cache: cache,
                   cancellationToken: cancellationToken,
                   tableName: tableName,
                   skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="cache"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<TResult>> ExecuteQueryAsyncInternalForDictionaryStringObject<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Call
            var result = (await ExecuteQueryAsyncInternal(connection: connection,
               commandText: commandText,
               param: param,
               commandType: commandType,
               cacheKey: null,
               cacheItemExpiration: null,
               commandTimeout: commandTimeout,
               transaction: transaction,
               cache: null,
               cancellationToken: cancellationToken,
               tableName: tableName,
               skipCommandArrayParametersCheck: skipCommandArrayParametersCheck)).WithType<TResult>();

            // Set Cache
            if (cacheKey != null)
            {
                cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
            }

            // Set the output parameters
            SetOutputParameters(param);

            // Return
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="tableName"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<TResult>> ExecuteQueryAsyncInternalForType<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default,
            string tableName = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<IEnumerable<TResult>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // DB Fields
            var dbFields = !string.IsNullOrWhiteSpace(tableName) ?
                await DbFieldCache.GetAsync(connection, tableName, transaction, false, cancellationToken) : null;

            // Execute the actual method
            using (var command = await CreateDbCommandForExecutionAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: typeof(TResult),
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = (IEnumerable<TResult>)null;

                // Execute
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    result = (await DataReader.ToEnumerableAsync<TResult>(reader, dbFields,
                        connection.GetDbSetting(), cancellationToken)).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<TResult>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteQueryMultiple(Results)

        /// <summary>
        /// Execute the multiple SQL statements from the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
        public static QueryMultipleExtractor ExecuteQueryMultiple(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null) =>
            ExecuteQueryMultipleInternal(connection,
                commandText,
                param,
                commandType,
                commandTimeout,
                transaction,
                false);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="isDisposeConnection"></param>
        /// <returns></returns>
        internal static QueryMultipleExtractor ExecuteQueryMultipleInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            bool isDisposeConnection = false)
        {
            // Call
            var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);

            // Return
            return new QueryMultipleExtractor((DbConnection)connection, (DbDataReader)reader, isDisposeConnection, param);
        }

        #endregion

        #region ExecuteQueryMultipleAsync(Results)

        /// <summary>
        /// Execute the multiple SQL statements from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
        public static Task<QueryMultipleExtractor> ExecuteQueryMultipleAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            ExecuteQueryMultipleAsyncInternal(connection,
                commandText,
                param,
                commandType,
                commandTimeout,
                transaction,
                false,
                cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="isDisposeConnection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<QueryMultipleExtractor> ExecuteQueryMultipleAsyncInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            bool isDisposeConnection = false,
            CancellationToken cancellationToken = default)
        {
            // Call
            var reader = await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);

            // Return
            return new QueryMultipleExtractor((DbConnection)connection, (DbDataReader)reader,
                isDisposeConnection, param, cancellationToken);
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns><returns>The instance of the <see cref="IDataReader"/> object.</returns></returns>
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static IDataReader ExecuteReaderInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            // Variables
            var setting = DbSettingMapper.Get(connection.GetType());
            var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            var hasError = false;

            // Ensure the DbCommand disposal
            try
            {
                var reader = command.ExecuteReader();

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return reader;
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                if (setting?.IsExecuteReaderDisposable == true || hasError)
                {
                    command.Dispose();
                }
            }
        }

        #endregion

        #region ExecuteReaderAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns><returns>The instance of the <see cref="IDataReader"/> object.</returns></returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static async Task<IDataReader> ExecuteReaderAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            // Variables
            var setting = connection.GetDbSetting();
            var command = await CreateDbCommandForExecutionAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            var hasError = false;

            // Ensure the DbCommand disposal
            try
            {
                var reader = await command.ExecuteReaderAsync(cancellationToken);

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return reader;
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                if (setting.IsExecuteReaderDisposable || hasError)
                {
                    command.Dispose();
                }
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int ExecuteNonQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static int ExecuteNonQueryInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = command.ExecuteNonQuery();

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> ExecuteNonQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteNonQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static async Task<int> ExecuteNonQueryAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            CancellationToken cancellationToken,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = await CreateDbCommandForExecutionAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = await command.ExecuteNonQueryAsync(cancellationToken);

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
        public static object ExecuteScalar(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null)
        {
            return ExecuteScalarInternal<object>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        #endregion

        #region ExecuteScalarAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An object that holds the first occurrence value (first column of first row) of the execution.</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsyncInternal<object>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        #endregion

        #region ExecuteScalar<TResult>

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
        public static TResult ExecuteScalar<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null)
        {
            return ExecuteScalarInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static TResult ExecuteScalarInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            string cacheKey,
            int? cacheItemExpiration,
            int? commandTimeout,
            IDbTransaction transaction,
            ICache cache,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<TResult>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = Converter.ToType<TResult>(command.ExecuteScalar());

                // Set Cache
                if (cacheKey != null)
                {
                    cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region ExecuteScalarAsync<TResult>

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurrence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="cacheKey">
        /// The key to the cache item.By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A first occurrence value (first column of first row) of the execution.</returns>
        public static Task<TResult> ExecuteScalarAsync<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsyncInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cache"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static async Task<TResult> ExecuteScalarAsyncInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            string cacheKey,
            int? cacheItemExpiration,
            int? commandTimeout,
            IDbTransaction transaction,
            ICache cache,
            CancellationToken cancellationToken,
            Type entityType,
            IEnumerable<DbField> dbFields,
            bool skipCommandArrayParametersCheck)
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get<TResult>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            using (var command = await CreateDbCommandForExecutionAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                var result = Converter.ToType<TResult>(await command.ExecuteScalarAsync(cancellationToken));

                // Set Cache
                if (cacheKey != null)
                {
                    cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                }

                // Set the output parameters
                SetOutputParameters(param);

                // Return
                return result;
            }
        }

        #endregion

        #region Mapped Operations

        /// <summary>
        /// Gets the associated <see cref="IDbSetting"/> object that is currently mapped on the target <see cref="IDbConnection"/> object.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <returns>An instance of the mapped <see cref="IDbSetting"/> object.</returns>
        public static IDbSetting GetDbSetting(this IDbConnection connection)
        {
            // Check the connection
            if (connection == null)
            {
                throw new NullReferenceException("The connection object cannot be null.");
            }

            // Get the setting
            var setting = DbSettingMapper.Get(connection.GetType());

            // Check the presence
            if (setting == null)
            {
                throw new MissingMappingException($"There is no database setting mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorial/installation).");
            }

            // Return the validator
            return setting;
        }

        /// <summary>
        /// Gets the associated <see cref="IDbHelper"/> object that is currently mapped on the target <see cref="IDbConnection"/> object.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <returns>An instance of the mapped <see cref="IDbHelper"/> object.</returns>
        public static IDbHelper GetDbHelper(this IDbConnection connection)
        {
            // Check the connection
            if (connection == null)
            {
                throw new NullReferenceException("The connection object cannot be null.");
            }

            // Get the setting
            var helper = DbHelperMapper.Get(connection.GetType());

            // Check the presence
            if (helper == null)
            {
                throw new MissingMappingException($"There is no database helper mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorial/installation).");
            }

            // Return the validator
            return helper;
        }

        /// <summary>
        /// Gets the associated <see cref="IStatementBuilder"/> object that is currently mapped on the target <see cref="IDbConnection"/> object.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <returns>An instance of the mapped <see cref="IStatementBuilder"/> object.</returns>
        public static IStatementBuilder GetStatementBuilder(this IDbConnection connection)
        {
            // Check the connection
            if (connection == null)
            {
                throw new NullReferenceException("The connection object cannot be null.");
            }

            // Get the setting
            var statementBuilder = StatementBuilderMapper.Get(connection.GetType());

            // Check the presence
            if (statementBuilder == null)
            {
                throw new MissingMappingException($"There is no database statement builder mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorial/installation).");
            }

            // Return the validator
            return statementBuilder;
        }

        #endregion

        #region Helper Methods

        #region DbParameters

        /// <summary>
        ///
        /// </summary>
        /// <param name="param"></param>
        internal static void SetOutputParameters(object param)
        {
            if (param is QueryGroup group)
            {
                SetOutputParameters(group);
            }
            else if (param is IEnumerable<QueryField> fields)
            {
                SetOutputParameters(fields);
            }
            else if (param is QueryField field)
            {
                SetOutputParameter(field);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryGroup"></param>
        internal static void SetOutputParameters(QueryGroup queryGroup) =>
            SetOutputParameters(queryGroup.GetFields(true));

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryFields"></param>
        internal static void SetOutputParameters(IEnumerable<QueryField> queryFields)
        {
            if (queryFields?.Any() != true)
            {
                return;
            }
            foreach (var queryField in queryFields.Where(e => e.DbParameter?.Direction != ParameterDirection.Input))
            {
                SetOutputParameter(queryField);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryField"></param>
        internal static void SetOutputParameter(QueryField queryField)
        {
            if (queryField == null)
            {
                return;
            }
            queryField.Parameter.SetValue(queryField.GetValue());
        }

        #endregion

        #region Order Columns

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="command"></param>
        /// <param name="entities"></param>
        private static void AddOrderColumnParameters<TEntity>(DbCommand command,
            IEnumerable<TEntity> entities)
            where TEntity : class
        {
            var index = 0;
            foreach (var item in entities)
            {
                var parameter = command.CreateParameter($"__RepoDb_OrderColumn_{index}", index, DbType.Int32);
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 4;
                parameter.Precision = 10;
                parameter.Scale = 0;
                command.Parameters.Add(parameter);
                index++;
            }
        }

        #endregion

        #region GetAndGuardPrimaryKeyOrIdentityKey

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Field GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(IDbConnection connection,
            IDbTransaction transaction)
            where TEntity : class =>
            GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Field GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
            where TEntity : class =>
            GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction, typeof(TEntity));

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static Field GetAndGuardPrimaryKeyOrIdentityKey(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            Type entity)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var key = GetAndGuardPrimaryKeyOrIdentityKey(entity, dbFields);
            if (key == null)
            {
                key = GetPrimaryOrIdentityKey(entity);
            }
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, key);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<Field> GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction, cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<Field> GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction, typeof(TEntity), cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<Field> GetAndGuardPrimaryKeyOrIdentityKeyAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            Type entityType,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var property = GetAndGuardPrimaryKeyOrIdentityKey(entityType, dbFields);
            if (property == null)
            {
                property = GetPrimaryOrIdentityKey(entityType);
            }
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, property);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static Field GetAndGuardPrimaryKeyOrIdentityKey(string tableName,
            Field field)
        {
            if (field == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the table '{tableName}'.");
            }
            return field;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static Field GetAndGuardPrimaryKeyOrIdentityKey(Type entityType,
            IEnumerable<DbField> dbFields)
        {
            if (entityType == null)
            {
                return null;
            }

            // Properties
            var key = (Field)null;

            if (entityType.IsDictionaryStringObject())
            {
                // Primary/Identity
                var dbField = dbFields?.FirstOrDefault(df => df.IsPrimary == true) ??
                    dbFields?.FirstOrDefault(df => df.IsPrimary == true) ??
                    dbFields?.FirstOrDefault(df => df.Name == "Id");

                // Set the key
                key = dbField?.AsField();

                // Return
                if (key == null)
                {
                    throw new KeyFieldNotFoundException($"No primary key and identify found at the target table and also to the given '{entityType.FullName}' object.");
                }
            }
            else
            {
                // Properties
                var properties = PropertyCache.Get(entityType) ?? entityType.GetClassProperties();
                var property = (ClassProperty)null;

                // Primary
                if (property == null)
                {
                    var dbField = dbFields?.FirstOrDefault(df => df.IsPrimary == true);
                    property = properties?.FirstOrDefault(p =>
                         string.Equals(p.GetMappedName(), dbField?.Name, StringComparison.OrdinalIgnoreCase)) ??
                         PrimaryCache.Get(entityType);
                }

                // Identity
                if (property == null)
                {
                    var dbField = dbFields?.FirstOrDefault(df => df.IsIdentity == true);
                    property = properties?.FirstOrDefault(p =>
                         string.Equals(p.GetMappedName(), dbField?.Name, StringComparison.OrdinalIgnoreCase)) ??
                         PrimaryCache.Get(entityType);
                }

                // Set the key
                key = property?.AsField();

                // Return
                if (key == null)
                {
                    throw new KeyFieldNotFoundException($"No primary key and identify found at type '{entityType.FullName}'.");
                }
            }

            // Return
            return key;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static DbField GetAndGuardPrimaryKeyOrIdentityKey(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var dbField = dbFields?.FirstOrDefault(df => df.IsPrimary == true) ?? dbFields?.FirstOrDefault(df => df.IsIdentity == true);
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, dbField);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<DbField> GetAndGuardPrimaryKeyOrIdentityKeyAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var dbField = dbFields?.FirstOrDefault(df => df.IsPrimary == true) ?? dbFields?.FirstOrDefault(df => df.IsIdentity == true);
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, dbField);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbField"></param>
        /// <returns></returns>
        internal static DbField GetAndGuardPrimaryKeyOrIdentityKey(string tableName,
            DbField dbField)
        {
            if (dbField == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the table '{tableName}'.");
            }
            return dbField;
        }

        #endregion

        #region WhatToQueryGroup

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="what"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(this IDbConnection connection,
            string tableName,
            T what,
            IDbTransaction transaction)
        {
            if (what == null)
            {
                return null;
            }
            var queryGroup = WhatToQueryGroup<T>(what);
            if (queryGroup == null)
            {
                var whatType = what.GetType();
                if (whatType.IsClassType() || whatType.IsAnonymousType())
                {
                    var field = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction, whatType);
                    queryGroup = WhatToQueryGroup<T>(field, what);
                }
                else
                {
                    var dbField = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction);
                    queryGroup = WhatToQueryGroup<T>(dbField, what);
                }
            }
            return queryGroup;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="what"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<QueryGroup> WhatToQueryGroupAsync<T>(this IDbConnection connection,
            string tableName,
            T what,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            if (what == null)
            {
                return null;
            }
            var queryGroup = WhatToQueryGroup<T>(what);
            if (queryGroup == null)
            {
                var whatType = what.GetType();
                if (whatType.IsClassType() || whatType.IsAnonymousType())
                {
                    var field = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction, whatType, cancellationToken);
                    queryGroup = WhatToQueryGroup<T>(field, what);
                }
                else
                {
                    var dbField = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction, cancellationToken);
                    queryGroup = WhatToQueryGroup<T>(dbField, what);
                }
            }
            return queryGroup;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="what"></param>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(string tableName,
            T what,
            IEnumerable<DbField> dbFields)
        {
            var key = dbFields?.FirstOrDefault(p => p.IsPrimary == true) ?? dbFields?.FirstOrDefault(p => p.IsIdentity == true);
            if (key == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the table '{tableName}'.");
            }
            else
            {
                return WhatToQueryGroup(key, what);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="what"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<TEntity>(IDbConnection connection,
            object what,
            IDbTransaction transaction)
            where TEntity : class
        {
            if (what == null)
            {
                return null;
            }
            var queryGroup = WhatToQueryGroup(what);
            if (queryGroup != null)
            {
                return queryGroup;
            }
            var key = GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
            return WhatToQueryGroup(key, what);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="what"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<QueryGroup> WhatToQueryGroupAsync<TEntity>(IDbConnection connection,
            object what,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (what == null)
            {
                return null;
            }
            var queryGroup = WhatToQueryGroup(what);
            if (queryGroup != null)
            {
                return queryGroup;
            }
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction, cancellationToken);
            return WhatToQueryGroup(key, what);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbField"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(DbField dbField,
            T what)
        {
            if (what == null)
            {
                return null;
            }
            var type = typeof(T);
            var properties = PropertyCache.Get(type) ?? type.GetClassProperties();
            var property = properties?
                .FirstOrDefault(p => string.Equals(p.GetMappedName(), dbField.Name, StringComparison.OrdinalIgnoreCase));
            if (property != null)
            {
                return WhatToQueryGroup<T>(property.AsField(), what);
            }
            else
            {
                return new QueryGroup(new QueryField(dbField.AsField(), what));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(Field field,
            T what)
        {
            var type = typeof(T);
            if (field == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the type '{type.FullName}'.");
            }
            if (type.IsClassType())
            {
                var classProperty = PropertyCache.Get(typeof(T), field);
                return new QueryGroup(classProperty?.PropertyInfo.AsQueryField(what));
            }
            else
            {
                return new QueryGroup(new QueryField(field, what));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(T what)
        {
            if (what == null)
            {
                return null;
            }
            else if (what is QueryField field)
            {
                return ToQueryGroup(field);
            }
            else if (what is IEnumerable<QueryField> fields)
            {
                return ToQueryGroup(fields);
            }
            else if (what is QueryGroup group)
            {
                return group;
            }
            else
            {
                var type = typeof(T).GetUnderlyingType();
                if (type.IsAnonymousType() || type == StaticType.Object)
                {
                    return QueryGroup.Parse(what, false);
                }
            }
            return null;
        }

        #endregion

        #region ToQueryGroup

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var type = obj.GetType();
            if (type.IsClassType())
            {
                return QueryGroup.Parse(obj, true);
            }
            else
            {
                throw new Exceptions.InvalidExpressionException("Only dynamic object is supported in the 'where' expression.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbField"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(DbField dbField,
            object entity)
        {
            if (entity == null)
            {
                return null;
            }
            if (dbField != null)
            {
                var type = entity.GetType();
                if (type.IsClassType())
                {
                    var properties = PropertyCache.Get(type) ?? type.GetClassProperties();
                    var property = properties?
                        .FirstOrDefault(p => string.Equals(p.GetMappedName(), dbField.Name, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        return new QueryGroup(property.PropertyInfo.AsQueryField(entity));
                    }
                }
                else
                {
                    return new QueryGroup(new QueryField(dbField.AsField(), entity));
                }
            }
            throw new KeyFieldNotFoundException($"No primary key and identity key found.");
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup<TEntity>(Expression<Func<TEntity, bool>> where)
            where TEntity : class
        {
            if (where == null)
            {
                return null;
            }
            return QueryGroup.Parse<TEntity>(where);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(Field field,
            IDictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey(field.Name))
            {
                throw new MissingFieldsException($"The field '{field.Name}' is not found from the given dictionary object.");
            }
            return ToQueryGroup(new QueryField(field, dictionary[field.Name]));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup<TEntity>(Field field,
            TEntity entity)
            where TEntity : class
        {
            var type = entity?.GetType() ?? typeof(TEntity);
            return type.IsDictionaryStringObject() ? ToQueryGroup(field, (IDictionary<string, object>)entity) :
                ToQueryGroup(PropertyCache.Get<TEntity>(field) ?? PropertyCache.Get(type, field), entity);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup<TEntity>(ClassProperty property,
            TEntity entity)
            where TEntity : class =>
            ToQueryGroup(property.PropertyInfo.AsQueryField(entity));

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryField"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(QueryField queryField)
        {
            if (queryField == null)
            {
                return null;
            }
            return new QueryGroup(queryField);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(IEnumerable<QueryField> queryFields)
        {
            if (queryFields == null)
            {
                return null;
            }
            return new QueryGroup(queryFields);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        internal static Field GetPrimaryOrIdentityKey(Type entityType) =>
            entityType != null ? (PrimaryCache.Get(entityType) ?? IdentityCache.Get(entityType))?.AsField() : null;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        internal static void ThrowIfNullOrEmpty<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities must not be null.");
            }
            if (entities.Any() == false)
            {
                throw new EmptyException("The entities must not be empty.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        internal static void ValidateTransactionConnectionObject(this IDbConnection connection,
            IDbTransaction transaction)
        {
            if (transaction != null && transaction.Connection != connection)
            {
                throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <param name="where"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        internal static void WhereToCommandParameters(DbCommand command,
            QueryGroup where,
            Type entityType,
            IEnumerable<DbField> dbFields) =>
            DbCommandExtension.CreateParameters(command, where, null, entityType, dbFields);

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        internal static QueryGroup CreateQueryGroupForUpsert(object entity,
            IEnumerable<ClassProperty> properties,
            IEnumerable<Field> qualifiers = null)
        {
            var queryFields = new List<QueryField>();
            foreach (var field in qualifiers)
            {
                var property = properties?.FirstOrDefault(
                    p => string.Equals(p.GetMappedName(), field.Name, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    queryFields.Add(new QueryField(field, property.PropertyInfo.GetValue(entity)));
                }
            }
            return new QueryGroup(queryFields);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="qualifiers"></param>
        /// <returns></returns>
        internal static QueryGroup CreateQueryGroupForUpsert(IDictionary<string, object> dictionary,
            IEnumerable<Field> qualifiers = null)
        {
            if (qualifiers?.Any() != true)
            {
                throw new MissingFieldsException("No qualifier fields found for the 'Upsert' operation.");
            }

            var queryFields = new List<QueryField>();

            foreach (var field in qualifiers)
            {
                if (dictionary.ContainsKey(field.Name))
                {
                    queryFields.Add(new QueryField(field, dictionary[field.Name]));
                }
            }

            if (queryFields.Any() != true)
            {
                throw new MissingFieldsException("No qualifier fields defined for the 'Upsert' operation. Please check the items defined at the dictionary object.");
            }

            return new QueryGroup(queryFields);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="entities"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static IEnumerable<TResult> ExtractPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
            ClassProperty property)
            where TEntity : class =>
            ClassExpression.GetEntitiesPropertyValues<TEntity, TResult>(entities, property);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static IEnumerable<Field> GetQualifiedFields<TEntity>(TEntity entity)
            where TEntity : class
        {
            var typeOfEntity = entity?.GetType() ?? typeof(TEntity);
            return typeOfEntity.IsClassType() == false ? Field.Parse(entity) : FieldCache.Get(typeOfEntity);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static IEnumerable<Field> GetQualifiedFields<TEntity>(IEnumerable<Field> fields)
            where TEntity : class =>
            fields ?? (typeof(TEntity).IsDictionaryStringObject() == false ? FieldCache.Get<TEntity>() : null);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static IEnumerable<Field> GetQualifiedFields<TEntity>(IEnumerable<Field> fields,
            TEntity entity)
            where TEntity : class =>
            fields ?? GetQualifiedFields(entity);

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string ToRawSqlWithArrayParams(string commandText,
            string parameterName,
            IEnumerable<object> values,
            IDbSetting dbSetting)
        {
            // Check for the defined parameter
            if (commandText.IndexOf(parameterName) < 0)
            {
                return commandText;
            }

            // Return if there is no values
            if (values?.Any() != true)
            {
                return commandText;
            }

            // Get the variables needed
            var parameters = values.Select((_, index) =>
                string.Concat(parameterName, index).AsParameter(dbSetting));

            // Replace the target parameter
            return commandText.Replace(parameterName.AsParameter(dbSetting), parameters.Join(", "));
        }

        #region CreateDbCommandForExecution

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static DbCommand CreateDbCommandForExecution(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null,
            IEnumerable<DbField> dbFields = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Validate
            ValidateTransactionConnectionObject(connection, transaction);

            // Open
            connection.EnsureOpen();

            // Call
            return CreateDbCommandForExecutionInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static async Task<DbCommand> CreateDbCommandForExecutionAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default,
            Type entityType = null,
            IEnumerable<DbField> dbFields = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Validate
            ValidateTransactionConnectionObject(connection, transaction);

            // Open
            await connection.EnsureOpenAsync(cancellationToken);

            // Call
            return CreateDbCommandForExecutionInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: entityType,
                dbFields: dbFields,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <param name="dbFields"></param>
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        private static DbCommand CreateDbCommandForExecutionInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null,
            IEnumerable<DbField> dbFields = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Command
            var command = connection
                .CreateCommand(commandText, commandType, commandTimeout, transaction);

            // Func
            if (param != null)
            {
                var func = FunctionCache.GetPlainTypeToDbParametersCompiledFunction(param.GetType(), entityType, dbFields);
                if (func != null)
                {
                    var cmd = (DbCommand)command;
                    func(cmd, param);
                    return cmd;
                }
            }

            // ArrayParameters
            var commandArrayParametersText = (CommandArrayParametersText)null;
            if (param != null && skipCommandArrayParametersCheck == false)
            {
                commandArrayParametersText = GetCommandArrayParametersText(commandText,
                   param,
                   DbSettingMapper.Get(connection.GetType()));
            }

            // Check
            if (commandArrayParametersText != null)
            {
                // CommandText
                command.CommandText = commandArrayParametersText.CommandText;

                // Array parameters
                command.CreateParametersFromArray(commandArrayParametersText.CommandArrayParameters);
            }

            // Normal parameters
            if (param != null)
            {
                command.CreateParameters(param,
                    commandArrayParametersText?.CommandArrayParameters?.Select(cap => cap.ParameterName),
                    entityType,
                    dbFields);
            }

            // Return the command
            return (DbCommand)command;
        }

        #endregion

        #region GetCommandArrayParameters

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static CommandArrayParametersText GetCommandArrayParametersText(string commandText,
            object param,
            IDbSetting dbSetting)
        {
            if (param == null)
            {
                return null;
            }

            // ExpandoObject
            if (param is ExpandoObject || param is System.Collections.IDictionary)
            {
                if (param is IDictionary<string, object> objects)
                {
                    return GetCommandArrayParametersText(commandText, objects, dbSetting);
                }
            }

            // QueryField
            else if (param is QueryField field)
            {
                return GetCommandArrayParametersText(commandText, field, dbSetting);
            }

            // QueryFields
            else if (param is IEnumerable<QueryField> fields)
            {
                return GetCommandArrayParametersText(commandText, fields, dbSetting);
            }

            // QueryGroup
            else if (param is QueryGroup group)
            {
                return GetCommandArrayParametersText(commandText, group, dbSetting);
            }

            // Others
            else
            {
                return GetCommandArrayParametersTextInternal(commandText, param, dbSetting);
            }

            // Return
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static CommandArrayParametersText GetCommandArrayParametersTextInternal(string commandText,
            object param,
            IDbSetting dbSetting)
        {
            if (param == null)
            {
                return null;
            }

            // Variables
            var commandArrayParametersText = (CommandArrayParametersText)null;

            // CommandArrayParameters
            foreach (var property in param.GetType().GetProperties())
            {
                var propertyHandler = PropertyHandlerCache.Get<object>(property.DeclaringType, property);
                if (propertyHandler != null ||
                    property.PropertyType == StaticType.String ||
                    StaticType.IEnumerable.IsAssignableFrom(property.PropertyType) == false)
                {
                    continue;
                }

                // Get
                var commandArrayParameter = GetCommandArrayParameter(
                    property.Name,
                    property.GetValue(param));

                // Skip
                if (commandArrayParameter == null)
                {
                    continue;
                }

                // Create
                if (commandArrayParametersText == null)
                {
                    commandArrayParametersText = new CommandArrayParametersText();
                }

                // CommandText
                commandText = GetRawSqlText(commandText,
                    property.Name,
                    commandArrayParameter.Values,
                    dbSetting);

                // Add
                commandArrayParametersText.CommandArrayParameters.Add(commandArrayParameter);
            }

            // CommandText
            if (commandArrayParametersText != null)
            {
                commandArrayParametersText.CommandText = commandText;
            }

            // Return
            return commandArrayParametersText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="dictionary"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static CommandArrayParametersText GetCommandArrayParametersText(string commandText,
            IDictionary<string, object> dictionary,
            IDbSetting dbSetting)
        {
            if (dictionary == null)
            {
                return null;
            }

            // Variables
            var commandArrayParametersText = (CommandArrayParametersText)null;

            // CommandArrayParameters
            foreach (var kvp in dictionary)
            {
                // Get
                var commandArrayParameter = GetCommandArrayParameter(
                    kvp.Key,
                    kvp.Value);

                // Skip
                if (commandArrayParameter == null)
                {
                    continue;
                }

                // Create
                if (commandArrayParametersText == null)
                {
                    commandArrayParametersText = new CommandArrayParametersText();
                }

                // CommandText
                commandText = GetRawSqlText(commandText,
                    kvp.Key,
                    commandArrayParameter.Values,
                    dbSetting);

                // Add
                commandArrayParametersText.CommandArrayParameters.Add(commandArrayParameter);
            }

            // CommandText
            if (commandArrayParametersText != null)
            {
                commandArrayParametersText.CommandText = commandText;
            }

            // Return
            return commandArrayParametersText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="queryField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static CommandArrayParametersText GetCommandArrayParametersText(string commandText,
            QueryField queryField,
            IDbSetting dbSetting)
        {
            if (queryField == null)
            {
                return null;
            }

            // Skip
            if (IsPreConstructed(commandText, queryField))
            {
                return null;
            }

            // Get
            var commandArrayParameter = GetCommandArrayParameter(
                queryField.Field.Name,
                queryField.Parameter.Value);

            // Check
            if (commandArrayParameter == null)
            {
                return null;
            }

            // Create
            var commandArrayParametersText = new CommandArrayParametersText
            {
                CommandText = GetRawSqlText(commandText, queryField.Field.Name,
                    commandArrayParameter.Values, dbSetting)
            };

            // CommandArrayParameters
            commandArrayParametersText.CommandArrayParameters.Add(commandArrayParameter);

            // Return
            return commandArrayParametersText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="queryFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static CommandArrayParametersText GetCommandArrayParametersText(string commandText,
            IEnumerable<QueryField> queryFields,
            IDbSetting dbSetting)
        {
            if (queryFields == null)
            {
                return null;
            }

            // Variables
            var commandArrayParametersText = (CommandArrayParametersText)null;

            // CommandArrayParameters
            foreach (var queryField in queryFields)
            {
                // Skip
                if (IsPreConstructed(commandText, queryField))
                {
                    continue;
                }

                // Get
                var commandArrayParameter = GetCommandArrayParameter(
                    queryField.Field.Name,
                    queryField.Parameter.Value);

                // Skip
                if (commandArrayParameter == null)
                {
                    continue;
                }

                // Create
                if (commandArrayParametersText == null)
                {
                    commandArrayParametersText = new CommandArrayParametersText();
                }

                // CommandText
                commandText = GetRawSqlText(commandText,
                    queryField.Field.Name,
                    commandArrayParameter.Values,
                    dbSetting);

                // Add
                commandArrayParametersText.CommandArrayParameters.Add(commandArrayParameter);
            }

            // CommandText
            if (commandArrayParametersText != null)
            {
                commandArrayParametersText.CommandText = commandText;
            }

            // Return
            return commandArrayParametersText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="queryGroup"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static CommandArrayParametersText GetCommandArrayParametersText(string commandText,
            QueryGroup queryGroup,
            IDbSetting dbSetting) =>
            GetCommandArrayParametersText(commandText, queryGroup.GetFields(true), dbSetting);

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static CommandArrayParameter GetCommandArrayParameter(string parameterName,
            object value)
        {
            var valueType = value?.GetType();
            var propertyHandler = valueType != null ? PropertyHandlerCache.Get<object>(valueType) : null;
            if (value == null || propertyHandler != null || value is string || value is System.Collections.IEnumerable == false)
            {
                return null;
            }

            // Values
            var values = (System.Collections.IEnumerable)value;

            // Return
            return new CommandArrayParameter(parameterName, values.WithType<object>());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string GetRawSqlText(string commandText,
            string parameterName,
            System.Collections.IEnumerable values,
            IDbSetting dbSetting)
        {
            if (commandText.IndexOf(parameterName) < 0)
            {
                return commandText;
            }

            // Items
            var items = values is IEnumerable<object> objects ? objects : values.WithType<object>();
            if (items.Any() != true)
            {
                var parameter = parameterName.AsParameter(dbSetting);
                return commandText.Replace(parameter, string.Concat("(SELECT ", parameter, " WHERE 1 = 0)"));
            }

            // Get the variables needed
            var parameters = items.Select((_, index) =>
                string.Concat(parameterName, index).AsParameter(dbSetting));

            // Replace the target parameter
            return commandText.Replace(parameterName.AsParameter(dbSetting), parameters.Join(", "));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="queryField"></param>
        /// <returns></returns>
        private static bool IsPreConstructed(string commandText,
            QueryField queryField)
        {
            // Check the IN operation parameters
            if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
            {
                if (commandText.IndexOf(string.Concat(queryField.Parameter.Name, "_In_")) > 0)
                {
                    return true;
                }
            }

            // Check the BETWEEN operation parameters
            else if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
            {
                if (commandText.IndexOf(string.Concat(queryField.Parameter.Name, "_Left")) > 0)
                {
                    return true;
                }
            }

            // Return
            return false;
        }

        #endregion

        #endregion
    }
}
