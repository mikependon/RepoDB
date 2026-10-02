#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Data;

namespace RepoDb.Data
{
    /// <summary>
    /// A class that is being used to manage the database connections of the library.
    /// </summary>
    public static class ConnectionManager
    {
        private static readonly ConcurrentDictionary<string, IDbConnection> _connections =
            new ConcurrentDictionary<string, IDbConnection>(StringComparer.Ordinal);

        #region Methods

        /// <summary>
        /// Registers a connection with the specified name. The same <see cref="IDbConnection"/> instance
        /// is returned by every subsequent call to <see cref="Get(string)"/> or <see cref="Get{T}(string)"/>.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="connection">The connection object to be registered.</param>
        /// <param name="force">
        /// Defines whether an existing registration with the same name is replaced. If <c>false</c> and the
        /// name is already registered, an <see cref="InvalidOperationException"/> is thrown.
        /// </param>
        public static void Register(string name,
            IDbConnection connection,
            bool force = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (force)
            {
                _connections[name] = connection;
            }
            else if (_connections.TryAdd(name, connection) == false)
            {
                throw new InvalidOperationException($"A connection with the name '{name}' is already registered. Use the 'force' argument to replace it.");
            }
        }

        /// <summary>
        /// Gets the connection that is registered with the specified name.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <returns>The registered <see cref="IDbConnection"/> object.</returns>
        public static IDbConnection Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (_connections.TryGetValue(name, out var connection) == false)
            {
                throw new InvalidOperationException($"No connection is registered with the name '{name}'.");
            }
            return connection;
        }

        /// <summary>
        /// Gets the connection that is registered with the specified name as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the connection object.</typeparam>
        /// <param name="name">The name of the connection.</param>
        /// <returns>The registered connection object, as <typeparamref name="T"/>.</returns>
        public static T Get<T>(string name)
            where T : IDbConnection
        {
            var connection = Get(name);
            if (connection is T typed)
            {
                return typed;
            }
            throw new InvalidCastException($"The connection '{name}' is of type '{connection.GetType().FullName}' and cannot be converted to '{typeof(T).FullName}'.");
        }

        #endregion
    }
}
