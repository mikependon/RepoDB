using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            return type
                .GetTypeInfo()
                .GetProperties()
                .Select(property => new ClassProperty(property));
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
        internal static string GetMappedName(Type type, bool quoted = true)
        {
            var name = type.GetTypeInfo().GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
            return quoted == true ? name.AsQuoted(true) : name;
        }

        /// <summary>
        /// Gets the mapped name of a data entity. It will return the value of <see cref="MapAttribute.Name"/> property. If the
        /// <see cref="MapAttribute"/> is not defined, then this will return the name of the class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <returns>A mapped name for the data entity.</returns>
        public static string GetMappedName<TEntity>(bool quoted = true)
            where TEntity : class
        {
            return GetMappedName(typeof(TEntity), quoted);
        }
    }
}