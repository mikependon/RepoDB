#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the <see cref="HanaDbType"/> into its equivalent database string name.
    /// </summary>
    public class SapHanaDbTypeToStringNameResolver : IResolver<HanaDbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(HanaDbType dbType)
        {
            return dbType switch
            {
                HanaDbType.TinyInt => "TINYINT",
                HanaDbType.SmallInt => "SMALLINT",
                HanaDbType.Integer => "INTEGER",
                HanaDbType.BigInt => "BIGINT",
                HanaDbType.Decimal => "DECIMAL",
                HanaDbType.SmallDecimal => "SMALLDECIMAL",
                HanaDbType.Real => "REAL",
                HanaDbType.Double => "DOUBLE",
                HanaDbType.Boolean => "BOOLEAN",
                HanaDbType.VarChar => "VARCHAR",
                HanaDbType.NVarChar => "NVARCHAR",
                HanaDbType.Text => "TEXT",
                HanaDbType.Clob => "CLOB",
                HanaDbType.NClob => "NCLOB",
                HanaDbType.Date => "DATE",
                HanaDbType.Time => "TIME",
                HanaDbType.TimeStamp => "TIMESTAMP",
                HanaDbType.SecondDate => "SECONDDATE",
                HanaDbType.Blob => "BLOB",
                HanaDbType.VarBinary => "VARBINARY",
                _ => "TEXT",
            };
        }
    }
}
