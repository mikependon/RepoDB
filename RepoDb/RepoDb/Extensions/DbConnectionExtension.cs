using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static class DbConnectionExtension
    {
        #region Other Methods

        /// <summary>
        /// Identify whether the current instance of <see cref="IDbConnection"/> object corresponds to the target provider.
        /// </summary>
        /// <param name="connection">The connection to be identified.</param>
        /// <param name="provider">The target provider for comparisson.</param>
        /// <returns>Returns true if the <see cref="IDbConnection"/> object corresponds to the target provider.</returns>
        public static bool IsForProvider(this IDbConnection connection, Provider provider)
        {
            switch (provider)
            {
                case Provider.Sql:
                    return NamespaceConstant.SqlConnection.Equals(connection.GetType().FullName);
                case Provider.Oracle:
                    return NamespaceConstant.OracleConnection.Equals(connection.GetType().FullName);
                case Provider.Sqlite:
                    return NamespaceConstant.SqliteConnection.Equals(connection.GetType().FullName);
                case Provider.Npgsql:
                    return NamespaceConstant.NpgsqlConnection.Equals(connection.GetType().FullName);
                case Provider.MySql:
                    return NamespaceConstant.MySqlConnection.Equals(connection.GetType().FullName);
                case Provider.OleDb:
                    return NamespaceConstant.OleDbConnection.Equals(connection.GetType().FullName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the provider of the current <see cref="IDbConnection"/> object.
        /// </summary>
        /// <param name="connection">The target connection object.</param>
        /// <returns>The provider of the target <see cref="IDbConnection"/> object.</returns>
        public static Provider GetProvider(this IDbConnection connection)
        {
            switch (connection.GetType().FullName)
            {
                case NamespaceConstant.SqlConnection:
                    return Provider.Sql;
                case NamespaceConstant.OracleConnection:
                    return Provider.Oracle;
                case NamespaceConstant.SqliteConnection:
                    return Provider.Sqlite;
                case NamespaceConstant.NpgsqlConnection:
                    return Provider.Npgsql;
                case NamespaceConstant.MySqlConnection:
                    return Provider.MySql;
                case NamespaceConstant.OleDbConnection:
                    return Provider.OleDb;
                default:
                    throw new NotSupportedException($"The connection object type '{connection.GetType().FullName}' is currently not supported.");
            }
        }

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
        /// Ensure that the connection object is on open state.
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
        /// Validates whether the transaction object connection is object is equals to the connection object.
        /// </summary>
        /// <param name="connection">The connection object to be validated.</param>
        /// <param name="transaction">The transaction object to compare.</param>
        private static void ValidateTransactionConnectionObject(this IDbConnection connection, IDbTransaction transaction)
        {
            if (transaction != null && transaction.Connection != connection)
            {
                throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
            }
        }

        /// <summary>
        /// Converts the primary key to <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <returns></returns>
        private static QueryGroup PrimaryKeyToQueryGroup<TEntity>(object primaryKey)
            where TEntity : class
        {
            if (primaryKey == null)
            {
                return null;
            }
            var primary = PrimaryKeyCache.Get<TEntity>();
            if (primary == null)
            {
                throw new PrimaryFieldNotFoundException(string.Format("Primary key not found for '{0}' entity.", typeof(TEntity).Name));
            }
            var queryField = new QueryField(primary.GetMappedName(), primaryKey);
            return new QueryGroup(queryField.AsEnumerable());
        }

        #endregion

        #region Guards

        // GuardPrimaryKey

        private static ClassProperty GetAndGuardPrimaryKey<TEntity>()
            where TEntity : class
        {
            var property = PrimaryKeyCache.Get<TEntity>();
            if (property == null)
            {
                throw new PrimaryFieldNotFoundException($"No primary key found at type '{typeof(TEntity).FullName}'.");
            }
            return property;
        }

        #endregion

        #region BatchQuery

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection, where: (QueryGroup)null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQuery<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryInternal<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> BatchQueryInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                where,
                page,
                rowsPerBatch,
                orderBy,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (IEnumerable<TEntity>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                result = DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader)?.ToList();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection, where: (QueryGroup)null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return BatchQueryInternalAsync<TEntity>(connection: connection,
                where: where,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static Task<IEnumerable<TEntity>> BatchQueryInternalAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int page,
            int rowsPerBatch,
            IEnumerable<OrderField> orderBy,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                where,
                page,
                rowsPerBatch,
                orderBy,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BatchQuery");
                    }
                    return Task.FromResult<IEnumerable<TEntity>>(null);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternalAsync<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BulkInsert

        /// <summary>
        /// Bulk-inserting the list of data entity objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternal<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                commandTimeout: commandTimeout,
                trace: trace);
        }

        /// <summary>
        /// Bulk-inserting a <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternal<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                commandTimeout: commandTimeout,
                trace: trace);
        }

        /// <summary>
        /// Bulk-inserting the list of data entity objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection.IsForProvider(Provider.Sql) == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection))
                {
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();
                    if (commandTimeout != null && commandTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                    }
                    if (mappings == null)
                    {
                        reader.Properties.ToList().ForEach(property =>
                        {
                            var columnName = property.GetMappedName();
                            sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                        });
                    }
                    else
                    {
                        mappings.ToList().ForEach(mapItem =>
                        {
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                        });
                    }
                    connection.EnsureOpen();
                    sqlBulkCopy.WriteToServer(reader);
                    result = reader.RecordsAffected;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        /// <summary>
        /// Bulk-inserting a <see cref="DbDataReader"/> object into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int BulkInsertInternal<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection.IsForProvider(Provider.Sql) == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", reader, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection))
            {
                sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();
                if (commandTimeout != null && commandTimeout.HasValue)
                {
                    sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                }
                if (mappings != null)
                {
                    mappings.ToList().ForEach(mapItem =>
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    });
                }
                connection.EnsureOpen();
                sqlBulkCopy.WriteToServer(reader);
                result = reader.RecordsAffected;
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of data entity objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternalAsync<TEntity>(connection: connection,
                entities: entities,
                mappings: mappings,
                commandTimeout: commandTimeout,
                trace: trace);
        }

        /// <summary>
        /// Bulk-inserting a <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            return BulkInsertInternalAsync<TEntity>(connection: connection,
                reader: reader,
                mappings: mappings,
                commandTimeout: commandTimeout,
                trace: trace);
        }

        /// <summary>
        /// Bulk-inserting the list of data entity objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal async static Task<int> BulkInsertInternalAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection.IsForProvider(Provider.Sql) == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities))
            {
                using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection))
                {
                    sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();
                    if (commandTimeout != null && commandTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                    }
                    if (mappings == null)
                    {
                        reader.Properties.ToList().ForEach(property =>
                        {
                            var columnName = property.GetMappedName();
                            sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                        });
                    }
                    else
                    {
                        mappings.ToList().ForEach(mapItem =>
                        {
                            sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                        });
                    }
                    connection.EnsureOpen();
                    await sqlBulkCopy.WriteToServerAsync(reader);
                    result = reader.RecordsAffected;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        /// <summary>
        /// Bulk-inserting a <see cref="DbDataReader"/> object into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="reader">The <see cref="DbDataReader"/> object to be used in the bulk-insert operation.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns will be used for mapping.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal async static Task<int> BulkInsertInternalAsync<TEntity>(this IDbConnection connection,
            DbDataReader reader,
            IEnumerable<BulkInsertMapItem> mappings = null,
            int? commandTimeout = null,
            ITrace trace = null)
            where TEntity : class
        {
            // Validate, only supports SqlConnection
            if (connection.IsForProvider(Provider.Sql) == false)
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog("BulkInsert", reader, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("BulkInsert");
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var sqlBulkCopy = new SqlBulkCopy((SqlConnection)connection))
            {
                sqlBulkCopy.DestinationTableName = ClassMappedNameCache.Get<TEntity>();
                if (commandTimeout != null && commandTimeout.HasValue)
                {
                    sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                }
                if (mappings != null)
                {
                    mappings.ToList().ForEach(mapItem =>
                    {
                        sqlBulkCopy.ColumnMappings.Add(mapItem.SourceColumn, mapItem.DestinationColumn);
                    });
                }
                connection.EnsureOpen();
                await sqlBulkCopy.WriteToServerAsync(reader);
                result = reader.RecordsAffected;
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog("BulkInsert", reader, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Count<TEntity>(connection: connection,
                where: (QueryGroup)null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Count<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Count<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Count<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountInternal<TEntity>(connection: connection,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        internal static long CountInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new CountRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetCountText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return default(int);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = Convert.ToInt64(ExecuteScalarInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction));

            // After Execution
            if (trace != null)
            {
                trace.AfterCount(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountAsync<TEntity>(connection: connection,
                where: (QueryGroup)null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountAsync<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountInternalAsync<TEntity>(connection: connection,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        internal static Task<long> CountInternalAsync<TEntity>(this IDbConnection connection,
            QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new CountRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetCountText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<long>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterCount(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return Task.FromResult<long>(Convert.ToInt64(result.Result));
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Delete<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, QueryField where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Delete<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Delete<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation. When is set to null, it deletes all the data from the database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return Delete<TEntity>(connection: connection,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteInternal<TEntity>(connection: connection,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int DeleteInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new DeleteRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterDelete(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region DeleteAsync

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAsync<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation. When is set to null, it deletes all the data from the database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return DeleteAsync<TEntity>(connection: connection,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteInternalAsync<TEntity>(connection: connection,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> DeleteInternalAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new DeleteRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterDelete(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region DeleteAll

        /// <summary>
        /// Deletes all data in the database based on the target data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int DeleteAll<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAllInternal<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all data in the database based on the target data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int DeleteAllInternal<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteAllText<TEntity>(request);

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
                commandText = (cancellableTraceLog?.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

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

        #region DeleteAllAsync

        /// <summary>
        /// Deletes all data in the database based on the target data entity in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAllInternalAsync<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Deletes all data in the database based on the target data entity in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> DeleteAllInternalAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteAllText<TEntity>(request);

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
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

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

        #region InlineInsert

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public static object InlineInsert<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineInsertInternal<TEntity>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        internal static object InlineInsertInternal<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InlineInsertRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetInlineInsertText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entity, null);
                trace.BeforeInlineInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternal(connection: connection,
                commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineInsert(new TraceLog(commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region InlineInsertAsync

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public static Task<object> InlineInsertAsync<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineInsertInternalAsync<TEntity>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        internal static Task<object> InlineInsertInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InlineInsertRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetInlineInsertText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entity, null);
                trace.BeforeInlineInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<object>(null);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternalAsync(connection: connection,
                commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            var primaryKey = DataEntityExtension.ValueToPrimaryType<TEntity>(result.Result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineInsert(new TraceLog(commandText, entity, primaryKey,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return Task.FromResult<object>(primaryKey);
        }

        #endregion

        #region InlineMerge

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used by the inline merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, object>> qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier)?.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used by the inline merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            Field qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeInternal<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int InlineMergeInternal<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();
            var commandType = CommandType.Text;
            var request = new InlineMergeRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineMergeText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entity, null);
                trace.BeforeInlineMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineMerge(new TraceLog(commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region InlineMergeAsync

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an aynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an aynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used by the inline merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, object>> qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier)?.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an aynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used by the inline merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            Field qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an aynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeInternalAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an aynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> InlineMergeInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();
            var commandType = CommandType.Text;
            var request = new InlineMergeRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineMergeText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, entity, null);
                trace.BeforeInlineMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                entity = (cancellableTraceLog?.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineMerge(new TraceLog(commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region InlineUpdate

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity, QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateInternal<TEntity>(connection: connection,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int InlineUpdateInternal<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InlineUpdateRequest(typeof(TEntity),
                connection,
                where,
                entity?.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetInlineUpdateText<TEntity>(request);
            var param = entity?.Merge(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineUpdate(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity, QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateInternalAsync<TEntity>(connection: connection,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic data entity object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> InlineUpdateInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InlineUpdateRequest(typeof(TEntity),
                connection,
                where,
                entity?.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetInlineUpdateText<TEntity>(request);
            var param = entity?.Merge(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineUpdate(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertInternal(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static object InsertInternal<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InsertRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetInsertText<TEntity>(request);
            var param = ClassExpression.Extract(entity);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = ((IEnumerable<PropertyValue>)cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInsert(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region InsertAsync

        /// <summary>
        /// Inserts a data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertInternalAsync(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data entity object. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        internal static Task<object> InsertInternalAsync<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new InsertRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetInsertText<TEntity>(request);
            var param = ClassExpression.Extract(entity);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<object>(null);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = ((IEnumerable<PropertyValue>)cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalarInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // Set back result equals to PrimaryKey type
            result = Task.FromResult<object>(DataEntityExtension.ValueToPrimaryType<TEntity>(result.Result));

            // After Execution
            if (trace != null)
            {
                trace.AfterInsert(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merges an existing data entity object in the database. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge(connection: connection,
                entity: entity,
                qualifiers: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier).AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge(connection: connection,
                entity: entity,
                qualifiers: qualifier.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Check
            GetAndGuardPrimaryKey<TEntity>();

            // Variables
            var commandType = CommandType.Text;
            var request = new MergeRequest(typeof(TEntity),
                connection,
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetMergeText<TEntity>(request);
            var param = entity?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region MergeAsync

        /// <summary>
        /// Merges an existing data entity object in the database in an asychronous way. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync(connection: connection,
                entity: entity,
                qualifiers: null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database in an asychronous way. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier).AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database in an asychronous way. By default, this operation uses the primary key property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync(connection: connection,
                entity: entity,
                qualifiers: qualifier.AsEnumerable(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeInternalAsync(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges an existing data entity object in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> MergeInternalAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Check
            GetAndGuardPrimaryKey<TEntity>();

            // Variables
            var commandType = CommandType.Text;
            var request = new MergeRequest(typeof(TEntity),
                connection,
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetMergeText<TEntity>(request);
            var param = entity?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: (QueryGroup)null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            object primaryKey,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                orderBy: null,
                top: 0,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null) where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static IEnumerable<TEntity> QueryInternal<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
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
                var item = cache?.Get(cacheKey, false);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryRequest(typeof(TEntity),
                connection,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (IEnumerable<TEntity>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                result = DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader)?.ToList();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null && result?.Any() == true)
            {
                cache?.Add(cacheKey, result);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: (QueryGroup)null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            object primaryKey,
            string hints = null,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                orderBy: null,
                top: 0,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null) where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryInternalAsync<TEntity>(connection: connection,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="hints">The table hints to be used when querying the records. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to null would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        internal static Task<IEnumerable<TEntity>> QueryInternalAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string hints = null,
            string cacheKey = null,
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
                var item = cache?.Get(cacheKey, false);
                if (item != null)
                {
                    return Task.FromResult<IEnumerable<TEntity>>((IEnumerable<TEntity>)item.Value);
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryRequest(typeof(TEntity),
                connection,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryText<TEntity>(request);
            var param = where?.AsObject(typeof(TEntity));

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<IEnumerable<TEntity>>(null);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteQueryInternalAsync<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null && result.Result?.Any() == true)
            {
                cache?.Add(cacheKey, result);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryMultiple

        #region T1, T2

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultipleInternal<T1, T2>(this IDbConnection connection,
        QueryGroup where1,
        QueryGroup where2,
        IEnumerable<OrderField> orderBy1 = null,
        int? top1 = 0,
        string hints1 = null,
        int? top2 = 0,
        IEnumerable<OrderField> orderBy2 = null,
        string hints2 = null,
        int? commandTimeout = null,
        IDbTransaction transaction = null,
        ITrace trace = null,
        IStatementBuilder statementBuilder = null)
        where T1 : class
        where T2 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultipleInternal<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(item1, item2, item3);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query a multiple resultset from the database based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultipleInternal<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(item1, item2, item3, item4);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultipleInternal<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(item1, item2, item3, item4, item5);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                where6: QueryGroup.Parse<T6>(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5, where6 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // T6 Variables
            var request6 = new QueryRequest(typeof(T6),
                connection,
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            var commandText6 = CommandTextCache.GetQueryText<T6>(request6);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
                FieldDefinitionCache.Get<T6>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5, commandText6);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Extract the sixth result
                reader?.NextResult();
                var item6 = DataReaderConverter.ToEnumerable<T6>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>(
                    item1, item2, item3, item4, item5, item6);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="where7">The query expression to be used by this operation for T7.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used by this operation for T7.</param>
        /// <param name="top7">The top number of rows to be used by this operation for T7.</param>
        /// <param name="hints7">The table hints to be used when querying the records for T7. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                where6: QueryGroup.Parse<T6>(where6),
                where7: QueryGroup.Parse<T7>(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="where7">The query expression to be used by this operation for T7.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used by this operation for T7.</param>
        /// <param name="top7">The top number of rows to be used by this operation for T7.</param>
        /// <param name="hints7">The table hints to be used when querying the records for T7. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5, where6, where7 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // T6 Variables
            var request6 = new QueryRequest(typeof(T6),
                connection,
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            var commandText6 = CommandTextCache.GetQueryText<T6>(request6);

            // T7 Variables
            var request7 = new QueryRequest(typeof(T7),
                connection,
                where7,
                orderBy7,
                top7,
                hints7,
                statementBuilder);
            var commandText7 = CommandTextCache.GetQueryText<T7>(request7);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
                FieldDefinitionCache.Get<T6>(connection.ConnectionString);
                FieldDefinitionCache.Get<T7>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5, commandText6, commandText7);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>)null;
            using (var reader = ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Extract the sixth result
                reader?.NextResult();
                var item6 = DataReaderConverter.ToEnumerable<T6>((DbDataReader)reader)?.ToList();

                // Extract the seventh result
                reader?.NextResult();
                var item7 = DataReaderConverter.ToEnumerable<T7>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>(
                    item1, item2, item3, item4, item5, item6, item7);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #endregion

        #region QueryMultipleAsync

        #region T1, T2

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternalAsync<T1, T2>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 2 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleInternalAsync<T1, T2>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternalAsync<T1, T2, T3>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleInternalAsync<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(item1, item2, item3);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query a multiple resultset from the database based on the given 4 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternalAsync<T1, T2, T3, T4>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 3 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleInternalAsync<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(item1, item2, item3, item4);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternalAsync<T1, T2, T3, T4, T5>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 5 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleInternalAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(item1, item2, item3, item4, item5);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternalAsync<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                where6: QueryGroup.Parse<T6>(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 6 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleInternalAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5, where6 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // T6 Variables
            var request6 = new QueryRequest(typeof(T6),
                connection,
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            var commandText6 = CommandTextCache.GetQueryText<T6>(request6);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
                FieldDefinitionCache.Get<T6>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5, commandText6);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Extract the sixth result
                reader?.NextResult();
                var item6 = DataReaderConverter.ToEnumerable<T6>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>(
                    item1, item2, item3, item4, item5, item6);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="where7">The query expression to be used by this operation for T7.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used by this operation for T7.</param>
        /// <param name="top7">The top number of rows to be used by this operation for T7.</param>
        /// <param name="hints7">The table hints to be used when querying the records for T7. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternalAsync<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: QueryGroup.Parse<T1>(where1),
                where2: QueryGroup.Parse<T2>(where2),
                where3: QueryGroup.Parse<T3>(where3),
                where4: QueryGroup.Parse<T4>(where4),
                where5: QueryGroup.Parse<T5>(where5),
                where6: QueryGroup.Parse<T6>(where6),
                where7: QueryGroup.Parse<T7>(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query a multiple resultset from the database based on the given 7 target types in an asychronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where1">The query expression to be used by this operation for T1.</param>
        /// <param name="where2">The query expression to be used by this operation for T2.</param>
        /// <param name="where3">The query expression to be used by this operation for T3.</param>
        /// <param name="where4">The query expression to be used by this operation for T4.</param>
        /// <param name="where5">The query expression to be used by this operation for T5.</param>
        /// <param name="where6">The query expression to be used by this operation for T6.</param>
        /// <param name="where7">The query expression to be used by this operation for T7.</param>
        /// <param name="orderBy1">The order definition of the fields to be used by this operation for T1.</param>
        /// <param name="top1">The top number of rows to be used by this operation for T1.</param>
        /// <param name="hints1">The table hints to be used when querying the records for T1. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy2">The order definition of the fields to be used by this operation for T2.</param>
        /// <param name="top2">The top number of rows to be used by this operation for T2.</param>
        /// <param name="hints2">The table hints to be used when querying the records for T2. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy3">The order definition of the fields to be used by this operation for T3.</param>
        /// <param name="top3">The top number of rows to be used by this operation for T3.</param>
        /// <param name="hints3">The table hints to be used when querying the records for T3. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy4">The order definition of the fields to be used by this operation for T4.</param>
        /// <param name="top4">The top number of rows to be used by this operation for T4.</param>
        /// <param name="hints4">The table hints to be used when querying the records for T4. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy5">The order definition of the fields to be used by this operation for T5.</param>
        /// <param name="top5">The top number of rows to be used by this operation for T5.</param>
        /// <param name="hints5">The table hints to be used when querying the records for T5. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy6">The order definition of the fields to be used by this operation for T6.</param>
        /// <param name="top6">The top number of rows to be used by this operation for T6.</param>
        /// <param name="hints6">The table hints to be used when querying the records for T6. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="orderBy7">The order definition of the fields to be used by this operation for T7.</param>
        /// <param name="top7">The top number of rows to be used by this operation for T7.</param>
        /// <param name="hints7">The table hints to be used when querying the records for T7. See <see cref="SqlTableHints"/> class.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleInternalAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Variables
            var commandType = CommandType.Text;

            // Add fix to the cross-collision of the variables for all the QueryGroup(s)
            var combinedWhere = QueryGroup.CombineForQueryMultiple(new[] { where1, where2, where3, where4, where5, where6, where7 });

            // T1 Variables
            var request1 = new QueryRequest(typeof(T1),
                connection,
                where1,
                orderBy1,
                top1,
                hints1,
                statementBuilder);
            var commandText1 = CommandTextCache.GetQueryText<T1>(request1);

            // T2 Variables
            var request2 = new QueryRequest(typeof(T2),
                connection,
                where2,
                orderBy2,
                top2,
                hints2,
                statementBuilder);
            var commandText2 = CommandTextCache.GetQueryText<T2>(request2);

            // T3 Variables
            var request3 = new QueryRequest(typeof(T3),
                connection,
                where3,
                orderBy3,
                top3,
                hints3,
                statementBuilder);
            var commandText3 = CommandTextCache.GetQueryText<T3>(request3);

            // T4 Variables
            var request4 = new QueryRequest(typeof(T4),
                connection,
                where4,
                orderBy4,
                top4,
                hints4,
                statementBuilder);
            var commandText4 = CommandTextCache.GetQueryText<T4>(request4);

            // T5 Variables
            var request5 = new QueryRequest(typeof(T5),
                connection,
                where5,
                orderBy5,
                top5,
                hints5,
                statementBuilder);
            var commandText5 = CommandTextCache.GetQueryText<T5>(request5);

            // T6 Variables
            var request6 = new QueryRequest(typeof(T6),
                connection,
                where6,
                orderBy6,
                top6,
                hints6,
                statementBuilder);
            var commandText6 = CommandTextCache.GetQueryText<T6>(request6);

            // T7 Variables
            var request7 = new QueryRequest(typeof(T7),
                connection,
                where7,
                orderBy7,
                top7,
                hints7,
                statementBuilder);
            var commandText7 = CommandTextCache.GetQueryText<T7>(request7);

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<T1>(connection.ConnectionString);
                FieldDefinitionCache.Get<T2>(connection.ConnectionString);
                FieldDefinitionCache.Get<T3>(connection.ConnectionString);
                FieldDefinitionCache.Get<T4>(connection.ConnectionString);
                FieldDefinitionCache.Get<T5>(connection.ConnectionString);
                FieldDefinitionCache.Get<T6>(connection.ConnectionString);
                FieldDefinitionCache.Get<T7>(connection.ConnectionString);
            }

            // Shared objects for all types
            var commandText = string.Join(" ", commandText1, commandText2, commandText3, commandText4, commandText5, commandText6, commandText7);
            var param = combinedWhere?.AsObject(null, false);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = (Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>)null;
            using (var reader = await ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction))
            {
                // Extract the first result
                var item1 = DataReaderConverter.ToEnumerable<T1>((DbDataReader)reader)?.ToList();

                // Extract the second result
                reader?.NextResult();
                var item2 = DataReaderConverter.ToEnumerable<T2>((DbDataReader)reader)?.ToList();

                // Extract the third result
                reader?.NextResult();
                var item3 = DataReaderConverter.ToEnumerable<T3>((DbDataReader)reader)?.ToList();

                // Extract the fourth result
                reader?.NextResult();
                var item4 = DataReaderConverter.ToEnumerable<T4>((DbDataReader)reader)?.ToList();

                // Extract the fifth result
                reader?.NextResult();
                var item5 = DataReaderConverter.ToEnumerable<T5>((DbDataReader)reader)?.ToList();

                // Extract the sixth result
                reader?.NextResult();
                var item6 = DataReaderConverter.ToEnumerable<T6>((DbDataReader)reader)?.ToList();

                // Extract the seventh result
                reader?.NextResult();
                var item7 = DataReaderConverter.ToEnumerable<T7>((DbDataReader)reader)?.ToList();

                // Set the result instance
                result = new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>(
                    item1, item2, item3, item4, item5, item6, item7);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQueryMultiple(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #endregion

        #region Truncate

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        public static void Truncate<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            TruncateInternal<TEntity>(connection, commandTimeout, trace, statementBuilder);
        }

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        internal static void TruncateInternal<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetTruncateText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout);

            // After Execution
            if (trace != null)
            {
                trace.AfterTruncate(new TraceLog(commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }
        }

        #endregion

        #region TruncateAsync

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        public static Task TruncateAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return TruncateInternalAsync<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        internal static Task TruncateInternalAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetTruncateText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout);

            // After Execution
            if (trace != null)
            {
                trace.AfterTruncate(new TraceLog(commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var property = GetAndGuardPrimaryKey<TEntity>();
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: new QueryGroup(property.PropertyInfo.AsQueryField(entity, true).AsEnumerable()),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int UpdateInternal<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText<TEntity>(request);
            var param = entity?.AsObject(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region  UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var property = GetAndGuardPrimaryKey<TEntity>();
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: new QueryGroup(property.PropertyInfo.AsQueryField(entity, true).AsEnumerable()),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="primaryKey">The primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            object primaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: PrimaryKeyToQueryGroup<TEntity>(primaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }


        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternalAsync<TEntity>(connection: connection,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of data entity object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> UpdateInternalAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var commandType = CommandType.Text;
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText<TEntity>(request);
            var param = entity?.AsObject(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return Task.FromResult<int>(0);
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region ExecuteQuery (dynamics)

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
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
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static IEnumerable<dynamic> ExecuteQueryInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return DataReaderConverter.ToEnumerable(command.ExecuteReader(), true).ToList();
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
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
            return ExecuteQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<object>> ExecuteQueryInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return DataReaderConverter.ToEnumerable(reader, true).ToList();
                }
            }
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
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
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static IEnumerable<TEntity> ExecuteQueryInternal<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return DataReaderConverter.ToEnumerable<TEntity>(command.ExecuteReader(), true).ToList();
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
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
            return ExecuteQueryInternalAsync<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// converts the result back to an enumerable list of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<TEntity>> ExecuteQueryInternalAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : class
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return DataReaderConverter.ToEnumerable<TEntity>(reader, true).ToList();
                }
            }
        }

        #endregion

        #region ExecuteQueryMultiple

        /// <summary>
        /// Executes a multiple query statement from the database.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
        public static QueryMultipleExtractor ExecuteQueryMultiple(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var reader = ExecuteReaderInternal(connection,
                commandText,
                param,
                commandType,
                commandTimeout,
                transaction);
            return new QueryMultipleExtractor((DbDataReader)reader);
        }

        /// <summary>
        /// Executes a multiple query statement from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of <see cref="QueryMultipleExtractor"/> used to extract the results.</returns>
        public static async Task<QueryMultipleExtractor> ExecuteQueryMultipleAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var reader = await ExecuteReaderInternalAsync(connection,
                commandText,
                param,
                commandType,
                commandTimeout,
                transaction);
            return new QueryMultipleExtractor((DbDataReader)reader);
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of the data reader object.</returns>
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteReaderInternal(connection, commandText, param, commandType, commandTimeout, transaction);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of the data reader object.</returns>
        internal static IDataReader ExecuteReaderInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of the data reader object.</returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteReaderInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteReader(CommandBehavior)"/> and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of the data reader object.</returns>
        internal static async Task<IDataReader> ExecuteReaderInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return await command.ExecuteReaderAsync();
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
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
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int ExecuteNonQueryInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a query from the database in asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> ExecuteNonQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteNonQueryInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database in asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteNonQuery"/> and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static async Task<int> ExecuteNonQueryInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
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
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying method <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static object ExecuteScalarInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return ObjectConverter.DbNullToNull(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteScalarInternalAsync(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying method <see cref="IDbCommand.ExecuteScalar"/> and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <see cref="IDbCommand.CommandText"/> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static async Task<object> ExecuteScalarInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                var result = await command.ExecuteScalarAsync();
                return ObjectConverter.DbNullToNull(result);
            }
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Creates a new instance of <see cref="DbCommand"/> object that is to be used execution.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <param name="commandText">The command text to be used.</param>
        /// <param name="param">The list of parameters.</param>
        /// <param name="commandType">The command type to be used.</param>
        /// <param name="commandTimeout">The command timeout to be used.</param>
        /// <param name="transaction">The transaction object to be used.</param>
        /// <returns>An instance of <see cref="DbCommand"/> object.</returns>
        private static DbCommand CreateDbCommandForExecution(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Identify target statement, for now, only support the param with single parameter that is an array
            var property = param?.GetType().GetProperties().FirstOrDefault();
            var arrayValues = (IEnumerable<object>)null;

            // Get the values for the arrays
            if (property != null && property.PropertyType.IsArray)
            {
                arrayValues = ((Array)property.GetValue(param)).AsEnumerable();
                commandText = ToRawSqlWithArrayParams(commandText, property.Name, arrayValues);
            }

            // Command object initialization
            var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction);

            // Identify target statement, for now, only support array with single parameters
            if (arrayValues != null)
            {
                command.CreateParametersFromArray(property.Name, arrayValues);
            }
            else
            {
                // Add the parameters
                command.CreateParameters(param);
            }

            // Execute the scalar
            return (DbCommand)command;
        }

        /// <summary>
        /// Converts the command text into a raw SQL with Array Parameters.
        /// </summary>
        /// <param name="commandText">The current command text where the raw sql parameters will be replaced.</param>
        /// <param name="parameterName">The name of the parameter to be replaced.</param>
        /// <param name="values">The array of the values.</param>
        /// <returns></returns>
        private static string ToRawSqlWithArrayParams(string commandText, string parameterName, IEnumerable<object> values)
        {
            // Check for the defined parameter
            if (commandText.IndexOf(parameterName) >= 0)
            {
                // Get the variables needed
                var length = values != null ? values.Count() : 0;
                var parameters = new string[length];

                // Iterate and set the parameter values
                for (var i = 0; i < length; i++)
                {
                    parameters[i] = string.Concat(parameterName, i).AsParameter();
                }

                // Replace the target parameter
                commandText = commandText.Replace(parameterName.AsParameter(), parameters.Join(", "));
            }

            // Return the newly composed command text
            return commandText;
        }

        #endregion
    }
}