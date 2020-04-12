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
    /// A class that is used to map a property handler into a .NET CLR Type.
    /// </summary>
    public static class PropertyTypeHandlerMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<int, object> m_maps = new ConcurrentDictionary<int, object>();

        #endregion

        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add<TType, TPropertyHandler>(TPropertyHandler propertyHandler,
            bool @override = false) =>
            Add(typeof(TType), propertyHandler, @override);

        /// <summary>
        /// Adds a mapping between the .NET CLR Type and a property handler.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <param name="propertyHandler">The instance of the property handler. The type must implement the <see cref="IPropertyHandler{TInput, TResult}"/> interface.</param>
        /// <param name="override">Set to true if to override the existing mapping, otherwise an exception will be thrown if the mapping is already present.</param>
        public static void Add(Type type,
            object propertyHandler,
            bool @override = false)
        {
            // Guard the type
            GuardPresence(type);
            Guard(propertyHandler?.GetType());

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var value = (object)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out value))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, propertyHandler, value);
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
                m_maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TType">The .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>()
        {
            return Get<TPropertyHandler>(typeof(TType));
        }

        /// <summary>
        /// Gets the mapped property handler for .NET CLR Type.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>An instance of mapped property handler for .NET CLR Type.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for the cache
            var value = (object)null;

            // get the value
            m_maps.TryGetValue(type.FullName.GetHashCode(), out value);

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
        /// Removes an existing property handler mapping.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <param name="throwException">If true, it throws an exception if the mapping is not present.</param>
        /// <returns>True if the removal is successful, otherwise false.</returns>
        public static bool Remove(Type type,
            bool throwException = true)
        {
            // Check the presence
            GuardPresence(type);

            // Variables for cache
            var key = type.FullName.GetHashCode();
            var existing = (object)null;
            var result = m_maps.TryRemove(key, out existing);

            // Throws an exception if necessary
            if (result == false && throwException == true)
            {
                throw new MissingMappingException($"There is no mapping defined for '{type.FullName}'.");
            }

            // Return false
            return result;
        }

        #endregion

        #region Property Level

        /*
         * Add
         */

        /// <summary>
        /// Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TType, TPropertyHandler>(Expression<Func<TType, object>> expression,
            TPropertyHandler propertyHandler)
            where TType : class =>
            Add<TType, TPropertyHandler>(expression, propertyHandler, false);

        /// <summary>
        /// Adds a property handler mapping into a class property (via expression).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(Expression<Func<TType, object>> expression,
            TPropertyHandler propertyHandler,
            bool force)
            where TType : class =>
            Add<TPropertyHandler>(ExpressionExtension.GetProperty<TType>(expression), propertyHandler, force);

        /// <summary>
        /// Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TType, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler)
            where TType : class =>
            Add<TType, TPropertyHandler>(propertyName, propertyHandler, false);

        /// <summary>
        /// Adds a property handler mapping into a class property (via property name).
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        /// <param name="propertyHandler">The instance of property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(string propertyName,
            TPropertyHandler propertyHandler,
            bool force)
            where TType : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<TType>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(TType).FullName}'.");
            }

            // Add to the mapping
            Add<TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TType, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler)
            where TType : class =>
            Add<TType, TPropertyHandler>(field, propertyHandler, false);

        /// <summary>
        /// Adds a property handler mapping into a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TPropertyHandler>(Field field,
            TPropertyHandler propertyHandler,
            bool force)
            where TType : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<TType>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(TType).FullName}'.");
            }

            // Add to the mapping
            Add<TPropertyHandler>(property, propertyHandler, force);
        }

        /// <summary>
        /// Adds a property handler into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TPropertyHandler>(ClassProperty classProperty,
            TPropertyHandler propertyHandler) =>
            Add<TPropertyHandler>(classProperty.PropertyInfo, propertyHandler, false);

        /// <summary>
        /// Adds a property handler into a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TPropertyHandler>(ClassProperty classProperty,
            TPropertyHandler propertyHandler,
            bool force) =>
            Add<TPropertyHandler>(classProperty?.PropertyInfo, propertyHandler, force);

        /// <summary>
        /// Adds a property handler into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        public static void Add<TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler) =>
            Add<TPropertyHandler>(propertyInfo, propertyHandler, false);

        /// <summary>
        /// Adds a property handler into a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        /// <param name="propertyHandler">The instance of the property handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TPropertyHandler>(PropertyInfo propertyInfo,
            TPropertyHandler propertyHandler,
            bool force)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");
            ThrowNullReferenceException(propertyHandler, "PropertyHandler");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;

            // Try get the cache
            if (m_maps.TryGetValue(key, out value))
            {
                if (force)
                {
                    // Update the existing one
                    m_maps.TryUpdate(key, propertyHandler, value);
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
                m_maps.TryAdd(key, propertyHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped property handler of the class property (via expression).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(Expression<Func<TType, object>> expression)
            where TType : class =>
            Get<TPropertyHandler>(ExpressionExtension.GetProperty<TType>(expression));

        /// <summary>
        /// Gets the mapped property handler of the class property (via property name).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(string propertyName)
            where TType : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TType>(propertyName));

        /// <summary>
        /// Gets the mapped property handler of the class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TType">The type of the entity.</typeparam>
        /// <typeparam name="TPropertyHandler">The type of the property handler.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TType, TPropertyHandler>(Field field)
            where TType : class =>
            Get<TPropertyHandler>(TypeExtension.GetProperty<TType>(field.Name));

        /// <summary>
        /// Gets the mapped property handler on a specific <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(ClassProperty classProperty) =>
            Get<TPropertyHandler>(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the mapped property handler on a specific <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The mapped property handler object of the property.</returns>
        public static TPropertyHandler Get<TPropertyHandler>(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;
            var result = default(TPropertyHandler);

            // Try get the value
            if (m_maps.TryGetValue(key, out value) == true)
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
        /// Removes a mapped property handler from a class property (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        public static void Remove<T>(Expression<Func<T, object>> expression)
            where T : class =>
            Remove(ExpressionExtension.GetProperty<T>(expression));

        /// <summary>
        /// Removes a mapped property handler from a class property (via property name).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The instance of property handler.</param>
        public static void Remove<T>(string propertyName)
            where T : class
        {
            // Validates
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Get the property
            var property = TypeExtension.GetProperty<T>(propertyName);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{propertyName}' is not found at type '{typeof(T).FullName}'.");
            }

            // Add to the mapping
            Remove(property);
        }

        /// <summary>
        /// Removes a mapped property handler from a class property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> to be mapped.</param>
        public static void Remove<T>(Field field)
            where T : class
        {
            // Validates
            ThrowNullReferenceException(field, "Field");

            // Get the property
            var property = TypeExtension.GetProperty<T>(field.Name);
            if (property == null)
            {
                throw new PropertyNotFoundException($"Property '{field.Name}' is not found at type '{typeof(T).FullName}'.");
            }

            // Add to the mapping
            Remove(property);
        }

        /// <summary>
        /// Removes a mapped property handler from a <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/> to be mapped.</param>
        public static void Remove(ClassProperty classProperty) =>
            Remove(classProperty.PropertyInfo);

        /// <summary>
        /// Removes a mapped property handler from a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/> to be mapped.</param>
        public static void Remove(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var value = (object)null;

            // Try get the value
            m_maps.TryRemove(key, out value);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property handlers.
        /// </summary>
        public static void Flush()
        {
            m_maps.Clear();
        }

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
            var isInterfacedTo = type.IsInterfacedTo(typeof(IPropertyHandler<,>));
            if (isInterfacedTo == false)
            {
                throw new InvalidTypeException($"Type '{type.FullName}' must implement the '{typeof(IPropertyHandler<,>).FullName}' interface.");
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