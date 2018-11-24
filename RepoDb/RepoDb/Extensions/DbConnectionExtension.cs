using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        /// Converts the (WHERE) query expression to <see cref="QueryGroup"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <returns></returns>
        private static QueryGroup WhereToQueryGroup<TEntity>(object whereOrWhat)
            where TEntity : class
        {
            if (whereOrWhat == null)
            {
                return null;
            }
            var queryGroup = (QueryGroup)null;
            if (whereOrWhat is QueryField)
            {
                var queryField = (QueryField)whereOrWhat;
                queryGroup = new QueryGroup(queryField.AsEnumerable());
            }
            else if (whereOrWhat is QueryGroup)
            {
                queryGroup = (QueryGroup)whereOrWhat;
            }
            else if (whereOrWhat is TEntity)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                if (primary != null)
                {
                    var queryField = primary.PropertyInfo.AsQueryField(whereOrWhat);
                    queryGroup = new QueryGroup(queryField.AsEnumerable());
                }
            }
            else
            {
                if (whereOrWhat?.GetType().IsGenericType == true)
                {
                    queryGroup = QueryGroup.Parse(whereOrWhat);
                }
                else
                {
                    var primary = PrimaryKeyCache.Get<TEntity>();
                    if (primary != null)
                    {
                        var queryField = new QueryField(primary.GetMappedName(), whereOrWhat);
                        queryGroup = new QueryGroup(queryField.AsEnumerable());
                    }
                }
            }
            if (queryGroup == null)
            {
                throw new InvalidQueryExpressionException("The query expression passed is not valid.");
            }
            return queryGroup;
        }

        #endregion

        #region Guards

        // GuardPrimaryKey

        private static ClassProperty GetAndGuardPrimaryKey<TEntity>(Command command)
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection,
            object whereOrWhat,
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
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.BatchQuery;
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                where,
                page,
                rowsPerBatch,
                orderBy,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText<TEntity>(request);
            var param = where?.AsObject();

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity)))
            {
                result = DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader)?.ToList();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of data entity object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection,
            object whereOrWhat,
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
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.BatchQuery;
            var commandType = CommandType.Text;
            var request = new BatchQueryRequest(typeof(TEntity),
                connection,
                where,
                page,
                rowsPerBatch,
                orderBy,
                statementBuilder);
            var commandText = CommandTextCache.GetBatchQueryText<TEntity>(request);
            var param = where?.AsObject();

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeBatchQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return Task.FromResult<object>(null);
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns defined via <see cref="Command.BulkInsert"/> will be used for mapping.</param>
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
        /// Bulk-inserting the list of data entity objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns defined via <see cref="Command.BulkInsert"/> will be used for mapping.</param>
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

            // Variables
            var command = mappings != null ? Command.None : Command.BulkInsert;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), command.ToString(), entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, command))
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
                trace.AfterBulkInsert(new TraceLog(MethodBase.GetCurrentMethod(), command.ToString(), entities, result,
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
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns defined via <see cref="Command.BulkInsert"/> will be used for mapping.</param>
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
        /// Bulk-inserting the list of data entity objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the data entities to be bulk-inserted.</param>
        /// <param name="mappings">The list of the columns to be used for mappings. If this parameter is not set, then all columns defined via <see cref="Command.BulkInsert"/> will be used for mapping.</param>
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

            // Variables
            var command = mappings != null ? Command.None : Command.BulkInsert;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), command.ToString(), entities, null);
                trace.BeforeBulkInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return 0;
                }
            }

            var result = 0;

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = new DataEntityDataReader<TEntity>(entities, command))
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
                trace.AfterBulkInsert(new TraceLog(MethodBase.GetCurrentMethod(), command.ToString(), entities, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection,
            object whereOrWhat,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Count<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.Count;
            var commandType = CommandType.Text;
            var request = new CountRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetCountText<TEntity>(request);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity)));

            // After Execution
            if (trace != null)
            {
                trace.AfterCount(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection,
            object whereOrWhat,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return CountAsync<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.Count;
            var commandType = CommandType.Text;
            var request = new CountRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetCountText<TEntity>(request);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeCount(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterCount(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return Task.FromResult<long>(Convert.ToInt64(result.Result));
        }

        #endregion

        #region Delete

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
        public static int Delete<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Delete<TEntity>(connection: connection,
                where: (QueryGroup)null,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation. When is set to null, it deletes all the data from the database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, object whereOrWhat, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>(Command.Delete);
            return Delete<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
                commandTimeout: commandTimeout,
                transaction: transaction);
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
            var command = Command.Delete;
            var commandType = CommandType.Text;
            var request = new DeleteRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteText<TEntity>(request);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region DeleteAsync

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
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return DeleteAsync<TEntity>(connection: connection,
                where: (QueryGroup)null,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation. When is set to null, it deletes all the data from the database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection,
            object whereOrWhat,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>(Command.Delete);
            return DeleteAsync<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.Delete;
            var commandType = CommandType.Text;
            var request = new DeleteRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteText<TEntity>(request);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeDelete(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterDelete(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.DeleteAll;
            var commandType = CommandType.Text;
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteAllText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, null, null);
                trace.BeforeDeleteAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterDeleteAll(new TraceLog(MethodBase.GetCurrentMethod(), commandText, null, result,
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
            var command = Command.DeleteAll;
            var commandType = CommandType.Text;
            var request = new DeleteAllRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetDeleteAllText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, null, null);
                trace.BeforeDeleteAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterDeleteAll(new TraceLog(MethodBase.GetCurrentMethod(), commandText, null, result,
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
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public static object InlineInsert<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineInsertInternal<TEntity>(connection: connection,
                entity: entity,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        internal static object InlineInsertInternal<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineInsert;
            var commandType = CommandType.Text;
            var request = new InlineInsertRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineInsertText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                trace.BeforeInlineInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, result,
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
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        public static Task<object> InlineInsertAsync<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineInsertInternalAsync<TEntity>(connection: connection,
                entity: entity,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>The value of the primary key of the newly inserted data entity object.</returns>
        internal static Task<object> InlineInsertInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineInsert;
            var commandType = CommandType.Text;
            var request = new InlineInsertRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineInsertText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                trace.BeforeInlineInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // Set back result equals to PrimaryKey type
            var primaryKey = DataEntityExtension.ValueToPrimaryType<TEntity>(result.Result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, primaryKey,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, object>> qualifier,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier)?.AsEnumerable(),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            Field qualifier,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMerge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeInternal<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int InlineMergeInternal<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineMerge;
            var entityProperties = entity?.GetType().GetProperties();
            var primary = PrimaryKeyCache.Get<TEntity>();
            var commandType = CommandType.Text;
            var request = new InlineMergeRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                qualifiers,
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineMergeText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                trace.BeforeInlineMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, result,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, object>> qualifier,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: Field.Parse(qualifier)?.AsEnumerable(),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            Field qualifier,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineMergeInternalAsync<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> InlineMergeInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<Field> qualifiers,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineMerge;
            var entityProperties = entity?.GetType().GetProperties();
            var primary = PrimaryKeyCache.Get<TEntity>();
            var commandType = CommandType.Text;
            var request = new InlineMergeRequest(typeof(TEntity),
                connection,
                entity?.AsFields(),
                qualifiers,
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineMergeText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, entity, null);
                trace.BeforeInlineMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, entity, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            object whereOrWhat,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, bool>> where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity, QueryField where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<QueryField> where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateInternal<TEntity>(connection: connection,
                entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int InlineUpdateInternal<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineUpdate;
            var commandType = CommandType.Text;
            var request = new InlineUpdateRequest(typeof(TEntity),
                connection,
                where,
                entity?.AsFields(),
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineUpdateText<TEntity>(request);
            var param = entity?.Merge(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            object whereOrWhat,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            Expression<Func<TEntity, bool>> where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity, QueryField where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            IEnumerable<QueryField> where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: where != null ? new QueryGroup(where) : null,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InlineUpdateInternalAsync<TEntity>(connection: connection,
                entity: entity,
                where: where,
                overrideIgnore: overrideIgnore,
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
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <see cref="IgnoreAttribute"/> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static Task<int> InlineUpdateInternalAsync<TEntity>(this IDbConnection connection,
            object entity,
            QueryGroup where,
            bool? overrideIgnore = false,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var command = Command.InlineUpdate;
            var commandType = CommandType.Text;
            var request = new InlineUpdateRequest(typeof(TEntity),
                connection,
                where,
                entity?.AsFields(),
                overrideIgnore,
                statementBuilder);
            var commandText = CommandTextCache.GetInlineUpdateText<TEntity>(request);
            var param = entity?.Merge(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeInlineUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.Insert;
            var commandType = CommandType.Text;
            var request = new InsertRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetInsertText<TEntity>(request);
            var param = ClassExpression.Extract(entity, command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // Set back result equals to PrimaryKey type
            result = DataEntityExtension.ValueToPrimaryType<TEntity>(result);

            // After Execution
            if (trace != null)
            {
                trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.Insert;
            var commandType = CommandType.Text;
            var request = new InsertRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetInsertText<TEntity>(request);
            var param = ClassExpression.Extract(entity, command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeInsert(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // Set back result equals to PrimaryKey type
            result = Task.FromResult<object>(DataEntityExtension.ValueToPrimaryType<TEntity>(result.Result));

            // After Execution
            if (trace != null)
            {
                trace.AfterInsert(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.Merge;

            // Check
            GetAndGuardPrimaryKey<TEntity>(command);

            // Variables
            var commandType = CommandType.Text;
            var request = new MergeRequest(typeof(TEntity),
                connection,
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetMergeText<TEntity>(request);
            var param = entity?.AsObject(command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.Merge;

            // Check
            GetAndGuardPrimaryKey<TEntity>(command);

            // Variables
            var commandType = CommandType.Text;
            var request = new MergeRequest(typeof(TEntity),
                connection,
                qualifiers,
                statementBuilder);
            var commandText = CommandTextCache.GetMergeText<TEntity>(request);
            var param = entity?.AsObject(command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
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
            object whereOrWhat,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Query<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
                orderBy: orderBy,
                top: top,
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
            var command = Command.Query;
            var commandType = CommandType.Text;
            var request = new QueryRequest(typeof(TEntity),
                connection,
                where,
                orderBy,
                top,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryText<TEntity>(request);
            var param = where?.AsObject();

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity)))
            {
                result = DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader)?.ToList();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
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
            object whereOrWhat,
            IEnumerable<OrderField> orderBy = null, int? top = 0,
            string cacheKey = null,
            ICache cache = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return QueryAsync<TEntity>(connection: connection,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
                orderBy: orderBy,
                top: top,
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
            var command = Command.Query;
            var commandType = CommandType.Text;
            var request = new QueryRequest(typeof(TEntity),
                connection,
                where,
                orderBy,
                top,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryText<TEntity>(request);
            var param = where?.AsObject();

            // Database pre-touch for field definitions
            if (connection.IsForProvider(Provider.Sql))
            {
                FieldDefinitionCache.Get<TEntity>(connection.ConnectionString);
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
                    }
                    return Task.FromResult<object>(null);
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var command = Command.Truncate;
            var commandType = CommandType.Text;
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetTruncateText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                commandTimeout: commandTimeout,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterTruncate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, null, result,
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
            var command = Command.Truncate;
            var commandType = CommandType.Text;
            var request = new TruncateRequest(typeof(TEntity),
                connection,
                statementBuilder);
            var commandText = CommandTextCache.GetTruncateText<TEntity>(request);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, null, null);
                trace.BeforeTruncate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                commandTimeout: commandTimeout,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterTruncate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, null, result,
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
            var property = GetAndGuardPrimaryKey<TEntity>(Command.Update);
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrWhat,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>(Command.Update);
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.Update;
            var commandType = CommandType.Text;
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText<TEntity>(request);
            var param = entity?.AsObject(where, command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
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
            var property = GetAndGuardPrimaryKey<TEntity>(Command.Update);
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
        /// <param name="whereOrWhat">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrWhat,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>(Command.Update);
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: WhereToQueryGroup<TEntity>(whereOrWhat),
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
            var command = Command.Update;
            var commandType = CommandType.Text;
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText<TEntity>(request);
            var param = entity?.AsObject(where, command);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(command.ToString());
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
                transaction: transaction,
                entityType: typeof(TEntity));

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        #endregion

        #region Execute Commands

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
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, null))
            {
                return DataReaderConverter.ToEnumerable(command.ExecuteReader());
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
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, null))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return DataReaderConverter.ToEnumerable(reader);
                }
            }
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
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, typeof(TEntity)))
            {
                return DataReaderConverter.ToEnumerable<TEntity>(command.ExecuteReader()).ToList();
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
                transaction: transaction,
                entityType: typeof(TEntity));
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>
        /// An enumerable list of data entity object containing the converted results of the underlying <see cref="IDataReader"/> object.
        /// </returns>
        internal static async Task<IEnumerable<TEntity>> ExecuteQueryInternalAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
            where TEntity : class
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, typeof(TEntity)))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return DataReaderConverter.ToEnumerable<TEntity>(reader).ToList();
                }
            }
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
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return ExecuteReaderInternal(connection, commandText, param, commandType, commandTimeout, transaction, null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An instance of the data reader object.</returns>
        internal static IDataReader ExecuteReaderInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
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
                transaction: transaction,
                entityType: null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An instance of the data reader object.</returns>
        internal static async Task<IDataReader> ExecuteReaderInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
            {
                return await command.ExecuteReaderAsync();
            }
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
                entityType: null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static int ExecuteNonQueryInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
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
                transaction: transaction,
                entityType: null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        internal static async Task<int> ExecuteNonQueryInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
            {
                return await command.ExecuteNonQueryAsync();
            }
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
                entityType: null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static object ExecuteScalarInternal(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
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
                transaction: transaction,
                entityType: null);
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
        /// <param name="entityType">The type of data entity where to map the current param types.</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
        internal static async Task<object> ExecuteScalarInternalAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
        {
            using (var command = CreateDbCommandForExecution(connection, commandText, param, commandType, commandTimeout, transaction, entityType))
            {
                var result = await command.ExecuteScalarAsync();
                return ObjectConverter.DbNullToNull(result);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbCommand"/> object that is to be used execution.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static DbCommand CreateDbCommandForExecution(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Type entityType = null)
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
                command.CreateParameters(param, entityType);
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