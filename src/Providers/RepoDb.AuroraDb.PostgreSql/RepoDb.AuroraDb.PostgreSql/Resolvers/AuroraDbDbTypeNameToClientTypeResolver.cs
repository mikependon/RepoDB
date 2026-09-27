#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NpgsqlTypes;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the Aurora PostgreSQL Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class AuroraDbDbTypeNameToClientTypeResolver : IResolver<string, Type>
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
                "character" or "char" or "character varying" or "varchar" or "citext" or "json" or "jsonb" or "jsonpath" or "ltree" or "name" or "regclass" or "regnamespace" or "regproc" or "regprocedure" or "regrole" or "string" or "text" or "xml" => typeof(String),
                "boolean" or "bool" => typeof(Boolean),
                "bit" or "bit varying" or "varbit" => typeof(System.Collections.BitArray),
                "bytea" or "bytes" => typeof(Byte[]),
                "oid" or "regtype" or "xid" or "cid" => typeof(UInt32),
                "money" => typeof(Decimal),
                "cidr" => typeof(NpgsqlCidr),
                "macaddr" or "macaddr8" => typeof(System.Net.NetworkInformation.PhysicalAddress),
                "box" => typeof(NpgsqlBox),
                "circle" => typeof(NpgsqlCircle),
                "line" => typeof(NpgsqlLine),
                "lseg" => typeof(NpgsqlLSeg),
                "path" => typeof(NpgsqlPath),
                "point" => typeof(NpgsqlPoint),
                "polygon" => typeof(NpgsqlPolygon),
                "int4range" => typeof(NpgsqlRange<Int32>),
                "int8range" => typeof(NpgsqlRange<Int64>),
                "numrange" => typeof(NpgsqlRange<Decimal>),
                "tsrange" or "tstzrange" or "daterange" => typeof(NpgsqlRange<DateTime>),
                "tsquery" => typeof(NpgsqlTsQuery),
                "tsvector" => typeof(NpgsqlTsVector),
                "hstore" => typeof(Dictionary<String, String>),
                "pg_lsn" => typeof(NpgsqlLogSequenceNumber),
                "tid" => typeof(NpgsqlTid),
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
                // The spatial (PostGIS geometry, geography) types need an Npgsql plugin (e.g. NetTopologySuite),
                // and the other extension types are reported by the schema as "USER-DEFINED".
                _ => typeof(object),
            };
        }
    }
}
