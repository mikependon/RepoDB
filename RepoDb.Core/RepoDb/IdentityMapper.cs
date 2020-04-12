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
    /// A class that is used to set a class property to be an identity property. This is an alternative class to <see cref="IdentityAttribute"/> object.
    /// </summary>
    public static class IdentityMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, ClassProperty> m_maps = new ConcurrentDictionary<int, ClassProperty>();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds an identity property mapping into an entity type (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Add(expression, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            bool force)
            where TEntity : class =>
            Add(ExpressionExtension.GetProperty<TEntity>(expression), force);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        public static void Add<TEntity>(string propertyName)
            where TEntity : class =>
            Add<TEntity>(propertyName, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string propertyName,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add(property, force);
        }

        /// <summary>
        /// Adds an identity property mapping into an entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        public static void Add<TEntity>(Field field)
            where TEntity : class =>
            Add<TEntity>(field, false);

        /// <summary>
        /// Adds an identity property mapping into an entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<TEntity>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add(property, force);
        }

        /// <summary>
        /// Adds an identity property mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        public static void Add(PropertyInfo propertyInfo) =>
            Add(propertyInfo, false);

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
        public static void Add(ClassProperty classProperty) =>
            Add(classProperty, false);

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
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the instance of <see cref="ClassProperty"/> that is mapped as identity key.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type entityType)
        {
            // Validate
            ThrowNullReferenceException(entityType, "Type");

            // Variables
            var key = entityType.FullName.GetHashCode();
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
        /// Removes the exising mapped identity property of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        public static void Remove<TEntity>()
            where TEntity : class =>
            Remove(typeof(TEntity));

        /// <summary>
        /// Removes the exising mapped identity property of the data entity.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        public static void Remove(Type entityType)
        {
            // Validate
            ThrowNullReferenceException(entityType, "Type");

            // Variables
            var key = entityType.FullName.GetHashCode();
            var value = (ClassProperty)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached identity properties.
        /// </summary>
        public static void Clear()
        {
            m_maps.Clear();
        }

        #endregion

        #region Helpers

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
