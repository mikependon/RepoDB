using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for data entity object.
    /// </summary>
    public static class DataEntityExtension
    {
        /// <summary>
        /// Converts the value to the type of the primary property of the target data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value to primary property type.</returns>
        internal static object ValueToPrimaryType<TEntity>(object value)
            where TEntity : class
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            var primary = PrimaryKeyCache.Get<TEntity>();
            if (primary != null)
            {
                if (primary.PropertyInfo.PropertyType == typeof(Guid))
                {
                    value = Guid.Parse(value.ToString());
                }
                else
                {
                    value = Convert.ChangeType(value, primary.PropertyInfo.PropertyType);
                }
            }
            return value;
        }

        // GetProperties
        internal static IEnumerable<ClassProperty> GetProperties(Type type)
        {
            return type
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

        // GetPropertyByAttribute

        internal static PropertyInfo GetPropertyByAttribute(Type type, Type attributeType)
        {
            return type
                .GetProperties()
                .FirstOrDefault(property => property.GetCustomAttribute(attributeType) != null);
        }

        internal static PropertyInfo GetPropertyByAttribute<TEntity>(Type attributeType)
            where TEntity : class
        {
            return GetPropertyByAttribute(typeof(TEntity), attributeType);
        }

        // GetMappedName
        internal static string GetMappedName(Type type)
        {
            return type.GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
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

        /// <summary>
        /// Converts the data entity object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity, QueryGroup queryGroup)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var properties = GetProperties(dataEntity.GetType());
            properties?
                .ToList()
                .ForEach(property =>
                {
                    expandObject[property.GetMappedName()] = property.PropertyInfo.GetValue(dataEntity);
                });
            queryGroup?
                .FixParameters()
                .GetAllQueryFields()?
                .ToList()
                .ForEach(queryField =>
                {
                    expandObject[queryField.Parameter.Name] = queryField.Parameter.Value;
                });
            return (ExpandoObject)expandObject;
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity)
        {
            return AsObject(dataEntity, null);
        }
    }
}