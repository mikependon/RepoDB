using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to map a .NET CLR type into its equivalent <see cref="DbType"/> object.
    /// </summary>
    public static class TypeMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> m_maps = new ConcurrentDictionary<int, DbType?>();

        #endregion

        static TypeMapper()
        {
            ConversionType = ConversionType.Default;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the conversion type when converting the instance of <see cref="DbDataReader"/> object into its destination .NET CLR Types.
        /// The default value is <see cref="ConversionType.Default"/>.
        /// </summary>
        public static ConversionType ConversionType { get; set; }

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add<T>(DbType dbType)
        {
            Add(typeof(T), dbType);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(DbType dbType,
            bool force)
        {
            Add(typeof(T), dbType, force);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        public static void Add(Type type,
            DbType dbType)
        {
            Add(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            DbType dbType,
            bool force)
        {
            var key = type.FullName.GetHashCode();
            var value = (DbType?)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"Mapping to '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, dbType);
            }
        }

        /*
         * Map
         */

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map<T>(DbType dbType)
        {
            Add(typeof(T), dbType);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type to be mapped.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map<T>(DbType dbType,
            bool force)
        {
            Add(typeof(T), dbType, force);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map(Type type,
            DbType dbType)
        {
            Add(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type to be mapped.</param>
        /// <param name="dbType">The <see cref="DbType"/> object where to map the .NET CLR type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        [Obsolete("Use the 'Add' method instead.")]
        public static void Map(Type type,
            DbType dbType,
            bool force)
        {
            Add(type, dbType, force);
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> object based on the .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The dynamic .NET CLR type used for mapping.</typeparam>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> object based on the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type used for mapping.</param>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get(Type type)
        {
            var value = (DbType?)null;
            var key = type.FullName.GetHashCode();

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type mapping to be removed.</typeparam>
        public static void Remove<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type mapping to be removed.</param>
        public static void Remove(Type type)
        {
            var key = type.FullName.GetHashCode();
            var value = (DbType?)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        /*
         * Unmap
         */

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type mapping to be removed.</typeparam>
        [Obsolete("Use the 'Remove' method instead.")]
        public static void Unmap<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes the mapping of the .NET CLR type and <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The .NET CLR type mapping to be removed.</param>
        [Obsolete("Use the 'Remove' method instead.")]
        public static void Unmap(Type type)
        {
            Remove(type);
        }

        #endregion
    }
}
