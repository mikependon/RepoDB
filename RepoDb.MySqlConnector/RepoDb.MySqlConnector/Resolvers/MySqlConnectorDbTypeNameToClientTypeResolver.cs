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
            switch (dbTypeName.ToLowerInvariant())
            {
                case "bigint":
                case "integer":
                    return typeof(long);
                case "blob":
                case "blobasarray":
                case "binary":
                case "longblob":
                case "mediumblob":
                case "tinyblob":
                case "varbinary":
                case "geometry":
                case "linestring":
                case "multilinestring":
                case "multipoint":
                case "multipolygon":
                case "point":
                case "polygon":
                    return typeof(byte[]);
                case "boolean":
                    return typeof(bool);
                case "char":
                case "json":
                case "longtext":
                case "mediumtext":
                case "nchar":
                case "nvarchar":
                case "string":
                case "text":
                case "tinytext":
                case "varchar":
                    return typeof(string);
                case "date":
                case "datetime":
                case "datetime2":
                case "timestamp":
                    return typeof(DateTime);
                case "time":
                    return typeof(TimeSpan);
                case "decimal":
                case "decimal2":
                case "numeric":
                    return typeof(decimal);
                case "double":
                case "real":
                    return typeof(double);
                case "float":
                    return typeof(float);
                case "int":
                case "int2":
                case "mediumint":
                case "year":
                    return typeof(int);
                case "smallint":
                    return typeof(short);
                case "tinyint":
                    return typeof(sbyte);
                case "bit":
                    return typeof(ulong);
                case "none":
                    return typeof(object);
                default:
                    return typeof(object);
            }
        }
    }
}
