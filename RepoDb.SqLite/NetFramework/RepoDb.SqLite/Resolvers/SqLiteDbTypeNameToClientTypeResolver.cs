using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the SqLite Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class SqLiteTypeNameToClientTypeResolver : IResolver<string, Type>
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
                Id : System.Int64
                ColumnBigInt : System.Int64
                ColumnBlob : System.Byte[]
                ColumnBoolean : System.Boolean
                ColumnChar : System.String
                ColumnDate : System.DateTime
                ColumnDateTime : System.DateTime
                ColumnDecimal : System.Decimal
                ColumnDouble : System.Double
                ColumnInteger : System.Int64
                ColumnInt : System.Int32
                ColumnNone : System.Double
                ColumnNumeric : System.Decimal
                ColumnReal : System.Double
                ColumnString : System.String
                ColumnText : System.String
                ColumnTime : System.DateTime
                ColumnVarChar : System.String
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
                case "time": //  return typeof(TimeSpan);
                    return typeof(DateTime);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "double":
                case "real": // return typeof(float);
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
