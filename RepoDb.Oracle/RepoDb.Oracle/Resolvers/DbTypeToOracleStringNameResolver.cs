using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent Oracle database string name.
    /// </summary>
    public class DbTypeToOracleStringNameResolver : IResolver<DbType, string>
    {
        /*
         * Taken:
         * https://docs.oracle.com/en/database/oracle/oracle-database/23/sqlrf/Data-Types.html
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(DbType dbType)
        {
            return dbType switch
            {
                DbType.Int64 => "NUMBER(19)",
                DbType.Binary => "BLOB",
                DbType.Boolean => "NUMBER(1)",
                DbType.String => "NVARCHAR2(2000)",
                DbType.Date => "DATE",
                DbType.DateTime => "DATE",
                DbType.DateTime2 => "TIMESTAMP",
                DbType.DateTimeOffset => "TIMESTAMP WITH TIME ZONE",
                DbType.Decimal => "NUMBER(18,2)",
                DbType.Single => "BINARY_FLOAT",
                DbType.Double => "BINARY_DOUBLE",
                DbType.Int32 => "NUMBER(10)",
                DbType.Int16 => "NUMBER(5)",
                DbType.Time => "INTERVAL DAY(0) TO SECOND(7)",
                DbType.Byte => "NUMBER(3)",
                DbType.Guid => "RAW(16)",
                DbType.AnsiString => "VARCHAR2(2000)",
                DbType.AnsiStringFixedLength => "CHAR(2000)",
                DbType.StringFixedLength => "NCHAR(2000)",
                DbType.Object => "BLOB",
                DbType.Xml => "XMLTYPE",
                _ => "NVARCHAR2(2000)",
            };
        }
    }
}
