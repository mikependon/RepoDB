using RepoDb.Connector.MariaDb;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="MariaDbType"/> into its equivalent database string name.
    /// </summary>
    public class MariaDbDbTypeToStringNameResolver : IResolver<MariaDbType, string>
    {
        /// <summary>
        /// Returns the equivalent database string name of the <see cref="MariaDbType"/>.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(MariaDbType dbType)
        {
            return dbType switch
            {
                MariaDbType.TinyInt => "TINYINT",
                MariaDbType.SmallInt => "SMALLINT",
                MariaDbType.MediumInt => "MEDIUMINT",
                MariaDbType.Int => "INT",
                MariaDbType.BigInt => "BIGINT",
                MariaDbType.Decimal => "DECIMAL",
                MariaDbType.Float => "FLOAT",
                MariaDbType.Double => "DOUBLE",
                MariaDbType.Bit => "BIT",
                MariaDbType.Char => "CHAR",
                MariaDbType.VarChar => "VARCHAR",
                MariaDbType.TinyText => "TINYTEXT",
                MariaDbType.Text or MariaDbType.Enum or MariaDbType.Set => "TEXT",
                MariaDbType.MediumText => "MEDIUMTEXT",
                MariaDbType.LongText => "LONGTEXT",
                MariaDbType.Binary => "BINARY",
                MariaDbType.VarBinary => "VARBINARY",
                MariaDbType.TinyBlob => "TINYBLOB",
                MariaDbType.Blob => "BLOB",
                MariaDbType.MediumBlob => "MEDIUMBLOB",
                MariaDbType.LongBlob => "LONGBLOB",
                MariaDbType.Date => "DATE",
                MariaDbType.Time => "TIME",
                MariaDbType.DateTime => "DATETIME",
                MariaDbType.Timestamp => "TIMESTAMP",
                MariaDbType.Year => "YEAR",
                MariaDbType.Json => "JSON",
                MariaDbType.Geometry
                    or MariaDbType.Point
                    or MariaDbType.LineString
                    or MariaDbType.Polygon
                    or MariaDbType.MultiPoint
                    or MariaDbType.MultiLineString
                    or MariaDbType.MultiPolygon
                    or MariaDbType.GeometryCollection => "GEOMETRY",
                _ => "TEXT",
            };
        }
    }
}
