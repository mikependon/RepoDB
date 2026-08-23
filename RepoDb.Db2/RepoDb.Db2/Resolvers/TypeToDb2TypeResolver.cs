using IBM.Data.Db2;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Type"/> into its equivalent Db2 database type.
    /// </summary>
    public class TypeToDb2TypeResolver : IResolver<Type, DB2Type>
    {
        /*
         * Taken:
         * https://www.ibm.com/docs/en/db2/11.5?topic=elements-data-types
         */

        private static readonly Dictionary<Type, DB2Type> typeMap = new()
        {
            { typeof(string), DB2Type.VarChar },
            { typeof(char), DB2Type.Char },
            { typeof(bool), DB2Type.SmallInt },
            { typeof(byte), DB2Type.Byte },
            { typeof(sbyte), DB2Type.SmallInt },
            { typeof(short), DB2Type.SmallInt },
            { typeof(ushort), DB2Type.Integer },
            { typeof(int), DB2Type.Integer },
            { typeof(uint), DB2Type.BigInt },
            { typeof(long), DB2Type.BigInt },
            { typeof(ulong), DB2Type.Decimal },
            { typeof(float), DB2Type.Real },
            { typeof(double), DB2Type.Double },
            { typeof(decimal), DB2Type.Decimal },
            { typeof(DateTime), DB2Type.Timestamp },
            { typeof(DateTimeOffset), DB2Type.TimeStampWithTimeZone },
            { typeof(TimeSpan), DB2Type.Time },
            { typeof(Guid), DB2Type.Binary },
            { typeof(byte[]), DB2Type.Blob },
        };

        /// <summary>
        /// Returns the equivalent <see cref="DB2Type"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The equivalent Db2 database type.</returns>
        public virtual DB2Type Resolve(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return typeMap.TryGetValue(underlyingType, out var db2Type) ? db2Type : DB2Type.VarChar;
        }
    }
}
