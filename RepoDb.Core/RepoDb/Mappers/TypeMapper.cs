using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a .NET CLR type or class property into its equivalent <see cref="DbType"/> object.
    /// </summary>
    public static class TypeMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> maps = new();

        #endregion

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="TType">The target type.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object to map.</param>
        public static void Add<TType>(DbType? dbType) =>
            Add(typeof(TType), dbType);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="TType">The target type.</typeparam>
        /// <param name="dbType">The <see cref="DbType"/> object to map.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType>(DbType? dbType,
            bool force) =>
            Add(typeof(TType), dbType, force);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="dbType">The <see cref="DbType"/> object to map.</param>
        public static void Add(Type type,
            DbType? dbType) =>
            Add(type, dbType, false);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="dbType">The <see cref="DbType"/> object to map.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            DbType? dbType,
            bool force)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    throw new MappingExistsException($"The mappings are already existing.");
                }
            }
            else
            {
                maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Type Level: Get the existing mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The dynamic .NET CLR type used for mapping.</typeparam>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get<TType>() =>
            Get(typeof(TType));

        /// <summary>
        /// Type Level: Get the existing mapped <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type used for mapping.</param>
        /// <returns>The instance of the mapped <see cref="DbType"/> object.</returns>
        public static DbType? Get(Type type)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Type Level: Remove the existing mapped <see cref="DbType"/> object from the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type where the mapping is to be removed.</typeparam>
        public static void Remove<TType>() =>
            Remove(typeof(TType));

        /// <summary>
        /// Type Level: Remove the existing mapped <see cref="DbType"/> object from the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type where the mapping is to be removed.</param>
        public static void Remove(Type type)
        {
            var key = type.GetHashCode();

            // Try get the value
            maps.TryRemove(key, out var _);
        }

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(expression, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            DbType? dbType,
            bool force)
            where TEntity : class =>
            Add<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression), dbType, force);

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(string propertyName,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(propertyName, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="dbType">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string propertyName,
            DbType? dbType,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(propertyName, "PropertyName");

            // Add to the mapping
            Add<TEntity>(DataEntityExtension.GetPropertyOrThrow<TEntity>(propertyName), dbType, force);
        }

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        public static void Add<TEntity>(Field field,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(field, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            DbType? dbType,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(field, "Field");

            // Add to the mapping
            Add<TEntity>(DataEntityExtension.GetPropertyOrThrow<TEntity>(field.Name), dbType, force);
        }

        /// <summary>
        /// Property Level: Adds a mapping between a class property and a <see cref="DbType"/> object (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        internal static void Add<TEntity>(PropertyInfo propertyInfo,
            DbType? dbType)
            where TEntity : class =>
            Add<TEntity>(propertyInfo, dbType, false);

        /// <summary>
        /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a <see cref="DbType"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add<TEntity>(PropertyInfo propertyInfo,
            DbType? dbType,
            bool force)
            where TEntity : class =>
            Add(typeof(TEntity), propertyInfo, dbType, force);

        /// <summary>
        /// Property Level: Adds a mapping between a <see cref="PropertyInfo"/> object and a <see cref="DbType"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add(Type entityType,
            PropertyInfo propertyInfo,
            DbType? dbType,
            bool force)
        {
            // Validate
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");
            ObjectExtension.ThrowIfNull(dbType, "DbType");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, dbType, value);
                }
                else
                {
                    throw new MappingExistsException($"The mappings are already existing.");
                }
            }
            else
            {
                maps.TryAdd(key, dbType);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Property Level: Get the existing mapped <see cref="DbType"/> object of the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Get the existing mapped <see cref="DbType"/> object of the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName, true));

        /// <summary>
        /// Property Level: Get the existing mapped <see cref="DbType"/> object of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Field field)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name, true));

        /// <summary>
        /// Property Level: Get the existing mapped <see cref="DbType"/> object of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        internal static DbType? Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Property Level: Remove the existing mapped <see cref="DbType"/> from the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Remove(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Remove the existing mapped <see cref="DbType"/> from the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        public static void Remove<TEntity>(string propertyName)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName, true));

        /// <summary>
        /// Property Level: Remove the existing mapped <see cref="DbType"/> from the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        public static void Remove<TEntity>(Field field)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name, true));

        /// <summary>
        /// Property Level: Remove the existing mapped <see cref="DbType"/> from the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        internal static void Remove(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            maps.TryRemove(key, out var _);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clear all the existing cached mappings.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion
    }
}
