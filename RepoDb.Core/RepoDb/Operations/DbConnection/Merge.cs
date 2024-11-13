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

namespace RepoDb
{
    /// <summary>
    /// Contains the extension methods for <see cref="IDbConnection"/> object.
    /// </summary>
    public static partial class DbConnectionExtension
    {
        #region Merge<TEntity>

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return MergeInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static TResult MergeInternal<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject() == true)
                {
                    return MergeInternalBase<IDictionary<string, object>, TResult>(connection: connection,
                        tableName: tableName,
                        entity: (IDictionary<string, object>)entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    return MergeInternalBase<TEntity, TResult>(connection: connection,
                        tableName: tableName,
                        entity: entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
            }
            else
            {
                if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject() == true)
                {
                    return UpsertInternalBase<IDictionary<string, object>, TResult>(connection: connection,
                        tableName: tableName,
                        entity: (IDictionary<string, object>)entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
                else
                {
                    return UpsertInternalBase<TEntity, TResult>(connection: connection,
                        tableName: tableName,
                        entity: entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                        traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder);
                }
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync<TEntity>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, object>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifier">The field to be used during merge operation.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The expression for the qualifier fields.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TEntity, TResult>(this IDbConnection connection,
            TEntity entity,
            Expression<Func<TEntity, object>> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return MergeAsyncInternal<TEntity, TResult>(connection: connection,
                tableName: GetMappedName<TEntity>(entity),
                entity: entity,
                qualifiers: Field.Parse<TEntity>(qualifiers),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Inserts a new row or updates an existing row in the table in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The object to be merged.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
		/// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static Task<TResult> MergeAsyncInternal<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables needed
            var setting = connection.GetDbSetting();

            // Return the result
            if (setting.IsUseUpsert == false)
            {
                if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject() == true)
                {
                    return MergeAsyncInternalBase<IDictionary<string, object>, TResult>(connection: connection,
                        tableName: tableName,
                        entity: (IDictionary<string, object>)entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    return MergeAsyncInternalBase<TEntity, TResult>(connection: connection,
                        tableName: tableName,
                        entity: entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                if (TypeCache.Get(GetEntityType(entity)).IsDictionaryStringObject() == true)
                {
                    return UpsertAsyncInternalBase<IDictionary<string, object>, TResult>(connection: connection,
                        tableName: tableName,
                        entity: (IDictionary<string, object>)entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    return UpsertAsyncInternalBase<TEntity, TResult>(connection: connection,
                        tableName: tableName,
                        entity: entity,
                        qualifiers: qualifiers,
                        fields: GetQualifiedFields<TEntity>(fields, entity),
                        hints: hints,
                        commandTimeout: commandTimeout,
                traceKey: traceKey,
                        transaction: transaction,
                        trace: trace,
                        statementBuilder: statementBuilder,
                        cancellationToken: cancellationToken);
                }
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static object Merge(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static TResult Merge<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return MergeInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region MergeAsync(TableName)

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<object> MergeAsync(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, object>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: null,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifier">The qualifier field to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            Field qualifier,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifier?.AsEnumerable(),
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Merges a dynamic object into an existing data in the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="TResult">The target type of the result.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table to be used.</param>
        /// <param name="entity">The dynamic object to be merged.</param>
        /// <param name="qualifiers">The qualifier <see cref="Field"/> objects to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        public static Task<TResult> MergeAsync<TResult>(this IDbConnection connection,
            string tableName,
            object entity,
            IEnumerable<Field> qualifiers,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return MergeAsyncInternal<object, TResult>(connection: connection,
                tableName: tableName,
                entity: entity,
                qualifiers: qualifiers,
                fields: fields,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
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
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static TResult MergeInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var key = GetAndGuardPrimaryKeyOrIdentityKey(connection, tableName, transaction,
                    entity?.GetType() ?? typeof(TEntity));
                qualifiers = key.AsEnumerable();
            }

            // Get the context
            var entityType = entity?.GetType() ?? typeof(TEntity);
            var context = MergeExecutionContextProvider.Create(entityType,
                connection,
                tableName,
                qualifiers,
                fields,
                hints,
                transaction,
                statementBuilder);
            var result = default(TResult);

            // Create the command
            using (var command = (DbCommand)connection.EnsureOpen().CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Before Execution
                var traceResult = Tracer
                    .InvokeBeforeExecution(traceKey, trace, command);

                // Silent cancellation
                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return result;
                }

                // Actual Execution
                result = Converter.ToType<TResult>(command.ExecuteScalar());

                // After Execution
                Tracer
                    .InvokeAfterExecution(traceResult, trace, result);

                // Set the return value
                if (result != null)
                {
                    context.KeyPropertySetterFunc?.Invoke(entity, result);
                }
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
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static TResult UpsertInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Variables needed
            var type = entity?.GetType() ?? typeof(TEntity);
            var isDictionaryType = TypeCache.Get(type).IsDictionaryStringObject();
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var primary = dbFields?.GetPrimary();
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;

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

            // Get the properties
            if (isDictionaryType == false)
            {
                if (type.IsGenericType == true)
                {
                    properties = type.GetClassProperties();
                }
                else
                {
                    properties = PropertyCache.Get(type);
                }

                // Set the primary key
                primaryKey = properties?.FirstOrDefault(p =>
                    string.Equals(primary?.Name, p.GetMappedName(), StringComparison.OrdinalIgnoreCase));
            }

            // Expression
            var where = (QueryGroup)null;
            if (isDictionaryType)
            {
                where = CreateQueryGroupForUpsert((IDictionary<string, object>)entity,
                    qualifiers);
            }
            else
            {
                where = CreateQueryGroupForUpsert(entity,
                    properties,
                    qualifiers);
            }

            // Validate
            if (where == null)
            {
                throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
            }

            // Execution variables
            var result = default(TResult);
            var exists = connection.Exists(tableName,
                where,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);

            // Check the existence
            if (exists == true)
            {
                // Call the update operation
                var updateResult = connection.Update<TEntity>(tableName,
                    entity,
                    where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);

                // Check if there is result
                if (updateResult > 0)
                {
                    if (isDictionaryType == false)
                    {
                        if (primaryKey != null)
                        {
                            result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity));
                        }
                    }
                    else
                    {
                        var dictionary = (IDictionary<string, object>)entity;
                        if (primary != null && dictionary.ContainsKey(primary.Name))
                        {
                            result = Converter.ToType<TResult>(dictionary[primary.Name]);
                        }
                    }
                }
            }
            else
            {
                // Call the insert operation
                var insertResult = connection.Insert(tableName,
                    entity,
                    fields: fields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder);

                // Set the result
                result = Converter.ToType<TResult>(insertResult);
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
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static async Task<TResult> MergeAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field> fields = null,
            IEnumerable<Field>? qualifiers = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Check the qualifiers
            if (qualifiers?.Any() != true)
            {
                var key = await GetAndGuardPrimaryKeyOrIdentityKeyAsync(connection, tableName, transaction,
                    entity?.GetType() ?? typeof(TEntity), cancellationToken);
                qualifiers = key.AsEnumerable();
            }

            // Get the context
            var entityType = entity?.GetType() ?? typeof(TEntity);
            var context = await MergeExecutionContextProvider.CreateAsync(entityType,
                connection,
                tableName,
                qualifiers,
                fields,
                hints,
                transaction,
                statementBuilder,
                cancellationToken);
            var result = default(TResult);

            // Create the command
            using (var command = (DbCommand)(await connection.EnsureOpenAsync(cancellationToken)).CreateCommand(context.CommandText,
                CommandType.Text, commandTimeout, transaction))
            {
                // Set the values
                context.ParametersSetterFunc(command, entity);

                // Before Execution
                var traceResult = await Tracer
                    .InvokeBeforeExecutionAsync(traceKey, trace, command, cancellationToken);

                // Silent cancellation
                if (traceResult?.CancellableTraceLog?.IsCancelled == true)
                {
                    return result;
                }

                // Actual Execution
                result = Converter.ToType<TResult>(await command.ExecuteScalarAsync(cancellationToken));

                // After Execution
                await Tracer
                    .InvokeAfterExecutionAsync(traceResult, trace, result, cancellationToken);

                // Set the return value
                if (result != null)
                {
                    context.KeyPropertySetterFunc?.Invoke(entity, result);
                }
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
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The value of the identity field if present, otherwise, the value of the primary field.</returns>
        internal static async Task<TResult> UpsertAsyncInternalBase<TEntity, TResult>(this IDbConnection connection,
            string tableName,
            TEntity entity,
            IEnumerable<Field>? qualifiers = null,
            IEnumerable<Field> fields = null,
            string? hints = null,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Merge,
            IDbTransaction? transaction = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Variables needed
            var type = entity?.GetType() ?? typeof(TEntity);
            var isDictionaryType = TypeCache.Get(type).IsDictionaryStringObject();
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            var primary = dbFields?.GetPrimary();
            var properties = (IEnumerable<ClassProperty>)null;
            var primaryKey = (ClassProperty)null;

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

            // Get the properties
            if (isDictionaryType == false)
            {
                if (type.IsGenericType == true)
                {
                    properties = type.GetClassProperties();
                }
                else
                {
                    properties = PropertyCache.Get(type);
                }

                // Set the primary key
                primaryKey = properties?.FirstOrDefault(p =>
                    string.Equals(primary?.Name, p.GetMappedName(), StringComparison.OrdinalIgnoreCase));
            }

            // Expression
            var where = (QueryGroup)null;
            if (isDictionaryType)
            {
                where = CreateQueryGroupForUpsert((IDictionary<string, object>)entity,
                    qualifiers);
            }
            else
            {
                where = CreateQueryGroupForUpsert(entity,
                    properties,
                    qualifiers);
            }

            // Validate
            if (where == null)
            {
                throw new Exceptions.InvalidExpressionException("The generated expression from the given qualifiers is not valid.");
            }

            // Execution variables
            var result = default(TResult);
            var exists = await connection.ExistsAsync(tableName,
                where,
                hints: hints,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);

            // Check the existence
            if (exists == true)
            {
                // Call the update operation
                var updateResult = await connection.UpdateAsync<TEntity>(tableName,
                    entity,
                    where,
                    fields: fields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);

                // Check if there is result
                if (updateResult > 0)
                {
                    if (isDictionaryType == false)
                    {
                        if (primaryKey != null)
                        {
                            result = Converter.ToType<TResult>(primaryKey.PropertyInfo.GetValue(entity));
                        }
                    }
                    else
                    {
                        var dictionary = (IDictionary<string, object>)entity;
                        if (primary != null && dictionary.ContainsKey(primary.Name))
                        {
                            result = Converter.ToType<TResult>(dictionary[primary.Name]);
                        }
                    }
                }
            }
            else
            {
                // Call the insert operation
                var insertResult = await connection.InsertAsync(tableName,
                    entity,
                    fields: fields,
                    hints: hints,
                    commandTimeout: commandTimeout,
                    traceKey: traceKey,
                    transaction: transaction,
                    trace: trace,
                    statementBuilder: statementBuilder,
                    cancellationToken: cancellationToken);

                // Set the result
                result = Converter.ToType<TResult>(insertResult);
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
