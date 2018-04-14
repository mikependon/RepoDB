using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class MapNameCache
    {
        private static readonly IDictionary<Type, string> _cache;

        static MapNameCache()
        {
            _cache = new Dictionary<Type, string>();
        }

        public static string Get<TEntity>()
            where TEntity : IDataEntity
        {
            var mappedName = (string)null;
            var type = typeof(TEntity);
            if (_cache.ContainsKey(type))
            {
                mappedName = _cache[type];
            }
            else
            {
                mappedName = DataEntityExtension.GetMappedName<TEntity>();
                _cache.Add(type, mappedName);
            }
            return mappedName;
        }
    }
}
