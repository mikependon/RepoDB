#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.Types;
using RepoDb.Interfaces;
using System.Numerics;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve a <see cref="decimal"/> into the <see cref="FbDecFloat"/> that is written into a Firebird <c>DECFLOAT</c> column.
    /// </summary>
    public class DecimalToFirebirdDecFloatResolver : IResolver<decimal, object>
    {
        /// <summary>
        /// Converts a <see cref="decimal"/> into an <see cref="FbDecFloat"/>, keeping its coefficient and scale.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> to convert.</param>
        /// <returns>The <see cref="FbDecFloat"/> (boxed).</returns>
        public virtual object Resolve(decimal value)
        {
            var bits = decimal.GetBits(value);
            var coefficient = (new BigInteger((uint)bits[2]) << 64) | (new BigInteger((uint)bits[1]) << 32) | new BigInteger((uint)bits[0]);
            if (bits[3] < 0)
            {
                coefficient = -coefficient;
            }
            var scale = (bits[3] >> 16) & 0xFF;
            return new FbDecFloat(coefficient, -scale);
        }
    }
}
