using System.Collections.Generic;
using RepoDb.Interfaces;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached value of <i>RepoDb.Attributes.MapAttribute.Name</i> property implemented at
    /// any <i>Data Transfer Object (DTO)</i> object.
    /// </summary>
    public static class ClassMapNameCache
    {
        private static readonly IDictionary<string, string> _cache = new Dictionary<string, string>();

        /// <summary>
        /// Gets the <i>RepoDb.Attributes.MapAttribute.Name</i> value implemented on the data entity on a target command.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the <i>RepoDb.Attributes.MapAttribute.Name</i> property value will be retrieved.
        /// This object must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="command">The target command where to get the mapped name of the data entity.</param>
        /// <returns>A string value that signifies the mapped name for the entity object on a target command.</returns>
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
