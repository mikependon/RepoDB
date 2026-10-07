#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
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

        private static readonly ConcurrentDictionary<int, object> maps = new();

        #endregion

        #region Methods

        #region Type Level

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between a class and a <see cref="IClassHandler{TEntity}"/> object. It uses the <see cref="Activator.CreateInstance(Type)"/> method to create the instance of target <see cref="IClassHandler{TEntity}"/>.
        /// Make sure a default constructor is available for the type of <see cref="IClassHandler{TEntity}"/>, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, [DynamicallyAccessedMembers(Trimming.Handler)] TClassHandler>(bool force = false)
            where TClassHandler : new()
        {
            AddInternal(typeof(TType), new TClassHandler(), typeof(TClassHandler), force);
        }

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TType, [DynamicallyAccessedMembers(Trimming.Handler)] TClassHandler>(TClassHandler classHandler,
            bool force = false)
        {
            AddInternal(typeof(TType), classHandler, TypeExtension.GetHandlerType<TClassHandler>(classHandler), force);
        }

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="classHandler">The instance of the class handler. The type must implement the <see cref="IClassHandler{TEntity}"/> interface.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        [RequiresUnreferencedCode("The implemented interfaces of the runtime type of the class handler might be trimmed. Use the generic 'Add' methods instead.")]
        public static void Add(Type type,
            object classHandler,
            bool force = false)
        {
            AddInternal(type, classHandler, classHandler?.GetType(), force);
        }

        /// <summary>
        /// Adds a mapping between a .NET CLR type and a <see cref="IClassHandler{TEntity}"/> object.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="classHandler">The instance of the class handler.</param>
        /// <param name="classHandlerType">The type of the class handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        internal static void AddInternal(Type type,
            object classHandler,
            [DynamicallyAccessedMembers(Trimming.Handler)] Type classHandlerType,
            bool force)
        {
            // Guard
            Guard(classHandlerType);

            // Variables for cache
            var key = GenerateHashCode(type);

            // Try get the mappings
            if (maps.TryGetValue(key, out var value))
            {
                if (force)
                {
                    maps.TryUpdate(key, classHandler, value);
                }
                else
                {
                    throw new MappingExistsException($"The class handler mapping for '{type.FullName}' already exists.");
                }
            }
            else
            {
                maps.TryAdd(key, classHandler);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the existing mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TType">The target .NET CLR type.</typeparam>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
        public static TClassHandler Get<TType, [DynamicallyAccessedMembers(Trimming.Handler)] TClassHandler>()
        {
            return Get<TClassHandler>(typeof(TType));
        }

        /// <summary>
        /// Get the existing mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="TClassHandler">The type of the handler.</typeparam>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>An instance of mapped class handler for .NET CLR type.</returns>
        public static TClassHandler Get<[DynamicallyAccessedMembers(Trimming.Handler)] TClassHandler>(Type type)
        {
            // Check the presence
            ObjectExtension.ThrowIfNull(type, "type");

            // get the value
            maps.TryGetValue(GenerateHashCode(type), out var value);

            // Check the result
            return value switch
            {
                null => default,
                TClassHandler classHandler => classHandler,
                
                // Throw an exception
                _ => throw new InvalidTypeException($"The cache item is not convertible to '{typeof(TClassHandler).FullName}' type.")
            };
        }

        /*
         * Remove
         */

        /// <summary>
        /// Removes the existing mapped class handler of the .NET CLR type.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        public static void Remove<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes the existing mapped class handler of the .NET CLR type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        public static void Remove(Type type)
        {
            // Check the presence
            ObjectExtension.ThrowIfNull(type, "type");

            // Variables for cache
            var key = GenerateHashCode(type);

            // Try get the value
            maps.TryRemove(key, out var _);
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
        private static int GenerateHashCode(Type type)
        {
            return TypeExtension.GenerateHashCode(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private static void Guard([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type)
        {
            ObjectExtension.ThrowIfNull(type, "type");
            if (!type.IsInterfacedTo(StaticType.IClassHandler))
            {
                throw new InvalidTypeException($"The type '{type.FullName}' must implement the '{StaticType.IClassHandler.FullName}' interface.");
            }
        }

        #endregion

        #endregion
    }
}
