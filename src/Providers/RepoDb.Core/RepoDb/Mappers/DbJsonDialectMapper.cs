#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Dialects;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Collections.Concurrent;

namespace RepoDb
{
    /// <summary>
    /// A class that maps the type of a <see cref="IDbSetting"/> to its <see cref="IDbJsonDialect"/>.
    /// A setting that derives from a mapped setting uses the dialect of the mapped one.
    /// </summary>
    public static class DbJsonDialectMapper
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<Type, IDbJsonDialect> maps = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// Maps the setting type to a dialect.
        /// </summary>
        /// <typeparam name="TDbSetting">The type of the setting.</typeparam>
        /// <param name="dialect">The dialect to map.</param>
        /// <param name="force">Replaces the existing mapping when <c>true</c>, otherwise a <see cref="MappingExistsException"/> is thrown.</param>
        public static void Add<TDbSetting>(IDbJsonDialect dialect,
            bool force)
            where TDbSetting : IDbSetting
        {
            var type = typeof(TDbSetting);
            if (maps.TryGetValue(type, out var existing) && !force)
            {
                throw new MappingExistsException(type.FullName);
            }
            maps[type] = dialect;
        }

        /// <summary>
        /// Removes all the mappings.
        /// </summary>
        public static void Clear()
        {
            maps.Clear();
        }

        /// <summary>
        /// Gets the dialect of the setting.
        /// </summary>
        /// <param name="dbSetting">The setting currently in used, or <c>null</c>.</param>
        /// <returns>The dialect mapped to the setting type (or its closest base type), otherwise the <see cref="DefaultDbJsonDialect"/>.</returns>
        public static IDbJsonDialect Get(IDbSetting dbSetting)
        {
            for (var type = dbSetting?.GetType(); type != null; type = type.BaseType)
            {
                if (maps.TryGetValue(type, out var dialect))
                {
                    return dialect;
                }
            }
            return DefaultDbJsonDialect.Instance;
        }

        /// <summary>
        /// Removes the mapping of the setting type.
        /// </summary>
        /// <typeparam name="TDbSetting">The type of the setting.</typeparam>
        public static void Remove<TDbSetting>()
            where TDbSetting : IDbSetting
        {
            maps.TryRemove(typeof(TDbSetting), out _);
        }

        #endregion
    }
}
