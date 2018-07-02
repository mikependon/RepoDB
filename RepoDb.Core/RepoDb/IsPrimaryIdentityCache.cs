using System.Collections.Generic;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A static class used to get the cached value of <i>RepoDb.DataEntity</i> primary property <i>IsIdentity</i> identification.
    /// </summary>
    internal static class IsPrimaryIdentityCache
    {
        private static readonly IDictionary<string, bool> _cache = new Dictionary<string, bool>();
        private static object _syncLock = new object();

        /// <summary>
        /// Gets the <i>RepoDb.Attributes.MapAttribute.Name</i> value implemented on the data entity on a target command.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionString">The connection string object to be used.</param>
        /// <param name="command">The target command.</param>
        /// <returns>A boolean value indicating the identification of the column.</returns>
        public static bool Get<TEntity>(string connectionString, Command command)
            where TEntity : DataEntity
        {
            var key = $"{typeof(TEntity).FullName}.{command.ToString()}".ToLower();
            var value = false;
            lock (_syncLock)
            {
                if (_cache.ContainsKey(key))
                {
                    value = _cache[key];
                }
                else
                {
                    var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
                    if (primary != null)
                    {
                        value = SqlDbHelper.IsIdentity<TEntity>(connectionString, command, primary.Name);
                    }
                    _cache.Add(key, value);
                }
            }
            return value;
        }
    }
}
