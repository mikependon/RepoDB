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
    /// A class used to resolve the <see cref="DbType"/> into its equivalent Db2 database string name.
    /// </summary>
    public class DbTypeToDb2StringNameResolver : IResolver<DbType, string>
    {
        /*
         * Taken:
         * https://www.ibm.com/docs/en/db2/11.5?topic=elements-data-types
         */

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
                DbType.Binary => "BLOB(1M)",
                DbType.Boolean => "SMALLINT",
                DbType.String => "VARCHAR(2000)",
                DbType.Date => "DATE",
                DbType.DateTime => "TIMESTAMP",
                DbType.DateTime2 => "TIMESTAMP",
                DbType.DateTimeOffset => "TIMESTAMP",
                DbType.Decimal => "DECIMAL(31,15)",
                DbType.Single => "REAL",
                DbType.Double => "DOUBLE",
                DbType.Int32 => "INTEGER",
                DbType.Int16 => "SMALLINT",
                DbType.Time => "TIME",
                DbType.Byte => "SMALLINT",
                DbType.Guid => "CHAR(16) FOR BIT DATA",
                DbType.AnsiString => "VARCHAR(2000)",
                DbType.AnsiStringFixedLength => "CHAR(254)",
                DbType.StringFixedLength => "GRAPHIC(127)",
                DbType.Object => "BLOB(1M)",
                DbType.Xml => "XML",
                _ => "VARCHAR(2000)",
            };
        }
    }
}
