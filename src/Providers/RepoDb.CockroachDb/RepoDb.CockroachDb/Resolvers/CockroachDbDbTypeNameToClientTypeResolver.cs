#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the CockroachDB Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class CockroachDbDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new ArgumentNullException(nameof(dbTypeName), "The DB Type name must not be null.");
            }

            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" or "int8" => typeof(Int64),
                "\"char\"" => typeof(Char),
                "array" => typeof(Array),
                "character" or "char" or "character varying" or "varchar" or "citext" or "json" or "jsonb" or "ltree" or "name" or "regclass" or "regnamespace" or "regproc" or "regprocedure" or "regrole" or "string" or "text" => typeof(String),
                "boolean" or "bool" => typeof(Boolean),
                "bit" or "bit varying" or "varbit" => typeof(System.Collections.BitArray),
                "bytea" or "bytes" => typeof(Byte[]),
                "oid" or "regtype" => typeof(UInt32),
                "date"
#if NET6_0_OR_GREATER
                    => typeof(DateOnly),
#else
                    or 
#endif
                "timestamp without time zone" or "timestamp" => typeof(DateTime),
                "timestamp with time zone" or "timestamptz" => typeof(DateTimeOffset),
                "double precision" or "float8" or "float" => typeof(Double),
                "inet" => typeof(System.Net.IPAddress),
                "integer" or "int4" => typeof(Int32),
                "time without time zone" or "time"
#if NET6_0_OR_GREATER
                    => typeof(TimeOnly),
#else
                    or 
#endif
                "interval" => typeof(TimeSpan),
                "numeric" or "decimal" => typeof(Decimal),
                "real" or "float4" => typeof(Single),
                "smallint" or "int2" => typeof(Int16),
                "timetz" or "time with time zone" => typeof(DateTimeOffset),
                "uuid" => typeof(Guid),
                // The spatial (geometry, geography) and the full-text search (tsquery, tsvector) types
                // have no direct CLR representation.
                _ => typeof(object),
            };
        }
    }
}
