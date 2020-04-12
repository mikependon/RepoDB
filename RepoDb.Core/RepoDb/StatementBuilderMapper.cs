using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map the type of <see cref="DbConnection"/> into an instance of <see cref="IStatementBuilder"/> object.
    /// </summary>
    public static class StatementBuilderMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, IStatementBuilder> m_maps = new ConcurrentDictionary<int, IStatementBuilder>();
        private static Type m_type = typeof(DbConnection);

        #endregion

        #region Methods

        /*
         * Add
         */

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="statementBuilder">The instance of <see cref="IStatementBuilder"/> object to mapped to.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add<TDbConnection>(IStatementBuilder statementBuilder,
            bool @override)
            where TDbConnection : DbConnection =>
            Add(typeof(TDbConnection), statementBuilder, @override);

        /// <summary>
        /// Adds a mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="statementBuilder">The statement builder to be mapped.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            IStatementBuilder statementBuilder,
            bool @override = false)
        {
            // Guard the type
            Guard(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (IStatementBuilder)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, statementBuilder, existing);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The statement builder to provider '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                m_maps.TryAdd(key, statementBuilder);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped <see cref="IStatementBuilder"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <returns>An instance of <see cref="IStatementBuilder"/> defined on the mapping.</returns>
        public static IStatementBuilder Get<TDbConnection>()
            where TDbConnection : DbConnection
        {
            return Get(typeof(TDbConnection));
        }

        /// <summary>
        /// Gets the mapped <see cref="IStatementBuilder"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/>.</param>
        /// <returns>An instance of <see cref="IStatementBuilder"/> defined on the mapping.</returns>
        public static IStatementBuilder Get(Type type)
        {
            // Guard the type
            Guard(type);

            // Variables for the cache
            var value = (IStatementBuilder)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        /// <param name="throwException">If true, it throws an exception if the mapping is not present.</param>
        /// <returns>True if the removal is successful, otherwise false.</returns>
        public static bool Remove<TDbConnection>(bool throwException = true)
            where TDbConnection : DbConnection =>
            Remove(typeof(TDbConnection), throwException);

        /// <summary>
        /// Removes an existing mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <param name="type">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="throwException">If true, it throws an exception if the mapping is not present.</param>
        /// <returns>True if the removal is successful, otherwise false.</returns>
        public static bool Remove(Type type,
            bool throwException = true)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (IStatementBuilder)null;
            var result = m_maps.TryRemove(key, out existing);

            // Throws an exception if necessary
            if (result == false && throwException == true)
            {
                throw new MissingMappingException($"There is no mapping defined for '{type.FullName}'.");
            }

            // Return false
            return result;
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IStatementBuilder"/> objects.
        /// </summary>
        public static void Clear()
        {
            m_maps.Clear();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Statement builder type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type is not a sublcass of type <see cref="DbConnection"/>.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            if (type.IsSubclassOf(m_type) == false)
            {
                throw new InvalidTypeException($"Type must be a subclass of '{m_type.FullName}'.");
            }
        }

        #endregion
    }
}
