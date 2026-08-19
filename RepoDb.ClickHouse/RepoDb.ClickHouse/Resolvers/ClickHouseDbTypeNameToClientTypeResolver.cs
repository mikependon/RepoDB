using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the ClickHouse Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class ClickHouseDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (as reported by <c>system.columns.type</c>).</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }

            // Unwrap Nullable(...) / LowCardinality(...) wrappers, e.g. "Nullable(LowCardinality(String))"
            var typeName = dbTypeName.Trim();
            while (true)
            {
                if (typeName.StartsWith("Nullable(", StringComparison.OrdinalIgnoreCase) && typeName.EndsWith(")"))
                {
                    typeName = typeName.Substring("Nullable(".Length, typeName.Length - "Nullable(".Length - 1).Trim();
                }
                else if (typeName.StartsWith("LowCardinality(", StringComparison.OrdinalIgnoreCase) && typeName.EndsWith(")"))
                {
                    typeName = typeName.Substring("LowCardinality(".Length, typeName.Length - "LowCardinality(".Length - 1).Trim();
                }
                else
                {
                    break;
                }
            }

            // Strip any parameterized suffix, e.g. "FixedString(16)" -> "FixedString", "DateTime64(3)" -> "DateTime64"
            var parenIndex = typeName.IndexOf('(');
            var baseName = (parenIndex >= 0 ? typeName.Substring(0, parenIndex) : typeName).Trim();

            return baseName.ToLowerInvariant() switch
            {
                "int8" => typeof(sbyte),
                "int16" => typeof(short),
                "int32" => typeof(int),
                "int64" => typeof(long),
                "int128" or "int256" => typeof(long),
                "uint8" => typeof(byte),
                "uint16" => typeof(ushort),
                "uint32" => typeof(uint),
                "uint64" => typeof(ulong),
                "uint128" or "uint256" => typeof(ulong),
                "float32" => typeof(float),
                "float64" => typeof(double),
                "bfloat16" => typeof(float),
                "bool" or "boolean" => typeof(bool),
                "string" or "fixedstring" or "json" or "object" or "enum8" or "enum16" => typeof(string),
                "date" or "date32" or "datetime" or "datetime32" or "datetime64" => typeof(DateTime),
                "time" or "time64" => typeof(TimeSpan),
                "decimal" or "decimal32" or "decimal64" or "decimal128" or "decimal256" => typeof(decimal),
                "uuid" => typeof(Guid),
                "ipv4" or "ipv6" => typeof(string),
                "none" or "nothing" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
