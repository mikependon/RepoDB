#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Data;
using RepoDb.Exceptions;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that is being used to map an instance of <see cref="ISchemaReader"/> of into the type of <see cref="IDbConnection"/> object.
    /// </summary>
    public static class SchemaReaderMapper
    {
        #region Privates

        private static readonly ConcurrentDictionary<Type, ISchemaReader> maps = new();

        #endregion

        #region Methods

        /*
         * Add
         */

        /// <summary>
        /// Adds a mapping between the type of <typeparamref name="TDbConnection"/> and an <see cref="ISchemaReader"/> object.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="schemaReader">The instance of <see cref="ISchemaReader"/> object to mapped to.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void Add<TDbConnection>(ISchemaReader schemaReader,
            bool force)
            where TDbConnection : IDbConnection
        {
            var type = typeof(TDbConnection);

            // Try get the mappings
            if (maps.TryGetValue(type, out var existing))
            {
                if (force)
                {
                    maps.TryUpdate(type, schemaReader, existing);
                }
                else
                {
                    throw new MappingExistsException(type.FullName);
                }
            }
            else
            {
                maps.TryAdd(type, schemaReader);
            }
        }

        /*
         * Get
         */

        /// <summary>
        /// Get the existing mapped <see cref="ISchemaReader"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <returns>The instance of existing mapped <see cref="ISchemaReader"/> object.</returns>
        public static ISchemaReader Get<TDbConnection>()
            where TDbConnection : IDbConnection
        {
            // get the value
            maps.TryGetValue(typeof(TDbConnection), out var value);

            // Return the value
            return value;
        }

        /// <summary>
        /// Get the existing mapped <see cref="ISchemaReader"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        /// <param name="connection">The instance of <see cref="IDbConnection"/>.</param>
        /// <returns>The instance of existing mapped <see cref="ISchemaReader"/> object.</returns>
        public static ISchemaReader Get<TDbConnection>(TDbConnection connection)
            where TDbConnection : IDbConnection
        {
            // get the value
            maps.TryGetValue(connection.GetType(), out var value);

            // Return the value
            return value;
        }

        /*
         * Remove
         */

        /// <summary>
        /// Remove the existing mapped <see cref="ISchemaReader"/> object from the type of <typeparamref name="TDbConnection"/>.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/>.</typeparam>
        public static void Remove<TDbConnection>()
            where TDbConnection : IDbConnection
        {
            // Variables for cache
            var key = typeof(TDbConnection);

            // Try get the the value
            maps.TryRemove(key, out _);
        }

        /*
         * Clear
         */

        /// <summary>
        /// Clears all the existing cached <see cref="ISchemaReader"/> objects.
        /// </summary>
        public static void Clear()
        {
            maps.Clear();
        }

        #endregion
    }
}
