#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the database type name of any database engine (i.e.: <c>int4</c>, <c>text</c> or <c>uuid</c>)
    /// into the name of its equivalent Firebird type (i.e.: <c>integer</c>, <c>varchar</c> or <c>binary(16)</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a Firebird type name already.
    /// </summary>
    public class DbTypeNameToFirebirdTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent Firebird type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent Firebird type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                return null;
            }

            var name = dbTypeName.Trim().ToLowerInvariant();
            switch (name)
            {
                case "varchar2":
                case "nvarchar2":
                case "nvarchar":
                case "character varying":
                case "string":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext": return "varchar";
                case "ntext":
                case "xml":
                case "json":
                case "jsonb":
                case "clob":
                case "sql_variant": return "blob_text";
                case "character":
                case "bpchar":
                case "nchar": return "char";
                case "bool":
                case "bit": return "boolean";
                case "int":
                case "int4":
                case "mediumint": return "integer";
                case "int8": return "bigint";
                case "int2":
                case "tinyint": return "smallint";
                case "number":
                case "money":
                case "smallmoney": return "decimal";
                case "float8":
                case "float":
                case "double":
                case "binary_double": return "double precision";
                case "float4":
                case "real":
                case "binary_float": return "float";
                case "uuid":
                case "uniqueidentifier": return "binary(16)";
                case "bytea":
                case "raw":
                case "image":
                case "tinyblob":
                case "mediumblob":
                case "longblob": return "blob_binary";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp without time zone": return "timestamp";
                case "datetimeoffset":
                case "timestamp with time zone":
                case "timestamptz": return "timestamp_tz";
                case "time without time zone": return "time";
                case "time with time zone":
                case "timetz": return "time_tz";
                default: return name;
            }
        }
    }
}
