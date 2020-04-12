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
        #region InsertAll<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity objects.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternal<TEntity>(connection: connection,
                entities: entities,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: false);
        }

        #endregion

        #region InsertAllAsync<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllAsyncInternal<TEntity>(connection: connection,
                entities: entities,
                batchSize: batchSize,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entities">The list of data entity objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal<TEntity>(this IDbConnection connection,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAllAsyncInternalBase<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entities: entities,
                batchSize: batchSize,
                fields: FieldCache.Get<TEntity>(),
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: false);
        }

        #endregion

        #region InsertAll(TableName)

        /// <summary>
        /// Inserts multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static int InsertAll(this IDbConnection connection,
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
            return InsertAllInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternal(this IDbConnection connection,
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
            // Return the result
            return InsertAllInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: true);
        }

        #endregion

        #region InsertAllAsync(TableName)

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        public static Task<int> InsertAllAsync(this IDbConnection connection,
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
            return InsertAllAsyncInternal(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way. By default, the database fields are used unless the 'fields' argument is defined.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static Task<int> InsertAllAsyncInternal(this IDbConnection connection,
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
            // Return the result
            return InsertAllAsyncInternalBase<object>(connection: connection,
                tableName: tableName,
                entities: entities,
                batchSize: batchSize,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: true);
        }

        #endregion

        #region InsertAllInternalBase<TEntity>

        /// <summary>
        /// Inserts multiple data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static int InsertAllInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            GuardInsertAll(entities);

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the fields
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            if (fields?.Any() != true)
            {
                var first = entities?.First();
                fields = first != null ? Field.Parse(first) : dbFields?.AsFields();
            }

            // Get the function
            var callback = new Func<int, InsertAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var identity = (Field)null;
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

                // Set the identity value
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
                    .Where(dbField => dbField.IsIdentity == false)
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var identitySettersFunc = (List<Action<TEntity, DbCommand>>)null;
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
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".InsertAll"),
                        inputFields?.AsList(),
                        null,
                        dbSetting);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".InsertAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue,
                        dbSetting);
                }

                // Identify the requests
                var insertAllRequest = (InsertAllRequest)null;
                var insertRequest = (InsertRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    if (batchSizeValue > 1)
                    {
                        insertAllRequest = new InsertAllRequest(tableName,
                            connection,
                            transaction,
                            fields,
                            batchSizeValue,
                            hints,
                            statementBuilder);
                    }
                    else
                    {
                        insertRequest = new InsertRequest(tableName,
                            connection,
                            transaction,
                            fields,
                            hints,
                            statementBuilder);
                    }
                }
                else
                {
                    if (batchSizeValue > 1)
                    {
                        insertAllRequest = new InsertAllRequest(typeof(TEntity),
                            connection,
                            transaction,
                            fields,
                            batchSizeValue,
                            hints,
                            statementBuilder);
                    }
                    else
                    {
                        insertRequest = new InsertRequest(typeof(TEntity),
                            connection,
                            transaction,
                            fields,
                            hints,
                            statementBuilder);
                    }
                }

                // Return the value
                return new InsertAllExecutionContext<TEntity>
                {
                    CommandText = batchSizeValue > 1 ? CommandTextCache.GetInsertAllText(insertAllRequest) : CommandTextCache.GetInsertText(insertRequest),
                    InputFields = inputFields,
                    BatchSize = batchSizeValue,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                    IdentityPropertySetterFunc = identitySetterFunc,
                    IdentityPropertySettersFunc = identitySettersFunc
                };
            });

            // Get the context
            var context = (InsertAllExecutionContext<TEntity>)null;

            // Identify the number of entities (performance), get an execution context from cache
            context = InsertAllExecutionContextCache<TEntity>.Get(tableName, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
                trace.BeforeInsertAll(cancellableTraceLog);
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
                    if (context.BatchSize == 1)
                    {
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
                            var returnValue = Converter.DbNullToNull(command.ExecuteScalar());

                            // Get explicity if needed
                            if (Equals(returnValue, null) == true && dbSetting.IsMultiStatementExecutable == false)
                            {
                                returnValue = Converter.DbNullToNull(connection.GetDbHelper().GetScopeIdentity(connection, transaction));
                            }

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
                                context = InsertAllExecutionContextCache<TEntity>.Get(tableName, fields, batchItems.Count, callback);

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
                            if (context.IdentityPropertySetterFunc == null)
                            {
                                result += command.ExecuteNonQuery();
                            }
                            else
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    var index = 0;

                                    // Get the results
                                    do
                                    {
                                        if (reader.Read())
                                        {
                                            var value = Converter.DbNullToNull(reader.GetValue(0));
                                            context.IdentityPropertySetterFunc.Invoke(batchItems[index], value);
                                            result++;
                                        }
                                        index++;
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
            if (trace != null)
            {
                trace.AfterInsertAll(new TraceLog(context.CommandText, entities, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region InsertAllAsyncInternalBase<TEntity>

        /// <summary>
        /// Inserts multiple data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entities">The list of data entity or dynamic objects to be inserted.</param>
        /// <param name="batchSize">The batch size of the insertion.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The number of inserted rows.</returns>
        internal static async Task<int> InsertAllAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            int batchSize = Constant.DefaultBatchOperationSize,
            IEnumerable<Field> fields = null,
            string hints = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Variables needed
            var dbSetting = connection.GetDbSetting();

            // Guard the parameters
            GuardInsertAll(entities);

            // Validate the batch size
            batchSize = (dbSetting.IsMultiStatementExecutable == true) ? Math.Min(batchSize, entities.Count()) : 1;

            // Get the fields
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction);
            if (fields?.Any() != true)
            {
                var first = entities?.First();
                fields = first != null ? Field.Parse(first) : dbFields?.AsFields();
            }

            // Get the function
            var callback = new Func<int, InsertAllExecutionContext<TEntity>>((int batchSizeValue) =>
            {
                // Variables needed
                var identity = (Field)null;
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields?.FirstOrDefault(f => f.IsIdentity);

                // Set the identity value
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
                    .Where(dbField => dbField.IsIdentity == false)
                    .Where(dbField =>
                        fields.FirstOrDefault(field => string.Equals(field.Name.AsUnquoted(true, dbSetting), dbField.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();

                // Variables for the context
                var multipleEntitiesFunc = (Action<DbCommand, IList<TEntity>>)null;
                var identitySettersFunc = (List<Action<TEntity, DbCommand>>)null;
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
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".InsertAll"),
                        inputFields?.AsList(),
                        null,
                        dbSetting);
                }
                else
                {
                    multipleEntitiesFunc = FunctionCache.GetDataEntitiesDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", tableName, ".InsertAll"),
                        inputFields?.AsList(),
                        null,
                        batchSizeValue,
                        dbSetting);
                }

                // Identify the requests
                var insertAllRequest = (InsertAllRequest)null;
                var insertRequest = (InsertRequest)null;

                // Create a different kind of requests
                if (typeof(TEntity) == typeof(object))
                {
                    if (batchSizeValue > 1)
                    {
                        insertAllRequest = new InsertAllRequest(tableName,
                            connection,
                            transaction,
                            fields,
                            batchSizeValue,
                            hints,
                            statementBuilder);
                    }
                    else
                    {
                        insertRequest = new InsertRequest(tableName,
                            connection,
                            transaction,
                            fields,
                            hints,
                            statementBuilder);
                    }
                }
                else
                {
                    if (batchSizeValue > 1)
                    {
                        insertAllRequest = new InsertAllRequest(typeof(TEntity),
                            connection,
                            transaction,
                            fields,
                            batchSizeValue,
                            hints,
                            statementBuilder);
                    }
                    else
                    {
                        insertRequest = new InsertRequest(typeof(TEntity),
                            connection,
                            transaction,
                            fields,
                            hints,
                            statementBuilder);
                    }
                }

                // Return the value
                return new InsertAllExecutionContext<TEntity>
                {
                    CommandText = batchSizeValue > 1 ? CommandTextCache.GetInsertAllText(insertAllRequest) : CommandTextCache.GetInsertText(insertRequest),
                    InputFields = inputFields,
                    BatchSize = batchSizeValue,
                    SingleDataEntityParametersSetterFunc = singleEntityFunc,
                    MultipleDataEntitiesParametersSetterFunc = multipleEntitiesFunc,
                    IdentityPropertySetterFunc = identitySetterFunc,
                    IdentityPropertySettersFunc = identitySettersFunc
                };
            });

            // Get the context
            var context = (InsertAllExecutionContext<TEntity>)null;

            // Identify the number of entities (performance), get an execution context from cache
            context = InsertAllExecutionContextCache<TEntity>.Get(tableName, fields, batchSize, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entities, null);
                trace.BeforeInsertAll(cancellableTraceLog);
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
                    if (context.BatchSize == 1)
                    {
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
                            var returnValue = Converter.DbNullToNull(await command.ExecuteScalarAsync());

                            // Get explicity if needed
                            if (Equals(returnValue, null) == true && dbSetting.IsMultiStatementExecutable == false)
                            {
                                returnValue = Converter.DbNullToNull(await connection.GetDbHelper().GetScopeIdentityAsync(connection, transaction));
                            }

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
                                context = InsertAllExecutionContextCache<TEntity>.Get(tableName, fields, batchItems.Count, callback);

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
                            if (context.IdentityPropertySetterFunc == null)
                            {
                                result += await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var index = 0;

                                    // Get the results
                                    do
                                    {
                                        if (await reader.ReadAsync())
                                        {
                                            var value = Converter.DbNullToNull(reader.GetValue(0));
                                            context.IdentityPropertySetterFunc.Invoke(batchItems[index], value);
                                            result++;
                                        }
                                        index++;
                                    }
                                    while (await reader.NextResultAsync());
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
                trace.AfterInsertAll(new TraceLog(context.CommandText, entities, result,
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
        private static void GuardInsertAll<TEntity>(IEnumerable<TEntity> entities)
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
