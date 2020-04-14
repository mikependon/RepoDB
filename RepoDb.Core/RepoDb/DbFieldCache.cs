using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the list of <see cref="DbField"/> of the database table.
    /// </summary>
    public static class DbFieldCache
    {
        private static readonly ConcurrentDictionary<long, IEnumerable<DbField>> m_cache = new ConcurrentDictionary<long, IEnumerable<DbField>>();

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="DbField"/> objects.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static IEnumerable<DbField> Get(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            return GetInternal(connection, tableName, transaction);
        }

        /// <summary>
        /// Gets the cached list of <see cref="DbField"/> of the database table based on the data entity mapped name in asynchronous way.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        public static Task<IEnumerable<DbField>> GetAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            return GetInternalAsync(connection, tableName, transaction);
        }

        /// <summary>
        /// Gets the cached field definitions of the entity.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static IEnumerable<DbField> GetInternal<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction)
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
                result = dbHelper?.GetFields(connection, tableName, transaction);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        /// <summary>
        /// Gets the cached field definitions of the entity in an asychronous way.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the <see cref="IDbConnection"/> object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The cached field definitions of the entity.</returns>
        internal static async Task<IEnumerable<DbField>> GetInternalAsync<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
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
                if (dbHelper != null)
                {
                    result = await dbHelper?.GetFieldsAsync(connection, tableName, transaction);
                }
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion
    }
}
