using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the <see cref="PropertyValueAttribute"/> of the class property.
    /// </summary>
    public static class PropertyValueAttributeCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, IEnumerable<PropertyValueAttribute>> cache = new();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <param name="includeMappings">True to evaluate the existing mappings.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Expression<Func<TEntity, object>> expression,
            bool includeMappings = false)
            where TEntity : class =>
            Get(ExpressionExtension.GetProperty<TEntity>(expression), includeMappings);

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="includeMappings">True to evaluate the existing mappings.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(string propertyName,
            bool includeMappings = false)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(propertyName, includeMappings), includeMappings);

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <param name="includeMappings">True to evaluate the existing mappings.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Field field,
            bool includeMappings = false)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(field.Name, includeMappings), includeMappings);

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> object.</param>
        /// <param name="includeMappings">True to evaluate the existing mappings.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get(PropertyInfo propertyInfo,
            bool includeMappings = false) =>
            Get(propertyInfo?.DeclaringType, propertyInfo, includeMappings);

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> object.</param>
        /// <param name="includeMappings">True to evaluate the existing mappings.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get(Type entityType,
            PropertyInfo propertyInfo,
            bool includeMappings = false)
        {
            // Validate
            ObjectExtension.ThrowIfNull(entityType, "EntityType");
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo) +
                includeMappings.GetHashCode();

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = new PropertyValueAttributeResolver().Resolve(propertyInfo, includeMappings);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached objects.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        #endregion
    }
}
