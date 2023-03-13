using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for data entity object.
    /// </summary>
    public static class DataEntityExtension
    {
        /// <summary>
        /// Gets the list of <see cref="PropertyInfo"/> objects from the data entity type as <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The list of <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> GetProperties(Type type)
        {
            foreach (var property in TypeCache.Get(type).GetProperties())
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

        /// <summary>
        /// Gets the mapped name of the data entity from the <see cref="TableAttribute"/> object.
        /// </summary>
        /// <param name="tableAttribute">The table attribute to be checked.</param>
        /// <returns>The mapped name for the data entity.</returns>
        internal static string GetMappedName(TableAttribute tableAttribute)
        {
            if (tableAttribute == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(tableAttribute.Schema))
            {
                return tableAttribute.Name;
            }
            return string.Concat(tableAttribute.Schema, CharConstant.Period, tableAttribute.Name);
        }

        /// <summary>
        /// Gets the mapped name of the data entity. This will return the value of <see cref="MapAttribute.Name"/> and/or <see cref="TableAttribute.Name"/> property.
        /// If the both attributes are not defined, then this will return the name of the class.
        /// </summary>
        /// <param name="type">The type of the data entity where to get the mapped name.</param>
        /// <returns>The mapped name for the data entity.</returns>
        public static string GetMappedName(Type type) =>
            type.GetCustomAttribute<MapAttribute>()?.Name ?? GetMappedName(type.GetCustomAttribute<TableAttribute>()) ??
                ClassMapper.Get(type) ?? type.Name;

        /// <summary>
        /// Gets the mapped name of the data entity. This will return the value of <see cref="MapAttribute.Name"/> and/or <see cref="TableAttribute.Name"/> property.
        /// If the both attributes are not defined, then this will return the name of the class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity where to get the mapped name.</typeparam>
        /// <returns>The mapped name for the data entity.</returns>
        public static string GetMappedName<TEntity>() =>
            GetMappedName(typeof(TEntity));

        /// <summary>
        /// Gets the schema portion of the passed table name.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The schema of the passed table name.</returns>
        [Obsolete("Use the overloaded method instead.")]
        public static string GetSchema(string tableName) =>
            GetSchema(tableName, null);

        /// <summary>
        /// Gets the schema of the table name.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The schema of the table name.</returns>
        public static string GetSchema(string tableName,
            IDbSetting dbSetting)
        {
            if (tableName.IsOpenQuoted(dbSetting))
            {
                var index = tableName.IndexOf(string.Concat(dbSetting.ClosingQuote, CharConstant.Period), StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return tableName.Substring(0, index + 1);
                }
            }
            else
            {
                var index = tableName.IndexOf(CharConstant.Period);
                if (index >= 0)
                {
                    return tableName.Substring(0, index);
                }

            }

            return dbSetting?.DefaultSchema;
        }

        /// <summary>
        /// Gets the actual name of the table without the schema.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The actual table name.</returns>
        [Obsolete("Use the overloaded method instead.")]
        public static string GetTableName(string tableName) =>
            GetTableName(tableName, null);

        /// <summary>
        /// Gets the actual name of the table without the schema.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The actual table name.</returns>
        public static string GetTableName(string tableName,
            IDbSetting dbSetting)
        {
            if (tableName.IsOpenQuoted(dbSetting))
            {
                var index = tableName.IndexOf(string.Concat(dbSetting.ClosingQuote, CharConstant.Period), StringComparison.OrdinalIgnoreCase);
                if (index >= 0 && tableName.Length > index + 2)
                {
                    return tableName.Substring(index + 2);
                }
            }
            else
            {
                var index = tableName.IndexOf(CharConstant.Period);
                if (index >= 0 && tableName.Length > index)
                {
                    return tableName.Substring(index + 1);
                }

            }

            return tableName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static PropertyInfo GetPropertyOrThrow<TEntity>(string propertyName)
            where TEntity : class =>
            GetPropertyOrThrow(typeof(TEntity), propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static PropertyInfo GetPropertyOrThrow(Type type,
            string propertyName)
        {
            var property = TypeExtension.GetProperty(type, propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"The property '{propertyName}' is not found from type '{type.FullName}'.");
            }
            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static ClassProperty GetClassPropertyOrThrow<TEntity>(string propertyName)
            where TEntity : class =>
            GetClassPropertyOrThrow(typeof(TEntity), propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static ClassProperty GetClassPropertyOrThrow(Type type,
            string propertyName)
        {
            var property = PropertyCache
                .Get(type)?
                .FirstOrDefault(
                    p => string.Equals(p.PropertyInfo.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                throw new PropertyNotFoundException($"The class property '{propertyName}' is not found from type '{type.FullName}'.");
            }
            return property;
        }
    }
}