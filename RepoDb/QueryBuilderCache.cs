using System;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached <i>RepoDb.QueryBuilder</i> object on a
    /// given <i>Data Entity</i> object.
    /// </summary>
    public static class QueryBuilderCache
    {
        private static readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the cached <i>RepoDb.QueryBuilder</i> object on a given <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the cached <i>RepoDb.QueryBuilder</i> object will be retrieved.
        /// </typeparam>
        /// <param name="createNew">Defines whether a new instance will be created if the object is not at the cache.</param>
        /// <returns>An instance of <i>RepoDb.QueryBuilder</i> object bound for an entity.</returns>
        public static QueryBuilder<TEntity> Get<TEntity>(bool createNew = true)
            where TEntity : DataEntity
        {
            var value = (QueryBuilder<TEntity>)null;
            var key = typeof(TEntity);
            if (_cache.ContainsKey(key))
            {
                value = (QueryBuilder<TEntity>)_cache[key];
            }
            else
            {
                value = new QueryBuilder<TEntity>();
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
