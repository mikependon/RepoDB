using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mapped-name of the property.
    /// </summary>
    public static class PropertyMappedNameCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, string> cache = new ConcurrentDictionary<int, string>();
        private static IResolver<PropertyInfo, Type, string> resolver = new PropertyMappedNameResolver();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the cached column name mappings of the property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        public static string Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Gets the cached column name mappings of the property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        public static string Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Gets the cached column name mappings of the property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        public static string Get<TEntity>(Field field)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(field.Name));


        /// <summary>
        /// Gets the cached column name mappings of the property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        internal static string Get<TEntity>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Get(typeof(TEntity), propertyInfo);

        /// <summary>
        /// Gets the cached column name mappings of the property.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        internal static string Get(PropertyInfo propertyInfo) =>
            Get(propertyInfo.DeclaringType, propertyInfo);

        /// <summary>
        /// Gets the cached column name mappings of the property.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached column name mappings of the property.</returns>
        internal static string Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = resolver.Resolve(propertyInfo, entityType);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

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
