using RepoDb.Enumerations;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the composed command text used by the library.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> _cache = new ConcurrentDictionary<BaseRequest, string>();

        /// <summary>
        /// Gets a command text from the cache for the <i>BatchQuery</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetBatchQueryText<TEntity>(BatchQueryRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateBatchQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    page: request.Page,
                    rowsPerBatch: request.RowsPerBatch,
                    orderBy: request.OrderBy);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Count</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetCountText<TEntity>(CountRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateCount(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Delete</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteText<TEntity>(DeleteRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateDelete(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>DeleteAll</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetDeleteAllText<TEntity>(DeleteAllRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateDeleteAll(queryBuilder: new QueryBuilder<TEntity>());
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>InlineInsert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineInsertText<TEntity>(InlineInsertRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = ((SqlDbStatementBuilder)statementBuilder);
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString, Command.InlineInsert);
                    }
                    commandText = sqlStatementBuilder.CreateInlineInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        overrideIgnore: request.OverrideIgnore,
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInlineInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        overrideIgnore: request.OverrideIgnore);
                }
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>InlineMerge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineMergeText<TEntity>(InlineMergeRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = ((SqlDbStatementBuilder)statementBuilder);
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString, Command.InlineMerge);
                    }
                    commandText = sqlStatementBuilder.CreateInlineMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        qualifiers: request.Qualifiers,
                        overrideIgnore: request.OverrideIgnore,
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInlineMerge(queryBuilder: new QueryBuilder<TEntity>(),
                        fields: request.Fields,
                        qualifiers: request.Qualifiers,
                        overrideIgnore: request.OverrideIgnore);
                }
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>InlineUpdate</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineUpdateText<TEntity>(InlineUpdateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateInlineUpdate(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    fields: request.Fields,
                    overrideIgnore: request.OverrideIgnore);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Insert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInsertText<TEntity>(InsertRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = ((SqlDbStatementBuilder)statementBuilder);
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString, Command.Insert);
                    }
                    commandText = sqlStatementBuilder.CreateInsert(queryBuilder: new QueryBuilder<TEntity>(),
                        isPrimaryIdentity: isPrimaryIdentity);
                }
                else
                {
                    commandText = statementBuilder.CreateInsert(queryBuilder: new QueryBuilder<TEntity>());
                }
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Merge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetMergeText<TEntity>(MergeRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                var identity = IdentityCache.Get<TEntity>();
                if (identity != null && identity != primary)
                {
                    throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
                }
                var isPrimaryIdentity = (identity != null);
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = ((SqlDbStatementBuilder)statementBuilder);
                    if (isPrimaryIdentity == false)
                    {
                        isPrimaryIdentity = PrimaryKeyIdentityCache.Get<TEntity>(request.Connection.ConnectionString, Command.Merge);
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
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Query</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetQueryText<TEntity>(QueryRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    orderBy: request.OrderBy,
                    top: request.Top);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Truncate</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetTruncateText<TEntity>(TruncateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateTruncate(queryBuilder: new QueryBuilder<TEntity>());
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <i>Update</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetUpdateText<TEntity>(UpdateRequest request)
            where TEntity : class
        {
            var commandText = (string)null;
            if (_cache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateUpdate(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                _cache.TryAdd(request, commandText);
            }
            return commandText;
        }
    }
}
