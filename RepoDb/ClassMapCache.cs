using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Attributes;

namespace RepoDb
{
    public static class ClassMapCache
    {
        private static readonly IDictionary<Type, MapAttribute> _cache = new Dictionary<Type, MapAttribute>();

        public static MapAttribute Get<TEntity>()
            where TEntity : IDataEntity
        {
            return Get(typeof(TEntity));
        }

        internal static MapAttribute Get(Type type)
        {
            var value = (MapAttribute)null;
            if (_cache.ContainsKey(type))
            {
                value = _cache[type];
            }
            else
            {
                value = type.GetCustomAttribute<MapAttribute>();
                _cache.Add(type, value);
            }
            return value;
        }
    }
}
