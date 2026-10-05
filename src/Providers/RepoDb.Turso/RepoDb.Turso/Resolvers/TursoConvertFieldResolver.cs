#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the <see cref="Field"/> name conversion for Turso.
    /// </summary>
    public class TursoConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="TursoConvertFieldResolver"/> class.
        /// </summary>
        public TursoConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToTursoStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="TursoConvertFieldResolver"/> class.
        /// </summary>
        public TursoConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }
    }
}
