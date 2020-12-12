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
            switch (dbType)
            {
                case DbType.Int64:
                    return StaticType.Int64;
                case DbType.Binary:
                case DbType.Byte:
                    return StaticType.ByteArray;
                case DbType.Boolean:
                    return StaticType.Boolean;
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return StaticType.String;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                    return StaticType.DateTime;
                case DbType.DateTimeOffset:
                    return StaticType.DateTimeOffset;
                case DbType.Decimal:
                    return StaticType.Decimal;
                case DbType.Single:
                    return StaticType.Single;
                case DbType.Double:
                    return StaticType.Double;
                case DbType.Int32:
                    return StaticType.Int32;
                case DbType.Int16:
                    return StaticType.Int16;
                case DbType.Time:
                    return StaticType.TimeSpan;
                case DbType.Guid:
                    return StaticType.Guid;
                case DbType.Object:
                    return StaticType.Object;
                //case DbType.Xml:
                //    return "XML";
                default:
                    return typeof(string);
            }
        }
    }
}
