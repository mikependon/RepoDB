using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a type of <see cref="IDbConnection"/> into an instance of <see cref="IDbSetting"/> object.
    /// </summary>
    public static class DbSettingMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<Type, IDbSetting> maps = new();

        #endregion

        #region Methods
        
        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the type of <see cref="IDbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add<TDbConnection>(IDbSetting dbSetting,
            bool @override)
            where TDbConnection : IDbConnection
        {
            var key = typeof(TDbConnection);
            
            // Try get the mappings
            if (maps.TryGetValue(key, out var existing))
            {
                if (@override)
                {
                    // Override the existing one
                    maps.TryUpdate(key, dbSetting, existing);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The database setting mapping to provider '{key.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                maps.TryAdd(key, dbSetting);
            }
        }

        /*
        * Get
        */

        /// <summary>
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="IDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get<TDbConnection>()
            where TDbConnection : IDbConnection
        {
            // get the value
            maps.TryGetValue(typeof(TDbConnection), out var value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="IDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <param name="connection">The instance of <see cref="IDbConnection"/>.</param>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get<TDbConnection>(TDbConnection connection)
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
        /// Removes the mapping between the type of <see cref="IDbConnection"/> and an instance of <see cref="IDbSetting"/> object.
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

        /// <summary>
        /// Clears all the existing cached <see cref="IDbSetting"/> objects.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion
    }
}
