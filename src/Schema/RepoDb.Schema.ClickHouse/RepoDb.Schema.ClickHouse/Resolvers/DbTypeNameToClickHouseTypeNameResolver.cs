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
    /// into the name of its equivalent ClickHouse type (i.e.: <c>Int32</c>, <c>String</c> or <c>UUID</c>).
    /// A type name that is not known is returned as is, as it is expected to be a ClickHouse type name already.
    /// </summary>
    public class DbTypeNameToClickHouseTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent ClickHouse type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent ClickHouse type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                return null;
            }

            var name = dbTypeName.Trim();
            switch (name.ToLowerInvariant())
            {
                case "varchar":
                case "varchar2":
                case "nvarchar":
                case "nvarchar2":
                case "character varying":
                case "string":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "ntext":
                case "char":
                case "character":
                case "bpchar":
                case "nchar":
                case "xml":
                case "json":
                case "jsonb":
                case "clob":
                case "sql_variant":
                case "time":
                case "time without time zone":
                case "time with time zone":
                case "timetz":
                case "bytea":
                case "raw":
                case "image":
                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                case "binary":
                case "varbinary": return "String";
                case "bool":
                case "boolean":
                case "bit": return "Bool";
                case "int":
                case "integer":
                case "int4":
                case "mediumint": return "Int32";
                case "bigint":
                case "int8": return "Int64";
                case "smallint":
                case "int2": return "Int16";
                case "tinyint": return "Int8";
                case "decimal":
                case "numeric":
                case "number":
                case "money":
                case "smallmoney": return "Decimal";
                case "float":
                case "float8":
                case "double":
                case "double precision":
                case "binary_double": return "Float64";
                case "float4":
                case "real":
                case "binary_float": return "Float32";
                case "uuid":
                case "uniqueidentifier": return "UUID";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp":
                case "timestamp without time zone":
                case "datetimeoffset":
                case "timestamp with time zone":
                case "timestamptz": return "DateTime64";
                case "date": return "Date";
                default: return name;
            }
        }
    }
}
