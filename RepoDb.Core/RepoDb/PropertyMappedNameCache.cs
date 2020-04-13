using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name of the property.
    /// </summary>
    public static class PropertyMappedNameCache
    {
        private static readonly ConcurrentDictionary<int, string> m_cache = new ConcurrentDictionary<int, string>();

        #region Methods

        /// <summary>
        /// Gets the cached mapped-name of the property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Gets the cached mapped-name of the property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Gets the cached mapped-name of the property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<TEntity>(Field field)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Gets the cached mapped-name of the <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(ClassProperty classProperty) =>
            Get(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the cached mapped-name of the property.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var result = (string)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = PropertyInfoExtension.GetMappedName(propertyInfo);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

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
