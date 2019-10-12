using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name for the entity.
    /// </summary>
    public static class ClassMappedNameCache
    {
        private static readonly ConcurrentDictionary<int, string> m_cache = new ConcurrentDictionary<int, string>();

        #region Methods

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        public static string Get<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            return Get<TEntity>(true, dbSetting);
        }

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        public static string Get<TEntity>(bool quoted,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return Get(typeof(TEntity), quoted, dbSetting);
        }

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <param name="type">The type of the target entity.</param>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The cached mapped name of the entity.</returns>
        internal static string Get(Type type,
            bool quoted,
            IDbSetting dbSetting)
        {
            var key = type.FullName.GetHashCode() + quoted.GetHashCode();
            var result = (string)null;

            // Add the DbSetting hashcode
            if (dbSetting != null)
            {
                key += dbSetting.GetHashCode();
            }

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = DataEntityExtension.GetMappedName(type, quoted, dbSetting);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached class mapped names.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
