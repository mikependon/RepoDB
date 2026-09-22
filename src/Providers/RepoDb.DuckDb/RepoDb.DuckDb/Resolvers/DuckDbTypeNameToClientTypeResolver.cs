#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the DuckDB Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class DuckDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (DuckDB's base type name, without any parenthesized precision/scale suffix).</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new ArgumentNullException(nameof(dbTypeName), "The DB Type name must not be null.");
            }
            /*
            Id (System.Int64)
            ColumnBoolean (System.Boolean)
            ColumnTinyInt (System.SByte)
            ColumnSmallInt (System.Int16)
            ColumnInteger (System.Int32)
            ColumnBigInt (System.Int64)
            ColumnHugeInt (System.Decimal)
            ColumnUTinyInt (System.Byte)
            ColumnUSmallInt (System.UInt16)
            ColumnUInteger (System.UInt32)
            ColumnUBigInt (System.UInt64)
            ColumnFloat (System.Single)
            ColumnDouble (System.Double)
            ColumnDecimal (System.Decimal)
            ColumnVarchar (System.String)
            ColumnBlob (System.Byte[])
            ColumnDate (System.DateTime)
            ColumnTime (System.TimeSpan)
            ColumnTimestamp (System.DateTime)
            ColumnTimestampTz (System.DateTimeOffset)
            ColumnUuid (System.Guid)
            ColumnJson (System.String)
            ColumnBit (System.String)
            ColumnInterval (System.TimeSpan)
             */
            return dbTypeName.ToLowerInvariant() switch
            {
                "boolean" or "bool" or "logical" => typeof(bool),
                "tinyint" or "int1" => typeof(sbyte),
                "smallint" or "int2" or "short" => typeof(short),
                "integer" or "int4" or "int" or "signed" => typeof(int),
                "bigint" or "int8" or "long" => typeof(long),
                "hugeint" => typeof(decimal),
                "utinyint" => typeof(byte),
                "usmallint" => typeof(ushort),
                "uinteger" => typeof(uint),
                "ubigint" => typeof(ulong),
                "uhugeint" => typeof(decimal),
                "float" or "float4" or "real" => typeof(float),
                "double" or "float8" => typeof(double),
                "decimal" or "numeric" => typeof(decimal),
                "varchar" or "char" or "bpchar" or "text" or "string" or "json" => typeof(string),
                "blob" or "bytea" or "binary" or "varbinary" => typeof(byte[]),
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "timestamp" or "datetime" or "timestamp_s" or "timestamp_ms" or "timestamp_ns" => typeof(DateTime),
                "timestamp with time zone" or "timestamptz" => typeof(DateTimeOffset),
                "uuid" => typeof(Guid),
                "bit" or "bitstring" => typeof(string),
                "interval" => typeof(TimeSpan),
                "none" => typeof(object),
                _ => typeof(object),
            };
        }
    }
}
