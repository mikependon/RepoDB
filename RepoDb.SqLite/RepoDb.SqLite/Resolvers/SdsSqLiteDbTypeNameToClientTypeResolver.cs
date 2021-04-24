using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the SqLite Database Types into its equivalent .NET CLR Types. This is only used for 'System.Data.SQLite.Core' library.
    /// </summary>
    public class SdsSqLiteDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" or "integer" => typeof(long),
                "blob" => typeof(byte[]),
                "boolean" => typeof(long),
                "char" or "string" or "text" or "varchar" => typeof(string),
                "date" or "datetime" => typeof(DateTime),
                "time" => typeof(DateTime),
                "decimal" or "numeric" => typeof(decimal),
                "double" or "real" => typeof(double),
                "int" => typeof(int),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
