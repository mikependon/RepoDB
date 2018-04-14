using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb
{
    public static class QueryBuilderCache
    {
        private static readonly IDictionary<Type, object> _cache;

        static QueryBuilderCache()
        {
            _cache = new Dictionary<Type, object>();
        }

        public static IQueryBuilder<TEntity> Get<TEntity>(bool createNew = true)
            where TEntity : IDataEntity
        {
            var queryBuilder = (IQueryBuilder<TEntity>)null;
            var type = typeof(TEntity);
            if (_cache.ContainsKey(type))
            {
                queryBuilder = (IQueryBuilder<TEntity>)_cache[type];
            }
            else
            {
                queryBuilder = new QueryBuilder<TEntity>();
                _cache.Add(type, queryBuilder);
            }
            return queryBuilder;
        }
    }
}
