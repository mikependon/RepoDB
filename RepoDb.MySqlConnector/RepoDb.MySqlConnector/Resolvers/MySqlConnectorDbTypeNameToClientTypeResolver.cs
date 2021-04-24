using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the MySql Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class MySqlConnectorDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }
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
            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" or "integer" => typeof(long),
                "blob" or "blobasarray" or "binary" or "longblob" or "mediumblob" or "tinyblob" or "varbinary" or "geometry" or "linestring" or "multilinestring" or "multipoint" or "multipolygon" or "point" or "polygon" => typeof(byte[]),
                "boolean" => typeof(bool),
                "char" or "json" or "longtext" or "mediumtext" or "nchar" or "nvarchar" or "string" or "text" or "tinytext" or "varchar" => typeof(string),
                "date" or "datetime" or "datetime2" or "timestamp" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "decimal" or "decimal2" or "numeric" => typeof(decimal),
                "double" or "real" => typeof(double),
                "float" => typeof(float),
                "int" or "int2" or "mediumint" or "year" => typeof(int),
                "smallint" => typeof(short),
                "tinyint" => typeof(sbyte),
                "bit" => typeof(ulong),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
