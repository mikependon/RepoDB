using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the composed command text used by the library.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> m_cache = new ConcurrentDictionary<BaseRequest, string>();

        /// <summary>
        /// Gets a command text from the cache for the batch query operation.
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
                commandText = statementBuilder.CreateBatchQuery<TEntity>(new QueryBuilder(),
                    request.Where,
                    request.Page,
                    request.RowsPerBatch,
                    request.OrderBy);
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
                commandText = statementBuilder.CreateCount<TEntity>(new QueryBuilder(),
                    request.Where);
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
                commandText = statementBuilder.CreateDelete<TEntity>(new QueryBuilder(),
                    request.Where);
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
                commandText = statementBuilder.CreateDeleteAll<TEntity>(new QueryBuilder());
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
                var primaryField = DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateInlineInsert<TEntity>(new QueryBuilder(),
                    primaryField,
                    request.Fields);
                m_cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        /// Gets a command text from the cache for the inline-insert operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetInlineInsertText(InlineInsertRequest request)
        {
            var commandText = (string)null;
            if (m_cache.TryGetValue(request, out commandText) == false)
            {
                var primaryField = DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateInlineInsert(new QueryBuilder(),
                    request.Name,
                    primaryField,
                    request.Fields);
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
                var primaryField = DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateInlineMerge<TEntity>(new QueryBuilder(),
                    primaryField,
                    request.Fields,
                    request.Qualifiers);
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
                commandText = statementBuilder.CreateInlineUpdate<TEntity>(new QueryBuilder(),
                     request.Fields,
                     request.Where);
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
                var primaryField = DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateInsert<TEntity>(new QueryBuilder(), primaryField);
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
                var primaryField = DbFieldCache.Get(request.Connection, request.Name)?.FirstOrDefault(f => f.IsPrimary);
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMerge<TEntity>(new QueryBuilder(),
                    primaryField,
                    request.Qualifiers);
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
                commandText = statementBuilder.CreateQuery<TEntity>(new QueryBuilder(),
                     request.Where,
                     request.OrderBy,
                     request.Top,
                     request.Hints);
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
                commandText = statementBuilder.CreateQuery<TEntity>(new QueryBuilder(),
                     request.Where,
                     request.OrderBy,
                     request.Top,
                     request.Hints);
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
                commandText = statementBuilder.CreateTruncate<TEntity>(new QueryBuilder());
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
                commandText = statementBuilder.CreateUpdate<TEntity>(new QueryBuilder(),
                     request.Where);
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
            builder = builder ?? StatementBuilderMapper.Get(connection.GetType());
            if (builder == null)
            {
                throw new InvalidOperationException($"There is no '{nameof(IStatementBuilder)}' object defined. Please visit your mappings and make sure to map a type of '{nameof(DbConnection)}' to a correct '{nameof(IStatementBuilder)}' object.");
            }
            return builder;
        }
    }
}
