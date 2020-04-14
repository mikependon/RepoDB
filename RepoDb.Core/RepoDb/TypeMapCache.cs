using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mappings between the <see cref="DbType"/> object and .NET CLR type or class property.
    /// </summary>
    public static class TypeMapCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, DbType?> m_cache = new ConcurrentDictionary<int, DbType?>();

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
            var result = (DbType?)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = TypeMapper.Get(type);
                m_cache.TryAdd(key, result);
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
            where TEntity : class
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(typeof(TEntity), propertyInfo);
            var result = (DbType?)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                var classProperty = PropertyCache.Get<TEntity>()
                    .FirstOrDefault(p => string.Equals(p.PropertyInfo.Name, propertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                result = classProperty?.GetDbType();
                m_cache.TryAdd(key, result);
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
        public static void Flush()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type)
        {
            return TypeExtension.GenerateHashCode(type);
        }

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo)
        {
            return TypeExtension.GenerateHashCode(entityType, propertyInfo);
        }

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
