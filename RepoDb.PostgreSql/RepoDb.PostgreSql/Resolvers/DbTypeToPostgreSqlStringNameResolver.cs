using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class DbTypeToPostgreSqlStringNameResolver : IResolver<DbType, string>
    {
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
                DbType.Binary or DbType.Byte => "BYTEA",
                DbType.Boolean => "BOOLEAN",
                DbType.AnsiString or DbType.AnsiStringFixedLength or DbType.String or DbType.StringFixedLength => "TEXT",
                DbType.Date => "DATE",
                DbType.DateTime => "TIMESTAMP",
                DbType.DateTime2 => "TIMESTAMP",
                DbType.DateTimeOffset => "TIMESTAMPTZ", 
                DbType.Decimal => "NUMERIC",
                DbType.Single => "REAL",
                DbType.Double => "DOUBLE PRECISION",
                DbType.Int32 => "INTEGER",
                DbType.Int16 => "SMALLINT",
                DbType.Time => "INTERVAL",
                /* 
                DbType.Guid
                DbType.Xml
                DbType.Object
                */
                _ => "TEXT"
            };
        }
    }
}
