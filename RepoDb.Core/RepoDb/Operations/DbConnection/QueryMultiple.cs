using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection;
using RepoDb.Requests;
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
        #region QueryMultiple<TEntity>

        #region T1, T2

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The dynamic expression or the key value to be used (for T1).</param>
        /// <param name="what2">The dynamic expression or the key value to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            object what1,
            object what2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: WhatToQueryGroup(typeof(T1), connection, what1, transaction),
                where2: WhatToQueryGroup(typeof(T2), connection, what2, transaction),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleInternal<T1, T2>(connection: connection,
                where1: where1,
                where2: where2,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultipleInternal<T1, T2>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleInternal<T1, T2, T3>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultipleInternal<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request3));
                maps.Add(where3.MapTo<T3>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // T3
                if (item3 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T3>(), transaction);
                    item3 = DataReader.ToEnumerable<T3>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2, item3);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultiple<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
            QueryMultipleInternal<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request3));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request4));
                maps.Add(where4.MapTo<T4>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // T3
                if (item3 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T3>(), transaction);
                    item3 = DataReader.ToEnumerable<T3>(reader, dbFields, dbSetting)?.AsList();
                }

                // T4
                if (item4 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T4>(), transaction);
                    item4 = DataReader.ToEnumerable<T4>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultiple<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>
            QueryMultipleInternal<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request3));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request4));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request5));
                maps.Add(where5.MapTo<T5>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // T3
                if (item3 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T3>(), transaction);
                    item3 = DataReader.ToEnumerable<T3>(reader, dbFields, dbSetting)?.AsList();
                }

                // T4
                if (item4 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T4>(), transaction);
                    item4 = DataReader.ToEnumerable<T4>(reader, dbFields, dbSetting)?.AsList();
                }

                // T5
                if (item5 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T5>(), transaction);
                    item5 = DataReader.ToEnumerable<T5>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="what6">The query expression or the key value to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            object what6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                where6: WhatToQueryGroup(what6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            QueryField where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<QueryField> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultiple<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
            QueryMultipleInternal<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            var item6 = EnsureQueryMultipleCachedItem<T6>(cacheKey6,
                cache,
                where6,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request3));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request4));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request5));
                maps.Add(where5.MapTo<T5>());
            }

            // Item6 Request
            if (item6 == null)
            {
                var request6 = new QueryMultipleRequest(6,
                    typeof(T6),
                    connection,
                    transaction,
                    FieldCache.Get<T6>(),
                    where6,
                    orderBy6,
                    top6,
                    hints6,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request6));
                maps.Add(where6.MapTo<T6>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // T3
                if (item3 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T3>(), transaction);
                    item3 = DataReader.ToEnumerable<T3>(reader, dbFields, dbSetting)?.AsList();
                }

                // T4
                if (item4 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T4>(), transaction);
                    item4 = DataReader.ToEnumerable<T4>(reader, dbFields, dbSetting)?.AsList();
                }

                // T5
                if (item5 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T5>(), transaction);
                    item5 = DataReader.ToEnumerable<T5>(reader, dbFields, dbSetting)?.AsList();
                }

                // T6
                if (item6 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T6>(), transaction);
                    item6 = DataReader.ToEnumerable<T6>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5, item6);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="what6">The query expression or the key value to be used (for T6).</param>
        /// <param name="what7">The query expression or the key value to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            object what6,
            object what7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                where6: WhatToQueryGroup(what6),
                where7: WhatToQueryGroup(what7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            QueryField where6,
            QueryField where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<QueryField> where6,
            IEnumerable<QueryField> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultiple<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        internal static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
            QueryMultipleInternal<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            var item6 = EnsureQueryMultipleCachedItem<T6>(cacheKey6,
                cache,
                where6,
                queryGroups);

            var item7 = EnsureQueryMultipleCachedItem<T7>(cacheKey7,
                cache,
                where7,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T1>(request1));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request2));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request3));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request4));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request5));
                maps.Add(where5.MapTo<T5>());
            }

            // Item6 Request
            if (item6 == null)
            {
                var request6 = new QueryMultipleRequest(6,
                    typeof(T6),
                    connection,
                    transaction,
                    FieldCache.Get<T6>(),
                    where6,
                    orderBy6,
                    top6,
                    hints6,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request6));
                maps.Add(where6.MapTo<T6>());
            }

            // Item7 Request
            if (item7 == null)
            {
                var request7 = new QueryMultipleRequest(7,
                    typeof(T7),
                    connection,
                    transaction,
                    FieldCache.Get<T7>(),
                    where7,
                    orderBy7,
                    top7,
                    hints7,
                    statementBuilder);
                commandTexts.Add(CommandTextCache.GetQueryMultipleText<T2>(request7));
                maps.Add(where7.MapTo<T7>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)ExecuteReaderInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T1>(), transaction);
                    item1 = DataReader.ToEnumerable<T1>(reader, dbFields, dbSetting)?.AsList();
                }

                // T2
                if (item2 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T2>(), transaction);
                    item2 = DataReader.ToEnumerable<T2>(reader, dbFields, dbSetting)?.AsList();
                }

                // T3
                if (item3 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T3>(), transaction);
                    item3 = DataReader.ToEnumerable<T3>(reader, dbFields, dbSetting)?.AsList();
                }

                // T4
                if (item4 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T4>(), transaction);
                    item4 = DataReader.ToEnumerable<T4>(reader, dbFields, dbSetting)?.AsList();
                }

                // T5
                if (item5 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T5>(), transaction);
                    item5 = DataReader.ToEnumerable<T5>(reader, dbFields, dbSetting)?.AsList();
                }

                // T6
                if (item6 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T6>(), transaction);
                    item6 = DataReader.ToEnumerable<T6>(reader, dbFields, dbSetting)?.AsList();
                }

                // T7
                if (item7 == null)
                {
                    reader?.NextResult();
                    dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<T7>(), transaction);
                    item7 = DataReader.ToEnumerable<T7>(reader, dbFields, dbSetting)?.AsList();
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5, item6, item7);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #endregion

        #region QueryMultipleAsync<TEntity>

        #region T1, T2

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            object what1,
            object what2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleAsyncInternal<T1, T2>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleAsyncInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleAsyncInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleAsyncInternal<T1, T2>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsync<T1, T2>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            return QueryMultipleAsyncInternal<T1, T2>(connection: connection,
                where1: where1,
                where2: where2,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                cacheKey1: cacheKey1,
                top2: top2,
                orderBy2: orderBy2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cache: cache,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 2 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 2 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> QueryMultipleAsyncInternal<T1, T2>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            int? top2 = 0,
            IEnumerable<OrderField> orderBy2 = null,
            string hints2 = null,
            string cacheKey2 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsync<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 3 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> QueryMultipleAsyncInternal<T1, T2, T3>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T3>(request3, cancellationToken));
                maps.Add(where3.MapTo<T3>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T3
                if (item3 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T3>(), transaction, true, cancellationToken);
                    item3 = await DataReader.ToEnumerableAsync<T3>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2, item3);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 4 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsync<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 3 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 4 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>
            QueryMultipleAsyncInternal<T1, T2, T3, T4>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T3>(request3, cancellationToken));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T4>(request4, cancellationToken));
                maps.Add(where4.MapTo<T4>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T3
                if (item3 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T3>(), transaction, true, cancellationToken);
                    item3 = await DataReader.ToEnumerableAsync<T3>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T4
                if (item4 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T4>(), transaction, true, cancellationToken);
                    item4 = await DataReader.ToEnumerableAsync<T4>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
            QueryMultipleAsyncInternal<T1, T2, T3, T4, T5>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T3>(request3, cancellationToken));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T4>(request4, cancellationToken));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T5>(request5, cancellationToken));
                maps.Add(where5.MapTo<T5>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T3
                if (item3 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T3>(), transaction, true, cancellationToken);
                    item3 = await DataReader.ToEnumerableAsync<T3>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T4
                if (item4 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T4>(), transaction, true, cancellationToken);
                    item4 = await DataReader.ToEnumerableAsync<T4>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T5
                if (item5 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T5>(), transaction, true, cancellationToken);
                    item5 = await DataReader.ToEnumerableAsync<T5>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="what6">The query expression or the key value to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            object what6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                where6: WhatToQueryGroup(what6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 5 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 5 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where5),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            QueryField where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<QueryField> where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 6 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 6 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            var item6 = EnsureQueryMultipleCachedItem<T6>(cacheKey6,
                cache,
                where6,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T3>(request3, cancellationToken));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T4>(request4, cancellationToken));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T5>(request5, cancellationToken));
                maps.Add(where5.MapTo<T5>());
            }

            // Item6 Request
            if (item6 == null)
            {
                var request6 = new QueryMultipleRequest(6,
                    typeof(T6),
                    connection,
                    transaction,
                    FieldCache.Get<T6>(),
                    where6,
                    orderBy6,
                    top6,
                    hints6,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T6>(request6, cancellationToken));
                maps.Add(where6.MapTo<T6>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T3
                if (item3 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T3>(), transaction, true, cancellationToken);
                    item3 = await DataReader.ToEnumerableAsync<T3>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T4
                if (item4 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T4>(), transaction, true, cancellationToken);
                    item4 = await DataReader.ToEnumerableAsync<T4>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T5
                if (item5 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T5>(), transaction, true, cancellationToken);
                    item5 = await DataReader.ToEnumerableAsync<T5>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T6
                if (item6 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T6>(), transaction, true, cancellationToken);
                    item6 = await DataReader.ToEnumerableAsync<T6>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5, item6);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #region T1, T2, T3, T4, T5, T6, T7

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="what1">The query expression or the key value to be used (for T1).</param>
        /// <param name="what2">The query expression or the key value to be used (for T2).</param>
        /// <param name="what3">The query expression or the key value to be used (for T3).</param>
        /// <param name="what4">The query expression or the key value to be used (for T4).</param>
        /// <param name="what5">The query expression or the key value to be used (for T5).</param>
        /// <param name="what6">The query expression or the key value to be used (for T6).</param>
        /// <param name="what7">The query expression or the key value to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            object what1,
            object what2,
            object what3,
            object what4,
            object what5,
            object what6,
            object what7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: WhatToQueryGroup(what1),
                where2: WhatToQueryGroup(what2),
                where3: WhatToQueryGroup(what3),
                where4: WhatToQueryGroup(what4),
                where5: WhatToQueryGroup(what5),
                where6: WhatToQueryGroup(what6),
                where7: WhatToQueryGroup(what7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            Expression<Func<T1, bool>> where1,
            Expression<Func<T2, bool>> where2,
            Expression<Func<T3, bool>> where3,
            Expression<Func<T4, bool>> where4,
            Expression<Func<T5, bool>> where5,
            Expression<Func<T6, bool>> where6,
            Expression<Func<T7, bool>> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryField where1,
            QueryField where2,
            QueryField where3,
            QueryField where4,
            QueryField where5,
            QueryField where6,
            QueryField where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            IEnumerable<QueryField> where1,
            IEnumerable<QueryField> where2,
            IEnumerable<QueryField> where3,
            IEnumerable<QueryField> where4,
            IEnumerable<QueryField> where5,
            IEnumerable<QueryField> where6,
            IEnumerable<QueryField> where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: ToQueryGroup(where1),
                where2: ToQueryGroup(where2),
                where3: ToQueryGroup(where3),
                where4: ToQueryGroup(where4),
                where5: ToQueryGroup(where5),
                where6: ToQueryGroup(where6),
                where7: ToQueryGroup(where7),
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        public static Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            return QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(connection: connection,
                where1: where1,
                where2: where2,
                where3: where3,
                where4: where4,
                where5: where5,
                where6: where6,
                where7: where7,
                orderBy1: orderBy1,
                top1: top1,
                hints1: hints1,
                orderBy2: orderBy2,
                top2: top2,
                hints2: hints2,
                cacheKey2: cacheKey2,
                orderBy3: orderBy3,
                top3: top3,
                hints3: hints3,
                cacheKey3: cacheKey3,
                orderBy4: orderBy4,
                top4: top4,
                hints4: hints4,
                cacheKey4: cacheKey4,
                orderBy5: orderBy5,
                top5: top5,
                hints5: hints5,
                cacheKey5: cacheKey5,
                orderBy6: orderBy6,
                top6: top6,
                hints6: hints6,
                cacheKey6: cacheKey6,
                orderBy7: orderBy7,
                top7: top7,
                hints7: hints7,
                cacheKey7: cacheKey7,
                commandTimeout: commandTimeout,
                transaction: transaction,
                trace: trace,
                statementBuilder: statementBuilder,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Query the data as multiple resultsets from the table based on the given 7 target types in an asynchronous way.
        /// </summary>
        /// <typeparam name="T1">The first target type.</typeparam>
        /// <typeparam name="T2">The second target type.</typeparam>
        /// <typeparam name="T3">The third target type.</typeparam>
        /// <typeparam name="T4">The fourth target type.</typeparam>
        /// <typeparam name="T5">The fifth target type.</typeparam>
        /// <typeparam name="T6">The sixth target type.</typeparam>
        /// <typeparam name="T7">The seventh target type.</typeparam>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="where1">The query expression to be used (for T1).</param>
        /// <param name="where2">The query expression to be used (for T2).</param>
        /// <param name="where3">The query expression to be used (for T3).</param>
        /// <param name="where4">The query expression to be used (for T4).</param>
        /// <param name="where5">The query expression to be used (for T5).</param>
        /// <param name="where6">The query expression to be used (for T6).</param>
        /// <param name="where7">The query expression to be used (for T7).</param>
        /// <param name="orderBy1">The order definition of the fields to be used (for T1).</param>
        /// <param name="top1">The number of rows to be returned (for T1).</param>
        /// <param name="hints1">The table hints to be used (for T1).</param>
        /// <param name="cacheKey1">
        /// The key to the cache item 1. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy2">The order definition of the fields to be used (for T2).</param>
        /// <param name="top2">The number of rows to be returned (for T2).</param>
        /// <param name="hints2">The table hints to be used (for T2).</param>
        /// <param name="cacheKey2">
        /// The key to the cache item 2. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy3">The order definition of the fields to be used (for T3).</param>
        /// <param name="top3">The number of rows to be returned (for T3).</param>
        /// <param name="hints3">The table hints to be used (for T3).</param>
        /// <param name="cacheKey3">
        /// The key to the cache item 3. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy4">The order definition of the fields to be used (for T4).</param>
        /// <param name="top4">The number of rows to be returned (for T4).</param>
        /// <param name="hints4">The table hints to be used (for T4).</param>
        /// <param name="cacheKey4">
        /// The key to the cache item 4. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy5">The order definition of the fields to be used (for T5).</param>
        /// <param name="top5">The number of rows to be returned (for T5).</param>
        /// <param name="hints5">The table hints to be used (for T5).</param>
        /// <param name="cacheKey5">
        /// The key to the cache item 5. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy6">The order definition of the fields to be used (for T6).</param>
        /// <param name="top6">The number of rows to be returned (for T6).</param>
        /// <param name="hints6">The table hints to be used (for T6).</param>
        /// <param name="cacheKey6">
        /// The key to the cache item 6. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="orderBy7">The order definition of the fields to be used (for T7).</param>
        /// <param name="top7">The number of rows to be returned (for T7).</param>
        /// <param name="hints7">The table hints to be used (for T7).</param>
        /// <param name="cacheKey7">
        /// The key to the cache item 7. By setting this argument, it will return the item from the cache if present, otherwise it will query the database.
        /// This will only work if the 'cache' argument is set.
        /// </param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cache">The cache object to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="statementBuilder">The statement builder object to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A tuple of 7 enumerable target data entity types.</returns>
        internal static async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            QueryMultipleAsyncInternal<T1, T2, T3, T4, T5, T6, T7>(this IDbConnection connection,
            QueryGroup where1,
            QueryGroup where2,
            QueryGroup where3,
            QueryGroup where4,
            QueryGroup where5,
            QueryGroup where6,
            QueryGroup where7,
            IEnumerable<OrderField> orderBy1 = null,
            int? top1 = 0,
            string hints1 = null,
            string cacheKey1 = null,
            IEnumerable<OrderField> orderBy2 = null,
            int? top2 = 0,
            string hints2 = null,
            string cacheKey2 = null,
            IEnumerable<OrderField> orderBy3 = null,
            int? top3 = 0,
            string hints3 = null,
            string cacheKey3 = null,
            IEnumerable<OrderField> orderBy4 = null,
            int? top4 = 0,
            string hints4 = null,
            string cacheKey4 = null,
            IEnumerable<OrderField> orderBy5 = null,
            int? top5 = 0,
            string hints5 = null,
            string cacheKey5 = null,
            IEnumerable<OrderField> orderBy6 = null,
            int? top6 = 0,
            string hints6 = null,
            string cacheKey6 = null,
            IEnumerable<OrderField> orderBy7 = null,
            int? top7 = 0,
            string hints7 = null,
            string cacheKey7 = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            ITrace trace = null,
            IStatementBuilder statementBuilder = null,
            CancellationToken cancellationToken = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
        {
            // Variables
            var commandType = CommandType.Text;
            var queryGroups = new List<QueryGroup>();
            var maps = new List<QueryGroupTypeMap>();
            var commandTexts = new List<string>();

            // Items
            var item1 = EnsureQueryMultipleCachedItem<T1>(cacheKey1,
                cache,
                where1,
                queryGroups);

            var item2 = EnsureQueryMultipleCachedItem<T2>(cacheKey2,
                cache,
                where2,
                queryGroups);

            var item3 = EnsureQueryMultipleCachedItem<T3>(cacheKey3,
                cache,
                where3,
                queryGroups);

            var item4 = EnsureQueryMultipleCachedItem<T4>(cacheKey4,
                cache,
                where4,
                queryGroups);

            var item5 = EnsureQueryMultipleCachedItem<T5>(cacheKey5,
                cache,
                where5,
                queryGroups);

            var item6 = EnsureQueryMultipleCachedItem<T6>(cacheKey6,
                cache,
                where6,
                queryGroups);

            var item7 = EnsureQueryMultipleCachedItem<T7>(cacheKey7,
                cache,
                where7,
                queryGroups);

            // Fix
            QueryGroup.FixForQueryMultiple(queryGroups.ToArray());

            // Item1 Request
            if (item1 == null)
            {
                var request1 = new QueryMultipleRequest(1,
                    typeof(T1),
                    connection,
                    transaction,
                    FieldCache.Get<T1>(),
                    where1,
                    orderBy1,
                    top1,
                    hints1,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T1>(request1, cancellationToken));
                maps.Add(where1.MapTo<T1>());
            }

            // Item2 Request
            if (item2 == null)
            {
                var request2 = new QueryMultipleRequest(2,
                    typeof(T2),
                    connection,
                    transaction,
                    FieldCache.Get<T2>(),
                    where2,
                    orderBy2,
                    top2,
                    hints2,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T2>(request2, cancellationToken));
                maps.Add(where2.MapTo<T2>());
            }

            // Item3 Request
            if (item3 == null)
            {
                var request3 = new QueryMultipleRequest(3,
                    typeof(T3),
                    connection,
                    transaction,
                    FieldCache.Get<T3>(),
                    where3,
                    orderBy3,
                    top3,
                    hints3,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T3>(request3, cancellationToken));
                maps.Add(where3.MapTo<T3>());
            }

            // Item4 Request
            if (item4 == null)
            {
                var request4 = new QueryMultipleRequest(4,
                    typeof(T4),
                    connection,
                    transaction,
                    FieldCache.Get<T4>(),
                    where4,
                    orderBy4,
                    top4,
                    hints4,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T4>(request4, cancellationToken));
                maps.Add(where4.MapTo<T4>());
            }

            // Item5 Request
            if (item5 == null)
            {
                var request5 = new QueryMultipleRequest(5,
                    typeof(T5),
                    connection,
                    transaction,
                    FieldCache.Get<T5>(),
                    where5,
                    orderBy5,
                    top5,
                    hints5,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T5>(request5, cancellationToken));
                maps.Add(where5.MapTo<T5>());
            }

            // Item6 Request
            if (item6 == null)
            {
                var request6 = new QueryMultipleRequest(6,
                    typeof(T6),
                    connection,
                    transaction,
                    FieldCache.Get<T6>(),
                    where6,
                    orderBy6,
                    top6,
                    hints6,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T6>(request6, cancellationToken));
                maps.Add(where6.MapTo<T6>());
            }

            // Item7 Request
            if (item7 == null)
            {
                var request7 = new QueryMultipleRequest(7,
                    typeof(T7),
                    connection,
                    transaction,
                    FieldCache.Get<T7>(),
                    where7,
                    orderBy7,
                    top7,
                    hints7,
                    statementBuilder);
                commandTexts.Add(await CommandTextCache.GetQueryMultipleTextAsync<T7>(request7, cancellationToken));
                maps.Add(where7.MapTo<T7>());
            }

            // Shared variables
            var commandText = string.Join(" ", commandTexts);
            var param = QueryGroup.AsMappedObject(maps.ToArray(), false);
            var sessionId = Guid.Empty;

            // Before Execution
            if (trace != null)
            {
                sessionId = Guid.NewGuid();
                var cancellableTraceLog = new CancellableTraceLog(sessionId, commandText, param, null);
                trace.BeforeQueryMultiple(cancellableTraceLog);
                if (cancellableTraceLog.IsCancelled)
                {
                    if (cancellableTraceLog.IsThrowException)
                    {
                        throw new CancelledExecutionException(commandText);
                    }
                    return null;
                }
                commandText = (cancellableTraceLog.Statement ?? commandText);
                param = (cancellableTraceLog.Parameter ?? param);
            }

            // Before Execution Time
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> result;
            var beforeExecutionTime = DateTime.UtcNow;

            // Actual Execution
            using (var reader = (DbDataReader)(await ExecuteReaderAsyncInternal(connection: connection,
                commandText: commandText,
                param: param,
                commandType: commandType,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                entityType: null,
                dbFields: null,
                skipCommandArrayParametersCheck: true)))
            {
                var dbSetting = connection.GetDbSetting();
                var dbFields = (IEnumerable<DbField>)null;

                // T1
                if (item1 == null)
                {
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T1>(), transaction, true, cancellationToken);
                    item1 = await DataReader.ToEnumerableAsync<T1>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T2
                if (item2 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T2>(), transaction, true, cancellationToken);
                    item2 = await DataReader.ToEnumerableAsync<T2>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T3
                if (item3 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T3>(), transaction, true, cancellationToken);
                    item3 = await DataReader.ToEnumerableAsync<T3>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T4
                if (item4 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T4>(), transaction, true, cancellationToken);
                    item4 = await DataReader.ToEnumerableAsync<T4>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T5
                if (item5 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T5>(), transaction, true, cancellationToken);
                    item5 = await DataReader.ToEnumerableAsync<T5>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T6
                if (item6 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T6>(), transaction, true, cancellationToken);
                    item6 = await DataReader.ToEnumerableAsync<T6>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // T7
                if (item7 == null)
                {
                    await reader.NextResultAsync(cancellationToken);
                    dbFields = await DbFieldCache.GetAsync(connection, ClassMappedNameCache.Get<T7>(), transaction, true, cancellationToken);
                    item7 = await DataReader.ToEnumerableAsync<T7>(reader, dbFields, dbSetting, cancellationToken).ToListAsync(cancellationToken);
                }

                // Result
                result = Tuple.Create(item1, item2, item3, item4, item5, item6, item7);
            }

            // After Execution
            trace?.AfterQueryMultiple(new TraceLog(sessionId, commandText, param, result,
                DateTime.UtcNow.Subtract(beforeExecutionTime)));

            // Result
            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cache"></param>
        /// <param name="where"></param>
        /// <param name="queryGroups"></param>
        /// <returns></returns>
        private static IEnumerable<T> EnsureQueryMultipleCachedItem<T>(string cacheKey,
            ICache cache,
            QueryGroup where,
            List<QueryGroup> queryGroups)
            where T : class
        {
            var item = cache?.Get<IEnumerable<T>>(cacheKey, false)?.Value;

            if (item == null)
            {
                queryGroups.Add(where);
            }

            return item;
        }

        #endregion
    }
}
