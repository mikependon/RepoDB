using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the properties of the entity.
    /// </summary>
    public static class PropertyCache
    {
        private static readonly ConcurrentDictionary<int, IEnumerable<ClassProperty>> m_cache = new ConcurrentDictionary<int, IEnumerable<ClassProperty>>();

        #region Methods

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> Get<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            var properties = (IEnumerable<ClassProperty>)null;
            var key = typeof(TEntity).FullName.GetHashCode();

            // Add the DbSetting hashcode
            if (dbSetting != null)
            {
                key += dbSetting.GetHashCode();
            }

            // Try get the value
            if (m_cache.TryGetValue(key, out properties) == false)
            {
                properties = ClassExpression.GetProperties<TEntity>(dbSetting);
                m_cache.TryAdd(key, properties);
            }

            // Return the value
            return properties;
        }

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> Get(Type type,
            IDbSetting dbSetting)
        {
            var properties = (IEnumerable<ClassProperty>)null;
            var key = type.FullName.GetHashCode();

            // Add the DbSetting hashcode
            if (dbSetting != null)
            {
                key += dbSetting.GetHashCode();
            }

            // Try get the value
            if (m_cache.TryGetValue(key, out properties) == false)
            {
                properties = type.GetProperties().Select(p => new ClassProperty(p, dbSetting));
                m_cache.TryAdd(key, properties);
            }

            // Return the value
            return properties;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="ClassProperty"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
