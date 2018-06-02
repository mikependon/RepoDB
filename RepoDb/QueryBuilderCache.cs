using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached <i>RepodDb.Interfaces.IQueryBuilder</i> object on a
    /// given <i>Data Transfer Object (DTO)</i> object.
    /// </summary>
    public static class QueryBuilderCache
    {
        private static readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the cached <i>RepodDb.Interfaces.IQueryBuilder</i> object on a
        /// given <i>Data Transfer Object (DTO)</i> object.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the cached <i>RepodDb.Interfaces.IQueryBuilder</i> object will be retrieved. This object must 
        /// implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="createNew">Defines whether a new instance will be created if the object is not at the cache.</param>
        /// <returns>An instance of <i>RepoDb.Interfaces.IQueryBuilder</i> object bound for an entity.</returns>
        public static IQueryBuilder<TEntity> Get<TEntity>(bool createNew = true)
            where TEntity : IDataEntity
        {
            var value = (IQueryBuilder<TEntity>)null;
            var key = typeof(TEntity);
            if (_cache.ContainsKey(key))
            {
                value = (IQueryBuilder<TEntity>)_cache[key];
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
