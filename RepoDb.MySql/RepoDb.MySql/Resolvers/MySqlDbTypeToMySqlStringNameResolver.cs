using MySql.Data.MySqlClient;
using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class MySqlDbTypeToMySqlStringNameResolver : IResolver<MySqlDbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(MySqlDbType dbType)
        {
            /*
            Id (System.Int64)
            ColumnVarchar (System.String)
            ColumnInt (System.Int32)
            ColumnDecimal2 (System.Decimal)
            ColumnDateTime (System.DateTime)
            ColumnBlob (System.Byte[])
            ColumnBlobAsArray (System.Byte[])
            ColumnBinary (System.Byte[])
            ColumnLongBlob (System.Byte[])
            ColumnMediumBlob (System.Byte[])
            ColumnTinyBlob (System.Byte[])
            ColumnVarBinary (System.Byte[])
            ColumnDate (System.DateTime)
            ColumnDateTime2 (System.DateTime)
            ColumnTime (System.TimeSpan)
            ColumnTimeStamp (System.DateTime)
            ColumnYear (System.Int16)
            ColumnGeometry (System.Byte[])
            ColumnLineString (System.Byte[])
            ColumnMultiLineString (System.Byte[])
            ColumnMultiPoint (System.Byte[])
            ColumnMultiPolygon (System.Byte[])
            ColumnPoint (System.Byte[])
            ColumnPolygon (System.Byte[])
            ColumnBigint (System.Int64)
            ColumnDecimal (System.Decimal)
            ColumnDouble (System.Double)
            ColumnFloat (System.Single)
            ColumnInt2 (System.Int32)
            ColumnMediumInt (System.Int32)
            ColumnReal (System.Double)
            ColumnSmallInt (System.Int16)
            ColumnTinyInt (System.SByte)
            ColumnChar (System.String)
            ColumnJson (System.String)
            ColumnNChar (System.String)
            ColumnNVarChar (System.String)
            ColumnLongText (System.String)
            ColumnMediumText (System.String)
            ColumnText (System.String)
            ColumnTinyText (System.String)
            ColumnBit (System.UInt64)
             */
            return dbType switch
            {
                MySqlDbType.Binary => "BINARY",
                MySqlDbType.Bit => "BIT",
                MySqlDbType.Blob => "BLOB",
                MySqlDbType.Byte or MySqlDbType.UByte => "TINYINT",
                MySqlDbType.Date => "DATE",
                MySqlDbType.DateTime => "DATETIME",
                MySqlDbType.Decimal => "DECIMAL",
                MySqlDbType.Double => "DOUBLE",
                MySqlDbType.Enum or MySqlDbType.Guid or MySqlDbType.Set or MySqlDbType.Text => "TEXT",
                MySqlDbType.Float => "FLOAT",
                MySqlDbType.Geometry => "GEOMETRY",
                MySqlDbType.Int16 or MySqlDbType.Int24 or MySqlDbType.UInt24 or MySqlDbType.UInt16 => "SMALLINT",
                MySqlDbType.Int32 or MySqlDbType.UInt32 => "INT",
                MySqlDbType.Int64 or MySqlDbType.UInt64 => "BIGINT",
                MySqlDbType.JSON => "JSON",
                MySqlDbType.LongBlob => "LONGBLOB",
                MySqlDbType.LongText => "LONGTEXT",
                MySqlDbType.MediumBlob => "MEDIUMBLOB",
                MySqlDbType.MediumText => "MEDIUMTEXT",
                MySqlDbType.Newdate => "DATE",
                MySqlDbType.NewDecimal => "DECIMAL",
                MySqlDbType.String => "STRING",
                MySqlDbType.Time => "TIME",
                MySqlDbType.Timestamp => "TIMESTAMP",
                MySqlDbType.TinyBlob => "TINYBLOB",
                MySqlDbType.TinyText => "TINYTEXT",
                MySqlDbType.VarBinary => "VARBINARY",
                MySqlDbType.VarChar => "VARCHAR",
                MySqlDbType.VarString => "VARCHAR",
                MySqlDbType.Year => "YEAR",
                _ => "TEXT",
            };
        }
    }
}
