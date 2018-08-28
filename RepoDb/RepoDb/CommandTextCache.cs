using RepoDb.Enumerations;
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
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
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Count{TEntity}(IDbConnection, QueryGroup, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateCount(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Delete{TEntity}(IDbConnection, QueryGroup, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateDelete(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.DeleteAll{TEntity}(IDbConnection, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateDeleteAll(queryBuilder: new QueryBuilder<TEntity>());
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.InlineInsert{TEntity}(IDbConnection, object, bool?, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlDbStatementBuilder)statementBuilder;
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
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.InlineMerge{TEntity}(IDbConnection, object, bool?, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlDbStatementBuilder)statementBuilder;
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
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.InlineUpdate{TEntity}(IDbConnection, object, QueryGroup, bool?, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateInlineUpdate(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    fields: request.Fields,
                    overrideIgnore: request.OverrideIgnore);
                m_cache.TryAdd(request, commandText);
            }
            else
            {
                request.Where?.AppendParametersPrefix();
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Insert{TEntity}(IDbConnection, TEntity, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlDbStatementBuilder)statementBuilder;
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
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Merge{TEntity}(IDbConnection, TEntity, IEnumerable{Field}, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                if (statementBuilder is SqlDbStatementBuilder)
                {
                    var sqlStatementBuilder = (SqlDbStatementBuilder)statementBuilder;
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
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Query{TEntity}(IDbConnection, QueryGroup, IEnumerable{OrderField}, int?, string, int?, IDbTransaction, ICache, ITrace, IStatementBuilder, bool?, int?)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateQuery(queryBuilder: new QueryBuilder<TEntity>(),
                    where: request.Where,
                    orderBy: request.OrderBy,
                    top: request.Top);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Truncate{TEntity}(IDbConnection, int?, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateTruncate(queryBuilder: new QueryBuilder<TEntity>());
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the <see cref="DbConnectionExtension.Update{TEntity}(IDbConnection, TEntity, QueryGroup, int?, IDbTransaction, ITrace, IStatementBuilder)"/> operation.
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
                var statementBuilder = (request.StatementBuilder ??
                    StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ??
                    new SqlDbStatementBuilder());
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
    }
}
