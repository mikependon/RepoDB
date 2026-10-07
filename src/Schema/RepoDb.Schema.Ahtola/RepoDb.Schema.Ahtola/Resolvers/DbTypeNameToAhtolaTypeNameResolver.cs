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
    /// into the name of its equivalent SQLite type (i.e.: <c>integer</c>, <c>varchar</c> or <c>uuid</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a SQLite type name already.
    /// </summary>
    public class DbTypeNameToSqliteTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent SQLite type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent SQLite type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                return null;
            }

            var name = dbTypeName.Trim().ToLowerInvariant();
            switch (name)
            {
                case "nvarchar":
                case "varchar2":
                case "nvarchar2":
                case "character varying":
                case "string": return "varchar";
                case "nchar":
                case "bpchar":
                case "character": return "char";
                case "ntext":
                case "clob":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "xml":
                case "sql_variant": return "text";
                case "bool":
                case "bit": return "boolean";
                case "int":
                case "int4": return "integer";
                case "int8": return "bigint";
                case "int2": return "smallint";
                case "float8":
                case "double precision": return "double";
                case "float4": return "real";
                case "uniqueidentifier": return "uuid";
                case "bytea":
                case "image":
                case "binary":
                case "varbinary":
                case "tinyblob":
                case "mediumblob":
                case "longblob": return "blob";
                case "money":
                case "smallmoney": return "decimal";
                case "datetime2":
                case "datetimeoffset":
                case "smalldatetime":
                case "timestamp":
                case "timestamp without time zone":
                case "timestamp with time zone":
                case "timestamptz": return "datetime";
                case "time without time zone":
                case "time with time zone":
                case "timetz": return "time";
                case "jsonb": return "json";
                default: return name;
            }
        }
    }
}
