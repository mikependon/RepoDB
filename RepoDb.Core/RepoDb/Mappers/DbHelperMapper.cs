using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map an instance of <see cref="IDbHelper"/> of into the type of <see cref="IDbConnection"/> object.
    /// </summary>
    public static class DbHelperMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<Type, IDbHelper> maps = new();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the type of <typeparamref name="TDbConnection"/> and an <see cref="IDbHelper"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="dbHelper">The instance of <see cref="IDbHelper"/> object to mapped to.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TDbConnection>(IDbHelper dbHelper,
            bool force)
            where TDbConnection : IDbConnection
        {
            var type = typeof(TDbConnection);
            
            // Try get the mappings
            if (maps.TryGetValue(type, out var existing))
            {
                if (force)
                {
                    maps.TryUpdate(type, dbHelper, existing);
                }
                else
                {
                    throw new MappingExistsException(type.FullName);
                }
            }
            else
            {
                maps.TryAdd(type, dbHelper);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the existing mapped <see cref="IDbHelper"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <returns>The instance of existing mapped <see cref="IDbHelper"/> object.</returns>
        public static IDbHelper Get<TDbConnection>()
            where TDbConnection : IDbConnection
        {
            // get the value
            maps.TryGetValue(typeof(TDbConnection), out var value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Get the existing mapped <see cref="IDbHelper"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <param name="connection">The instance of <see cref="IDbConnection"/>.</param>
        /// <returns>The instance of exising mapped <see cref="IDbHelper"/> object.</returns>
        public static IDbHelper Get<TDbConnection>(TDbConnection connection)
            where TDbConnection : IDbConnection
        {
            // get the value
            maps.TryGetValue(connection.GetType(), out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Remove the existing mapped <see cref="IDbHelper"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        public static void Remove<TDbConnection>()
            where TDbConnection : IDbConnection
        {
            // Variables for cache
            var key = typeof(TDbConnection);

            // Try get the the value
            maps.TryRemove(key, out _);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IDbHelper"/> objects.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion
    }
}
