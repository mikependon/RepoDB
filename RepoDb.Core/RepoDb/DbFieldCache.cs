using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the list of <see cref="DbField"/> of the database table.
    /// </summary>
    public static class DbFieldCache
    {
        private static readonly ConcurrentDictionary<long, IEnumerable<DbField>> m_cache = new ConcurrentDictionary<long, IEnumerable<DbField>>();

        #region Methods

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static IEnumerable<DbField> Get(IDbConnection connection, string tableName)
        {
            return Get(connection?.GetType(), connection?.ConnectionString, tableName);
        }

        /// <summary>
        /// Gets the cached field definitions of the entity.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of the <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connectionString">The connection string to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static IEnumerable<DbField> Get<TDbConnection>(string connectionString, string tableName)
            where TDbConnection : IDbConnection
        {
            return Get(typeof(TDbConnection), connectionString, tableName);
        }

        /// <summary>
        /// Gets the cached field definitions of the entity.
        /// </summary>
        /// <param name="type">The type of <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static IEnumerable<DbField> Get(Type type, string connectionString, string tableName)
        {
            var key = (long)type?.FullName.GetHashCode();
            var result = (IEnumerable<DbField>)null;

            // Set the keys
            if (string.IsNullOrEmpty(connectionString) == false)
            {
                key += connectionString.GetHashCode();
            }
            if (string.IsNullOrEmpty(tableName) == false)
            {
                key += tableName.GetHashCode();
            }

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                var dbHelper = DbHelperMapper.Get(type);
                result = dbHelper?.GetFields(connectionString, tableName);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="DbField"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion
    }
}
