using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class SqlDbTypeToClientTypeResolver : IResolver<DbType, Type>
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
                    return typeof(long);
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.String:
                    return typeof(string);
                case DbType.Date:
                    return typeof(DateTime);
                case DbType.DateTime:
                    return typeof(DateTime);
                case DbType.DateTime2:
                    return typeof(DateTime);
                case DbType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                case DbType.Decimal:
                    return typeof(decimal);
                case DbType.Single:
                    return typeof(Single);
                case DbType.Double:
                    return typeof(double);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Time:
                    return typeof(TimeSpan);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.AnsiString:
                    return typeof(string);
                case DbType.AnsiStringFixedLength:
                    return typeof(string);
                case DbType.StringFixedLength:
                    return typeof(string);
                case DbType.Object:
                    return typeof(object);
                //case DbType.Xml:
                //    return "XML";
                default:
                    return typeof(string);
            }
        }
    }
}
