using RepoDb.Requests;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the composed command text used by the library.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<QueryRequest, string> _queryCache = new ConcurrentDictionary<QueryRequest, string>();

        /// <summary>
        /// Gets a command text from the cache for <i>Query</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        public static string GetForQuery<TEntity>(QueryRequest request) where TEntity : DataEntity
        {
            var commandText = (string)null;
            if (_queryCache.TryGetValue(request, out commandText) == false)
            {
                var statementBuilder = (request.StatementBuilder ?? StatementBuilderMapper.Get(request.Connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateQuery(new QueryBuilder<TEntity>(), request.Where, request.OrderBy, request.Top);
                _queryCache.TryAdd(request, commandText);
            }
            return commandText;
        }
    }
}
