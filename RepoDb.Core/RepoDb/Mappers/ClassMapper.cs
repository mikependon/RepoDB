using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a class into its equivalent database object (ie: Table, View). This is an alternative class to <see cref="MapAttribute"/> object for class mapping.
    /// </summary>
    public static class ClassMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, string> maps = new();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a database object (i.e.: Table, View).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="name">The name of the database object (ie: Table, View).</param>
        public static void Add<TEntity>(string name)
            where TEntity : class =>
            Add(typeof(TEntity), name);

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a database object (i.e.: Table, View).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="name">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string name,
            bool force)
            where TEntity : class =>
            Add(typeof(TEntity), name, force);

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a database object (i.e.: Table, View).
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="name">The name of the database object (ie: Table, View).</param>
        public static void Add(Type type,
            string name) =>
            Add(type, name, false);

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a database object (i.e.: Table, View).
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="name">The name of the database object (ie: Table, View).</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            string name,
            bool force)
        {
            // Validate
            StringExtension.ThrowIfNullOrWhiteSpace(name, "name");

            // Variables
            var key = type.GetHashCode();

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, name, value);
                }
                else
                {
                    throw new MappingExistsException("The mappings are already existing.");
                }
            }
            else
            {
                maps.TryAdd(key, name);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the existing mapped database object of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <returns>The mapped name of the class.</returns>
        public static string Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Get the existing mapped database object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The mapped name of the class.</returns>
        public static string Get(Type type)
        {
            var key = type.GetHashCode();

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Remove the exising mapped database object on the .NET CLR type.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        public static void Remove<TEntity>()
            where TEntity : class =>
            Remove(typeof(TEntity));

        /// <summary>
        /// Remove the exising mapped database object on the .NET CLR type.
        /// </summary>
        /// <param name="type">The target type.</param>
        public static void Remove(Type type)
        {
            var key = type.GetHashCode();

            // Try get the value
            maps.TryRemove(key, out var _);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached objects.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion
    }
}
