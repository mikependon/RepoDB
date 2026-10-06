#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the database type names (of CockroachDB or of another database) into the equivalent CockroachDB type names.
    /// </summary>
    public class DbTypeNameToCockroachDbTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Returns the equivalent CockroachDB type name of the database type name.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent CockroachDB type name (the given name, in lower-case, if it has no known equivalent), or <c>null</c> if the name is blank.</returns>
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
                case "character varying":
                case "string": return "varchar";
                case "nchar":
                case "character": return "char";
                case "ntext":
                case "clob":
                case "longtext": return "text";
                case "bit": return "boolean";
                case "int":
                case "int4": return "integer";
                case "int8": return "bigint";
                case "tinyint":
                case "int2": return "smallint";
                case "float":
                case "float8": return "double precision";
                case "float4": return "real";
                case "decimal":
                case "money":
                case "smallmoney": return "numeric";
                case "uniqueidentifier": return "uuid";
                case "binary":
                case "varbinary":
                case "image":
                case "blob": return "bytea";
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "timestamp without time zone": return "timestamp";
                case "datetimeoffset":
                case "timestamp with time zone": return "timestamptz";
                case "time without time zone": return "time";
                case "time with time zone": return "timetz";
                case "sql_variant": return "text";
                default: return name;
            }
        }
    }
}
