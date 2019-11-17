using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class DbTypeToMySqlStringNameResolver : IResolver<DbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public string Resolve(DbType dbType)
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
            ColumText (System.String)
            ColumnTinyText (System.String)
            ColumnBit (System.UInt64)
             */
            switch (dbType)
            {
                case DbType.Int64:
                    return "BIGINT";
                case DbType.Byte:
                case DbType.Binary:
                    return "BLOB";
                case DbType.Boolean:
                    return "BOOLEAN";
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return "TEXT";
                case DbType.Date:
                    return "DATE";
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return "DATETIME";
                case DbType.Decimal:
                    return "DECIMAL";
                case DbType.Single:
                    return "REAL";
                case DbType.Double:
                    return "DOUBLE";
                case DbType.Int32:
                case DbType.Int16:
                    return "INT";
                case DbType.Time:
                    return "TIME";
                default:
                    /* DbType.Guid
                     * DbType.Xml
                     * DbType.Object
                     */
                    return "TEXT";
            }
        }
    }
}
