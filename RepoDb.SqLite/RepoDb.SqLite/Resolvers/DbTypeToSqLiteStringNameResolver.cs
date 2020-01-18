using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class DbTypeToSqLiteStringNameResolver : IResolver<DbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(DbType dbType)
        {
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
