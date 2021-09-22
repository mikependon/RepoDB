using RepoDb.Attributes.Parameter;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a list of <see cref="PropertyValueAttribute"/> object into a class property.
    /// </summary>
    public static class PropertyValueAttributeMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, IEnumerable<PropertyValueAttribute>> maps = new();

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            IEnumerable<PropertyValueAttribute> attributes)
            where TEntity : class =>
            Add<TEntity>(expression, attributes, false);

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
            where TEntity : class =>
            Add(ExpressionExtension.GetProperty<TEntity>(expression), attributes, force);

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the target class property.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        public static void Add<TEntity>(string propertyName,
            IEnumerable<PropertyValueAttribute> attributes)
            where TEntity : class =>
            Add<TEntity>(propertyName, attributes, false);

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the target class property.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string propertyName,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(propertyName, "PropertyName");

            // Add to the mapping
            Add(DataEntityExtension.GetPropertyOrThrow<TEntity>(propertyName), attributes, force);
        }

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        public static void Add<TEntity>(Field field,
            IEnumerable<PropertyValueAttribute> attributes)
            where TEntity : class =>
            Add<TEntity>(field, attributes, false);

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(field, "Field");

            // Add to the mapping
            Add(DataEntityExtension.GetPropertyOrThrow<TEntity>(field.Name), attributes, force);
        }

        /// <summary>
        /// Adds a mapping between a class property and a list of <see cref="PropertyValueAttribute"/> object (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        public static void Add(PropertyInfo propertyInfo,
            IEnumerable<PropertyValueAttribute> attributes) =>
            Add(propertyInfo, attributes, false);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(PropertyInfo propertyInfo,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force) =>
            Add(propertyInfo.DeclaringType, propertyInfo, attributes, force);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        public static void Add(Type entityType,
            PropertyInfo propertyInfo,
            IEnumerable<PropertyValueAttribute> attributes) =>
            Add(entityType, propertyInfo, attributes, false);

        /// <summary>
        /// Adds a mapping between a <see cref="PropertyInfo"/> object and a list of <see cref="PropertyValueAttribute"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of the target <see cref="PropertyInfo"/> object.</param>
        /// <param name="attributes">The list of <see cref="PropertyValueAttribute"/> object.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type entityType,
            PropertyInfo propertyInfo,
            IEnumerable<PropertyValueAttribute> attributes,
            bool force)
        {
            // Validate
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");
            ObjectExtension.ThrowIfNull(attributes, "Attributes");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Add to the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, attributes, value);
                }
                else
                {
                    throw new MappingExistsException($"The mappings are already existing.");
                }
            }
            else
            {
                maps.TryAdd(key, attributes);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the target class property.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName, true));

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Field field)
            where TEntity : class =>
            Get(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name, true));

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get(PropertyInfo propertyInfo) =>
            Get(propertyInfo.DeclaringType, propertyInfo);

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get(Type entityType,
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
        /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Remove(typeof(TEntity), ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the target class property.</param>
        public static void Remove<TEntity>(string propertyName)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(propertyName, true));

        /// <summary>
        /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        public static void Remove<TEntity>(Field field)
            where TEntity : class =>
            Remove(typeof(TEntity), TypeExtension.GetProperty<TEntity>(field.Name, true));

        /// <summary>
        /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        public static void Remove(PropertyInfo propertyInfo) =>
            Remove(propertyInfo.DeclaringType, propertyInfo);

        /// <summary>
        /// Removes the existing mapped <see cref="PropertyValueAttribute"/> objects of the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        public static void Remove(Type entityType,
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
