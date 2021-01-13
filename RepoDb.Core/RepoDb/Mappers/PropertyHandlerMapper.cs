using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a .NET CLR type or a class property into a <see cref="IPropertyHandler{TInput, TResult}"/> object.
    /// </summary>
    public static class PropertyHandlerMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, object> maps = new ConcurrentDictionary<int, object>();

        #endregion

        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(bool force = false) =>
            Add(typeof(TType), Activator.CreateInstance<TPropertyHandler>(), force);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force = false) =>
            Add(typeof(TType), propertyHandler, force);

        /// <summary>
        /// Type Level: Adds a mapping between a .NET CLR type and a <see cref="IPropertyHandler{TInput, TResult}"/> object.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            object propertyHandler,
            bool force = false)
        {
            // Guard the type
            GuardPresence(type);
            Guard(propertyHandler?.GetType());

            // Variables for cache
            var key = GenerateHashCode(type);

            // Try get the mappings
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Override the existing one
                    maps.TryUpdate(key, propertyHandler, value);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The property handler mapping for '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Type Level: Gets the mapped property handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped property handler for .NET CLR type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>() =>
            Get<TPropertyHandler>(typeof(TType));

        /// <summary>
        /// Type Level: Gets the mapped property handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>An instance of mapped property handler for .NET CLR type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // Get the value
            maps.TryGetValue(GenerateHashCode(type), out var value);

            // Check the result
            if (value == null || value is TPropertyHandler)
            {
                return (TPropertyHandler)value;
            }

            // Throw an exception
            throw new InvalidTypeException($"The cache item is not convertible to '{typeof(TPropertyHandler).FullName}' type.");
        }

        /*
         * Remove
         */

        /// <summary>
        /// Type Level: Removes the existing mapped property handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        public static void Remove<T>() =>
            Remove(typeof(T));

        /// <summary>
        /// Type Level: Removes the existing mapped property handler of the .NET CLR type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        public static void Remove(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for cache
            var key = GenerateHashCode(type);
            var existing = (object)null;

            // Try get the value
            maps.TryRemove(key, out existing);
        }

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via expression). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(expression, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(expression, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via expression). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression,
            bool force)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(expression, Activator.CreateInstance<TPropertyHandler>(), force);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via expression).
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
            Add<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression), propertyHandler, force);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via property name). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyName, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyName, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via property name). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            bool force)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyName, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler,
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
            Add<TEntity, TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via <see cref="Field"/> object). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(field, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(field, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via <see cref="Field"/> object). It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            bool force)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(field, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TEntity, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler,
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
            Add<TEntity, TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Property Level: Adds a property handler mapping into a <see cref="PropertyInfo"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target property handler.
        /// Make sure a default constructor is available for the type of property handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        internal static void Add<TEntity, TPropertyHandler>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyInfo, Activator.CreateInstance<TPropertyHandler>(), false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        internal static void Add<TEntity, TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler)
            where TEntity : class =>
            Add<TEntity, TPropertyHandler>(propertyInfo, propertyHandler, false);

        /// <summary>
        /// Property Level: Adds a property handler mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add<TEntity, TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler,
            bool force)
            where TEntity : class =>
            Add<TPropertyHandler>(typeof(TEntity), propertyInfo, propertyHandler, force);


        /// <summary>
        /// Property Level: Adds a property handler mapping into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="entityType">The target .NET CLR type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void Add<TPropertyHandler>(Type entityType,
            PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");
            ThrowNullReferenceException(propertyHandler, "PropertyHandler");
            Guard(propertyHandler?.GetType() ?? typeof(TPropertyHandler));

            /*
             * Note: The reflected type of the property info if explored via expression is different, therefore, we
             * need to manually extract it.
             */

            // Extract
            if (propertyInfo != null)
            {
                propertyInfo = PropertyCache.Get(entityType, propertyInfo.AsField()).PropertyInfo ?? propertyInfo;
            }

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);

            // Try get the cache
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Update the existing one
                    maps.TryUpdate(key, propertyHandler, value);
                }
                else
                {
                    // Throws an exception
                    throw new MappingExistsException($"A property handler mapping to '{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}' already exists.");
                }
            }
            else
            {
                // Add the mapping
                maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Property Level: Gets the mapped property handler object of the data entity type property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Gets the mapped property handler object of the data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(string propertyName)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(propertyName));

        /// <summary>
        /// Property Level: Gets the mapped property handler object of the data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TEntity, TPropertyHandler>(Field field)
            where TEntity : class =>
            Get<TEntity, TPropertyHandler>(TypeExtension.GetProperty<TEntity>(field.Name));

        /// <summary>
        /// Property Level: Gets the mapped property handler on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        internal static TPropertyHandler Get<TEntity, TPropertyHandler>(PropertyInfo propertyInfo)
            where TEntity : class =>
            Get<TPropertyHandler>(typeof(TEntity), propertyInfo);

        /// <summary>
        /// Property Level: Gets the mapped property handler on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        internal static TPropertyHandler Get<TPropertyHandler>(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);
            var result = default(TPropertyHandler);

            // Try get the value
            if (maps.TryGetValue(key, out var value) == true)
            {
                result = Converter.ToType<TPropertyHandler>(value);
            }

            // Return the value
            return result;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Property Level: Removes the existing mapped property handler from a data entity type property (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Remove<TEntity>(ExpressionExtension.GetProperty<TEntity>(expression));

        /// <summary>
        /// Property Level: Removes the existing mapped property handler from a data entity type property (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        public static void Remove<TEntity>(string propertyName)
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
            Remove<TEntity>(property);
        }

        /// <summary>
        /// Property Level: Removes the existing mapped property handler from a data entity type property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object to be mapped.</param>
        public static void Remove<TEntity>(Field field)
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
            Remove<TEntity>(property);
        }

        /// <summary>
        /// Property Level: Removes a mapped property handler from a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target .NET CLR type.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        internal static void Remove<TEntity>(PropertyInfo propertyInfo) =>
            Remove(typeof(TEntity), propertyInfo);

        /// <summary>
        /// Property Level: Removes a mapped property handler from a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="entityType">The target .NET CLR type.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        internal static void Remove(Type entityType,
            PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = GenerateHashCode(entityType, propertyInfo);
            var value = (object)null;

            // Try to remove the value
            maps.TryRemove(key, out value);
        }

        #endregion

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IPropertyHandler{TInput, TResult}"/> objects.
        /// </summary>
        public static void Clear() =>
            maps.Clear();

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo) =>
            TypeExtension.GenerateHashCode(entityType, propertyInfo);

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Property handler type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type does not implemented the <see cref="IPropertyHandler{TInput, TResult}"/> interface.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            if (type.IsInterfacedTo(StaticType.IPropertyHandler) == false)
            {
                throw new InvalidTypeException($"Type '{type.FullName}' must implement the '{StaticType.IPropertyHandler.FullName}' interface.");
            }
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="TType">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<TType>(TType obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion

        #endregion
    }
}
