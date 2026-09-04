#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="EDBType"/>.
    /// </summary>
    public class ClientTypeToEDBDbTypeResolver : IResolver<Type, EDBType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="EDBType"/> based from the .NET CLR Type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The equivalent <see cref="EDBType"/>.</returns>
        public virtual EDBType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            if (type == typeof(Boolean))
            {
                return EDBType.Boolean;
            }
            else if (type == typeof(Byte[]))
            {
                return EDBType.Bytea;
            }
            else if (type == typeof(Char))
            {
                return EDBType.Char;
            }
            else if (type == typeof(System.Collections.BitArray))
            {
                return EDBType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return EDBType.Timestamp;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return EDBType.TimestampTz;
            }
            #if NET6_0_OR_GREATER
            else if (type == typeof(DateOnly))
            {
                return EDBType.Date;
            }
            else if (type == typeof(TimeOnly))
            {
                return EDBType.Time;
            }
            #endif
            else if (type == typeof(Decimal))
            {
                return EDBType.Money;
            }
            else if (type == typeof(Double))
            {
                return EDBType.Double;
            }
            else if (type == typeof(Guid))
            {
                return EDBType.Uuid;
            }
            else if (type == typeof(Int16))
            {
                return EDBType.SmallInt;
            }
            else if (type == typeof(Int32))
            {
                return EDBType.Integer;
            }
            else if (type == typeof(Int64))
            {
                return EDBType.BigInt;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return EDBType.Inet;
            }
            else if (type == typeof(System.Net.NetworkInformation.PhysicalAddress))
            {
                return EDBType.MacAddr;
            }
            else if (type == typeof(Single))
            {
                return EDBType.Real;
            }
            else if (type == typeof(String))
            {
                return EDBType.Char;
            }
            else if (type == typeof(TimeSpan))
            {
                return EDBType.Interval;
            }
            else if (type == typeof(ValueTuple<System.Net.IPAddress, Int32>))
            {
                return EDBType.Cidr;
            }

            // No equivalent CLR representation exists for the remaining PostgreSQL-specific types
            // (geometric types, ranges, pg_lsn, tid, cid, arrays, ...) on the Npgsql-backed
            // RepoDb.Connector.EnterpriseDb driver, unlike the official EnterpriseDB.EDBClient driver.
            throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(EDBType).FullName}'.");
        }
    }
}
