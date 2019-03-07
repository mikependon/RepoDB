using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the composed command text used by the library.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> m_cache = new ConcurrentDictionary<BaseRequest, string>();

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.BatchQuery{TEntity}(IDbConnection, QueryGroup, int, int, IEnumerable{OrderField}, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetBatchQueryText<TEntity>(BatchQueryRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateBatchQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    page: request.Page,
                    rowsPerBatch: request.RowsPerBatch,
                    orderBy: request.OrderBy);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the count operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetCountText<TEntity>(CountRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCount(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the delete operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteText<TEntity>(DeleteRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDelete(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the delete-all operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteAllText<TEntity>(DeleteAllRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDeleteAll(queryBuilder: new QueryBuilder<TEntity>());
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the inline-insert operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineInsertText<TEntity>(InlineInsertRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                if (statementBuilder is SqlStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlStatementBuilder)statementBuilder;
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString);
                    }
                    commandText = sqlStatementBuilder.CreateInlineInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInlineInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields);
                }
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the inline-merge operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineMergeText<TEntity>(InlineMergeRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                if (statementBuilder is SqlStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlStatementBuilder)statementBuilder;
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString);
                    }
                    commandText = sqlStatementBuilder.CreateInlineMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        qualifiers: request.Qualifiers,
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInlineMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        qualifiers: request.Qualifiers);
                }
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the inline-update operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineUpdateText<TEntity>(InlineUpdateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateInlineUpdate(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    fields: request.Fields);
                m_cache.TryAdd(request, commandText);
            }
            else
            {
                request.Where?.AppendParametersPrefix();
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the insert operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInsertText<TEntity>(InsertRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                if (statementBuilder is SqlStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlStatementBuilder)statementBuilder;
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString);
                    }
                    commandText = sqlStatementBuilder.CreateInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInsert(queryBuilder: new QueryBuilder<TEntity>());
                }
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the merge operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetMergeText<TEntity>(MergeRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                if (statementBuilder is SqlStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlStatementBuilder)statementBuilder;
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString);
                    }
                    commandText = sqlStatementBuilder.CreateMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        qualifiers: request.Qualifiers,
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        qualifiers: request.Qualifiers);
                }
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the query operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryText<TEntity>(QueryRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    orderBy: request.OrderBy,
                    top: request.Top,
                    hints: request.Hints);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the query-multiple operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryMultipleText<TEntity>(QueryMultipleRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    orderBy: request.OrderBy,
                    top: request.Top,
                    hints: request.Hints);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the truncate operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetTruncateText<TEntity>(TruncateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateTruncate(queryBuilder: new QueryBuilder<TEntity>());
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the update operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetUpdateText<TEntity>(UpdateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateUpdate(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                m_cache.TryAdd(request, commandText);
            }
            else
            {
                request.Where?.AppendParametersPrefix();
            }
            return commandText;
        }

        /// <summary>
        /// Throws an exception of the builder is not defined.
        /// </summary>
        /// <param name="connection">The connection object to identified.</param>
        /// <param name="builder">The builder to be checked.</param>
        /// <returns>The instance of available statement builder.</returns>
        private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection, IStatementBuilder builder)
        {
            builder = builder ?? StatementBuilderMapper.Get(connection.GetProvider())?.StatementBuilder;
            if (builder == null)
            {
                throw new InvalidOperationException("There is no 'IStatementBuilder' object defined. Please visit your mapping and make sure to map a 'Provider' to a correct 'IStatementBuilder' object.");
            }
            return builder;
        }
    }
}
