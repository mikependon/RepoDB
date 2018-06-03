using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Attributes;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached <i>RepoDb.Attributes.MapAttribute</i> implemented at any <i>Data Entity</i> object.
    /// </summary>
    public static class ClassMapCache
    {
        private static readonly IDictionary<Type, MapAttribute> _cache = new Dictionary<Type, MapAttribute>();

        /// <summary>
        /// Gets the <i>RepoDb.Attributes.MapAttribute</i> object on a given data entity.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the <i>RepoDb.Attributes.MapAttribute</i> will be retrieved.
        /// This object must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <returns>An instance of <i>RepoDb.Attributes.MapAttribute</i> object mapped at the given data entity.</returns>
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
