using RepoDb.EventArguments;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
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
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : DataEntity
        {
            var cancellableTraceLog = (CancellableTraceLog)null;

            // Before Execution
            if (trace != null)
            {
                cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeExecuteReader(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("ExecuteReader");
                    }
                    return null;
                }
            }
            commandText = (cancellableTraceLog?.Statement ?? commandText);
            param = (cancellableTraceLog?.Parameter ?? param);

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
                    if (trace != null)
                    {
                        trace.AfterExecuteReader(new TraceLog(commandText, param, list));
                    }

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
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<IEnumerable<TEntity>>(() =>
                ExecuteReader<TEntity>(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }

        // ExecuteReaderEx
        public static IEnumerable<object> ExecuteReaderEx(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var cancellableTraceLog = (CancellableTraceLog)null;

            // Before Execution
            if (trace != null)
            {
                cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeExecuteReaderEx(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("ExecuteReaderEx");
                    }
                    return null;
                }
            }
            commandText = (cancellableTraceLog?.Statement ?? commandText);
            param = (cancellableTraceLog?.Parameter ?? param);

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
                    if (trace != null)
                    {
                        trace.AfterExecuteReaderEx(new TraceLog(commandText, param, list));
                    }

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
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew<IEnumerable<object>>(() =>
                ExecuteReaderEx(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }

        // ExecuteNonQuery
        public static int ExecuteNonQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var cancellableTraceLog = (CancellableTraceLog)null;

            // Before Execution
            if (trace != null)
            {
                cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeExecuteNonQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("ExecuteNonQuery");
                    }
                    return 0;
                }
            }
            commandText = (cancellableTraceLog?.Statement ?? commandText);
            param = (cancellableTraceLog?.Parameter ?? param);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var result = command.ExecuteNonQuery();

                // After Execution
                if (trace != null)
                {
                    trace.AfterExecuteNonQuery(new TraceLog(commandText, param, result));
                }

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
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew<int>(() =>
                ExecuteNonQuery(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }

        // ExecuteScalar
        public static object ExecuteScalar(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var cancellableTraceLog = (CancellableTraceLog)null;

            // Before Execution
            if (trace != null)
            {
                cancellableTraceLog = new CancellableTraceLog(commandText, param, null);
                trace.BeforeExecuteScalar(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("ExecuteScalar");
                    }
                    return null;
                }
            }
            commandText = (cancellableTraceLog?.Statement ?? commandText);
            param = (cancellableTraceLog?.Parameter ?? param);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var result = command.ExecuteScalar();
                if (result == DBNull.Value)
                {
                    result = null;
                }

                // After Execution
                if (trace != null)
                {
                    trace.AfterExecuteScalar(new TraceLog(commandText, param, result));
                }

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
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew<object>(() =>
                ExecuteScalarAsync(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }
    }
}