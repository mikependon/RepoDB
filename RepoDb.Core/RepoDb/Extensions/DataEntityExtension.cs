using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        // TODO: Remove the filter in the 'TEntity'

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
        private static string GetMappedName(TableAttribute tableAttribute)
        {
            if (tableAttribute == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(tableAttribute.Schema))
            {
                return tableAttribute.Name;
            }
            return string.Concat(tableAttribute.Schema, StringConstant.Period, tableAttribute.Name);
        }

        /// <summary>
        /// Gets the mapped name of the data entity. This will return the value of <see cref="MapAttribute.Name"/> and/or <see cref="TableAttribute.Name"/> property.
        /// If the both attributes are not defined, then this will return the name of the class.
        /// </summary>
        /// <param name="type">The type of the data entity where to get the mapped name.</param>
        /// <returns>The mapped name for the data entity.</returns>
        internal static string GetMappedName(Type type) =>
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
        /// Gets the actual schema of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table schema.</returns>
        public static string GetSchema(string tableName) =>
            GetSchema(tableName, null);

        /// <summary>
        /// Gets the actual schema of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The actual table schema.</returns>
        public static string GetSchema(string tableName,
            IDbSetting dbSetting)
        {
            // Get the schema and table name
            var index = tableName.IndexOf(".");
            if (index > 0)
            {
                return tableName.Substring(0, index).AsUnquoted(true, dbSetting);
            }

            // Return the unquoted
            return dbSetting.DefaultSchema;
        }

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table name.</returns>
        public static string GetTableName(string tableName) =>
            GetTableName(tableName, null);

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The actual table name.</returns>
        public static string GetTableName(string tableName,
            IDbSetting dbSetting)
        {
            // Get the schema and table name
            var index = tableName.IndexOf(".");
            if (index > 0)
            {
                if (tableName.Length > index)
                {
                    return tableName.Substring(index + 1).AsUnquoted(true, dbSetting);
                }
            }

            // Return the unquoted
            return tableName.AsUnquoted(true, dbSetting);
        }
    }
}