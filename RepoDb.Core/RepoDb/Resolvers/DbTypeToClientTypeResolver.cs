using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the <see cref="DbType"/> into its equivalent .NET CLR Types.
    /// </summary>
    public class DbTypeToClientTypeResolver : IResolver<DbType, Type>
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
        public Type Resolve(DbType dbType)
        {
            return dbType switch
            {
                DbType.Int64 => StaticType.Int64,
                DbType.Binary or DbType.Byte => StaticType.ByteArray,
                DbType.Boolean => StaticType.Boolean,
                DbType.String or DbType.AnsiString or DbType.AnsiStringFixedLength or DbType.StringFixedLength => StaticType.String,
                DbType.Date or DbType.DateTime or DbType.DateTime2 => StaticType.DateTime,
                DbType.DateTimeOffset => StaticType.DateTimeOffset,
                DbType.Decimal => StaticType.Decimal,
                DbType.Single => StaticType.Single,
                DbType.Double => StaticType.Double,
                DbType.Int32 => StaticType.Int32,
                DbType.Int16 => StaticType.Int16,
                DbType.Time => StaticType.TimeSpan,
                DbType.Guid => StaticType.Guid,
                DbType.Object => StaticType.Object,
                //case DbType.Xml:
                //    return "XML";
                _ => typeof(string),
            };
        }
    }
}
