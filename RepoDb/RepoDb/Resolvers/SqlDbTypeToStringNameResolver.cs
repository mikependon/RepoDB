using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class SqlDbTypeToStringNameResolver : IResolver<DbType, string>
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
        public string Resolve(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Int64:
                    return "BIGINT";
                case DbType.Binary:
                    return "BINARY";
                case DbType.Boolean:
                    return "BIT";
                case DbType.String:
                    return "NVARCHAR";
                case DbType.Date:
                    return "DATE";
                case DbType.DateTime:
                    return "DATETIME";
                case DbType.DateTime2:
                    return "DATETIME2";
                case DbType.DateTimeOffset:
                    return "DATETIMEOFFSET";
                case DbType.Decimal:
                    return "DECIMAL(18,2)";
                case DbType.Single:
                    return "REAL";
                case DbType.Double:
                    return "FLOAT";
                case DbType.Int32:
                    return "INT";
                case DbType.Int16:
                    return "SMALLINT";
                case DbType.Time:
                    return "TIME";
                case DbType.Byte:
                    return "TINYINT";
                case DbType.Guid:
                    return "UNIQUEIDENTIFIER";
                case DbType.AnsiString:
                    return "VARCHAR";
                case DbType.AnsiStringFixedLength:
                    return "CHAR";
                case DbType.StringFixedLength:
                    return "NCHAR";
                case DbType.Object:
                    return "OBJECT";
                case DbType.Xml:
                    return "XML";
                default:
                    return "NVARCHAR";
            }
        }
    }
}
