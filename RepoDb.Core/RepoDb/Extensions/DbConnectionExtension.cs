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
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
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
        /// <returns>The instance of the current connection object.</returns>
        public static async Task<IDbConnection> EnsureOpenAsync(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                await ((DbConnection)connection).OpenAsync();
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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
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

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = command.ExecuteReader())
                {
                    var result = DataReader.ToEnumerable(reader,
                        tableName,
                        connection,
                        transaction).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<dynamic>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }

                    // Return
                    return result;
                }
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
            ICache cache = null)
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
                tableName: null,
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<dynamic>> ExecuteQueryAsyncInternal(this IDbConnection connection,
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

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var result = (await DataReader.ToEnumerableAsync(reader,
                        tableName,
                        connection,
                        transaction)).AsList();

                    // Set Cache
                    if (cacheKey != null)
                    {
                        cache?.Add(cacheKey, (IEnumerable<dynamic>)result, cacheItemExpiration.GetValueOrDefault(), false);
                    }

                    // Return
                    return result;
                }
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
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static IEnumerable<TResult> ExecuteQueryInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
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
                var result = (IEnumerable<TResult>)ExecuteQueryInternal(connection: connection,
                   commandText: commandText,
                   param: param,
                   commandType: commandType,
                   cacheKey: null,
                   cacheItemExpiration: null,
                   transaction: transaction,
                   cache: null,
                   tableName: null,
                   skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);

                // Set Cache
                if (cacheKey != null)
                {
                    cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                }

                // Return
                return result;
            }
            else
            {
                // Variables needed on this operation
                var connectionString = connection.ConnectionString;

                // Trigger the cache to avoid reusing the connection
                if (connection.State == ConnectionState.Open || transaction != null)
                {
                    connectionString = null;
                    DbFieldCache.Get(connection, ClassMappedNameCache.Get<TResult>(), transaction, false);
                }

                // Execute the actual method
                using (var command = CreateDbCommandForExecution(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    entityType: typeOfResult,
                    skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var result = (IEnumerable<TResult>)DataReader.ToEnumerableInternal<TResult>(reader,
                            connection,
                            connectionString,
                            transaction,
                            false).AsList();

                        // Set Cache
                        if (cacheKey != null)
                        {
                            cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                        }

                        // Return
                        return result;
                    }
                }
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
            ICache cache = null)
        {
            return ExecuteQueryAsyncInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of the target result type instances containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<TResult>> ExecuteQueryAsyncInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            string cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
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
                var result = (IEnumerable<TResult>)await ExecuteQueryAsyncInternal(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    cacheKey: null,
                    cacheItemExpiration: null,
                    transaction: transaction,
                    cache: null,
                    tableName: null,
                    skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);

                // Set Cache
                if (cacheKey != null)
                {
                    cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                }

                // Return
                return result;
            }
            else
            {
                // Variables needed on this operation
                var connectionString = connection.ConnectionString;

                // Trigger the cache to avoid reusing the connection
                if (connection.State == ConnectionState.Open || transaction != null)
                {
                    connectionString = null;
                    await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<TResult>(), transaction, false);
                }

                // Execute the actual method
                using (var command = CreateDbCommandForExecution(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    entityType: typeOfResult,
                    skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = (IEnumerable<TResult>)(await DataReader.ToEnumerableInternalAsync<TResult>(reader,
                            connection,
                            connectionString,
                            transaction,
                            false)).AsList();

                        // Set Cache
                        if (cacheKey != null)
                        {
                            cache?.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
                        }

                        // Return
                        return result;
                    }
                }
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
            IDbTransaction transaction = null)
        {
            // Variables needed on this operation
            var connectionString = connection.ConnectionString;

            // Read the result
            var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);

            // Create an extractor class
            return new QueryMultipleExtractor((DbDataReader)reader, connection, transaction, connectionString);
        }

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
        /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
        public static async Task<QueryMultipleExtractor> ExecuteQueryMultipleAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            // Variables needed on this operation
            var connectionString = connection.ConnectionString;

            // Read the result
            var reader = await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);

            // Create an extractor class
            return new QueryMultipleExtractor((DbDataReader)reader, connection, transaction, connectionString);
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
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns><returns>The instance of the <see cref="IDataReader"/> object.</returns></returns>
        internal static IDataReader ExecuteReaderInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
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
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            var hasError = false;

            // Ensure the DbCommand disposal
            try
            {
                return command.ExecuteReader();
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
        /// <returns><returns>The instance of the <see cref="IDataReader"/> object.</returns></returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns><returns>The instance of the <see cref="IDataReader"/> object.</returns></returns>
        internal static async Task<IDataReader> ExecuteReaderAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            // Variables
            var setting = connection.GetDbSetting();
            var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck);
            var hasError = false;

            // Ensure the DbCommand disposal
            try
            {
                return await command.ExecuteReaderAsync();
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
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int ExecuteNonQueryInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return command.ExecuteNonQuery();
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> ExecuteNonQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteNonQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> ExecuteNonQueryAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public static object ExecuteScalar(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteScalarInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static object ExecuteScalarInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.DbNullToNull(command.ExecuteScalar());
            }
        }

        #endregion

        #region ExecuteScalarAsync

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteScalarAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static async Task<object> ExecuteScalarAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.DbNullToNull(await command.ExecuteScalarAsync());
            }
        }

        #endregion

        #region ExecuteScalar<TResult>

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        public static TResult ExecuteScalar<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteScalarInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a SQL statement from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        internal static TResult ExecuteScalarInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.ToType<TResult>(command.ExecuteScalar());
            }
        }

        #endregion

        #region ExecuteScalarAsync<TResult>

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <typeparam name="TResult">The target return type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        public static Task<TResult> ExecuteScalarAsync<TResult>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteScalarAsyncInternal<TResult>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a SQL statement from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
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
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>A first occurence value (first column of first row) of the execution.</returns>
        internal static async Task<TResult> ExecuteScalarAsyncInternal<TResult>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
        {
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.ToType<TResult>(await command.ExecuteScalarAsync());
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

        #region GetAndGuardPrimaryKeyOrIdentityKey

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static ClassProperty GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(IDbConnection connection,
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
        internal static ClassProperty GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
            where TEntity : class
        {
            var property = PrimaryCache.Get<TEntity>() ?? IdentityCache.Get<TEntity>();
            if (property == null)
            {
                var dbFields = DbFieldCache.Get(connection, tableName, transaction);
                property = GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(dbFields);
            }
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static Task<ClassProperty> GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(IDbConnection connection,
            IDbTransaction transaction)
            where TEntity : class =>
            GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        internal static async Task<ClassProperty> GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
            where TEntity : class
        {
            var property = PrimaryCache.Get<TEntity>() ?? IdentityCache.Get<TEntity>();
            if (property == null)
            {
                var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
                property = GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(dbFields);
            }
            return GetAndGuardPrimaryKeyOrIdentityKey(tableName, property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static ClassProperty GetAndGuardPrimaryKeyOrIdentityKey(string tableName,
            ClassProperty property)
        {
            if (property == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the table '{tableName}'.");
            }
            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        internal static ClassProperty GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(IEnumerable<DbField> dbFields)
            where TEntity : class
        {
            var dbField = dbFields?.FirstOrDefault(df => df.IsPrimary == true) ??
                dbFields?.FirstOrDefault(df => df.IsIdentity == true);
            if (dbField != null)
            {
                var properties = PropertyCache.Get<TEntity>() ?? typeof(TEntity).GetClassProperties();
                return properties.FirstOrDefault(p =>
                    string.Equals(p.GetMappedName(), dbField.Name, StringComparison.OrdinalIgnoreCase));
            }
            throw new KeyFieldNotFoundException($"No primary key and identify found at type '{typeof(TEntity).Name}'.");
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
        /// <returns></returns>
        internal static async Task<DbField> GetAndGuardPrimaryKeyOrIdentityKeyAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
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
                var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction);
                queryGroup = WhatToQueryGroup<T>(key, what);
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
        /// <returns></returns>
        internal static async Task<QueryGroup> WhatToQueryGroupAsync<T>(this IDbConnection connection,
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
                var dbField = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction);
                queryGroup = WhatToQueryGroup<T>(dbField, what);
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
        /// <returns></returns>
        internal static async Task<QueryGroup> WhatToQueryGroupAsync<TEntity>(IDbConnection connection,
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
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
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
                return WhatToQueryGroup<T>(property, what);
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
        /// <param name="property"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        internal static QueryGroup WhatToQueryGroup<T>(ClassProperty property,
            T what)
        {
            var type = typeof(T);
            if (property == null)
            {
                throw new KeyFieldNotFoundException($"No primary key and identity key found at the type '{type.FullName}'.");
            }
            if (type.IsClassType())
            {
                return new QueryGroup(property.PropertyInfo.AsQueryField(what));
            }
            else
            {
                return new QueryGroup(new QueryField(property.AsField(), what));
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
            else if (what is QueryField)
            {
                return ToQueryGroup(what as QueryField);
            }
            else if (what is IEnumerable<QueryField>)
            {
                return ToQueryGroup(what as IEnumerable<QueryField>);
            }
            else if (what is QueryGroup)
            {
                return what as QueryGroup;
            }
            else
            {
                var type = typeof(T);
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
        /// <param name="where"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(object where)
        {
            if (where == null)
            {
                return null;
            }
            var type = where.GetType();
            if (type.IsClassType())
            {
                return QueryGroup.Parse(where, true);
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
                var type = entity?.GetType();
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
        /// <param name="field"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(QueryField field)
        {
            if (field == null)
            {
                return null;
            }
            return new QueryGroup(field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static QueryGroup ToQueryGroup(IEnumerable<QueryField> fields)
        {
            if (fields == null)
            {
                return null;
            }
            return new QueryGroup(fields);
        }

        #endregion

        /// <summary>
        /// Throws an exception if the entities argument is null or empty.
        /// </summary>
        /// <typeparam name="TEntity">The type of the result.</typeparam>
        /// <param name="entities">The enumerable list of entity objects.</param>
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
        internal static void WhereToCommandParameters(DbCommand command,
            QueryGroup where)
        {
            // Check the presence
            if (where == null)
            {
                return;
            }

            // Iterate the fields
            foreach (var queryField in where.GetFields(true))
            {
                // Create a parameter
                var parameter = command
                    .CreateParameter(queryField.Parameter.Name, queryField.Parameter.Value, null);

                // Add to the command object
                command.Parameters.Add(parameter);
            }
        }

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
            var typeOfEntity = typeof(TEntity);
            if (typeOfEntity.IsDictionaryStringObject())
            {
                return ((IDictionary<string, object>)entity)
                    .Keys
                    .Select(key => new Field(key));
            }
            return typeOfEntity.IsClassType() == false ? Field.Parse(entity) : FieldCache.Get<TEntity>();
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
            fields ?? (typeof(TEntity).IsClassType() == false ? Field.Parse(entity) : FieldCache.Get<TEntity>());

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
        /// <param name="skipCommandArrayParametersCheck"></param>
        /// <returns></returns>
        internal static DbCommand CreateDbCommandForExecution(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null,
            bool skipCommandArrayParametersCheck = true)
        {
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Process the array parameters
            var commandArrayParameters = (IEnumerable<CommandArrayParameter>)null;

            // Check the array parameters
            if (skipCommandArrayParametersCheck == false)
            {
                commandArrayParameters = AsCommandArrayParameters(param, DbSettingMapper.Get(connection.GetType()), ref commandText);
            }

            // Command object initialization
            var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction);

            // Add the parameters
            command.CreateParameters(param, commandArrayParameters?.Select(cap => cap.ParameterName), entityType);

            // Identify target statement, for now, only support array with single parameters
            if (commandArrayParameters != null)
            {
                command.CreateParametersFromArray(commandArrayParameters);
            }

            // Return the command
            return (DbCommand)command;
        }

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
            var parameters = values.Select((value, index) =>
                string.Concat(parameterName, index).AsParameter(dbSetting));

            // Replace the target parameter
            return commandText.Replace(parameterName.AsParameter(dbSetting), parameters.Join(", "));
        }

        #region AsCommandArrayParameters

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static IList<CommandArrayParameter> AsCommandArrayParameters(object param,
            IDbSetting dbSetting,
            ref string commandText)
        {
            // TODO: Refactor this

            if (param == null)
            {
                return null;
            }

            // Declare return values
            var commandArrayParameters = (IList<CommandArrayParameter>)null;

            // Return if any of this
            if (param is ExpandoObject || param is System.Collections.IDictionary)
            {
                if (param is IDictionary<string, object>)
                {
                    return AsCommandArrayParameters((IDictionary<string, object>)param, dbSetting, ref commandText);
                }
            }
            else if (param is QueryField)
            {
                return AsCommandArrayParameters((QueryField)param, dbSetting, ref commandText);
            }
            else if (param is IEnumerable<QueryField>)
            {
                return AsCommandArrayParameters((IEnumerable<QueryField>)param, dbSetting, ref commandText);
            }
            else if (param is QueryGroup)
            {
                return AsCommandArrayParameters((QueryGroup)param, dbSetting, ref commandText);
            }
            else
            {
                // Iterate the properties
                foreach (var property in param.GetType().GetProperties())
                {
                    if (property.PropertyType.IsArray == false)
                    {
                        // String is an enumerable
                        if (property.PropertyType == StaticType.String)
                        {
                            continue;
                        }

                        // Get the value
                        var value = property.GetValue(param);

                        // Skip if it is not an enumerable
                        if ((value is System.Collections.IEnumerable) == false)
                        {
                            continue;
                        }
                    }

                    // Initialize the array if it not yet initialized
                    if (commandArrayParameters == null)
                    {
                        commandArrayParameters = new List<CommandArrayParameter>();
                    }

                    // Replace the target parameters
                    var items = ((System.Collections.IEnumerable)property.GetValue(param))
                        .OfType<object>();
                    var parameter = AsCommandArrayParameter(property.Name, items,
                        dbSetting,
                        ref commandText);
                    commandArrayParameters.Add(parameter);
                }
            }

            // Return the values
            return commandArrayParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static IList<CommandArrayParameter> AsCommandArrayParameters(IDictionary<string, object> dictionary,
            IDbSetting dbSetting,
            ref string commandText)
        {
            if (dictionary == null)
            {
                return null;
            }

            // Declare return values
            var commandArrayParameters = (IList<CommandArrayParameter>)null;

            // Iterate the properties
            foreach (var kvp in dictionary)
            {
                // Get type of the value
                var type = kvp.Value?.GetType();

                // String is an enumerable
                if (type == StaticType.String)
                {
                    continue;
                }

                // Skip if it is not an enumerable
                if ((kvp.Value is System.Collections.IEnumerable) == false)
                {
                    continue;
                }

                // Initialize the array if it not yet initialized
                if (commandArrayParameters == null)
                {
                    commandArrayParameters = new List<CommandArrayParameter>();
                }

                // Replace the target parameters
                var items = ((System.Collections.IEnumerable)kvp.Value)
                    .OfType<object>();
                var parameter = AsCommandArrayParameter(kvp.Key,
                    items,
                    dbSetting,
                    ref commandText);
                commandArrayParameters.Add(parameter);
            }

            // Return the values
            return commandArrayParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryGroup"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static IList<CommandArrayParameter> AsCommandArrayParameters(QueryGroup queryGroup,
            IDbSetting dbSetting,
            ref string commandText) =>
            AsCommandArrayParameters(queryGroup.GetFields(true), dbSetting, ref commandText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryFields"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static IList<CommandArrayParameter> AsCommandArrayParameters(IEnumerable<QueryField> queryFields,
            IDbSetting dbSetting,
            ref string commandText)
        {
            // TODO: Refactor this

            if (queryFields == null)
            {
                return null;
            }

            // Declare return values
            var commandArrayParameters = (IList<CommandArrayParameter>)null;

            // Iterate the properties
            foreach (var field in queryFields)
            {
                // Get type of the value
                var type = field.Parameter.Value?.GetType();

                // String is an enumerable
                if (type == StaticType.String)
                {
                    continue;
                }

                // Skip if it is not an enumerable
                if ((field.Parameter.Value is System.Collections.IEnumerable) == false)
                {
                    continue;
                }

                // Check the IN operation parameters
                if (field.Operation == Operation.In || field.Operation == Operation.NotIn)
                {
                    if (commandText.IndexOf(string.Concat(field.Parameter.Name, "_In_")) > 0)
                    {
                        continue;
                    }
                }

                // Check the BETWEEN operation parameters
                else if (field.Operation == Operation.Between || field.Operation == Operation.NotBetween)
                {
                    if (commandText.IndexOf(string.Concat(field.Parameter.Name, "_Left")) > 0)
                    {
                        continue;
                    }
                }

                // Initialize the array if it not yet initialized
                if (commandArrayParameters == null)
                {
                    commandArrayParameters = new List<CommandArrayParameter>();
                }

                // Replace the target parameters
                var items = ((System.Collections.IEnumerable)field.Parameter.Value)
                    .OfType<object>();
                var parameter = AsCommandArrayParameter(field.Field.Name,
                    items,
                    dbSetting,
                    ref commandText);
                commandArrayParameters.Add(parameter);
            }

            // Return the values
            return commandArrayParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static IList<CommandArrayParameter> AsCommandArrayParameters(QueryField queryField,
            IDbSetting dbSetting,
            ref string commandText)
        {
            if (queryField == null)
            {
                return null;
            }

            // Get type of the value
            var type = queryField.Parameter.Value?.GetType();

            // String is an enumerable
            if (type == StaticType.String)
            {
                return default;
            }

            // Skip if it is not an enumerable
            if ((queryField.Parameter.Value is System.Collections.IEnumerable) == false)
            {
                return default;
            }

            // Initialize the array if it not yet initialized
            var commandArrayParameters = new List<CommandArrayParameter>();

            // Replace the target parameters
            var items = ((System.Collections.IEnumerable)queryField.Parameter.Value)
                .OfType<object>();
            var parameter = AsCommandArrayParameter(queryField.Field.Name,
                items,
                dbSetting,
                ref commandText);
            commandArrayParameters.Add(parameter);

            // Return the values
            return commandArrayParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="dbSetting"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static CommandArrayParameter AsCommandArrayParameter(string name,
            IEnumerable<object> values,
            IDbSetting dbSetting,
            ref string commandText)
        {
            // Convert to raw sql
            commandText = ToRawSqlWithArrayParams(commandText, name, values, dbSetting);

            // Add to the list
            return new CommandArrayParameter(name, values);
        }

        #endregion

        #endregion
    }
}
