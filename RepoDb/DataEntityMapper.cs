using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb
{
    /// <summary>
    /// A class that manage the mappings of a RepoDb.Interfaces.IDataEntity object into database object.
    /// </summary>
    public static class DataEntityMapper
    {
        private static readonly IDictionary<Type, object> _cache;
        private static readonly object _syncLock;

        static DataEntityMapper()
        {
            _cache = new Dictionary<Type, object>();
            _syncLock = new object();
        }

        /// <summary>
        /// Create a new entity and database mapping.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity type where to apply the mapping.</typeparam>
        /// <param name="command">The type of command to be used for mapping.</param>
        /// <returns>An instance of RepoDb.DataEntityMapperItem that is used for mapping.</returns>
        public static DataEntityMapperItem<TEntity> For<TEntity>()
            where TEntity : IDataEntity
        {
            var key = typeof(TEntity);
            var value = (DataEntityMapperItem<TEntity>)null;
            lock (_syncLock)
            {
                if (_cache.ContainsKey(key))
                {
                    value = (DataEntityMapperItem<TEntity>)_cache[key];
                }
                else
                {
                    value = new DataEntityMapperItem<TEntity>();
                    _cache.Add(key, value);
                }
            }
            return value;
        }
    }
}
