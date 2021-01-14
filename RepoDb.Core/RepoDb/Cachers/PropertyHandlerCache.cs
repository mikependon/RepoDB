using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mappings between a class property and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
    /// </summary>
    public static class PropertyHandlerCache
    {
        #region Privates

        private static readonly ConcurrentDictionary<MemberInfo, object> cache = new ConcurrentDictionary<MemberInfo, object>();
        private static readonly IResolver<Type, PropertyInfo, object> propertyLevelResolver = new PropertyHandlerPropertyLevelResolver();
        private static readonly IResolver<Type, object> typeLevelResolver = new PropertyHandlerTypeLevelResolver();

        #endregion

        #region Methods

        #region Type Level

        /// <summary>
        /// Type Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped to a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>() =>
            Get<TPropertyHandler>(typeof(TType));

        /// <summary>
        /// Type Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped to a specific .NET CLR type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type)
        {
            // Validate
            ThrowNullReferenceException(type, "Type");

            return (TPropertyHandler)cache.GetOrAdd(type.GetUnderlyingType(), _ => typeLevelResolver.Resolve(_));
        }

        #endregion

        #region Property Level

        /// <summary>
        /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(string propertyName)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Field field)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
        internal static TPropertyHandler Get<TEntity, TPropertyHandler>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Get<TPropertyHandler>(typeof(TEntity), propertyInfo);

        /// <summary>
        /// Property Level: Gets the cached <see cref="IPropertyHandler{TInput, TResult}"/> object that is being mapped on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped <see cref="IPropertyHandler{TInput, TResult}"/> object of the property.</returns>
        internal static TPropertyHandler Get<TPropertyHandler>(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            return (TPropertyHandler)cache.GetOrAdd(propertyInfo, _ => propertyLevelResolver.Resolve(entityType, propertyInfo));
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached <see cref="IPropertyHandler{TInput, TResult}"/> objects.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

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
