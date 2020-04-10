using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A static class that is used to set a class property to be a identity property. This is an alternative class to <see cref="IdentityAttribute"/> object.
    /// </summary>
    public static class IdentityMapper
    {
        private static readonly ConcurrentDictionary<int, ClassProperty> m_maps = new ConcurrentDictionary<int, ClassProperty>();

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds an identity property mapping into an entity type (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Add<T>(Expression<Func<T, object>> expression)
            where T : class => Add(expression, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Expression<Func<T, object>> expression,
            bool force)
            where T : class => Add(ExpressionExtension.GetProperty<T>(expression), force);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        public static void Add<T>(string propertyName)
            where T : class => Add<T>(propertyName, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(string propertyName,
            bool force)
            where T : class
        {
            Add(TypeExtension.GetProperty<T>(propertyName), force);
        }

        /// <summary>
        /// Adds an identity property mapping into an entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        public static void Add<T>(Field field)
            where T : class => Add<T>(field, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<T>(Field field,
            bool force)
            where T : class
        {
            Add(TypeExtension.GetProperty<T>(field.Name), force);
        }

        /// <summary>
        /// Adds an identity property mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        public static void Add(PropertyInfo propertyInfo) => Add(propertyInfo, false);

        /// <summary>
        /// Adds an identity property mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(PropertyInfo propertyInfo,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var properties = PropertyCache.Get(propertyInfo.DeclaringType);

            // Get the class property
            var classProperty = properties.FirstOrDefault(p => p.PropertyInfo == propertyInfo);
            if (classProperty == null)
            {
                throw new PropertyNotFoundException($"The class property '{propertyInfo.Name}' is not found at type '{propertyInfo.DeclaringType.FullName}'.");
            }

            // Adds a mapping
            Add(classProperty, force);
        }

        /// <summary>
        /// Adds an identity property mapping into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        public static void Add(ClassProperty classProperty) => Add(classProperty.PropertyInfo, false);

        /// <summary>
        /// Adds an identity property mapping into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(ClassProperty classProperty,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(classProperty, "ClassProperty");

            // Variables
            var key = classProperty.PropertyInfo.DeclaringType.FullName.GetHashCode();
            var value = (ClassProperty)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, classProperty, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"The identity property mapping to type '{classProperty.PropertyInfo.DeclaringType.FullName}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                m_maps.TryAdd(key, classProperty);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the instance of <see cref="ClassProperty"/> that is mapped as identity key.
        /// </summary>
        /// <typeparam name="T">The type of the data entity object.</typeparam>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<T>()
            where T : class => Get(typeof(T));

        /// <summary>
        /// Gets the instance of <see cref="ClassProperty"/> that is mapped as identity key.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = type.FullName.GetHashCode();
            var value = (ClassProperty)null;

            // Try get the value
            m_maps.TryGetValue(key, out value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the exising mapped identity property of the entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        public static void Remove<T>()
            where T : class => Remove(typeof(T));

        /// <summary>
        /// Removes the exising mapped identity property of the entity type.
        /// </summary>
        /// <param name="type">The target type.</param>
        public static void Remove(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            // Variables
            var key = type.FullName.GetHashCode();
            var value = (ClassProperty)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached identity properties.
        /// </summary>
        public static void Flush()
        {
            m_maps.Clear();
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
