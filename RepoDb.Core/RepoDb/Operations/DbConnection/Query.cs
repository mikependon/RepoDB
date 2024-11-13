using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Generic;
using System.Data;
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
        #region Query<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity, TWhat>(this IDbConnection connection,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: WhatToQueryGroup(typeof(TEntity), connection, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null) where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            return QueryInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> QueryInternal<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Ensure the fields
            fields = GetQualifiedFields<TEntity>(fields) ??
                DbFieldCache.Get(connection, tableName, transaction)?.GetAsFields();

            // Return
            return QueryInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAsync<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: ToQueryGroup(where),
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TWhat>(this IDbConnection connection,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return await QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: await WhatToQueryGroupAsync(typeof(TEntity), connection, what, transaction, cancellationToken),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this IDbConnection connection,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return QueryAsyncInternal<TEntity>(connection: connection,
                tableName: ClassMappedNameCache.Get<TEntity>(),
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> QueryAsyncInternal<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Ensure the fields
            fields = GetQualifiedFields<TEntity>(fields) ??
                (await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken))?.GetAsFields();

            // Return
            return await QueryAsyncInternalBase<TEntity>(connection: connection,
                tableName: tableName,
                fields: fields,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region Query(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> Query<TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return Query(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> Query(this IDbConnection connection,
            string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return Query(connection: connection,
                tableName: tableName,
                where: WhatToQueryGroup(connection, tableName, what, transaction),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> Query(this IDbConnection connection,
            string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return Query(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> Query(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return Query(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static IEnumerable<dynamic> Query(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return QueryInternal(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static IEnumerable<dynamic> QueryInternal(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
        {
            return QueryInternal<dynamic>(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        #endregion

        #region QueryAsync(TableName)

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TWhat">The type of the expression or the key value.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static async Task<IEnumerable<dynamic>> QueryAsync<TWhat>(this IDbConnection connection,
            string tableName,
            TWhat what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return await QueryAsync(connection: connection,
                tableName: tableName,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="what">The dynamic expression or the primary/identity key value to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
            string tableName,
            object what,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return await QueryAsync(connection: connection,
                tableName: tableName,
                where: await WhatToQueryGroupAsync(connection, tableName, what, transaction, cancellationToken),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
            string tableName,
            QueryField where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return QueryAsync(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
            string tableName,
            IEnumerable<QueryField> where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return QueryAsync(connection: connection,
                tableName: tableName,
                where: ToQueryGroup(where),
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return QueryAsyncInternal(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of dynamic objects.</returns>
        internal static Task<IEnumerable<dynamic>> QueryAsyncInternal(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
        {
            return QueryAsyncInternal<dynamic>(connection: connection,
                tableName: tableName,
                where: where,
                fields: fields,
                orderBy: orderBy,
                top: top,
                hints: hints,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region QueryInternalBase<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static IEnumerable<TEntity> QueryInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null)
            where TEntity : class
        {
            // Get Cache
            if (cache != null && cacheKey != null)
            {
                var item = cache.Get<IEnumerable<TEntity>>(cacheKey, false);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryRequest(tableName,
                connection,
                transaction,
                fields,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = CommandTextCache.GetQueryText(request);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Actual Execution
            var result = ExecuteQueryInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cache: null,
                trace: trace,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                cache.Add(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false);
            }

            // Result
            return result;
        }

        #endregion

        #region QueryAsyncInternalBase<TEntity>

        /// <summary>
        /// Query the existing rows from the table based on a given expression in an asynchronous way.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="fields">The mapping list of <see cref="Field"/> objects to be used.</param>
        /// <param name="where">The query expression to be used.</param>
        /// <param name="orderBy">The order definition of the fields to be used.</param>
        /// <param name="top">The number of rows to be returned.</param>
        /// <param name="hints">The table hints to be used.</param>
        /// <param name="cacheKey">The key to the cache item. By setting this argument, it will return the item from the cache if present, otherwise it will query the database. This will only work if the 'cache' argument is set.</param>
        /// <param name="cacheItemExpiration">The expiration in minutes of the cache item.</param>
        /// <param name="traceKey">The tracing key to be used.</param>
		/// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>An enumerable list of data entity objects.</returns>
        internal static async Task<IEnumerable<TEntity>> QueryAsyncInternalBase<TEntity>(this IDbConnection connection,
            string tableName,
            QueryGroup where,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = 0,
            string? hints = null,
            string? cacheKey = null,
            int? cacheItemExpiration = Constant.DefaultCacheItemExpirationInMinutes,
            int? commandTimeout = null,
            string traceKey = TraceKeys.Query,
            IDbTransaction? transaction = null,
            ICache? cache = null,
            ITrace? trace = null,
            IStatementBuilder? statementBuilder = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            // Get Cache
            if (cache != null && cacheKey != null)
            {
                var item = await cache.GetAsync<IEnumerable<TEntity>>(cacheKey, false, cancellationToken);
                if (item != null)
                {
                    return item.Value;
                }
            }

            // Variables
            var commandType = CommandType.Text;
            var request = new QueryRequest(tableName,
                connection,
                transaction,
                fields,
                where,
                orderBy,
                top,
                hints,
                statementBuilder);
            var commandText = await CommandTextCache.GetQueryTextAsync(request, cancellationToken);
            var param = (object)null;

            // Converts to property mapped object
            if (where != null)
            {
                param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            }

            // Actual Execution
            var result = await ExecuteQueryAsyncInternal<TEntity>(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                cacheKey: null,
                cacheItemExpiration: null,
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                cancellationToken: cancellationToken,
                cache: null,
                trace: trace,
                tableName: tableName,
                skipCommandArrayParametersCheck: true);

            // Set Cache
            if (cache != null && cacheKey != null)
            {
                await cache.AddAsync(cacheKey, result, cacheItemExpiration.GetValueOrDefault(), false, cancellationToken);
            }

            // Result
            return result;
        }

        #endregion
    }
}
