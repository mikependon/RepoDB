#region Copyright Attributions

// Copyright (c) 2026 mamoreau-devolutions and Michael Camara Pendon.
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
    /// A class that is being used to resolve the <see cref="Field"/> name conversion for SqLite.
    /// </summary>
    public class AhtolaConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="AhtolaConvertFieldResolver"/> class.
        /// </summary>
        public AhtolaConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToAhtolaStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="AhtolaConvertFieldResolver"/> class.
        /// </summary>
        public AhtolaConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }
    }
}
