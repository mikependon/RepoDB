using RepoDb.Attributes;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mappings between the class property and property handler object.
    /// </summary>
    public static class PropertyHandlerCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, object> m_cache = new ConcurrentDictionary<int, object>();

        #endregion

        #region Methods

        #region Type Level

        /// <summary>
        /// Gets the cached property handler object that is being mapped into a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The mapped property handler object of the .NET CLR type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>() =>
            Get<TPropertyHandler>(typeof(TType));

        /// <summary>
        /// Gets the cached property handler object that is being mapped into a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The mapped property handler object of the .NET CLR type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = type.GetUnderlyingType().FullName.GetHashCode();
            var value = (object)null;
            var result = default(TPropertyHandler);

            // Try get the value
            if (m_cache.TryGetValue(key, out value) == false)
            {
                result = PropertyHandlerMapper.Get<TPropertyHandler>(type);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Property Level

        /// <summary>
        /// Gets the cached property handler object that is being mapped on a specific class property (via expression).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(Expression<Func<TType, object>> expression)
            where TType : class =>
            Get<TPropertyHandler>(ExpressionExtension.GetProperty<TType>(expression));

        /// <summary>
        /// Gets the cached property handler object that is being mapped on a specific class property (via property name).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(string propertyName)
            where TType : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TType>(propertyName));

        /// <summary>
        /// Gets the cached property handler object that is being mapped on a specific class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(Field field)
            where TType : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TType>(field.Name));

        /// <summary>
        /// Gets the cached property handler object that is being mapped on a specific <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(ClassProperty classProperty) =>
            Get<TPropertyHandler>(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the cached property handler object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;
            var result = default(TPropertyHandler);

            // Try get the value
            if (m_cache.TryGetValue(key, out value) == false)
            {
                // Attribute
                var attribute = propertyInfo.GetCustomAttribute<PropertyHandlerAttribute>();
                if (attribute != null)
                {
                    result = Converter.ToType<TPropertyHandler>(Activator.CreateInstance(attribute.HandlerType));
                }

                // Property Level
                if (result == null)
                {
                    result = PropertyHandlerMapper.Get<TPropertyHandler>(propertyInfo);
                }

                // Type Level
                if (result == null)
                {
                    result = PropertyHandlerMapper.Get<TPropertyHandler>(propertyInfo.PropertyType);
                }

                // Add to cache
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
