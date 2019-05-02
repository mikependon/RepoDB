using RepoDb.Interfaces;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to resolve the .NET CLR Types into its equivalent database string name.
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
                case DbType.DateTime:
                    return "DATETIME";
                case DbType.DateTimeOffset:
                    return "DATETIMEOFFSET";
                case DbType.Decimal:
                    return "DECIMAL(18,2)";
                case DbType.Single:
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
                default:
                    return "NVARCHAR";
            }
        }
    }
}
