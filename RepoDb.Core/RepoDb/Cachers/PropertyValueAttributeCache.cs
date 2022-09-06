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

        #region Property Level

        /// <summary>
        /// Property Level: Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="expression">The property expression.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TEntity>(Field field)
            where TEntity : class =>
            Get(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> object.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get(PropertyInfo propertyInfo) =>
            Get(propertyInfo?.DeclaringType, propertyInfo);

        /// <summary>
        /// Property Level: Gets the list of cached <see cref="PropertyValueAttribute"/> objects that is currently mapped to the <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> object.</param>
        /// <returns>The list of <see cref="PropertyValueAttribute"/> object.</returns>
        internal static IEnumerable<PropertyValueAttribute> Get(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ObjectExtension.ThrowIfNull(entityType, "EntityType");
            ObjectExtension.ThrowIfNull(propertyInfo, "PropertyInfo");

            // Variables
            var key = TypeExtension.GenerateHashCode(entityType, propertyInfo);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = new PropertyValueAttributePropertyLevelResolver().Resolve(propertyInfo);
                cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Type Level

        /// <summary>
        /// Type Level: Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The target type.</typeparam>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get<TType>() =>
            Get(typeof(TType));

        /// <summary>
        /// Get the list of mapped <see cref="PropertyValueAttribute"/> objects of the .NET CLR type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The list of mapped <see cref="PropertyValueAttribute"/> objects.</returns>
        public static IEnumerable<PropertyValueAttribute> Get(Type type)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the value
            if (cache.TryGetValue(key, out var result) == false)
            {
                result = new PropertyValueAttributeTypeLevelResolver().Resolve(type);
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
