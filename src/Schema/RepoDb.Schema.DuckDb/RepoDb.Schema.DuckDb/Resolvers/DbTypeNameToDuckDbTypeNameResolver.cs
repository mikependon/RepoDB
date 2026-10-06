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
    /// into the name of its equivalent DuckDb type (i.e.: <c>integer</c>, <c>varchar</c> or <c>uuid</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a DuckDb type name already.
    /// </summary>
    public class DbTypeNameToDuckDbTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent DuckDb type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent DuckDb type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
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
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "text":
                case "ntext":
                case "xml":
                case "clob":
                case "sql_variant":
                case "character":
                case "bpchar":
                case "nchar":
                case "char": return "varchar";
                case "jsonb": return "json";
                case "bool":
                case "bit": return "boolean";
                case "int":
                case "int4":
                case "mediumint": return "integer";
                case "int8": return "bigint";
                case "int2": return "smallint";
                case "numeric":
                case "number":
                case "money":
                case "smallmoney": return "decimal";
                case "float8":
                case "binary_double":
                case "double precision": return "double";
                case "float4":
                case "real":
                case "binary_float": return "float";
                case "uniqueidentifier": return "uuid";
                case "bytea":
                case "raw":
                case "image":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                case "binary":
                case "varbinary": return "blob";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp without time zone": return "timestamp";
                case "datetimeoffset":
                case "timestamptz":
                case "timestamp with time zone": return "timestamp with time zone";
                case "time without time zone": return "time";
                case "timetz":
                case "time with time zone": return "time with time zone";
                default: return name;
            }
        }
    }
}
