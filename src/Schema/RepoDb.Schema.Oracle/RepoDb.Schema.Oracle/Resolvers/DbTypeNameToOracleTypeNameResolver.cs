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
    /// into the name of its equivalent Oracle type (i.e.: <c>number(10)</c>, <c>varchar2</c> or <c>raw(16)</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a Oracle type name already.
    /// </summary>
    public class DbTypeNameToOracleTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent Oracle type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent Oracle type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                return null;
            }

            var name = dbTypeName.Trim().ToLowerInvariant();
            switch (name)
            {
                case "varchar":
                case "character varying":
                case "string":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext": return "varchar2";
                case "nvarchar":
                case "ntext": return "nvarchar2";
                case "character":
                case "bpchar": return "char";
                case "xml":
                case "sql_variant": return "clob";
                case "bool":
                case "boolean":
                case "bit": return "number(1)";
                case "int":
                case "int4":
                case "integer":
                case "mediumint": return "number(10)";
                case "int8":
                case "bigint": return "number(19)";
                case "int2":
                case "smallint": return "number(5)";
                case "tinyint": return "number(3)";
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney": return "number";
                case "float8":
                case "float":
                case "double":
                case "double precision": return "binary_double";
                case "float4":
                case "real": return "binary_float";
                case "uuid":
                case "uniqueidentifier": return "raw(16)";
                case "bytea":
                case "binary":
                case "varbinary":
                case "image":
                case "tinyblob":
                case "mediumblob":
                case "longblob": return "raw";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp without time zone": return "timestamp";
                case "datetimeoffset":
                case "timestamp with time zone":
                case "timestamptz": return "timestamp with time zone";
                case "time":
                case "time without time zone":
                case "time with time zone":
                case "timetz": return "interval day to second";
                case "jsonb": return "json";
                default: return name;
            }
        }
    }
}
