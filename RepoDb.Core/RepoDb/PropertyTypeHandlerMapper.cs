using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to map a property handler into a .NET CLR Type.
    /// </summary>
    public static class PropertyTypeHandlerMapper
    {
        private static readonly ConcurrentDictionary<int, object> m_maps = new ConcurrentDictionary<int, object>();

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
            var @interface = type.GetInterfaces()
                .Where(e => e.FullName.StartsWith("RepoDb.Interfaces.IPropertyHandler"))
                .FirstOrDefault();
            if (@interface == null)
            {
                throw new InvalidTypeException($"Type '{type.FullName}' must implement the '{typeof(IPropertyHandler<,>).FullName}' interface.");
            }
        }

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

        /// <summary>
        /// Adds a mapping between the .NET CLR Type and a property handler..
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
            var existing = (object)null;

            // Try get the mappings
            if (m_maps.TryGetValue(key, out existing))
            {
                if (@override)
                {
                    // Override the existing one
                    m_maps.TryUpdate(key, propertyHandler, existing);
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
    }
}
