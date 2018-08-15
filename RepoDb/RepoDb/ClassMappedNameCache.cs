using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name for the entity.
    /// </summary>
    public static class ClassMappedNameCache
    {
        private static readonly ConcurrentDictionary<string, string> m_cache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The cached command type of the entity.</returns>
        public static string Get<TEntity>(Command command = Command.None)
            where TEntity : class
        {
            return Get(typeof(TEntity), command);
        }

        /// <summary>
        /// Gets the cached mapped-name for the entity.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <param name="command">The target command.</param>
        /// <returns>The cached command type of the entity.</returns>
        public static string Get(Type type, Command command = Command.None)
        {
            var key = $"{type.FullName}.{command.ToString()}";
            var result = (string)null;
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = DataEntityExtension.GetMappedName(type, command);
                m_cache.TryAdd(key, result);
            }
            return result;
        }
    }
}
