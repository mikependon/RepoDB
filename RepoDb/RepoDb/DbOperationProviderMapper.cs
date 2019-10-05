using RepoDb.DbOperationProviders;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map the type of <see cref="DbConnection"/> into an instance of <see cref="IDbOperation"/> object.
    /// </summary>
    public static class DbOperationProviderMapper
    {
        private static readonly ConcurrentDictionary<int, IDbOperation> m_maps = new ConcurrentDictionary<int, IDbOperation>();
        private static Type m_type = typeof(DbConnection);

        static DbOperationProviderMapper()
        {
            // Default for SqlConnection
            Add(typeof(SqlConnection), new SqlServerOperation());
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
        /// Gets the mapped <see cref="IDbOperation"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of <see cref="IDbOperation"/> defined on the mapping.</returns>
        public static IDbOperation Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets the mapped <see cref="IDbOperation"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/>.</param>
        /// <returns>An instance of <see cref="IDbOperation"/> defined on the mapping.</returns>
        public static IDbOperation Get(Type type)
        {
            Guard(type);

            var value = (IDbOperation)null;

            if (m_maps.TryGetValue(type.FullName.GetHashCode(), out value))
            {
                return value;
            }

            throw new InvalidOperationException($"There is no existing database operation provider mapping for '{type.FullName}'.");
        }

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbOperation"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="statementBuilder">The statement builder to be mapped.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type, IDbOperation statementBuilder, bool @override = false)
        {
            Guard(type);

            var value = (IDbOperation)null;
            var key = type.FullName.GetHashCode();

            if (m_maps.TryGetValue(key, out value))
            {
                if (@override)
                {
                    m_maps.TryUpdate(key, statementBuilder, value);
                }
                else
                {
                    throw new InvalidOperationException($"Database operation provider mapping already exists ('{type.Name}' = '{value?.GetType().Name}').");
                }
            }
            else
            {
                m_maps.TryAdd(key, statementBuilder);
            }
        }
    }
}
