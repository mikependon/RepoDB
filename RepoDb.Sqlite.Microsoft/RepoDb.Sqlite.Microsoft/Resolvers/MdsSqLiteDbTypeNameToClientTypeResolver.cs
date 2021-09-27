using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the SqLite Database Types into its equivalent .NET CLR Types. This is only used for 'Microsoft.Data.Sqlite' library.
    /// </summary>
    public class MdsSqLiteDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
            Id : System.Int64
            ColumnBigInt : System.Int64
            ColumnBlob : System.Byte[]
            ColumnBoolean : System.String
            ColumnChar : System.String
            ColumnDate : System.String
            ColumnDateTime : System.String
            ColumnDecimal : System.String / System.Int64 (if has value)
            ColumnDouble : System.Double
            ColumnInteger : System.Int64
            ColumnInt : System.Int64
            ColumnNone : System.String
            ColumnNumeric : System.String / System.Int64 (if has value)
            ColumnReal : System.Double
            ColumnString : System.String
            ColumnText : System.String
            ColumnTime : System.String
            ColumnVarChar : System.String
             */
            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" or "decimal" or "int" or "integer" or "numeric" => typeof(long),
                "blob" => typeof(byte[]),
                "double" or "real" => typeof(double),
                "boolean" or "char" or "date" or "datetime" or "none" or "string" or "text" or "time" or "varchar" => typeof(string),
                _ => typeof(object),
            };
        }
    }
}
