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
    /// A class that is being used to resolve the CockroachDB Database Types into its <see cref="CockroachDbType"/>.
    /// </summary>
    public class CockroachDbDbTypeNameToCockroachDbTypeResolver : IResolver<string, CockroachDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="CockroachDbType"/> of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent <see cref="CockroachDbType"/>.</returns>
        public virtual CockroachDbType? Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                throw new ArgumentNullException(nameof(dbTypeName), "The database type name must not be a null or whitespace.");
            }

            // Try parse
            if (Enum.TryParse<CockroachDbType>(dbTypeName, true, out var result))
            {
                return result;
            }

            // User-Defined - no "Unknown" member exists on CockroachDbType.
            if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Covert to .NET CLR Type
            var clientTypeResolver = new CockroachDbDbTypeNameToClientTypeResolver()
                .Resolve(dbTypeName);

            // Try resolve
            try
            {
                return new ClientTypeToCockroachDbTypeResolver().Resolve(clientTypeResolver);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
