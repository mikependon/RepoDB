#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the equivalent <see cref="DbType"/> object of the .NET CLR type.
    /// </summary>
    public class TypeMapTypeLevelResolver : IResolver<Type, DbType?>
    {
        /// <summary>
        /// Resolves the equivalent <see cref="DbType"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The equivalent <see cref="DbType"/> object of the .NET CLR type.</returns>
        public DbType? Resolve(Type type) =>
            TypeMapper.Get(type);
    }
}
