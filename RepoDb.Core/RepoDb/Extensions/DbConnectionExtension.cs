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

        #region ExecuteQuery(Dynamics)

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static IEnumerable<dynamic> ExecuteQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static IEnumerable<dynamic> ExecuteQueryInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            string tableName,
            bool skipCommandArrayParametersCheck)
        {
            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = command.ExecuteReader())
                {
                    return DataReader.ToEnumerable(reader, tableName, connection, transaction).AsList();
                }
            }
        }

        #endregion

        #region ExecuteQueryAsync(Dynamics)

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static Task<IEnumerable<object>> ExecuteQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteQueryAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                tableName: null,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<object>> ExecuteQueryAsyncInternal(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            string tableName,
            bool skipCommandArrayParametersCheck)
        {
            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await DataReader.ToEnumerableAsync(reader, tableName, connection, transaction);
                }
            }
        }

        #endregion

        #region ExecuteQuery<TEntity>

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return ExecuteQueryInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static IEnumerable<TEntity> ExecuteQueryInternal<TEntity>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
            where TEntity : class
        {
            // Trigger the cache to void reusing the connection
            DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = command.ExecuteReader())
                {
                    return DataReader.ToEnumerable<TEntity>(reader, connection, transaction).AsList();
                }
            }
        }

        #endregion

        #region ExecuteQueryAsync<TEntity>

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">
        /// The parameters/values defined in the <see cref="IDbCommand.CommandText"/> property. Supports a dynamic object, <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="ExpandoObject"/>, <see cref="QueryField"/>, <see cref="QueryGroup"/> and an enumerable of <see cref="QueryField"/> objects.
        /// </param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        public static Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            return ExecuteQueryAsyncInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: false);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
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
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<TEntity>> ExecuteQueryAsyncInternal<TEntity>(this IDbConnection connection,
            string commandText,
            object param,
            CommandType? commandType,
            int? commandTimeout,
            IDbTransaction transaction,
            bool skipCommandArrayParametersCheck)
            where TEntity : class
        {
            // Trigger the cache to void reusing the connection
            await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<TEntity>(), transaction);

            // Execute the actual method
            using (var command = CreateDbCommandForExecution(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await DataReader.ToEnumerableAsync<TEntity>(reader, connection, transaction);
                }
            }
        }

        #endregion

        #region ExecuteQueryMultiple(Results)

        /// <summary>
        /// Executes a multiple query statement from the database.
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
            // As the connection string is being modified by ADO.Net if the (Integrated Security=False), right after opening the connection unless (Persist Security Info=True)
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
        /// Executes a multiple query statement from the database in an asynchronous way.
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
            // As the connection string is being modified by ADO.Net if the (Integrated Security=False), right after opening the connection unless (Persist Security Info=True)
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
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
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
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
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return command.ExecuteNonQuery();
            }
        }

        #endregion

        #region ExecuteNonQueryAsync

        /// <summary>
        /// Executes a query from the database in asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
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
        /// Executes a query from the database in asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected data during the execution.
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.DbNullToNull(command.ExecuteScalar());
            }
        }

        #endregion

        #region ExecuteScalarAsync

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.DbNullToNull(await command.ExecuteScalarAsync());
            }
        }

        #endregion

        #region ExecuteScalar<TResult>

        /// <summary>
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
        /// Executes a query from the database. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.ToType<TResult>(command.ExecuteScalar());
            }
        }

        #endregion

        #region ExecuteScalarAsync<TResult>

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
        /// Executes a query from the database in an asynchronous way. It uses the underlying method of <see cref="IDbCommand.ExecuteScalar"/> and
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
                skipCommandArrayParametersCheck: skipCommandArrayParametersCheck))
            {
                return Converter.ToType<TResult>(await command.ExecuteScalarAsync());
            }
        }

        #endregion

        #region Mapped Operations

        /// <summary>
        /// Gets the associated <see cref="IDbSetting"/> object that is currently mapped for the target <see cref="IDbConnection"/> object.
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
                throw new MissingMappingException($"There is no database setting mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorials/installation).");
            }

            // Return the validator
            return setting;
        }

        /// <summary>
        /// Gets the associated <see cref="IDbHelper"/> object that is currently mapped for the target <see cref="IDbConnection"/> object.
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
                throw new MissingMappingException($"There is no database helper mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorials/installation).");
            }

            // Return the validator
            return helper;
        }

        /// <summary>
        /// Gets the associated <see cref="IStatementBuilder"/> object that is currently mapped for the target <see cref="IDbConnection"/> object.
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
                throw new MissingMappingException($"There is no database statement builder mapping found for '{connection.GetType().FullName}'. Make sure to install the correct extension library and call the bootstrapper method. You can also visit the library's installation page (http://repodb.net/tutorials/installation).");
            }

            // Return the validator
            return statementBuilder;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a <see cref="QueryGroup"/> object based on the given qualifiers.
        /// </summary>
        /// <param name="entity">The data entity or dynamic object to be merged.</param>
        /// <param name="properties">The list of properties for the entity object.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup CreateQueryGroupForUpsert(object entity,
            IEnumerable<ClassProperty> properties,
            IEnumerable<Field> qualifiers = null)
        {
            // Variables needed
            var queryFields = (IList<QueryField>)null;

            // Iterate the fields
            foreach (var field in qualifiers)
            {
                // Get the property
                var property = properties?.FirstOrDefault(
                    p => string.Equals(p.GetMappedName(), field.Name, StringComparison.OrdinalIgnoreCase));

                // Get the value
                if (property != null)
                {
                    // Create the list if necessary
                    if (queryFields == null)
                    {
                        queryFields = new List<QueryField>();
                    }

                    // Add the fields
                    queryFields.Add(new QueryField(field, property.PropertyInfo.GetValue(entity)));
                }
            }

            // Return the value
            return new QueryGroup(queryFields);
        }

        /// <summary>
        /// Throws an exception if there is no defined primary key on the data entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The primary <see cref="ClassProperty"/> of the type.</returns>
        private static ClassProperty GetAndGuardPrimaryKey<TEntity>(IDbConnection connection,
            IDbTransaction transaction)
            where TEntity : class
        {
            var property = PrimaryCache.Get<TEntity>();
            if (property == null)
            {
                var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
                if (primary != null)
                {
                    var properties = PropertyCache.Get<TEntity>();
                    property = properties.FirstOrDefault(p =>
                        string.Equals(p.GetMappedName(), primary.Name, StringComparison.OrdinalIgnoreCase));
                }
            }
            if (property == null)
            {
                throw new PrimaryFieldNotFoundException($"No primary key found at type '{typeof(TEntity).FullName}'.");
            }
            return property;
        }

        /// <summary>
        /// Throws an exception if there is no defined primary key on the data entity type.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The primary <see cref="DbField"/> of the table.</returns>
        private static DbField GetAndGuardPrimaryKey(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
            if (primary == null)
            {
                throw new PrimaryFieldNotFoundException($"No primary key found at table '{tableName}'.");
            }
            return primary;
        }

        /// <summary>
        /// Extract the property value from the instances
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The list of data entity objects to be extracted.</param>
        /// <param name="property">The class property to be used.</param>
        /// <returns>An array of the results based on the target types.</returns>
        private static IEnumerable<object> ExtractPropertyValues<TEntity>(IEnumerable<TEntity> entities,
            ClassProperty property)
            where TEntity : class
        {
            return ClassExpression.GetEntitiesPropertyValues<TEntity, object>(entities, property);
        }

        /// <summary>
        /// Validates whether the transaction object connection is object is equals to the connection object.
        /// </summary>
        /// <param name="connection">The connection object to be validated.</param>
        /// <param name="transaction">The transaction object to compare.</param>
        internal static void ValidateTransactionConnectionObject(this IDbConnection connection,
            IDbTransaction transaction)
        {
            if (transaction != null && transaction.Connection != connection)
            {
                throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
            }
        }

        /// <summary>
        /// Converts the object expression into a <see cref="QueryGroup"/> object with the PrimaryKey value.
        /// </summary>
        /// <param name="instance">The object expression instance.</param>
        /// <param name="defaultPrimaryKey">The default name of primary key to be used.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object with the PrimaryKey value.</returns>
        private static QueryGroup DataEntityToPrimaryKeyQueryGroup(object instance,
            string defaultPrimaryKey)
        {
            if (instance == null)
            {
                return null;
            }

            // Variables
            var type = instance?.GetType();
            var properties = (IEnumerable<ClassProperty>)null;

            // Identify
            if (type.IsGenericType)
            {
                properties = type.GetClassProperties();
            }
            else
            {
                properties = PropertyCache.Get(type);
            }

            // Get all the PrimaryKey(s)
            var primaryProperties = properties.Where(p => p.IsPrimary() == true);

            // Get the PrimaryKey via IsPrimary
            var property = primaryProperties.FirstOrDefault(p => p.IsPrimary() == true);

            // Check if there is forced [Primary] attribute
            if (property == null)
            {
                property = primaryProperties.FirstOrDefault(p => p.GetPrimaryAttribute() != null);
            }

            // If it still null, get the first one
            if (property == null)
            {
                property = primaryProperties?.FirstOrDefault();
            }

            // Otherwise, check the default one
            if (property == null && !string.IsNullOrEmpty(defaultPrimaryKey))
            {
                property = properties.FirstOrDefault(p =>
                    string.Equals(p.GetMappedName(), defaultPrimaryKey, StringComparison.OrdinalIgnoreCase));
            }

            // Return the instance
            if (property != null)
            {
                return new QueryGroup(new QueryField(property.AsField(), property.PropertyInfo.GetValue(instance)));
            }
            else
            {
                throw new PrimaryFieldNotFoundException("The primary field is not found.");
            }
        }

        /// <summary>
        /// Converts the dynamic expression into a <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the actual value of the primary key.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup WhereOrPrimaryKeyToQueryGroup(IDbConnection connection,
            string tableName,
            object whereOrPrimaryKey,
            IDbTransaction transaction)
        {
            if (whereOrPrimaryKey == null)
            {
                return null;
            }
            if (whereOrPrimaryKey.GetType().IsGenericType)
            {
                return QueryGroup.Parse(whereOrPrimaryKey);
            }
            else
            {
                var primary = DbFieldCache.Get(connection, tableName, transaction)?.FirstOrDefault(p => p.IsPrimary == true);
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException(string.Format("There is no primary key field found for table '{0}'.", tableName));
                }
                else
                {
                    return new QueryGroup(new QueryField(primary.AsField(), whereOrPrimaryKey));
                }
            }
        }

        /// <summary>
        /// Converts the dynamic expression into a <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression or the actual value of the primary key.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup WhereOrPrimaryKeyToQueryGroup<TEntity>(IDbConnection connection,
            object whereOrPrimaryKey,
            IDbTransaction transaction)
            where TEntity : class
        {
            if (whereOrPrimaryKey == null)
            {
                return null;
            }
            if (whereOrPrimaryKey.GetType().IsGenericType)
            {
                return QueryGroup.Parse(whereOrPrimaryKey);
            }
            else
            {
                var field = PrimaryCache.Get<TEntity>()?.AsField();
                if (field == null)
                {
                    field = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction)?
                        .FirstOrDefault(p => p.IsPrimary == true)?
                        .AsField();
                }
                if (field == null)
                {
                    throw new PrimaryFieldNotFoundException(string.Format("There is no primary key field found for table '{0}'.", ClassMappedNameCache.Get<TEntity>()));
                }
                else
                {
                    return new QueryGroup(new QueryField(field, whereOrPrimaryKey));
                }
            }
        }

        /// <summary>
        /// Converts an object into a <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="where">The dynamic expression.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup ToQueryGroup(object where)
        {
            if (where == null)
            {
                return null;
            }
            if (where.GetType().IsGenericType)
            {
                return QueryGroup.Parse(where);
            }
            else
            {
                throw new Exceptions.InvalidExpressionException("Only dynamic object is supported in the 'where' expression.");
            }
        }

        /// <summary>
        /// Converts the primary key to <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="where">The query expression.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup ToQueryGroup<TEntity>(Expression<Func<TEntity, bool>> where)
            where TEntity : class
        {
            if (where == null)
            {
                return null;
            }
            return QueryGroup.Parse<TEntity>(where);
        }

        /// <summary>
        /// Converts the primary key to <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="property">The instance of <see cref="ClassProperty"/> to be converted.</param>
        /// <param name="entity">The instance of the actual entity.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup ToQueryGroup<TEntity>(ClassProperty property,
            TEntity entity)
            where TEntity : class
        {
            if (property == null)
            {
                return null;
            }
            return new QueryGroup(property.PropertyInfo.AsQueryField(entity));
        }

        /// <summary>
        /// Converts the <see cref="QueryField"/> to become a <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="field">The instance of <see cref="QueryField"/> to be converted.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup ToQueryGroup(QueryField field)
        {
            if (field == null)
            {
                return null;
            }
            return new QueryGroup(field);
        }

        /// <summary>
        /// Converts the <see cref="QueryField"/> to become a <see cref="QueryGroup"/> object.
        /// </summary>
        /// <param name="fields">The list of <see cref="QueryField"/> objects to be converted.</param>
        /// <returns>An instance of <see cref="QueryGroup"/> object.</returns>
        private static QueryGroup ToQueryGroup(IEnumerable<QueryField> fields)
        {
            if (fields == null)
            {
                return null;
            }
            return new QueryGroup(fields);
        }

        /// <summary>
        /// Create a new instance of <see cref="DbCommand"/> object to be used for execution.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">The list of parameters.</param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout to be used.</param>
        /// <param name="transaction">The transaction object to be used.</param>
        /// <param name="skipCommandArrayParametersCheck">True to skip the checking of the array parameters.</param>
        /// <returns>An instance of <see cref="DbCommand"/> object.</returns>
        private static DbCommand CreateDbCommandForExecution(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
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
            command.CreateParameters(param, commandArrayParameters?.Select(cap => cap.ParameterName));

            // Identify target statement, for now, only support array with single parameters
            if (commandArrayParameters != null)
            {
                command.CreateParametersFromArray(commandArrayParameters);
            }

            // Return the command
            return (DbCommand)command;
        }

        /// <summary>
        /// Converts the command text into a raw SQL with array parameters.
        /// </summary>
        /// <param name="commandText">The current command text where the raw sql parameters will be replaced.</param>
        /// <param name="parameterName">The name of the parameter to be replaced.</param>
        /// <param name="values">The array of the values.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The raw SQL with array parameters.</returns>
        private static string ToRawSqlWithArrayParams(string commandText,
            string parameterName,
            IEnumerable<object> values,
            IDbSetting dbSetting)
        {
            // Check for the defined parameter
            if (commandText.IndexOf(parameterName) < 0)
            {
                return commandText;
            }

            // Get the variables needed
            var length = values != null ? values.Count() : 0;
            var parameters = new string[length];

            // Iterate and set the parameter values
            for (var i = 0; i < length; i++)
            {
                parameters[i] = string.Concat(parameterName, i).AsParameter(dbSetting);
            }

            // Replace the target parameter
            return commandText.Replace(parameterName.AsParameter(dbSetting), parameters.Join(", "));
        }

        #region AsCommandArrayParameters

        /// <summary>
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="param">The parameter passed.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A list of <see cref="CommandArrayParameter"/> objects.</returns>
        private static IList<CommandArrayParameter> AsCommandArrayParameters(object param,
            IDbSetting dbSetting,
            ref string commandText)
        {
            if (param == null)
            {
                return null;
            }

            // Declare return values
            var commandArrayParameters = (IList<CommandArrayParameter>)null;

            // Return if any of this
            if (param is ExpandoObject || param is IDictionary<string, object>)
            {
                return AsCommandArrayParameters((IDictionary<string, object>)param, dbSetting, ref commandText);
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
                    // Skip if it is not an array
                    if (property.PropertyType.IsArray == false)
                    {
                        continue;
                    }

                    // Skip if it is an array
                    if (property.DeclaringType.IsGenericType == false && property.PropertyType == typeof(byte[]))
                    {
                        continue;
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
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="dictionary">The parameters from the <see cref="Dictionary{TKey, TValue}"/> object.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A list of <see cref="CommandArrayParameter"/> objects.</returns>
        private static IList<CommandArrayParameter> AsCommandArrayParameters(IDictionary<string, object> dictionary,
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

                // Skip if it is not an array
                if (type?.IsArray == false)
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
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="queryGroup">The value of the <see cref="QueryGroup"/> object.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A list of <see cref="CommandArrayParameter"/> objects.</returns>
        private static IList<CommandArrayParameter> AsCommandArrayParameters(QueryGroup queryGroup,
            IDbSetting dbSetting,
            ref string commandText)
        {
            return AsCommandArrayParameters(queryGroup.GetFields(true), dbSetting, ref commandText);
        }

        /// <summary>
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>A list of <see cref="CommandArrayParameter"/> objects.</returns>
        private static IList<CommandArrayParameter> AsCommandArrayParameters(IEnumerable<QueryField> queryFields,
            IDbSetting dbSetting,
            ref string commandText)
        {
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

                // Skip if it is not an array
                if (type.IsArray == false)
                {
                    continue;
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
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="queryField">The value of <see cref="QueryField"/> object.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <returns>A list of <see cref="CommandArrayParameter"/> objects.</returns>
        private static IList<CommandArrayParameter> AsCommandArrayParameters(QueryField queryField,
            IDbSetting dbSetting,
            ref string commandText)
        {
            if (queryField == null)
            {
                return null;
            }

            // Get type of the value
            var type = queryField.Parameter.Value?.GetType();

            // Skip if it is not an array
            if (type.IsArray == false)
            {
                return null;
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
        /// Replaces the array parameter command texts and return the list of <see cref="CommandArrayParameter"/> objects.
        /// </summary>
        /// <param name="name">The target name of the <see cref="CommandArrayParameter"/> object.</param>
        /// <param name="values">The array value of the <see cref="CommandArrayParameter"/> object.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <param name="commandText">The command text to be replaced.</param>
        /// <returns>An instance of <see cref="CommandArrayParameter"/> object.</returns>
        private static CommandArrayParameter AsCommandArrayParameter(string name,
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
