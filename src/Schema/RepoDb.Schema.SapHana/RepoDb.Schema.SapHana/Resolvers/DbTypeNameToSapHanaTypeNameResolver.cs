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
    /// into the name of its equivalent SapHana type (i.e.: <c>integer</c>, <c>nvarchar</c> or <c>varbinary(16)</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a SapHana type name already.
    /// </summary>
    public class DbTypeNameToSapHanaTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent SapHana type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent SapHana type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
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
                case "character varying":
                case "string":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "character":
                case "bpchar":
                case "nchar": return "nvarchar";
                case "ntext":
                case "xml":
                case "json":
                case "jsonb":
                case "clob":
                case "sql_variant": return "nclob";
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
                case "float":
                case "binary_double":
                case "double precision": return "double";
                case "float4":
                case "binary_float": return "real";
                case "uuid":
                case "uniqueidentifier": return "varbinary(16)";
                case "bytea":
                case "raw":
                case "image":
                case "tinyblob":
                case "mediumblob":
                case "longblob": return "blob";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "datetimeoffset":
                case "timestamp without time zone":
                case "timestamp with time zone":
                case "timestamptz": return "timestamp";
                case "time without time zone":
                case "time with time zone":
                case "timetz": return "time";
                default: return name;
            }
        }
    }
}
