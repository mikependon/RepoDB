using RepoDb.Contexts.Execution;
using RepoDb.Contexts.Providers;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        #region Update<TEntity>(TableName)

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The entity object to be used for update.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(connection, tableName, transaction);
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup<TEntity>(key, entity),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var where = WhereOrPrimaryKeyToQueryGroup(whereOrPrimaryKey);
            if (where == null)
            {
                var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction);
                where = WhereOrPrimaryKeyToQueryGroup(key, whereOrPrimaryKey);
            }
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region Update<TEntity>

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The entity object to be used for update.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(connection, transaction);
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup<TEntity>(key, entity),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            GetAndGuardPrimaryKeyOrIdentityKey<TEntity>(connection, transaction);
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: WhereOrPrimaryKeyToQueryGroup<TEntity>(connection, whereOrPrimaryKey, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        internal static int UpdateInternal<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: entity.AsFields(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region Update<TEntity>(TableName)

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, tableName, transaction);
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup<TEntity>(key, entity),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var where = WhereOrPrimaryKeyToQueryGroup(whereOrPrimaryKey);
            if (where == null)
            {
                var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction);
                where = WhereOrPrimaryKeyToQueryGroup(key, whereOrPrimaryKey);
            }
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, transaction);
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup<TEntity>(key, entity),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="whereOrPrimaryKey">The dynamic expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            object whereOrPrimaryKey,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, transaction);
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: await WhereOrPrimaryKeyToQueryGroupAsync<TEntity>(connection, whereOrPrimaryKey, transaction),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        internal static Task<int> UpdateAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Return the result
            return UpdateAsyncInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: entity.AsFields(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region Update(TableName)

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static int Update(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    // Variables needed
        //    var primary = DbFieldCache.Get(connection, tableName, transaction)?.FirstOrDefault(dbField => dbField.IsPrimary);
        //    var where = DataEntityToPrimaryKeyQueryGroup(entity, primary?.Name);

        //    // Execute the proper method
        //    return UpdateInternal(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        trace: trace,
        //        statementBuilder: statementBuilder,
        //        transaction: transaction);
        //}

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static int Update(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    object whereOrPrimaryKey,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return Update(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: WhereOrPrimaryKeyToQueryGroup(connection, tableName, whereOrPrimaryKey, transaction),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        trace: trace,
        //        statementBuilder: statementBuilder,
        //        transaction: transaction);
        //}

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static int Update(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryField where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return Update(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: ToQueryGroup(where),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static int Update(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    IEnumerable<QueryField> where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return Update(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: ToQueryGroup(where),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static int Update(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryGroup where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return UpdateInternal(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //internal static int UpdateInternal(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryGroup where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    // Return the result
        //    return UpdateInternalBase<object>(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        fields: entity?.AsFields(),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        #endregion

        #region UpdateAsync(TableName)

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static async Task<int> UpdateAsync(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    // Variables needed
        //    var primary = (await DbFieldCache.GetAsync(connection, tableName, transaction))?.FirstOrDefault(dbField => dbField.IsPrimary);
        //    var where = DataEntityToPrimaryKeyQueryGroup(entity, primary?.Name);

        //    // Execute the proper method
        //    return await UpdateAsyncInternal(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        trace: trace,
        //        statementBuilder: statementBuilder,
        //        transaction: transaction);
        //}

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="whereOrPrimaryKey">The dynamic expression or the primary key value to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static async Task<int> UpdateAsync(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    object whereOrPrimaryKey,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return await UpdateAsync(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: await WhereOrPrimaryKeyToQueryGroupAsync(connection, tableName, whereOrPrimaryKey, transaction),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        trace: trace,
        //        statementBuilder: statementBuilder,
        //        transaction: transaction);
        //}

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static Task<int> UpdateAsync(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryField where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return UpdateAsync(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: ToQueryGroup(where),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static Task<int> UpdateAsync(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    IEnumerable<QueryField> where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return UpdateAsync(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: ToQueryGroup(where),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //public static Task<int> UpdateAsync(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryGroup where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    return UpdateAsyncInternal(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        ///// <summary>
        ///// Updates an existing row in the table in an asynchronous way.
        ///// </summary>
        ///// <param name="connection">The connection object to be used.</param>
        ///// <param name="tableName">The name of the target table to be used.</param>
        ///// <param name="entity">The dynamic object to be used for update.</param>
        ///// <param name="where">The query expression to be used.</param>
        ///// <param name="hints">The table hints to be used.</param>
        ///// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        ///// <param name="transaction">The transaction to be used.</param>
        ///// <param name="trace">The trace object to be used.</param>
        ///// <param name="statementBuilder">The statement builder object to be used.</param>
        ///// <returns>The number of affected rows during the update process..</returns>
        //internal static Task<int> UpdateAsyncInternal(this IDbConnection connection,
        //    string tableName,
        //    object entity,
        //    QueryGroup where,
        //    string hints = null,
        //    int? commandTimeout = null,
        //    IDbTransaction transaction = null,
        //    ITrace trace = null,
        //    IStatementBuilder statementBuilder = null)
        //{
        //    // Return the result
        //    return UpdateAsyncInternalBase<object>(connection: connection,
        //        tableName: tableName,
        //        entity: entity,
        //        where: where,
        //        fields: entity?.AsFields(),
        //        hints: hints,
        //        commandTimeout: commandTimeout,
        //        transaction: transaction,
        //        trace: trace,
        //        statementBuilder: statementBuilder);
        //}

        #endregion

        #region UpdateInternalBase<TEntity>

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        internal static int UpdateInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Set the flags
            where?.IsForUpdate();

            // Get the context
            var context = UpdateExecutionContextProvider.Create<TEntity>(connection,
                tableName,
                where,
                fields,
                hints,
                transaction,
                statementBuilder);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, context.CommandText, entity, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return 0;
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = 0;

            // Create the command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Add the fields from the query group
                WhereToCommandParameters(command, where);

                // Actual Execution
                result = command.ExecuteNonQuery();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(sessionId, context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region UpdateAsyncInternalBase<TEntity>

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process..</returns>
        internal static async Task<int> UpdateAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Set the flags
            where?.IsForUpdate();

            // Get the context
            var context = await UpdateExecutionContextProvider.CreateAsync<TEntity>(connection,
                tableName,
                where,
                fields,
                hints,
                transaction,
                statementBuilder);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, context.CommandText, entity, null);
                trace.BeforeUpdate(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return 0;
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = 0;

            // Create the command
            using (var command = (DbCommand)(await connection.EnsureOpenAsync()).CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Add the fields from the query group
                WhereToCommandParameters(command, where);

                // Actual Execution
                result = await command.ExecuteNonQueryAsync();
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterUpdate(new TraceLog(sessionId, context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
