using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the composed command text used by the library.
    /// </summary>
    internal static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        
        /// <summary>
        /// Gets a command text from the cache based on the combination of the arguments.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="connection">The connection object used on this operation.</param>
        /// <param name="where">The query expression used on this operation.</param>
        /// <param name="orderBy">The fields ordering used on this operation.</param>
        /// <param name="top">The row filtering used on this operation.</param>
        /// <param name="statementBuilder">The statement builder used on this operation.</param>
        /// <returns>The cached command text.</returns>
        public static string GetForQuery<TEntity>(IDbConnection connection, QueryGroup where, IEnumerable<OrderField> orderBy = null, int? top = null, IStatementBuilder statementBuilder = null) where TEntity : DataEntity
        {
            var commandText = (string)null;
            var key = $"entity:{typeof(TEntity).FullName}";
            if (where != null)
            {
                key = string.Concat(key, $"-where:{where.FixParameters().GetString()}");
            }
            if (orderBy != null)
            {
                key = string.Concat(key, $"-orderby:{orderBy?.Select(orderField => orderField.AsField()).Join(",")}");
            }
            if (top != null)
            {
                key = string.Concat(key, $"-top:{top}");
            }
            key = key.ToLower();
            if (_cache.TryGetValue(key, out commandText) == false)
            {
                statementBuilder = (statementBuilder ?? StatementBuilderMapper.Get(connection?.GetType())?.StatementBuilder ?? new SqlDbStatementBuilder());
                commandText = statementBuilder.CreateQuery(new QueryBuilder<TEntity>(), where, orderBy, top);
                _cache.TryAdd(key, commandText);
            }
            return commandText;
        }
    }
}
