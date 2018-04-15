using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb
{
    public static class PropertyMapNameCache
    {
        private static readonly IDictionary<string, IEnumerable<string>> _cache = new Dictionary<string, IEnumerable<string>>();

        public static IEnumerable<string> Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = (IEnumerable<string>)null;
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}";
            if (_cache.ContainsKey(key))
            {
                value = _cache[key];
            }
            else
            {
                value = PropertyCache
                    .Get<TEntity>(command)?
                    .Select(property => property.GetMappedName());
                _cache.Add(key, value);
            }
            return value;
        }
    }
}
