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
            return dbType switch
            {
                DbType.Int64 => "BIGINT",
                DbType.Byte or DbType.Binary => "BLOB",
                DbType.Boolean => "BOOLEAN",
                DbType.String or DbType.AnsiString or DbType.AnsiStringFixedLength or DbType.StringFixedLength => "TEXT",
                DbType.Date => "DATE",
                DbType.DateTime or DbType.DateTime2 or DbType.DateTimeOffset => "DATETIME",
                DbType.Decimal => "DECIMAL",
                DbType.Single => "REAL",
                DbType.Double => "DOUBLE",
                DbType.Int32 or DbType.Int16 => "INT",
                DbType.Time => "TIME",
                _ => "TEXT",/* DbType.Guid
                     * DbType.Xml
                     * DbType.Object
                     */
            };
        }
    }
}
