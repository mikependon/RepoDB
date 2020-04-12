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
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <returns>The mapped <see cref="DbType"/> object of the .NET CLR type.</returns>
        public static DbType? Get<T>() =>
            Get(typeof(T));

        /// <summary>
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific .NET CLR type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the .NET CLR type.</returns>
        public static DbType? Get(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = type.GetUnderlyingType().FullName.GetHashCode();
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
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<T>(Expression<Func<T, object>> expression)
            where T : class =>
            Get(ExpressionExtension.GetProperty<T>(expression));

        /// <summary>
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via property name).
        /// </summary>
        /// <typeparam name="T">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<T>(string propertyName)
            where T : class =>
            Get(TypeExtension.GetProperty<T>(propertyName));

        /// <summary>
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get<T>(Field field)
            where T : class =>
            Get(TypeExtension.GetProperty<T>(field.Name));

        /// <summary>
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var classProperty = PropertyCache.Get(propertyInfo.DeclaringType)
                .FirstOrDefault(p => p.PropertyInfo == propertyInfo);

            // Reuse
            return Get(classProperty);
        }

        /// <summary>
        /// Gets the cached <see cref="DbType"/> object that is being mapped on a specific <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped <see cref="DbType"/> object of the property.</returns>
        public static DbType? Get(ClassProperty classProperty)
        {
            // Validate
            ThrowNullReferenceException(classProperty, "ClassProperty");

            // Variables
            var key = classProperty.GetHashCode();
            var result = (DbType?)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = classProperty.GetDbType();
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
