using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb
{
    public static class QueryBuilderCache
    {
        private static readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

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
