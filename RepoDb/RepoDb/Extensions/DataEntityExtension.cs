using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for data entity object.
    /// </summary>
    public static class DataEntityExtension
    {
        // GetProperties
        internal static IEnumerable<ClassProperty> GetProperties(Type type,
            IDbSetting dbSetting)
        {
            return type
                .GetProperties()
                .Select(property => new ClassProperty(property, dbSetting));
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the list of the properties.</typeparam>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            return GetProperties(typeof(TEntity), dbSetting);
        }

        // GetMappedName
        internal static string GetMappedName(Type type,
            bool quoted,
            IDbSetting dbSetting)
        {
            var name = type.GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
            return quoted == true ? name.AsQuoted(true, dbSetting) : name;
        }

        /// <summary>
        /// Gets the mapped name of a data entity. It will return the value of <see cref="MapAttribute.Name"/> property. If the
        /// <see cref="MapAttribute"/> is not defined, then this will return the name of the class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="quoted">True whether the string is quoted.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>A mapped name for the data entity.</returns>
        public static string GetMappedName<TEntity>(bool quoted,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return GetMappedName(typeof(TEntity), quoted, dbSetting);
        }
    }
}