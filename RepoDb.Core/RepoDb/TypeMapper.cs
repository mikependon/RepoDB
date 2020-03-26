using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to map a .NET CLR Type into its equivalent database type.
    /// </summary>
    public static class TypeMapper
    {
        private static readonly ConcurrentDictionary<int, DbType?> m_maps = new ConcurrentDictionary<int, DbType?>();

        static TypeMapper()
        {
            ConversionType = ConversionType.Default;
        }

        /// <summary>
        /// Gets or sets the conversion type when converting the instance of <see cref="DbDataReader"/> object into its destination .NET CLR types.
        /// The default value is <see cref="ConversionType.Default"/>.
        /// </summary>
        public static ConversionType ConversionType { get; set; }

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type where to map the .NET CLR Type.</param>
        public static void Map(Type type,
            DbType dbType)
        {
            Map(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type where to map the .NET CLR Type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Map(Type type,
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
                    throw new MissingMappingException($"Mapping to '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, dbType);
            }
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type used for mapping.</param>
        /// <returns>The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</returns>
        public static DbType? Get(Type type)
        {
            var value = (DbType?)null;
            var key = type.FullName.GetHashCode();

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .NET CLR Type and database type.
        /// </summary>
        /// <typeparam name="T">The dynamic .NET CLR Type used for mapping.</typeparam>
        /// <returns>The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</returns>
        public static DbType? Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Removes a mapping of targetted .NET CLR Type from the collection.
        /// </summary>
        /// <param name="type">The .NET CLR Type mapping to be removed.</param>
        public static void Unmap(Type type)
        {
            var key = type.FullName.GetHashCode();
            var value = (DbType?)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }
    }
}
