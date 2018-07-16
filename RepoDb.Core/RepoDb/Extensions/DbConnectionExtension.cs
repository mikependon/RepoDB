using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <i>System.Data.IDbConnection</i> object.
    /// </summary>
    public static class DbConnectionExtension
    {
        #region Other Methods

        /// <summary>
        /// Creates a command object.
        /// </summary>
        /// <param name="connection">The connection to be used when creating a command object.</param>
        /// <param name="commandText">The value of the <i>CommandText</i> property.</param>
        /// <param name="commandType">The value of the <i>CommandType</i> property.</param>
        /// <param name="commandTimeout">The value of the <i>CommandTimeout</i> property.</param>
        /// <param name="transaction">The value of the <i>Transaction</i> property (if present).</param>
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
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private static void ValidateTransactionConnectionObject(this IDbConnection connection, IDbTransaction transaction)
        {
            if (transaction != null)
            {
                if (transaction.Connection != connection)
                {
                    throw new InvalidOperationException("The transaction connection object is different from the current connection object.");
                }
            }
        }

        /// <summary>
        /// Converts the <i>where</i> query expression to <i>RepoDb.QueryGroup</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <returns></returns>
        private static QueryGroup WhereToQueryGroup<TEntity>(object where) where TEntity : DataEntity
        {
            if (where == null)
            {
                return null;
            }
            var queryGroup = (QueryGroup)null;
            if (where is QueryField)
            {
                var queryField = (QueryField)where;
                queryGroup = new QueryGroup(queryField.AsEnumerable());
            }
            else if (where is QueryGroup)
            {
                queryGroup = (QueryGroup)where;
            }
            else if (where is TEntity)
            {
                var primaryProperty = DataEntityExtension.GetPrimaryProperty<TEntity>();
                if (primaryProperty != null)
                {
                    var queryField = primaryProperty.AsQueryField(where);
                    queryGroup = new QueryGroup(queryField.AsEnumerable());
                }
            }
            else
            {
                if (where?.GetType().GetTypeInfo().IsGenericType == true)
                {
                    queryGroup = QueryGroup.Parse(where);
                }
                else
                {
                    var primaryProperty = DataEntityExtension.GetPrimaryProperty<TEntity>();
                    if (primaryProperty != null)
                    {
                        var queryField = new QueryField(primaryProperty.GetMappedName(), where);
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

        private static PropertyInfo GetAndGuardPrimaryKey<TEntity>(Command command)
            where TEntity : DataEntity
        {
            var property = DataEntityExtension.GetPrimaryProperty<TEntity>();
            if (property == null)
            {
                throw new PrimaryFieldNotFoundException($"No primary key found at type '{typeof(TEntity).FullName}'.");
            }
            return property;
        }

        // GuardBatchQueryable

        private static void GuardBatchQueryable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsBatchQueryable<TEntity>())
            {
                throw new EntityNotBatchQueryableException(DataEntityExtension.GetMappedName<TEntity>(Command.BatchQuery));
            }
        }

        // GuardBulkInsert

        private static void GuardBulkInsert<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsBulkInsertable<TEntity>())
            {
                throw new EntityNotBulkInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.BulkInsert));
            }
        }

        // GuardCountable

        private static void GuardCountable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsCountable<TEntity>())
            {
                throw new EntityNotCountableException(DataEntityExtension.GetMappedName<TEntity>(Command.Count));
            }
        }

        // GuardDeletable

        private static void GuardDeletable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsDeletable<TEntity>())
            {
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>(Command.Delete));
            }
        }

        // GuardDeletableAll

        private static void GuardDeletableAll<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsDeletableAll<TEntity>())
            {
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>(Command.DeleteAll));
            }
        }

        // GuardInlineInsertable

        private static void GuardInlineInsertable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineInsertable<TEntity>())
            {
                throw new EntityNotInlineInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineInsert));
            }
        }

        // GuardInlineMergeable

        private static void GuardInlineMergeable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineMergeable<TEntity>())
            {
                throw new EntityNotInlineMergeableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineMerge));
            }
        }

        // GuardInlineUpdateable

        private static void GuardInlineUpdateable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInlineUpdateable<TEntity>())
            {
                throw new EntityNotInlineUpdateableException(DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate));
            }
        }

        // GuardInsertable

        private static void GuardInsertable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsInsertable<TEntity>())
            {
                throw new EntityNotInsertableException(DataEntityExtension.GetMappedName<TEntity>(Command.Insert));
            }
        }

        // GuardMergeable

        private static void GuardMergeable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsMergeable<TEntity>())
            {
                throw new EntityNotMergeableException(DataEntityExtension.GetMappedName<TEntity>(Command.Merge));
            }
        }

        // GuardQueryable

        private static void GuardQueryable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsQueryable<TEntity>())
            {
                throw new EntityNotQueryableException(DataEntityExtension.GetMappedName<TEntity>(Command.Query));
            }
        }

        // GuardTruncatable

        private static void GuardTruncatable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsTruncatable<TEntity>())
            {
                throw new EntityNotDeletableException(DataEntityExtension.GetMappedName<TEntity>(Command.Truncate));
            }
        }

        // GuardUpdateable

        private static void GuardUpdateable<TEntity>()
            where TEntity : DataEntity
        {
            if (!DataEntityExtension.IsUpdateable<TEntity>())
            {
                throw new EntityNotUpdateableException(DataEntityExtension.GetMappedName<TEntity>(Command.Update));
            }
        }

        #endregion

        #region Operational Commands

        // BatchQuery

        /// <summary>
        /// Query the data from the database by batch. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection, object where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return BatchQuery<TEntity>(connection: connection,
                where: queryGroup,
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return BatchQuery<TEntity>(connection: connection,
                where: new QueryGroup(where),
                page: page,
                rowsPerBatch: rowsPerBatch,
                orderBy: orderBy,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> objects.</returns>
        public static IEnumerable<TEntity> BatchQuery<TEntity>(this IDbConnection connection, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardBatchQueryable<TEntity>();

            // Variables
            var command = Command.BatchQuery;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateBatchQuery(new QueryBuilder<TEntity>(), where, page, rowsPerBatch, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteQuery<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterBatchQuery(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // BatchQueryAsync

        /// <summary>
        /// Query the data from the database by batch in an asynchronous way. The batching will vary on the page number and number of rows per batch defined on this
        /// operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(connection: connection,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection, object where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(connection: connection,
                    where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(connection: connection,
                    where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Query the data from the database by batch based on the given query expression in an asynchronous way. The batching will vary on the page number and number of rows
        /// per batch defined by this operation. This operation is useful for paging purposes.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="page">The page of the batch to be used by this operation.</param>
        /// <param name="rowsPerBatch">The number of rows per batch to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> BatchQueryAsync<TEntity>(this IDbConnection connection, QueryGroup where, int page, int rowsPerBatch,
            IEnumerable<OrderField> orderBy, int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BatchQuery<TEntity>(connection: connection,
                    where: where,
                    page: page,
                    rowsPerBatch: rowsPerBatch,
                    orderBy: orderBy,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // BulkInsert

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int BulkInsert<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, int? commandTimeout = null, ITrace trace = null)
            where TEntity : DataEntity
        {
            // Validate, only supports SqlConnection
            if (connection.GetType() != typeof(System.Data.SqlClient.SqlConnection))
            {
                throw new NotSupportedException("The bulk-insert is only applicable for SQL Server database connection.");
            }

            // Check
            GuardBulkInsert<TEntity>();

            // Variables
            var command = Command.BulkInsert;

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, command.ToString(), entities, null);
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
            using (var reader = new DataEntityListDataReader<TEntity>(entities, command))
            {
                using (var sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy((System.Data.SqlClient.SqlConnection)connection))
                {
                    sqlBulkCopy.DestinationTableName = DataEntityExtension.GetMappedName<TEntity>(command);
                    if (commandTimeout != null && commandTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                    }
                    reader.Properties.ToList().ForEach(property =>
                    {
                        var columnName = property.GetMappedName();
                        sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                    });
                    connection.EnsureOpen();
                    sqlBulkCopy.WriteToServer(reader);
                    result = reader.RecordsAffected;
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterBulkInsert(new TraceLog(null, command.ToString(), entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // BulkInsertAsync

        /// <summary>
        /// Bulk-inserting the list of <i>DataEntity</i> objects in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entities">The list of the <i>Data Entities</i> to be bulk-inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> BulkInsertAsync<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, int? commandTimeout = null, ITrace trace = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                BulkInsert(connection: connection,
                    entities: entities,
                    commandTimeout: commandTimeout,
                    trace: trace));
        }

        // Count

        /// <summary>
        /// Counts the number of rows from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public static long Count<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection, object where, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return Count<TEntity>(connection: connection,
                where: queryGroup,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static long Count<TEntity>(this IDbConnection connection, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardCountable<TEntity>();

            // Variables
            var command = Command.Count;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateCount(new QueryBuilder<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = Convert.ToInt64(ExecuteScalar(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction));

            // After Execution
            if (trace != null)
            {
                trace.AfterCount(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // CountAsync

        /// <summary>
        /// Counts the number of rows from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(connection: connection,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, object where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Counts the number of rows from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An integer value for the number of rows counted from the database based on the given query expression.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Count<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // Delete

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation. When is set to <i>NULL</i>, it deletes all the data from the database.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, object where, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            GetAndGuardPrimaryKey<TEntity>(Command.Delete);
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return Delete<TEntity>(connection: connection,
                where: queryGroup,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Delete<TEntity>(this IDbConnection connection, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletable<TEntity>();

            // Variables
            var command = Command.Delete;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateDelete(new QueryBuilder<TEntity>(), where);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterDelete(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // DeleteAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(connection: connection,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection, object where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Deletes a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Delete<TEntity>(connection: connection,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // DeleteAll

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int DeleteAll<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardDeletableAll<TEntity>();

            // Variables
            var command = Command.DeleteAll;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateDeleteAll(new QueryBuilder<TEntity>());

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, null, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterDeleteAll(new TraceLog(null, commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // DeleteAllAsync

        /// <summary>
        /// Deletes all data in the database based on the target <i>DataEntity</i> in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                DeleteAll<TEntity>(connection: connection,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // InlineInsert

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public static object InlineInsert<TEntity>(this IDbConnection connection, object entity, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInlineInsertable<TEntity>();

            // Variables
            var command = Command.InlineInsert;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = IsPrimaryIdentityCache.Get<TEntity>(connection.ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)statementBuilder).CreateInlineInsert(new QueryBuilder<TEntity>(), entity?.AsFields(),
                        overrideIgnore, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateInlineInsert(new QueryBuilder<TEntity>(), entity?.AsFields(), overrideIgnore);
                }
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, entity, null);
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
            var result = ExecuteScalar(connection: connection,
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
                trace.AfterInlineInsert(new TraceLog(null, commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineInsertAsync

        /// <summary>
        /// Inserts a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The object that contains the targetted columns to be inserted.</param>
        /// <param name="overrideIgnore">True if to allow the insert operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public static Task<object> InlineInsertAsync<TEntity>(this IDbConnection connection, object entity, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineInsert<TEntity>(connection: connection,
                    entity: entity,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // InlineMerge

        /// <summary>
        /// Merges a data in the database by targetting certain fields only. It uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be merged.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection, object entity, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineMerge<TEntity>(this IDbConnection connection, object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInlineMergeable<TEntity>();

            // Variables
            var command = Command.InlineMerge;
            var entityProperties = entity?.GetType().GetTypeInfo().GetProperties();

            // Force to use the PrimaryKey
            if (qualifiers == null)
            {
                var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
                var hasError = (primary != null) && (entityProperties?.Any(property => property.Name.ToLower() == primary?.GetMappedName().ToLower()) == false);
                if (hasError)
                {
                    throw new PrimaryFieldNotFoundException($"Merge operation could proceed with missing primary key. Either specify a qualifier or " +
                        $"include the primary key in the dynamic entity.");
                }
            }

            // All qualifiers must be present in the dynamic entity
            var missingFields = qualifiers?.Where(qualifier => entityProperties.FirstOrDefault(property =>
                property.GetMappedName().ToLower() == qualifier.Name.ToLower()) == null);
            if (missingFields?.Count() > 0)
            {
                throw new MissingFieldException($"All qualifier fields must be presented in the given dynamic entity object. " +
                    $"The missing field(s) are {missingFields.Select(f => f.AsField()).Join(", ")}.");
            }

            // Other variables
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = IsPrimaryIdentityCache.Get<TEntity>(connection.ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)statementBuilder).CreateInlineMerge(new QueryBuilder<TEntity>(), entity?.AsFields(),
                        qualifiers, overrideIgnore, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateInlineMerge(new QueryBuilder<TEntity>(), entity?.AsFields(), qualifiers,
                        overrideIgnore);
                }
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, entity, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: entity,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineMerge(new TraceLog(null, commandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineMergeAsync

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an asynchronous way. Uses the <i>PrimaryKey</i> as the default qualifier field.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be merged.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection, object entity, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineMerge<TEntity>(connection: connection,
                    entity: entity,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Merges a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be merged.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">True if to allow the merge operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineMergeAsync<TEntity>(this IDbConnection connection, object entity, IEnumerable<Field> qualifiers, bool? overrideIgnore = false,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineMerge<TEntity>(connection: connection,
                    entity: entity,
                    qualifiers: qualifiers,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // InlineUpdate

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection, object entity, object where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: queryGroup,
                overrideIgnore: overrideIgnore,
                commandTimeout: commandTimeout,
                transaction: transaction);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection, object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return InlineUpdate<TEntity>(connection: connection,
                entity: entity,
                where: new QueryGroup(where),
                overrideIgnore: overrideIgnore,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int InlineUpdate<TEntity>(this IDbConnection connection, object entity, QueryGroup where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInlineUpdateable<TEntity>();

            // Variables
            var command = Command.InlineUpdate;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateInlineUpdate(new QueryBuilder<TEntity>(),
                entity.AsFields(), where, overrideIgnore);
            var param = entity?.Merge(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterInlineUpdate(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InlineUpdateAsync

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection, object entity, object where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(connection: connection,
                    entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection, object entity, IEnumerable<QueryField> where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(connection: connection,
                    entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Updates a data in the database by targetting certain fields only in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The dynamic <i>DataEntity</i> object that contains the targetted columns to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="overrideIgnore">True if to allow the update operation on the properties with <i>RepoDb.Attributes.IgnoreAttribute</i> defined.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> InlineUpdateAsync<TEntity>(this IDbConnection connection, object entity, QueryGroup where, bool? overrideIgnore = false, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                InlineUpdate<TEntity>(connection: connection,
                    entity: entity,
                    where: where,
                    overrideIgnore: overrideIgnore,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // Insert

        /// <summary>
        /// Inserts a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null, IDbTransaction transaction = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardInsertable<TEntity>();

            // Variables
            var command = Command.Insert;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;
            var param = entity?.AsObject();

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = IsPrimaryIdentityCache.Get<TEntity>(connection.ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)statementBuilder).CreateInsert(new QueryBuilder<TEntity>(), isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateInsert(new QueryBuilder<TEntity>());
                }
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
                param = (cancellableTraceLog?.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = ExecuteScalar(connection: connection,
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
                trace.AfterInsert(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // InsertAsync

        /// <summary>
        /// Inserts a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The <i>DataEntity</i> object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>
        /// The value of the <i>PrimaryKey</i> of the newly inserted <i>DataEntity</i> object. Returns <i>NULL</i> if the 
        /// <i>PrimaryKey</i> property is not present.
        /// </returns>
        public static Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Insert(connection: connection,
                    entity: entity,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // Merge

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
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
        /// Merges an existing <i>DataEntity</i> object in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Merge<TEntity>(this IDbConnection connection, TEntity entity, IEnumerable<Field> qualifiers, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            var command = Command.Merge;

            // Check
            GuardMergeable<TEntity>();
            GetAndGuardPrimaryKey<TEntity>(command);

            // Variables
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = string.Empty;
            var param = entity?.AsObject();

            // Compose command text
            if (commandType == CommandType.StoredProcedure)
            {
                commandText = DataEntityExtension.GetMappedName<TEntity>(command);
            }
            else
            {
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    // Cache only if the 'isIdentity' is not defined, only for SQL Server
                    var isPrimaryIdentity = IsPrimaryIdentityCache.Get<TEntity>(connection.ConnectionString, command);
                    commandText = ((SqlDbStatementBuilder)statementBuilder).CreateMerge(new QueryBuilder<TEntity>(), qualifiers, isPrimaryIdentity);
                }
                else
                {
                    // Other Sql Data Providers
                    commandText = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateMerge(new QueryBuilder<TEntity>(), qualifiers);
                }
            }

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // MergeAsync

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way. By default, this operation uses the <i>PrimaryKey</i> property as
        /// the qualifier.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew<int>(() =>
                Merge(connection: connection,
                    entity: entity,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Merges an existing <i>DataEntity</i> object in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The entity to be merged.</param>
        /// <param name="qualifiers">
        /// The list of qualifer fields to be used during merge operation. The qualifers are the fields used when qualifying the condition
        /// (equation of the fields) of the source and destination tables.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> MergeAsync<TEntity>(this IDbConnection connection, TEntity entity, IEnumerable<Field> qualifiers, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Merge(connection: connection,
                    entity: entity,
                    qualifiers: qualifiers,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // Query

        /// <summary>
        /// Query a data from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, int? commandTimeout = null,
            IDbTransaction transaction = null, ICache cache = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null) where TEntity : DataEntity
        {
            return Query<TEntity>(connection: connection,
                where: (QueryGroup)null,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, ICache cache = null,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null,
             IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null) where TEntity : DataEntity
        {
            return Query<TEntity>(connection: connection,
                where: where != null ? new QueryGroup(where) : null,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection, object where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, ICache cache = null,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null) where TEntity : DataEntity
        {
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return Query<TEntity>(connection: connection,
                where: queryGroup,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection, QueryGroup where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, int? commandTimeout = null,
            IDbTransaction transaction = null, ICache cache = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null)
            where TEntity : DataEntity
        {
            return QueryData<TEntity>(connection: connection,
                where: where,
                top: top,
                orderBy: orderBy,
                cacheKey: cacheKey,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                recursive: recursive,
                recursionDepth: recursionDepth);
        }

        /// <summary>
        /// Query a data from the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        private static IEnumerable<TEntity> QueryData<TEntity>(this IDbConnection connection, QueryGroup where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, int? commandTimeout = null,
            IDbTransaction transaction = null, ICache cache = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null)
            where TEntity : DataEntity
        {
            // Get Cache
            if (cacheKey != null)
            {
                var item = cache?.Get(cacheKey);
                if (item != null)
                {
                    return (IEnumerable<TEntity>)item;
                }
            }

            // Check
            GuardQueryable<TEntity>();

            // Variables
            var command = Command.Query;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateQuery(new QueryBuilder<TEntity>(), where, top, orderBy);
            var param = where?.AsObject();

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteQuery<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterQuery(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Set Cache
            if (cacheKey != null && result?.Any() == true)
            {
                cache?.Add(cacheKey, result);
            }

            // Check the recursiveness
            if (recursive == true && result?.Any() == true && (recursionDepth == null || recursionDepth >= 1))
            {
                var what = DataEntityExtension.GetDataEntityChildrenData<TEntity>();

                // Recurse only if we have children
                if (what != null && what.Any() == true)
                {
                    QueryChildData<TEntity>(connection: connection,
                        what: what,
                        result: result,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        cache: cache,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        recursive: recursive,
                        recursionDepth: recursionDepth > Constant.RecursiveMaxRecursion ?
                            Constant.RecursiveMaxRecursion : recursionDepth);
                }
            }

            // Result
            return result;
        }

        /// <summary>
        /// Query a data from the database based on the given query expression recursively.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="what">The list of the children data list to be queried.</param>
        /// <param name="result">The result from previous query of the parent <i>DataEntity</i> objects.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        private static void QueryChildData<TEntity>(this IDbConnection connection, IEnumerable<DataEntityChildListData> what, IEnumerable<TEntity> result,
            int? commandTimeout = null, IDbTransaction transaction = null, ICache cache = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null)
            where TEntity : DataEntity
        {
            // Variables for recursive
            var command = Command.Query;
            var primary = GetAndGuardPrimaryKey<TEntity>(command);
            var entityName = DataEntityExtension.GetMappedName<TEntity>(command).AsUnquoted();
            var primaryKey = primary.GetMappedName().AsUnquoted();
            var foreignKey = $"{entityName}{primaryKey}";

            // Split the list
            var childItemDataList = result.Select(entity => new DataEntityChildItemData(entity)).ToList();
            var splittedList = new List<IEnumerable<DataEntityChildItemData>>();
            for (var index = 0; index < childItemDataList.Count; index += Constant.RecursiveQueryBatchCount)
            {
                splittedList.Add(childItemDataList.Skip(index).Take(Constant.RecursiveQueryBatchCount));
            }

            // Iterate the splitted list
            splittedList.ForEach(list =>
            {
                // Iterate the list of recursive data
                what?.ToList().ForEach(recursiveData =>
                {
                    // Check if the property can be written
                    if (recursiveData.ChildListProperty.CanWrite == false)
                    {
                        throw new InvalidOperationException($"Property '{recursiveData.ChildListProperty.Name}' from type '{recursiveData.ParentDataEntityType.FullName}' is read-only.");
                    }

                    // Set the query expresssion
                    var foreignAttribute = recursiveData.ChildListProperty.GetCustomAttribute<Attributes.ForeignAttribute>();
                    var parentFieldName = foreignAttribute?.ParentFieldName ?? primaryKey;
                    var childFieldName = foreignAttribute?.ChildFieldName ?? foreignKey;

                    // Check for the parent property
                    var parentFieldProperty = (PropertyInfo)null;
                    if (parentFieldName != primaryKey)
                    {
                        parentFieldProperty = recursiveData.ParentDataEntityType.GetTypeInfo().GetProperty(parentFieldName);
                        if (parentFieldProperty == null)
                        {
                            throw new MissingFieldException($"Parent property '{parentFieldName}' from type '{recursiveData.ParentDataEntityType.FullName}' is not found.");
                        }
                    }
                    parentFieldProperty = (parentFieldProperty ?? primary);

                    // Check for the foreign property
                    var foreignProperty = recursiveData.ChildListType.GetTypeInfo().GetProperty(childFieldName.AsUnquoted());
                    if (foreignProperty == null)
                    {
                        throw new MissingFieldException($"Foreign property '{childFieldName}' from type '{recursiveData.ChildListType.FullName}' is not found.");
                    }

                    // Set the context with the given keys
                    list.ToList().ForEach(item => item.Key = parentFieldProperty.GetValue(item.DataEntity));
                    var context = new QueryGroup(new QueryField(childFieldName, Operation.In, list.Select(item => item.Key).ToArray()).AsEnumerable());

                    // Parameters
                    var parameters = new object[]
                    {
                        connection, // connection
                        context, // where
                        null, // top
                        null, // orderBy
                        null, // cacheKey
                        commandTimeout, // commandTimeout
                        transaction, // transaction
                        cache, // cache
                        trace, // trace
                        statementBuilder, // statementBuilder
                        recursive, // recursive,
                        (recursionDepth - 1) // recursionDepth
                    };

                    // Get the method and query the data
                    var bindings = (BindingFlags.Static | BindingFlags.NonPublic);
                    var method = typeof(DbConnectionExtension).GetTypeInfo().GetMethod("QueryData", bindings).MakeGenericMethod(recursiveData.ChildListType);
                    var recursiveResult = Enumerable.OfType<DataEntity>((IEnumerable)method.Invoke(connection, parameters)).ToList();

                    // Break the current iteration if there is no result
                    if (recursiveResult.Any() == false)
                    {
                        return;
                    }

                    // Iterate the current result
                    list.ToList().ForEach(item =>
                    {
                        // Create a list
                        var enumerableType = typeof(List<>).MakeGenericType(recursiveData.ChildListType);
                        var childList = Activator.CreateInstance(enumerableType);
                        var addMethod = enumerableType.GetTypeInfo().GetMethod("Add");

                        // Extreme reflection, need to optimize soon
                        var childEntities = recursiveResult
                            .Where(entity => item.Key.Equals(foreignProperty.GetValue(entity)))
                            .ToList();

                        // Iterate each child entity
                        childEntities?.ForEach(childEntity =>
                        {
                            addMethod.Invoke(childList, new[] { childEntity });
                        });

                        // Set back the value
                        recursiveData.ChildListProperty.SetValue(item.DataEntity, childList);
                    });
                });
            });
        }

        // QueryAsync

        /// <summary>
        /// Query a data from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, int? commandTimeout = null,
            IDbTransaction transaction = null, ICache cache = null, ITrace trace = null,
             IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null) where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(connection: connection,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    cache: cache,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    recursive: recursive,
                    recursionDepth: recursionDepth));
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection, IEnumerable<QueryField> where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, ICache cache = null, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(connection: connection,
                    where: where,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    cache: cache,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    recursive: recursive,
                    recursionDepth: recursionDepth));
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection, object where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, string cacheKey = null, ICache cache = null,
            int? commandTimeout = null, IDbTransaction transaction = null, ITrace trace = null,
            IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null) where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(connection: connection,
                    where: where,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    cache: cache,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    recursive: recursive,
                    recursionDepth: recursionDepth));
        }

        /// <summary>
        /// Query a data from the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="top">The top number of rows to be used by this operation.</param>
        /// <param name="orderBy">The order definition of the fields to be used by this operation.</param>
        /// <param name="cacheKey">
        /// The key to the cache. If the cache key is present in the cache, then the item from the cache will be returned instead. Setting this
        /// to <i>NULL</i> would force to query from the database.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="cache">The cache object to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <param name="recursive">
        /// The value that indicates whether the child <i>DataEntity</i> objects defined in the target <i>DataEntity</i> object will
        /// be included in the result of the query. The default value is <i>False</i>.
        /// </param>
        /// <param name="recursionDepth">
        /// Defines the depth of the recursion when querying the data from the database. By default, the value is <i>NULL</i> to enable the querying of all 
        /// child data entities defined on the targetted <i>DataEntity</i>. Maximum recursion of 15 cycles only to avoid cyclomatic overflow operation.
        /// </param>
        /// <returns>An enumerable list of <i>DataEntity</i> object.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection, QueryGroup where, int? top = 0,
            IEnumerable<OrderField> orderBy = null, int? commandTimeout = null, IDbTransaction transaction = null, string cacheKey = null,
            ICache cache = null, ITrace trace = null, IStatementBuilder statementBuilder = null, bool? recursive = false, int? recursionDepth = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Query<TEntity>(connection: connection,
                    where: where,
                    top: top,
                    orderBy: orderBy,
                    cacheKey: cacheKey,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    cache: cache,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    recursive: recursive,
                    recursionDepth: recursionDepth));
        }

        // Truncate

        /// <summary>
        /// Truncates a table from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        public static void Truncate<TEntity>(this IDbConnection connection, int? commandTimeout = null, ITrace trace = null, IStatementBuilder statementBuilder = null) where TEntity : DataEntity
        {
            // Check
            GuardTruncatable<TEntity>();

            // Variables
            var command = Command.Truncate;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateTruncate(new QueryBuilder<TEntity>());

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, null, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: null,
                commandType: commandType,
                commandTimeout: commandTimeout);

            // After Execution
            if (trace != null)
            {
                trace.AfterTruncate(new TraceLog(null, commandText, null, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }
        }

        // TruncateAsync

        /// <summary>
        /// Truncates a table from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        public static Task TruncateAsync<TEntity>(this IDbConnection connection, int? commandTimeout = null,
            ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Truncate<TEntity>(connection: connection,
                    commandTimeout: commandTimeout,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        // Update

        /// <summary>
        /// Updates a data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            var property = GetAndGuardPrimaryKey<TEntity>(Command.Update);
            return Update(connection: connection,
                entity: entity,
                where: new QueryGroup(property.AsQueryField(entity, true).AsEnumerable()),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entity, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Update(connection: connection,
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
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entity, object where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {

            GetAndGuardPrimaryKey<TEntity>(Command.Update);
            var queryGroup = WhereToQueryGroup<TEntity>(where);
            return Update(connection: connection,
                entity: entity,
                where: queryGroup,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entity, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            // Check
            GuardUpdateable<TEntity>();

            // Variables
            var command = Command.Update;
            var commandType = DataEntityExtension.GetCommandType<TEntity>(command);
            if (commandType != CommandType.StoredProcedure)
            {
                // Append prefix to all parameters for non StoredProcedure (this is mappable, that's why)
                where.AppendParametersPrefix();
            }
            var commandText = commandType == CommandType.StoredProcedure ?
                DataEntityExtension.GetMappedName<TEntity>(command) :
                (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder()).CreateUpdate(new QueryBuilder<TEntity>(), where);
            var param = entity?.AsObject(where);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(null, commandText, param, null);
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
            var result = ExecuteNonQuery(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction);

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(null, commandText, param, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Result
            return result;
        }

        // UpdateAsync

        /// <summary>
        /// Updates a data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(connection: connection,
                    entity: entity,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, IEnumerable<QueryField> where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(connection: connection,
                    entity: entity,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression or primary key value to be used by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, object where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(connection: connection,
                    entity: entity,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        /// <summary>
        /// Updates a data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <i>DataEntity</i> object.</typeparam>
        /// <param name="connection">The connection object to be used by this operation.</param>
        /// <param name="entity">The instance of <i>DataEntity</i> object to be updated.</param>
        /// <param name="where">The query expression to be used  by this operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used by this operation.</param>
        /// <param name="trace">The trace object to be used by this operation.</param>
        /// <param name="statementBuilder">The statement builder object to be used by this operation.</param>
        /// <returns>An instance of integer that holds the number of rows affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, QueryGroup where, int? commandTimeout = null,
            IDbTransaction transaction = null, ITrace trace = null, IStatementBuilder statementBuilder = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                Update(connection: connection,
                    entity: entity,
                    where: where,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder));
        }

        #endregion

        #region Execute Commands

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public static IEnumerable<dynamic> ExecuteQuery(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                using (var reader = command.ExecuteReader())
                {
                    //var result = reader.AsEnumerable();
                    return DataReaderConverter.ToEnumerable((DbDataReader)reader);
                }
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of dynamic objects.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of dynamic objects containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public static Task<IEnumerable<object>> ExecuteQueryAsync(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return Task.Factory.StartNew(() =>
                ExecuteQuery(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Actual Execution
            using (var reader = ExecuteReader(connection, commandText, param, commandType, commandTimeout, transaction))
            {
                return DataReaderConverter.ToEnumerable<TEntity>((DbDataReader)reader);
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// converts the result back to an enumerable list of <i>DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity to convert to.</typeparam>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <returns>
        /// An enumerable list of <i>DataEntity</i> object containing the converted results of the underlying <i>System.Data.IDataReader</i> object.
        /// </returns>
        public static Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this IDbConnection connection,
            string commandText,
            object param = null,
            CommandType? commandType = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
            where TEntity : DataEntity
        {
            return Task.Factory.StartNew(() =>
                ExecuteQuery<TEntity>(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
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
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteReader</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the instance of the data reader.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
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
            return Task.Factory.StartNew(() =>
                ExecuteReader(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteNonQuery</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
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
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a query from the database in asynchronous way. It uses the underlying <i>ExecuteNonQuery</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the number of affected rows during the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
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
            return Task.Factory.StartNew<int>(() =>
                ExecuteNonQuery(connection: connection,
                    commandText: commandText,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout,
                    transaction: transaction));
        }

        /// <summary>
        /// Executes a query from the database. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
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
            // Check Transaction
            ValidateTransactionConnectionObject(connection, transaction);

            // Actual Execution
            using (var command = connection.EnsureOpen().CreateCommand(commandText, commandType, commandTimeout, transaction))
            {
                command.CreateParameters(param);
                return ObjectConverter.DbNullToNull(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Executes a query from the database in an asynchronous way. It uses the underlying <i>ExecuteScalar</i> method of the <i>System.Data.IDataReader</i> object and
        /// returns the first occurence value (first column of first row) of the execution.
        /// </summary>
        /// <param name="connection">The connection to be used during execution.</param>
        /// <param name="commandText">The command text to be used on the execution.</param>
        /// <param name="param">
        /// The dynamic object to be used as parameter. This object must contain all the values for all the parameters
        /// defined in the <i>CommandText</i> property.
        /// </param>
        /// <param name="commandType">The command type to be used on the execution.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used on the execution.</param>
        /// <param name="transaction">The transaction to be used on the execution (if present).</param>
        /// <param name="trace">The trace object to be used on the execution (if present).</param>
        /// <returns>An object that holds the first occurence value (first column of first row) of the execution.</returns>
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

        #endregion
    }
}