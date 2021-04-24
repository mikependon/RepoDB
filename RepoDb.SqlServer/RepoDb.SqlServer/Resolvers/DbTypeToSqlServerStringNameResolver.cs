using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent SQL Server database string name.
    /// </summary>
    public class DbTypeToSqlServerStringNameResolver : IResolver<DbType, string>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
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
                DbType.Int64 => "BIGINT",
                DbType.Binary => "BINARY",
                DbType.Boolean => "BIT",
                DbType.String => "NVARCHAR",
                DbType.Date => "DATE",
                DbType.DateTime => "DATETIME",
                DbType.DateTime2 => "DATETIME2",
                DbType.DateTimeOffset => "DATETIMEOFFSET",
                DbType.Decimal => "DECIMAL(18,2)",
                DbType.Single => "REAL",
                DbType.Double => "FLOAT",
                DbType.Int32 => "INT",
                DbType.Int16 => "SMALLINT",
                DbType.Time => "TIME",
                DbType.Byte => "TINYINT",
                DbType.Guid => "UNIQUEIDENTIFIER",
                DbType.AnsiString => "VARCHAR",
                DbType.AnsiStringFixedLength => "CHAR",
                DbType.StringFixedLength => "NCHAR",
                DbType.Object => "OBJECT",
                DbType.Xml => "XML",
                _ => "NVARCHAR",
            };
        }
    }
}
