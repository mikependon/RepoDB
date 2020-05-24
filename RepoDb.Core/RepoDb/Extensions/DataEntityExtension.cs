using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity type as <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The list of <see cref="ClassProperty"/> objects.</returns>
        internal static IEnumerable<ClassProperty> GetProperties(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                yield return new ClassProperty(type, property);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity type as <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The list of <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>()
            where TEntity : class =>
            GetProperties(typeof(TEntity));

        // GetMappedName

        /// <summary>
        /// Gets the mapped name of a data entity. It will return the value of <see cref="MapAttribute.Name"/> property. If the
        /// <see cref="MapAttribute"/> is not defined, then this will return the name of the class.
        /// </summary>
        /// <param name="type">The type of the data entity where to get the mapped name.</param>
        /// <returns>A mapped name for the data entity.</returns>
        internal static string GetMappedName(Type type)
        {
            return type.GetCustomAttribute<MapAttribute>()?.Name ??
                   type.GetCustomAttribute<TableAttribute>()?.Name ??
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
            where TEntity : class =>
            GetMappedName(typeof(TEntity));
    }
}