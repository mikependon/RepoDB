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
    /// A class that is being used to resolve the <see cref="Field"/> name conversion for SqLite.
    /// </summary>
    public class SqLiteConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteConvertFieldResolver"/> class.
        /// </summary>
        public SqLiteConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToSqLiteStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteConvertFieldResolver"/> class.
        /// </summary>
        public SqLiteConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }
    }
}
