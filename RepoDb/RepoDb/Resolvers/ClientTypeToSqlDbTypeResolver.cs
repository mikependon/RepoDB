using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class used to resolve the .NET CLR Types into SQL Database Types.
    /// </summary>
    internal class ClientTypeToSqlDbTypeResolver : IResolver<Type, DbType>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <returns>The equivalent <see cref="DbType"/> Type.</returns>
        public DbType Resolve(Type type)
        {
            if (type == typeof(long))
            {
                return DbType.Int64;
            }
            else if (type == typeof(byte[]))
            {
                return DbType.Binary;
            }
            else if (type == typeof(bool))
            {
                return DbType.Boolean;
            }
            else if (type == typeof(string))
            {
                return DbType.String;
            }
            else if (type == typeof(DateTime))
            {
                return DbType.DateTime;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return DbType.DateTimeOffset;
            }
            else if (type == typeof(decimal))
            {
                return DbType.Decimal;
            }
            else if (type == typeof(double))
            {
                return DbType.Double;
            }
            else if (type == typeof(int))
            {
                return DbType.Int32;
            }
            else if (type == typeof(float))
            {
                return DbType.Single;
            }
            else if (type == typeof(short))
            {
                return DbType.Int16;
            }
            /*else if (type == typeof(object))
            {
                return DbType.Object;
            }*/
            else if (type == typeof(TimeSpan))
            {
                return DbType.Time;
            }
            else if (type == typeof(byte))
            {
                return DbType.Byte;
            }
            else if (type == typeof(Guid))
            {
                return DbType.Guid;
            }
            /*else if (type == typeof(Xml))
            {
                return DbType.Xml;
            }*/
            return DbType.String;
        }
    }
}
