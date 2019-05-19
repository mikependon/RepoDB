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
        #region MergeAll<TEntity>

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAll<TEntity>(connection: connection,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The qualifer field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAll(connection: connection,
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAllInternal<TEntity>(connection: connection,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple data entity objects into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int MergeAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Check the primary
            var primary = GetAndGuardPrimaryKey<TEntity>();

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Return the result
            return MergeAllInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region MergeAllAsync<TEntity>

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAllAsync(connection: connection,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAllAsync(connection: connection,
                entities: entities,
                qualifiers: qualifier.AsEnumerable(),
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return MergeAllAsyncInternal(connection: connection,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges a data entity object into an existing data in the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be merged.</param>
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> MergeAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Check the primary
            var primary = GetAndGuardPrimaryKey<TEntity>();

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Return the result
            return MergeAllAsyncInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region MergeAll(TableName)

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAll(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAll(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static int MergeAll(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAllInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int MergeAllInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName);

            // Check the fields
            if (fields?.Any() != true)
            {
                fields = dbFields?.AsFields();
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                // Get the DB primary
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);

                // Throw if there is no primary
                if (primary == null)
                {
                    throw new PrimaryFieldNotFoundException($"There is no primary found for '{tableName}'.");
                }

                // Set the primary as the qualifier
                qualifiers = primary.AsField().AsEnumerable();
            }

            // Return the result
            return MergeAllInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region MergeAllAsync(TableName)

        /// <summary>
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAllAsync(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: null,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            Field qualifier,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAllAsync(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifier?.AsEnumerable(),
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        public static Task<int> MergeAllAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return MergeAllAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Merges the multiple dynamic objects into the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static Task<int> MergeAllAsyncInternal(this IDbConnection connection,
            string tableName,
            IEnumerable<object> entities,
            IEnumerable<Field> qualifiers,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName);

            // Check the fields
            if (fields?.Any() != true)
            {
                fields = dbFields?.AsFields();
            }

            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var primary = dbFields?.FirstOrDefault(dbField => dbField.IsPrimary == true);
                qualifiers = primary?.AsField().AsEnumerable();
            }

            // Return the result
            return MergeAllAsyncInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                qualifiers: qualifiers,
                batchSize: batchSize,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
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
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static int MergeAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Guard the parameters
            var count = GuardMergeAll(entities);

            // Validate the batch size
            batchSize = Math.Min(batchSize, count);

            // Get the function
            var callback = new Func<int, MergeAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var identity = (Field)null;
                var dbFields = DbFieldCache.Get(connection, tableName);
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

                // Set the identity value
                if (skipIdentityCheck == false)
                {
                    identity = IdentityCache.Get<TEntity>()?.AsField();
                    if (identity == null && identityDbField != null)
                    {
                        identity = FieldCache.Get<TEntity>().FirstOrDefault(field =>
                            field.UnquotedName.ToLower() == identityDbField.UnquotedName.ToLower());
                    }
                }

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => field.UnquotedName.ToLower() == dbField.UnquotedName.ToLower()) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var singleEntityFunc = (Action<DbCommand, TEntity>)null;
                var identitySetterFunc = (Action<TEntity, object>)null;

                // Get if we have not skipped it
                if (skipIdentityCheck == false && identity != null)
                {
                    identitySetterFunc = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Identity which objects to set
                if (batchSizeValue <= 1)
                {
                    singleEntityFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".MergeAll"),
                        inputFields?.AsList(),
                        null);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".MergeAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue);
                }

                // Identify the requests
                var mergeAllRequest = (MergeAllRequest)null;
                var mergeRequest = (MergeRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    if (batchSizeValue > 1)
                    {
                        mergeAllRequest = new MergeAllRequest(tableName,
                            connection,
                            fields,
                            qualifiers,
                            batchSizeValue,
                            statementBuilder);
                    }
                    else
                    {
                        mergeRequest = new MergeRequest(tableName,
                            connection,
                            fields,
                            qualifiers,
                            statementBuilder);
                    }
                }
                else
                {
                    if (batchSizeValue > 1)
                    {
                        mergeAllRequest = new MergeAllRequest(typeof(TEntity),
                            connection,
                            fields,
                            qualifiers,
                            batchSizeValue,
                            statementBuilder);
                    }
                    else
                    {
                        mergeRequest = new MergeRequest(typeof(TEntity),
                            connection,
                            fields,
                            qualifiers,
                            statementBuilder);
                    }
                }

                // Return the value
                return new MergeAllExecutionContext<TEntity>
                {
                    CommandText = batchSizeValue > 1 ? CommandTextCache.GetMergeAllText(mergeAllRequest) : CommandTextCache.GetMergeText(mergeRequest),
                    InputFields = inputFields,
                    BatchSize = batchSizeValue,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                    IdentityPropertySetterFunc = identitySetterFunc
                };
            });

            // Get the context
            var context = MergeAllExecutionContextCache<TEntity>.Get(tableName, qualifiers, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
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
                    // Prepare the command
                    command.Prepare();

                    // Directly execute if the entities is only 1 (performance)
                    if (batchSize == 1)
                    {
                        // Much better to use the actual single-based setter (performance)
                        foreach (var entity in entities)
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc(command, entity);

                            // Actual Execution
                            var returnValue = ObjectConverter.DbNullToNull(command.ExecuteScalar());

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
                        foreach (var batchEntities in entities.Split(batchSize))
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
                                context = MergeAllExecutionContextCache<TEntity>.Get(tableName, fields, qualifiers, batchItems.Count, callback);

                                // Set the command properties
                                command.CommandText = context.CommandText;

                                // Prepare the command
                                command.Prepare();
                            }

                            // Set the values
                            context.MultipleDataEntitiesParametersSetterFunc(command, batchItems);

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
                                    // Variables for the reader
                                    var readerPosition = 0;
                                    var readable = (reader != null);

                                    // Visit the next result
                                    while (readable)
                                    {
                                        if (reader.Read())
                                        {
                                            var value = ObjectConverter.DbNullToNull(reader.GetValue(0));
                                            context.IdentityPropertySetterFunc(batchItems[readerPosition], value);
                                        }

                                        // Iterate the result
                                        result++;
                                        readerPosition++;

                                        // Set the flag
                                        readable = reader.NextResult();
                                    }
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
            if (trace != null)
            {
                trace.AfterMergeAll(new TraceLog(context.CommandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

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
        /// <param name="qualifiers">The list of qualifer fields to be used.</param>
        /// <param name="batchSize">The batch size of the merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The number of rows affected by the execution.</returns>
        internal static async Task<int> MergeAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            int batchSize,
            IEnumerable<Field> fields,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Guard the parameters
            var count = GuardMergeAll(entities);

            // Validate the batch size
            batchSize = Math.Min(batchSize, count);

            // Get the function
            var callback = new Func<int, MergeAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var identity = (Field)null;
                var dbFields = DbFieldCache.Get(connection, tableName);
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

                // Set the identity value
                if (skipIdentityCheck == false)
                {
                    identity = IdentityCache.Get<TEntity>()?.AsField();
                    if (identity == null && identityDbField != null)
                    {
                        identity = FieldCache.Get<TEntity>().FirstOrDefault(field =>
                            field.UnquotedName.ToLower() == identityDbField.UnquotedName.ToLower());
                    }
                }

                // Filter the actual properties for input fields
                inputFields = dbFields?
                    .Where(dbField =>
                        fields.FirstOrDefault(field => field.UnquotedName.ToLower() == dbField.UnquotedName.ToLower()) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var singleEntityFunc = (Action<DbCommand, TEntity>)null;
                var identitySetterFunc = (Action<TEntity, object>)null;

                // Get if we have not skipped it
                if (skipIdentityCheck == false && identity != null)
                {
                    identitySetterFunc = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Identity which objects to set
                if (batchSizeValue <= 1)
                {
                    singleEntityFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".MergeAll"),
                        inputFields?.AsList(),
                        null);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".MergeAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue);
                }

                // Identify the requests
                var mergeAllRequest = (MergeAllRequest)null;
                var mergeRequest = (MergeRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    if (batchSizeValue > 1)
                    {
                        mergeAllRequest = new MergeAllRequest(tableName,
                            connection,
                            fields,
                            qualifiers,
                            batchSizeValue,
                            statementBuilder);
                    }
                    else
                    {
                        mergeRequest = new MergeRequest(tableName,
                            connection,
                            fields,
                            qualifiers,
                            statementBuilder);
                    }
                }
                else
                {
                    if (batchSizeValue > 1)
                    {
                        mergeAllRequest = new MergeAllRequest(typeof(TEntity),
                            connection,
                            fields,
                            qualifiers,
                            batchSizeValue,
                            statementBuilder);
                    }
                    else
                    {
                        mergeRequest = new MergeRequest(typeof(TEntity),
                            connection,
                            fields,
                            qualifiers,
                            statementBuilder);
                    }
                }

                // Return the value
                return new MergeAllExecutionContext<TEntity>
                {
                    CommandText = batchSizeValue > 1 ? CommandTextCache.GetMergeAllText(mergeAllRequest) : CommandTextCache.GetMergeText(mergeRequest),
                    InputFields = inputFields,
                    BatchSize = batchSizeValue,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                    IdentityPropertySetterFunc = identitySetterFunc
                };
            });

            // Get the context
            var context = MergeAllExecutionContextCache<TEntity>.Get(tableName, qualifiers, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
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
                    // Prepare the command
                    command.Prepare();

                    // Directly execute if the entities is only 1 (performance)
                    if (batchSize == 1)
                    {
                        // Much better to use the actual single-based setter (performance)
                        foreach (var entity in entities)
                        {
                            // Set the values
                            context.SingleDataEntityParametersSetterFunc(command, entity);

                            // Actual Execution
                            var returnValue = ObjectConverter.DbNullToNull(await command.ExecuteScalarAsync());

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
                        foreach (var batchEntities in entities.Split(batchSize))
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
                                context = MergeAllExecutionContextCache<TEntity>.Get(tableName, fields, qualifiers, batchItems.Count, callback);

                                // Set the command properties
                                command.CommandText = context.CommandText;

                                // Prepare the command
                                command.Prepare();
                            }

                            // Set the values
                            context.MultipleDataEntitiesParametersSetterFunc(command, batchItems);

                            // Actual Execution
                            if (context.IdentityPropertySetterFunc == null)
                            {
                                // No identity setters
                                result += await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                // Set the identity back
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    // Variables for the reader
                                    var readerPosition = 0;
                                    var readable = (reader != null);

                                    // Visit the next result
                                    while (readable)
                                    {
                                        if (await reader.ReadAsync())
                                        {
                                            var value = ObjectConverter.DbNullToNull(reader.GetValue(0));
                                            context.IdentityPropertySetterFunc(batchItems[readerPosition], value);
                                        }

                                        // Iterate the result
                                        result++;
                                        readerPosition++;

                                        // Set the flag
                                        readable = reader.NextResult();
                                    }
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
            if (trace != null)
            {
                trace.AfterMergeAll(new TraceLog(context.CommandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region Helpers

        private static int GuardMergeAll<TEntity>(IEnumerable<TEntity> entities)
        {
            var count = entities?.Count();
            if (count <= 0)
            {
                throw new InvalidOperationException("The entities must not be empty or null.");
            }
            return count.Value;
        }

        #endregion
    }
}
