using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map the type of <see cref="DbConnection"/> into an instance of <see cref="IStatementBuilder"/> object.
    /// </summary>
    public static class StatementBuilderMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, IStatementBuilder> maps = new ConcurrentDictionary<int, IStatementBuilder>();

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
        /// <param name="connectionType">The type of <see cref="DbConnection"/> object.</param>
        /// <param name="statementBuilder">The statement builder to be mapped.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type connectionType,
            IStatementBuilder statementBuilder,
            bool @override = false)
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
                    maps.TryUpdate(key, statementBuilder, existing);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The statement builder to provider '{connectionType.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                maps.TryAdd(key, statementBuilder);
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
            where TDbConnection : DbConnection =>
            Get(typeof(TDbConnection));

        /// <summary>
        /// Gets the mapped <see cref="IStatementBuilder"/> from the type of <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="connectionType">The type of <see cref="DbConnection"/>.</param>
        /// <returns>An instance of <see cref="IStatementBuilder"/> defined on the mapping.</returns>
        public static IStatementBuilder Get(Type connectionType)
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
        /// Removes the mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/>.</typeparam>
        public static void Remove<TDbConnection>()
            where TDbConnection : DbConnection =>
            Remove(typeof(TDbConnection));

        /// <summary>
        /// Removes an existing mapping between the type of <see cref="DbConnection"/> and an instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        /// <param name="connectionType">The type of <see cref="DbConnection"/> object.</param>
        public static void Remove(Type connectionType)
        {
            // Check the presence
            GuardPresence(connectionType);

            // Variables for cache
            var key = GenerateHashCode(connectionType);
            var existing = (IStatementBuilder)null;

            // Try to remove the value
            maps.TryRemove(key, out existing);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IStatementBuilder"/> objects.
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
                throw new NullReferenceException("Statement builder type.");
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
                throw new InvalidTypeException($"Type must be a subclass of '{StaticType.DbConnection}'.");
            }
        }

        #endregion
    }
}
