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

        // GetDataEntityChildrenData

        /// <summary>
        /// Gets the recursive data of the target data entity object.
        /// </summary>
        /// <param name="type">The type of the target data entity.</param>
        /// <returns>An enumerable list of <see cref="DataEntityChildListData"/> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData(Type type)
        {
            return type?
                .GetProperties()
                .Where(property => property.IsRecursive())
                .Select(property => new DataEntityChildListData()
                {
                    ChildListType = property.PropertyType.GetGenericArguments().First(),
                    ParentDataEntityType = type,
                    ChildListProperty = property
                });
        }

        /// <summary>
        /// Gets the recursive data of the target data entity object.
        /// </summary>
        /// <typeparam name="T">The type of the target data entity.</typeparam>
        /// <returns>An enumerable list of <see cref="DataEntityChildListData"/> object.</returns>
        internal static IEnumerable<DataEntityChildListData> GetDataEntityChildrenData<T>()
        {
            return GetDataEntityChildrenData(typeof(T));
        }

        // GetPropertiesFor
        internal static IEnumerable<ClassProperty> GetPropertiesFor(Type type, Command command)
        {
            return type
                .GetProperties()
                .Select(property => new ClassProperty(property))
                .Where(property =>
                    property.IsIgnored(command) == false &&
                    property.IsRecursive() == false);
        }

        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity class based on the
        /// target command.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the list of the properties.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>The list of data entity properties based on the target command.</returns>
        public static IEnumerable<ClassProperty> GetPropertiesFor<TEntity>(Command command)
            where TEntity : class
        {
            return GetPropertiesFor(typeof(TEntity), command);
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
        internal static string GetMappedName(Type type, Command command)
        {
            return type.GetCustomAttribute<MapAttribute>()?.Name ?? type.Name;
        }

        /// <summary>
        /// Gets the mapped name of the data entity type on a target command. The identification process it to check the <see cref="MapAttribute"/>
        /// and get the value of the name property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <param name="command">The target command.</param>
        /// <returns>A string that contains the mapped name for the target command.</returns>
        public static string GetMappedName<TEntity>(Command command)
            where TEntity : class
        {
            return GetMappedName(typeof(TEntity), command);
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object. During the conversion, the passed query groups are being merged.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <param name="queryGroup">The query group to be merged.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity, QueryGroup queryGroup, Command command)
        {
            var expandObject = new ExpandoObject() as IDictionary<string, object>;
            var properties = GetPropertiesFor(dataEntity.GetType(), command);
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
            return AsObject(dataEntity, null, Command.None);
        }

        /// <summary>
        /// Converts the data entity object into a dynamic object.
        /// </summary>
        /// <param name="dataEntity">The data entity object to be converted.</param>
        /// <param name="command">The target command type to be used for the object transformation.</param>
        /// <returns>An instance of converted dynamic object.</returns>
        internal static object AsObject(this object dataEntity, Command command)
        {
            return AsObject(dataEntity, null, command);
        }
    }
}
