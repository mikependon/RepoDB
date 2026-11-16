#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.Exceptions;

namespace RepoDb.Schema
{
    /// <summary>
    /// Registers dialects by connection type or provider name for live and saved schemas.
    /// </summary>
    public static class SchemaDialectMapper
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<string, ISchemaDialect> Dialects = new ConcurrentDictionary<string, ISchemaDialect>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the dialect using the connection type's provider setting.
        /// </summary>
        /// <typeparam name="TDbConnection">The connection type.</typeparam>
        /// <param name="dialect">The schema dialect.</param>
        /// <exception cref="ArgumentNullException">The dialect is null.</exception>
        /// <exception cref="MissingMappingException">The connection has no valid provider setting.</exception>
        public static void Add<TDbConnection>(ISchemaDialect dialect)
            where TDbConnection : IDbConnection
        {
            if (dialect == null)
                throw new ArgumentNullException(nameof(dialect));
            var provider = DbSettingMapper.Get<TDbConnection>()?.Provider;
            if (string.IsNullOrWhiteSpace(provider))
                throw new MissingMappingException($"No provider setting is registered for '{typeof(TDbConnection).FullName}'. Configure the database provider before registering its schema dialect.");
            Add(provider,
                dialect);
        }

        /// <summary>
        /// Maps the provider to its dialect, replacing the one that it had.
        /// </summary>
        /// <param name="provider">The provider of the database.</param>
        /// <param name="dialect">The dialect of the database.</param>
        /// <exception cref="ArgumentNullException">The provider is <c>null</c> (or white space), or the dialect is <c>null</c>.</exception>
        public static void Add(string provider,
            ISchemaDialect dialect)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Dialects[provider] = dialect ?? throw new ArgumentNullException(nameof(dialect));
        }

        /// <summary>
        /// Removes all the mappings.
        /// </summary>
        public static void Clear() =>
            Dialects.Clear();

        /// <summary>
        /// Gets the dialect of the provider.
        /// </summary>
        /// <param name="provider">The provider of the database.</param>
        /// <returns>The dialect, or <c>null</c> if the provider is not known.</returns>
        public static ISchemaDialect Get(string provider) =>
            provider != null && Dialects.TryGetValue(provider,
                out var dialect) ? dialect : null;

        /// <summary>
        /// Gets the dialects that are mapped, without repeating the one that is mapped to many providers.
        /// </summary>
        /// <returns>The dialects.</returns>
        public static IReadOnlyList<ISchemaDialect> GetAll() =>
            Dialects.Values.Distinct().ToList();

        /// <summary>
        /// Gets the dialects that are mapped, and the dialect that is given (so it is always one of them, even if its extension did not register it).
        /// </summary>
        /// <param name="include">The dialect that is always included.</param>
        /// <returns>The dialects, without repeating any.</returns>
        public static IReadOnlyList<ISchemaDialect> GetAll(ISchemaDialect include) =>
            Dialects.Values.Concat(new[] { include }).Distinct().ToList();

        /// <summary>
        /// Removes the mapping of the provider.
        /// </summary>
        /// <param name="provider">The provider of the database.</param>
        public static void Remove(string provider)
        {
            if (provider != null)
            {
                Dialects.TryRemove(provider,
                    out _);
            }
        }

        #endregion
    }
}
