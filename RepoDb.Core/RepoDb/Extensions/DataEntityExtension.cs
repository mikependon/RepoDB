using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Attributes;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for data entity object.
    /// </summary>
    public static class DataEntityExtension
    {
        // GetProperties
        internal static IEnumerable<ClassProperty> GetProperties(Type type)
        {
            //return type
            //    .GetProperties()
            //    .Select(property => new ClassProperty(type, property));
            foreach (var property in type.GetProperties())
            {
                yield return new ClassProperty(type, property);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the list of the properties.</typeparam>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>()
            where TEntity : class
        {
            return GetProperties(typeof(TEntity));
        }

        // GetMappedName
        internal static string GetMappedName(Type type)
        {
            return type.GetCustomAttribute<MapAttribute>()?.Name ??
                ClassMapper.Get(type) ??
                type.Name;
        }

        /// <summary>
        /// Gets the mapped name of a data entity. It will return the value of <see cref="MapAttribute.Name"/> property. If the
        /// <see cref="MapAttribute"/> is not defined, then this will return the name of the class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <returns>A mapped name for the data entity.</returns>
        public static string GetMappedName<TEntity>()
            where TEntity : class
        {
            return GetMappedName(typeof(TEntity));
        }
    }
}