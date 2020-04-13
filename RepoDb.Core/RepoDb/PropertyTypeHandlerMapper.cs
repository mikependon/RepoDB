using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map a .NET CLR type or a class property into a property handler object.
    /// </summary>
    [Obsolete("Please use the 'PropertyHandlerMapper' class instead.")]
    public static class PropertyTypeHandlerMapper
    {
        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Type Level: Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add<TType, TPropertyHandler>(TPropertyHandler propertyHandler,
            bool @override = false) =>
            PropertyHandlerMapper.Add(typeof(TType), propertyHandler, @override);

        /// <summary>
        /// Type Level: Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            object propertyHandler,
            bool @override = false) =>
            PropertyHandlerMapper.Add(type, propertyHandler, @override);

        /*
         * Get
         */

        /// <summary>
        /// Type Level: Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>() =>
            PropertyHandlerMapper.Get<TType, TPropertyHandler>();

        /// <summary>
        /// Type Level: Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type) =>
            PropertyHandlerMapper.Get<TPropertyHandler>(type);

        /*
         * Remove
         */

        /// <summary>
        /// Type Level: Removes an existing property handler mapping.
        /// </summary>
        /// <typeparam name="T">The .NET CLR type.</typeparam>
        public static void Remove<T>() =>
            PropertyHandlerMapper.Remove(typeof(T));

        /// <summary>
        /// Type Level: Removes an existing property handler mapping.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        public static void Remove(Type type) =>
            PropertyHandlerMapper.Remove(type);

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(expression, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression), propertyHandler, force);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(propertyName, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(propertyName, propertyHandler, force);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(field, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class =>
            PropertyHandlerMapper.Add<TEntity, TPropertyHandler>(field, propertyHandler, force);

        /*
         * Get
         */

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            PropertyHandlerMapper.Get<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(string propertyName)
            where TEntity : class =>
            PropertyHandlerMapper.Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the mapped property handler of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Field field)
            where TEntity : class =>
            PropertyHandlerMapper.Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(field.Name));

        /*
         * Remove
         */

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            PropertyHandlerMapper.Remove<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        public static void Remove<TEntity>(string propertyName)
            where TEntity : class =>
            PropertyHandlerMapper.Remove<TEntity>(propertyName);

        /// <summary>
        /// Property Level: Removes a mapped property handler from a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        public static void Remove<TEntity>(Field field)
            where TEntity : class =>
            PropertyHandlerMapper.Remove<TEntity>(field);

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached property handlers.
        /// </summary>
        public static void Clear() =>
            PropertyHandlerMapper.Clear();

        #endregion

        #endregion
    }
}