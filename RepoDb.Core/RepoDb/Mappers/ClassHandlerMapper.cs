using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to map a .NET CLR type into a <see cref="IClassHandler{TEntity}"/> object.
    /// </summary>
    public static class ClassHandlerMapper
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
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target class handler.
        /// Make sure a default constructor is available for the type of class handler, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TClassHandler>(bool force = false) =>
            Add(typeof(TType), Activator.CreateInstance<TClassHandler>(), force);

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, TClassHandler>(TClassHandler classHandler,
            bool force = false) =>
            Add(typeof(TType), classHandler, force);

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add(Type type,
            object classHandler,
            bool force = false)
        {
            // Guard the type
            GuardPresence(type);
            Guard(classHandler?.GetType());

            // Variables for cache
            var key = GenerateHashCode(type);

            // Try get the mappings
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    // Override the existing one
                    maps.TryUpdate(key, classHandler, value);
                }
                else
                {
                    // Throw an exception
                    throw new MappingExistsException($"The class handler mapping for '{type.FullName}' already exists.");
                }
            }
            else
            {
                // Add to mapping
                maps.TryAdd(key, classHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Gets the mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
        public static TClassHandler Get<TType, TClassHandler>() =>
            Get<TClassHandler>(typeof(TType));

        /// <summary>
        /// Gets the mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
        public static TClassHandler Get<TClassHandler>(Type type)
        {
            // Check the presence
            GuardPresence(type);

            // get the value
            maps.TryGetValue(GenerateHashCode(type), out var value);

            // Check the result
            if (value == null || value is TClassHandler)
            {
                return (TClassHandler)value;
            }

            // Throw an exception
            throw new InvalidTypeException($"The cache item is not convertible to '{typeof(TClassHandler).FullName}' type.");
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the existing mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        public static void Remove<T>() =>
            Remove(typeof(T));

        /// <summary>
        /// Removes the existing mapped class handler of the .NET CLR type.
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

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="IClassHandler{TEntity}"/> objects.
        /// </summary>
        public static void Clear()
        {
            maps.Clear();
        }

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Throws an exception if null.
        /// </summary>
        private static void GuardPresence(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("Class handler type.");
            }
        }

        /// <summary>
        /// Throws an exception if the type does not implemented the <see cref="IClassHandler{TEntity}"/> interface.
        /// </summary>
        private static void Guard(Type type)
        {
            GuardPresence(type);
            if (type.IsInterfacedTo(StaticType.IClassHandler) == false)
            {
                throw new InvalidTypeException($"The type '{type.FullName}' must implement the '{StaticType.IClassHandler.FullName}' interface.");
            }
        }

        #endregion

        #endregion
    }
}
