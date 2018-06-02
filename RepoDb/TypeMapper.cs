using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// A static class used to map the .Net CLR Types into database types.
    /// </summary>
    public static class TypeMapper
    {
        private static readonly IList<ITypeMap> _typeMaps = new List<ITypeMap>();

        static TypeMapper()
        {
            new List<ITypeMap>();
        }

        /// <summary>
        /// Gets the list of type-mapping objects.
        /// </summary>
        public static IEnumerable<ITypeMap> TypeMaps => _typeMaps;

        /// <summary>
        /// Adds a mapping between .Net CLR Type and database type.
        /// </summary>
        /// <param name="type">The .Net CLR Type to be mapped.</param>
        /// <param name="dbType">The database type where to map the .Net CLR Type.</param>
        public static void AddMap(Type type, DbType dbType)
        {
            AddMap(new TypeMap(type, dbType));
        }

        /// <summary>
        /// Adds a mapping between .Net CLR Type and database type.
        /// </summary>
        /// <param name="typeMap">The instance of type-mapping object that holds the mapping of .Net CLR Type and database type.</param>
        public static void AddMap(ITypeMap typeMap)
        {
            var target = Get(typeMap.Type);
            if (target != null)
            {
                throw new DuplicateTypeMapException(target.Type);
            }
            _typeMaps.Add(typeMap);
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .Net CLR Type and database type.
        /// </summary>
        /// <param name="type">The .Net CLR Type used for mapping.</param>
        /// <returns>The instance of type-mapping object that holds the mapping of .Net CLR Type and database type.</returns>
        public static ITypeMap Get(Type type)
        {
            return _typeMaps.FirstOrDefault(t => t.Type == type);
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .Net CLR Type and database type.
        /// </summary>
        /// <typeparam name="T">The dynamic .Net CLR Type used for mapping.</typeparam>
        /// <returns>The instance of type-mapping object that holds the mapping of .Net CLR Type and database type.</returns>
        public static ITypeMap Get<T>()
        {
            return Get(typeof(T));
        }
    }
}
