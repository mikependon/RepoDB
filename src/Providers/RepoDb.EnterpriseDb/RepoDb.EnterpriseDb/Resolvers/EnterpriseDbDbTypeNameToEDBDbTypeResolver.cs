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
    /// A class that is being used to resolve the EnterpriseDb Database Types into its <see cref="EDBType"/>.
    /// </summary>
    public class EnterpriseDbDbTypeNameToEDBDbTypeResolver : IResolver<string, EDBType?>
    {
        /// <summary>
        /// Returns the equivalent <see cref="EDBType"/> of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent <see cref="EDBType"/>.</returns>
        public virtual EDBType? Resolve(string dbTypeName)
        {
            if (string.IsNullOrWhiteSpace(dbTypeName))
            {
                throw new NullReferenceException("The database type name must not be a null or whitespace.");
            }

            // Try parse
            if (Enum.TryParse<EDBType>(dbTypeName, true, out var result))
            {
                return result;
            }

            // User-Defined - no "Unknown" member exists on EDBType.
            if ("USER-DEFINED".Equals(dbTypeName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Covert to .NET CLR Type
            var clientTypeResolver = new EnterpriseDbDbTypeNameToClientTypeResolver()
                .Resolve(dbTypeName);

            // Try resolve
            try
            {
                return new ClientTypeToEDBDbTypeResolver().Resolve(clientTypeResolver);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
