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
    /// into the name of its equivalent SQL Server type (i.e.: <c>int</c>, <c>nvarchar</c> or <c>uniqueidentifier</c>).
    /// A type name that is not known is returned as is (in lower case), as it is expected to be a SQL Server type name already.
    /// </summary>
    public class DbTypeNameToSqlServerTypeNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Resolves a database type name into the name of its equivalent SQL Server type.
        /// </summary>
        /// <param name="dbTypeName">The database type name to be resolved.</param>
        /// <returns>The name of the equivalent SQL Server type, or <c>null</c> if the <paramref name="dbTypeName"/> is empty.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                return null;
            }

            var name = dbTypeName.Trim().ToLowerInvariant();
            switch (name)
            {
                case "text":
                case "clob":
                case "longtext":
                case "string": return "nvarchar";
                case "bool":
                case "boolean": return "bit";
                case "integer":
                case "int4": return "int";
                case "int8": return "bigint";
                case "int2": return "smallint";
                case "float8":
                case "double precision": return "float";
                case "float4": return "real";
                case "uuid": return "uniqueidentifier";
                case "bytea":
                case "blob": return "varbinary";
                case "character varying": return "varchar";
                case "character": return "char";
                default: return name;
            }
        }
    }
}
