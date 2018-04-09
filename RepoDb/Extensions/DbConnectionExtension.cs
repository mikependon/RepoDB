using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
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
                cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteReader(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.ExecuteReader);
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
                    var list = reader.AsEnumerable<TEntity>();

                    // After Execution
                    if (trace != null)
                    {
                        trace.AfterExecuteReader(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, list));
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

        // ExecuteReader
        public static IEnumerable<object> ExecuteReader(this IDbConnection connection,
            string commandText,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            var cancellableTraceLog = (CancellableTraceLog)null;

            // Before Execution
            if (trace != null)
            {
                cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteReaderEx(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.ExecuteReaderEx);
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
                    var list = reader.AsObjects();

                    // After Execution
                    if (trace != null)
                    {
                        trace.AfterExecuteReaderEx(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, list));
                    }

                    // Result
                    return list;
                }
            }
        }

        // ExecuteReaderAsync
        public static Task<IEnumerable<object>> ExecuteReaderAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew<IEnumerable<object>>(() =>
                ExecuteReader(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandTimeout: commandTimeout,
                    commandType: commandType,
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
                cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteNonQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.ExecuteNonQuery);
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
                    trace.AfterExecuteNonQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
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
                cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteScalar(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(Constant.ExecuteScalar);
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
                    trace.AfterExecuteScalar(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result));
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