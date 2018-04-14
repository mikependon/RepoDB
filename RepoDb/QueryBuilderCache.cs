using System;
using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb
{
    public static class QueryBuilderCache
    {
        private static readonly IDictionary<Type, object> _queryBuilders;

        static QueryBuilderCache()
        {
            _queryBuilders = new Dictionary<Type, object>();
        }

        public static IQueryBuilder<TEntity> Get<TEntity>(bool createNew = true)
            where TEntity : IDataEntity
        {
            var queryBuilder = (IQueryBuilder<TEntity>)null;
            var type = typeof(TEntity);
            if (_queryBuilders.ContainsKey(type))
            {
                queryBuilder = (IQueryBuilder<TEntity>)_queryBuilders[type];
            }
            else
            {
                queryBuilder = new QueryBuilder<TEntity>();
                _queryBuilders.Add(type, queryBuilder);
            }
            return queryBuilder;
        }
    }
}
