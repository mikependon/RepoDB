#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the Aurora PostgreSQL Database Types into its <see cref="AuroraDbType"/>.
    /// </summary>
    public class AuroraDbDbTypeNameToAuroraDbTypeResolver : IResolver<string, AuroraDbType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="AuroraDbType"/> of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent <see cref="AuroraDbType"/>.</returns>
        public virtual AuroraDbType? Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                throw new ArgumentNullException(nameof(dbTypeName), "The database type name must not be a null or whitespace.");
            }

            // Try parse
            if (Enum.TryParse<AuroraDbType>(dbTypeName, true, out var result))
            {
                return result;
            }

            // Range types whose names differ from the enumeration members
            if ("tsrange".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return AuroraDbType.TimestampRange;
            }
            if ("tstzrange".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return AuroraDbType.TimestampTzRange;
            }

            // User-Defined - no "Unknown" member exists on AuroraDbType.
            if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Covert to .NET CLR Type
            var clientTypeResolver = new AuroraDbDbTypeNameToClientTypeResolver()
                .Resolve(dbTypeName);

            // Try resolve
            try
            {
                return new ClientTypeToAuroraDbTypeResolver().Resolve(clientTypeResolver);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
