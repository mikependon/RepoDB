using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Update<TEntity>

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
                where: ToQueryGroup(property, entity),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrPrimaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return Update<TEntity>(connection: connection,
                entity: entity,
                where: WhereOrPrimaryKeyToQueryGroup<TEntity>(whereOrPrimaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                entity.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText(request);
            var param = entity?.AsMergedObject(where);

            // Return the result
            return UpdateInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Update an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
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
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrPrimaryKey,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKey<TEntity>();
            return UpdateAsync<TEntity>(connection: connection,
                entity: entity,
                where: WhereOrPrimaryKeyToQueryGroup<TEntity>(whereOrPrimaryKey),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
        /// Update an existing data in the database based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
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
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static Task<int> UpdateAsyncInternal<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables
            var request = new UpdateRequest(typeof(TEntity),
                connection,
                where,
                entity.AsFields(),
                statementBuilder);
            var commandText = CommandTextCache.GetUpdateText(request);
            var param = entity?.AsMergedObject(where);

            // Return the result
            return UpdateAsyncInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region Update(TableName)

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            object where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Update(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Update(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Update(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateInternal(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static int UpdateInternal(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new UpdateRequest(tableName,
                connection,
                where,
                entity?.AsFields(),
                statementBuilder);
            var param = entity?.Merge(where);

            // Return the result
            return UpdateInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region UpdateAsync(TableName)

        /// <summary>
        /// Update an existing data in the database in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The dynamic expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            object where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Update an existing data in the database in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            QueryField where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<QueryField> where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAsyncInternal(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update an existing data in the database in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static Task<int> UpdateAsyncInternal(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables
            var request = new UpdateRequest(tableName,
                connection,
                where,
                entity?.AsFields(),
                statementBuilder);
            var param = entity?.Merge(where);

            // Return the result
            return UpdateAsyncInternalBase(connection: connection,
                request: request,
                param: param,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace);
        }

        #endregion

        #region UpdateInternalBase

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="UpdateRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static int UpdateInternalBase(this IDbConnection connection,
            UpdateRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetUpdateText(request);

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
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
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

        #region UpdateAsyncInternalBase

        /// <summary>
        /// Updates an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="request">The actual <see cref="UpdateRequest"/> object.</param>
        /// <param name="param">The mapped object parameters.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>An instance of integer that holds the number of data affected by the execution.</returns>
        internal static async Task<int> UpdateAsyncInternalBase(this IDbConnection connection,
            UpdateRequest request,
            object param,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null)
        {
            // Variables
            var commandType = CommandType.Text;
            var commandText = CommandTextCache.GetUpdateText(request);

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
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            var result = await ExecuteNonQueryAsyncInternal(connection: connection,
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
    }
}
