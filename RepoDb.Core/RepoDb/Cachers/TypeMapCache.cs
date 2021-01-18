using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mappings between the <see cref="DbType"/> objects and .NET CLR type or class properties.
    /// </summary>
    public static class TypeMapCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> cache = new ConcurrentDictionary<int, DbType?>();
        private static readonly IResolver<PropertyInfo, DbType?> propertyLevelResolver = new TypeMapPropertyLevelResolver();
        private static readonly IResolver<Type, DbType?> typeLevelResolver = new TypeMapTypeLevelResolver();

        #endregion

        #region Methods

        #region TypeLevel

        /// <summary>
        /// Type Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <returns>The mapped <see cref="DbType"/> object of the .NET CLR type.</returns>
        public static DbType? Get<TType>() =>
            Get(typeof(TType));

        /// <summary>
        /// Type Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific .NET CLR type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the .NET CLR type.</returns>
        public static DbType? Get(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = GenerateHashCode(type);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = typeLevelResolver.Resolve(type);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Property Level

        /// <summary>
        /// Property Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<TEntity>(Field field)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        internal static DbType? Get<TEntity>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Get(typeof(TEntity), propertyInfo) ?? Get(propertyInfo.PropertyType);

        /// <summary>
        /// Property Level: Gets the cached <see cref="DbType"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        internal static DbType? Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = propertyLevelResolver.Resolve(propertyInfo);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property mapped names.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo) =>
            TypeExtension.GenerateHashCode(entityType, propertyInfo);

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion
    }
}
