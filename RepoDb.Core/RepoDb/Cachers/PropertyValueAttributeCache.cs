using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
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
        private static readonly IResolver<PropertyInfo, IEnumerable<PropertyValueAttribute>> resolver = new PropertyValueAttributeResolver();

        #endregion

        #region Methods

        #region Property Level

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> object that is currently mapped to the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> object that is currently mapped to the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> object that is currently mapped to the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Field field)
            where TEntity : class =>
            Get<TEntity>(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> object that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get<TEntity>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Get(typeof(TEntity), propertyInfo);

        /// <summary>
        /// Gets the list of cached <see cref="PropertyValueAttribute"/> object that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = new PropertyValueAttributeResolver().Resolve(propertyInfo);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

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
