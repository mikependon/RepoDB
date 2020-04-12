using RepoDb.Contexts.Execution;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region UpdateAll<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAll<TEntity>(connection: connection,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int UpdateAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int UpdateAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            if (qualifiers?.Any() != true)
            {
                var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
                qualifiers = primary.AsField().AsEnumerable();
            }
            return UpdateAllInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAllAsync<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifier">The qualifier <see cref="Field"/> object to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> UpdateAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return UpdateAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be used for update.</param>
        /// <param name="qualifiers">The list of qualifier <see cref="Field"/> objects to be used for update.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The table hints to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> UpdateAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            if (qualifiers?.Any() != true)
            {
                var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
                qualifiers = primary.AsField().AsEnumerable();
            }
            return UpdateAllAsyncInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAll(TableName)

        /// <summary>
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            return UpdateAllInternal(connection: connection,
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
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            return UpdateAllInternal(connection: connection,
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
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            return UpdateAllInternal(connection: connection,
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
        /// Updates existing multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int UpdateAllInternal(this IDbConnection connection,
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
            return UpdateAllInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region UpdateAllAsync(TableName)

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> UpdateAllAsync(this IDbConnection connection,
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
            return UpdateAllAsyncInternal(connection: connection,
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
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAllAsyncInternal(connection: connection,
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
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            IStatementBuilder statementBuilder = null)
        {
            return UpdateAllAsyncInternal(connection: connection,
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
        /// Updates existing multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
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
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> UpdateAllAsyncInternal(this IDbConnection connection,
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
            return UpdateAllAsyncInternalBase<object>(connection: connection,
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

        #region UpdateAllInternalBase<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            GuardUpdateAll(entities);

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the fields
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            if (fields?.Any() != true)
            {
                var first = entities?.First();
                fields = first != null ? Field.Parse(first) : dbFields?.AsFields();
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
                qualifiers = primary?.AsField().AsEnumerable();
            }

            // Get the function
            var callback = new Func<int, UpdateAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var inputFields = new List<DbField>();

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var singleEntityFunc = (Action<DbCommand, TEntity>)null;

                // Identity which objects to set
                if (batchSizeValue <= 1)
                {
                    singleEntityFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".UpdateAll"),
                        inputFields?.AsList(),
                        null,
                        dbSetting);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".UpdateAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue,
                        dbSetting);
                }

                // Identity the requests
                var updateAllRequest = (UpdateAllRequest)null;

                // Variables
                if (typeof(TEntity) == typeof(object))
                {
                    updateAllRequest = new UpdateAllRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSizeValue,
                        hints,
                        statementBuilder);
                }
                else
                {
                    updateAllRequest = new UpdateAllRequest(typeof(TEntity),
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSizeValue,
                        hints,
                        statementBuilder);
                }

                // Return the value
                return new UpdateAllExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetUpdateAllText(updateAllRequest),
                    InputFields = inputFields,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc
                };
            });

            // Get the context
            var context = UpdateAllExecutionContextCache<TEntity>.Get(tableName, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
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
            var hasTransaction = (transaction != null);

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
                            context.SingleDataEntityParametersSetterFunc(command, entity);

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
                                context = UpdateAllExecutionContextCache<TEntity>.Get(tableName, fields, batchItems.Count, callback);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count() == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc(command, batchItems);
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
            if (trace != null)
            {
                trace.AfterUpdateAll(new TraceLog(context.CommandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region UpdateAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Updates existing multiple data in the database in an asynchronous way.
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
        /// <returns>The number of rows affected by the execution.</returns>
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
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            GuardUpdateAll(entities);

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the fields
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
            if (fields?.Any() != true)
            {
                var first = entities?.First();
                fields = first != null ? Field.Parse(first) : dbFields?.AsFields();
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
                qualifiers = primary?.AsField().AsEnumerable();
            }

            // Get the function
            var callback = new Func<int, UpdateAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var inputFields = new List<DbField>();

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var singleEntityFunc = (Action<DbCommand, TEntity>)null;

                // Identity which objects to set
                if (batchSizeValue <= 1)
                {
                    singleEntityFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".UpdateAll"),
                        inputFields?.AsList(),
                        null,
                        dbSetting);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".UpdateAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue,
                        dbSetting);
                }

                // Identity the requests
                var updateAllRequest = (UpdateAllRequest)null;

                // Variables
                if (typeof(TEntity) == typeof(object))
                {
                    updateAllRequest = new UpdateAllRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSizeValue,
                        hints,
                        statementBuilder);
                }
                else
                {
                    updateAllRequest = new UpdateAllRequest(typeof(TEntity),
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        batchSizeValue,
                        hints,
                        statementBuilder);
                }

                // Return the value
                return new UpdateAllExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetUpdateAllText(updateAllRequest),
                    InputFields = inputFields,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc
                };
            });

            // Get the context
            var context = UpdateAllExecutionContextCache<TEntity>.Get(tableName, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
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
            var hasTransaction = (transaction != null);

            try
            {
                // Ensure the connection is open
                await connection.EnsureOpenAsync();

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
                            context.SingleDataEntityParametersSetterFunc(command, entity);

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += await command.ExecuteNonQueryAsync();
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
                                context = UpdateAllExecutionContextCache<TEntity>.Get(tableName, fields, batchItems.Count, callback);

                                // Set the command properties
                                command.CommandText = context.CommandText;
                            }

                            // Set the values
                            if (batchItems?.Count() == 1)
                            {
                                context.SingleDataEntityParametersSetterFunc(command, batchItems.First());
                            }
                            else
                            {
                                context.MultipleDataEntitiesParametersSetterFunc(command, batchItems);
                            }

                            // Prepare the command
                            if (dbSetting.IsPreparable)
                            {
                                command.Prepare();
                            }

                            // Actual Execution
                            result += await command.ExecuteNonQueryAsync();
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
            if (trace != null)
            {
                trace.AfterUpdateAll(new TraceLog(context.CommandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Throws an exception if the entities argument is null or empty.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="entities">The enumerable list of entity objects.</param>
        private static void GuardUpdateAll<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities must not be null.");
            }
            if (entities.Any() == false)
            {
                throw new EmptyException("The entities must not be empty.");
            }
        }

        #endregion
    }
}
