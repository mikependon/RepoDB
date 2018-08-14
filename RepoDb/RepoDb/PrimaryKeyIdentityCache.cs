using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached value of <i>RepoDb.DataEntity</i> primary property <i>IsIdentity</i> identification.
    /// </summary>
    internal static class PrimaryKeyIdentityCache
    {
        private static readonly ConcurrentDictionary<string, bool> m_cache = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Gets the <i>RepoDb.Attributes.MapAttribute.Name</i> value implemented on the data entity on a target command.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionString">The connection string object to be used.</param>
        /// <param name="command">The target command.</param>
        /// <returns>A boolean value indicating the identification of the column.</returns>
        public static bool Get<TEntity>(string connectionString, Command command)
           where TEntity : class
        {
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}";
            var value = false;
            if (!m_cache.TryGetValue(key, out value))
            {
                var primary = PrimaryKeyCache.Get<TEntity>();
                if (primary != null)
                {
                    value = SqlDbHelper.IsIdentity<TEntity>(connectionString, command, PropertyMappedNameCache.Get(primary));
                }
                m_cache.TryAdd(key, value);
            }
            return value;
        }
    }
}
