#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent Vertica database string name.
    /// </summary>
    public class DbTypeToVerticaStringNameResolver : IResolver<DbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(DbType dbType)
        {
            return dbType switch
            {
                DbType.Int64 => "BIGINT",
                DbType.Binary => "BLOB SUB_TYPE 0",
                DbType.Boolean => "BOOLEAN",
                DbType.String => "VARCHAR(8191)",
                DbType.Date => "DATE",
                DbType.DateTime => "TIMESTAMP",
                DbType.DateTime2 => "TIMESTAMP",
                DbType.DateTimeOffset => "TIMESTAMP WITH TIME ZONE",
                DbType.Decimal => "DECIMAL(18,2)",
                DbType.Single => "FLOAT",
                DbType.Double => "DOUBLE PRECISION",
                DbType.Int32 => "INTEGER",
                DbType.Int16 => "SMALLINT",
                DbType.Time => "TIME",
                DbType.Byte => "SMALLINT",
                DbType.Guid => "CHAR(16) CHARACTER SET OCTETS",
                DbType.AnsiString => "VARCHAR(8191)",
                DbType.AnsiStringFixedLength => "CHAR(8191)",
                DbType.StringFixedLength => "NCHAR(8191)",
                DbType.Object => "BLOB SUB_TYPE 0",
                DbType.Xml => "BLOB SUB_TYPE TEXT",
                _ => "VARCHAR(8191)",
            };
        }
    }
}
