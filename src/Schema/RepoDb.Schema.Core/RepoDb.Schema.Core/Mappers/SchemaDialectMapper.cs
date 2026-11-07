#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that maps the provider of a database (see <see cref="RepoDb.Interfaces.IDbSetting.Provider"/>) to its dialect (see <see cref="ISchemaDialect"/>).
    /// The extension of each database registers its dialect, and a provider without a registered dialect is not converted.
    /// </summary>
    public static class SchemaDialectMapper
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<string, ISchemaDialect> Dialects = new ConcurrentDictionary<string, ISchemaDialect>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Methods

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
