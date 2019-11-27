using MySql.Data.MySqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the .NET CLR Types into its equivalent SQL Server <see cref="DbType"/> value.
    /// </summary>
    public class ClientTypeToMySqlDbTypeResolver : IResolver<Type, MySqlDbType?>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the target .NET CLR Type.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <returns>The equivalent <see cref="DbType"/> Type.</returns>
        public MySqlDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            /*
            Id (System.Int64)
            ColumnVarchar (System.String)
            ColumnInt (System.Int32)
            ColumnDecimal2 (System.Decimal)
            ColumnDateTime (System.DateTime)
            ColumnBlob (System.Byte[])
            ColumnBlobAsArray (System.Byte[])
            ColumnBinary (System.Byte[])
            ColumnLongBlob (System.Byte[])
            ColumnMediumBlob (System.Byte[])
            ColumnTinyBlob (System.Byte[])
            ColumnVarBinary (System.Byte[])
            ColumnDate (System.DateTime)
            ColumnDateTime2 (System.DateTime)
            ColumnTime (System.TimeSpan)
            ColumnTimeStamp (System.DateTime)
            ColumnYear (System.Int16)
            ColumnGeometry (System.Byte[])
            ColumnLineString (System.Byte[])
            ColumnMultiLineString (System.Byte[])
            ColumnMultiPoint (System.Byte[])
            ColumnMultiPolygon (System.Byte[])
            ColumnPoint (System.Byte[])
            ColumnPolygon (System.Byte[])
            ColumnBigint (System.Int64)
            ColumnDecimal (System.Decimal)
            ColumnDouble (System.Double)
            ColumnFloat (System.Single)
            ColumnInt2 (System.Int32)
            ColumnMediumInt (System.Int32)
            ColumnReal (System.Double)
            ColumnSmallInt (System.Int16)
            ColumnTinyInt (System.SByte)
            ColumnChar (System.String)
            ColumnJson (System.String)
            ColumnNChar (System.String)
            ColumnNVarChar (System.String)
            ColumnLongText (System.String)
            ColumnMediumText (System.String)
            ColumText (System.String)
            ColumnTinyText (System.String)
            ColumnBit (System.UInt64)
             */
            type = type?.GetUnderlyingType();

            if (type == typeof(long))
            {
                return MySqlDbType.Int64;
            }
            else if (type == typeof(string))
            {
                return MySqlDbType.String;
            }
            else if (type == typeof(int))
            {
                return MySqlDbType.Int32;
            }
            else if (type == typeof(uint))
            {
                return MySqlDbType.UInt32;
            }
            else if (type == typeof(short))
            {
                return MySqlDbType.Int16;
            }
            else if (type == typeof(ushort))
            {
                return MySqlDbType.UInt16;
            }
            else if (type == typeof(decimal))
            {
                return MySqlDbType.Decimal;
            }
            else if (type == typeof(double))
            {
                return MySqlDbType.Double;
            }
            else if (type == typeof(float))
            {
                return MySqlDbType.Float;
            }
            else if (type == typeof(sbyte))
            {
                return MySqlDbType.Byte;
            }
            else if (type == typeof(bool)||type == typeof(ulong))
            {
                return MySqlDbType.Bit;
            }
            else if (type == typeof(ulong))
            {
                return MySqlDbType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return MySqlDbType.DateTime;
            }
            else if (type == typeof(byte[]))
            {
                return MySqlDbType.Blob;
            }
            else if (type == typeof(TimeSpan))
            {
                return MySqlDbType.Time;
            }

            return null;
        }
    }
}
