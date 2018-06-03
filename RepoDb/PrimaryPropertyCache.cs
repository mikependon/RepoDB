using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Interfaces;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached <i>RepoDb.Attributes.PrimaryAttribute</i> object that is implemented on a given <i>Data Entity</i> object.
    /// </summary>
    public static class PrimaryPropertyCache
    {
        private static readonly IDictionary<Type, PropertyInfo> _cache = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// Gets the first occurence <i>System.Reflection.PropertyInfo</i> object that has implemented
        /// the <i>RepoDb.Attributes.PrimaryAttribute</i> from a given <i>Data Entity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity where the cached <i>System.Reflection.PropertyInfo</i> object will be retrieved. This object must 
        /// implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <returns>An instance of <i>System.Reflection.PropertyInfo</i> object.</returns>
        public static PropertyInfo Get<TEntity>()
            where TEntity : IDataEntity
        {
            return Get(typeof(TEntity));
        }

        internal static PropertyInfo Get(Type type)
        {
            var value = (PropertyInfo)null;
            if (_cache.ContainsKey(type))
            {
                value = _cache[type];
            }
            else
            {
                value = DataEntityExtension.GetPrimaryProperty(type);
                _cache.Add(type, value);
            }
            return value;
        }
    }
}
