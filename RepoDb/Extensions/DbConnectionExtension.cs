using RepoDb.EventArguments;
using RepoDb.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb.Extensions
{
    public static class DbConnectionExtension
    {
        // CreateCommand
        internal static IDbCommand CreateCommand(this IDbConnection connection,
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

        // EnsureOpen
        public static IDbConnection EnsureOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        // ExecuteReader
        public static IEnumerable<TEntity> ExecuteReader<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            var eventArgs = new CancellableExecutionEventArgs(commandText, param);

            // Before Execution
            EventNotifier.OnBeforeExecuteReaderExecution(connection, eventArgs);

            // Cancel Execution
            if (eventArgs.IsCancelled)
            {
                EventNotifier.OnCancelledExecution(connection, new CancelledExecutionEventArgs(commandText, param));
                return null;
            }

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                using (var reader = command.ExecuteReader())
                {
                    var list = new List<TEntity>();
                    while (reader.Read())
                    {
                        var obj = reader.ToDataEntity<TEntity>();
                        list.Add(obj);
                    }

                    // After Execution
                    EventNotifier.OnAfterExecuteReaderExecution(connection, new ExecutionEventArgs(commandText, list));

                    // Result
                    return list;
                }
            }
        }

        // ExecuteReaderAsync
        public static Task<IEnumerable<TEntity>> ExecuteReaderAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                ExecuteReader<TEntity>(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        // ExecuteReaderEx
        public static IEnumerable<object> ExecuteReaderEx(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var eventArgs = new CancellableExecutionEventArgs(commandText, param);

            // Before Execution
            EventNotifier.OnBeforeExecuteReaderExExecution(connection, eventArgs);

            // Cancel Execution
            if (eventArgs.IsCancelled)
            {
                EventNotifier.OnCancelledExecution(connection, new CancelledExecutionEventArgs(commandText, param));
                return null;
            }

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                using (var reader = command.ExecuteReader())
                {
                    var list = new List<object>();
                    while (reader.Read())
                    {
                        var obj = reader.ToObject();
                        list.Add(obj);
                    }

                    // After Execution
                    EventNotifier.OnAfterExecuteReaderExExecution(connection, new ExecutionEventArgs(commandText, list));

                    // Result
                    return list;
                }
            }
        }

        // ExecuteReaderExAsync
        public static Task<IEnumerable<object>> ExecuteReaderExAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<IEnumerable<object>>(() =>
                ExecuteReaderEx(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        // ExecuteNonQuery
        public static int ExecuteNonQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var eventArgs = new CancellableExecutionEventArgs(commandText, param);

            // Before Execution
            EventNotifier.OnBeforeExecuteNonQueryExecution(connection, eventArgs);

            // Cancel Execution
            if (eventArgs.IsCancelled)
            {
                EventNotifier.OnCancelledExecution(connection, new CancelledExecutionEventArgs(commandText, param));
                return 0;
            }

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var result = command.ExecuteNonQuery();

                // After Execution
                EventNotifier.OnAfterExecuteNonQueryExecution(connection, new ExecutionEventArgs(commandText, result));

                // Result
                return result;
            }
        }

        // ExecuteNonQueryAsync
        public static Task<int> ExecuteNonQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<int>(() =>
                ExecuteNonQuery(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        // ExecuteScalar
        public static object ExecuteScalar(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            var eventArgs = new CancellableExecutionEventArgs(commandText, param);

            // Before Execution
            EventNotifier.OnBeforeExecuteScalarExecution(connection, eventArgs);

            // Cancel Execution
            if (eventArgs.IsCancelled)
            {
                EventNotifier.OnCancelledExecution(connection, new CancelledExecutionEventArgs(commandText, param));
                return null;
            }

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var result = command.ExecuteScalar();

                // After Execution
                EventNotifier.OnAfterExecuteScalarExecution(connection, new ExecutionEventArgs(commandText, result));

                // Result
                return result;
            }
        }

        // ExecuteScalarAsync
        public static Task<object> ExecuteScalarAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew<object>(() =>
                ExecuteScalarAsync(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }
    }
}