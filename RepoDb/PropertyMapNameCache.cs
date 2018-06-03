using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the enumerable cached <i>System.Reflection.PropertyInfo</i> objects
    /// <i>RepoDb.Attributes.MapAttribute.Name</i> property values for a given <i>Data Entity</i> object
    /// </summary>
    public static class PropertyMapNameCache
    {
        private static readonly IDictionary<string, IEnumerable<string>> _cache = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Gets the cached <i>System.Reflection.PropertyInfo</i> objects <i>RepoDb.Attributes.MapAttribute.Name</i> property
        /// values for a given <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the cached <i>System.Reflection.PropertyInfo</i> objects <i>RepoDb.Attributes.MapAttribute.Name</i> property
        /// values for a given <i>Data Entity</i> object.
        /// </typeparam>
        /// <param name="command">
        /// The target command where the cached <i>System.Reflection.PropertyInfo</i> objects has the implementation.
        /// </param>
        /// <returns>An enumerable of strings that signifies the mapped name ofs each property.</returns>
        public static IEnumerable<string> Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = (IEnumerable<string>)null;
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}".ToLower();
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
