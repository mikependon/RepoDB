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
using System.Reflection;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Merge<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge<TEntity>(connection: connection,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge<TEntity>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return Merge<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static TResult MergeInternal<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
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
                var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                return MergeInternalBase<TEntity, TResult>(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    entity: entity,
                    fields: entity.AsFields(),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    skipIdentityCheck: false);
            }
            else
            {
                return UpsertInternalBase<TEntity, TResult>(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    entity: entity,
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync<TEntity>(connection: connection,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync<TEntity, object>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsync<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static Task<TResult> MergeAsyncInternal<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
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
                var primary = GetAndGuardPrimaryKey<TEntity>(connection, transaction);
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                return MergeAsyncInternalBase<TEntity, TResult>(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    entity: entity,
                    fields: entity.AsFields(),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    skipIdentityCheck: false);
            }
            else
            {
                return UpsertAsyncInternalBase<TEntity, TResult>(connection: connection,
                    tableName: ClassMappedNameCache.Get<TEntity>(),
                    entity: entity,
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region Merge(TableName)

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Merge(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Merge(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Merge<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return Merge<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeInternal<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static TResult MergeInternal<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                return MergeInternalBase<object, TResult>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    fields: entity.AsFields(),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    skipIdentityCheck: true);
            }
            else
            {
                return UpsertInternalBase<object, TResult>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsync(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsync<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifier: null,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsync<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAsyncInternal<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static async Task<TResult> MergeAsyncInternal<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                return await MergeAsyncInternalBase<object, TResult>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    fields: entity.AsFields(),
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    skipIdentityCheck: true);
            }
            else
            {
                return await UpsertAsyncInternalBase<object, TResult>(connection: connection,
                    tableName: tableName,
                    entity: entity,
                    qualifiers: qualifiers,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
        }

        #endregion

        #region MergeInternalBase<TEntity>

        /// <summary>
        /// Merges a data entity or dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static TResult MergeInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the database fields
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                // Get the DB primary
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);

                // Throw if there is no primary
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException($"There is no primary found for '{tableName}'.");
                }

                // Set the primary as the qualifier
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Get the function
            var callback = new Func<MergeExecutionContext<TEntity>>(() =>
            {
                // Variables needed
                var identity = (Field)null;
                var inputFields = new List<DbField>();
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Set the identity field
                if (skipIdentityCheck == false)
                {
                    identity = IdentityCache.Get<TEntity>()?.AsField();
                    if (identity == null && identityDbField != null)
                    {
                        identity = FieldCache.Get<TEntity>().FirstOrDefault(field =>
                            string.Equals(field.Name.AsUnquoted(true, dbSetting), identityDbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                    }
                }

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the entity action
                var identityPropertySetter = (Action<TEntity, object>)null;

                // Get the identity setter
                if (skipIdentityCheck == false && identity != null)
                {
                    identityPropertySetter = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Identify the requests
                var mergeRequest = (MergeRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    mergeRequest = new MergeRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        hints,
                        statementBuilder);
                }
                else
                {
                    mergeRequest = new MergeRequest(typeof(TEntity),
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        hints,
                        statementBuilder);
                }

                // Return the value
                return new MergeExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetMergeText(mergeRequest),
                    InputFields = inputFields,
                    ParametersSetterFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".Merge"),
                        inputFields?.AsList(),
                        null,
                        dbSetting),
                    IdentityPropertySetterFunc = identityPropertySetter
                };
            });

            // Get the context
            var context = MergeExecutionContextCache<TEntity>.Get(tableName, qualifiers, fields, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entity, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return default(TResult);
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = default(TResult);

            // Create the command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Actual Execution
                result = Converter.ToType<TResult>(command.ExecuteScalar());

                // Set the return value
                if (Equals(result, default(TResult)) == false)
                {
                    context.IdentityPropertySetterFunc?.Invoke(entity, result);
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region UpsertInternalBase<TEntity>

        /// <summary>
        /// Upserts a data entity or dynamic object into an existing data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static TResult UpsertInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var type = entity?.GetType() ?? typeof(TEntity);
            var isObjectType = typeof(TEntity) == typeof(object);
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;

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
                var cancellableTraceLog = new CancellableTraceLog("Upsert.Before", entity, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("Upsert.Cancelled");
                    }
                    return default(TResult);
                }
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Expression
            var where = CreateQueryGroupForUpsert(entity,
                properties,
                qualifiers);

            // Validate
            if (where == null)
            {
                throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = default(TResult);
            var exists = false;

            if (isObjectType == true)
            {
                exists = connection.Exists(tableName,
                    where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
            else
            {
                exists = connection.Exists<TEntity>(where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }

            // Check the existence
            if (exists == true)
            {
                // Call the update operation
                var updateResult = default(int);

                if (isObjectType == true)
                {
                    updateResult = connection.Update(tableName,
                        entity,
                        where,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    updateResult = connection.Update<TEntity>(entity,
                        where,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Check if there is result
                if (updateResult > 0)
                {
                    if (primaryKey != null)
                    {
                        // Set the result
                        result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity));
                    }
                }
            }
            else
            {
                // Call the insert operation
                var insertResult = (object)null;

                if (isObjectType == true)
                {
                    insertResult = connection.Insert(tableName,
                        entity,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    insertResult = connection.Insert<TEntity>(entity,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Set the result
                result = Converter.ToType<TResult>(insertResult);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog("Upsert.After", entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region MergeAsyncInternalBase<TEntity>

        /// <summary>
        /// Merges a data entity or dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static async Task<TResult> MergeAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the database fields
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);

            // Get the function
            var callback = new Func<MergeExecutionContext<TEntity>>(() =>
            {
                // Variables needed
                var identity = (Field)null;
                var inputFields = new List<DbField>();
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);
                var dbSetting = connection.GetDbSetting();

                // Set the identity field
                if (skipIdentityCheck == false)
                {
                    identity = IdentityCache.Get<TEntity>()?.AsField();
                    if (identity == null && identityDbField != null)
                    {
                        identity = FieldCache.Get<TEntity>().FirstOrDefault(field =>
                            string.Equals(field.Name.AsUnquoted(true, dbSetting), identityDbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase));
                    }
                }

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the entity action
                var identityPropertySetter = (Action<TEntity, object>)null;

                // Get the identity setter
                if (skipIdentityCheck == false && identity != null)
                {
                    identityPropertySetter = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Identify the requests
                var mergeRequest = (MergeRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    mergeRequest = new MergeRequest(tableName,
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        hints,
                        statementBuilder);
                }
                else
                {
                    mergeRequest = new MergeRequest(typeof(TEntity),
                        connection,
                        transaction,
                        fields,
                        qualifiers,
                        hints,
                        statementBuilder);
                }

                // Return the value
                return new MergeExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetMergeText(mergeRequest),
                    InputFields = inputFields,
                    ParametersSetterFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".Merge"),
                        inputFields?.AsList(),
                        null,
                        dbSetting),
                    IdentityPropertySetterFunc = identityPropertySetter
                };
            });

            // Get the context
            var context = MergeExecutionContextCache<TEntity>.Get(tableName, qualifiers, fields, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entity, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(context.CommandText);
                    }
                    return default(TResult);
                }
                context.CommandText = (cancellableTraceLog.Statement ?? context.CommandText);
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = default(TResult);

            // Create the command
            using (var command = (DbCommand)(await connection.EnsureOpenAsync()).CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Actual Execution
                result = Converter.ToType<TResult>(await command.ExecuteScalarAsync());

                // Set the return value
                if (Equals(result, default(TResult)) == false)
                {
                    context.IdentityPropertySetterFunc?.Invoke(entity, result);
                }
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog(context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region UpsertAsyncInternalBase<TEntity>

        /// <summary>
        /// Upserts a data entity or dynamic object into an existing data in the database in an .
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity or dynamic object to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of primary field.</returns>
        internal static async Task<TResult> UpsertAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var type = entity?.GetType() ?? typeof(TEntity);
            var isObjectType = typeof(TEntity) == typeof(object);
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
            var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary);
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;

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
                var cancellableTraceLog = new CancellableTraceLog("Upsert.Before", entity, null);
                trace.BeforeMerge(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException("Upsert.Cancelled");
                    }
                    return default(TResult);
                }
                entity = (TEntity)(cancellableTraceLog.Parameter ?? entity);
            }

            // Expression
            var where = CreateQueryGroupForUpsert(entity,
                properties,
                qualifiers);

            // Validate
            if (where == null)
            {
                throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
            }

            // Before Execution Time
            var beforeExecutionTime = DateTime.UtcNow;

            // Execution variables
            var result = default(TResult);
            var exists = false;

            if (isObjectType == true)
            {
                exists = await connection.ExistsAsync(tableName,
                    where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }
            else
            {
                exists = await connection.ExistsAsync<TEntity>(where,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);
            }

            // Check the existence
            if (exists == true)
            {
                // Call the update operation
                var updateResult = default(int);

                if (isObjectType == true)
                {
                    updateResult = await connection.UpdateAsync(tableName,
                        entity,
                        where,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    updateResult = await connection.UpdateAsync<TEntity>(entity,
                        where,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Check if there is result
                if (updateResult > 0)
                {
                    if (primaryKey != null)
                    {
                        // Set the result
                        result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity));
                    }
                }
            }
            else
            {
                // Call the insert operation
                var insertResult = (object)null;

                if (isObjectType == true)
                {
                    insertResult = await connection.InsertAsync(tableName,
                        entity,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    insertResult = await connection.InsertAsync<TEntity>(entity,
                        hints: hints,
                        commandTimeout: commandTimeout,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }

                // Set the result
                result = Converter.ToType<TResult>(insertResult);
            }

            // After Execution
            if (trace != null)
            {
                trace.AfterMerge(new TraceLog("Upsert.After", entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
