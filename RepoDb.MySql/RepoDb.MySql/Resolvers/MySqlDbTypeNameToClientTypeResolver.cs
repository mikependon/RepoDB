using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the MySql Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class MySqlDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public Type Resolve(string dbTypeName)
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
            ColumText (System.String)
            ColumnTinyText (System.String)
            ColumnBit (System.UInt64)
             */
            switch (dbTypeName.ToLower())
            {
                case "bigint":
                case "integer":
                    return typeof(long);
                case "blob":
                    return typeof(byte[]);
                case "boolean":
                    return typeof(bool);
                case "char":
                case "string":
                case "text":
                case "varchar":
                    return typeof(string);
                case "date":
                case "datetime":
                    return typeof(DateTime);
                case "time":
                    return typeof(TimeSpan);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "double":
                case "real":
                    return typeof(double);
                case "int":
                    return typeof(int);
                case "none":
                    return typeof(object);
                default:
                    return typeof(object);
            }
        }
    }
}
