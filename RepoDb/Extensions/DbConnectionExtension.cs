using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        // ExecuteQuery
        public static IEnumerable<object> ExecuteQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCanceled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CanceledExecutionException(Constant.ExecuteQuery);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                using (var reader = command.ExecuteReader())
                {
                    //var result = reader.AsEnumerable();
                    var result = DataReaderConverter.ToEnumerable((DbDataReader)reader);

                    // After Execution
                    if (trace != null)
                    {
                        trace.AfterExecuteQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                            DateTime.UtcNow.Subtract(beforeExecutionTime)));
                    }

                    // Result
                    return result;
                }
            }
        }

        // ExecuteQueryAsync
        public static Task<IEnumerable<object>> ExecuteQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew(() =>
                ExecuteQuery(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }

        // ExecuteQuery
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : IDataEntity
        {
            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCanceled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CanceledExecutionException(Constant.ExecuteQuery);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = ExecuteReader(connection, commandText, param, commandType, commandTimeout, transaction, trace))
            {
                var result = DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader);

                // After Execution
                if (trace != null)
                {
                    trace.AfterExecuteQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }

                // Result
                return result;
            }
        }

        // ExecuteQueryAsync
        public static Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
            where TEntity : IDataEntity
        {
            return Task.Factory.StartNew(() =>
                ExecuteQuery<TEntity>(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace));
        }

        // ExecuteReader
        public static IDataReader ExecuteReader(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteReader(cancellableTraceLog);
                if (cancellableTraceLog.IsCanceled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CanceledExecutionException(Constant.ExecuteReader);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var reader = command.ExecuteReader();
                if (trace != null)
                {
                    trace.AfterExecuteReader(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, reader,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
                }
                return reader;
            }
        }

        // ExecuteReaderAsync
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            return Task.Factory.StartNew(() =>
                ExecuteReader(connection: connection,
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
            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteNonQuery(cancellableTraceLog);
                if (cancellableTraceLog.IsCanceled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CanceledExecutionException(Constant.ExecuteNonQuery);
                    }
                    return 0;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                var result = command.ExecuteNonQuery();

                // After Execution
                if (trace != null)
                {
                    trace.AfterExecuteNonQuery(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
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
            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(MethodBase.GetCurrentMethod(), commandText, param, null);
                trace.BeforeExecuteScalar(cancellableTraceLog);
                if (cancellableTraceLog.IsCanceled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CanceledExecutionException(Constant.ExecuteScalar);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog?.Statement ?? commandText);
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

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
                    trace.AfterExecuteScalar(new TraceLog(MethodBase.GetCurrentMethod(), commandText, param, result,
                        DateTime.UtcNow.Subtract(beforeExecutionTime)));
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