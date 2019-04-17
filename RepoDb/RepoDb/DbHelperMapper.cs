using RepoDb.DbHelpers;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class used to map a type of <see cref="DbConnection"/> into an instance of <see cref="IDbHelper"/> object.
    /// </summary>
    public static class DbHelperMapper
    {
        private static readonly ConcurrentDictionary<string, IDbHelper> m_maps = new ConcurrentDictionary<string, IDbHelper>();
        private static Type m_type = typeof(DbConnection);

        static DbHelperMapper()
        {
            // By default, map the Sql
            Add(typeof(SqlConnection), new SqlDbHelper());
        }

        /// <summary>
        /// Throws an exception if the type is not a sublcass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            if (type.IsSubclassOf(m_type) == false)
            {
                throw new InvalidOperationException($"Type must be a subclass of '{m_type.FullName}'.");
            }
        }

        /// <summary>
        /// Gets an existing <see cref="IDbHelper"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbHelper"/></returns>
        public static IDbHelper Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets an existing <see cref="IDbHelper"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <returns>An instance of mapped <see cref="IDbHelper"/></returns>
        public static IDbHelper Get(Type type)
        {
            Guard(type);
            var value = (IDbHelper)null;
            m_maps.TryGetValue(type.FullName, out value);
            return value;
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbHelper"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="dbHelper">The instance of database helper object to link to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type, IDbHelper dbHelper, bool @override = false)
        {
            Guard(type);

            var existing = (IDbHelper)null;
            var key = type.FullName;

            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    m_maps.TryUpdate(key, dbHelper, existing);
                }
                else
                {
                    throw new InvalidOperationException($"Mapping to provider '{key}' already exists.");
                }
            }
            else
            {
                m_maps.TryAdd(key, dbHelper);
            }
        }
    }
}
