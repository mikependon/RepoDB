using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    public static class PropertyCache
    {
        private static readonly IDictionary<string, IEnumerable<PropertyInfo>> _cache;

        static PropertyCache()
        {
            _cache = new Dictionary<string, IEnumerable<PropertyInfo>>();
        }

        public static IEnumerable<PropertyInfo> GetFor<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = (IEnumerable<PropertyInfo>)null;
            // var key = $"{MapNameCache.Get<TEntity>()}.{command.ToString()}"; // Will fail if there is multiple same mapping
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}";
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = DataEntityExtension.GetPropertiesFor<TEntity>(command);
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
