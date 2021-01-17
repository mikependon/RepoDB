using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to set a class property to be an identity property. This is an alternative class to <see cref="IdentityAttribute"/> object.
    /// </summary>
    public static class IdentityMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, ClassProperty> maps = new ConcurrentDictionary<int, ClassProperty>();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Add<TEntity>(expression, false);

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(expression, "Expression");

            // Get the property
            var property = ExpressionExtension.GetProperty<TEntity>(expression);

            // Get the class property
            var classProperty = GetClassProperty<TEntity>(property?.Name);
            if (classProperty == null)
            {
                throw new PropertyNotFoundException($"Property '{property.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TEntity>(classProperty, force);
        }

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        public static void Add<TEntity>(string propertyName)
            where TEntity : class =>
            Add<TEntity>(propertyName, false);

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via property name).
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

            // Get the class property
            var classProperty = GetClassProperty<TEntity>(propertyName);
            if (classProperty == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TEntity>(classProperty, force);
        }

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        public static void Add<TEntity>(Field field)
            where TEntity : class =>
            Add<TEntity>(field, false);

        /// <summary>
        /// Adds an identity property mapping into a target data entity type (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            bool force)
            where TEntity : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the class property
            var classProperty = GetClassProperty<TEntity>(field.Name);
            if (classProperty == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TEntity).FullName}'.");
            }

            // Add to the mapping
            Add<TEntity>(classProperty, force);
        }

        /// <summary>
        /// Adds an identity property mapping into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add<TEntity>(ClassProperty classProperty,
            bool force)
            where TEntity : class =>
            Add(typeof(TEntity), classProperty, force);

        /// <summary>
        /// Adds an identity property mapping into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add(Type entityType,
            ClassProperty classProperty,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(entityType, "EntityType");
            ThrowNullReferenceException(classProperty, "ClassProperty");

            // Variables
            var key = GenerateHashCode(entityType);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Update the existing one
                    maps.TryUpdate(key, classProperty, value);
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
                maps.TryAdd(key, classProperty);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped identity property on the target data entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the mapped identity property on the target data entity type.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type entityType)
        {
            // Validate
            ThrowNullReferenceException(entityType, "Type");

            // Variables
            var key = GenerateHashCode(entityType);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the existing mapped identity property of the data entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        public static void Remove<TEntity>()
            where TEntity : class =>
            Remove(typeof(TEntity));

        /// <summary>
        /// Removes the existing mapped identity property of the data entity type.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        public static void Remove(Type entityType)
        {
            // Validate
            ThrowNullReferenceException(entityType, "Type");

            // Variables
            var key = GenerateHashCode(entityType);
            var value = (ClassProperty)null;

            // Try get the value
            maps.TryRemove(key, out value);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached identity properties.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #endregion

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Gets the instance of <see cref="ClassProperty"/> object from of the data entity based on name.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>An instance of <see cref="ClassProperty"/> object.</returns>
        private static ClassProperty GetClassProperty<TEntity>(string propertyName)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>();
            return properties.FirstOrDefault(
                p => string.Equals(p.PropertyInfo.Name, propertyName, StringComparison.OrdinalIgnoreCase));
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
