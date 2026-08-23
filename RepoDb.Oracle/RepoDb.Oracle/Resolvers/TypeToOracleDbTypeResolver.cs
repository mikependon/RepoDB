using Oracle.ManagedDataAccess.Client;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Type"/> into its equivalent Oracle database type.
    /// </summary>
    public class TypeToOracleDbTypeResolver : IResolver<Type, OracleDbType>
    {
        /*
         * Taken:
         * https://docs.oracle.com/en/database/oracle/oracle-database/23/sqlrf/Data-Types.html
         */

        private static readonly Dictionary<Type, OracleDbType> typeMap = new()
        {
            { typeof(string), OracleDbType.NVarchar2 },
            { typeof(char), OracleDbType.NChar },
            { typeof(bool), OracleDbType.Int32 },
            { typeof(byte), OracleDbType.Byte },
            { typeof(sbyte), OracleDbType.Int16 },
            { typeof(short), OracleDbType.Int16 },
            { typeof(ushort), OracleDbType.Int32 },
            { typeof(int), OracleDbType.Int32 },
            { typeof(uint), OracleDbType.Int64 },
            { typeof(long), OracleDbType.Int64 },
            { typeof(ulong), OracleDbType.Decimal },
            { typeof(float), OracleDbType.BinaryFloat },
            { typeof(double), OracleDbType.BinaryDouble },
            { typeof(decimal), OracleDbType.Decimal },
            { typeof(DateTime), OracleDbType.TimeStamp },
            { typeof(DateTimeOffset), OracleDbType.TimeStampTZ },
            { typeof(TimeSpan), OracleDbType.IntervalDS },
            { typeof(Guid), OracleDbType.Raw },
            { typeof(byte[]), OracleDbType.Blob },
        };

        /// <summary>
        /// Returns the equivalent <see cref="OracleDbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The equivalent Oracle database type.</returns>
        public virtual OracleDbType Resolve(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return typeMap.TryGetValue(underlyingType, out var oracleDbType) ? oracleDbType : OracleDbType.NVarchar2;
        }
    }
}
