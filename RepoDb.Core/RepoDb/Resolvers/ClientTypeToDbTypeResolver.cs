using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Types into its equivalent SQL Server <see cref="DbType"/> value.
    /// </summary>
    public class ClientTypeToDbTypeResolver : IResolver<Type, DbType?>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the target .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The equivalent <see cref="DbType"/> Type.</returns>
        public DbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }
            if (type.IsEnum)
            {
                return null;
            }

            type = type.GetUnderlyingType();

            if (type == StaticType.Int64)
            {
                return DbType.Int64;
            }
            else if (type == StaticType.Byte)
            {
                return DbType.Byte;
            }
            else if (type == StaticType.ByteArray)
            {
                return DbType.Binary;
            }
            else if (type == StaticType.Boolean)
            {
                return DbType.Boolean;
            }
            else if (type == StaticType.String)
            {
                return DbType.String;
            }
            else if (type == StaticType.DateTime)
            {
                return DbType.DateTime;
            }
            else if (type == StaticType.DateTimeOffset)
            {
                return DbType.DateTimeOffset;
            }
            else if (type == StaticType.Decimal)
            {
                return DbType.Decimal;
            }
            else if (type == StaticType.Double)
            {
                return DbType.Double;
            }
            else if (type == StaticType.Int32)
            {
                return DbType.Int32;
            }
            else if (type == StaticType.Single)
            {
                return DbType.Single;
            }
            else if (type == StaticType.Int16)
            {
                return DbType.Int16;
            }
            else if (type == StaticType.SqlVariant)
            {
                return DbType.Object;
            }
            // Object must be defaulted to String, defaulted by .NET for DbType
            else if (type == StaticType.Object)
            {
                //return DbType.Object;
                return DbType.String;
            }
            else if (type == StaticType.CharArray)
            {
                return DbType.String;
            }
            else if (type == StaticType.TimeSpan)
            {
                return DbType.Time;
            }
            else if (type == StaticType.Byte)
            {
                return DbType.Byte;
            }
            else if (type == StaticType.Guid)
            {
                return DbType.Guid;
            }
            // XML must be defaulted to String, defaulted by .NET for DbType
            /*else if (type == typeof(Xml))
            {
                return DbType.Xml;
            }*/

            return null;
        }
    }
}
