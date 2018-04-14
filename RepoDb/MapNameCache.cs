using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class MapNameCache
    {
        private static readonly IDictionary<Type, string> _cache = new Dictionary<Type, string>();

        public static string Get<TEntity>()
            where TEntity : IDataEntity
        {
            var value = (string)null;
            var key = typeof(TEntity);
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetMappedName<TEntity>();
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
