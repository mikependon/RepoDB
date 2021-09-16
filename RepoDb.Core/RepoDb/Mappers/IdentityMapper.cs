using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to set a class property to be an identity property. This is an alternative class to <see cref="IdentityAttribute"/> object.
    /// </summary>
    public static class IdentityMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, ClassProperty> maps = new();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds an identity property mapping into a target class (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Add<TEntity>(expression, false);

        /// <summary>
        /// Adds an identity property mapping into a target class (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Expression<Func<TEntity, object>> expression,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(expression, "Expression");

            // Get the property
            var property = ExpressionExtension.GetProperty<TEntity>(expression);

            // Add to the mapping
            Add<TEntity>(DataEntityExtension.GetClassPropertyOrThrow<TEntity>(property?.Name), force);
        }

        /// <summary>
        /// Adds an identity property mapping into a target class (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        public static void Add<TEntity>(string propertyName)
            where TEntity : class =>
            Add<TEntity>(propertyName, false);

        /// <summary>
        /// Adds an identity property mapping into a target class (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(string propertyName,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(propertyName, "PropertyName");

            // Add to the mapping
            Add<TEntity>(DataEntityExtension.GetClassPropertyOrThrow<TEntity>(propertyName), force);
        }

        /// <summary>
        /// Adds an identity property mapping into a target class (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        public static void Add<TEntity>(Field field)
            where TEntity : class =>
            Add<TEntity>(field, false);

        /// <summary>
        /// Adds an identity property mapping into a target class (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity>(Field field,
            bool force)
            where TEntity : class
        {
            // Validates
            ObjectExtension.ThrowIfNull(field, "Field");

            // Add to the mapping
            Add<TEntity>(DataEntityExtension.GetClassPropertyOrThrow<TEntity>(field.Name), force);
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
        /// <param name="type">The type of the data entity.</param>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add(Type type,
            ClassProperty classProperty,
            bool force)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");
            ObjectExtension.ThrowIfNull(classProperty, "ClassProperty");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, classProperty, value);
                }
                else
                {
                    throw new MappingExistsException("The mapping is already existing.");
                }
            }
            else
            {
                maps.TryAdd(key, classProperty);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the exising mapped identity property of the target class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Get the exising mapped identity property of the target class.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>An instance of the mapped <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type type)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the value
            maps.TryGetValue(key, out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the existing mapped identity property of the class.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        public static void Remove<TEntity>()
            where TEntity : class =>
            Remove(typeof(TEntity));

        /// <summary>
        /// Removes the existing mapped identity property of the class.
        /// </summary>
        /// <param name="type">The target type.</param>
        public static void Remove(Type type)
        {
            // Validate
            ObjectExtension.ThrowIfNull(type, "Type");

            // Variables
            var key = TypeExtension.GenerateHashCode(type);

            // Try get the value
            maps.TryRemove(key, out var _);
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
    }
}
