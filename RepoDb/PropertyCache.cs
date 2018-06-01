using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the enumerable cached <i>System.Reflection.PropertyInfo</i> objects
    /// of a given <i>Data Transfer Object (DTO)</i> object for each command.
    /// </summary>
    public static class PropertyCache
    {
        private static readonly IDictionary<string, IEnumerable<PropertyInfo>> _cache = new Dictionary<string, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// Gets the cached <i>System.Reflection.PropertyInfo</i> objects of a given <i>Data Transfer Object (DTO)</i> object for each command.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the enumerable cached <i>System.Reflection.PropertyInfo</i> objects will be retrieved. This object must 
        /// implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="command">
        /// The target command where the enumerable cached <i>System.Reflection.PropertyInfo</i> objects has the implementation.
        /// </param>
        /// <returns>An enumerable of <i>System.Reflection.PropertyInfo</i> objects.</returns>
        public static IEnumerable<PropertyInfo> Get<TEntity>(Command command)
            where TEntity : IDataEntity
        {
            var value = (IEnumerable<PropertyInfo>)null;
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