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
            switch (dbType)
            {
                case MySqlDbType.Binary:
                    return "BINARY";
                case MySqlDbType.Bit:
                    return "BIT";
                case MySqlDbType.Blob:
                    return "BLOB";
                case MySqlDbType.Byte:
                case MySqlDbType.UByte:
                    return "TINYINT";
                case MySqlDbType.Date:
                    return "DATE";
                case MySqlDbType.DateTime:
                    return "DATETIME";
                case MySqlDbType.Decimal:
                    return "DECIMAL";
                case MySqlDbType.Double:
                    return "DOUBLE";
                case MySqlDbType.Enum:
                case MySqlDbType.Guid:
                case MySqlDbType.Set:
                case MySqlDbType.Text:
                    return "TEXT";
                case MySqlDbType.Float:
                    return "FLOAT";
                case MySqlDbType.Geometry:
                    return "GEOMETRY";
                case MySqlDbType.Int16:
                case MySqlDbType.Int24:
                case MySqlDbType.UInt24:
                case MySqlDbType.UInt16:
                    return "SMALLINT";
                case MySqlDbType.Int32:
                case MySqlDbType.UInt32:
                    return "INT";
                case MySqlDbType.Int64:
                case MySqlDbType.UInt64:
                    return "BIGINT";
                case MySqlDbType.JSON:
                    return "JSON";
                case MySqlDbType.LongBlob:
                    return "LONGBLOB";
                case MySqlDbType.LongText:
                    return "LONGTEXT";
                case MySqlDbType.MediumBlob:
                    return "MEDIUMBLOB";
                case MySqlDbType.MediumText:
                    return "MEDIUMTEXT";
                case MySqlDbType.Newdate:
                    return "DATE";
                case MySqlDbType.NewDecimal:
                    return "DECIMAL";
                case MySqlDbType.String:
                    return "STRING";
                case MySqlDbType.Time:
                    return "TIME";
                case MySqlDbType.Timestamp:
                    return "TIMESTAMP";
                case MySqlDbType.TinyBlob:
                    return "TINYBLOB";
                case MySqlDbType.TinyText:
                    return "TINYTEXT";
                case MySqlDbType.VarBinary:
                    return "VARBINARY";
                case MySqlDbType.VarChar:
                    return "VARCHAR";
                case MySqlDbType.VarString:
                    return "VARCHAR";
                case MySqlDbType.Year:
                    return "YEAR";
                default:
                    return "TEXT";
            }
        }
    }
}
