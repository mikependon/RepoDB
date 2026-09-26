#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Type into its equivalent <see cref="CockroachDbType"/>.
    /// </summary>
    public class ClientTypeToCockroachDbTypeResolver : IResolver<Type, CockroachDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="CockroachDbType"/> based from the .NET CLR Type.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <returns>The equivalent <see cref="CockroachDbType"/>.</returns>
        public virtual CockroachDbType? Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "The type must not be null.");
            }

            if (type == typeof(Boolean))
            {
                return CockroachDbType.Boolean;
            }
            else if (type == typeof(Byte[]))
            {
                return CockroachDbType.Bytea;
            }
            else if (type == typeof(Char))
            {
                return CockroachDbType.Char;
            }
            else if (type == typeof(System.Collections.BitArray))
            {
                return CockroachDbType.Bit;
            }
            else if (type == typeof(DateTime))
            {
                return CockroachDbType.Timestamp;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return CockroachDbType.TimestampTz;
            }
            #if NET6_0_OR_GREATER
            else if (type == typeof(DateOnly))
            {
                return CockroachDbType.Date;
            }
            else if (type == typeof(TimeOnly))
            {
                return CockroachDbType.Time;
            }
            #endif
            else if (type == typeof(Decimal))
            {
                return CockroachDbType.Decimal;
            }
            else if (type == typeof(Double))
            {
                return CockroachDbType.Double;
            }
            else if (type == typeof(Guid))
            {
                return CockroachDbType.Uuid;
            }
            else if (type == typeof(Int16))
            {
                return CockroachDbType.SmallInt;
            }
            else if (type == typeof(Int32))
            {
                return CockroachDbType.Integer;
            }
            else if (type == typeof(Int64))
            {
                return CockroachDbType.BigInt;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return CockroachDbType.Inet;
            }
            else if (type == typeof(Single))
            {
                return CockroachDbType.Real;
            }
            else if (type == typeof(String))
            {
                return CockroachDbType.Text;
            }
            else if (type == typeof(TimeSpan))
            {
                return CockroachDbType.Interval;
            }

            // No equivalent CLR representation exists for the remaining CockroachDB types
            // (arrays, spatial, ltree, ...).
            throw new InvalidOperationException($"The type '{type.FullName}' could not be resolved to '{typeof(CockroachDbType).FullName}'.");
        }
    }
}
