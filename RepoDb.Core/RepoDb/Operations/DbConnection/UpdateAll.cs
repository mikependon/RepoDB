using RepoDb.Contexts.Providers;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region UpdateAll<TEntity>

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static int UpdateAllInternal<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            if (qualifiers?.Any() != true)
            {
                var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                    entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity));
                qualifiers = key.AsEnumerable();
            }
            if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
            {
                return UpdateAllInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entities: entities?.WithType<IDictionary<string, object>>(),
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
            else
            {
                return UpdateAllInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: fields,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static async Task<int> UpdateAllAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (qualifiers?.Any() != true)
            {
                var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                    entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity), cancellationToken);
                qualifiers = key.AsEnumerable();
            }
            if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
            {
                return await UpdateAllAsyncInternalBase<IDictionary<string, object>>(connection: connection,
                    tableName: tableName,
                    entities: entities?.WithType<IDictionary<string, object>>(),
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
            else
            {
                return await UpdateAllAsyncInternalBase<TEntity>(connection: connection,
                    tableName: tableName,
                    entities: entities,
                    batchSize: batchSize,
                    fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region UpdateAll(TableName)

        /// <summary>
        /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAllInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAllInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static int UpdateAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAllInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAllAsync(TableName)

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAllAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAllAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way. By default, the table fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of dynamic objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        public static Task<int> UpdateAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return UpdateAllAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region UpdateAllInternalBase<TEntity>

        /// <summary>
        /// Update the existing rows in the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be updated.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be updated.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static int UpdateAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            if (entities?.Any() != true)
            {
                return default;
            }

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the context
            var context = UpdateAllExecutionContextProvider.Create<TEntity>(connection,
                tableName,
                entities,
                qualifiers,
                batchSize,
                fields,
                hints,
                transaction,
                statementBuilder);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, context.CommandText, entities, null);
                trace.BeforeUpdateAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return 0;
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = 0;

            // Make sure to create transaction if there is no passed one
            var hasTransaction = (transaction != null || Transaction.Current != null);

            try
            {
                // Ensure the connection is open
                connection.EnsureOpen();

                if (hasTransaction == false)
                {
                    // Create a transaction
                    transaction = connection.BeginTransaction();
                }

                // Create the command
                using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
                    CommandType.Text, commandTimeout, transaction))
                {
                    // Directly execute if the entities is only 1 (performance)
                    if (batchSize == 1)
                    {
                        // Much better to use the actual single-based setter (performance)
                        foreach (var entity in entities.AsList())
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        foreach (var batchEntities in entities.AsList().Split(batchSize))
                        {
                            var batchItems = batchEntities.AsList();

                            // Break if there is no more records
                            if (batchItems.Count <= 0)
                            {
                                break;
                            }

                            // Check if the batch size has changed (probably the last batch on the enumerables)
                            if (batchItems.Count != batchSize)
                            {
                                // Get a new execution context from cache
                                context = UpdateAllExecutionContextProvider.Create<TEntity>(connection,
                                    tableName,
                                    batchItems,
                                    qualifiers,
                                    batchItems.Count,
                                    fields,
                                    hints,
                                    transaction,
                                    statementBuilder);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += command.ExecuteNonQuery();
                        }
                    }
                }

                if (hasTransaction == false)
                {
                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    // Rollback for any exception
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    // Rollback and dispose the transaction
                    transaction.Dispose();
                }
            }

            // After Execution
            trace?.AfterUpdateAll(new TraceLog(sessionId, context.CommandText, entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion

        #region UpdateAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be updated.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="fields">The list of <see cref="Field"/> objects to be updated.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the update process.</returns>
        internal static async Task<int> UpdateAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            if (entities?.Any() != true)
            {
                return default;
            }

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the context
            var context = await UpdateAllExecutionContextProvider.CreateAsync<TEntity>(connection,
                tableName,
                entities,
                qualifiers,
                batchSize,
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
                var cancellableTraceLog = new CancellableTraceLog(sessionId, context.CommandText, entities, null);
                trace.BeforeUpdateAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return 0;
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entities = (IEnumerable<TEntity>)(cancellableTraceLog.Parameter ?? entities);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = 0;

            // Make sure to create transaction if there is no passed one
            var hasTransaction = (transaction != null || Transaction.Current != null);

            try
            {
                // Ensure the connection is open
                await connection.EnsureOpenAsync(cancellationToken);

                if (hasTransaction == false)
                {
                    // Create a transaction
                    transaction = connection.BeginTransaction();
                }

                // Create the command
                using (var command = (DbCommand)connection.CreateCommand(context.CommandText,
                    CommandType.Text, commandTimeout, transaction))
                {
                    // Directly execute if the entities is only 1 (performance)
                    if (batchSize == 1)
                    {
                        // Much better to use the actual single-based setter (performance)
                        foreach (var entity in entities.AsList())
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc?.Invoke(command, entity);

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += await command.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
                    else
                    {
                        foreach (var batchEntities in entities.AsList().Split(batchSize))
                        {
                            var batchItems = batchEntities.AsList();

                            // Break if there is no more records
                            if (batchItems.Count <= 0)
                            {
                                break;
                            }

                            // Check if the batch size has changed (probably the last batch on the enumerables)
                            if (batchItems.Count != batchSize)
                            {
                                // Get a new execution context from cache
                                context = await UpdateAllExecutionContextProvider.CreateAsync<TEntity>(connection,
                                    tableName,
                                    batchItems,
                                    qualifiers,
                                    batchItems.Count,
                                    fields,
                                    hints,
                                    transaction,
                                    statementBuilder,
                                    cancellationToken);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc?.Invoke(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc?.Invoke(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += await command.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
                }

                if (hasTransaction == false)
                {
                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch
            {
                if (hasTransaction == false)
                {
                    // Rollback for any exception
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (hasTransaction == false)
                {
                    // Rollback and dispose the transaction
                    transaction.Dispose();
                }
            }

            // After Execution
            trace?.AfterUpdateAll(new TraceLog(sessionId, context.CommandText, entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion
    }
}
