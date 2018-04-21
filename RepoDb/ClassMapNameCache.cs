using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    public static class ClassMapNameCache
    {
        private static readonly IDictionary<string, string> _cache = new Dictionary<string, string>();

        public static string Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = (string)null;
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}".ToLower();
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetMappedName<TEntity>(command);
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
