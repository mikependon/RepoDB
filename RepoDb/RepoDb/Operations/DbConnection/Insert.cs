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
        #region Insert<TEntity>

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static object Insert<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertInternal<TEntity, object>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static TResult Insert<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertInternal<TEntity, TResult>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        internal static TResult InsertInternal<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Return the result
            return InsertInternalBase<TEntity, TResult>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                fields: FieldCache.Get<TEntity>(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: false);
        }

        #endregion

        #region InsertAsync<TEntity>

        /// <summary>
        /// Inserts a new data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static Task<object> InsertAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAsyncInternal<TEntity, object>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>
        /// The value of the primary key of the newly inserted data. Returns null if the 
        /// primary key property is not present.
        /// </returns>
        public static Task<TResult> InsertAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            return InsertAsyncInternal<TEntity, TResult>(connection: connection,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database in asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity object.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        internal static Task<TResult> InsertAsyncInternal<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where TEntity : class
        {
            // Return the result
            return InsertAsyncInternalBase<TEntity, TResult>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                entity: entity,
                fields: FieldCache.Get<TEntity>(),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: false);
        }

        #endregion

        #region Insert(TableName)

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static object Insert(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static TResult Insert<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertInternal<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database (certain fields only).
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        internal static TResult InsertInternal<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Return the result
            return InsertInternalBase<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                fields: Field.Parse(entity),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: true);
        }

        #endregion

        #region InsertAsync(TableName)

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static Task<object> InsertAsync(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAsyncInternal<object>(connection: connection,
                tableName: tableName,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        public static Task<TResult> InsertAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            return InsertAsyncInternal<TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be inserted.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the primary field.</returns>
        internal static Task<TResult> InsertAsyncInternal<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
        {
            // Return the result
            return InsertAsyncInternalBase<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                fields: Field.Parse(entity),
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                skipIdentityCheck: true);
        }

        #endregion

        #region InsertInternalBase<TEntity>

        /// <summary>
        /// Inserts a new data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The value of the primary field.</returns>
        internal static TResult InsertInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the function
            var callback = new Func<InsertExecutionContext<TEntity>>(() =>
            {
                // Variables
                var request = new InsertRequest(tableName,
                    connection,
                    fields,
                    statementBuilder);

                // Variables needed
                var identity = (Field)null;
                var dbFields = DbFieldCache.Get(connection, request.Name);
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields.FirstOrDefault(f => f.IsIdentity);

                // Set the identity field
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
                inputFields = dbFields
                    .Where(dbField => dbField.IsIdentity == false)
                    .Where(dbField =>
                        fields.FirstOrDefault(field => field.UnquotedName.ToLower() == dbField.UnquotedName.ToLower()) != null)
                    .AsList();

                // Variables for the entity action
                var identityPropertySetter = (Action<TEntity, object>)null;

                // Get the identity setter
                if (skipIdentityCheck == false && identity != null)
                {
                    identityPropertySetter = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Return the value
                return new InsertExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetInsertText(request),
                    InputFields = inputFields,
                    ParametersSetterFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", request.Name),
                        inputFields.AsList()),
                    IdentityPropertySetterFunc = identityPropertySetter
                };
            });

            // Get the context
            var context = InsertExecutionContextCache<TEntity>.Get(tableName, fields, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entity, null);
                trace.BeforeInsert(cancellableTraceLog);
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
                    // Set the values
                    context.ParametersSetterFunc(command, entity);

                    // Actual Execution
                    result = ObjectConverter.ToType<TResult>(command.ExecuteScalar());

                    // Set the return value
                    context.IdentityPropertySetterFunc?.Invoke(entity, result);
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
                trace.AfterInsert(new TraceLog(context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion

        #region InsertAsyncInternalBase<TEntity, TResult>

        /// <summary>
        /// Inserts a new data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the object (whether a data entity or a dynamic).</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The data entity object to be inserted.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/>s to be used.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="skipIdentityCheck">True to skip the identity check.</param>
        /// <returns>The value of the primary field.</returns>
        internal async static Task<TResult> InsertAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            bool skipIdentityCheck = false)
            where TEntity : class
        {
            // Get the function
            var callback = new Func<InsertExecutionContext<TEntity>>(() =>
            {
                // Variables
                var request = new InsertRequest(tableName,
                    connection,
                    fields,
                    statementBuilder);

                // Variables needed
                var identity = (Field)null;
                var dbFields = DbFieldCache.Get(connection, request.Name);
                var inputFields = (IEnumerable<DbField>)null;
                var identityDbField = dbFields.FirstOrDefault(f => f.IsIdentity);

                // Set the identity field
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
                inputFields = dbFields
                    .Where(dbField => dbField.IsIdentity == false)
                    .Where(dbField =>
                        fields.FirstOrDefault(field => field.UnquotedName.ToLower() == dbField.UnquotedName.ToLower()) != null)
                    .AsList();

                // Variables for the entity action
                var identityPropertySetter = (Action<TEntity, object>)null;

                // Get the identity setter
                if (skipIdentityCheck == false && identity != null)
                {
                    identityPropertySetter = FunctionCache.GetDataEntityPropertyValueSetterFunction<TEntity>(identity);
                }

                // Return the value
                return new InsertExecutionContext<TEntity>
                {
                    CommandText = CommandTextCache.GetInsertText(request),
                    InputFields = inputFields,
                    ParametersSetterFunc = FunctionCache.GetDataEntityDbCommandParameterSetterFunction<TEntity>(
                        string.Concat(typeof(TEntity).FullName, ".", request.Name),
                        inputFields.AsList()),
                    IdentityPropertySetterFunc = identityPropertySetter
                };
            });

            // Get the context
            var context = InsertExecutionContextCache<TEntity>.Get(tableName, fields, callback);

            // Before Execution
            if (trace != null)
            {
                var cancellableTraceLog = new CancellableTraceLog(context.CommandText, entity, null);
                trace.BeforeInsert(cancellableTraceLog);
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
                    // Set the values
                    context.ParametersSetterFunc(command, entity);

                    // Actual Execution
                    result = ObjectConverter.ToType<TResult>(await command.ExecuteScalarAsync());

                    // Set the return value
                    context.IdentityPropertySetterFunc?.Invoke(entity, result);
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
                trace.AfterInsert(new TraceLog(context.CommandText, entity, result,
                    DateTime.UtcNow.Subtract(beforeExecutionTime)));
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
