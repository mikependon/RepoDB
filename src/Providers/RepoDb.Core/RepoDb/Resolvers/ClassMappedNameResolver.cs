#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the database object name mappings of the data entity type.
    /// </summary>
    public class ClassMappedNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves the mapped database object name mappings of the data entity type.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The mapped database object name.</returns>
        public string Resolve(Type entityType) =>
            DataEntityExtension.GetMappedName(entityType);
    }
}
