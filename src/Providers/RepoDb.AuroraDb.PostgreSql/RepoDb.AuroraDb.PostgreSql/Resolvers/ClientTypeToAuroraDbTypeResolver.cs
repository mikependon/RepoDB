#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NpgsqlTypes;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="AuroraDbType"/>.
    /// </summary>
    public class ClientTypeToAuroraDbTypeResolver : IResolver<Type, AuroraDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="AuroraDbType"/> based from the .NET CLR Type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The equivalent <see cref="AuroraDbType"/>.</returns>
        public virtual AuroraDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "The type must not be null.");
            }

            if (type == typeof(NpgsqlBox))
            {
                return AuroraDbType.Box;
            }
            else if (type == typeof(NpgsqlCircle))
            {
                return AuroraDbType.Circle;
            }
            else if (type == typeof(NpgsqlLine))
            {
                return AuroraDbType.Line;
            }
            else if (type == typeof(NpgsqlLSeg))
            {
                return AuroraDbType.LSeg;
            }
            else if (type == typeof(NpgsqlPath))
            {
                return AuroraDbType.Path;
            }
            else if (type == typeof(NpgsqlPoint))
            {
                return AuroraDbType.Point;
            }
            else if (type == typeof(NpgsqlPolygon))
            {
                return AuroraDbType.Polygon;
            }
            else if (type == typeof(NpgsqlCidr))
            {
                return AuroraDbType.Cidr;
            }
            else if (type == typeof(NpgsqlRange<Int32>))
            {
                return AuroraDbType.IntegerRange;
            }
            else if (type == typeof(NpgsqlRange<Int64>))
            {
                return AuroraDbType.BigIntRange;
            }
            else if (type == typeof(NpgsqlRange<Decimal>))
            {
                return AuroraDbType.NumericRange;
            }
            #if NET6_0_OR_GREATER
            else if (type == typeof(NpgsqlRange<DateOnly>))
            {
                return AuroraDbType.DateRange;
            }
            #endif
            else if (type == typeof(NpgsqlTsQuery))
            {
                return AuroraDbType.TsQuery;
            }
            else if (type == typeof(NpgsqlTsVector))
            {
                return AuroraDbType.TsVector;
            }
            else if (type == typeof(System.Net.NetworkInformation.PhysicalAddress))
            {
                return AuroraDbType.MacAddr;
            }
            else if (type == typeof(Dictionary<String, String>))
            {
                return AuroraDbType.Hstore;
            }
            else if (type == typeof(Boolean))
            {
                return AuroraDbType.Boolean;
            }
            else if (type == typeof(Byte[]))
            {
                return AuroraDbType.Bytea;
            }
            else if (type == typeof(Char))
            {
                return AuroraDbType.Char;
            }
            else if (type == typeof(System.Collections.BitArray))
            {
                return AuroraDbType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return AuroraDbType.Timestamp;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return AuroraDbType.TimestampTz;
            }
            #if NET6_0_OR_GREATER
            else if (type == typeof(DateOnly))
            {
                return AuroraDbType.Date;
            }
            else if (type == typeof(TimeOnly))
            {
                return AuroraDbType.Time;
            }
            #endif
            else if (type == typeof(Decimal))
            {
                return AuroraDbType.Decimal;
            }
            else if (type == typeof(Double))
            {
                return AuroraDbType.Double;
            }
            else if (type == typeof(Guid))
            {
                return AuroraDbType.Uuid;
            }
            else if (type == typeof(Int16))
            {
                return AuroraDbType.SmallInt;
            }
            else if (type == typeof(Int32))
            {
                return AuroraDbType.Integer;
            }
            else if (type == typeof(Int64))
            {
                return AuroraDbType.BigInt;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return AuroraDbType.Inet;
            }
            else if (type == typeof(Single))
            {
                return AuroraDbType.Real;
            }
            else if (type == typeof(String))
            {
                return AuroraDbType.Text;
            }
            else if (type == typeof(TimeSpan))
            {
                return AuroraDbType.Interval;
            }

            // No single equivalent exists for the remaining Aurora PostgreSQL types: the arrays, the
            // spatial (PostGIS) and the ltree types, and NpgsqlRange<DateTime> (which can be a TSRANGE,
            // a TSTZRANGE or a DATERANGE).
            throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(AuroraDbType).FullName}'.");
        }
    }
}
