using RepoDb.Contexts.Providers;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
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
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                entity?.GetType() ?? typeof(TEntity));
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(key, entity),
                fields: fields,
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
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity, TWhat>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
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
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                fields: fields,
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
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
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
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
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
            return UpdateInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields,
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
        /// <param name="entity">The data entity object to be used for update.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity, TWhat>(this IDbConnection connection,
            TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
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
                where: WhatToQueryGroup<TEntity>(connection, what, transaction),
                fields: fields,
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
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
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
                where: WhatToQueryGroup<TEntity>(connection, what, transaction),
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
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
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update<TEntity>(this IDbConnection connection,
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
            return UpdateInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where,
                fields: fields,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static int UpdateInternal<TEntity>(this IDbConnection connection,
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
            if (entity?.GetType().IsDictionaryStringObject() == true)
            {
                return UpdateInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entity: (IDictionary<string, object>)entity,
                    where: where,
                    fields: GetQualifiedFields<TEntity>(fields, entity),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
            else
            {
                return UpdateInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    where: where,
                    fields: GetQualifiedFields<TEntity>(fields, entity),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region UpdateAsync<TEntity>

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                entity?.GetType() ?? typeof(TEntity), cancellationToken);
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(key, entity),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where != null ? new QueryGroup(where.AsEnumerable()) : null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync<TEntity>(connection, transaction, cancellationToken);
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup<TEntity>(key, entity),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity, TWhat>(this IDbConnection connection,
            TEntity entity,
            TWhat what,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: await WhatToQueryGroupAsync<TEntity>(connection, what, transaction, cancellationToken),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            object what,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: await WhatToQueryGroupAsync<TEntity>(connection, what, transaction, cancellationToken),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table based on the given query expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be updated.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static Task<int> UpdateAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity?.GetType().IsDictionaryStringObject() == true)
            {
                return UpdateAsyncInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entity: (IDictionary<string, object>)entity,
                    where: where,
                    fields: GetQualifiedFields<TEntity>(fields, entity),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
            else
            {
                return UpdateAsyncInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    where: where,
                    fields: GetQualifiedFields<TEntity>(fields, entity),
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Update(TableName)

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction, entity?.GetType());
            return UpdateInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(key, entity),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            object where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                trace: trace,
                statementBuilder: statementBuilder,
                transaction: transaction);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates an existing row in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int Update(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAsync(TableName)

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static async Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                entity?.GetType(), cancellationToken);
            return await UpdateAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(key, entity),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            object where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            QueryField where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: ToQueryGroup(where),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be used for update.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAsync(this IDbConnection connection,
            string tableName,
            object entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                where: where,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

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
        /// <returns>The number of affected rows during the update process.</returns>
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
                WhereToCommandParameters(command, where, entity?.GetType(),
                    DbFieldCache.Get(connection, tableName, transaction));

                // Actual Execution
                result = command.ExecuteNonQuery();
            }

            // After Execution
            trace?.AfterUpdate(new TraceLog(sessionId, context.CommandText, entity, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

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
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static async Task<int> UpdateAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
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
                statementBuilder,
                cancellationToken);
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
            using (var command = (DbCommand)(await connection.EnsureOpenAsync(cancellationToken)).CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Add the fields from the query group
                WhereToCommandParameters(command, where, entity?.GetType(),
                    await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken));

                // Actual Execution
                result = await command.ExecuteNonQueryAsync(cancellationToken);
            }

            // After Execution
            trace?.AfterUpdate(new TraceLog(sessionId, context.CommandText, entity, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion
    }
}
