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
    /// into the name of its equivalent Vertica type (i.e.: <c>int</c>, <c>varchar</c> or <c>uuid</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a Vertica type name already.
    /// </summary>
    public class DbTypeNameToVerticaTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent Vertica type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent Vertica type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
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
                case "text": return "varchar";
                case "ntext":
                case "xml":
                case "json":
                case "jsonb":
                case "clob":
                case "sql_variant": return "long varchar";
                case "character":
                case "bpchar":
                case "nchar": return "char";
                case "bool":
                case "bit": return "boolean";
                case "integer":
                case "int4":
                case "mediumint":
                case "int8":
                case "bigint":
                case "int2":
                case "smallint":
                case "tinyint": return "int";
                case "decimal":
                case "number":
                case "money":
                case "smallmoney": return "numeric";
                case "float8":
                case "double":
                case "double precision":
                case "binary_double":
                case "float4":
                case "real":
                case "binary_float": return "float";
                case "uniqueidentifier": return "uuid";
                case "bytea":
                case "raw":
                case "image":
                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob": return "long varbinary";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp without time zone": return "timestamp";
                case "datetimeoffset":
                case "timestamptz":
                case "timestamp with time zone": return "timestamp with timezone";
                case "time without time zone": return "time";
                case "timetz":
                case "time with time zone": return "time with timezone";
                default: return name;
            }
        }
    }
}
