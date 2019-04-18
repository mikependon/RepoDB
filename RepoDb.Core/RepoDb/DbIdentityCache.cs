using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the database identity column of the table.
    /// </summary>
    public static class DbIdentityCache
    {
        private static readonly ConcurrentDictionary<string, DbField> m_cache = new ConcurrentDictionary<string, DbField>();

        /// <summary>
        /// Gets the cached identity column of the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached identity <see cref="DbField"/> object.</returns>
        public static DbField Get<TEntity>(IDbConnection connection)
            where TEntity : class
        {
            return Get(connection.GetType(), connection.ConnectionString, ClassMappedNameCache.Get<TEntity>());
        }

        /// <summary>
        /// Gets the cached identity column of the table.
        /// </summary>
        /// <param name="connection">The connection object to be used.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached identity <see cref="DbField"/> object.</returns>
        public static DbField Get(IDbConnection connection, string tableName)
        {
            return Get(connection.GetType(), connection.ConnectionString, tableName);
        }

        /// <summary>
        /// Gets the cached identity column of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of the <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <typeparam name="TEntity">The type of the target entity.</typeparam>
        /// <returns>The cached identity <see cref="DbField"/> object.</returns>
        public static DbField Get<TDbConnection, TEntity>(string connectionString)
            where TDbConnection : IDbConnection
            where TEntity : class
        {
            return Get(typeof(TDbConnection), connectionString, ClassMappedNameCache.Get<TEntity>());
        }

        /// <summary>
        /// Gets the cached identity column of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of the <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached identity <see cref="DbField"/> object.</returns>
        public static DbField Get<TDbConnection>(string connectionString, string tableName)
            where TDbConnection : IDbConnection
        {
            return Get(typeof(TDbConnection), connectionString, tableName);
        }

        /// <summary>
        /// Gets the cached identity column of the table.
        /// </summary>
        /// <param name="type">The type of <see cref="IDbConnection"/> object.</param>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The cached identity <see cref="DbField"/> object.</returns>
        public static DbField Get(Type type, string connectionString, string tableName)
        {
            var key = string.Concat(type.FullName, "-", connectionString, "-", tableName);
            var field = (DbField)null;
            if (m_cache.TryGetValue(key, out field) == false)
            {
                field = DbFieldCache.Get(type, connectionString, tableName).FirstOrDefault(f => f.IsIdentity == true);
                m_cache.TryAdd(key, field);
            }
            return field;
        }
    }
}
