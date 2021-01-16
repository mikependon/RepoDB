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
        #region MergeAll<TEntity>

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
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
            return MergeAllInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static int MergeAllInternal<TEntity>(this IDbConnection connection,
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
            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                    entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity));
                qualifiers = key.AsEnumerable();
            }

            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
                {
                    return MergeAllInternalBase<IDictionary<string, object>>(connection: connection,
                        tableName: tableName,
                        entities: entities?.WithType<IDictionary<string, object>>(),
                        qualifiers: qualifiers,
                        batchSize: batchSize,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    return MergeAllInternalBase<TEntity>(connection: connection,
                        tableName: tableName,
                        entities: entities,
                        qualifiers: qualifiers,
                        batchSize: batchSize,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
            }
            else
            {
                if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
                {
                    return UpsertAllInternalBase<IDictionary<string, object>>(connection: connection,
                        tableName: tableName,
                        entities: entities?.WithType<IDictionary<string, object>>(),
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    return UpsertAllInternalBase<TEntity>(connection: connection,
                        tableName: tableName,
                        entities: entities,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
            }
        }

        #endregion

        #region MergeAllAsync<TEntity>

        /// <summary>
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
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
            return MergeAllAsyncInternal<TEntity>(connection: connection,
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
        /// Insert multiple rows or update the existing rows in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static async Task<int> MergeAllAsyncInternal<TEntity>(this IDbConnection connection,
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
            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                    entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity), cancellationToken);
                qualifiers = key.AsEnumerable();
            }

            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
                {
                    return await MergeAllAsyncInternalBase<IDictionary<string, object>>(connection: connection,
                        tableName: tableName,
                        entities: entities?.WithType<IDictionary<string, object>>(),
                        qualifiers: qualifiers,
                        batchSize: batchSize,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    return await MergeAllAsyncInternalBase<TEntity>(connection: connection,
                        tableName: tableName,
                        entities: entities,
                        qualifiers: qualifiers,
                        batchSize: batchSize,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                if ((entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity)).IsDictionaryStringObject())
                {
                    return await UpsertAllAsyncInternalBase<IDictionary<string, object>>(connection: connection,
                        tableName: tableName,
                        entities: entities?.WithType<IDictionary<string, object>>(),
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    return await UpsertAllAsyncInternalBase<TEntity>(connection: connection,
                        tableName: tableName,
                        entities: entities,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entities?.FirstOrDefault()),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
            }
        }

        #endregion

        #region MergeAll(TableName)

        /// <summary>
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll(this IDbConnection connection,
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
            return MergeAllInternal<object>(connection: connection,
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
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll(this IDbConnection connection,
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
            return MergeAllInternal<object>(connection: connection,
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
        /// Insert the multiple dynamic objects (as new rows) or update the existing rows in the table. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static int MergeAll(this IDbConnection connection,
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
            return MergeAllInternal<object>(connection: connection,
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

        #region MergeAllAsync(TableName)

        /// <summary>
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
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
            return MergeAllAsyncInternal<object>(connection: connection,
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
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
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
            return MergeAllAsyncInternal<object>(connection: connection,
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
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the table fields are used unless the 'fields' argument is explicitly defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
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
            return MergeAllAsyncInternal<object>(connection: connection,
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

        #region MergeAllInternalBase<TEntity>

        /// <summary>
        /// Merges the multiple data entity or dynamic objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity or dynamic object to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static int MergeAllInternalBase<TEntity>(this IDbConnection connection,
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
            batchSize = Math.Min(batchSize, entities.Count());

            // Get the context
            var context = MergeAllExecutionContextProvider.Create<TEntity>(connection,
                entities,
                tableName,
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
                trace.BeforeMergeAll(cancellableTraceLog);
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
                            var returnValue = Converter.DbNullToNull(command.ExecuteScalar());

                            // Set the return value
                            if (returnValue != null)
                            {
                                context.IdentityPropertySetterFunc?.Invoke(entity, returnValue);
                            }

                            // Iterate the result
                            result++;
                        }
                    }
                    else
                    {
                        // Iterate the batches
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
                                context = MergeAllExecutionContextProvider.Create<TEntity>(connection,
                                    batchItems,
                                    tableName,
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
                                AddOrderColumnParameters(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            if (context.IdentityPropertySetterFunc == null)
                            {
                                // No identity setters
                                result += command.ExecuteNonQuery();
                            }
                            else
                            {
                                // Set the identity back
                                using (var reader = command.ExecuteReader())
                                {
                                    // Get the results
                                    var position = 0;
                                    do
                                    {
                                        if (reader.Read())
                                        {
                                            var value = Converter.DbNullToNull(reader.GetValue(0));
                                            var index = batchItems.Count > 1 && reader.FieldCount > 1 ? reader.GetInt32(1) : position;
                                            context.IdentityPropertySetterFunc.Invoke(batchItems[index], value);
                                            result++;
                                        }
                                        position++;
                                    }
                                    while (reader.NextResult());
                                }
                            }
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
            trace?.AfterMergeAll(new TraceLog(sessionId, context.CommandText, entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion

        #region UpsertAllInternalBase<TEntity>

        /// <summary>
        /// Upserts the multiple data entity or dynamic objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity or dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static int UpsertAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var type = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;
            var sessionId = Guid.Empty;

            // Get the properties
            if (type.IsGenericType == true)
            {
                properties = type.GetClassProperties();
            }
            else
            {
                properties = PropertyCache.Get(type);
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                // Throw if there is no primary
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException($"There is no primary found for '{tableName}'.");
                }

                // Set the primary as the qualifier
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Set the primary key
            primaryKey = properties?.FirstOrDefault(p =>
                string.Equals(primary?.Name, p.GetMappedName(), StringComparison.OrdinalIgnoreCase));

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, "UpsertAll.Before", entities, null);
                trace.BeforeMergeAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("UpsertAll.Cancelled");
                    }
                    return default(int);
                }
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
                // Ensure to open the connection
                connection.EnsureOpen();

                // Create a transaction
                if (hasTransaction == false)
                {
                    transaction = connection.BeginTransaction();
                }

                // Iterate the entities
                var immutableFields = fields.AsList(); // Fix for the IDictionary<string, object> object
                foreach (var entity in entities.AsList())
                {
                    // Call the upsert
                    var upsertResult = connection.UpsertInternalBase<TEntity, object>(tableName,
                        entity,
                        qualifiers,
                        immutableFields,
                        hints,
                        commandTimeout,
                        transaction,
                        trace,
                        statementBuilder);

                    // Iterate the result
                    if (Converter.DbNullToNull(upsertResult) != null)
                    {
                        result += 1;
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
            trace?.AfterMergeAll(new TraceLog(sessionId, "UpsertAll.After", entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion

        #region MergeAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Merges the multiple data entity or dynamic objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity or dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static async Task<int> MergeAllAsyncInternalBase<TEntity>(this IDbConnection connection,
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
            batchSize = Math.Min(batchSize, entities.Count());

            // Get the context
            var context = await MergeAllExecutionContextProvider.CreateAsync<TEntity>(connection,
                entities,
                tableName,
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
                trace.BeforeMergeAll(cancellableTraceLog);
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
                            var returnValue = Converter.DbNullToNull(await command.ExecuteScalarAsync(cancellationToken));

                            // Set the return value
                            if (returnValue != null)
                            {
                                context.IdentityPropertySetterFunc?.Invoke(entity, returnValue);
                            }

                            // Iterate the result
                            result++;
                        }
                    }
                    else
                    {
                        // Iterate the batches
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
                                context = await MergeAllExecutionContextProvider.CreateAsync<TEntity>(connection,
                                    batchItems,
                                    tableName,
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
                                AddOrderColumnParameters<TEntity>(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            if (context.IdentityPropertySetterFunc == null)
                            {
                                // No identity setters
                                result += await command.ExecuteNonQueryAsync(cancellationToken);
                            }
                            else
                            {
                                // Set the identity back
                                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                                {
                                    // Get the results
                                    var position = 0;
                                    do
                                    {
                                        if (await reader.ReadAsync(cancellationToken))
                                        {
                                            // No need to use async on this level (await reader.GetFieldValueAsync<object>(0, cancellationToken))
                                            var value = Converter.DbNullToNull(reader.GetValue(0));
                                            var index = batchItems.Count > 1 && reader.FieldCount > 1 ? reader.GetInt32(1) : position;
                                            context.IdentityPropertySetterFunc.Invoke(batchItems[index], value);
                                            result++;
                                        }
                                        position++;
                                    }
                                    while (await reader.NextResultAsync(cancellationToken));
                                }
                            }
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
            trace?.AfterMergeAll(new TraceLog(sessionId, context.CommandText, entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion

        #region UpsertAllInternalBase<TEntity>

        /// <summary>
        /// Upserts the multiple data entity or dynamic objects into the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The data entity or dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The number of affected rows during the merge process.</returns>
        internal static async Task<int> UpsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables needed
            var type = entities?.FirstOrDefault()?.GetType() ?? typeof(TEntity);
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;
            var sessionId = Guid.Empty;

            // Get the properties
            if (type.IsGenericType == true)
            {
                properties = type.GetClassProperties();
            }
            else
            {
                properties = PropertyCache.Get(type);
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                // Throw if there is no primary
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException($"There is no primary found for '{tableName}'.");
                }

                // Set the primary as the qualifier
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Set the primary key
            primaryKey = properties?.FirstOrDefault(p =>
                string.Equals(primary?.Name, p.GetMappedName(), StringComparison.OrdinalIgnoreCase));

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, "UpsertAll.Before", entities, null);
                trace.BeforeMergeAll(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("UpsertAll.Cancelled");
                    }
                    return default(int);
                }
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
                // Ensure to open the connection
                await connection.EnsureOpenAsync(cancellationToken);

                // Create a transaction
                if (hasTransaction == false)
                {
                    transaction = connection.BeginTransaction();
                }

                // Iterate the entities
                var immutableFields = fields.AsList(); // Fix for the IDictionary<string, object> object
                foreach (var entity in entities.AsList())
                {
                    // Call the upsert
                    var upsertResult = await connection.UpsertAsyncInternalBase<TEntity, object>(tableName,
                        entity,
                        qualifiers,
                        immutableFields,
                        hints,
                        commandTimeout,
                        transaction,
                        trace,
                        statementBuilder,
                        cancellationToken: cancellationToken);

                    // Iterate the result
                    if (Converter.DbNullToNull(upsertResult) != null)
                    {
                        result += 1;
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
            trace?.AfterMergeAll(new TraceLog(sessionId, "UpsertAll.After", entities, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Return the result
            return result;
        }

        #endregion
    }
}
