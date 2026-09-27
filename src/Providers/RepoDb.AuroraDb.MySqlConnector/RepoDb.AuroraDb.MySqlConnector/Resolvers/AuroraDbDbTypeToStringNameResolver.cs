#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="AuroraDbType"/> into its equivalent database string name.
    /// </summary>
    public class AuroraDbDbTypeToStringNameResolver : IResolver<AuroraDbType, string>
    {
        /// <summary>
        /// Returns the equivalent database string name of the <see cref="AuroraDbType"/>.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(AuroraDbType dbType)
        {
            return dbType switch
            {
                AuroraDbType.TinyInt => "TINYINT",
                AuroraDbType.SmallInt => "SMALLINT",
                AuroraDbType.MediumInt => "MEDIUMINT",
                AuroraDbType.Int => "INT",
                AuroraDbType.BigInt => "BIGINT",
                AuroraDbType.Decimal => "DECIMAL",
                AuroraDbType.Float => "FLOAT",
                AuroraDbType.Double => "DOUBLE",
                AuroraDbType.Bit => "BIT",
                AuroraDbType.Char => "CHAR",
                AuroraDbType.VarChar => "VARCHAR",
                AuroraDbType.TinyText => "TINYTEXT",
                AuroraDbType.Text or AuroraDbType.Enum or AuroraDbType.Set => "TEXT",
                AuroraDbType.MediumText => "MEDIUMTEXT",
                AuroraDbType.LongText => "LONGTEXT",
                AuroraDbType.Binary => "BINARY",
                AuroraDbType.VarBinary => "VARBINARY",
                AuroraDbType.TinyBlob => "TINYBLOB",
                AuroraDbType.Blob => "BLOB",
                AuroraDbType.MediumBlob => "MEDIUMBLOB",
                AuroraDbType.LongBlob => "LONGBLOB",
                AuroraDbType.Date => "DATE",
                AuroraDbType.Time => "TIME",
                AuroraDbType.DateTime => "DATETIME",
                AuroraDbType.Timestamp => "TIMESTAMP",
                AuroraDbType.Year => "YEAR",
                AuroraDbType.Json => "JSON",
                AuroraDbType.Geometry
                    or AuroraDbType.Point
                    or AuroraDbType.LineString
                    or AuroraDbType.Polygon
                    or AuroraDbType.MultiPoint
                    or AuroraDbType.MultiLineString
                    or AuroraDbType.MultiPolygon
                    or AuroraDbType.GeometryCollection => "GEOMETRY",
                _ => "TEXT",
            };
        }
    }
}
