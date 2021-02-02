using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a type of <see cref="DbConnection"/> into an instance of <see cref="IDbSetting"/> object.
    /// </summary>
    public static class DbSettingMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, IDbSetting> maps = new ConcurrentDictionary<int, IDbSetting>();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add<TDbConnection>(IDbSetting dbSetting,
            bool @override)
            where TDbConnection : DbConnection =>
            Add(typeof(TDbConnection), dbSetting, @override);

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <param name="connectionType">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="dbSetting">The instance of <see cref="IDbSetting"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type connectionType,
            IDbSetting dbSetting,
            bool @override)
        {
            // Guard the type
            Guard(connectionType);

            // Variables for cache
            var key = GenerateHashCode(connectionType);

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
                    throw new MappingExistsException($"The database setting mapping to provider '{connectionType.FullName}' already exists.");
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
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get<TDbConnection>()
            where TDbConnection : DbConnection =>
            Get(typeof(TDbConnection));

        /// <summary>
        /// Gets an existing <see cref="IDbSetting"/> object that is mapped to type <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="connectionType">The type of <see cref="DbConnection"/> object.</param>
        /// <returns>An instance of mapped <see cref="IDbSetting"/></returns>
        public static IDbSetting Get(Type connectionType)
        {
            // Guard the type
            Guard(connectionType);

            // get the value
            maps.TryGetValue(GenerateHashCode(connectionType), out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        public static void Remove<TDbConnection>()
            where TDbConnection : DbConnection =>
            Remove(typeof(TDbConnection));

        /// <summary>
        /// Removes the mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IDbSetting"/> object.
        /// </summary>
        /// <param name="connectionType">The type of <see cref="DbConnection"/> object.</param>
        public static void Remove(Type connectionType)
        {
            // Check the presence
            GuardPresence(connectionType);

            // Variables for cache
            var key = GenerateHashCode(connectionType);
            var existing = (IDbSetting)null;

            // Try get the the value
            maps.TryRemove(key, out existing);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IDbSetting"/> objects.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Database setting type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type is not a subclass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            if (type.IsSubclassOf(StaticType.DbConnection) == false)
            {
                throw new InvalidTypeException($"Type must be a subclass of '{StaticType.DbConnection.FullName}'.");
            }
        }

        #endregion
    }
}
