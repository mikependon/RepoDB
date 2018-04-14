using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class PrimaryPropertyCache
    {
        private static readonly IDictionary<Type, PropertyInfo> _cache = new Dictionary<Type, PropertyInfo>();

        public static PropertyInfo Get<TEntity>()
            where TEntity : IDataEntity
        {
            var value = (PropertyInfo)null;
            var key = typeof(TEntity);
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetPrimaryProperty<TEntity>();
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
