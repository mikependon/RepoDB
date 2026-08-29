#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for Db2.
    /// </summary>
    public class Db2ConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2ConvertFieldResolver"/> class.
        /// </summary>
        public Db2ConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToDb2StringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Db2ConvertFieldResolver"/> class.
        /// </summary>
        public Db2ConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object for Db2.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted.</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object for Db2.</returns>
        public override string Resolve(Field field,
            IDbSetting dbSetting)
        {
            if (field?.Type != null)
            {
                var dbType = DbTypeResolver.Resolve(field.Type);
                if (dbType != null)
                {
                    var dbTypeName = StringNameResolver.Resolve(dbType.Value).ToUpperInvariant();
                    return string.Concat("CAST(", field.Name.AsField(dbSetting), " AS ", dbTypeName, ")");
                }
            }
            return field?.Name?.AsField(dbSetting);
        }

        #endregion
    }
}
