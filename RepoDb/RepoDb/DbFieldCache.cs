using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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
            return GetInternal(connection, tableName);
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
            using (var connection = (TDbConnection)Activator.CreateInstance(typeof(TDbConnection), new object[] { connectionString }))
            {
                return GetInternal(connection, tableName);
            }
        }

        /// <summary>
        /// Gets the cached field definitions of the entity.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static IEnumerable<DbField> GetInternal<TDbConnection>(TDbConnection connection, string tableName)
            where TDbConnection : IDbConnection
        {
            var type = connection.GetType();
            var key = (long)type.FullName.GetHashCode();
            var result = (IEnumerable<DbField>)null;

            // Note: For SqlConnection, the ConnectionString is changing if the (Integrated Security=False). Actually for this isolation, the database name is enough.
            if (!string.IsNullOrEmpty(connection?.Database))
            {
                key += connection.Database.GetHashCode();
            }

            // Add the hashcode of the table name
            if (string.IsNullOrEmpty(tableName) == false)
            {
                key += tableName.GetHashCode();
            }

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                var dbHelper = DbHelperMapper.Get(type);
                result = dbHelper?.GetFields(connection, tableName);
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
